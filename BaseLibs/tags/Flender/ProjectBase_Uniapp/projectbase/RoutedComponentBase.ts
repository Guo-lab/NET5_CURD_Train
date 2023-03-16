import { pbcWorker } from './ComponentWorker';
import { NameValueObj } from './projectbase.type';
import { pbui } from './projectbase.ui.service';
import { VmComponentBase } from './VmComponentBase';
import Component from "vue-class-component";
import { PBInjector } from './PbInjector';
import { BaseAppService } from './BaseAppService';
import { pbErrorHandler } from './PBErrorHandler';
/**
 * use this base for routed components that aren't a layout or frame. non-routed child will use the same route-related data
 */
@Component
export class RoutedComponentBaseNoHook<Tvm = any> extends VmComponentBase<Tvm> {

    //////////routeData: any;
    routeParam: NameValueObj;
    /////////requiredParams: { [name: string]: string };

    guardChange = false;
    changeSaved = true;

    RoutedComponentBaseNoHook_constructor() {
        this.pbcOpn.actLikeSection = false;
        this.isRouted=true;
        this.VmComponentBase_constructor();
        pbcWorker.RoutedComponentBase_construct(this);
    }
    RoutedComponentBaseNoHook_onLoad(param?: NameValueObj) {
        this.OnOpnInit(this.pbcOpn);
        this.RoutedComponentBaseNoHook_constructor();
        this.routeParam = param;
        this.VmComponentBase_onLoad(param);
        pbcWorker.RoutedComponentBase_onLoad(this);
    }
    CanDeactivate() {
        try {
            if (!this.guardChange) {
                return true;
            }
            let changeSaved = this.changeSaved;
            if (!changeSaved) {
                if (this.vmForm) {
                    ///////////////////changeSaved = this.vmForm.pristine;
                } else {
                    return true;
                }
            }
            if (changeSaved) {
                return true;
            }
            return pbui.Confirm('M.Confirm_LeaveWithoutSave');
        } catch (e) {
            pbErrorHandler.Throw(e);
        }
    }

    // // called before onNavBackRefresh() if get back from a nav route with changed data. use this function to sync data on client side.
    // onNavBackChange(changedData: AjaxNavChangedData, changedDataQueue: AjaxNavChangedData[]) { }
    // called after get back from a nav route with cached data by default. use this function to replace the cached data.
    OnNavBackRefresh(backSrc:string) {}
    // // 返回前被调用
    // preNavBack(): boolean | void | null {
    //     return;
    // }

    // /** 此方法仅根据querystring内容判断，未考虑浏览器刷新的情况 */
    // ajaxNavBackFrom() {
    //     const source = this.pb.ajaxNav.backSource(this.route.snapshot);
    //     const isBack = source ? source.startsWith('back:') : null;
    //     //if (!(source && isBack && this.pb.ajaxNav.navStack.length > 1)) return null;
    //     return isBack ? source : null;
    // }
    // /** 此方法仅根据querystring内容判断，未考虑浏览器刷新的情况 */
    // ajaxNavIsBackFrom(substrOfRouteName: string | string[]) {
    //     const src = this.ajaxNavBackFrom();
    //     if (!src) return false;
    //     return this.pb.ajaxNav.isBackFrom(src, substrOfRouteName);
    // }
    // ajaxNavIsBackTo(substrOfRouteName: string | string[]) {
    //     const src = this.pb.ajaxNav.forwardSource();
    //     if (!src) return false;
    //     return this.pb.ajaxNav.isBackTo(src, substrOfRouteName);
    // }
}

@Component
export class RoutedComponentBase<Tvm = any> extends RoutedComponentBaseNoHook<Tvm> {
    onLoad(param?: NameValueObj) {
        try {
            this.RoutedComponentBaseNoHook_onLoad(param);
        } catch (e) {
            pbErrorHandler.Throw(e);
        }
    }
  
    onBackPress(arg: any) {
        try {
            const pages = getCurrentPages();
            const appInstance:BaseAppService=PBInjector.get(PBInjector.InjectToken.AppService);
            appInstance.navInfo.backSrc = pages[pages.length - 1].route;
        } catch (e) {
            pbErrorHandler.Throw(e);
        }
    }
    onShow() {
        try {
            const appInstance: BaseAppService = PBInjector.get(PBInjector.InjectToken.AppService);
            if (appInstance.navInfo.backSrc) {
                this.OnNavBackRefresh(appInstance.navInfo.backSrc);
                appInstance.navInfo.backSrc = null;
            }
        } catch (e) {
            pbErrorHandler.Throw(e);
        }
    }
}

