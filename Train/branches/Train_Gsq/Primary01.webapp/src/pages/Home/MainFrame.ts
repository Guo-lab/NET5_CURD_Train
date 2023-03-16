import Component from 'vue-class-component';
import { RoutedComponentBase } from 'projectbase/RoutedComponentBase';
import { ConstructOption } from '@/projectbase/projectbase.type';

@Component
export default class MainFrame extends RoutedComponentBase {
    Greeting = 'hhhh';
    i = 1;
    OnOpnInit(opn: ConstructOption) {
        opn.routeConfig.noResolve = true;
    }
    btn_click() {
        this.Greeting = 'ddddddddd';
    }
}
