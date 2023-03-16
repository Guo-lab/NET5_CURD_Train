import { DisplayPipe } from "./pipe/display.pipe";
import Vue from "vue";
import { DictPipe } from "./pipe/dict.pipe";
import { DORefPipe } from "./pipe/doref.pipe";
import { DatePipe } from "./pipe/date.pipe";
import { DecimalPipe } from "./pipe/decimal.pipe";
import { PBInjector } from "./PbInjector";
import { pbui } from "./projectbase.ui.service";
import { pbErrorHandler } from "./PBErrorHandler";

export class BaseAppService {
    navInfo:{backSrc:string}={backSrc:null};//记录页面栈导航相关的信息，如返回的出发点，即从哪里返回等
    constructor() {
        this.RegisterServices();
        this.RegisterFilters();
        PBInjector.register(PBInjector.InjectToken.AppService,this);
    }

    RegisterFilters() {
        Vue.filter('display', DisplayPipe.transform);
        Vue.filter('dict', DictPipe.transform);
        Vue.filter('doref', DORefPipe.transform);
        Vue.filter('date', DatePipe.transform);
        Vue.filter('decimal', DecimalPipe.transform);
    }
    RegisterServices() {

    }
    protected initTranslateService() {

    }

    Application_Error(err: any) {
        this.MarkAsHandledByApplication(err);
        if (pbErrorHandler.AlreadyHandled(err)) return;
        console.error(err);
        let msg = '系统错误,请联系管理员:';
        if (err instanceof Error) {
            msg = msg + err.message;
        } else if (typeof err == 'string'){
            msg = msg + err;
        } else {
            msg = msg + JSON.stringify(err);
        }
        pbui.Alert(msg);
    }

    protected MarkAsHandledByApplication(err: any) {
        if (err && typeof err === 'object') {
            err.pbErrorHandledByApplication = true;
        }
    }
    AlreadyHandled(err: any) {
        return err?.pbErrorHandledByApplication === true;
    }
}
