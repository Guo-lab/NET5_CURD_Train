//import Component,Prop from "vue-class-component";
import { RoutedComponentBase } from 'projectbase/RoutedComponentBase';
import { ConstructOption } from '@/projectbase/projectbase.type';
import { UserEditVM, UserEditVM$EditInput } from '../TR.def';
import { VmComponentBase } from '@/projectbase/VmComponentBase';
import { Component, Prop } from 'vue-property-decorator';

@Component
export default class UserEditEmbeded extends VmComponentBase<UserEditVM,UserEditVM$EditInput> {
    @Prop(Number) prop1!: number;
    @Prop(Object) pvm!: any;

    OnOpnInit(opn: ConstructOption) {
        opn.vmInputTypeName = 'UserEditVM$EditInput';
        var param = this.GetRouteParam();
        opn.resolvedVm=false;
        opn.resolveUrl = 'Edit?Id=' + param.Id;
    }

    save_click() {
        this.AjaxSubmit('Save');
    }
    test(){
        console.log(123);
    }
}