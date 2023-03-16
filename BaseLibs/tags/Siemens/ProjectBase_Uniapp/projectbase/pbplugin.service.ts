// inherit this class to plug into projectbase

import { ClientPromise } from './ClientPromiseManager';
import { ClientServerConst } from './ClientServerConst';
import { pbErrorHandler } from './PBErrorHandler';
import { ProjectBaseService } from './projectbase.service';
import { NgxArchError, RcResult } from './projectbase.type';
import { pbui } from './projectbase.ui.service';
import { ProjectBaseConfig } from './ProjectBaseConfig';
import { pbTranslate } from './TranslateService';
import { urlHelper } from './UrlHelper';
import { util } from './UtilHelper';

export class PBPlugInService {
    AbsPathPrefix = '/pages';
    ExecuteResult(rcJsonResult: RcResult, pb: ProjectBaseService, hostComponentInstance?: any) {
        if (rcJsonResult.isVM) {
            // hostComponentInstance.ViewModelFormToken = rcJsonResult.data.ViewModelFormToken;
        } else if (rcJsonResult.isMsg) {
            this.ShowResultMessage(rcJsonResult.data);
        } else if (rcJsonResult.isRedirect) {
            let absPath = rcJsonResult.data.url || rcJsonResult.data;
            if (absPath.indexOf('$') < 0) {
                const link = absPath;//$$def.routeNameMapToLink[absPath];
                if (link) {
                    //absPath = link.absPath;
                } else {
                    pbErrorHandler.Throw(new NgxArchError(`routeName [${absPath}] not found for mapping to path`));
                }
            }
            if (rcJsonResult.data.params) {
                absPath = urlHelper.MergeRouteParams(absPath, rcJsonResult.data.params);
            }
            uni.reLaunch({ url: this.AbsPathPrefix +absPath });
        } else if (rcJsonResult.isAppPage) {
            const url = rcJsonResult.data;
            location.href = url;
        } else if (rcJsonResult.isEvent) {
            // pb.provideAnyData(rcJsonResult.data.eventData, rcJsonResult.data.eventName, hostComponentInstance);
        } else if (rcJsonResult.isChangedData) {
            this.ExecuteResult(rcJsonResult.data.innerResult, pb, hostComponentInstance);
        } else if (rcJsonResult.isData|| rcJsonResult.isNoop) {

        } else {
            this.ExecuteCommand(rcJsonResult);
        }
    }
    ExecuteErrorResult(rcJsonResult: RcResult, pb: ProjectBaseService, senderId?: string) {
        if (rcJsonResult.isMsg) {
            this.ShowErrorResultMessage(rcJsonResult.data);
        } else if (rcJsonResult.isException) {
            pbui.Alert(this.TranslateException(rcJsonResult.data));
        } else if (rcJsonResult.isRedirect) {
            let absPath = rcJsonResult.data.url || rcJsonResult.data;
            if (rcJsonResult.data.params) {
                absPath = urlHelper.MergeRouteParams(absPath, rcJsonResult.data.params);
            }
            uni.reLaunch({ url: this.AbsPathPrefix+absPath });
        } else if (rcJsonResult.isAppPage) {
            const url = rcJsonResult.data;
            location.href = url;
        } else if (rcJsonResult.isEvent && rcJsonResult.data.eventName === ClientServerConst.Event_ServerOnlyValidation_Fail) {
            this.ShowServerOnlyValidationMessage(rcJsonResult.data.eventData);
        } else if (rcJsonResult.isData) { // like remote val result
            // do nothing
        } else if (rcJsonResult.isBizException) {
            let data = null;
            let params=[] as any;
            const keyword = 'Exception:';
            let index = rcJsonResult.data.indexOf(keyword);
            if (index >= 0) {
                index = index + keyword.length-1;
                data = rcJsonResult.data.substr(0, index);
                params = rcJsonResult.data.substring(index + 1).split(',')
            } else {
                data = rcJsonResult.data;
                params = [];
            }
            let msg = this.TranslateException(data);
            if (params.length > 0) {
                let pindex = 0;
                for (const p of params) {
                    msg = msg.replace("{" + pindex + "}", pbTranslate.Translate(p));
                    pindex++;
                }
            }
            pbui.Alert(msg);
        } else {
            this.ExecuteCommand(rcJsonResult);
        }
    }
    private TranslateException(msgKey:string) {
        let msg = pbTranslate.Error(msgKey);
        if (pbTranslate.DeprefixError(msg) == msgKey) {
            msg = msgKey;
        }
        return msg;
    }
    ExecuteCommand(r: RcResult) {
        if (!r.ok && r.command === ClientServerConst.Command_BizException) {
            pbui.Alert(this.TranslateException(r.data));
        } else {
            pbui.Alert('服务器返回命令，请在子类里编写命令执行逻辑:' + r.command);
        }
    }
    ExecuteHttpError(error: any) {
        pbui.Alert('http error status: ' + error.status);
        console.log(error);
    }
    ExecuteSocketError(error: any) {
        pbui.Alert('SignalR error : ' + error);
        console.log(error);
    }
    ShowResultMessage(msgKey: string) {
        pbui.Alert(msgKey);
    }
    ShowErrorResultMessage(msgKey: string) {
        // 当routing resolve返回错误信息时，因为出于resolve阶段，此时ui变化要等tick后才更新，因此此处需要setTimeout来tick一下。
        const msg = this.TranslateException(msgKey);
        setTimeout(() => pbui.Alert(msg));
    }
    ShowFormValidationMessage(msgKey: string) {
        pbui.Alert(msgKey);
    }
    ShowServerOnlyValidationMessage(msgKeys: string[]) {
        msgKeys.forEach((value, i) => {
            msgKeys[i] = value;
        });
        pbui.Alert(msgKeys);
    }
    GetLoginToken():Promise<any> {
        return null;
    }
    GetClientPromiseFromServerResult(responseClientPromiseObj:ClientPromise):Promise<any>{
        //默认不延迟，子类应覆盖此方法
        return Promise.resolve(responseClientPromiseObj);
    }
    /**
     * 提示用户确认操作  
     * @param confirm false表示在confirmIdTextOrHasRows表示是否选择记录时不再进行确认提示。string表示进行确认且以此参数为msgKey  
     * @param confirmIdTextOrHasRows string表示单条操作确认，此参数值对应ajaxConfig.confirmSingle,是确认时的显示文本。boolean表示多选多选时是否有选中的
     *
     */
    async ConfirmSubmit(confirm: string | false, confirmIdTextOrHasRows: string | boolean): Promise<any> {
        let _confirmMsgKey = confirm;
        if (confirm !== false && typeof confirmIdTextOrHasRows === 'string') {
            _confirmMsgKey = _confirmMsgKey || '确认删除吗？';
            return pbui.Confirm(_confirmMsgKey, { target: confirmIdTextOrHasRows });
        } else if (!confirmIdTextOrHasRows) {
            return pbui.Alert('未选择行');
        } else if (confirm !== false) {
            _confirmMsgKey = _confirmMsgKey || '确认删除所选记录吗？';
            return pbui.Confirm(_confirmMsgKey);
        } else {
            return true;
        }
    }

