import Component from "vue-class-component";
import { RoutedComponentBase } from "projectbase/RoutedComponentBase";
import { UserListVM } from "../TR.def";
import { ConstructOption } from "@/projectbase/projectbase.type";
import UserEditEmbeded from "./EditEmbeded.vue";
import TUserEditEmbeded from "./EditEmbeded";

@Component({
  components: { UserEditEmbeded },
})
export default class UserList extends RoutedComponentBase<UserListVM> {
t={
  "ViewModel": {
    "Input": {
      "Id": 10,
      "Code": "Rainy9",
      "Name": "ra9",
      "Age": 9,
      "BirthDate": null,
      "Salary": 9,
      "Rank": 9,
      "Active": true
    }
  },
  "ViewModelTypeName": "UserEditVM",
  "ViewModelFormToken": null
} as any;

  OnMVReady(){
    let t=this.$refs.myEmbeded;
  //  (PBInjector.get('signalr') as SignalRService).CallMe();
  }
  btn_resolve(){
    let t=this.$refs.myEmbeded as TUserEditEmbeded;
    t.Resolve();
  }
}

