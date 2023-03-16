import { AjaxSubmitConfig, CallActionConfig, ConstructOption, NameValueObj, NgxArchError, PbAbstractControl, PbRouteConfig, RcResult, ValidateOnOpt, ValidatorFn, VmForm } from './projectbase.type';
import { util } from './UtilHelper';
import { Check } from './Check';
import { pbcWorker } from './ComponentWorker';
import { ComponentBase } from './ComponentBase';
import { ClientServerConst } from './ClientServerConst';
import { VmMeta } from './VmMeta';
import { pbAutoValidator } from './AutoValidator';
import Component from "vue-class-component";

@Component
export class VmComponentBase<Tvm = any> extends ComponentBase {

    static readonly Event_VmComponentResoveError = 'pbVmComponentResoveError';

    pbcOpn=new ConstructOption();

    vmTypeName: string;
    vmInputTypeName: string;
    vmForm: VmForm;
    vm: Tvm = null as any;
    modelResult: RcResult = null as any as RcResult;
    innerValidators: {
        [name: string]: ValidatorFn;
    };

    fv = {} as any;

    private _resolveUrl: string;
    private _isRouted=false;
   // private _constructorExecuted = false;
    // constructor() {//如果子类继承自RoutedComponentBase,则该构建器不会执行
    //     super();
    //     //this._constructorExecuted = true;
    // }
    /** 在onLoad的最早时执行，设置初始参数 */
    OnOpnInit(opn?:ConstructOption) {
        
    }
    InitOpn(vmInputTypeName?: string,
        autoCreateVmForm: boolean | ValidateOnOpt = true,
        autoAttachVmInputToForm = false,
        regTlistDel?: boolean,
        actLikeSection = true) {
        // if (!this._constructorExecuted && this.pbcOpn) {
        //     throw new NgxArchError('设置初始参数的方法只可以调用一次');
        // }
        // if (!this._constructorExecuted) {
        //     this.pbcOpn = new ConstructOption();
        // }
        this.pbcOpn.vmInputTypeName = vmInputTypeName;
        this.pbcOpn.autoCreateVmForm = autoCreateVmForm;
        this.pbcOpn.autoAttachVmInputToForm = autoAttachVmInputToForm;
        this.pbcOpn.regTlistDel = regTlistDel;
        this.pbcOpn.actLikeSection = actLikeSection;
    }
    VmComponentBase_constructor() {
        Check.Require(this.pbcOpn.autoAttachVmInputToForm ? !!this.pbcOpn.autoCreateVmForm : true, 'autoCreateVmForm为false即不自动创建VmForm时不能设置autoAttachVmInputToForm为true');

        pbcWorker.VmComponentBase_construct(this, this.pbcOpn.autoAttachVmInputToForm);

        this.vmInputTypeName = this.pbcOpn.vmInputTypeName;
        if (this.vmInputTypeName) {
            const meta = this.pb.GetVmMeta(this.vmInputTypeName);
            if (!meta) {
                throw new NgxArchError('未找到对应的meta数据：' + this.vmInputTypeName);
            }
            this.recursiveBuildVmValue(meta, this.fv);
        }
    }
    VmComponentBase_onLoad(param?: NameValueObj) {
        this.OnOpnInit(this.pbcOpn);
        if (this.pbcOpn.resolveUrl) {
            this._resolveUrl = this.pbcOpn.resolveUrl;
        } else if (this.isRouted &&(!this.pbcOpn.routeConfig || !this.pbcOpn.routeConfig.noResolve && !this.pbcOpn.routeConfig.noServerAction)) {
            const stack = getCurrentPages();
            const currentPage = stack[stack.length - 1];
            this._resolveUrl = this.Url.MergeRouteParams(currentPage.route.split('/').pop(), param);
        }
        pbcWorker.VmComponentBase_onLoad(this);
    }
    SuperLoad_VmComponentBase(param?: NameValueObj,resolvedVm?:Tvm){
        this.VmComponentBase_constructor();
        this._resolveUrl=null;
        this.vm=resolvedVm;
        this.VmComponentBase_onLoad(param);
    }
    recursiveBuildVmValue(meta:VmMeta,obj:any){
        for(const prop in meta.properties){
            const propMeta=meta.properties[prop];
            if(propMeta.properties){
                this.recursiveBuildVmValue(meta.properties[prop],obj[prop]);
            }else if(propMeta.isDORef()){
                obj[prop]={id:null};
            }else{
                obj[prop]=null;
            }
        }
    }
    protected get isRouted() {
        return this._isRouted;
    }
    protected set isRouted(v:boolean) {
        this._isRouted=v;
    }
    get ResolveReady() {
        return !!this.vm;
    }

