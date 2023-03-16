import Component from "vue-class-component";
import { RoutedComponentBase } from "projectbase/RoutedComponentBase";
import { ConstructOption, RcResult } from "@/projectbase/projectbase.type";
import { UserEditVM,UserEditVM$EditInput } from "../TR.def";

@Component
export default class UserEdit extends RoutedComponentBase<UserEditVM,UserEditVM$EditInput> {
    
  OnOpnInit(opn:ConstructOption){
    opn.vmInputTypeName='UserEditVM$EditInput';
    this.RegisterVmInnerValidator('ValAge',
                     (rule: any, value: any, data: any)=>{
                 return value>10;
    });
  }
  OnInit(){
      console.log('ffff');
    console.log(this.vm);
    
  }

  btn_Click(){
console.log(this.fv);
  }
  save_click() {
    this.AjaxSubmit('Save',null,(r:RcResult)=>{
        console.log(111);
    });
  }
  VmFormValidate(){
    return null;//this.fv.Code;
  }
}

