import Vue from 'vue';
import { BaseAppService } from './BaseAppService';
import { PBInjector } from './PbInjector';
import { pbui } from './projectbase.ui.service';
export class PBErrorHandler {
    static NETWORK_ERROR = 'pbNetWorkError';

    Throw(err: any) {
        if (this.AlreadyHandled(err)) throw err;
        const handled=this.CallApplicationErrorHandler(err);
        if (handled) throw err;

        let msg = err;
        if (err.message) {
            msg = err.message;
        } 
        if (!Vue.config.productionTip) {
            pbui.Alert(JSON.stringify(msg));
        }
        console.error(err);
        if (typeof err != 'string') {
            err.pbErrorHandled = true;
        }
        throw err;
    }
    ThrowAsNetWorkError(err: any) {
        this.MarkAsNetWorkError(err);
        this.Throw(err);
    }
    IsNetWorkError(err: any) {
        return err?.name === PBErrorHandler.NETWORK_ERROR;
    }
    MarkAsNetWorkError(err: any) {
        if (err && typeof err === 'object') {
            err.name = PBErrorHandler.NETWORK_ERROR;
        }
    }
    AlreadyHandled(err: any) {
        return err?.pbErrorHandled === true;
    }
    private CallApplicationErrorHandler(err: any) {
        const appInstance: BaseAppService = PBInjector.get(PBInjector.InjectToken.AppService);
        appInstance.Application_Error(err);
        return appInstance.AlreadyHandled(err);
    }
}

export const pbErrorHandler = new PBErrorHandler();