    get ResolveUrl() {
        return this._resolveUrl;
    }
    OnInit() {

    }
    OnReloadModel() {
        
    }
    OnResolveStart() {
        uni.showLoading({});
    }
    OnResolveEnd() {
        uni.hideLoading();
    }
    VmComponentBase_onUnload() {
        pbcWorker.VmComponentBase_ngOnDestroy(this);
    }
    /** 仅对nonrouted有效,在resolve出错时被调用 */
    OnResolveError(r: RcResult) {
        uni.$emit(VmComponentBase.Event_VmComponentResoveError, r);
    }
    /** 起到类似routing中resolve的作用的方法 */
    ResolveVm(vm?:Tvm,bindVmForm=false): Promise<RcResult> {
        if (this.vm && (this.vm as any)._FromResult) {
            this.modelResult = (this.vm as any)._FromResult;
        } else {
            this.modelResult = util.CreateRcResult('derived');
        }
        if(bindVmForm){
            this.BindVm(); 
        }
        return null;
    }
    _testing_createLocalViewResult(objAsVm:any){
        const r=util.CreateRcResult(ClientServerConst.Command_ServerVM,{},true);
        r.data={ViewModel:objAsVm,ViewModelTypeName:this.vmTypeName};
        objAsVm._FromResult=r;
        pbcWorker._testing_Init(this,r);
    }

    BindVm(vm?: any) {
        if (vm) {
            this.vm = vm;
        }
        if (this.vmForm) {
            const Input = (this.vm as any).Input || this.vm;
            this.pb.SetFormValue(this.vmForm, Input);// 绑定数据更新
            this.fv = this.vmForm.value;
            if (this.pbcOpn.autoAttachVmInputToForm) {
                this.AttachVmInputToForm(this.vmForm, Input);
            }
        }
    }

