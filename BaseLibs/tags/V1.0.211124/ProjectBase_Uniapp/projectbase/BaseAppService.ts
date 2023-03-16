import { DisplayPipe } from "./pipe/display.pipe";
import Vue from "vue";
import { DictPipe } from "./pipe/dict.pipe";
import { DORefPipe } from "./pipe/doref.pipe";
import { DatePipe } from "./pipe/date.pipe";
import { DecimalPipe } from "./pipe/decimal.pipe";

export class BaseAppService {

    constructor() {
        this.RegisterServices();
        this.RegisterFilters();
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

}
