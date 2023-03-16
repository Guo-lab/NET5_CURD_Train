import { AjaxSubmitConfig, CallActionConfig, ConstructOption, NameValueObj, NgxArchError, Pager, PbAbstractControl, PbRouteConfig, RcResult, ValidateOnOpt, ValidatorFn, VmForm } from './projectbase.type';
import { util } from './UtilHelper';
import { Check } from './Check';
import { pbcWorker } from './ComponentWorker';
import { ComponentBase } from './ComponentBase';
import { ClientServerConst } from './ClientServerConst';
import { VmMeta } from './VmMeta';
import { pbAutoValidator } from './AutoValidator';
import Component from "vue-class-component";
import { pbErrorHandler } from './PBErrorHandler';

@Component
export class VmComponentBase<Tvm = any,Tvminput=any> extends ComponentBase {

    static readonly Event_VmComponentResoveError = 'pbVmComponentResoveError';
    static EVENT_VM_RESOLVED = 'pbVmResolved';

    pbcOpn=new ConstructOption<Tvm>();
    routeParam: NameValueObj={};//只有子类RoutedComponent此变量才有值
    vmTypeName?: string;
    vmInputTypeName?: string|null;
    vmForm?: VmForm;
    vm: Tvm = null as any;
    modelResult: RcResult|null = null;
    innerValidators?: {
        [name: string]: ValidatorFn;
    };

    fv = {} as Tvminput;

    private _resolveUrl: string|null=null;
    private _isRouted = false;
    private onMvReadyFlag:string|null = null;

    /** 子类不要实现此hook <br/>
     * 此处实现使得嵌入组件以created为入口开始处理过程，created将进入onLoad同样处理过程
    */
    created() {
        this.Override_created();
    }
   
    protected Override_created(){
        this.SuperLoad_VmComponentBase();  
    }
   
