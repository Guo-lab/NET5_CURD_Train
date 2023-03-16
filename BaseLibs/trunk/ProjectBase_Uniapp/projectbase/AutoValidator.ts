import { ClientServerConst } from './ClientServerConst';
import { pb } from './projectbase.service';
import { PbAbstractControl, PbResolvedErrors, VmForm } from './projectbase.type';
import { pbui } from './projectbase.ui.service';

export interface ErrorsResolvedEvent { control: PbAbstractControl; errors: PbResolvedErrors; }

export class AutoValidator {
    static DefaultTargetNameForForm = 'DefaultTargetNameForForm';

    attach(form: VmForm) {
        form.ref.$on('validate',(errors:any) => {
            form.pbResolvedErrors =errors;
        });
    }
   
    /**
    * validate a form or control. 绑定相应验证规则并执行。
    *  @param: ctl:
    *  @param: valGroups: 不指定则表示不涉及分组验证，'none'表示不验证, 涉及分组验证则必须指出组名，包括缺省组也要指出
    *  @param: partialValid: 要验证的部分子控件名，仅支持一级。缺省时验证form中的所有子控件
    */
    // tslint:disable-next-line: cyclomatic-complexity
    validate(form: VmForm, valGroups?: string | string[]|null, partialValid?: string | string[]|null): Promise<boolean> {
        form.ref.clearValidate([]);
        form.pbResolvedErrors=null;  
        if(!valGroups || valGroups===ClientServerConst.VmMeta_DefaultRuleGroup||valGroups===ClientServerConst.VmMeta_DefaultRuleGroup.toLowerCase()){
            return form.ref.validate().then(()=>{
                return this.runVmFormValidate(form);
            }).catch(()=>{
                return false;
            });
        }else if(valGroups==='none'){
            return Promise.resolve(true);
        }
        const rules=pb.buildRuleConfig(form,valGroups);
        form.ref.setRules(rules);
        return form.ref.validate().then(()=>{
            form.ref.setRules(form.pbRuleConfig);
            return this.runVmFormValidate(form);
        }).catch(()=>{
            form.ref.setRules(form.pbRuleConfig);
            return false;
        });
    }

    private runVmFormValidate(form: VmForm) {
        if (form.vmFormValidate) {
            const err = form.vmFormValidate.apply(form.hostComponent);
            if (err) {
                this.resolveFormError(form, err);
            }
            return !err;
        }
        return true;
    }

    resolveFormError(ctl: PbAbstractControl,errmsg:string){
        if(errmsg){
            ctl.pbResolvedErrors=ctl.pbResolvedErrors||{};
            ctl.pbResolvedErrors.classValidate=ctl.pbResolvedErrors.classValidate||{}
            ctl.pbResolvedErrors.classValidate.errMsg=errmsg;
           // pbui.Alert(errmsg);
        }else{
            if(ctl.pbResolvedErrors&&ctl.pbResolvedErrors.classValidate){
                delete ctl.pbResolvedErrors.classValidate;
            }
            if(ctl.pbResolvedErrors && Object.keys(ctl.pbResolvedErrors).length===0){
                ctl.pbResolvedErrors=null;
            }
        }
    }
}

export const pbAutoValidator = new AutoValidator();

