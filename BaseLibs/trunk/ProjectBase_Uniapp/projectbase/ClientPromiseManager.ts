import { ClientServerConst } from "./ClientServerConst";

export class ClientPromise
{
	IsClientPromise!:boolean;
    Key!:string;
    ClientDispatchKey!:string;
	InitiatingACA?:string;
    CompletingACA?:string;

    ResolvedDataType?:string;
    ServerReturnStatus?:'Resolved'|'Rejected';
    ServerResolvedValue?:any;
    ServerRejectReason?:any;   
}
export class ClientPromiseManager {

    private registraMap: Map<string, Deferred> = new Map();

    Defer(key: string) {
        let d = new Deferred();
        this.registraMap.set(key, d);
        return d;
    }

    Resolve(key: string, clientDispatchKey: any, data: any) {
        if (this.registraMap.has(key)) {
            this.registraMap.get(key)!.Resolve(data);
            this.registraMap.delete(key);
        } else {//目前服务器端未对ACK进行注册管理，所以无论action是否返回ACKPromise，ACK报文都会通知客户端。此时客户端就是收到未注册的Promise。
            console.debug('未注册或已经完成的Promise又被Resolve,key='+key);
            console.debug(data);
        }
    }
    Reject(key: string, clientDispatchKey: any, reason?: any) {
        if (this.registraMap.has(key)) {
            this.registraMap.get(key)!.Reject(reason);
            this.registraMap.delete(key);
        } else {
            console.debug('未注册或已经完成的Promise又被Reject,key='+key);
            console.debug(reason);
        }
    }

    //SignalR callback
    ClientPromiseManager_CallbackOnServerComplete(clientPromiseObj: ClientPromise) {
        if (clientPromiseObj.ServerReturnStatus == ClientServerConst.CLIENT_PROMISE_SERVER_RETURN_STATUS_RESOLVED) {
            this.Resolve(clientPromiseObj.Key, clientPromiseObj.ClientDispatchKey, clientPromiseObj.ServerResolvedValue);
        } else if (clientPromiseObj.ServerReturnStatus == ClientServerConst.CLIENT_PROMISE_SERVER_RETURN_STATUS_REJECTED) {
            this.Reject(clientPromiseObj.Key, clientPromiseObj.ClientDispatchKey, clientPromiseObj.ServerRejectReason);
        }
    }

}
export class Deferred {

    Promise: Promise<any>;
    private ResolveFunc!: (value: any) => void;
    private RejectFunc!: (reason?: any) => void;

    constructor() {
        this.Promise = new Promise<any>((resolve, reject) => {
            this.ResolveFunc = resolve;
            this.RejectFunc = reject;
        });
    }

    Resolve(value: any) {
        this.ResolveFunc(value);
    }
    Reject(reason?: any) {
        this.RejectFunc(reason);
    }
}