    ConvertResponseToRcResult(responseData: any):RcResult {
        return responseData;
    }
    AddLoginToken(loginToken: string, config: ProjectBaseConfig, httpPostActionOptionsJson: any, httpPostActionOptions:any,httpGetActionOptions:any) {
        loginToken = loginToken || uni.getStorageSync(ClientServerConst.NAME_LOGINTOKEN);
        const channel = config.TokenChannel.toLowerCase();

        if (channel === 'header' && loginToken) {
            httpPostActionOptionsJson.header[ClientServerConst.NAME_LOGINTOKEN] = loginToken as string;
            httpPostActionOptions.header[ClientServerConst.NAME_LOGINTOKEN] = loginToken as string;
            httpGetActionOptions.header[ClientServerConst.NAME_LOGINTOKEN] = loginToken as string;
        } else if (channel === 'request') {
            const obj: any = {};
            obj[ClientServerConst.NAME_LOGINTOKEN] = encodeURIComponent(loginToken);
            return loginToken ? obj : {};
        }
    }
    SaveLoginToken(response: any[],config:ProjectBaseConfig) {
        const channel = config.TokenChannel.toLowerCase();
        if (channel == 'header') {
            let token = util.GetFromResponseHeader(response[1].header,ClientServerConst.NAME_LOGINTOKEN);
            if (token) uni.setStorageSync(ClientServerConst.NAME_LOGINTOKEN, token);
        } else if (channel == 'request' && response[1].data?.extra?.LoginToken) {
            uni.setStorageSync(ClientServerConst.NAME_LOGINTOKEN, response[1].data.extra.LoginToken);
        }
    }
}
