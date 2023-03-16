import { pbAutoValidator } from './AutoValidator';
import { Check } from './Check';
import { ClientServerConst } from './ClientServerConst';
import { pbErrorHandler } from './PBErrorHandler';
import { PBInjector } from './PbInjector';
import { PBPlugInService } from './pbplugin.service';
import {
    AjaxSubmitConfig, AjaxSubmitSimpleConfig, AnyDataSubject, CallActionConfig,
    ControlRuleConfig,
    FormArray,
    FormGroupRuleConfig,
    NameValueObj, NgxArchError, PbAbstractControl, RcResult, TypeIsAjaxSubmitConfig, ValidateOnOpt, ValidationRule, ValidatorFn, VmDataSubject, VmForm
} from './projectbase.type';
import { pbConfig } from './ProjectBaseConfig';
import { pbTranslate } from './TranslateService';
import { urlHelper } from './UrlHelper';
import { util } from './UtilHelper';
import { ClassValidator, pbValidatorRepo } from './validators';
import { VmComponentBase } from './VmComponentBase';
import { VmMeta, VmMetaMiniFied } from './VmMeta';

export class ProjectBaseService {

    static readonly Event_AjaxSubmitStart = 'pbAjaxSubmitStart';
    static readonly Event_AjaxSubmitResult = 'pbAjaxSubmitResult';
    static readonly Event_AjaxSubmitResultError = 'pbAjaxSubmitResultError';
    static readonly Event_AjaxSubmitValError = 'pbAjaxSubmitValError';
    static readonly Event_AjaxSubmitIgnored = 'pbAjaxSubmitIgnored';

    static readonly MetaTypeNameForListInput = 'ListInput';
    static readonly ControlNameForEmptyForm = '_fakedummy';

    public get pbPlugin() {
        if (!this._pbPlugin) {
            this._pbPlugin = PBInjector.get(PBInjector.InjectToken.PBPlugIn);
        }
        return this._pbPlugin;
    }

    private _pbPlugin: PBPlugInService;

    private vmMetas: { [vmTypeName: string]: VmMeta } = {};
    private vmMetasFb: { [vmTypeName: string]: PbAbstractControl } = {};