    /** 在onLoad的最早时执行，设置初始参数 */
    OnOpnInit(opn?:ConstructOption<Tvm>) {
        
    }
    protected InitOpn(vmInputTypeName?: string,
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

        uni.$on(ComponentBase.EVENT_VIEW_READY, (emmittedFrom: ComponentBase) => {
            if (this != emmittedFrom) return;
            if (!this.onMvReadyFlag) {
                this.onMvReadyFlag = ComponentBase.EVENT_VIEW_READY;
            } else if (this.onMvReadyFlag == VmComponentBase.EVENT_VM_RESOLVED) {
                this.OnMVReady();
                this.onMvReadyFlag = null;
            }
        });
        uni.$on(VmComponentBase.EVENT_VM_RESOLVED, (emmittedFrom: ComponentBase) => {
            if (this != emmittedFrom) return;
            if (!this.onMvReadyFlag) {
                this.onMvReadyFlag = VmComponentBase.EVENT_VM_RESOLVED;
            } else if (this.onMvReadyFlag == ComponentBase.EVENT_VIEW_READY) {
                this.OnMVReady();
                this.onMvReadyFlag = null;
            }
        });

        pbcWorker.VmComponentBase_construct(this, this.pbcOpn.autoAttachVmInputToForm);

        this.vmInputTypeName = this.pbcOpn.vmInputTypeName;
        if (this.vmInputTypeName) {
            const meta = this.pb.GetVmMeta(this.vmInputTypeName);
            if (!meta) {
                pbErrorHandler.Throw(new NgxArchError('未找到对应的meta数据：' + this.vmInputTypeName));
            }
            this.recursiveBuildVmValue(meta, this.fv);
        }
    }
    VmComponentBase_onLoad(param?: NameValueObj) {
        if (this.pbcOpn.resolveUrl) {
            this._resolveUrl = this.pbcOpn.resolveUrl;
        } else if (this.isRouted &&(!this.pbcOpn.routeConfig || !this.pbcOpn.routeConfig.noResolve && !this.pbcOpn.routeConfig.noServerAction)) {
            const stack = getCurrentPages();
            const currentPage = stack[stack.length - 1];
            Check.Require(!!currentPage.route,'未找到当前路由');
            const url=currentPage.route!.split('/').pop();
            Check.Require(!!url,'未找到当前url: '+currentPage.route);
            this._resolveUrl = this.Url.MergeRouteParams(url!, param);
        }
        pbcWorker.VmComponentBase_onLoad(this);
    }
    SuperLoad_VmComponentBase() {
        try {
            this.OnOpnInit(this.pbcOpn);
            this.VmComponentBase_constructor();
            this._resolveUrl=null;
            if(this.pbcOpn.resolvedVm){
                this.vm=this.pbcOpn.resolvedVm;
            }
            if(this.pbcOpn.resolvedVm===false) return;
            this.VmComponentBase_onLoad();            
        } catch (e) {
            pbErrorHandler.Throw(e);
        }
    }
    private recursiveBuildVmValue(meta: VmMeta, obj: any) {
        try {
            for (const prop in meta.properties) {
                const propMeta = meta.properties[prop];
                if (propMeta.properties) {
                    this.recursiveBuildVmValue(meta.properties[prop], obj[prop]);
                } else if (propMeta.isDORef()) {
                    obj[prop] = { id: null };
                } else {
                    obj[prop] = null;
                }
            }
        } catch (e) {
            pbErrorHandler.Throw(e);
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
    OnMVReady() {

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
        try{
            pbcWorker.VmComponentBase_ngOnDestroy(this);
        } catch (e) {
            pbErrorHandler.Throw(e);
        }
    }
    /** 仅对nonrouted有效,在resolve出错时被调用 */
    OnResolveError(r: RcResult) {
        uni.$emit(VmComponentBase.Event_VmComponentResoveError, r);
    }

    ResolveVm(vm?: Tvm, bindVmForm = false) {
        try {
            if (vm && !this.vm) this.vm = vm;
            if (this.vm && (this.vm as any)._FromResult) {
                this.modelResult = (this.vm as any)._FromResult;
            } else {
                this.modelResult = util.CreateRcResult('derived');
            }
            if (bindVmForm) {
                this.BindVm();
            }
        } catch (e) {
            pbErrorHandler.Throw(e);
        }
    }
    CustomResolve(): Promise<RcResult>|null {
        return null;
    }
    /** 起到类似routing中resolve的作用的方法 */
    Resolve() {
        try {
            if (this.pbcOpn.resolveUrl) {
                this._resolveUrl = this.pbcOpn.resolveUrl;
            }
            pbcWorker.ResolveAndPrepForm(this);
        } catch (e) {
            pbErrorHandler.Throw(e);
        }
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
        Check.Require(!!this.vmInputTypeName,'未设置vmInputTypeName');
        return this.pb.CreateFormGroup(this.vmInputTypeName!, opt, this.innerValidators);
    }
    /** 该方法主要用于将vm中未对应formcontrol的输入数据关联到vmForm上，这样在ajaxSubmit时关联的输入数据也会一同提交。如常用的listInput */
    AttachVmInputToForm(form: VmForm, inputInVm: any) {
        (form as VmForm).pbVmInput = inputInVm;
    }
    get FormToken() {
        // if (!this.modelResult) {
        //     pbErrorHandler.Throw(new NgxArchError('当前视图中没有经reslove得到的数据，因此无法获得formToken'));
        // }
        return this.modelResult?.data.ViewModelFormToken;
    }
    set FormToken(v: string) {
        if(!this.modelResult) return;
        this.modelResult.data.ViewModelFormToken = v;
    }
    AjaxSubmit(action: string, bodyData?: NameValueObj|null, funcExecuteResult?: ((result: RcResult) => any|Promise<any>)|null, 
        options: AjaxSubmitConfig = {}): Promise<any> {
        try {
            if(this.vmForm){
                this.vmForm.vmFormValidate = ()=>this.VmFormValidate();
            }
            options.url = action;
            options.bodyData = bodyData;
            options.formToken = options.formToken || this.FormToken;
            options.hostComponent = options.hostComponent || this;
            options.ngForm = options.ngForm || this.vmForm as VmForm;

            return this.pb.AjaxSubmit(options, null, null, null, funcExecuteResult);        
        } catch (e) {
            pbErrorHandler.Throw(e);
            throw e;//这句应该永远不会执行到,只是为了编译器检查返回值
        }
    }
    CallAction(action: string, bodyData?: NameValueObj|null, funcExecuteResult?: ((result: RcResult, argsForFunc?: any) => any | Promise<any>)|null,
        options: CallActionConfig = {}): Promise<any> {
            try {
                options.formToken = options.formToken || this.FormToken;
                options.hostComponent = options.hostComponent || this;
                return this.pb.CallAction(action, bodyData, funcExecuteResult, options);
            } catch (e) {
                pbErrorHandler.Throw(e);
                throw e;//这句应该永远不会执行到,只是为了编译器检查返回值
            }
    }
    ReloadModel() {
        try {
            pbcWorker.VmComponentBase_ReloadModel(this);
        } catch (e) {
            pbErrorHandler.Throw(e);
        }
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
    PagingBind(evt: any, action: string) {
        try {
            const pageSize = (this.vm as any).Input.ListInput.Pager.PageSize;
            (this.vm as any).Input.ListInput.Pager = { PageNum: evt, PageSize: pageSize };
            if (!(this.vm as any).Input.ListInput.SelectedValues) {
                delete (this.vm as any).Input.ListInput.SelectedValues;
            }
            const listInput = { Input: { ListInput: (this.vm as any).Input.ListInput } };
            this.AjaxSubmit(action, listInput, r => {
                (this.$refs.paging as any).complete(r.data.ResultList);
            }).then((r: RcResult) => {
                if (r.ok == false) {
                    (this.$refs.paging as any).complete(false);
                }
            }).catch(e => {
                (this.$refs.paging as any).complete(false);
            });
        } catch (e) {
            pbErrorHandler.Throw(e);
        }
    }
    protected VmFormValidate(): string|null {
        return null;
    }
    protected ShouldValidateGroups(): string|string[]|null {
        return null;
    }
    protected RegisterVmInnerValidator(name: string | NameValueObj, validator: ValidatorFn) {
        try {
            Check.Require(!this.vmForm, 'registerVmInnerValidator方法应在OnOpnInit中使用，以保证在创建vmForm之前执行');
            this.innerValidators = this.innerValidators || {};
            if (typeof name === 'string') {
                this.innerValidators[ClientServerConst.ValidationType_VmInnerValPrefix + name] = validator;
            } else {
                for (const prop of Object.keys(name)) {
                    this.innerValidators[ClientServerConst.ValidationType_VmInnerValPrefix + prop] = name[prop];
                }
            }
        } catch (e) {
            pbErrorHandler.Throw(e);
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
    protected GetRouteParam() {
        try {
            Check.Require(!this.isRouted, 'VmComponent使用GetRouteParam()获取当前路由参数，但RoutedComponent应直接使用this.routeParam');
            var node = this.$parent as any;
            while (node) {
                var routeParam = node.routeParam;
                if (routeParam) return routeParam;
                node = node.$parent;
            }
        } catch (e) {
            pbErrorHandler.Throw(e);
        }
    }
}