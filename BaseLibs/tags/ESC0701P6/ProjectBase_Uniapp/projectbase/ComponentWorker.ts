// tslint:disable: member-ordering

import { pbAutoValidator } from './AutoValidator';
import { ComponentBase } from './ComponentBase';
import { pbErrorHandler } from './PBErrorHandler';
import { pb } from './projectbase.service';
import { CallActionConfig, PbAbstractControl, RcResult, ResolveError } from './projectbase.type';
import { pubsub } from './pubsub.service';
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
        let promise =this.ResolveAndPrepForm(thisC);
        if (!promise) {
            thisC.OnInit();
            return;
        }
        promise.then(() => thisC.OnInit())
            .catch(e => {
                pbErrorHandler.Throw(e);
            });
    }
    ResolveAndPrepForm(thisC: VmComponentBase) {
        let promise = this.VmComponentBase_Resolve(thisC);
        if (!promise) {
            this.InternalAfterResolve(thisC,true);
            return;
        }
        return promise.then((r: RcResult) => {
            thisC.modelResult = r;
            if (thisC.modelResult && r.isVM) {
                thisC.vm = pb.GetVm(thisC.modelResult);
                thisC.vmTypeName = pb.GetVmTypeName(thisC.modelResult);
            }
            this.InternalAfterResolve(thisC,r.ok);
        }).catch (e => {
            pbErrorHandler.Throw(e);
        });
    } 
    private InternalAfterResolve(thisC: VmComponentBase, rcResultOk: boolean) {
        if (rcResultOk) {
            const hasInputType = !!thisC.vmInputTypeName;
            if (thisC.vmTypeName && thisC.pbcOpn.autoCreateVmForm) {
                if (!thisC.vmInputTypeName) {
                    thisC.vmInputTypeName = thisC.pb.CheckVmInputType(thisC.vmTypeName);
                }
                if (thisC.vmInputTypeName && hasInputType && !thisC.vmForm) {
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
        }
        uni.$emit(VmComponentBase.EVENT_VM_RESOLVED,thisC);
    }
    /** resolve过程 */
    VmComponentBase_Resolve(thisC: VmComponentBase) {
        let promise = thisC.ResolveUrl ? thisC.ResolveUrl : thisC.CustomResolve();
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
                pbErrorHandler.Throw(new Error('AjaxSubmit not correctly performed to return a promise'));
            }
        }

        (promise as Promise<any>).then((r: RcResult) => {
            if (!r) {
                thisC.OnResolveError(r);
                pbErrorHandler.Throw(new ResolveError());
            } else if (r.ok === false) {
                
            }
            thisC.OnResolveEnd();
        }).catch (e => {
            pbErrorHandler.Throw(e);
        });;
        return promise as Promise<RcResult>;
    }
    VmComponentBase_ReloadModel(thisC: VmComponentBase) {
        let promise = this.VmComponentBase_Resolve(thisC);
        if (!promise) {
            thisC.OnInit();
            return;
        }
        promise.then((r: RcResult) => {
            if (!r.ok || !r.isVM) return;
            thisC.modelResult = r;
            thisC.vm = pb.GetVm(thisC.modelResult);
            thisC.BindVm();
            thisC.ResetForm(thisC.vmForm as PbAbstractControl);
            thisC.OnInit();
            thisC.OnReloadModel();
        }).catch(e => {
            pbErrorHandler.Throw(e);
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

    Emit(thisC: ComponentBase, eventName: string, event?: any, srcId?: string) {
        pubsub.provideAnyData(event, eventName, null, srcId);
    }

    On(thisC: ComponentBase, eventName: string, func: (event: any) => void, srcId?: string) {
        pubsub.subscribeAnyData(eventName, dataSubject => func(dataSubject.anyData), dataSubject => dataSubject.srcId === srcId);
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