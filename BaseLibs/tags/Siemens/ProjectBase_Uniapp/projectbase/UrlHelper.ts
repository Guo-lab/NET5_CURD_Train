import { BaseAppService } from './BaseAppService';
import { Check } from './Check';
import { ClientServerConst } from './ClientServerConst';
import { PBInjector } from './PbInjector';
import { NameValueObj } from './projectbase.type';
import { pbui } from './projectbase.ui.service';
import { pbConfig } from './ProjectBaseConfig';
import { RoutedComponentBase } from './RoutedComponentBase';
import { util } from './UtilHelper';

export class UrlHelper {

    ////////////static ClientOnlyParam_Prefix = 'clientOnly$';
    private static appParams: NameValueObj;

    static SetAppParams(appParams: NameValueObj) {
        UrlHelper.appParams = appParams;
    }
    NavRoot(anyUrl: string, params?: NameValueObj, options?: UniApp.ReLaunchOptions) {
        const url = this.Link(anyUrl, params);
        options = options || { url };
        options.url = url;
        uni.reLaunch(options);
    }
    NavForward(anyUrl: string, params?: NameValueObj, options?: UniApp.NavigateToOptions,forwardData?:any) {
        const url = this.Link(anyUrl, params);
        options = options || { url };
        options.url = url;
        if (forwardData) {
            const appInstance: BaseAppService = PBInjector.get(PBInjector.InjectToken.AppService);
            appInstance.navInfo.forwardData = forwardData;
        }
        uni.navigateTo(options);
    }
    NavBack(backData?:any,options?: UniApp.NavigateBackOptions) {
        options = options || {};
        const appInstance: BaseAppService = PBInjector.get(PBInjector.InjectToken.AppService);
        appInstance.navInfo.backData = backData;
        uni.navigateBack(options);
    }
    NavRefresh(options?: UniApp.RedirectToOptions) {
        const pages = getCurrentPages();
        const url = '/'+pages[pages.length - 1].route;
        options = options || { url };
        options.url = this.MergeRouteParams(url, (pages[pages.length - 1] as RoutedComponentBase).routeParam);
        uni.redirectTo(options);
    }
    /** 在面包屑导航链上跳到任意节点。导航链上目的节点前面的节点保留，后面的移除 */
    NavTo(anyUrl: string, params?: NameValueObj, options?: UniApp.RedirectToOptions) {
        let urlonly = this.Link(anyUrl);
        urlonly = urlonly.substring(1);
        const pages = getCurrentPages();
        let i = pages.length - 1;
        let page = pages[i];
        while (i > 0 && page && !page.route.startsWith(urlonly)) {
            i--;
            page = pages[i];
        }
        Check.Require(!!page, '未找到相应的page，url=' + anyUrl);

        const url = params ? this.Link(anyUrl, params) : this.MergeRouteParams(page.route,(page as RoutedComponentBase).routeParam);
        const step = pages.length - 1 - i;
        options = options || { url };
        options.url = url;
        uni.navigateBack({
            delta: step,
            success: () => {
                setTimeout(() => uni.redirectTo(options),100);
            }
        });
    }

    Link(anyUrl: string, params?: NameValueObj, pbFunc?: true | string) {
        let _params = params || {};
        _params = { ..._params, ...UrlHelper.appParams };
        let _anyUrl: string = anyUrl;////////// this.checkPbFunc(anyUrl, _params, pbFunc);
        _anyUrl = _anyUrl ? this.generateUrl(_anyUrl, null, true) : null;

        /*******************
        const clientOnly = _params[UrlHelper.ClientOnlyParam_Prefix];
        if (clientOnly) {
            for (const prop of Object.keys(clientOnly)) {
                _params[UrlHelper.ClientOnlyParam_Prefix + prop] = clientOnly[prop];
            }
            delete _params[UrlHelper.ClientOnlyParam_Prefix];
        }
        */
        return _anyUrl ? this.MergeRouteParams(_anyUrl, _params) : null;
    }

