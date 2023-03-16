import { VmComponentBase } from './VmComponentBase';
import { VmMeta } from './VmMeta';

export class NgxArchError extends Error {
    constructor(msg: string) {
        super(msg);
    }
}
export class ArgumentNullError extends Error {
    constructor(msg: string) {
        super('参数值为null or undefined：' + msg);
    }
}
export interface InterComponentDataSubject {
    srcComponentType?: string;////////////////Type<VmComponentBase>;
    srcId?: any; // identify the src further from the component, ex, which element
    targetId?: any; // identify the target further from the component, ex, which element
}
export interface VmDataSubject extends InterComponentDataSubject {
    vm: any;
    vmTypeName: string;
}
export interface AnyDataSubject extends InterComponentDataSubject {
    subjectName: string;
    anyData: any;
    targetComponentType?: string;////////////////Type<VmComponentBase>;
}
export interface ComponentCallSubject extends InterComponentDataSubject {
    callFunc: Function;
    args?: any;
    targetComponentType?: string;////////////////Type<VmComponentBase>;
}
export class ConstructOption {
    vmInputTypeName?: string;
    autoCreateVmForm?: boolean | ValidateOnOpt = true;
    autoAttachVmInputToForm?: boolean = false;
    regTlistDel?: boolean;
    actLikeSection: boolean = true;
    resolveUrl?: string;
    resolveOpn?: CallActionConfig;
    routeConfig?: PbRouteConfig = {};
}
export interface PbRouteConfig {
    /**
     * unique name for a route as well as the server request url for a resolve.
     * By default, implied by the component name.
     */
    routeName?: string;
    absPath?: string;
    vmInputTypeName?: string | false; // by default, use the vmTypeName to imply. false means no autoform.
    parentCoat?: string | boolean;
    noServerAction?: boolean;
    noResolve?: boolean;
    resultGetter?: any;
    navBackRefresh?: boolean | string | string[] | 'onchange'; // 从哪里返回时自动重进
    requiredParams?: string | string[];
    funcCode?: string;
    guardChange?: boolean;
}
export type VmForm = PbAbstractControl;
/**
 * 服务器获取数据作为route的resolve数据时未得到正确结果
 */
export class ResolveError extends Error {
    static isResolveError(err: Error) {
        return err.name === 'ResolveError';
    }
    constructor() {
        super();
        this.name = 'ResolveError';
    }
}

/** 服务器验证规则数据 */
export interface ValidationRule {
    param?: any;
    type: string;
    groups: string[];
    errmsg?: string;
}
export interface PbValidationErrors { [validationType: string]: { shouldBe?: any; actual?: any;[paramName: string]: any }; }
export type ControlRuleConfig = {
    label?: string;
    validateTrigger?: string;
    rules?: any[];
}
export type FormGroupRuleConfig = { [prop: string]: ControlRuleConfig };

export interface PbAbstractControl {
    pbVmMeta?: VmMeta;
    /** 缺省组验证规则 */
    pbRuleConfig?: ControlRuleConfig | FormGroupRuleConfig;
    value?: any;
    pbResolvedErrors?: NameValueObj;
    pbVmInput?: any; // only for the root FormGroup
    ref?: any; // the $ref.xxx
    controls?: { [name: string]: PbAbstractControl }; // controls in the form control
    innerValidators?: {
        [name: string]: ValidatorFn;
    }; // only for the root FormGroup to register in-class validator
    vmFormValidate?: () => string;
    hostComponent?: VmComponentBase; // only for the root FormGroup to save a pointer to the host controller instance
}
export interface FormArray extends PbAbstractControl {
    namePrefix?: string;
    arrayValidators?: ValidatorFn[];
};
export type ValidatorFn = (rule: any, value: any, data: any, callback: (err?: string) => void) => boolean | Promise<any>;
export interface PbResolvedErrors { [validationType: string]: { targetName: string; errMsg: string }; }
export interface CascadeValidateOnOpt {
    _validateOn?: 'blur' | 'change' | 'submit';
    [prop: string]: ValidateOnOpt; // 'blur' |'change' |'submit' | NameValueObj;//updateOnOpt
}
export type ValidateOnOpt = CascadeValidateOnOpt | 'blur' | 'change' | 'submit';

