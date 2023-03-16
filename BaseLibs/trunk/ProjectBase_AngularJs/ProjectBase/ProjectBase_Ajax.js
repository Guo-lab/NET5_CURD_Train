(function (String, angular) {
    'use strict';
    var AjaxNavRoot = { PrevState: "tobeset", PrevStateParams: null, State: null, StateParams: null, StateData: null };
    var AjaxNavData = { ControllerDataMapForCache: {}, Stack: [] };//note that this must be an {} object with a property to store the array,otherwise angular may not track it.
    var ToAjaxNav = { Stack: [] }; //只传入当前要进入的state及stateparams

    //begin-------------pb service:centered on AjaxSubmit,provide all common utilities about it---------------------------------------------------------------
    function pbFn(validationManager, $http, $state, $translate, $parse, $window, $injector, $timeout, $rootScope, pbui, App_MenuData, myCustomErrorMessageResolver, $cookies, $q, ProjectBaseGlobalVar) {
        var $ = function (ele) {
            if (angular.isString(ele)) {
                var found = $window.document.getElementById(ele);
                if (found) return angular.element(found);
                found = $window.document.getElementsByName(ele);
                return angular.element(found);
            }
            return angular.element(ele);
        };
        var parentForm = function (ele) {
            var f = ele;
            while (f.length != 0 && f.prop('tagName') != 'FORM' && f.prop('tagName') != 'NG-FORM') {
                f = f.parent();
            }
            return f;
        };
        var PrefixUrl = function (url) {
            if (ProjectBaseGlobalVar.UrlContextPrefix != '' && !url.startWith(ProjectBaseGlobalVar.UrlContextPrefix))
                return ProjectBaseGlobalVar.UrlContextPrefix + url;
            return url;
        };
        var DePrefixUrl = function (url) {
            if (ProjectBaseGlobalVar.UrlContextPrefix != '' && url.startWith(ProjectBaseGlobalVar.UrlContextPrefix))
                return url.substring(ProjectBaseGlobalVar.UrlContextPrefix.length, url.length);
            return url;
        };
        var SetResultMarker = function (rcJsonResult) {
            rcJsonResult.Ok = rcJsonResult.isRcResult;
            var command = rcJsonResult.command;
            if (command == ProjectBaseGlobalVar.Command_ServerVM || rcJsonResult.command == ProjectBaseGlobalVar.Command_AjaxNavVM) rcJsonResult.IsVM = true;
            else if (command == ProjectBaseGlobalVar.Command_Message) rcJsonResult.IsMsg = true;
            else if (command == ProjectBaseGlobalVar.Command_ServerData) rcJsonResult.IsData = true;
            else if (command == ProjectBaseGlobalVar.Command_Noop) rcJsonResult.IsNoop = true;
            else if (command == ProjectBaseGlobalVar.Command_Redirect) rcJsonResult.IsRedirect = true;
            else if (command == ProjectBaseGlobalVar.Command_AppPage) rcJsonResult.IsAppPage = true;
            else if (command == ProjectBaseGlobalVar.Command_BizException) rcJsonResult.IsBizException = true;
            else if (command == ProjectBaseGlobalVar.Command_Exception) rcJsonResult.IsException = true;
        };
        /**
         * 获取元素所在的Controller。这是对jqEle.controller()的改进。因为对于Dialog，其总是返回最外层的BaseCtrl。此处改进后返回自定义的与View关联的Controller
         * @param {any} jqEle jquery元素对象
         * @param {any} baseAsNull  缺省为true.如果是最外层Controller是否返回null。
         * @param {any} controllerAs 保留
         */
        var GetMyController = function (jqEle, baseAsNull, controllerAs) {
            baseAsNull = baseAsNull || true;
            var c = jqEle.controller();
            if (c && c.IsRecognizedAsDialogController && jqEle.scope()) {
                var formName;
                if (jqEle.is('form')) {
                    formName = jqEle[0].name;
                } else if (jqEle[0].form) {
                    formName = jqEle[0].form.name;
                }
                if (formName&& !controllerAs) {
                    controllerAs = formName.split('.')[0];
                }
                c = jqEle.scope()[controllerAs];
            }
            if (c && c.IsRecognizedAsDialogController && baseAsNull == true) {
                c = null;
            }
            return c;
        };
        //begin---------AjaxSubmit 主过程
        /**
         *  提交表单。将表单中的数据作为要提交的数据，根据配置执行确认提示和表单验证，并处理服务器返回结果。
         *  control：
         *      字符串。表单或表单中的控件的id或name。如果是表单名，可以是表单数组。
         *  qsOrBodyData：
         *      名值对格式字符串或js对象。没有表单时，此项作为qs，有表单时，此项如果是字符串，则作为qs参数；如果是对象，则作为bodyData
         *  ajaxOpn: 
         *      js对象。可选配置参数，此处配置优先于control的相关属性。包括：
                    ajax-url: 字符串。请求服务器action的url。缺省为control的ajax-url属性值。
                    ajax-form: 字符串。form元素id.默认为controller所在的form。使用该form中的输入作为要提交的数据。
                    ajax-data: 名值对字符串或其中的ajax-data，如果是字符串，则作为bodyData参数；如果是对象，则自动转换为名值对后作为bodyData除form中数据外要提交的数据。
                    ajax-method: 字符串。请求使用的http方法。默认为post。
                    ajax-valgroup: 逗号分隔的字符串。提交时指定验证某一组规则。缺省为验证整个form，none表示不做验证，其他字符串值为验证规则分组名称。
                    ajax-bind: 字符串。nobind/parent/元素id。从服务器得到的数据自动绑定到指定元素所在的controller的vm对象。默认为control元素。
                    ajax-processing: 字符串或dom对象或false。从提交开始到响应返回期间需要遮盖的区域元素，id或formname或引用。缺省为form的第一个子节点。false表示不加遮罩。
                    以上参数名可去掉前缀ajax-.如{'ajax-url':'xxx'}等效于{url:'xxx'}
                    removeNames:字符串或数组。如果是字符串多个qs名逗号分隔。这些名字对应的名值对将不会在请求中出现。
         *  funcExecuteResult: 
         *      回调函数，仅在服务器返回正确结果时被调用。该方法有两个参数，第一个是服务器返回的ActionResult（RichClientResult）对象，第二个是调用时的附带参数argsForFunc，一般只用第一个。
         *  argsForFunc：
         *      极少用到。要传递给funcExecuteResult方法的参数值
         *  overrideSuper：
         *      通常使用缺省值。是否覆盖框架对结果的自动处理。缺省情况是框架处理结果后再由脚本定制处理。如果此参数为true则框架不做任何处理。
         *  
         * @param {any} control 字符串。表单或表单中的控件的id或name。如果是表单名，可以是表单数组。
         * @param {any} qsOrBodyData：名值对格式字符串或js对象。没有表单时，此项作为qs，有表单时，此项如果是字符串，则作为qs参数；如果是对象，则作为bodyData
         * @param {any} ajaxOpn: js对象。可选配置参数，此处配置优先于control的相关属性。包括：
                    ajax-url: 字符串。请求服务器action的url。缺省为control的ajax-url属性值。
                    ajax-form: 字符串。form元素id.默认为controller所在的form。使用该form中的输入作为要提交的数据。
                    ajax-data: 名值对字符串或其中的ajax-data，如果是字符串，则作为bodyData参数；如果是对象，则自动转换为名值对后作为bodyData除form中数据外要提交的数据。
                    ajax-method: 字符串。请求使用的http方法。默认为post。
                    ajax-valgroup: 逗号分隔的字符串。提交时指定验证某一组规则。缺省为验证整个form，none表示不做验证，其他字符串值为验证规则分组名称。
                    ajax-bind: 字符串。nobind/parent/元素id。从服务器得到的数据自动绑定到指定元素所在的controller的vm对象。默认为control元素。
                    ajax-processing: 字符串或dom对象或false。从提交开始到响应返回期间需要遮盖的区域元素，id或formname或引用。缺省为form的第一个子节点。false表示不加遮罩。
                    以上参数名可去掉前缀ajax-.如{'ajax-url':'xxx'}等效于{url:'xxx'}
         * @param {any} funcExecuteResult: 回调函数，仅在服务器返回正确结果时被调用。该方法有两个参数，第一个是服务器返回的ActionResult（RichClientResult）对象，第二个是调用时的附带参数argsForFunc，一般只用第一个。
         * @param {any} argsForFunc：极少用到。要传递给funcExecuteResult方法的参数值
         * @param {any} overrideSuper：通常使用缺省值。是否覆盖框架对结果的自动处理。缺省情况是框架处理结果后再由脚本定制处理。如果此参数为true则框架不做任何处理。
         */
        var AjaxSubmit = function (control, qsOrBodyData, ajaxOpn, funcExecuteResult, argsForFunc, overrideSuper) {
            var confirmText, confirmParams = '';

            if (control == null) control = undefined;
            if (control) {
                control = $(control);
                if (control.length > 1 && $(control).prop('tagName') != 'FORM' && $(control).prop('tagName') != 'NG-FORM') {
                    control = control.eq(0);
                }
                if (control.length < 1) {
                    $window.alert("根据参数control的值未能找到元素：" + control);
                    return;
                }
            }
            var confirmMulti;
            if (ajaxOpn) {//ajaxOpn优先于controller的属性
                confirmText = ajaxOpn['ajax-confirm'] || ajaxOpn['confirm'];
                confirmParams = ajaxOpn['ajax-confirm-single'] || ajaxOpn['confirmSingle'];
                confirmMulti = ajaxOpn['ajax-confirm-multi'] || ajaxOpn['confirmMulti'];
            }
            confirmText = confirmText || $(control).attr('ajax-confirm');
            confirmParams = confirmParams || $(control).attr('ajax-confirm-single');
            confirmMulti = confirmMulti || $(control).attr('ajax-confirm-multi');
            if (confirmText && (confirmParams || confirmMulti)) {
                if (confirmMulti) {
                    if (!angular.isArray(confirmMulti)) {//控件属性传来的是字符串，需要解析
                        var parseFunc = $parse(confirmMulti);
                        confirmMulti = parseFunc($(control).scope());
                    }
                    confirmParams = HasSelectedRows(confirmMulti);
                }
                ConfirmSubmit(confirmText, confirmParams).then(function (confirmed) {
                    if (confirmed != true) return;
                    return AjaxSubmitMain(control, qsOrBodyData, ajaxOpn, funcExecuteResult, argsForFunc, overrideSuper);
                });
            } else {
                return AjaxSubmitMain(control, qsOrBodyData, ajaxOpn, funcExecuteResult, argsForFunc, overrideSuper);
            }
        };
        var AjaxSubmitMain = function (control, qsOrBodyData, ajaxOpn, funcExecuteResult, argsForFunc, overrideSuper) {
            var form, url, processingAreaId, bindTargetId, confirmText, method, valgroup, ajaxData, ajaxBodyData;
            var bindTargetControllerInstance;
            var noform = false;
            var autoBind = true;//默认自动绑定返回的数据

            if (ajaxOpn) {//ajaxOpn优先于controller的属性
                ajaxData = ajaxOpn['ajax-data'] || ajaxOpn['data'];
                if (ajaxOpn['ajax-form'] || ajaxOpn['form']) {
                    form = $(ajaxOpn['ajax-form'] || ajaxOpn['form']);
                }
                url = ajaxOpn['ajax-url'] || ajaxOpn['url'];
                bindTargetId = ajaxOpn['ajax-bind'] || ajaxOpn['bind'];
                processingAreaId = ajaxOpn['ajax-processing'] || ajaxOpn['processing'];
                method = ajaxOpn['ajax-method'] || ajaxOpn['method'];
                valgroup = ajaxOpn['ajax-valgroup'] || ajaxOpn['ajax-valgroups'] || ajaxOpn['valgroup'] || ajaxOpn['valgroups'];
            }
            if (typeof (form) == 'undefined') {
                if (typeof (control) == 'undefined') {
                    noform = true;
                }
                else if ($(control).prop('tagName') == 'FORM' || $(control).prop('tagName') == 'NG-FORM') {
                    form = $(control);
                    if (form.length > 1) {
                        if (ajaxOpn.formIndex >= 0) {//可以指定只提交表单数组中的一个
                            form = form.eq(ajaxOpn.formIndex);
                        }
                    }
                } else {
                    var formid = $(control).attr('ajax-form');
                    if (typeof (formid) != 'undefined') {
                        if (formid == '') {
                            noform = true;
                        } else {
                            form = $(formid);
                            if (form.length == 0) {
                                $window.alert('form not found,Id=' + formid);
                                return;
                            }
                        }
                    } else {
                        form = parentForm($(control));
                    }
                }
            }

            if (typeof (url) == 'undefined' || url == '') url = $(control).attr('ajax-url') || $(form).attr('ajax-url');
            if (typeof (url) == 'undefined' || url == '') {
                return $window.alert('need value for ajax-url!');
            }
            if (typeof (processingAreaId) == 'undefined') processingAreaId = $(control).attr('ajax-processing') || $(form).attr('Id');
            if (typeof (method) == 'undefined') method = $(control).attr('ajax-method') || $(form).attr('ajax-method') || 'POST';
            if (typeof (valgroup) == 'undefined') valgroup = $(control).attr('ajax-valgroup') || $(control).attr('ajax-valgroups') || $(form).attr('ajax-valgroup') || $(form).attr('ajax-valgroups');
            if (typeof (bindTargetId) == 'undefined') bindTargetId = $(control).attr("ajax-bind") || $(form).attr("ajax-bind");
            //使用元素指定ajax属性时默认ajax-bind为当前元素，不使用元素而直接指定ajaxOpn,ajaxOpn默认auto-bind=nobind
            if (typeof (bindTargetId) == 'undefined' && ajaxOpn && !ajaxOpn['ajax-bind']) bindTargetId = 'nobind';

            if (bindTargetId == 'nobind') {
                autoBind = false;
            } else if (bindTargetId == 'parent') {
                bindTargetControllerInstance = GetMyController($(control).parent());
                if (bindTargetControllerInstance == null) bindTargetControllerInstance = GetMyController($(form));
                if (bindTargetControllerInstance == null) {
                    $window.alert('ajax-bind=parent targets no controller!');
                    return;
                }
            } else if (typeof (bindTargetId) == 'undefined' || bindTargetId == '') {
                bindTargetControllerInstance = GetMyController($(control));
                if (bindTargetControllerInstance == null) bindTargetControllerInstance = GetMyController($(form));
                if (bindTargetControllerInstance == null) {
                    $window.alert('an default ajax-bind targets no controller!');
                    return;
                }
            }

            if (autoBind && bindTargetControllerInstance == null) {
                if (typeof (processingAreaId) == 'undefined') processingAreaId = bindTargetId;
                if (typeof ($(bindTargetId).attr('ui-view')) != 'undefined') {
                    bindTargetControllerInstance = GetMyController($(bindTargetId).children().eq(0));
                    if (bindTargetControllerInstance == null) {
                        $window.alert('bindTargetId:' + bindTargetId + ' targets a ui-view which has not a controller!');
                        return;
                    }
                } else {
                    $window.alert('bindTargetId:' + bindTargetId + ' should target an ui-view !');
                    return;
                }
            }

            var controller = (ajaxOpn ? ajaxOpn.controller : void 0) || GetMyController($(control));
            var formData;
            var errmsg = '';
            if ($(form).length == 1) {//单个表单
                errmsg = Validate(form, valgroup);
            } else {//表单数组
                formData = '';
                angular.forEach($(form), function (oneform, index) {
                    var msg = Validate($(form).eq(index), valgroup);
                    if (msg) {
                        errmsg = errmsg + '\r\n' + msg;
                    }
                });
            }
            if (errmsg) {
                // this.NotifyValError(ajaxConfig.senderId as string, errmsg);
                return new Promise(function (resolve, reject) {
                    var err = new Error(errmsg);
                    err.name = 'ValError';
                    resolve(err);
                    console.log(errmsg);
                });
            }
            if ($(form).length == 1) {
                formData = SerializeAngularForm(form);
            } else {
                formData = '';
                angular.forEach($(form), function (oneform, index) {
                    formData = MergeQS(formData, SerializeAngularForm($(form).eq(index), ajaxOpn.formArrayAs));
                });
            }

            if (ajaxData && typeof ajaxData == 'object') {
                ajaxData = obj2Qs(ajaxData);
            }
            if (formData && !ajaxData && qsOrBodyData && typeof qsOrBodyData == 'object') {
                ajaxData = obj2Qs(qsOrBodyData);
                qsOrBodyData = void 0;
            } else if (qsOrBodyData && typeof qsOrBodyData == 'object') {
                qsOrBodyData = RemoveNameFromQS(obj2Qs(qsOrBodyData), ajaxOpn.removeNames);
            }
            ajaxBodyData = RemoveNameFromQS(MergeQS(formData, ajaxData), ajaxOpn.removeNames);

            return doSubmit(form, url, method, qsOrBodyData, ajaxBodyData, funcExecuteResult, argsForFunc, overrideSuper,
                processingAreaId, bindTargetControllerInstance, controller, $(control).attr('Id'), null, autoBind);
        };
        //对相对url自动添加area和controller部分
        var makeFullRouteUrl = function (controller,url) {
            if (controller && controller.__proto__._DefinedBy && !controller.__proto__._DefinedBy.startsWith('/Shared') && !url.startsWith('/')) {
                var stateNameParts = controller.__proto__._DefinedBy.split('/');
                var urlParts = url.split('/');
                if (urlParts.length == 1) {
                    url = '/' + stateNameParts[1] + '/' + stateNameParts[2] + '/' + url;
                } else if (urlParts.length == 2) {
                    url = '/' + stateNameParts[1] + '/' + url;
                }
            }
            return url;
        }
        var doSubmit = function (form, url, method, qs, ajaxBodyData, funcExecuteResult, argsForFunc, overrideSuper,
            processingAreaId, bindTargetControllerInstance, controller, senderId, formToken, autoBind) {

            url = makeFullRouteUrl(controller,url);

            url = PrefixUrl(url);
            var urlfull = url.split('?');
            url = urlfull[0] + '?' + ProjectBaseGlobalVar.KeyForViewModelOnly + '=true' + (urlfull[1] ? '&' + urlfull[1] : '');
            if (qs && method.toLowerCase() != 'get') {//非get方法又给了qs参数，则合并到url中，因为非get时$http对params字符串解析有误

            } else if (method.toLowerCase() == 'get' && ajaxBodyData) {//get方法不宜传body，因此合并到qs中
                qs = MergeQS(qs, ajaxBodyData);
            }
            url = MergeQS(url, qs);//发现get时$http对params字符串解析也错误，所以所有qs都合并到url
            qs = void 0;

            pbui.PutProcessing(processingAreaId);

            var headers = {};
            var channel = ProjectBaseGlobalVar.Token_Channel.toLowerCase();
            if (channel == 'header') {
                headers = addLoginToken(null, headers, false);
                if (method.toLowerCase() == 'post') {
                    headers = addFormToken(controller, formToken, headers, false);
                }
            } else if (channel == 'request') {
                if (method.toLowerCase() == 'get') {
                    qs = addLoginToken(null, qs, typeof qs == 'string');
                } else if (method.toLowerCase() == 'post') {
                    ajaxBodyData = addLoginToken(null, ajaxBodyData, true);
                    ajaxBodyData = addFormToken(controller, formToken, ajaxBodyData, true);
                }
            } else if (channel == 'cookie') {
                if (method.toLowerCase() == 'post') {
                    ajaxBodyData = addFormToken(controller, formToken, ajaxBodyData, true);
                }
            }
            var request = { method: method, url: url, data: ajaxBodyData, params: qs, headers };
            var promise = $http(request)
                .success(function (responseData, status, headers, config) {
                    PB_Global_WebApiTestingInterceptor.PutWebApi(request, responseData, status, headers, config);
                    SetResultMarker(responseData);
                    if (responseData.IsData || responseData.IsVM) {
                            responseData.data = ConvertDateInObj(responseData.data);
                    }
                    saveLoginToken(responseData, headers);
                    if (autoBind && responseData.IsVM) {
                        var bindData = responseData.data.ViewModel;
                        bindTargetControllerInstance.vm = bindData;
                    }
                    if (responseData.isRcResult == true) {
                        if (overrideSuper != true) {
                            ExecuteResult(responseData);
                        }
                        if (!funcExecuteResult) {
                            if (controller && controller.ExecuteResult) {//自定义的结果处理优先
                                controller.ExecuteResult(senderId, responseData);
                            }
                            if (responseData.IsNoop && controller && controller.ExecuteNoopResult) {//没有服务器指令则查找客户端定义的callback
                                controller.ExecuteNoopResult(senderId);
                            }
                        }
                    } else if (responseData.isRcResult == false) {
                        ExecuteErrorResult(responseData);
                    }
                    pbui.PutProcessing(false);
                })
                .error(function (responseData, status, headers, config, statusText) {
                    ExecuteHttpError(url,responseData, status, headers, config);
                    pbui.PutProcessing(false);
                });
            if (!funcExecuteResult) return promise;
            return promise.then(function (response) {
                pbui.PutProcessing(false);
                if (response.data.isRcResult == true)
                    return funcExecuteResult(response.data, argsForFunc);
                return response.data;
            });
        }
        /**
         * 发送调用服务器Action的请求。此方法与AjaxSubmit的不同之处是无表单相关的处理（如收集表单数据，验证表单等）。
         * url: 
         *      字符串。请求的url，可带qs
         * bodyData: 
         *      作为bodyData传递的参数，可以是名值对字符串，如果是对象，则自动转换为名值对
         * funcExecuteResult:
         *      回调函数，仅在服务器返回正确结果时被调用。该方法有两个参数，第一个是服务器返回的ActionResult（RichClientResult）对象，第二个是调用时的附带参数argsForFunc，一般只用第一个。
         *  argsForFunc：
         *      极少用到。要传递给funcExecuteResult方法的参数值
         * overrideSuperOrAjaxOpn：
         *      布尔或js对象。如果是布尔值，则表示是否OverrideSuper。如果是对象，则为配置参数，见AjaxSubmit的ajaxOpn。
         *         如果是对象，则为配置参数，同AjaxSubmit的ajaxOpn。包括：
                    ajax-url: 字符串。请求服务器action的url。缺省为control的ajax-url属性值。
                    ajax-form: 字符串。form元素id.默认为controller所在的form。使用该form中的输入作为要提交的数据。
                    ajax-data: 名值对字符串或其中的ajax-data，如果是字符串，则作为bodyData参数；如果是对象，则自动转换为名值对后作为bodyData除form中数据外要提交的数据。
                    ajax-method: 字符串。请求使用的http方法。默认为post。
                    ajax-valgroup: 逗号分隔的字符串。提交时指定验证某一组规则。缺省为验证整个form，none表示不做验证，其他字符串值为验证规则分组名称。
                    ajax-bind: 字符串。nobind/parent/元素id。从服务器得到的数据自动绑定到指定元素所在的controller的vm对象。默认为control元素。
                    ajax-processing: 字符串或dom对象或false。从提交开始到响应返回期间需要遮盖的区域元素，id或formname或引用。缺省为form的第一个子节点。false表示不加遮罩。
                    以上参数名可去掉前缀ajax-.如{'ajax-url':'xxx'}等效于{url:'xxx'}
                    overrideSuper：布尔型。缺省为true。如果要指定ajaxOpn又要同时设置overrideSuper，则在配置对象中设置此属性。
         * @param {any} url：字符串。请求的url，可带qs
         * @param {any} bodyData：作为bodyData传递的参数，可以是名值对字符串，如果是对象，则自动转换为名值对
         * @param {any} funcExecuteResult： 回调函数，仅在服务器返回正确结果时被调用。该方法有两个参数，第一个是服务器返回的ActionResult（RichClientResult）对象，第二个是调用时的附带参数argsForFunc，一般只用第一个。
         * @param {any} argsForFunc：极少用到。要传递给funcExecuteResult方法的参数值
         * @param {any} overrideSuperOrAjaxOpn：布尔或js对象。如果是布尔值，则表示是否OverrideSuper。如果是对象，则为配置参数，见AjaxSubmit的ajaxOpn。
         *         如果是对象，则为配置参数，同AjaxSubmit的ajaxOpn。包括：
                    ajax-url: 字符串。请求服务器action的url。缺省为control的ajax-url属性值。
                    ajax-form: 字符串。form元素id.默认为controller所在的form。使用该form中的输入作为要提交的数据。
                    ajax-data: 名值对字符串或其中的ajax-data，如果是字符串，则作为bodyData参数；如果是对象，则自动转换为名值对后作为bodyData除form中数据外要提交的数据。
                    ajax-method: 字符串。请求使用的http方法。默认为post。
                    ajax-valgroup: 逗号分隔的字符串。提交时指定验证某一组规则。缺省为验证整个form，none表示不做验证，其他字符串值为验证规则分组名称。
                    ajax-bind: 字符串。nobind/parent/元素id。从服务器得到的数据自动绑定到指定元素所在的controller的vm对象。默认为control元素。
                    ajax-processing: 字符串或dom对象或false。从提交开始到响应返回期间需要遮盖的区域元素，id或formname或引用。缺省为form的第一个子节点。false表示不加遮罩。
                    以上参数名可去掉前缀ajax-.如{'ajax-url':'xxx'}等效于{url:'xxx'}
                    overrideSuper：布尔型。缺省为true。如果要指定ajaxOpn又要同时设置overrideSuper，则在配置对象中设置此属性。
         */
        var CallAction = function (url, bodyData, funcExecuteResult, argsForFunc, overrideSuperOrAjaxOpn) {
            if (bodyData && typeof bodyData == 'object') {
                bodyData = obj2Qs(bodyData);
            }
            var overrideSuper, ajaxOpn, ajaxBodyData;
            ajaxBodyData = bodyData;
            if (typeof (overrideSuperOrAjaxOpn) == 'object') {
                ajaxOpn = overrideSuperOrAjaxOpn;
                overrideSuper = ajaxOpn.overrideSuper;
            } else {
                overrideSuper = overrideSuperOrAjaxOpn;
            }
            var formToken, controller, processingAreaId, method;
            if (ajaxOpn) {
                formToken = ajaxOpn.formToken;
                controller = ajaxOpn.controller;
                processingAreaId = ajaxOpn.processing;
                method = ajaxOpn.method || ajaxOpn['ajax-method'];
            }
            method = method||'post';
            return doSubmit(null, url, method, null, ajaxBodyData, funcExecuteResult, argsForFunc, overrideSuper, processingAreaId, null, controller, null, formToken);
        };

        var addFormToken = function (controller, token, headersOrBodyData, addToString) {
            token = token || (controller ? controller.pbFormToken : null);
            if (token) {
                if (addToString) {
                    headersOrBodyData = headersOrBodyData || '';
                    if (headersOrBodyData) {
                        headersOrBodyData += '&';
                    }
                    headersOrBodyData += ProjectBaseGlobalVar.NAME_FORMTOKEN + '=' + $window.encodeURIComponent(token);
                } else {
                    headersOrBodyData = headersOrBodyData || {};
                    headersOrBodyData[ProjectBaseGlobalVar.NAME_FORMTOKEN] = token;
                }
            }
            return headersOrBodyData;
        };
        var addLoginToken = function (token, headersOrBodyData, addToString) {
            token = token || SessionGet(ProjectBaseGlobalVar.NAME_LOGINTOKEN);
            if (token) {
                if (addToString) {
                    headersOrBodyData = headersOrBodyData || '';
                    if (headersOrBodyData) {
                        headersOrBodyData += '&';
                    }
                    headersOrBodyData += ProjectBaseGlobalVar.NAME_LOGINTOKEN + '=' + $window.encodeURIComponent(token);
                } else {
                    headersOrBodyData = headersOrBodyData || {};
                    headersOrBodyData[ProjectBaseGlobalVar.NAME_LOGINTOKEN] = token;
                }
            }
            return headersOrBodyData;
        };
        var saveLoginToken = function (responseData, responseHeaderFunc) {
            var channel = ProjectBaseGlobalVar.Token_Channel.toLowerCase();
            var token;
            if (channel == 'header' && responseHeaderFunc) {
                token = responseHeaderFunc(ProjectBaseGlobalVar.NAME_LOGINTOKEN);
            } else if (channel == 'request' && responseData && responseData.extra && responseData.extra.LoginToken) {
                token = responseData.extra.LoginToken;
            }
            if (token) {
                SessionSet(ProjectBaseGlobalVar.NAME_LOGINTOKEN, token);
            }
        };
        var saveFormToken = function (controller, rcJsonResult, scope) {
            if (!controller) return;
            if (rcJsonResult && rcJsonResult.data && rcJsonResult.data.ViewModelFormToken) {
                controller.pbFormToken = rcJsonResult.data.ViewModelFormToken;
            } else if (!rcJsonResult && scope) {
                var parent = scope.$parent;
                while (parent) {
                    if (parent && parent.c && parent.c.pbFormToken) {
                        controller.pbFormToken = parent.c.pbFormToken;
                        break;
                    }
                    parent = parent.$parent;
                }
            }
        };
        var RemoveLoginToken = function () {
            SessionSet(ProjectBaseGlobalVar.NAME_LOGINTOKEN, null);
        };
        //end-----------AjaxSubmit 主过程

        //begin---------functions parts used by ajaxsubmit,can be used as plugin points by modifing their code-------------------------------------------------------------------
        var SerializeAngularForm = function (jqForm, propNamePrefix) {
            if (propNamePrefix && typeof propNamePrefix == 'string') {
                propNamePrefix += '.';
            } else {
                propNamePrefix = '';
            }
            var r20 = /%20/g,
                rbracket = /\[\]$/,
                rCRLF = /\r?\n/g,
                rinput = /^(?:color|date|datetime|datetime-local|email|hidden|month|number|password|range|search|tel|text|time|url|week)$/i,
                rselectTextarea = /^(?:select|textarea)/i;

            var inputs = jqForm.find('input');
            var ipt2 = jqForm.find('textarea');
            var ipt3 = jqForm.find('select');
            var resultArray = [];
            var f = function (control, index1) {
                if (control.tagName == 'INPUT' || control.tagName == 'SELECT' || control.tagName == 'TEXTAREA') {
                    if (control.name && control.name.substring(0, 1) != "_" && !control.disabled &&
                        (control.checked || rselectTextarea.test(control.nodeName) || rinput.test(control.type))) {
                        var name = propNamePrefix + angular.element(control).attr('name');
                        var val = angular.element(control).val();
                        if (angular.isArray(val)) {
                            angular.forEach(val, function (value, index2) {
                                value = value.replace(rCRLF, "\r\n");
                                resultArray[resultArray.length] = $window.encodeURIComponent(name) + "=" + $window.encodeURIComponent(value);
                            });
                        } else {
                            if (val == undefined) {
                                val = ""
                            } else {
                                val = val.replace(rCRLF, "\r\n");
                                if (control.type == 'radio' && val == 'on') val = '';
                            }
                            resultArray[resultArray.length] = $window.encodeURIComponent(name) + "=" + $window.encodeURIComponent(val);
                        }
                    }
                }
            };

            angular.forEach(inputs, f);
            angular.forEach(ipt2, f);
            angular.forEach(ipt3, f);
            var result = resultArray.join("&").replace(r20, "+");
            return result.replace(/number%3A/g, '').replace(/string%3A/g, '');
        };
        var ConvertDateInObj = function (obj) {
            if (typeof (obj) == 'string' && obj.startsWith('/Date(')) {
                var dt = parseInt(obj.substr(6));
                obj = dt > 0 ? new Date(dt) : null;
            } else if (angular.isArray(obj)) {
                for (var j = 0; j < obj.length; j++) {
                    obj[j]=ConvertDateInObj(obj[j]);
                }
            } else if (typeof (obj) == 'object' && !angular.isDate(obj)) {
                for (var prop in obj) {
                    var v = obj[prop];
                    obj[prop]=ConvertDateInObj(v);
                }
            }
            return obj;
        }
        var ExecuteResult = function (rcJsonResult, bindController) {
            if (bindController && rcJsonResult.IsVM) {
                bindController.vm = rcJsonResult.data.ViewModel;
                saveFormToken(bindController, rcJsonResult);
            } else if (rcJsonResult.IsMsg) {
                ShowMessage(rcJsonResult.data);
            } else if (rcJsonResult.IsRedirect) {
                $state.go(DePrefixUrl(rcJsonResult.data));
            } else if (rcJsonResult.IsAppPage) {
                $window.location = rcJsonResult.data;
            }
        };
        var ExecuteErrorResult = function (rcJsonResult) {
            if (rcJsonResult.IsMsg) {
                ShowErrorMessage(rcJsonResult.data);
            } else if (rcJsonResult.IsRedirect) {
                $state.go(DePrefixUrl(rcJsonResult.data));
            } else if (rcJsonResult.IsAppPage) {
                $window.location = rcJsonResult.data;
            } else if (rcJsonResult.IsData) {

            } else if (rcJsonResult.IsBizException) {
                var data, params;
                var keyword = 'Exception:';
                var index = rcJsonResult.data.indexOf(keyword);
                if (index >= 0) {
                    index = index + keyword.length-1;
                    data = rcJsonResult.data.substr(0, index);
                    params = rcJsonResult.data.substring(index + 1).split(',')
                } else {
                    data = rcJsonResult.data;
                    params = [];
                }
                $translate(data).then(function (msg) {
                    if (params.length == 0) {
                        pbui.ShowCommand(rcJsonResult.command, msg);
                    } else {
                        $translate(params).then(function (result) {
                            var index = 0;
                            for (var key in result) {
                                msg = msg.replace("{" + index + "}", result[key]);
                                index += 1;
                            }
                            pbui.ShowCommand(rcJsonResult.command, msg);
                        })
                    }
                }, function (notTranslatedMsgKey) {
                    pbui.ShowCommand(rcJsonResult.command, notTranslatedMsgKey);
                });
            } else if (rcJsonResult.IsException) {
                pbui.Alert(rcJsonResult.data);
            } else {
                throw new Error(rcJsonResult.command);
            }

        };
        var ExecuteHttpError = function (requestUrl,data, status, headers, config) {
            if (status == '-1') {
                var url = ProjectBaseGlobalVar.HttpErrorMinus1_Redirect;
                if (url && typeof (ProjectBaseGlobalVar.HttpErrorMinus1_Redirect) == 'function') {
                    url = ProjectBaseGlobalVar.HttpErrorMinus1_Redirect(requestUrl);
                }
                if (url && requestUrl.indexOf(url) < 0) {//重定向的url请求仍然报-1错，通常是网络断开了，因此不再循环请求
                    if (url.toLowerCase().startWith('http')) {
                        $window.location = url;
                    } else {
                        $state.go(url);
                    }
                } else {
                    console.error('HttpStatus=-1,  url=' + requestUrl);
                }
                return;
            }
            if (status == "500")
                pbui.Alert("UnexpectedError");
            else
                pbui.Alert('HttpStatus: '+status);
        };
        var ShowMessage = function (msgKey) {
            $translate(msgKey).then(function (msg) {
                pbui.Alert(msg);
            }, function (notTranslatedMsgKey) {
                pbui.Alert(notTranslatedMsgKey);
            });
        };
        var ShowErrorMessage = function (msgKey) {
            $translate(msgKey).then(function (msg) {
                pbui.Alert(msg);
            }, function (notTranslatedMsgKey) {
                pbui.Alert(notTranslatedMsgKey);
            });
        };
        var HasSelectedRows = function (array) { //angular对绑定的多选列表中未选项保留绑定值为false，数组长度不变。
            var cnt = array.length;
            for (var i = 0; i < cnt; i++) {
                if (array[i] != null && array[i] != false) return true;
            }
            return false;
        };
        var ConfirmSubmit = function (confirmMsgKey, confirmIdTextOrHasRows) {
            if (!confirmMsgKey) confirmMsgKey = 'ConfirmDelete';
            if (angular.isString(confirmIdTextOrHasRows))
                return pbui.Confirm(confirmMsgKey, confirmIdTextOrHasRows);
            else if (!confirmIdTextOrHasRows) {
                return pbui.Alert('EmptySelectionException').then(function () { return false; });
            } else {
                return pbui.Confirm('ConfirmDeleteMulti');
            }
        };
        //end--------pb plugin functions

        //begin--------------------------------------------表单验证
        /**
        *  验证指定表单的指定的规则。
        *  form : 可以是字符串也可以是jquery对象.如果是字符串其值为form的id或name值
        *  valGroups: 不指定则表示不涉及分组验证，'none'表示不验证, 涉及分组验证则必须指出组名，包括缺省组也要指出
        *  angularFormController: 可缺省
        *  @param {any} form :可以是字符串也可以是jquery对象.如果是字符串其值为form的id或name值
        *  @param {any} valGroups: 不指定则表示不涉及分组验证，'none'表示不验证, 涉及分组验证则必须指出组名，包括缺省组也要指出
        *  @param {any} angularFormController:可缺省
        *  @param: partialValid: 要验证的部分子控件名，仅支持一级。缺省时验证form中的所有子控件
        */
        var Validate = function (form, valGroups, angularFormController) {
            var formIsObj = false;
            if (typeof form == 'string') {
                form = $(form);
            }
            if (form.length) {
                form = $(form).eq(0);
                angularFormController = $(form).controller('form');
            } else {
                formIsObj = true;
                throw Error("暂不支持直接指定form对象");
            }
            var ok = AutoValidatorValidate(form, valGroups, formIsObj, angularFormController);
            if (ok) return null;
            return 'ValError';
        };
        /**
        * validate a form. 绑定相应验证规则并执行。
        *  @param: formElementOrCtrl:
        *  @param: valGroups: 不指定则表示不涉及分组验证，'none'表示不验证, 涉及分组验证则必须指出组名，包括缺省组也要指出
        *  @param: partialValid: 要验证的部分子控件名，仅支持一级。缺省时验证form中的所有子控件
        */
        var AutoValidatorValidate = function (formJq, valGroups, formIsObj, angularFormController) {
            if (valGroups == 'none') return true;

            var formCtrl, formElement;
            if (formIsObj) {
                formCtrl = formJq;
                throw Error("暂不支持直接指定form对象");
            } else {
                formElement = formJq;
                var formCtrl = $(formElement).data().kendoValidator || angularFormController;
            }
            if (!formCtrl) {
                throw new Error('未找到表单对象');
            }

            formCtrl.pbCurrentValGroups = valGroups;
            var ok = true;
            if (formCtrl.validate) {
                formCtrl.validate();
                ok = Object.keys(formCtrl._errors).length == 0;
                if (!ok) {
                    console.warn(formCtrl._errors);
                }
            }
            if (angularFormController) {
                for (var prop in angularFormController) {
                    var member = angularFormController[prop];
                    if (member && member.$validate) {
                        member.$validate();
                    }
                }
                ok = ok && angularFormController.$valid;
            }
            if (ok) {
                var vmFormValidateFunc = formCtrl.pbVmFormValidator;
                if (!vmFormValidateFunc) {
                    var controller = GetMyController(formElement);
                    var formName = formElement[0].name;
                    if (controller.pbFormArrayVmValidatorRegistra && controller.pbFormArrayVmValidatorRegistra[formName]) {
                        vmFormValidateFunc = controller.pbFormArrayVmValidatorRegistra[formName].pbVmFormValidator;
                    }
                }
                if (vmFormValidateFunc) {
                    var formIndex = GetFormIndex(formElement);
                    var msg = vmFormValidateFunc(formIndex);
                    if (msg == null) return true;
                    ShowErrorMessage(msg);
                    return false;
                }
            }
            if (!ok) {
                validationManager.validateForm(formElement);
                return false;
            }
            return true;
        };
        var GetFormObj = function (ele, ngModelController) {
            if (ngModelController) {
                return ngModelController.$$parentForm;
            } else if (ele[0].form) {
                var form = $(ele[0].form).data().kendoValidator;
                if (!form) {
                    form = $(ele[0].form).controller('form');
                }
                return form;
            }
            throw new Error('找不到输入控件所在表单：' + ele[0].name);
        };
        var GetFormName = function (ele, ngModelController) {
            if (ngModelController) {
                return ngModelController.$$parentForm.$name;
            } else if (ele[0].form) {
                return ele[0].form.name;
            }
            throw new Error('找不到输入控件所在表单：' + ele[0].name);
        };
        //如果表单是表单数组中的一个，返回其Index，否则返回-1。
        var GetFormIndex = function (ele, scope) {
            var index = $(ele[0].form).attr('pb-form-array-index') || $(ele[0]).attr('pb-form-array-index');
            index = -(-index);
            if (!(index >= 0) && scope) {//此处必须用!(index>=0)，不能用index<0
                index = $parse('$index')(scope);
            }
            return index >= 0 ? index : -1;
        };
        var GetFormNameWithIndex = function (ele, scope) {
            var index = GetFormIndex(ele, scope);
            if (index == -1) index = '';
            return GetFormName(ele) + index;
        };
        var GetFormArray = function (formArrayName) {
            var jqForms = $(formArrayName);
            if (jqForms.length == 0) throw new Error("未找到表单数组：" + formArrayName);
            var forms = [];
            var cnt = jqForms.length;
            for (var i = 0; i < cnt; i++) {
                var form = jqForms.eq(i).data().kendoValidator;
                forms[i] = form || jqForms(i).controller('form');
            }
            return forms;
        };
        /**
         * 为控件注册通用验证方法（由分组验证标记定义的）
         * @param {any} scope
         * @param {any} attrs
         * @param {any} ngModelController 为空则是kendo系
         * @param {any} validatorName 验证类型名
         * @param {any} validator:函数function (modelValue, viewValue)
         */
        var RegisterGroupedValidator = function (scope, ele, attrs, ngModelController, validatorName, validator) {
            var formName = GetFormName(ele, ngModelController);
            var ctrlName = attrs.name;
            if (ctrlName.substring(0, 1) == ProjectBaseGlobalVar.UnSubmitNameMarker) return;
            var rulegroups = attrs[validatorName + "Groups"];
            var belongToDefault = !rulegroups;
            if (belongToDefault) {
                rulegroups = ProjectBaseGlobalVar.VmMeta_DefaultRuleGroup;
            }
            rulegroups = rulegroups.split(',');
            //此时form还是angular form,但ele是kendo ele。form后面会被kendo form替代
            var controller = GetMyController($(ele[0].form));//$parse(formName.split('.')[0])($(ele[0]).scope());// 不能用$(ele).controller();因为对Dialog里的ele返回的是BaseCtrl
            var validatorFunc = function (modelValue, viewValue, inputEle) {
                //<在一个表单第一次验证前为其注册验证器:(因为弹窗没有viewContentLoaded，无法找到合适时机，所以只能在第一次验证前)
                controller.pbRegisterVmValidatorDone = controller.pbRegisterVmValidatorDone || {};
                if (!controller.pbRegisterVmValidatorDone[formName] && controller.OnRegisterVmValidator) {
                    controller.OnRegisterVmValidator(formName);//每次只能注册formName对应的当前表单或表单数组
                    controller.pbRegisterVmValidatorDone[formName] = true;
                }
                //>

                /*var form = $parse(formName)(scope);//由于kendo-validator使用了与name相同的值，导致kendoForm加载后会替换掉原来的form对象，所以此处不能使用原来的form对象而是找到新的form对象。
                表单数组同名多个表单，因此不能再按名找表单。*/
                var form = GetFormObj(inputEle || ele);//inputEle为undefined时是angularForm控件
                var formIndex = GetFormIndex(inputEle || ele);

                var shouldVal = false;
                var onlyAlways = false;
                var valGroups = form.pbCurrentValGroups || scope.pbCurrentValGroups;
                if (!valGroups) {//pb.Validate中没指定要验证的组，可能在ValidateWhen中指定
                    var whenFunc = form.pbValidateWhen;
                    if (!whenFunc && controller.pbFormArrayVmValidatorRegistra && controller.pbFormArrayVmValidatorRegistra[formName]) {
                        whenFunc = controller.pbFormArrayVmValidatorRegistra[formName].pbValidateWhen;
                    }
                    if (whenFunc) {
                        valGroups = whenFunc(formIndex);
                        if (!valGroups) {//返回null表示只验证总是组
                            onlyAlways = true;
                            valGroups = ProjectBaseGlobalVar.VmMeta_AlwaysRuleGroup;
                        }
                    }
                }
                //没有指定验证组也没有ValidateWhen指定，则验证缺省和总是组
                if (!valGroups && belongToDefault) {
                    shouldVal = true;
                } else {
                    if (!valGroups) {
                        valGroups = ProjectBaseGlobalVar.VmMeta_DefaultRuleGroup;
                    }
                    if (valGroups) {
                        valGroups = valGroups.split(',');
                    }
                    if (!onlyAlways) {
                        valGroups[valGroups.length] = ProjectBaseGlobalVar.VmMeta_AlwaysRuleGroup;
                    }
                    angular.forEach(rulegroups, function (v, i) {
                        if (valGroups.includesStringIgnoreCase(v)) {
                            shouldVal = true;
                            return true;
                        }
                    });
                }
                if (shouldVal) {
                    return validator(modelValue, viewValue, formIndex);
                } else {
                    return true;
                }
            };

            if (ngModelController) {//非kendo控件的validators记录在angularForm对象中对应的ngModelController中
                ngModelController.$validators[validatorName] = validatorFunc;
            } else {
                /*kendoValidator没有为每个控件记录validators的地方，而且此时还未创建kendoForm(kendoValidator)对象，
                 * 因此这里仿造angular form的控件ngModelController，在controller上为每个kendoForm记录每个kendo控件的validators，
                 * 这些validators实际由kendoForm(kendoValidator)在其定制的rule中组合调用
                */
                var formNameWithIndex = GetFormNameWithIndex(ele, scope);
                controller.pbKendoForms = controller.pbKendoForms || {};
                controller.pbKendoForms[formNameWithIndex] = controller.pbKendoForms[formNameWithIndex] || {};
                var pbKendoForm = controller.pbKendoForms[formNameWithIndex];
                pbKendoForm.pbKendoModelControllers = pbKendoForm.pbKendoModelControllers || {};
                pbKendoForm.pbKendoModelControllers[ctrlName] = pbKendoForm.pbKendoModelControllers[ctrlName] || { validators: {} };
                pbKendoForm.pbKendoModelControllers[ctrlName].validators[validatorName] = validatorFunc;
            }
        };
        var BuildGroupedValidatorAsCustomRuleForKendo = function (kendoForm) {
            return function (input) {//此方法为执行定制验证，对hidden控件也执行。只在内置规则通过后才会执行。
                if (!input[0].name) return true;
                var ctrlName = input[0].name;
                var formName = GetFormNameWithIndex(input);
                var controller = GetMyController($(kendoForm.element[0]));//不能用$(kendoForm.element[0]).controller();
                if (!controller.pbKendoForms) return true;
                var pbKendoForm = controller.pbKendoForms[formName];
                if (!pbKendoForm||!pbKendoForm.pbKendoModelControllers || !pbKendoForm.pbKendoModelControllers[ctrlName]
                    || !pbKendoForm.pbKendoModelControllers[ctrlName].validators) return true;
                var validators = pbKendoForm.pbKendoModelControllers[ctrlName].validators;
                for (var vType in validators) {
                    var func = validators[vType];
                    var ok = func(void 0, input[0].value, input);
                    if (!ok) {
                        var ctl = getTooltipControl(input);
                        myCustomErrorMessageResolver.resolve(vType, input, ctl);
                        return false;
                    }
                }
                return true;
            }
        };

        /**
         * 注册所有验证方法
         * @param {any} form: 一个form对象，或字符串表示formArray名
         * @param {{ShouldValidateGroups,VmFormValidate,所有VmInner标记指定的方法名}} validatorRegistrar:对象用于定义验证函数，属性名对应函数名，属性值对应验证函数
        */
        var RegisterVmValidator = function (formOrFormArray, validatorRegistrar,controller) {
            if (!formOrFormArray) {
                throw new Error('registerVmInnerValidator方法应在c.OnRegisterVmValidator中使用，以保证form对象已创建');
            }
            var form;
            if (typeof formOrFormArray == 'string') {//表单数组的验证器注册只记一套，按表单名记在controller上。
                if (!controller) throw Error("表单数组验证注册时必须传第三个参数controller");
                var forms = GetFormArray(formOrFormArray);
                form = forms[0];
                var formName = form.$name || form.element[0].name;
                controller.pbFormArrayVmValidatorRegistra = controller.pbFormArrayVmValidatorRegistra || {};
                controller.pbFormArrayVmValidatorRegistra[formName] = {};
                form = controller.pbFormArrayVmValidatorRegistra[formName];
            } else {
                form = formOrFormArray;//单个表单的验证器就记录在单个表单对象上
            }

            if (validatorRegistrar.VmFormValidate) {
                    RegisterVmInnerValidator(form, null, null, validatorRegistrar.VmFormValidate);
            }
            if (validatorRegistrar.ShouldValidateGroups) {
                    ValidateWhen(form, validatorRegistrar.ShouldValidateGroups);
            }
            for (var prop in validatorRegistrar) {
                if (prop == 'VmFormValidate' || prop == 'ShouldValidateGroups') continue;
                    RegisterVmInnerValidator(form, prop, validatorRegistrar[prop]);
            }
        }
        /**
         * 注册VmInner验证方法
         * @param {any} form:是form对象
         * @param {any} name:与服务器方法名对应
         * @param {any} validator:函数function (modelValue, viewValue)
         * @param {form} vmFormValidator:函数用于整体验证
         */
        var RegisterVmInnerValidator = function (form, name, validator, vmFormValidator) {
            if (!form) {
                throw new Error('registerVmInnerValidator方法应在c.OnRegisterVmValidator中使用，以保证form对象已创建');
            }
            if (vmFormValidator && !angular.isFunction(vmFormValidator)) {
                throw new Error('vmFormValidator必须是函数');
            }
            if (validator && !angular.isFunction(validator)) {
                throw new Error('validator必须是函数');
            }
            if (vmFormValidator) {
                form.pbVmFormValidator = vmFormValidator;
            }
            if (name) {
                form.pbVmInnerValidators = form.pbVmInnerValidators || {};
                form.pbVmInnerValidators[name] = validator;
            }
        };
        //注册ShouldValidateGroups验证方法
        var ValidateWhen = function (formCtrl, funcShouldValidateGroups) {
            formCtrl.pbValidateWhen = funcShouldValidateGroups;
        };
        var KendoValidator = function ($scope, formName) {//仅为兼容保留
            var array = formName.split(".")
            var validator = $scope[array[0]];
            for (var i = 1; i < array.length; i++)
                validator = validator[array[i]];
            return validator;
        }
        var getTooltipControl = function (element) {
            var widget = kendo.widgetInstance(element);
            if (widget == undefined)
                return element;
            else {
                if (widget.wrapper.find("input[role='spinbutton']").length > 0)
                    return widget.wrapper.find("input[role='spinbutton']").first();
                else
                    return widget.wrapper;
            }
        }
        var KendoBindValidate = function (scope, kendoForm) {
            var getErrorType = function (element) {
                var validity = element[0].validity;
                if (validity.patternMismatch)
                    return "pattern";
                else if (validity.valueMissing)
                    return "required";
                else if (validity.rangeOverflow)
                    return "max";
                else if (validity.rangUnderFlow)
                    return "min";
                else if (validity.typeMismatch)
                    return "email";
            }

            if (!kendoForm.options.rules.pbCustomValidator) {
                kendoForm.bind("validateInput", function (e) {//此事件只在验证状态改变时触发，即从valid变为invalid或从invalid变为valid时才触发，否则不触发
                    var ctl = getTooltipControl(e.input);
                    if (!e.valid) {
                        var errType = getErrorType(e.input);
                        if (errType) {
                            myCustomErrorMessageResolver.resolve(errType, e.input, ctl);
                        }
                    } else {
                        ctl.attr("title", "");
                    }
                });
                kendoForm.options.rules.pbCustomValidator = BuildGroupedValidatorAsCustomRuleForKendo(kendoForm);
            }
        }
        var SetPristine = function (frmCtrl, groupName) {
            angular.forEach(frmCtrl.PB_Group[groupName], function (ele, eleName) {
                frmCtrl[eleName].$setPristine();
            });
        };
        var ValidateAGroupOfCtrls = function (frmCtrl, ctrlGroupName) {
            var isvalid = true;
            angular.forEach(frmCtrl.PB_Group[ctrlGroupName], function (ele, eleName) {
                var ctrl = ele.controller("ngModel");
                if (ctrl.$invalid) {
                    isvalid = false;
                    validationManager.validateElement(ctrl, ele, { disabled: false, forceValidation: true });
                }
            });
            return isvalid;
        };
        //end-------------------表单验证--------------

        //begin---------AjaxNav-------------------------------------------------------------------
        var AjaxNavRegister = function (toState, toParams, fromState, fromParams, onleaving) {
            var isRefresh = false; //是否通过浏览器刷新进入
            if (AjaxNavData.Stack.length == 0 && window.localStorage.getItem("AjaxNavDataStack")) {
                AjaxNavData.Stack = JSON.retrocycle(JSON.parse(decodeURIComponent(window.localStorage.getItem("AjaxNavDataStack"))));
                isRefresh = true;
            }
            if (isRefresh) {
                if (AjaxNavData.Stack[AjaxNavData.Stack.length - 1].State.name == toState.name) {
                    AjaxNavData.Stack[AjaxNavData.Stack.length - 1].State = toState;
                } else {
                    var hasroot = false;
                    for (var i = AjaxNavData.Stack.length - 1; i >= 0; i--) {
                        if (AjaxNavData.Stack[i].State.name == toState.name) {
                            hasroot = true;
                            AjaxNavData.Stack.splice(i + 1, AjaxNavData.Stack.length - (i + 1));
                            return;
                        }
                    }
                    if (hasroot == false) {
                        AjaxNavData.Stack = [{ State: {} }];
                        AjaxNavData.Stack[0].State = toState;
                    }
                }
                return;
            };
            if (fromState.name == toState.name || toState.name == AjaxNavRoot.PrevState) return;
            //进入已记录路径中任一个,栈中已经被AjaxNav弹出后面的因而最后一个就是当前进入的，不需要再重复记录
            if (AjaxNavData.Stack.length > 0 && toState.name == AjaxNavData.Stack[AjaxNavData.Stack.length - 1].State.name) return;

            var tonav, map = {};
            if (toParams && toParams['ajax-nav']) {
                tonav = angular.lowercase(toParams['ajax-nav']);
            }

            if (onleaving) {//from是当前所在，根据去向决定是否需要将数据保存
                if (tonav == 'forward') {
                    if (AjaxNavData.Stack.length == 1 && AjaxNavData.Stack[0].State == null) {
                        AjaxNavData.Stack[0].State = fromState;
                    }
                    var lastnav = AjaxNavData.Stack[AjaxNavData.Stack.length - 1];
                    if (fromState.name == lastnav.State.name) {//should always be true
                        var st = lastnav.State;
                        do {
                            if (st.views) {
                                angular.forEach(st.views, function (config, viewname) {
                                    var name = st.name + ".views_" + viewname;
                                    if (AjaxNavData.ControllerDataMapForCache[name]) map[name] = AjaxNavData.ControllerDataMapForCache[name].vm;
                                });
                            } else {
                                if (AjaxNavData.ControllerDataMapForCache[st.name]) map[st.name] = AjaxNavData.ControllerDataMapForCache[st.name].vm;
                            }
                            st = $state.get(st.parent);
                        } while (!st.name.startWith('/Shared/'));
                        lastnav.StateData = map;
                        //console.log('remember:'+fromState.name);
                    }
                }
                AjaxNavData.ControllerDataMapForCache = {};
                //console.log('leaving:'+fromState.name);
            } else { //to是当前所在，要记录上一
                var tonavback = angular.lowercase(toParams['ajax-nav-back']);
                if (tonavback) return;//从路径上返回时不需要注册

                for (var i = AjaxNavData.Stack.length - 1; i >= 0; i--) {
                    if (AjaxNavData.Stack[i].State.name == toState.name) {
                        AjaxNavData.Stack.splice(i + 1, AjaxNavData.Stack.length - (i + 1));
                        return;
                    }
                }

                tonav = tonav || 'root';
                var navData = { PrevState: fromState, PrevStateParams: fromParams, State: toState, StateParams: toParams, StateData: null };
                if (tonav == 'root') {
                    AjaxNavData.Stack = [AjaxNavRoot];
                    AjaxNavData.Stack[0].State = toState;
                    AjaxNavData.Stack[0].StateParams = toParams;
                } else if (tonav == 'forward') {
                    AjaxNavData.Stack.push(navData);
                } else {
                    alert('nav has invalid value!');
                }

                ToAjaxNav.Stack = { State: [], StateParams: [] };
                ToAjaxNav.Stack.State.push(AjaxNavData.Stack[AjaxNavData.Stack.length - 1].State);
                ToAjaxNav.Stack.StateParams.push(AjaxNavData.Stack[AjaxNavData.Stack.length - 1].StateParams);

                window.localStorage.setItem("AjaxNavDataStack", encodeURIComponent(JSON.stringify(JSON.decycle(AjaxNavData.Stack))));
            }
        };
        var AjaxNavRefresh = function () {
            var state = AjaxNavData.Stack[AjaxNavData.Stack.length - 1].State;
            var stateParams = AjaxNavData.Stack[AjaxNavData.Stack.length - 1].StateParams;
            $state.go(state.name, stateParams, { inherit: false, reload: true });
        };
        /**
         * 
         * @param {any} state 必须是state对象
         * @param {any} stateParams
         * @param {any} stateName 如果state为null，可以指定此参数.
         */
        var AjaxNavTo = function (state, stateParams,stateName) {
            if (!state) {
                state = AjaxNavFindStateByName(stateName);
            }
            if (!state) {
                throw new Error("未找到相应的state，stateName=" + stateName);
            }
            while (AjaxNavData.Stack.length > 0 && state != AjaxNavData.Stack[AjaxNavData.Stack.length - 1].State) {
                AjaxNavData.Stack.pop();
                ToAjaxNav.Stack = { State: [], StateParams: [] };
                ToAjaxNav.Stack.State.push(AjaxNavData.Stack[AjaxNavData.Stack.length - 1].State);
                ToAjaxNav.Stack.StateParams.push(AjaxNavData.Stack[AjaxNavData.Stack.length - 1].StateParams);
            }
            stateParams = stateParams || state.StateParams;
            $state.go(state.name, stateParams, { inherit: false, reload: true });
        };
        var AjaxNavFindStateByName = function (stateName) {
            let i = AjaxNavData.Stack.length - 1;
            let state = AjaxNavData.Stack[i].State;
            while (i > 0 && state && stateName != state.name) {
                i--;
                state = AjaxNavData.Stack[i].State;
            }
            return state;
        }
        var AjaxNavBack = function () {
            var last = AjaxNavData.Stack.pop();
            var params;
            if (last.PrevStateParams) {
                params = angular.copy(last.PrevStateParams);
                params['ajax-nav-back'] = 'back:' + $state.current.name;
            }
            if (last.PrevState) {
                $state.go(last.PrevState.name, params, { inherit: false, reload: true });
            }
        };
        var AjaxNavGetBackSrc = function (stateName, stateParams) {
            var source = stateParams ? stateParams['ajax-nav-back'] : undefined;
            var isBack = source && source.startWith ? source.startWith('back:') : false;
            if (source && isBack) {
                var t = CreateRcResult(true);
                t.data.ViewModel = AjaxNavData.Stack[AjaxNavData.Stack.length - 1].StateData[stateName];
                return {
                    IsBack: isBack,
                    BackFrom: isBack ? source.substring(5, source.length) : undefined,
                    LastData: t
                };
            } else {
                return false;
            }
            return false;
        };
        var CreateRcResult = function (isRcResult, command, data) {
            if (!command && !data) {
                command = ProjectBaseGlobalVar.Command_AjaxNavVM;
                data = { ViewModel: null };
            }
            return {
                isRcResult: isRcResult,
                command: command,
                data: data
            };
        };
        //end---------AjaxNav-------------------------------------------------------------------

        var Super = function (controller, serverVm, scope) {
            controller.vm = serverVm;
            saveFormToken(controller, serverVm ? serverVm._FromResult : null, scope);

            controller.tmp = {};
            controller.pbPreventFormSubmit = function (event) {
                event.preventDefault();
                return false;
            }
            controller.AjaxSubmit = function (control, qsOrBodyData, ajaxOpn, funcExecuteResult, argsForFunc, overrideSuper) {
                ajaxOpn = ajaxOpn || {};
                ajaxOpn.controller = controller;
                return AjaxSubmit(control, qsOrBodyData, ajaxOpn, funcExecuteResult, argsForFunc, overrideSuper);
            };
            controller.CallAction = function (url, bodyData, funcExecuteResult, argsForFunc, overrideSuperOrAjaxOpn) {
                var ajaxOpn = {};
                if (typeof overrideSuperOrAjaxOpn == 'boolean') {
                    ajaxOpn.overrideSuper = overrideSuperOrAjaxOpn;
                } else if (typeof overrideSuperOrAjaxOpn == 'object') {
                    ajaxOpn = overrideSuperOrAjaxOpn;
                }
                ajaxOpn.controller = controller;
                return CallAction(url, bodyData, funcExecuteResult, argsForFunc, ajaxOpn);
            };
            controller.GetFormArray = function (formArrayName) {
                return GetFormArray(formArrayName);
            };
            controller.SetFormToken = function (token) {
                return controller.pbFormToken = token;
            };
            controller.GetFormToken = function () {
                return controller.pbFormToken;
            };
            controller.ShowCollectionItem = function (container, key, url, eventName,alwaysRefresh) {
                ShowCollectionItem(container, key, url, eventName, controller,alwaysRefresh);
            };

            //初始化controller时记住对数据的引用，以state+view名为键
            var ctrlname = controller.__proto__._DefinedBy;
            if (!ctrlname.startWith('/Shared/')) {
                var dataOfController = controller;// {vm:controller.vm,tmp:controller.tmp};

                AjaxNavData.ControllerDataMapForCache[ctrlname] = dataOfController;
                for (var prop in controller.__proto__._UsedBy) {
                    AjaxNavData.ControllerDataMapForCache[prop] = dataOfController;
                };
                //console.log('cache:'+ctrlname);
            }
            PB_Global_WebApiTestingInterceptor.PutController(controller);

            scope.$on('$viewContentLoaded', function (event) {//返回会自动callback执行动作以刷新页面数据
                if (serverVm && serverVm._FromResult && serverVm._FromResult.command == ProjectBaseGlobalVar.Command_AjaxNavVM) {
                    if (angular.isFunction(controller.OnNavBackRefresh)) {
                        $timeout(function () {
                            controller.OnNavBackRefresh();
                        });
                    }
                }
                if (angular.isFunction(controller.OnViewContentLoaded)) {
                    controller.OnViewContentLoaded(event);
                } else if (angular.isFunction(controller.OnReady)) {
                    controller.OnReady(event);
                }
            });

            controller.NavForward = function (stateName, params, extras) {
                NavForward(stateName, params, extras, controller);
            };
            controller.NavRoot = function (stateName, params, extras) {
                NavRoot(stateName, params, extras, controller);
            };
            controller.NavBack = function () {
                NavBack();
            };
            controller.NavRefresh = function () {
                NavRefresh();
            };
            controller.NavTo = function (stateName, params, extras) {
                NavTo(stateName, params, extras, controller);
            };

            return controller;
        };

        var MergeQS = function (qs1, qs2) {//用第二个参数覆盖第一个
            if (typeof (qs1) == 'undefined' || qs1==null) {
                if (typeof (qs2) == 'undefined' || qs2 == null) {
                    return '';
                }
                return qs2;
            } else {
                if (typeof (qs2) == 'undefined' || qs2 == null) {
                    return qs1;
                }
            }

            if (qs1 == '' || qs2 == '') return qs1 + qs2;

            var pairs1 = qs1.split('&');
            var pairs2 = qs2.split('&');
            var newqs = '';
            var qs1new = '';
            var qs2new = '';
            for (var i = 0; i < pairs2.length; i++) {
                var a = pairs2[i].split('=');
                var found = false;
                for (var j = 0; j < pairs1.length; j++) {
                    if (pairs1[j].split('=')[0].toLowerCase() == a[0].toLowerCase()) {
                        pairs1[j] = pairs2[i];
                        found = true;
                        break;
                    }
                }
                if (!found && pairs2[i] != '') qs2new = qs2new + pairs2[i] + '&';
            }
            for (var i = 0; i < pairs1.length; i++) {
                qs1new = qs1new + pairs1[i] + '&';
            }
            if (qs1new.endWith('&')) {
                newqs = qs1new;
            }
            if (qs2new.endWith('&')) {
                newqs = newqs + qs2new;
            }
            if (newqs.endWith('&')) {
                newqs = newqs.substr(0, newqs.length - 1);
            }
            return newqs;
        };
        var RemoveNameFromQS=function(qs, removeNames) {
            if (!qs || !removeNames) return qs;
            let pairs = qs.split('&');
            let qsnew = '';
            if (typeof (removeNames) == 'string') {
                removeNames = removeNames.split(',');
            }
            for (var i = 0; i < pairs.length; i++) {
                if (!removeNames.includes(pairs[i].split('=')[0])) {
                    qsnew = qsnew + pairs[i] + '&';
                }
            }
            return qsnew;
        }
        var ShowCollectionItem = function (container, key, url, eventName, controller,alwaysRefresh) {
            if (container[key] == undefined || alwaysRefresh) {
                CallAction(url, null, function (ret) {
                    container[key] = ret.data;
                    $rootScope.$emit(eventName, ret.data);
                    if (controller && ret.data.ViewModelFormToken) {
                        controller.SetFormToken(ret.data.ViewModelFormToken);
                    }
                });
            }
        }
        var TabHandler = function (scope, pageName, parameter, showTab) {
            var contentLoaded = false;
            scope.$on('$viewContentLoaded', function () {
                if (contentLoaded) { return; }
                contentLoaded = true;
                if (sessionStorage[pageName]) {
                    var info = angular.fromJson(sessionStorage[pageName]);
                    if (info.Param == parameter) {
                        showTab(info.Index);
                    } else {
                        showTab(0);
                    }
                } else {
                    showTab(0);
                }
            })
            return {
                RecordIndex: function (index) {
                    sessionStorage[pageName] = angular.toJson({ Param: parameter, Index: index });
                }
            }
        }
        var TimeZoneDiff = 28800000; // ms， 8小时。
        var obj2QsDict = function (obj, map, enclosingPropName) {
            map = map || {};
            if (IsNullOrUndefined(obj)) {
                map[enclosingPropName] = '';
            } else if (Array.isArray(obj)) {
                if (obj.length === 0) {
                    map[enclosingPropName] = '';
                } else {
                    for (let i = 0, j = 0; i < obj.length; i++) {
                        if (!IsNullOrUndefined(obj[i])) {
                            obj2QsDict(obj[i], map, enclosingPropName + '[' + j + ']');
                            j++;
                        }
                    }
                }
            } else if (typeof obj !== 'object') {// simple value
                map[enclosingPropName] = obj;
            } else if (obj instanceof Date) {// Date
                const adjusted = new Date(obj.getTime() + TimeZoneDiff);
                map[enclosingPropName] = adjusted.toISOString();
            } else { // object
                for (var prop of Object.keys(obj)) {
                    var pname = (enclosingPropName === '' ? '' : (enclosingPropName + '.')) + prop;
                    obj2QsDict(obj[prop], map, pname);
                }
            }
            return map;
        }
        /** 用于将js对象转换为级联格式的qs名值对 */
        var obj2Qs = function (obj) {
            if (!obj) { return ''; }
            const map = obj2QsDict(obj, {}, '');
            let qs = '';
            for (var prop in map) {
                qs = qs + '&' + prop + '=' + map[prop];
            }
            return qs.substring(1);
        }
        var SessionGet = function (key) {
            return $window.sessionStorage.getItem(key);
        }
        var SessionSet = function (key, value) {
            if (value == void 0 || value == null) {
                $window.sessionStorage.removeItem(key);
            } else {
                $window.sessionStorage.setItem(key, value);
            }
        };
        var IsNullOrUndefined = function (value) {
            return (value === null || value === void 0);
        };
        //如果没有第四个参数则只支持绝对路由
        var NavRoot = function (stateName, params, extras, controller) {
            stateName = makeFullRouteUrl(controller, stateName);
            params = params || {};
            params['ajax-nav'] = 'root';
            return $state.go(stateName, params, { inherit: false, reload: true });
        };
         //如果没有第四个参数则只支持绝对路由
        var NavForward = function (stateName, params, extras,controller) {
            stateName = makeFullRouteUrl(controller, stateName);
            params = params || {};
            params['ajax-nav'] = 'forward';
            return $state.go(stateName, params, { inherit: false, reload: true });
        };
        //同AjaxNavBack
        var NavBack = function () {
            AjaxNavBack();
        };
        //同AjaxNavRefresh
        var NavRefresh = function () {
            AjaxNavRefresh();
        };
        //如果没有第四个参数则只支持绝对路由. 
        var NavTo = function (stateName, params, extras, controller) {
            stateName = makeFullRouteUrl(controller, stateName);
            AjaxNavTo(null, params, stateName);
        };
        return {
            AjaxSubmit: AjaxSubmit,
            CallAction: CallAction,
            ExecuteResult: ExecuteResult,
            AjaxNavRegister: AjaxNavRegister,
            AjaxNavGetBackSrc: AjaxNavGetBackSrc,
            AjaxNavBack: AjaxNavBack,
            AjaxNavRefresh: AjaxNavRefresh,
            AjaxNavTo: AjaxNavTo,
            CreateRcResult: CreateRcResult,
            AjaxNavData: AjaxNavData,
            ParentForm: parentForm,
            ElementById: $,
            Super: Super,
            Validate: Validate,
            ValidateUtil: {
                SetPristine: SetPristine,
                Validate: ValidateAGroupOfCtrls
            },
            RegisterVmValidator: RegisterVmValidator,
            RegisterGroupedValidator: RegisterGroupedValidator,
            ToAjaxNav: ToAjaxNav,
            GetMyController: GetMyController,
            KendoBindValidate: KendoBindValidate,
            ShowCollectionItem: ShowCollectionItem,
            TabHandler: TabHandler,
            obj2Qs: obj2Qs,
            RemoveLoginToken: RemoveLoginToken,
            SetResultMarker: SetResultMarker,
            IsNullOrUndefined: IsNullOrUndefined,
            GetFormArray: GetFormArray,
            GetFormNameWithIndex: GetFormNameWithIndex,
            Url: {
                NavRoot: NavRoot,
                NavForward: NavForward,
                NavBack: NavBack,
                NavRefresh: NavRefresh,
                NavTo:NavTo
            },
            KendoValidator: KendoValidator
        };
    };
    pbFn.$inject = ['validationManager', '$http', '$state', '$translate', '$parse', '$window', '$injector', '$timeout', '$rootScope', 'pbui', 'App_MenuData', 'myCustomErrorMessageResolver', '$cookies', '$q', 'ProjectBaseGlobalVar'];
    var pbm = angular.module('projectbase');
    pbm.provider('pb', function () {
        var me = this;
        me.$get = pbFn;

        me.SetAjaxNavRootState = function (statename) {
            AjaxNavRoot.PrevState = statename;
        };
    });


}(String, angular));//end pack

if (!PB_Global_WebApiTestingInterceptor) {//如果不接入测试框架，定义空的Interceptor对象
    var PB_Global_WebApiTestingInterceptor = new function () {
        this.PutWebApi = function (request, responseData, status, headers, config) {
        };
        this.PutController = function (controller) {
        };
    }
}
