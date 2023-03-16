import { ClientPromise, ClientPromiseManager } from "./ClientPromiseManager";
import { ClientServerConst } from "./ClientServerConst";
import { pb } from "./projectbase.service";
import { NgxArchError } from "./projectbase.type";
import { RcResult } from "./projectbase.type";
import { CallActionConfig, NameValueObj } from "./projectbase.type";
import { pbConfig } from "./ProjectBaseConfig";
import { util } from "./UtilHelper";
import { ConnectionBuilder, HubConnectionState,HubConnection, SignalRService } from "./Lyuan-SignalR/SignalRService";
import { pbErrorHandler } from "./PBErrorHandler";

export interface ServerCallbackParam{
    ClientDispatchKey: any;
    Data?: any;
    Reason?: any;
}

export class SocketService {

    static AutoReconnectInterval = 5000;
    static DefaultProcessing = false;

    private singleConnection: HubConnection;
    private state: number; //Lyuan-SignalR未提供state，此处自己维护。暂未使用。
    private closedByStop: boolean = false;

	ClientPromiseManager:ClientPromiseManager=null;
	
    calleeMap: { [name: string]: ((...args: any[]) => void) } = {};
    
    serverCallbackMap: {
        [dispatchKey: string]: {
            onServerReturn?: (value: any) => void,
            onServerError?: (reason?: any) => void
        }
    } = {};

    serverCallback_onServerReturn?: (value: any) => void;//不区分ClientDispatchKey的处理程序
    serverCallback_onServerError?: (reason?: any) => void;//不区分ClientDispatchKey的处理程序

    constructor(useClientPromiseManager: boolean, autoReconnectInterval?:number){
		if(useClientPromiseManager){
			this.ClientPromiseManager=new ClientPromiseManager();
        }
        if (autoReconnectInterval) {
            SocketService.AutoReconnectInterval = autoReconnectInterval;
        }
    }

    RegisterServerCallbacks(dispatchKey: string | number, onServerReturn?: (value: any) => void, onServerError?: (reason?: any) => void) {
        if (!dispatchKey) {
            this.serverCallback_onServerReturn = onServerReturn;
            this.serverCallback_onServerError = onServerError;
        } else {
            this.serverCallbackMap[dispatchKey] = {
                onServerReturn,
                onServerError
            }
        }
    }

    OnServerReturn(param: ServerCallbackParam) {
        if (!param.ClientDispatchKey && this.serverCallback_onServerReturn) {
            this.serverCallback_onServerReturn(param);
        } else if (param.ClientDispatchKey && this.serverCallbackMap[param.ClientDispatchKey] && this.serverCallbackMap[param.ClientDispatchKey].onServerReturn) {
            this.serverCallbackMap[param.ClientDispatchKey].onServerReturn(param.Data);
        } else if (this.serverCallback_onServerReturn){
            this.serverCallback_onServerReturn(param.Data||param);
        } else {
            console.info('未注册回调方法onServerReturn. 服务器返回数据=' + param);
        }
    }
    OnServerError(param: ServerCallbackParam) {
        if (!param.ClientDispatchKey && this.serverCallback_onServerError) {
            this.serverCallback_onServerError(param);
        } else if (param.ClientDispatchKey && this.serverCallbackMap[param.ClientDispatchKey] && this.serverCallbackMap[param.ClientDispatchKey].onServerError) {
            this.serverCallbackMap[param.ClientDispatchKey].onServerError(param.Reason);
        } else if (this.serverCallback_onServerError) {
            this.serverCallback_onServerError(param.Reason||param);
        }else{
            console.info('未注册回调方法onServerError. 服务器返回错误=' + param);
        }
    }
    