    /**
     * 在pb.ajaxSubmit合并参数后但未执行其它逻辑前调用此方法。
     * @param ajaxConfig 合并后的参数，包括传参时传的ajaxSubmitConfig对象和url、form。不包括未显示传参的和hostComponentInstance。
     * @returns 是否执行pb.ajaxSubmit
     */
    OnAjaxSubmit(config: AjaxSubmitConfig): boolean {
        return true;
    }
    CreateVmForm() {
        let opt = typeof (this.pbcOpn.autoCreateVmForm) === 'boolean' ? null : this.pbcOpn.autoCreateVmForm;
        return this.pb.CreateFormGroup(this.vmInputTypeName, opt, this.innerValidators);
    }
    /** 该方法主要用于将vm中未对应formcontrol的输入数据关联到vmForm上，这样在ajaxSubmit时关联的输入数据也会一同提交。如常用的listInput */
    AttachVmInputToForm(form: VmForm, inputInVm: any) {
        (form as VmForm).pbVmInput = inputInVm;
    }
    get FormToken() {
        if (!this.modelResult) {
            throw new NgxArchError('当前视图中没有经reslove得到的数据，因此无法获得formToken');
        }
        return this.modelResult.data.ViewModelFormToken;
    }
    set FormToken(v: string) {
        this.modelResult.data.ViewModelFormToken = v;
    }
    AjaxSubmit(action: string, bodyData?: NameValueObj, funcExecuteResult?: (result: RcResult) => any, options: AjaxSubmitConfig={}) {
        const err=this.VmFormValidate();
        pbAutoValidator.resolveFormError(this.vmForm,err);
        if(err){ 
            return;
        }
        options.url = action;
        options.bodyData = bodyData;
        options.formToken = options.formToken || this.FormToken;
        options.hostComponent = options.hostComponent || this;
        options.ngForm = options.ngForm || this.vmForm as VmForm;

        this.pb.AjaxSubmit(options, null, null, null, funcExecuteResult);
    }
    CallAction(action: string, bodyData?: NameValueObj, funcExecuteResult?: (result: RcResult, argsForFunc?: any) => any | Promise<any>,
        options: CallActionConfig = {}): Promise<any> {
        options.formToken = options.formToken || this.FormToken;
        options.hostComponent = options.hostComponent || this;
        return this.pb.CallAction(action, bodyData, funcExecuteResult, options);
    }
    ReloadModel() {
        pbcWorker.VmComponentBase_ReloadModel(this);
    }
    ResetForm(form: VmForm, opts?: { clear?: boolean; include?: string | string[]; exclude?: string | string[]; initValue?: NameValueObj }) {
        // Check.Require(!!form);
        // if (!opts) {
        //     form.markAsPristine();
        //     form.markAsUntouched();
        //     this.pb.clearResolvedErrors(form as PbAbstractControl);
        //     return;
        // }
        // let clear = opts.clear;
        // if (clear !== false) clear = true;
        // if (clear && !opts.initValue) {
        //     opts.initValue = {};
        // }
        // let ctrl: PbAbstractControl;
        // if (opts.include) {
        //     let include = opts.include;
        //     if (typeof include === 'string') include = [include];
        //     for (let name of include) {
        //         ctrl = form.get(name) as PbAbstractControl;
        //         Check.require(!!ctrl);
        //         this.clearFormControl(ctrl, clear, opts.initValue[name]);
        //     }
        // } else if (opts.exclude) {
        //     let exclude = opts.exclude;
        //     if (typeof exclude === 'string') exclude = [exclude];
        //     for (let name of Object.keys(form.controls)) {
        //         if (!exclude.includes(name)) {
        //             ctrl = form.get(name) as PbAbstractControl;
        //             this.clearFormControl(ctrl, clear, opts.initValue[name]);
        //         }
        //     }
        // } else {
        //     for (let name of Object.keys(form.controls)) {
        //         if (name in opts.initValue) {
        //             ctrl = form.get(name) as PbAbstractControl;
        //             this.clearFormControl(ctrl, clear, opts.initValue[name]);
        //         }
        //     }
        // }
    }

    protected VmFormValidate(): string {
        return null;
    }
    protected ShouldValidateGroups(): string|string[] {
        return null;
    }
    protected RegisterVmInnerValidator(name: string|NameValueObj, validator: ValidatorFn) {
        Check.Require(!this.vmForm, 'registerVmInnerValidator方法应在OnOpnInit中使用，以保证在创建vmForm之前执行');
        this.innerValidators = this.innerValidators || {};
        if (typeof name === 'string') {
            this.innerValidators[ClientServerConst.ValidationType_VmInnerValPrefix + name] = validator;
        } else {
            for (const prop of Object.keys(name)) {
                this.innerValidators[ClientServerConst.ValidationType_VmInnerValPrefix + prop] = name[prop];
            }
        }
    }
    private clearFormControl(ctrl: PbAbstractControl, clear: boolean, initValue: any) {
        // if (clear) {
        //     if (ctrl.pbVmMeta.isDORef() && !initValue) {
        //         ctrl.setValue({ id: null });
        //     } else {
        //         ctrl.setValue(initValue === void 0 ? null : initValue, { emitEvent: false });
        //     }
        // }
        // ctrl.markAsPristine();
        // ctrl.markAsUntouched();
        // this.pb.clearResolvedErrors(ctrl);
    }
}