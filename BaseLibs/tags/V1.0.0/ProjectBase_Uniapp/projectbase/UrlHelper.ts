//import { ClientServerConst } from './ClientServerConst';
//import { NameValueObj, NgxArchError } from './projectbase.type';

import { ClientServerConst } from './ClientServerConst';
import { NameValueObj } from './projectbase.type';
import { pbui } from './projectbase.ui.service';
import { pbConfig } from './ProjectBaseConfig';
import { util } from './UtilHelper';

export class UrlHelper {

    ////////////static ClientOnlyParam_Prefix = 'clientOnly$';
    private static appParams: NameValueObj;

    static SetAppParams(appParams: NameValueObj) {
        UrlHelper.appParams = appParams;
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

    Action(anyUrl: string, params: NameValueObj = {}) {
        params[ClientServerConst.KeyForViewModelOnly] = true;
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
