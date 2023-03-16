var PB_Global_WebApiTestingInterceptor = new function () {
    this.Config = {
        CanCallActionByC: true,
        CallActionWithAjaxOpn:true,
        pb: null
    };
    this.WebApiRepo = {};
    this.ControllerRepo = {};

    this.PutWebApi = function (request, responseData, status, headers, config) {
        if (app.PB_ProductionTip) return;
        var DePrefixUrl = function (url) {
            var prefix = def.GetUrlContextPrefix();
            if (prefix && url.startWith(prefix))
                return url.substring(prefix.length, url.length);
            return url;
        };
        var aca = DePrefixUrl(request.url);
        var pos = aca.indexOf('?');
        if (pos > 0) {
            aca = aca.substring(0, pos);
        }
        //var parts = aca.split('/');
        //var len = parts.length;
        //if (len > 3) {
        //    aca = parts.splice(len - 3, 3).join('/');
        //}
        this.WebApiRepo[aca] = { request, response: { responseData, status, headers, config } };
    };
    this.PutController = function (controller,scope) {
        if (app.PB_ProductionTip) return;
        var name = controller.__proto__._DefinedBy || 'noname';
        this.ControllerRepo.Current = { controller, scope};
    };
    this.GetController = function () {
        return this.ControllerRepo.Current.controller;
    };
    this.GetScope = function () {
        return this.ControllerRepo.Current.scope;
    };
    this.HasResponse = function (aca) {
        return !!this.WebApiRepo[aca];
    };
    this.GetResponse = function (aca) {
        return this.WebApiRepo[aca].response;
    };
    this.GetRcResult = function (aca) {
        var responseData=this.GetResponse(aca).responseData;
        return this.ConvertDateInObj(responseData);
    };
    this.GetRcResultJson = function (aca) {
        var r = this.GetRcResult(aca);
        return JSON.stringify(r, function (key, value) {
            if (key == '_FromResult') return null;
            return value;
        });
    };
    this.TestingDoInController = function (func) {
        var c = this.GetController();
        var scope = this.GetScope();
        func(c, scope);
        scope.$apply();
    };
    this.TestingGetInController = function (func) {
        var c = this.GetController();
        var scope = this.GetScope();
        var rtn = func(c, scope);
        return JSON.stringify(rtn);
    };
    this.TestingCallAction = function (url, body, ajaxOpn, byPb,aca) {
        //清空上次同名请求的response
        this.WebApiRepo[aca] = null;

        if (this.Config.CallActionWithAjaxOpn) {
            ajaxOpn = ajaxOpn || {};
            ajaxOpn.overrideSuper = true;
        } else {
            ajaxOpn = true;//overrideSuper
        }
        if (byPb || !this.Config.CanCallActionByC) {
            this.Config.pb.CallAction(url, body, null, null, ajaxOpn);
        } else {
            var c = this.GetController();
            c.CallAction(url, body, null, null, ajaxOpn);
        }
    };
    this.ConvertDateInObj = function (obj) {
        if (!obj) return obj;
        const df = 'yyyy-MM-dd hh:mm:ss';
        if (typeof (obj) == 'number' && obj > 88499223296) {//数字
            obj = new Date(obj);
            obj = 'Date(' + obj.Format(df);
        }else if (typeof (obj) == 'object' && obj.__proto__.toUTCString) {//日期
            obj = 'Date(' + obj.Format(df);
        } else if (typeof (obj) == 'string' && obj.startsWith('/Date(')) {//特殊字符串
            var dt = parseInt(obj.substr(6));
            obj = dt > 0 ? 'Date(' + new Date(dt).Format(df) : null;
        } else if (typeof (obj) != 'string' && obj.length>0) {//数组
            for (var j = 0; j < obj.length; j++) {
                obj[j] = this.ConvertDateInObj(obj[j]);
            }
        } else if (typeof (obj) == 'object' ) {//复合对象
            for (var prop in obj) {
                if (prop == '_FromResult') continue;
                var v = obj[prop];
                obj[prop] = this.ConvertDateInObj(v);
            }
        }
        return obj;
    }
};