	async Connect(topic:string|{Name:string,Value:any},onFirstConnect?:()=>void,calleeMap?:{[name:string]:((...args:any[])=>void)},hubContext?: string): Promise<void> {
        try {
            await this.Close();
            this.closedByStop = false;
            if (!hubContext) {
                hubContext = pbConfig.SocketServiceConfig.SignalRHubContext;
            }
            if (typeof (topic) == 'string') {
                topic = { Name: pbConfig.SocketServiceConfig.DefaultTopic, Value: topic };
            }
            let connection: HubConnection;
            connection = new ConnectionBuilder() //SignalRService.HubConnectionBuilder()//new HubConnectionBuilder()
                    .withUrl(pbConfig.SocketServiceConfig.UrlContextPrefix + hubContext + '?SignalR_TopicName=' + topic.Name + '&SignalR_TopicValue=' + topic.Value)
                    //.withAutomaticReconnect()不支持，自己实现见AutoReconnect方法
                    .configureLogging(SignalRService.LogLevel.Information)
                    .build();

            connection.onclose((a: any) => {
                console.log("onclose:" + a);
                this.singleConnection = null;
                this.state = HubConnectionState.Disconnected;
                setTimeout(() => this.AutoReconnect(connection), SocketService.AutoReconnectInterval);
            });
            if (this.ClientPromiseManager) {
                connection.on(ClientServerConst.CLIENT_PROMISE_MANAGER_CALL_BACK_METHOD,
                    (promiseResult: any) => this.ClientPromiseManager.ClientPromiseManager_CallbackOnServerComplete(promiseResult));
            }
            connection.on(ClientServerConst.CLIENT_CALLBACK_ON_SERVER_RETURN,
                (param: ServerCallbackParam) => this.OnServerReturn(param));
            connection.on(ClientServerConst.CLIENT_CALLBACK_ON_SERVER_ERROR,
                (param: ServerCallbackParam) => this.OnServerError(param));

            if (calleeMap) {
                this.calleeMap = calleeMap;
                for (let calleeName of Object.keys(this.calleeMap)) {
                    let name = calleeName;
                    connection.on(name, this.calleeMap[name]);
                }
            }
            return this.Start(connection, onFirstConnect);
        } catch (e) {
            pbErrorHandler.Throw(e);
        }
    }
    private Start(connection: HubConnection, onFirstConnect?: () => void) {
        if (this.closedByStop) return;//主动关闭的不再自动启动
        console.info('正在启动连接');
        return connection.start().then(()=>{
            this.singleConnection = connection;
            this.state = HubConnectionState.Connected;
            console.info('启动连接成功');
            this.closedByStop = false;
            if (onFirstConnect) {
                onFirstConnect();
            }
			//console.log(connection.connectionId);
		}).catch((err:any)=>{
            console.info('启动连接服务器失败:' + err);
            console.info(SocketService.AutoReconnectInterval+'毫秒后重试启动');
            setTimeout(() => this.Start(connection, onFirstConnect), SocketService.AutoReconnectInterval);
        });
    }
    private AutoReconnect(connection:HubConnection) {
        if (this.singleConnection) return;
        if (this.closedByStop) return;//主动关闭的不再自动重连
        this.state = HubConnectionState.Reconnecting;
        console.info('正在自动重连');
        connection.start().then(()=>{
            this.singleConnection = connection;
            this.state = HubConnectionState.Reconnected;
            console.info('自动重连成功');
            this.closedByStop = false;
			//console.log(connection.connectionId);
		}).catch((err:any)=>{
            console.info('自动重连失败:' + err);
            console.info(SocketService.AutoReconnectInterval +'毫秒后自动重连');
            setTimeout(() => this.AutoReconnect(connection), SocketService.AutoReconnectInterval);
        });
    }
	CallAction(actionUrl: string, param?: NameValueObj,
        funcExecuteResult?: (result: any) => any | Promise<any>,
        options: CallActionConfig = {}): Promise<any> {

        try {
            if (!this.IsConnected) {
                pbErrorHandler.ThrowAsNetWorkError(new Error("SignalR未连接"));
            }
            const requestParam: any = {};
            requestParam.ACA = actionUrl.replace(new RegExp('/'), '.');
            requestParam.ActionParamJsonMap = null;
            if (param) {
                requestParam.ActionParamJsonMap = {};
                for (let key of Object.keys(param)) {
                    requestParam.ActionParamJsonMap[key] = JSON.stringify(param[key]);
                }
            }
            const ajaxConfig = options;
            if (util.IsNullOrUndefined(ajaxConfig.processing)) {
                ajaxConfig.processing = SocketService.DefaultProcessing;
            }
            if (ajaxConfig.processing === true) {
                uni.showLoading({
                    mask: false
                });
            }
            let promise = this.singleConnection.invoke(ClientServerConst.FRONT_CONTROLLER_METHOD_HANDLE_SIGNALR_REQUEST, requestParam).then((responseData_: RcResult) => {
                uni.hideLoading();
                let responseData: RcResult = util.ConvertDateInObj(responseData_);
                try {
                    responseData.ok = responseData.isRcResult;
                } catch (e) {
                    throw new NgxArchError('响应非RcResult类型');
                }
                pb.setResultMarker(responseData);
                if (responseData.ok === true) {
                    if (responseData.data?.IsClientPromise || responseData.data?.IsClientPromiseArray) {
                        let callbackPair;
                        let clientDispatchKey;
                        if (responseData.data?.IsClientPromise) {
                            clientDispatchKey = responseData.data.ClientDispatchKey;
                        } else {
                            clientDispatchKey = responseData.data.ClientPromises[0].ClientDispatchKey;
                        }
                        if (clientDispatchKey && this.serverCallbackMap[clientDispatchKey]) {
                            callbackPair = this.serverCallbackMap[clientDispatchKey];
                        }
 
                        const thenFunc = callbackPair?.onServerReturn || this.serverCallback_onServerReturn;
                        const catchFunc = callbackPair?.onServerError|| this.serverCallback_onServerError;

                        if (responseData.data?.IsClientPromise) {//服务器返回单个Promise
                            responseData.data.Promise = this.bindPromise(responseData.data, thenFunc, catchFunc);
                        } else {//多个Promise的数组
                            responseData.data.PromiseArray = [];
                            var clientPromises: ClientPromise[] = responseData.data.ClientPromises;
                            clientPromises.forEach(cp => {
                                responseData.data.PromiseArray.push(this.bindPromise(cp, thenFunc, catchFunc));
                            });
                        }
                    } else {
                        if (ajaxConfig.overrideSuper !== true) {
                            pb.pbPlugin.ExecuteResult(responseData, pb, null);
                        }
                    }
                    if (funcExecuteResult) {
                        funcExecuteResult(responseData);
                    }
                } else if (responseData.ok === false && ajaxConfig.executeErrorResult !== false) {
                    pb.pbPlugin.ExecuteErrorResult(responseData, pb, null);
                }
                return responseData;
            }).catch((err: any) => {
                uni.hideLoading();
                if (err instanceof Error) {
                    pbErrorHandler.MarkAsNetWorkError(err);
                    pbErrorHandler.Throw(err);
                } else {
                    pb.pbPlugin.ExecuteSocketError(err);
                }
            });
            return promise;

        } catch (e) {
            pbErrorHandler.Throw(e);
        }
	}

    private bindPromise(clientPromise:ClientPromise,thenFunc:(value:any)=>void, catchFunc:(reason:any)=>void) {
        const promise = pb.pbPlugin.GetClientPromiseFromServerResult(clientPromise);
        if (thenFunc) {
            if (catchFunc) {
                promise.then(thenFunc).catch(catchFunc);
            } else {
                promise.then(thenFunc)
                    .catch((e: any) => {
                        pbErrorHandler.Throw(e);
                    });
            }
        }
        return promise;
    }
    
    Close() {
        try {
            this.state = HubConnectionState.Disconnected;
            this.closedByStop = true;
            if (!this.singleConnection) return Promise.resolve();
            return this.singleConnection.stop().then(() => {
                this.singleConnection = null;
            }).catch((err: any) => {
                console.log(err);
            });
        } catch (e) {
            pbErrorHandler.Throw(e);
        }
	}

	get IsConnected() {
        return this.singleConnection != null && this.singleConnection.connection.connectionState == HubConnectionState.Connected;
	}
}
