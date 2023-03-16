import { DisplayPipe } from "./pipe/display.pipe";
import Vue from "vue";
import { DictPipe } from "./pipe/dict.pipe";
import { DORefPipe } from "./pipe/doref.pipe";
import { DatePipe } from "./pipe/date.pipe";
import { DecimalPipe } from "./pipe/decimal.pipe";
import { PBInjector } from "./PbInjector";
import { pbui } from "./projectbase.ui.service";
import { pbErrorHandler } from "./PBErrorHandler";
import { util } from "./UtilHelper";
import { NameValueObj, NavInfo } from "./projectbase.type";

export class BaseAppService {

    static KEY_APPSTATE_STORAGE = 'pbAppStateForDevRefresh';
    static PREFIX_GLOBAL_DATA_STORAGE = 'pbGlobalData.';

    navInfo: NavInfo = { backSrc: null };

    protected enableClientLog = false;

    constructor() {
        this.RegisterServices();
        this.RegisterFilters();
        PBInjector.register(PBInjector.InjectToken.AppService, this);
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
    Application_Start() {
        if (!Vue.config.productionTip) {
            const savedState = uni.getStorageSync(BaseAppService.KEY_APPSTATE_STORAGE);
            if (savedState) {
                this.OnDevRefresh(savedState);
            }
        }
    }

    Application_Error(err: any) {
        this.MarkAsHandledByApplication(err);
        
        if (this.ShouldIgnore(err)) return;

        if (this.enableClientLog) {
            if (pbErrorHandler.IsNetWorkError(err)) {
                //网络错误不记日志
            } else if (typeof (err) === 'string') {
                util.AddLog(err);
            } else if (err.stack) {
                util.AddLog(err.stack);
            } else {
                util.AddLog(JSON.stringify(err));
            }
        }
        
        if (pbErrorHandler.AlreadyHandled(err)) return;
        console.error(err);
        let msg = '系统错误,请联系管理员:';
        if (err.message) {
            msg = msg + err.message;
        } else if (typeof err == 'string'){
            msg = msg + err;
        } else {
            msg = msg + JSON.stringify(err);
        }
        pbui.Alert(msg);
    }
    protected ShouldIgnore(err: any) {
        if (err.message && err.message.indexOf('plus is not defined') >= 0) return true;
        return false;
    }
    protected MarkAsHandledByApplication(err: any) {
        if (err && typeof err === 'object') {
            err.pbErrorHandledByApplication = true;
        }
    }
    AlreadyHandled(err: any) {
        return err?.pbErrorHandledByApplication === true;
    }

    SaveAppStateForDevRefresh(stateObj?: any) {
        if (Vue.config.productionTip) return;
        stateObj = stateObj || this;
        let saved = uni.getStorageSync(BaseAppService.KEY_APPSTATE_STORAGE);
        let merged = util.MergeDeep(saved, stateObj);
        uni.setStorageSync(BaseAppService.KEY_APPSTATE_STORAGE, merged);
    }
    ClearAppStateForDevRefresh() {
        if (Vue.config.productionTip) return;
        uni.removeStorageSync(BaseAppService.KEY_APPSTATE_STORAGE);
    }
    /**
     * 子类在此处将希望保持的全局数据进行恢复
     * @param stateObj 默认为app对象
     */
    OnDevRefresh(stateObj: any) {
        
    }
    SaveGlobalData(key: string, data?: NameValueObj, merge = true) {
        const key0 = BaseAppService.PREFIX_GLOBAL_DATA_STORAGE + key;
        let merged;
        if (merge) {
            const saved = uni.getStorageSync(key0)||{};
            merged = util.MergeDeep(saved, data);
        } else {
            uni.removeStorageSync(key0);
            merged = data;
        }
        uni.setStorageSync(key0, merged);
    }
    GetGlobalData(key: string) {
        return uni.getStorageSync(BaseAppService.PREFIX_GLOBAL_DATA_STORAGE + key);
    }
}
