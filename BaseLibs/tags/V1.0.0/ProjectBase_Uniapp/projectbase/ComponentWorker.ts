// tslint:disable: member-ordering

import { pbAutoValidator } from './AutoValidator';
import { pb } from './projectbase.service';
import { CallActionConfig, PbAbstractControl, RcResult, ResolveError } from './projectbase.type';
import { RoutedComponentBase, RoutedComponentBaseNoHook } from './RoutedComponentBase';
import { urlHelper } from './UrlHelper';
import { VmComponentBase } from './VmComponentBase';


/**
 * 将Component基类的逻辑提到这里，便于测试
 */
export class ComponentWorker {
    static ResolverAsSender = 'pbResolverAsSender';
    static Event_ReloadModel = 'pbReloadModel';

    VmComponentBase_construct(thisC: VmComponentBase,
        attachVmInputToForm = false) {

    }

    VmComponentBase_onLoad(thisC: VmComponentBase) {
        let promise = this.VmComponentBase_Resolve(thisC);
        if (!promise) {
            thisC.OnInit();
            return;
        }
        promise.then((r: RcResult) => {
            thisC.modelResult = r;
            if (thisC.modelResult) {
                thisC.vm = pb.GetVm(thisC.modelResult);
                thisC.vmTypeName = pb.GetVmTypeName(thisC.modelResult);
            }
            const hasInputType = !!thisC.vmInputTypeName;
            if (thisC.vmTypeName && thisC.pbcOpn.autoCreateVmForm) {
                if (!thisC.vmInputTypeName) {
                    thisC.vmInputTypeName = thisC.pb.CheckVmInputType(thisC.vmTypeName);
                }
                if (thisC.vmInputTypeName && hasInputType) {
                    // if (thisC.vmInputTypeName === ProjectBaseService.MetaTypeNameForListInput) {
                    //     throw new NgxArchError('不能对ListInput类型自动创建表单，请将autoCreateVmForm设为false.此错误很可能是因为未正确设置vmInputTypeName');
                    // }
                    thisC.vmForm = thisC.CreateVmForm() as PbAbstractControl;
                    this.prepVmForm(thisC);
                }
            }

            if (thisC.vmForm) {
                (thisC.vmForm as PbAbstractControl).hostComponent = thisC;
            }
            thisC.OnInit();
        }).catch(()=>void 0);
    }
    /** resolve过程 */
    VmComponentBase_Resolve(thisC: VmComponentBase) {
        let promise = thisC.ResolveUrl ? thisC.ResolveUrl : thisC.ResolveVm();
        if (!promise) {
            return;
        }
        thisC.OnResolveStart();
        if (thisC.ResolveUrl) {
            const url = urlHelper.Action(thisC.ResolveUrl);
            let opn: CallActionConfig = thisC.pbcOpn.resolveOpn || { method: 'GET', senderId: false };
            opn.method = opn.method || 'GET';
            opn.senderId = opn.senderId || false;
            promise = thisC.pb.CallAction(url, null, null, opn);
            if (!promise) {
                throw new Error('AjaxSubmit not correctly performed to return a promise');
            }
        }

        (promise as Promise<any>).then((r: RcResult) => {
            if (!r) {
                thisC.OnResolveError(r);
                throw new ResolveError();
            } else if (r.ok === false) {
                
            }
            thisC.OnResolveEnd();
        });
        return promise as Promise<RcResult>;
    }
    VmComponentBase_ReloadModel(thisC: VmComponentBase) {
        let promise = this.VmComponentBase_Resolve(thisC);
        if (!promise) {
            thisC.OnInit();
            return;
        }
        promise.then((r: RcResult) => {
            thisC.modelResult = r;
            thisC.vm = pb.GetVm(thisC.modelResult);
            thisC.BindVm();
            thisC.ResetForm(thisC.vmForm as PbAbstractControl);
            thisC.OnInit();
            thisC.OnReloadModel();
        });
    }
    private prepVmForm(thisC: VmComponentBase) {
        if (thisC.$refs.vmForm) {
            thisC.vmForm.ref = thisC.$refs.vmForm;
            pbAutoValidator.attach(thisC.vmForm);
            thisC.BindVm();
            thisC.vmForm.ref.setRules(thisC.vmForm.pbRuleConfig);
            uni.hideLoading();
        } else {
            setTimeout(() => this.prepVmForm(thisC), 100);
        }
    }
    VmComponentBase_ngOnDestroy(thisC: VmComponentBase) {
        // 因为重复使用form实例所以此处清理一下
        /////////////////////if (thisC.vmForm) (thisC.vmForm as PbAbstractControl).hostComponent = null;
    }

    RoutedComponentBase_construct(thisC: RoutedComponentBase | RoutedComponentBaseNoHook) {

    }

    RoutedComponentBase_onLoad(thisC: RoutedComponentBase | RoutedComponentBaseNoHook) {

    }

    _testing_Init(thisC: VmComponentBase,r:RcResult){
        thisC.modelResult = r;
        if (thisC.modelResult) {
            thisC.vm = pb.GetVm(thisC.modelResult);
            thisC.vmTypeName = pb.GetVmTypeName(thisC.modelResult);
        }
        if (thisC.pbcOpn.autoCreateVmForm) {
                thisC.vmForm = thisC.CreateVmForm() as PbAbstractControl;
                this.prepVmForm(thisC);
        }

        if (thisC.vmForm) {
            (thisC.vmForm as PbAbstractControl).hostComponent = thisC;
        }
    }

}

export const pbcWorker = new ComponentWorker();