  Action(anyUrl: string, params?: NameValueObj) {
        params = params || {};
        params[ClientServerConst.KeyForViewModelOnly] = true;
        params[ClientServerConst.KeyForKeepDateAsNumber] = true;
        if (pbui.IsSmallScreen) {
            params[ClientServerConst.KeyForSmallScreen] = true;
        }
        return this.generateUrl(anyUrl, params, false);
    }
    private getFuncCodeFromUrl(anyUrl: string) {
        let funccode;
        if (anyUrl && anyUrl !== '') {
            funccode = this.Action(anyUrl);
            let pos0 = funccode.indexOf('?');
            if (pos0 > 0) {
                funccode = funccode.substring(0, pos0);
            }
            pos0 = funccode.indexOf(';');
            if (pos0 > 0) {
                funccode = funccode.substring(0, pos0);
            }
            const keys = funccode.replace('$', '/').split('/');
            const l = keys.length;
            funccode = keys[l - 2] + '.' + keys[l - 1];
        }
        return funccode;
    }
    MergeRouteParams(url: string, params?: NameValueObj) {
        const urlfull = url.split('?');
        let qs = util.Obj2Qs(params);
        if (qs !== '') {
            const url1 = urlfull[1] || '';
            qs = util.MergeQS(url1, qs);
            if (qs !== '') {
                return urlfull[0] + '?' + qs;
            }
        }
        return url;
    }

    private prefixUrl(url: string) {
        if (pbConfig.UrlContextPrefix !== '' && !url.startsWith(pbConfig.UrlContextPrefix)) {
            return pbConfig.UrlContextPrefix + url;
        }
        return url;
    }

    /*******************************
    private checkPbFunc(anyUrl: string, params?: NameValueObj, pbFunc?: true | string) {
        let funccode: true | string;
        if (pbFunc) {
            funccode = pbFunc;
            if (funccode === true) {
                funccode = this.getFuncCodeFromUrl(anyUrl);
            }
            if (this.funcTree.getElementDisabledByFunc(funccode)) {
                return null;
            }
        }
        return anyUrl;
    }
    */

    private generateUrl(anyUrl: string, qsParams?: NameValueObj, forState?: boolean) {
        let _anyUrl = anyUrl;

        const delimiter = '/';
        if (!_anyUrl.startsWith(delimiter) && !_anyUrl.startsWith('http://') && !_anyUrl.startsWith('https://')) {
            const stack = getCurrentPages();
            const currentActionPath = stack[stack.length - 1].route.split(delimiter);
            const pos = currentActionPath.length - 1;
            let areaParts = currentActionPath.slice(1, pos - 1);
            let area = areaParts.length === 0 ? '' : areaParts.join('.');
            let actionPart = _anyUrl.split(delimiter);
            if (actionPart.length === 1) {
                actionPart = _anyUrl.split(delimiter);
                if (actionPart.length === 1) {
                    _anyUrl = area + delimiter + currentActionPath[pos-1] + delimiter + actionPart[0];
                } else {
                    _anyUrl = area + delimiter + actionPart[0] + delimiter + actionPart[1];
                }
            } else if (actionPart.length === 2) {
                _anyUrl = area + delimiter + actionPart[0] + delimiter + actionPart[1];
            }
            _anyUrl = delimiter + _anyUrl;
            /*******************
            if (forState) {
                const pos = _anyUrl.indexOf('?');
                const routeName = pos >= 0 ? _anyUrl.substring(0, pos) : _anyUrl;
                _anyUrl = $$def.routeNameMapToLink[routeName].absPath;
            }*/
        }
        if (forState) {
            return '/pages' + _anyUrl;
        }

        if (_anyUrl.startsWith('/')) {
            if (pbConfig.UrlMappingPrefix.endsWith('.')) {
                _anyUrl = this.prefixUrl(pbConfig.UrlMappingPrefix + _anyUrl.substring(1));
            } else {
                _anyUrl = this.prefixUrl(pbConfig.UrlMappingPrefix + _anyUrl);
            }
        }
        return this.mergeUrlParams(_anyUrl, qsParams);
    }

    private mergeUrlParams(url: string, qsParams?: NameValueObj) {
        const urlfull = url.split('?');
        let qs = util.Obj2Qs(qsParams);
        if (urlfull[1] && qs !== '') {
            qs = util.MergeQS(urlfull[1], qs);
        }
        if (qs !== '') {
            return urlfull[0] + '?' + qs;
        }
        return url;
    }

}

export const urlHelper = new UrlHelper();