    private httpPostActionOptions = {
        header: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'X-Requested-With': 'XMLHttpRequest',
            // 'X-XSRF-TOKEN-FORM': '',
            // 'X-XSRF-TOKEN-LOGIN': '',
            //'X-Client-Type': ''
        } as any,
        withCredentials: true
    };
    private httpPostActionOptionsJson = {
        header: {
            'Content-Type': 'application/json',
            'X-Requested-With': 'XMLHttpRequest',
            // 'X-XSRF-TOKEN-FORM': '',
            // 'X-XSRF-TOKEN-LOGIN': '',
            //'X-Client-Type': ''
        } as any,
        withCredentials: true
    };
    private httpGetActionOptions = {
        header: {
            'X-Requested-With': 'XMLHttpRequest',
            // 'X-XSRF-TOKEN-LOGIN': '',
            //'X-Client-Type': ''
        } as any,
        withCredentials: true
    };

    public config = pbConfig;
    public autoValidator = pbAutoValidator;
    private validatorRepo = pbValidatorRepo;

    // <-------------------------- AjaxSubmit and result handling
    setResultMarker(rcJsonResult: RcResult) {
        if (!rcJsonResult) {
            console.error(`expected RcResult but got null or false,
                which if is an intended value, should be embeded in a RcResult object`);
        }
        const command = rcJsonResult.command;
        if (command === ClientServerConst.Command_ServerVM
            || rcJsonResult.command === ClientServerConst.Command_AjaxNavVM) {
            rcJsonResult.isVM = true;
        } else if (command === ClientServerConst.Command_ServerVMPatch) {
            rcJsonResult.isVMPatch = true;
        } else if (command === ClientServerConst.Command_ServerScrollList) {
            rcJsonResult.isScrollList = true;
        } else if (command === ClientServerConst.Command_Message) {
            rcJsonResult.isMsg = true;
        } else if (command === ClientServerConst.Command_ServerData) {
            rcJsonResult.isData = true;
        } else if (command === ClientServerConst.Command_ChangedData) {
            rcJsonResult.isChangedData = true;
            this.setResultMarker(rcJsonResult.data.innerResult);
        } else if (command === ClientServerConst.Command_Noop) {
            rcJsonResult.isNoop = true;
        } else if (command === ClientServerConst.Command_Redirect) {
            rcJsonResult.isRedirect = true;
        } else if (command === ClientServerConst.Command_AppPage) {
            rcJsonResult.isAppPage = true;
        } else if (command === ClientServerConst.Command_Event) {
            rcJsonResult.isEvent = true;
        } else if (command === ClientServerConst.Command_Exception) {
            rcJsonResult.isException = true;
        } else if (command === ClientServerConst.Command_BizException) {
            rcJsonResult.isBizException = true;
        }
    }

    /**
     * 向服务器提交请求并处理结果
     * @param ajaxUrlorAjaxSubmitConfig:只有此参数是字符型时才使用参数ajaxConfig，否则将忽略参数ajaxConfig
     */
    AjaxSubmit(ajaxUrlorAjaxSubmitConfig: string | AjaxSubmitConfig | AjaxSubmitSimpleConfig,
        hostComponent?: VmComponentBase, form_?: VmForm, vmBind_?: VmComponentBase | boolean,
        funcExecuteResult?: (result: any, argsForFunc?: any) => any | Promise<any>,
        ajaxConfig_: AjaxSubmitConfig = {}): Promise<any> | undefined {

        let ajaxUrl: string | undefined;
        let formConfig: VmForm | boolean;
        let hostComponentInstance = hostComponent;
        let form = form_;
        let vmBind = vmBind_;
        let ajaxConfig = ajaxConfig_;
        if (typeof ajaxUrlorAjaxSubmitConfig === 'string') {
            ajaxUrl = ajaxUrlorAjaxSubmitConfig;
        } else if (TypeIsAjaxSubmitConfig(ajaxUrlorAjaxSubmitConfig)) {
            ajaxConfig = ajaxUrlorAjaxSubmitConfig;
            ajaxUrl = ajaxConfig.url;
            hostComponentInstance = ajaxConfig.hostComponent || hostComponentInstance;
            formConfig = ajaxConfig.ngForm;
            vmBind = ajaxConfig.vmBind || vmBind;
        } else {
            [ajaxUrl, formConfig, vmBind] = ajaxUrlorAjaxSubmitConfig;
        }
        if (typeof formConfig === 'boolean') {
            form = formConfig ? form : null;
        } else {
            form = formConfig || form;
        }

        if (hostComponentInstance) {
            let config: AjaxSubmitConfig = { ...ajaxConfig };
            config.url = ajaxUrl;
            config.ngForm = form;
            if (!hostComponentInstance.OnAjaxSubmit(config)) {
                return new Promise((resolve, reject) => {
                    resolve(null);
                });
            }
        }
        let confirmKey: string | false | undefined;
        let confirmParams: string | boolean = '';

        if (ajaxConfig && (ajaxConfig.confirm || ajaxConfig.confirmSingle || ajaxConfig.confirmMulti)) {
            confirmKey = ajaxConfig.confirm;
            if (ajaxConfig.confirmSingle) {
                confirmParams = ajaxConfig.confirmSingle;
            } else if (ajaxConfig.confirmMulti) {
                confirmParams = this.HasSelectedRows(ajaxConfig.confirmMulti);
            }
            this.pbPlugin.ConfirmSubmit(confirmKey as string, confirmParams).then((confirmed) => {
                if (confirmed !== true) { return; }
                return this.ajaxSubmitMain(hostComponentInstance, ajaxUrl as string, ajaxConfig,
                    form, vmBind, funcExecuteResult);
            }).catch(e => {
                pbErrorHandler.Throw(e);
            });
        } else {
            return this.ajaxSubmitMain(hostComponentInstance, ajaxUrl as string, ajaxConfig,
                form, vmBind, funcExecuteResult);
        }
    }

    CallAction(actionUrl: string, params?: NameValueObj,
        funcExecuteResult?: (result: any, argsForFunc?: any) => any | Promise<any>,
        options: CallActionConfig = {}): Promise<any> {

        let { senderId, method } = options;
        method = method || 'POST';
        if (options.postJson !== true) options.postJson = false;
        const ajaxBodyData = options.postJson ? params : util.Obj2Qs(params);

        let url = actionUrl;
        const qs = {};
        url = urlHelper.Action(url, qs);
        if (util.IsNullOrUndefined(senderId)) senderId = true;
        options.senderId = senderId === true ? this.getSenderIdFromUrl(actionUrl) : senderId;
        return this.doSubmit(url, method, ajaxBodyData, options.hostComponent, null, options, funcExecuteResult);
    }
    NotifyValError(senderId: string, msgKey?: string) {
        this.ProvideAnyData({
            msgKey: msgKey || ProjectBaseService.Event_AjaxSubmitValError
        }, ProjectBaseService.Event_AjaxSubmitValError, null, senderId);
    }
    async Validate(formGroup: VmForm, valGroups?: string | string[], partialValid?: string | string[]): Promise<string> {
        const ok = await this.autoValidator.validate(formGroup as VmForm, valGroups, partialValid);
        if (!ok) {
            var msg= (formGroup as PbAbstractControl).pbResolvedErrors && (formGroup as PbAbstractControl).pbResolvedErrors.classValidate ?
                (formGroup as PbAbstractControl).pbResolvedErrors.classValidate.errMsg : ProjectBaseService.Event_AjaxSubmitValError;
            if (msg == ProjectBaseService.Event_AjaxSubmitValError) {
                msg = '表单验证未通过';
            }
            this.pbPlugin.ShowFormValidationMessage(msg);
            return msg;
        } else {
            return null;
        }
    }

    HasSelectedRows(array: any[]) { // pbCheckValue对绑定的多选列表中未选项保留绑定值为null，数组长度不变。
        return array.some((item) => item !== null && item !== undefined);
    }
    HasSingleRowSelected(array: any[]) { // pbCheckValue对绑定的多选列表中未选项保留绑定值为null，数组长度不变。
        let cnt = 0;
        array.forEach((item) => {
            if (item !== null && item !== undefined) {
                cnt++;
                if (cnt > 1) return false;
            }
        });
        return cnt === 1;
    }

    // --------------------------> AjaxSubmit and result handling


    // <---------------------------VM metadata and type related logic-----------------------
    GetVm(serverResult: RcResult) {
        this.setResultMarker(serverResult);
        if (serverResult.isVM) {
            if (serverResult.data.ViewModel) {
                const noncircularObj = {
                    ok: serverResult.ok,
                    command: serverResult.command,
                    data: {
                        ViewModelFormToken: serverResult.data.ViewModelFormToken,
                        ViewModelTypeName: serverResult.data.ViewModelTypeName
                    }
                } as any;
                serverResult.data.ViewModel._FromResult = noncircularObj;
            } else {
                console.log("can't append _FromResult, serverResult.data.ViewModel is null");
            }
            return serverResult.data.ViewModel;
        } else if (serverResult.ok === false) {
            throw serverResult; // trigger state change error,make state change fail
        } else {
            return {};
        }
    }
    GetVmTypeName(serverResult: RcResult) {
        return serverResult.isVM ? serverResult.data.ViewModelTypeName : null;
    }
    CheckVmInputType(vmTypeName: string): string {
        let rtn = null;
        if (this.GetVmMeta(vmTypeName)) {
            rtn = vmTypeName;
        } else if (this.GetVmMeta(vmTypeName + ClientServerConst.ViewModel_InputSuffix)) {
            rtn = vmTypeName + ClientServerConst.ViewModel_InputSuffix;
        } else if (this.GetVmMeta(vmTypeName + ClientServerConst.ViewModel_InputSuffix2)) {
            rtn = vmTypeName + ClientServerConst.ViewModel_InputSuffix2;
        } else if (vmTypeName.endsWith('ListVM') && this.GetVmMeta(ProjectBaseService.MetaTypeNameForListInput)) {
            rtn = ProjectBaseService.MetaTypeNameForListInput;
        } else {
            console.warn('CheckVmType failed: ' + vmTypeName);
        }
        return rtn;
    }

    /**
     * 创建表单验证规则对象
     */
    CreateFormGroup(vmInputTypeNameOrVmMeta: string | VmMeta, validateOn?: ValidateOnOpt, innerValidators?: {
        [name: string]: ValidatorFn;
    }): VmForm {
        try {
            let metafb: PbAbstractControl;
            let vmInputTypeName: string;
            if (typeof vmInputTypeNameOrVmMeta === 'string') {
                vmInputTypeName = vmInputTypeNameOrVmMeta;
                metafb = this.getVmMetaFb(vmInputTypeNameOrVmMeta);
                if (!metafb) {
                    metafb = this.buildVmMetaFb(vmInputTypeName, validateOn, innerValidators);
                }
                this.setVmMetaFb(vmInputTypeName, metafb);
            } else {
                vmInputTypeName = vmInputTypeNameOrVmMeta.modelType;
                metafb = this.recursiveBuildVmMetaFb(vmInputTypeNameOrVmMeta, vmInputTypeName, null, null, validateOn, innerValidators);
            }
            metafb.innerValidators = innerValidators;
            return metafb;
        } catch (e) {
            pbErrorHandler.Throw(e);
        }
    }
    /**
    * 指定FormArray从属的FormGroup的属性，则添加数组的整体验证。否则只指定数组元素类型不添加整体验证，包括数组大小验证。
    * @param containingFormGroup - 返回的formarray作为已有formGroup的子节点
    * @param arrayPropNameOrVmInputTypeNameOfEle - 如果containingFormGroup不为空，此参数值应为containingFormGroup的属性名，否则应为数组元素对应的类型名
    * @param arrayValue - 数组值用于初始赋值，如果不赋值，此参数值为整数表示创建多少元素。
    */
    CreateFormArray(containingFormGroup: VmForm, arrayPropNameOrVmInputTypeNameOfEle: string, arrayValue: any[] | number, classValidator?: ClassValidator): FormArray {
        try {
            return this.CreateFormArrayInternal(containingFormGroup, arrayPropNameOrVmInputTypeNameOfEle, arrayValue, classValidator);
        } catch (e) {
            pbErrorHandler.Throw(e);
        }
    }

    private CreateFormArrayInternal(containingFormGroup: VmForm, arrayPropNameOrVmInputTypeNameOfEle: string, arrayValue: any[] | number, classValidator ?: ClassValidator): FormArray {
        let arrayMeta: VmMeta;
        let vmInputTypeNameOfEle;
        let formArray: FormArray;

        if (containingFormGroup) {
            formArray = containingFormGroup.controls[arrayPropNameOrVmInputTypeNameOfEle] as FormArray;
            Check.Require(!!formArray, '未找到FormArray对应的属性：[' + arrayPropNameOrVmInputTypeNameOfEle + ']。如果是想指定数组元素类型名应将参数containingFormGroup置空');
            arrayMeta = (containingFormGroup as PbAbstractControl).pbVmMeta.properties[arrayPropNameOrVmInputTypeNameOfEle];
            vmInputTypeNameOfEle = this.getEleTypeName(arrayMeta.modelType);
        } else {
            vmInputTypeNameOfEle = arrayPropNameOrVmInputTypeNameOfEle;
        }
        if (formArray && formArray.controls && Object.keys(formArray.controls).length > 0) {// containingFormGroup的相应子节点已经创建为FormArray，进行初始赋值
            if (Array.isArray(arrayValue)) {
                this.SetFormValue(formArray, arrayValue);
            } else {
                const placeholder: any[] = [];
                for (let i = 0; i < arrayValue; i++) placeholder.push(null);
                this.SetFormValue(formArray, placeholder);
            }
            return formArray;
        }
        // containingFormGroup未指定，根据meta创建FormArray
        const controls = [];
        const length = Array.isArray(arrayValue) ? arrayValue.length : arrayValue;
        if (arrayMeta && !arrayMeta.properties) {
            for (let i = 0; i < length; i++) {
                controls[i] = this.recursiveBuildVmMetaFb(arrayMeta, arrayPropNameOrVmInputTypeNameOfEle, null, true);
            }
        } else {
            for (let i = 0; i < length; i++) {
                controls[i] = this.CreateFormGroup(vmInputTypeNameOfEle);
            }
        }
        let validators = containingFormGroup ? this.getValidators(arrayMeta) : null;
        if (classValidator) {
            validators = validators || [];
            validators.push(this.validatorRepo.getValidator('classValidate')(classValidator));
        }
        if (formArray && formArray.controls) {
            formArray.arrayValidators = validators;
        } else {
            formArray = { namePrefix: arrayPropNameOrVmInputTypeNameOfEle, arrayValidators: validators };
        }
        controls.forEach((ctrl: PbAbstractControl, i: number) => formArray.controls[i] = ctrl);
        (formArray as PbAbstractControl).pbVmMeta = arrayMeta;
        if (Array.isArray(arrayValue)) {
            this.SetFormValue(formArray, arrayValue);
        }

        return formArray;
    }
    LoadVmMeta(jsonObj: any) {
        if (!jsonObj) { return; }
        const restoredMetas = {} as any;
        for (const key of Object.keys(jsonObj)) {
            restoredMetas[key] = this.restoreVmMeta(jsonObj[key]);
        }
        Object.assign(this.vmMetas, restoredMetas);
        for (const key of Object.keys(restoredMetas)) {
            this.linkVmMeta(restoredMetas[key]);
        }
    }
    private restoreVmMeta(jsonObj: VmMetaMiniFied/*minified VmMeta*/) {
        const meta = new VmMeta();
        meta.modelType = jsonObj.m;
        meta.required = jsonObj.r;
        meta.translate = jsonObj.k;
        meta.formIgnore = jsonObj.f;
        meta.submitIgnore = jsonObj.s;
        if (jsonObj.v) {
            meta.rules = {};
            for (const key of Object.keys(jsonObj.v)) {
                const minirule = jsonObj.v[key];
                const rule = { type: minirule.t, param: minirule.p, groups: minirule.g, errmsg: minirule.e };
                meta.rules[key] = rule;
            }
        }
        if (jsonObj.c) {
            meta.properties = {};
            for (const key of Object.keys(jsonObj.c)) {
                meta.properties[key] = this.restoreVmMeta(jsonObj.c[key]);
            }
        }
        return meta;
    }
    GetVmMeta(vmInputTypeName: string) {
        return this.vmMetas[vmInputTypeName];
    }
    private addFormToken(formToken:string) {
        formToken=formToken||'';
        const channel=this.config.TokenChannel.toLowerCase();
        if(channel==='header'&&formToken){
            this.httpPostActionOptionsJson.header[ClientServerConst.NAME_FORMTOKEN] = formToken || '';
            this.httpPostActionOptions.header[ClientServerConst.NAME_FORMTOKEN] = formToken || '';
        }else{
            const obj:any={};
            obj[ClientServerConst.NAME_FORMTOKEN]=encodeURIComponent(formToken);
            return formToken?obj:{};
        }
    }
    private addClientType() {
        const channel=this.config.TokenChannel.toLowerCase();
        if(channel==='header'){
            this.httpPostActionOptionsJson.header[ClientServerConst.NAME_CLIENTTYPE] = this.config.ClientType;
            this.httpPostActionOptions.header[ClientServerConst.NAME_CLIENTTYPE] = this.config.ClientType;
            this.httpGetActionOptions.header[ClientServerConst.NAME_CLIENTTYPE] = this.config.ClientType;
        } else{
            const obj:any={};
            obj[ClientServerConst.NAME_CLIENTTYPE]=this.config.ClientType;
            return obj;
        }
    }
    private addLoginToken(loginToken: string) {
        loginToken=loginToken||uni.getStorageSync(ClientServerConst.NAME_LOGINTOKEN);
        const channel=this.config.TokenChannel.toLowerCase();

        if(channel==='header'&&loginToken){
            this.httpPostActionOptionsJson.header[ClientServerConst.NAME_LOGINTOKEN] = loginToken as string;
            this.httpPostActionOptions.header[ClientServerConst.NAME_LOGINTOKEN] = loginToken as string;
            this.httpGetActionOptions.header[ClientServerConst.NAME_LOGINTOKEN] = loginToken as string;
        } else if (channel === 'request') {
            const obj:any={};
            obj[ClientServerConst.NAME_LOGINTOKEN] = encodeURIComponent(loginToken);
            return loginToken?obj:{};
        }
    }
    private saveLoginToken(response:any[]) {
        const channel = this.config.TokenChannel.toLowerCase();
        if (channel == 'header') {
            let token=response[1].header[ClientServerConst.NAME_LOGINTOKEN.toLowerCase()];
            if(token) uni.setStorageSync(ClientServerConst.NAME_LOGINTOKEN, token);
        } else if (channel == 'request' && response[1].data?.extra?.LoginToken) {
            uni.setStorageSync(ClientServerConst.NAME_LOGINTOKEN, response[1].data.extra.LoginToken);
        }
    }
    RemoveLoginToken() {
        uni.removeStorageSync(ClientServerConst.NAME_LOGINTOKEN);
    }
    private ajaxSubmitMain(hostComponentInstance: VmComponentBase | any, ajaxUrl: string, ajaxConfig_?: AjaxSubmitConfig,
        form?: VmForm, vmBind?: VmComponentBase | boolean,
        funcExecuteResult?: (result: any, argsForFunc?: any) => any | Promise<any>): Promise<any> {

        // let processingAreaId;
        let method: string;
        let formValueContainingProp: string | boolean;
        let ajaxData: any;
        let ajaxBodyData;
        let url = ajaxUrl;

        let ajaxConfig = ajaxConfig_ || { url: 'placeholder' };
        ajaxData = ajaxConfig.bodyData;
        // processingAreaId = ajaxConfig.processing;
        method = ajaxConfig.method || 'POST';
        if (ajaxConfig.postJson !== true) ajaxConfig.postJson = false;
        formValueContainingProp = ajaxConfig.formValueContainingProp;
        if (formValueContainingProp === true) formValueContainingProp = 'Input';

        // if (!processingAreaId) { processingAreaId = form || true; }
        if (ajaxConfig.senderId !== false) {
            ajaxConfig.senderId = ajaxConfig.senderId || (!funcExecuteResult ? ajaxConfig.argsForFunc : null) || this.getSenderIdFromUrl(ajaxUrl);
        }

        let formData: any;
        const afterVal = () => {
            if(form){
                formData = this.serializeAngularForm(form, (formValueContainingProp || 'Input') as string, ajaxConfig.postJson);
            }else{
                formData=ajaxConfig.postJson?{}:'';
            }
            if (!formValueContainingProp) {
                if (ajaxConfig.postJson) {
                    formData = formData['Input'];
                } else {
                    formData = (formData as string).replace(new RegExp('Input.', 'gm'),'');
                }
            }
            ajaxBodyData = ajaxConfig.postJson ? util.MergeDeep(formData || {}, ajaxData) : util.MergeQS(formData || '', util.Obj2Qs(ajaxData));

            const qs: NameValueObj = {};
            if (ajaxConfig.pbInstruct !== null) {
                qs.pbInstruct = ajaxConfig.pbInstruct;
            }
            if (ajaxConfig && ajaxConfig.qs) {
                Object.assign(qs, ajaxConfig.qs);
            }
            url = urlHelper.Action(url, qs);
            return this.doSubmit(url, method, ajaxBodyData, hostComponentInstance, form, ajaxConfig, funcExecuteResult);
        };

        if (form) {
            return this.Validate(form as VmForm, ajaxConfig.valGroups, ajaxConfig.partialValid)
                .then((errmsg) => {
                    if (errmsg) {
                        this.NotifyValError(ajaxConfig.senderId as string, errmsg);
                    } else {
                        // formData = this.serializeAngularForm(form, formValueContainingProp as string || 'input', ajaxConfig.postJson);
                        // if (!formValueContainingProp) {
                        //     formData = formData[formValueContainingProp as string || 'input'];
                        // }
                        return afterVal();
                    }
                }).catch(e => {
                    pbErrorHandler.Throw(e);
                });
        }
        return afterVal();
    }

    private async doSubmit(url: string, method: string, ajaxBodyData: any, hostComponentInstance: VmComponentBase | any, form?: VmForm, ajaxConfig?: AjaxSubmitConfig,
        funcExecuteResult?: (result: any, argsForFunc?: any) => any | Promise<any>): Promise<any> {

        let bindTarget: boolean | VmComponentBase;
        let autoBind = true; // bind returned result by default
        if (ajaxConfig.vmBind === false) {
            autoBind = false;
        } else {
            bindTarget = ajaxConfig.vmBind || hostComponentInstance;
        }
        if (!bindTarget) {
            autoBind = false;
        }

        if (ajaxConfig.processing !== false) {
            uni.showLoading({
                mask:true
            });
        }
        let senderId = ajaxConfig.senderId;

        let request: Promise<any>;
        /*******************************
        let checker = method.toLowerCase() === 'get' ? this.getRequestChecker : this.postRequestChecker;
        if (ajaxConfig.requestCheck !== false) {
            const checkRtn = checker.check(url, ajaxConfig.requestCheck);
            if (!checkRtn) {
                if (ajaxConfig.requestCheckEmit && senderId) {
                    this.provideAnyData({ msgKey: this.util.msgKey(ProjectBaseService.Event_AjaxSubmitIgnored), checkRtn }, ProjectBaseService.Event_AjaxSubmitIgnored, null, senderId as string);
                }
                return new Promise((resolve, reject) => {
                    reject({ reason: ProjectBaseService.Event_AjaxSubmitIgnored, checkRtn });
                });
            }
        }
        */

        if (senderId) {
            this.ProvideAnyData({}, ProjectBaseService.Event_AjaxSubmitStart, void 0, senderId as string);
        }

        const byRequestClientType=this.addClientType();
        const byRequestLoginToken=this.addLoginToken(ajaxConfig.loginToken);
        var byRequestObj={...byRequestLoginToken,...byRequestClientType};
        if (method.toLowerCase() === 'get') {
            request = uni.request({ url, method: 'GET', ...this.httpGetActionOptions,data:byRequestObj }) as any as Promise<any>;
            //////////////////////if (this.config.HttpRetry > 0) request = request.pipe(retryWhen(genericRetryStrategy()));
        } else if (method.toLowerCase() === 'post') {
            const byRequestFormToken:any=this.addFormToken(ajaxConfig.formToken);
            byRequestObj={...byRequestObj,...byRequestFormToken};
            if(byRequestObj){
                if(ajaxConfig.postJson){
                    ajaxBodyData={...ajaxBodyData,...byRequestObj};
                }else{
                    if(ajaxBodyData){
                        ajaxBodyData+='&';
                    }
                    ajaxBodyData=ajaxBodyData+util.ParamsToQS(byRequestObj);
                }
            }
            const opn = ajaxConfig.postJson ? this.httpPostActionOptionsJson : this.httpPostActionOptions;
            request = uni.request({ url, method: 'POST', data: ajaxBodyData, ...opn }) as any as Promise<any>;
            //////////////////if (this.config.HttpRetry > 0) request = request.pipe(retryWhen(genericRetryStrategy()));
        } else {
            pbErrorHandler.Throw(new Error('仅支持get/post'));
        }
        // tslint:disable-next-line: cyclomatic-complexity
        let promise = request.then((response: any[]) => {
            uni.hideLoading();
            if (response[1]) {
                if (response[1].statusCode == 200) {
                    this.saveLoginToken(response);
                    return response[1].data;
                } else {
                    throw { name:'NetWorkError',status: response[1].statusCode };
                }
            } else {
                const err = new Error('http响应格式异常:' + JSON.stringify(response));
                err.name = 'NetWorkError';
                throw err;
            }

        }).then((responseData_: RcResult) => {
            let responseData = responseData_;
            try {
                responseData.ok = responseData.isRcResult;
            } catch (e) {
                throw new NgxArchError('响应非RcResult类型');
            }
            ////////////////////////checker.uncheck(url);
            this.setResultMarker(responseData);
            if (autoBind && responseData.isVM) {
                if (bindTarget === hostComponentInstance) {
                    hostComponentInstance.BindVm(responseData.data.ViewModel);
                    // if (hostComponentInstance.vmTypeName === responseData.data.ViewModelTypeName) {
                    //     hostComponentInstance.BindVm(responseData.data.ViewModel);
                    // } else {
                    //     throw new Error(`View model types mismatch: ${responseData.data.ViewModelTypeName}(server)
                    //             != ${hostComponentInstance.vmTypeName}(client)`);
                    // }
                } else {
                    const subjectData: VmDataSubject = {
                        srcComponentType: void 0,
                        srcId: ajaxConfig.senderId,
                        vm: responseData.data.ViewModel,
                        vmTypeName: responseData.data.ViewModelTypeName
                    };
                    this.provideVm(subjectData);
                }
            }
            if (responseData.ok === true) {
                let hasChangedData = responseData.isChangedData;
                if (hasChangedData) {// 对changedData类型的响应结构数据进行结构调整
                    let wrapper = responseData;
                    responseData = wrapper.data.innerResult;
                    responseData.deletedIds = wrapper.data.deletedIds;
                    responseData.changedData = wrapper.data.changedData;
                }
                if (ajaxConfig.overrideSuper !== true && hasChangedData && hostComponentInstance) {
                    if (responseData.deletedIds) hostComponentInstance.onServerDeleted(responseData.deletedIds, ajaxConfig.senderId);
                    if (responseData.changedData) hostComponentInstance.onServerChanged(responseData.changedData, ajaxConfig.senderId);
                }
                if (autoBind && hostComponentInstance) {
                    if (responseData.isVMPatch) {
                        hostComponentInstance.patchVm(responseData.data);
                    } else if (responseData.isVM) {
                        hostComponentInstance.BindVm(responseData.data.ViewModel);
                        // if (hostComponentInstance.vmTypeName === responseData.data.ViewModelTypeName) {

                        // } else {
                        //     throw new Error(`View model types mismatch: ${responseData.data.ViewModelTypeName}(server)
                        //         != ${hostComponentInstance.vmTypeName}(client)`);
                        // }
                    }
                }
                if (ajaxConfig.overrideSuper !== true) {
                    this.pbPlugin.ExecuteResult(responseData, this, hostComponentInstance);
                }
                let markFormAsPristine = true;
                if (funcExecuteResult) {
                    markFormAsPristine = funcExecuteResult(responseData, ajaxConfig.argsForFunc);
                } else {
                    if (hostComponentInstance && hostComponentInstance.executeResult) {
                        markFormAsPristine = hostComponentInstance.executeResult(responseData, ajaxConfig.senderId);
                    }
                    if (responseData.isNoop && hostComponentInstance && hostComponentInstance.executeNoopResult) {
                        // No server command then look for client callback
                        markFormAsPristine = hostComponentInstance.executeNoopResult(ajaxConfig.senderId);
                    }
                }
                if (markFormAsPristine !== false) {
                    markFormAsPristine = true;
                }
                if (form && markFormAsPristine) {
                    ///////////////////form.markAsPristine();
                }
                const msg = responseData.isMsg ? responseData.data : util.MsgKey(ProjectBaseService.Event_AjaxSubmitResult);
                if (senderId) {
                    this.ProvideAnyData({ msgKey: msg }, ProjectBaseService.Event_AjaxSubmitResult, void 0, senderId as string);
                }
            } else if (responseData.ok === false && ajaxConfig.executeErrorResult !== false) {
                this.pbPlugin.ExecuteErrorResult(responseData, this, senderId as string);
                const msg = responseData.isMsg ? responseData.data : util.MsgKey(ProjectBaseService.Event_AjaxSubmitResultError);
                if (senderId) {
                    this.ProvideAnyData({ msgKey: msg }, ProjectBaseService.Event_AjaxSubmitResultError, void 0, senderId as string);
                }
            }
            return responseData;
        }).catch((err: any) => {
            uni.hideLoading();
            ////////////////////////checker.uncheck(url);
            if (err instanceof Error) {
                pbErrorHandler.MarkAsNetWorkError(err);
                pbErrorHandler.Throw(err);
            } else {
                this.pbPlugin.ExecuteHttpError(err);
            }
            if (senderId) {
                this.ProvideAnyData({ msgKey: 'system.error' }, util.MsgKey(ProjectBaseService.Event_AjaxSubmitResultError), void 0, senderId as string);
            }
        });
        return promise;
    }
    
    private serializeAngularForm(formGroup: VmForm, formValueContainingProp: string, postJson = true): string|any {
        if (postJson) {
            return this.serializeAngularFormJson(formGroup, formValueContainingProp);
        } else {
            return this.serializeAngularFormNv(formGroup, formValueContainingProp);
        }
    }
    /**
     * serilize from data to a string in format of qs namevalue pairs. include @FormIgnore,exclude @SubmitIgnore
     */
    private serializeAngularFormNv(formGroup: VmForm, formValueContainingProp: string): string {
        const submitdata: { [containingProp: string]: any } = {};
        const meta = new VmMeta();
        meta.properties = {};
        meta.properties[formValueContainingProp] = (formGroup as VmForm).pbVmMeta;
        submitdata[formValueContainingProp] = formGroup.value;
        const qs1 = util.Obj2Qs(submitdata, meta);
        if ((formGroup as VmForm).pbVmInput) {
            submitdata[formValueContainingProp] = (formGroup as VmForm).pbVmInput;
            const qs2 = util.Obj2Qs(submitdata, meta, true);
            return util.MergeQS(qs1, qs2);
        } else {
            return qs1;
        }
    }
    /**
     * serilize from data to an object . include @FormIgnore,exclude @SubmitIgnore
     */
    private serializeAngularFormJson(formGroup: VmForm, formValueContainingProp: string): any {
        const submitdata: { [containingProp: string]: any } = {};
        const meta = new VmMeta();
        meta.properties = {};
        meta.properties[formValueContainingProp] = (formGroup as VmForm).pbVmMeta;
        submitdata[formValueContainingProp] = formGroup.value;
        util.MetaFilter(submitdata, null, meta);
        if ((formGroup as VmForm).pbVmInput) {
            const submitdata2: { [containingProp: string]: any } = {};
            submitdata2[formValueContainingProp] = util.CopyDeep((formGroup as VmForm).pbVmInput);
            util.MetaFilter(submitdata2, null, meta, true);
            return util.MergeDeep(submitdata, submitdata2);
        } else {
            return submitdata;
        }
    }
    private linkVmMeta(vmMeta: VmMeta) {
        if (!vmMeta.properties) {
            let referencedMeta = this.vmMetas[vmMeta.modelType];
            if (!referencedMeta) {
                if (this.modelTypeIsArray(vmMeta.modelType)) {
                    const eleType = this.getEleTypeName(vmMeta.modelType);
                    referencedMeta = this.vmMetas[eleType];
                } else if (vmMeta.isDORef()) {
                    referencedMeta = this.vmMetas.DORef;
                }
            }
            if (referencedMeta) {
                vmMeta.properties = referencedMeta.properties;
            }
        } else {
            for (const key of Object.keys(vmMeta.properties)) {
                this.linkVmMeta(vmMeta.properties[key]);
            }
        }
    }

    /** if prop of obj type of the vm is null, remove the prop so not to set the formgroup's value */
    SetFormValue(vmForm: VmForm, vmInput: any) {
        const patchData = Object.assign(Array.isArray(vmInput) ? [] : {}, vmInput);
        this.recursiveRemoveNullValueForFormGroup(vmForm, patchData);
        vmForm.value = patchData;
    }

    // <-----------------Component intercommunication
    private provideVm(data: VmDataSubject | any, vmTypeName?: string,
        srcComponentType?: string, srcId?: string, targetId?: string) {
        if (!vmTypeName) {
            uni.$emit(data.vmTypeName, data as VmDataSubject);
        } else {
            uni.$emit(vmTypeName, {
                vm: data, vmTypeName, srcComponentType, srcId, targetId
            });
        }
    }

    // subscribeVm(targetComponentInstance: VmComponentBase, func?: (data: VmDataSubject) => void) {
    //     let _func = func;
    //     if (!_func) {
    //         _func = (data: VmDataSubject) => {
    //             // if (targetComponentInstance.vmTypeName === data.vmTypeName) {
    //             targetComponentInstance.bindVm(data.vm);
    //             //  }
    //         };
    //     }
    //     return this.vmSource.pipe(filter((data) => targetComponentInstance.vmTypeName === data.vmTypeName)).subscribe(_func);
    //     return uni.$on((subjectName, func);
    // }


    ProvideAnyData(data: AnyDataSubject | any, subjectName?: string, srcComponentType?: string, srcId?: string,
        targetComponentType?: string, targetId?: string) {
        if (!subjectName) {
            uni.$emit(data.subjectName, data as AnyDataSubject);
        } else {
            uni.$emit(subjectName, {
                subjectName, anyData: data, srcComponentType, srcId,
                targetComponentType, targetId
            });
        }
    }

    // subscribeAnyData(subjectName: string, func?: (data: AnyDataSubject) => void){
    //     const ob = this.anyDataSource.pipe(filter((data) => subjectName === data.subjectName));
    //     return func ? ob.subscribe(func) : ob;
    // }

    // --------------------------------->Component intercommunication

    private getEleTypeName(modelType: string) {
        return modelType.substr(modelType.indexOf('=') + 1).split(',')[0];
    }
    private modelTypeIsArray(modelType: string) {
        return modelType.includes('=');
    }
    private recursiveRemoveNullValueForFormGroup(vmFormorCtrl: PbAbstractControl, patchData: any) {
        const ctls = vmFormorCtrl.controls;
        for (const ctlname of Object.keys(ctls)) {
            if (ctls[ctlname].controls) {
                if (!patchData[ctlname]) {
                    if ((ctls[ctlname] as PbAbstractControl).pbVmMeta.isDORef()) {
                        patchData[ctlname] = { id: null };
                    } else {
                        delete patchData[ctlname];
                    }
                } else {
                    this.recursiveRemoveNullValueForFormGroup(ctls[ctlname], patchData);
                }
            }
        }
    }
    private getVmMetaFb(vmInputTypeName: string): PbAbstractControl {
        return this.vmMetasFb[vmInputTypeName];
    }
    private setVmMetaFb(vmInputTypeName: string, vmMetaFb: PbAbstractControl) {
        this.vmMetasFb[vmInputTypeName] = vmMetaFb;
    }
    private buildVmMetaFb(vmInputTypeName: string, validateOn?: ValidateOnOpt, innerValidators?: {
        [name: string]: ValidatorFn;
    }): PbAbstractControl {
        const meta: VmMeta = this.GetVmMeta(vmInputTypeName);
        if (!meta) {
            pbErrorHandler.Throw(new Error(`VmMeta data not found for vmInputTypeName "${vmInputTypeName}"`));
        }
        let form = this.recursiveBuildVmMetaFb(meta, vmInputTypeName, null, null, validateOn, innerValidators);
        return form;
    }

    /**
     * @param justForLeafEle - 仅对简单类型成员的数组的成员生成节点。简单类型成员的数组对应VmMeta中没有properties，
     * 因此数组整体验证规则与成员验证规则都一起同级存在meta.rules中，因此需要justForLeafEle参数来表明特殊处理这个情况。
     */
    // tslint:disable-next-line: cyclomatic-complexity
    private recursiveBuildVmMetaFb(meta: VmMeta, nodeName: string, parentMeta?: VmMeta, justForLeafEle = false, validateOn?: ValidateOnOpt, innerValidators?: {
        [name: string]: ValidatorFn;
    }): PbAbstractControl {
        if ((meta.formIgnore || meta.modelType === ProjectBaseService.MetaTypeNameForListInput) && !justForLeafEle) {
            return null;
        }

        const controlsConfig = {} as any;
        const controls = {} as any;
        let metaFbPointer: PbAbstractControl;

        let validators: any[];//这里any指uni-forms的rule对象
        if (nodeName === 'id' && parentMeta && parentMeta.isDORef() && (this.shouldVal(parentMeta.rules.required) || this.shouldVal(parentMeta.rules.dorefNotNull))) {
            validators = [];
            validators.push(this.validatorRepo.getValidator('dorefNotNullId')());
            meta.rules = {};
            meta.rules.dorefNotNullId = { type: 'dorefNotNullId', groups: parentMeta.rules.dorefNotNull.groups };
        } else {
            validators = this.getValidators(meta, justForLeafEle, null, innerValidators);
        }

        let defaultValidateOn = 'blur';
        if (nodeName === 'id' && parentMeta && parentMeta.isDORef() || meta.isBool() || meta.isEnum()) {
            defaultValidateOn = 'change';
        }
        let nodeValidateOn = validateOn || defaultValidateOn;
        let applyValidateOn = (typeof (nodeValidateOn) === 'string' ? nodeValidateOn : nodeValidateOn._validateOn) || defaultValidateOn;

        /*每个属性建立一个节点，属性为非数组的复合类型时递归建立子节点,对数组和叶型不生成子节点。数组对应的节点创建为FormArray。
         * （此句指createFormArray方法：因为FormArray中的control为手动生成，因此默认不能验证，如果想一起验证FormArray的control，可以手动将手动生成的FormArray的controls添加到FormGroup中的FormArray中）
         */
        if (this.modelTypeIsArray(meta.modelType) && !justForLeafEle) {// array prop
            metaFbPointer = {
                pbRuleConfig: {
                    rules: validators //////////////////////?????????????
                }
            };
        } else if (meta.properties) {
            for (const prop of Object.keys(meta.properties)) {
                const propValidateOn = (validateOn || {} as any)[prop] || applyValidateOn;
                const fctl = this.recursiveBuildVmMetaFb(meta.properties[prop], prop, meta, false, propValidateOn, innerValidators);
                if (fctl) {
                    controlsConfig[prop] = fctl.pbRuleConfig;
                    controls[prop] = fctl;
                }
            }
            if (controlsConfig && Object.keys(controlsConfig).length === 0) {
                const fakeControl = {} as PbAbstractControl;
                fakeControl.pbVmMeta = new VmMeta();
                fakeControl.pbVmMeta.submitIgnore = true;
                controlsConfig[ProjectBaseService.ControlNameForEmptyForm] = fakeControl;
            }
            // tslint:disable-next-line: no-object-literal-type-assertion
            metaFbPointer = {
                pbRuleConfig: controlsConfig && Object.keys(controlsConfig).length === 0 ? null : controlsConfig,
                controls: controls
            };
        } else {
            if (validators) {
                validators.forEach((v, i) => {
                    ///////////////////////v.trigger = applyValidateOn;目前无法实现此效果
                });
            }
            metaFbPointer = {
                pbRuleConfig: validators ? {
                    label: meta.translate,
                    rules: validators
                } : null
            };
        }
        metaFbPointer.pbVmMeta = meta;
        return metaFbPointer;
    }
    private getValidators(meta: VmMeta, justForLeafEle = false, valGroups?: string | string[], innerValidators?: {
        [name: string]: ValidatorFn;
    }) {
        const rules = meta.rules;
        let validators: any[] = [];//这里any指uni-forms的rule对象
        if (rules) {
            for (const rulekey of Object.keys(rules)) {
                let rule = rules[rulekey];
                if (!this.shouldVal(rule, valGroups)) continue;
                const isEleRule = rulekey.startsWith(ClientServerConst.VmMeta_LeafElePrefix);
                if (!justForLeafEle && isEleRule
                    || justForLeafEle && !isEleRule) {
                    continue;
                }
                let validator = this.validatorRepo.getValidator(rule.type);
                if (validator === 'ignore') continue;
                if (validator) {
                    if (rule.type === ClientServerConst.ValidationType_ClassValidate) {
                        //////////////////// if (classValidator) validators.push(validator(classValidator));
                    } else {
                        validators.push(validator(rule.param));
                    }
                } else if (rule.type.startsWith(ClientServerConst.ValidationType_RemoteValPrefix)) {
                    validators.push({
                        validateFunction:
                            (rule: any, value: any, data: any, callback: () => void): Promise<any> => {
                                let url = (rule.param as string).replace(ClientServerConst.RemoveValidation_ParamPlaceHolder, value);
                                return this.CallAction(url, null, (r: RcResult) => {
                                    if (r.ok && r.data === true) {
                                        return;
                                    } else {
                                        pbErrorHandler.Throw(new Error(rule.type));
                                    }
                                });
                            }
                    });
                } else if (rule.type.startsWith(ClientServerConst.ValidationType_VmInnerValPrefix)) {
                    let f;
                    if (innerValidators) f = innerValidators[rule.type];
                    if (!f) pbErrorHandler.Throw(new NgxArchError('未注册类内部验证器：' + rule.type));
                    validators.push({ validateFunction: f });
                } else if (rule.type !== 'dorefNotNull') {
                    console.log('no validator found for validationType: ' + rule.type);
                }
                if (validators.length > 0) {
                    validators[validators.length - 1].errorMessage = rule.errmsg || pbTranslate.Val(rule.type, rule.param, meta.translate);
                }
            }
        }
        if (validators.length === 0) validators = null;
        return validators;
    }
    private shouldVal(valRule: ValidationRule, valGroups?: string | string[]) {
        let onlyAlways = false;
        let ruleGrps = valRule.groups || [ClientServerConst.VmMeta_DefaultRuleGroup];
        if (ruleGrps.length === 0) ruleGrps = [ClientServerConst.VmMeta_DefaultRuleGroup];

        const belongToDefault=!valRule.groups || ruleGrps.find(g=>g.toLowerCase()===ClientServerConst.VmMeta_DefaultRuleGroup.toLowerCase());
        if (!valGroups && belongToDefault) return true;

        if (typeof valGroups === 'string') valGroups = [valGroups];
        if(!onlyAlways){
            valGroups[valGroups.length] = ClientServerConst.VmMeta_AlwaysRuleGroup;
        }
        let should = false;
        valGroups.forEach((grp, i) => {
            if (ruleGrps.find(g=>g.toLowerCase()===grp.toLowerCase())) {
                should = true;
                return false;
            }
        });
        return should;
    }
    /** 构造指定分组的验证规则 */
    public buildRuleConfig(form: VmForm, valGroups?: string | string[]): FormGroupRuleConfig {
        return this.recursiveBuildRuleConfig(form.pbVmMeta, null, null, valGroups, form.innerValidators) as FormGroupRuleConfig;
    }
    private recursiveBuildRuleConfig(meta: VmMeta, nodeName: string, parentMeta?: VmMeta, valGroups?: string | string[], innerValidators?: {
        [name: string]: ValidatorFn;
    }): ControlRuleConfig | FormGroupRuleConfig {
        const controlsConfig = {} as FormGroupRuleConfig;

        let validators: any[];//这里any指uni-forms的rule对象
        if (nodeName === 'id' && parentMeta && parentMeta.isDORef() && (this.shouldVal(parentMeta.rules.required, valGroups) || this.shouldVal(parentMeta.rules.dorefNotNull, valGroups))) {
            validators = [];
            validators.push(this.validatorRepo.getValidator('dorefNotNullId')());
            meta.rules = {};
            meta.rules.dorefNotNullId = { type: 'dorefNotNullId', groups: parentMeta.rules.dorefNotNull.groups };
        } else {
            validators = this.getValidators(meta, false, valGroups, innerValidators);
        }

        if (meta.properties) {
            for (const prop of Object.keys(meta.properties)) {
                const cfg = this.recursiveBuildRuleConfig(meta.properties[prop], prop, meta, valGroups, innerValidators);
                if (cfg) {
                    controlsConfig[prop] = cfg;
                }
            }
            if (Object.keys(controlsConfig).length === 0) {
                controlsConfig == null;
            }
            return controlsConfig;
        } else {
            return { label: meta.translate, rules: validators };
        }
    }
    private getSenderIdFromUrl(url: string) {
        const parts = url.split('?');
        const pos = parts[0].lastIndexOf('/');
        return pos < 0 ? parts[0] : parts[0].substr(pos + 1);
    }
}

export const pb = new ProjectBaseService();