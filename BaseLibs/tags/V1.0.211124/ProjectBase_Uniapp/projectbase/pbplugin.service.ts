// inherit this class to plug into projectbase

import { ClientServerConst } from './ClientServerConst';
import { ProjectBaseService } from './projectbase.service';
import { NgxArchError, RcResult } from './projectbase.type';
import { pbui } from './projectbase.ui.service';
import { pbTranslate } from './TranslateService';
import { urlHelper } from './UrlHelper';

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
                    throw new NgxArchError(`routeName [${absPath}] not found for mapping to path`);
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
        } else if (rcJsonResult.isData) {

        } else {
            this.ExecuteCommand(rcJsonResult);
        }
    }
    ExecuteErrorResult(rcJsonResult: RcResult, pb: ProjectBaseService, senderId?: string) {
        if (rcJsonResult.isMsg) {
            this.ShowErrorResultMessage(rcJsonResult.data);
        } else if (rcJsonResult.isException) {
            pbui.Alert(rcJsonResult.data);
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
        } else {
            this.ExecuteCommand(rcJsonResult);
        }
    }
    ExecuteCommand(r: RcResult) {
        if (!r.ok && r.command === ClientServerConst.Command_BizException) {
            pbui.Alert(pbTranslate.Error(r.data));
        } else {
            pbui.Alert('服务器返回命令，请在子类里编写命令执行逻辑:' + r.command);
        }
    }
    ExecuteHttpError(error: any) {
        pbui.Alert('http error status: ' + error.status);
        console.log(error);
    }
    ShowResultMessage(msgKey: string) {
        pbui.ToastInfo(msgKey);
    }
    ShowErrorResultMessage(msgKey: string) {
        // 当routing resolve返回错误信息时，因为出于resolve阶段，此时ui变化要等tick后才更新，因此此处需要setTimeout来tick一下。
        const msg=pbTranslate.Error(msgKey);
        setTimeout(() => pbui.ToastWarn(msg, void 0, 0));
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
}