export interface NameValueObj {
    [key: string]: any;
}

export interface RcResult {
    /** 此接口不可改为class，因为数据来源是从json构造出的静态值，而不是new出来的Object */

    ok: boolean; // false means it's a error result.
    isRcResult?:boolean;//兼容考虑
    command: string;
    data: any;
    isVM?: boolean;
    isVMPatch?: boolean;
    isScrollList?: boolean;
    isMsg?: boolean;
    isData?: boolean;
    isChangedData?: boolean;
    isNoop?: boolean;
    isRedirect?: boolean;
    isAppPage?: boolean;
    isEvent?: boolean;
    isException?: boolean;
    isBizException?: boolean;

    /*
     *  deletedIds和changedData用于对changedData类型的响应结构数据进行结构重整。
     * 服务器返回的结构是innerResult和变化数据都在data下
     * r-data-deletedId
     *       -changedData
     *       -innerResult
     * 重整后客户端代码格式简化为：
     * 1.在回调方法中得到的参数r总是不含changedData的，即如果有innerResult，那么r就是innerResult
     * 2. deletedIds和changedData可直接通过属性获得
     */
    deletedIds?: any | any[];
    changedData?: any;
}

export interface CallActionConfig {
    url?: string;
    qs?: NameValueObj;
    bodyData?: NameValueObj;
    vmBind?: VmComponentBase | boolean;
    processing?: true|false; // 暂时只支持truefalse
    method?: 'GET' | 'POST' | 'NV';
    hostComponent?: VmComponentBase;
    senderId?: string | boolean; // false表示不发送事件通知，true表示从url推算此参数值
    requestCheck?: number | false; // 检查请求间隔ms
    requestCheckEmit?: boolean; // 检测到过频请求时是否发送事件 缺省true.
    formToken?: string; // 一般来说hostComponent里自带表单令牌，但如果没有，可以设置此属性来传递表单令牌
    loginToken?: any|boolean; // 登录令牌缺省为true
    argsForFunc?: any; // argument for funcExecuteResult or ExecuteResult method of the component, and for the latter, value is the element id by default.
    overrideSuper?: boolean;
    executeErrorResult?: boolean;//是否自动处理错误结果，缺省为是
    postJson?: boolean;
}
export interface AjaxSubmitConfig extends CallActionConfig {
    confirm?: string | false;
    confirmSingle?: string;
    confirmMulti?: Object[];
    pbInstruct?: boolean | number | string;
    ngForm?: VmForm | boolean;
    valGroups?: string | string[]; // default to null and validate all.'none' to validate none. other string value to validate the group.
    partialValid?: string | string[]; // 要验证的部分子控件名，仅支持一级。缺省时验证form中的所有子控件
    formValueContainingProp?: string | boolean; // 表单值对象提交时是否使用上级对象包含。 defalut to false表示不使用上级包含.true表示使用'Input'包含。
}
export type AjaxSubmitSimpleConfig = [string, VmForm | boolean, VmComponentBase | boolean];

export function TypeIsAjaxSubmitConfig(obj: AjaxSubmitConfig | AjaxSubmitSimpleConfig): obj is AjaxSubmitConfig {
    return (obj as AjaxSubmitConfig).url !== undefined;
}
export class Pager {
    PageNum: number;
    PageSize: number;
    PageCount: number;
    ItemCount: number;
    PageEmpty: boolean;
    get FromRowNum() {
        return (this.PageNum - 1) * this.PageSize + 1;
    }
    get ToRowNum() {
        return this.PageNum === this.PageCount ? this.ItemCount : this.PageNum * this.PageSize;
    }
}
export class DORef {
    Id: number | string | null = null;
    RefText: string = '';
}
export interface ListInput {
    OrderExpression?: string;
    Pager: Pager;
    SelectedValues: number[] | string[];
    ForScroll?: boolean;
}
export interface ProjectHierarchy {
    Modules: string[];
}
