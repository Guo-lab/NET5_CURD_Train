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
            var command = rcJsonResult.command;
            if (command == ProjectBaseGlobalVar.Command_ServerVM || rcJsonResult.command == ProjectBaseGlobalVar.Command_AjaxNavVM) rcJsonResult.IsVM = true;
            else if (command == ProjectBaseGlobalVar.Command_Message) rcJsonResult.IsMsg = true;
            else if (command == ProjectBaseGlobalVar.Command_ServerData) rcJsonResult.IsData = true;
            else if (command == ProjectBaseGlobalVar.Command_Noop) rcJsonResult.IsNoop = true;
            else if (command == ProjectBaseGlobalVar.Command_Redirect) rcJsonResult.IsRedirect = true;
            else if (command == ProjectBaseGlobalVar.Command_AppPage) rcJsonResult.IsAppPage = true;
        };

        //begin---------AjaxSubmit 主过程
        /**
         *  提交表单
         * @param {any} control 控件或表单的id或name
         * @param {any} qsOrBodyData：没有表单时，此项作为qs，有表单时，此项如果是字符串，则作为qs参数；如果是对象，则作为bodyData
         * @param {any} ajaxOpn:配置参数，其中的ajax-data，如果是字符串（名值对），则作为bodyData参数；如果是对象，则自动转换为名值对后作为bodyData
         * @param {any} funcExecuteResult
         * @param {any} argsForFunc
         * @param {any} overrideSuper
         */
        var AjaxSubmit = function (control, qsOrBodyData, ajaxOpn, funcExecuteResult, argsForFunc, overrideSuper) {
            var confirmText, confirmParams = '';

            if (control == null) control = undefined;
            if (control) {
                control = $(control);
                if (control.length > 1) control = control.eq(0);
                if (control.length < 1) {
                    $window.alert("param control's indicated no control");
                    return;
                }
            }

            if (typeof ($(control).attr('ajax-confirm')) != 'undefined'
                || typeof ($(control).attr('ajax-confirm-single')) != 'undefined'
                || typeof ($(control).attr('ajax-confirm-multi')) != 'undefined') {
                confirmText = $(control).attr('ajax-confirm');
                if (typeof ($(control).attr('ajax-confirm-single')) != 'undefined') {
                    confirmParams = $(control).attr('ajax-confirm-single');
                } else if (typeof ($(control).attr('ajax-confirm-multi')) != 'undefined') {
                    var parseFunc = $parse($(control).attr('ajax-confirm-multi'));
                    var a = parseFunc($(control).scope());
                    confirmParams = HasSelectedRows(a);
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
                bindTargetControllerInstance = $(control).parent().controller();
                if (bindTargetControllerInstance == null) bindTargetControllerInstance = $(form).controller();
                if (bindTargetControllerInstance == null) {
                    $window.alert('ajax-bind=parent targets no controller!');
                    return;
                }
            } else if (typeof (bindTargetId) == 'undefined' || bindTargetId == '') {
                bindTargetControllerInstance = $(control).controller();
                if (bindTargetControllerInstance == null) bindTargetControllerInstance = $(form).controller();
                if (bindTargetControllerInstance == null) {
                    $window.alert('an default ajax-bind targets no controller!');
                    return;
                }
            }

            if (autoBind && bindTargetControllerInstance == null) {
                if (typeof (processingAreaId) == 'undefined') processingAreaId = bindTargetId;
                if (typeof ($(bindTargetId).attr('ui-view')) != 'undefined') {
                    bindTargetControllerInstance = $(bindTargetId).children().eq(0).controller();
                    if (bindTargetControllerInstance == null) {
                        $window.alert('bindTargetId:' + bindTargetId + ' targets a ui-view which has not a controller!');
                        return;
                    }
                } else {
                    $window.alert('bindTargetId:' + bindTargetId + ' should target an ui-view !');
                    return;
                }
            }
            var controller = (ajaxOpn ? ajaxOpn.controller : void 0) || $(control).controller();
            var formData;
            if ($(form).length > 0) {
                var errmsg = Validate(form, valgroup);
                if (errmsg) {
                    // this.NotifyValError(ajaxConfig.senderId as string, errmsg);
                    return new Promise(function (resolve, reject) {
                        resolve(new Error(errmsg));
                    });
                }
                formData = SerializeAngularForm(form);
            }
            if (ajaxData && typeof ajaxData == 'object') {
                ajaxData = obj2Qs(ajaxData);
            }
            if (formData && !ajaxData && qsOrBodyData && typeof qsOrBodyData == 'object') {
                ajaxData = obj2Qs(qsOrBodyData);
                qsOrBodyData = void 0;
            }
            ajaxBodyData = MergeQS(formData, ajaxData);
            url = PrefixUrl(url);
            var urlfull = url.split('?');
            url = urlfull[0] + '?' + ProjectBaseGlobalVar.KeyForViewModelOnly + '=true' + (urlfull[1] ? '&' + urlfull[1] : '');

            return doSubmit(form, url, method, qsOrBodyData, ajaxBodyData, funcExecuteResult, argsForFunc, overrideSuper,
                processingAreaId, bindTargetControllerInstance, controller, $(control).attr('Id'), null, autoBind);
        };
        var doSubmit = function (form, url, method, qs, ajaxBodyData, funcExecuteResult, argsForFunc, overrideSuper,
            processingAreaId, bindTargetControllerInstance, controller, senderId, formToken, autoBind) {
            var useKendoProcessing = !processingAreaId && (!form || form && !form.$name);
            if (processingAreaId !== false) {
                if (useKendoProcessing) {
                    pbui.PutKendoProcessing(true);
                } else {
                    pbui.PutProcessing(processingAreaId);
                }
            }

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
            var promise = $http({ method: method, url: url, data: ajaxBodyData, params: qs, headers })
                .success(function (responseData, status, headers, config) {
                    SetResultMarker(responseData);
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
                    pbui.PutKendoProcessing(false);
                })
                .error(function (responseData, status, headers, config, statusText) {
                    ExecuteHttpError(responseData, status, headers, config);
                    pbui.PutProcessing(false);
                    pbui.PutKendoProcessing(false);
                });
            if (!funcExecuteResult) return promise;
            return promise.then(function (response) {
                pbui.PutProcessing(false);
                pbui.PutKendoProcessing(false);
                if (response.data.isRcResult == true)
                    return funcExecuteResult(response.data, argsForFunc);
                return response.data;
            });
        }
        /**
         * 发送调用服务器Action的请求
         * @param {any} url  可带qs
         * @param {any} bodyData  作为bodyData传递的参数，可以是名值对字符串，如果是对象，则自动转换为名值对
         * @param {any} funcExecuteResult
         * @param {any} argsForFunc
         * @param {any} overrideSuperOrAjaxOpn
         */
        var CallAction = function (url, bodyData, funcExecuteResult, argsForFunc, overrideSuperOrAjaxOpn) {
            url = PrefixUrl(url);
            var urlfull = url.split('?');
            url = urlfull[0] + '?' + ProjectBaseGlobalVar.KeyForViewModelOnly + '=true' + (urlfull[1] ? '&' + urlfull[1] : '');
            if (bodyData && typeof bodyData == 'object') {
                bodyData = obj2Qs(bodyData);
            }
            var overrideSuper, ajaxOpn, ajaxBodyData;
            ajaxBodyData = bodyData;
            if (typeof (overrideSuperOrAjaxOpn) == 'object') {
                ajaxOpn = overrideSuperOrAjaxOpn;
            } else {
                overrideSuper = overrideSuperOrAjaxOpn;
            }
            var formToken, controller, processingAreaId;
            if (ajaxOpn) {
                formToken = ajaxOpn.formToken;
                controller = ajaxOpn.controller;
                processingAreaId = ajaxOpn.processing;
            }
            return doSubmit(null, url, 'Post', null, ajaxBodyData, funcExecuteResult, argsForFunc, overrideSuper, processingAreaId, null, controller, null, formToken);
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
        var SerializeAngularForm = function (jqForm) {
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
                        var name = angular.element(control).attr('name');
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
            } else if (rcJsonResult.data) {
                var data, params;
                var index = rcJsonResult.data.indexOf(':');
                if (index > 0) {
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
                                msg=msg.replace("{" + index + "}", result[key]);
                                index += 1;
                            }
                            pbui.ShowCommand(rcJsonResult.command, msg);
                        })
                    }
                },
                function (notTranslatedMsgKey) {
                        pbui.ShowCommand(rcJsonResult.command, notTranslatedMsgKey);
                });
            } else {
                throw new Error(rcJsonResult.command);
            }

        };
        var ExecuteHttpError = function (data, status, headers, config) {
            if (status == '-1') {
                var url = ProjectBaseGlobalVar.HttpErrorMinus1_Redirect;
                if (url) {
                    if (url.toLowerCase().startWith('http')) {
                        $window.location = url;
                    } else {
                        $state.go(url);
                    }
                    return;
                }
            }
            if (status == "500")
                pbui.Alert("UnexpectedError");
            else
                pbui.Alert(status);
        };
        var ShowMessage = function (msgKey) {
            $translate(msgKey).then(function (msg) {
                $window.alert(msg);
            }, function (notTranslatedMsgKey) {
                $window.alert(notTranslatedMsgKey);
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
        * validate a form. 绑定相应验证规则并执行。
        *  @param: form:可以是字符串也可以是jquery对象或直接的form对象
        *  @param: valGroups: 不指定则表示不涉及分组验证，'none'表示不验证, 涉及分组验证则必须指出组名，包括缺省组也要指出
        *  @param: partialValid: 要验证的部分子控件名，仅支持一级。缺省时验证form中的所有子控件
        */
        var Validate = function (form, valGroups, angularFormController) {
            var formIsObj = false;
            if (typeof form == 'string') {
                form = $(form);
            }
            if (form.length) {
                form = form.eq(0);
                angularFormController = $(form).controller('form');
            } else {
                formIsObj = true;
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
        var AutoValidatorValidate = function (formJqOrCtrl, valGroups, formIsObj, angularFormController) {
            if (valGroups == 'none') return true;

            var formCtrl, formElement;
            if (formIsObj) {
                formCtrl = formJqOrCtrl;
            } else {
                formElement = formJqOrCtrl;
                var fname = formElement[0].name;
                var scope = $(formElement).scope();
                var formCtrl = $parse(fname)(scope);
            }
            if (!formCtrl) {
                throw new Error('未找到表单对象');
            }

            formCtrl.pbCurrentValGroups = valGroups;
            var ok = true;
            if (formCtrl.validate) {
                formCtrl.validate();
                ok = Object.keys(formCtrl._errors).length == 0;
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
                if (vmFormValidateFunc) {
                    var msg = vmFormValidateFunc();
                    if (msg == null) return true;
                    alert(msg);
                    return false;
                }
            }
            if (!ok) {
                validationManager.validateForm(formElement);
                return false;
            }
            return true;
        };

        /**
         * 
         * @param {any} scope
         * @param {any} attrs
         * @param {any} ngModelController 为空则是kendo系
         * @param {any} validatorName 验证类型名
         * @param {any} validator:函数function (modelValue, viewValue)
         */
        var RegisterGroupedValidator = function (scope, ele, attrs, ngModelController, validatorName, validator) {
            var theform;
            if (ngModelController) {
                theform = ngModelController.$$parentForm;
            } else if (ele[0].form) {
                theform = $parse(ele[0].form.name)(scope);
            }
            if (!theform) throw new Error('找不到输入控件所在表单：' + ele[0].name);
            var formName = theform.$name;
            var ctrlName = attrs.name;
            if (ctrlName.substring(0, 1) == ProjectBaseGlobalVar.UnSubmitNameMarker) return;
            var rulegroups = attrs[validatorName + "Groups"];
            var belongToDefault = !rulegroups;
            if (belongToDefault) {
                rulegroups = ProjectBaseGlobalVar.VmMeta_DefaultRuleGroup;
            }
            rulegroups = rulegroups.split(',');
            var controller = $parse(formName.split('.')[0])($(ele[0]).scope());//此时form还是angular form,但ele是kendo ele。form后面会被kendo form替代
            var validatorFunc = function (modelValue, viewValue) {
                if (!controller.pbRegisterVmValidatorDone && controller.OnRegisterVmValidator) {
                    controller.OnRegisterVmValidator();
                    controller.pbRegisterVmValidatorDone = true;
                }
                var form = $parse(formName)(scope);//由于kendo form记载后会替换掉原来的form对象，所以此处不能使用原来的form对象而是找到新的form对象
                var shouldVal = false;
                var valGroups = form.pbCurrentValGroups || scope.pbCurrentValGroups;
                if (!valGroups && form.pbValidateWhen) {
                    valGroups = form.pbValidateWhen();
                }
                if (!valGroups && belongToDefault) {
                    shouldVal = true;
                } else {
                    if (!valGroups) {
                        valGroups = ProjectBaseGlobalVar.VmMeta_DefaultRuleGroup;
                    }
                    if (valGroups) {
                        valGroups = valGroups.split(',');
                    }
                    angular.forEach(rulegroups, function (v, i) {
                        if (valGroups.includes(v)) {
                            shouldVal = true;
                            return true;
                        }
                    });
                }
                if (shouldVal) {
                    return validator(modelValue, viewValue);
                } else {
                    return true;
                }
            };

            if (ngModelController) {
                ngModelController.$validators[validatorName] = validatorFunc;
            } else {
                controller.pbKendoForms = controller.pbKendoForms || {};
                controller.pbKendoForms[formName] = controller.pbKendoForms[formName] || {};
                var pbKendoForm = controller.pbKendoForms[formName];
                pbKendoForm.pbKendoModelControllers = pbKendoForm.pbKendoModelControllers || {};
                pbKendoForm.pbKendoModelControllers[ctrlName] = pbKendoForm.pbKendoModelControllers[ctrlName] || { validators: {} };
                pbKendoForm.pbKendoModelControllers[ctrlName].validators[validatorName] = validatorFunc;
            }
        };
        var BuildGroupedValidatorAsCustomRuleForKendo = function (kendoForm, scope, formName) {
            return function (input) {
                if (!input[0].name) return true;
                var ctrlName = input[0].name;
                var controller = $parse(formName.split('.')[0])(scope);//此时form是kendo form
                if (!controller.pbKendoForms) return true;
                var pbKendoForm = controller.pbKendoForms[kendoForm.element[0].name];
                if (!pbKendoForm.pbKendoModelControllers || !pbKendoForm.pbKendoModelControllers[ctrlName]
                    || !pbKendoForm.pbKendoModelControllers[ctrlName].validators) return true;
                var validators = pbKendoForm.pbKendoModelControllers[ctrlName].validators;
                for (var vType in validators) {
                    var func = validators[vType];
                    var ok = func(void 0, input[0].value);
                    if (!ok) {
                        var ctl = getTooltipControl(input);
                        myCustomErrorMessageResolver.resolve(vType, input, ctl);
                        return false;
                    }
                }
                return true;
            }
        };
        //form: form对象
        var RegisterVmValidator = function (form, validatorRegistrar) {
            if (!form) {
                throw new Error('registerVmInnerValidator方法应在OnViewContentLoaded中使用，以保证form对象已创建');
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
         * 
         * @param {any} form
         * @param {any} name:与服务器方法名对应
         * @param {any} validator:函数function (modelValue, viewValue)
         * @param {form} vmFormValidator:函数用于整体验证
         */
        var RegisterVmInnerValidator = function (form, name, validator, vmFormValidator) {
            if (!form) {
                throw new Error('registerVmInnerValidator方法应在OnViewContentLoaded中使用，以保证form对象已创建');
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
        var ValidateWhen = function (formCtrl, funcShouldValidateGroups) {
            formCtrl.pbValidateWhen = funcShouldValidateGroups;
        };
        var KendoValidator = function ($scope, formName) {
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
        var KendoValidate = function ($scope, formName) {
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

            $scope.$on("kendoRendered", function (e) {
                KendoValidator($scope, formName).bind("validateInput", function (e) {
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
                var kendoForm = KendoValidator($scope, formName);
                kendoForm.options.rules.pbCustomValidator = BuildGroupedValidatorAsCustomRuleForKendo(kendoForm, $scope, formName);
            });
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
            //console.log('reg:'+fromState.name+'--'+toState.name+'--'+(onleaving?'leave':'enter'));
            //记录页面跳转
            //var StateFromTo = [];
            //if ($cookies.get('StateFromTo')) {
            //    StateFromTo = JSON.parse(decodeURIComponent($cookies.get('StateFromTo')));
            //}
            //StateFromTo.push({ Statefrom: fromState.name, StateTo: toState.name });
            //$cookies.remove('StateFromTo');
            //$cookies.put('StateFromTo', encodeURIComponent(JSON.stringify(StateFromTo)));
            //刷新导航
            var isRefresh = false;
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
            if (fromState == toState || toState.name == AjaxNavRoot.PrevState) return;
            //进入已记录路径中任一个,栈中已经被AjaxNav弹出后面的因而最后一个就是当前进入的，不需要再重复记录
            if (AjaxNavData.Stack.length > 0 && toState.name == AjaxNavData.Stack[AjaxNavData.Stack.length - 1].State.name) return;

            var tonav, map = {};
            if (toParams && toParams['ajax-nav']) {
                tonav = angular.lowercase(toParams['ajax-nav']);
            }

            var tonavback = angular.lowercase(toParams['ajax-nav-back']);
            if (onleaving) {//from是当前所在，根据去向决定是否需要将数据保存
                if (tonav == 'forward') {
                    if (AjaxNavData.Stack.length == 1 && AjaxNavData.Stack[0].State == null) {
                        AjaxNavData.Stack[0].State = fromState;
                    }
                    var lastnav = AjaxNavData.Stack[AjaxNavData.Stack.length - 1];
                    if (fromState == lastnav.State) {//should always be true
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
                        //lastnav.StateData = map;
                        //console.log('remember:'+fromState.name);
                    }
                }
                AjaxNavData.ControllerDataMapForCache = {};
                //console.log('leaving:'+fromState.name);
            } else { //to是当前所在，要记录上一
                /*  if (tonavback) return;//从路径上返回时不需要注册*/


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
            console.log(AjaxNavData);
            var state = AjaxNavData.Stack[AjaxNavData.Stack.length - 1].State;
            var stateParams = AjaxNavData.Stack[AjaxNavData.Stack.length - 1].StateParams;
            $state.go(state.name, stateParams, { inherit: false, reload: true });
        };
        var AjaxNavTo = function (state, stateParams) {
            while (AjaxNavData.Stack.length > 0 && state != AjaxNavData.Stack[AjaxNavData.Stack.length - 1].State) {
                AjaxNavData.Stack.pop();
                ToAjaxNav.Stack = { State: [], StateParams: [] };
                ToAjaxNav.Stack.State.push(AjaxNavData.Stack[AjaxNavData.Stack.length - 1].State);
                ToAjaxNav.Stack.StateParams.push(AjaxNavData.Stack[AjaxNavData.Stack.length - 1].StateParams);
            }
            $state.go(state.name, stateParams, { inherit: false, reload: true });
        };
        var AjaxNavBack = function () {

            var last = AjaxNavData.Stack.pop();
            var params;
            if (last.PrevStateParams) {
                params = angular.copy(last.PrevStateParams);
                params['ajax-nav-back'] = 'back:' + $state.current.name;
            }
            $state.go(last.PrevState.name, params, { inherit: false, reload: true });

        };
        var AjaxNavGetBackSrc = function (stateName, stateParams) {
            //var source = stateParams ? stateParams['ajax-nav-back'] : undefined;
            //var isBack = source && source.startWith ? source.startWith('back:') : false;
            //if (source && isBack) {
            //    var t = CreateRcResult();
            //    t.data.ViewModel = AjaxNavData.Stack[AjaxNavData.Stack.length - 1].StateData[stateName];
            //    return {
            //        IsBack: isBack,
            //        BackFrom: isBack ? source.substring(5, source.length) : undefined,
            //        LastData: t
            //    };
            //} else {
            //    return false;
            //}
            return false;
        };
        var CreateRcResult = function (result, command, data) {
            if (!command && !data) {
                command = ProjectBaseGlobalVar.Command_AjaxNavVM;
                data = { ViewModel: null };
            }
            return {
                result: result,
                command: command,
                data: data
            };
        };
        //end---------AjaxNav-------------------------------------------------------------------

        var Super = function (controller, serverVm, scope, hookviewContentLoaded) {
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

            var fn = controller.__proto__._NavBackRefresh;
            if (fn) {
                scope.$on('$viewContentLoaded', function (event) {//返回会自动callback执行动作以刷新页面数据
                    if (serverVm._FromResult && serverVm._FromResult.command == ProjectBaseGlobalVar.Command_AjaxNavVM) {
                        $timeout(function () { $injector.invoke(fn); });
                    }
                });
            }
            if (hookviewContentLoaded) {
                scope.$on('$viewContentLoaded', function (event) {
                    if (angular.isFunction(controller.OnViewContentLoaded)) {
                        controller.OnViewContentLoaded(event);
                    } else {
                        throw new Error('未定义作为Hook的OnViewContentLoaded方法');
                    }
                });
            }
            return controller;
        };

        var MergeQS = function (qs1, qs2) {//用第二个参数覆盖第一个
            if (typeof (qs1) == 'undefined') {
                if (typeof (qs2) == 'undefined') {
                    return '';
                }
                return qs2;
            } else {
                if (typeof (qs2) == 'undefined') {
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
        var ShowCollectionItem = function (container, key, url, eventName) {
            if (container[key] == undefined) {
                CallAction(url, null, function (ret) {
                    container[key] = ret.data;
                    $rootScope.$emit(eventName, ret.data);
                })
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
        }
        var IsNullOrUndefined = function (value) {
            return (value === null || value === void 0);
        }
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
            KendoValidator: KendoValidator,
            KendoValidate: KendoValidate,
            ShowCollectionItem: ShowCollectionItem,
            obj2Qs: obj2Qs,
            RemoveLoginToken: RemoveLoginToken,
            SetResultMarker: SetResultMarker,
            IsNullOrUndefined: IsNullOrUndefined
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
