var PB_Global_StateDefiner;

(function (String, angular) {
    'use strict';

    var ProjectBaseGlobalVar = {//这里的常量与服务器GlobalConstant一致以互相配合使用
        DefaultPopupWindowFeature: "height=600, width=600, top=50, left=300, toolbar=yes, menubar=no, scrollbars=yes, resizable=yes,location=no, status=no",
        AuthFailure: "_AuthFailure",
        AuthFailureMaskElementId: "divAuthFailureMask",
        KeyForViewModelOnly: "_ForViewModelOnly",
        KeyForForceViewName:"_ForceViewName",
        Command_Noop: "Noop",
        Command_Message: "Message",
        Command_Redirect: "Redirect",  //重定向到state
        Command_AppPage: "AppPage",//重新进入onepage app
        Command_ServerVM: "ServerVM",
        Command_ServerData: "ServerData",
        Command_AjaxNavVM: "AjaxNavVM",
        Command_BizException: "BizException",
        Command_Exception: "Exception",
        Command_HttpStatus: "HttpStatus",
        Command_NewWindow: "NewWindow", //打开新窗口
        UrlContextPrefix: "tobeset",
        UrlMappingPrefix: "tobeset",
        AutoControllerMarker: "PB_Auto",
        UnSubmitNameMarker: "_",
        VmMeta_DefaultRuleGroup: '_D',
        VmMeta_AlwaysRuleGroup: '_A',
        ValidationType_VmInnerValPrefix: 'vminner_',
        Token_Channel: 'Header',//可设置，缺省为Header
        HttpErrorMinus1_Redirect: '',//http错误状态为-1时重定向
        NAME_FORMTOKEN: "X-XSRF-TOKEN-FORM",
        NAME_LOGINTOKEN: "X-XSRF-TOKEN-LOGIN"
    };
    var pbm = angular.module('projectbase');
    pbm.constant('ProjectBaseGlobalVar', ProjectBaseGlobalVar);

    //begin-----------PB_Global_Definer:define state-----------------------------------------------
    var States = {};
    PB_Global_StateDefiner = function (app) {
        var me = this;
        var isViewRef = function (name) {
            return name.endWith('.htm') || name.endWith('.html') || name.endWith('.jsp') || name.endWith('.aspx') || name.endWith('.cshtml');
        }
        me.ns = "";
        me.DefaultLayout = "";
        me.tmpStates = {};//map
        me.MultiState = {};
        me.SetDefaultLayout = function (stateName) {
            me.DefaultLayout = stateName;
        };
        me.SetUrlContextPrefix = function (prefix) {
            ProjectBaseGlobalVar.UrlContextPrefix = prefix;
        };
        me.SetUrlMappingPrefix = function (prefix) {
            ProjectBaseGlobalVar.UrlMappingPrefix = prefix;
        };
        me.GetUrlContextPrefix = function () {
            return ProjectBaseGlobalVar.UrlContextPrefix;
        };
        me.GetUrlMappingPrefix = function () {
            return ProjectBaseGlobalVar.UrlMappingPrefix;
        };
        //create state or view config
        //ownerOfView:name of the owner state of the view
        me.CreateWrappedState = function (templateUrlOrRawState, params, parent, isHtm, ownerOfView, ajaxOption) {
            var isForView = ownerOfView ? true : false;
            var CustomizedResultGetter, ResultGetter, VmGetter, templateUrlFn, stateOrView, uri, templateUrl, controllerInternalName;
            if (angular.isString(templateUrlOrRawState)) {
                templateUrl = (templateUrlOrRawState.startWith('/') ? '' : me.ns) + templateUrlOrRawState;
                uri = isHtm ? templateUrl.substring(0, templateUrl.indexOf('.')) : templateUrl;
                if (uri.indexOf('?') > 0) uri = uri.substring(0, uri.indexOf('?'));
                if (isForView) {
                    stateOrView = {
                        name: isHtm ? templateUrlOrRawState.substring(0, templateUrlOrRawState.indexOf('.')) : templateUrlOrRawState,
                        //templateUrl:isHtm?templateUrl:null,
                        controller: ProjectBaseGlobalVar.AutoControllerMarker,
                        controllerAs: "c",
                        resolve: null
                    };
                    controllerInternalName = ownerOfView + ".views_" + stateOrView.name;
                } else {
                    stateOrView = {
                        name: uri,
                        url: uri + (params ? '?' + params : ""),
                        //templateUrl:isHtm?templateUrl:null,
                        controller: ProjectBaseGlobalVar.AutoControllerMarker,
                        controllerAs: "c",
                        params: {
                            'ajax-nav': {
                                value: undefined,
                                squash: true,
                                array: false
                            },
                            'ajax-nav-back': {
                                value: undefined,
                                squash: true,
                                array: false
                            },
                            'UrlTranslate': {
                                value: undefined,
                                squash: true,
                                array: false
                            }
                        },
                        reloadOnSearch: false,
                        resolve: null,
                        "abstract": false,
                        parent: typeof (parent) == 'undefined' ? me.DefaultLayout : parent
                    };
                    controllerInternalName = stateOrView.name;
                }
                if (isHtm) {
                    stateOrView.templateUrl = ProjectBaseGlobalVar.UrlContextPrefix + templateUrl;
                } else {
                    templateUrlFn = function () {
                        return ProjectBaseGlobalVar.UrlContextPrefix + templateUrl;//no qs to ensure get view from server only once and use cache thereafter
                    };
                    CustomizedResultGetter = ['pb', '$stateParams', function (pb, $stateParams) {//默认AjaxNavBack返回后使用cache
                        var src = pb.AjaxNavGetBackSrc(controllerInternalName, $stateParams);
                        if (src && src.IsBack) {
                            return src.LastData;
                        }
                        return false;
                    }];
                    ResultGetter = ['pb', 'customizedResultGetter', '$stateParams', function (pb, customizedResultGetter, $stateParams) {
                        if (customizedResultGetter) return customizedResultGetter;
                        var url = ProjectBaseGlobalVar.UrlContextPrefix + ProjectBaseGlobalVar.UrlMappingPrefix + uri;
                        if (!url) return null;
                        var p = angular.copy($stateParams);
                        delete p['ajax-nav'];
                        delete p['ajax-nav-back'];
                        delete p['UrlTranslate'];
                        if (ajaxOption && ajaxOption.method && ajaxOption.method.toLowerCase() == 'post') {
                            return pb.AjaxSubmit(null, null, { "ajax-url": url, "ajax-bind": "nobind", "ajax-method": "POST", "ajax-data": p }, function (responsedata) {
                                return responsedata;
                            });
                        }
                        return pb.AjaxSubmit(null, p, { "ajax-url": url, "ajax-bind": "nobind", "ajax-method": "GET" }, function (responsedata) {
                            return responsedata;
                        });
                    }];
                    VmGetter = ['pb', 'serverResult', function (pb, serverResult) {
                        pb.SetResultMarker(serverResult);
                        if (serverResult.IsVM) {
                            if (serverResult.data.ViewModel)
                                serverResult.data.ViewModel._FromResult = serverResult;
                            else
                                console.log("can't append _FromResult, serverResult.data.ViewModel is null");
                            return serverResult.data.ViewModel;
                        } else if (serverResult.isRcResult == false)
                            throw serverResult;//trigger state change error,make state change fail 
                        else
                            return {};
                    }];
                    stateOrView.templateUrl = templateUrlFn;
                    stateOrView.resolve = {
                        customizedResultGetter: CustomizedResultGetter,
                        serverResult: ResultGetter,
                        serverVm: VmGetter
                    };
                }
            } else {
                stateOrView = templateUrlOrRawState;
            }
            if (!isForView && stateOrView && !stateOrView.data) stateOrView.data = {};
            return {
                isForView: isForView,
                state: stateOrView,
                WithController: function (stateName) {//use the given state's controller and template,usually when getting data from a different url
                    var st, name;
                    if (this.isForView) {
                        st = this.OwnerState.views[stateName];
                        name = this.OwnerState.name + ".views_" + st.name;
                    } else {
                        stateName = (stateName.startWith('/') ? '' : me.ns) + stateName;
                        st = me.tmpStates[stateName];
                        name = st.name;
                    }
                    this.state.controller = st.controller;
                    this.state.templateUrl = st.templateUrl;
                    this.state.templateUrlFn = st.templateUrlFn;
                    var fn = st.controller;
                    var f = angular.isArray(fn) ? fn[fn.length - 1] : fn;
                    if (f) {//named controller can be used by other states/views.
                        f.prototype._UsedBy = f.prototype._UsedBy || {};
                        f.prototype._UsedBy[name] = true;
                    }
                    return this;
                },
                WithChild: function (childname, childparams) {
                    if (this.isForView) throw "WithChild is only for state to use";
                    var ishtm = isViewRef(childname);
                    var ws = me.CreateWrappedState(childname, childparams, this.state.name, ishtm);
                    me.tmpStates[ws.state.name] = ws.state;
                    this.state['abstract'] = true;
                    return ws;
                },
                Controller: function (fn) {
                    this.state.controller = fn;
                    var f = angular.isArray(fn) ? fn[fn.length - 1] : fn;
                    if (f) {
                        f.prototype._DefinedBy = this.isForView ?
                            this.OwnerState.name + ".views_" + this.state.name
                            : this.state.name;//name controller after state/view name
                    }
                    return this;
                },
                NoController: function () {
                    this.state.controller = null;
                    this.state.controllerAs = null;
                    return this;
                },
                NoResolve: function () {
                    this.state.resolve = null;
                    return this;
                },
                ResultGetter: function (fn) {
                    this.state.resolve.customizedResultGetter = fn;
                    return this;
                },
                NavBackRefresh: function () {//use this to force refresh(reenter state) after navback,otherwise no refresh
                    this.state.resolve.customizedResultGetter = function () { return false; };
                    return this;
                },
                ForceViewByAction: function (url) {
                    if (!url) url = this.state.name;
                    this.state.templateUrl = ProjectBaseGlobalVar.UrlContextPrefix + ProjectBaseGlobalVar.UrlMappingPrefix + url + "?" + ProjectBaseGlobalVar.KeyForViewModelOnly + "=false";
                    this.state.templateFn = null;
                    return this;
                },
                ForceView: function (viewName) {
                    var stateName = this.state.name;
                    if (this.OwnerState) {//multiViewState的brachview需要计算url
                        stateName = this.OwnerState.name; 
                    }
                    this.state.templateUrl = ProjectBaseGlobalVar.UrlContextPrefix + ProjectBaseGlobalVar.UrlMappingPrefix + stateName + "?" + ProjectBaseGlobalVar.KeyForForceViewName + '=' + viewName;
                    this.state.templateFn = null;
                    return this;
                },
                NoServerResolve: function () {
                    if (this.state.resolve) {
                        this.state.resolve.serverResult = function () { return null; };
                        this.state.resolve.serverVm = function () { return null; };
                    }
                    return this;
                },
                NoServerAction: function () {
                    if (this.state.resolve) {
                        this.state.resolve.serverResult = function () { return null; };
                        this.state.resolve.serverVm = function () { return null; };
                    }
                    return this;
                },
                BranchView: function (name, params) {
                    if (!this.isForView) throw "meant to be called by a wrapped view";
                    var isHtm = isViewRef(name);
                    var wrappedview = me.CreateWrappedState(name, params, undefined, isHtm, this.OwnerState.name);
                    this.OwnerState.views[wrappedview.state.name] = wrappedview.state;
                    wrappedview.OwnerState = this.OwnerState;
                    return wrappedview;
                }
            };
        };
        me.RootState = function (name, params, option) {
            var isHtm = isViewRef(name);
            var ws = me.CreateWrappedState(name, params, '', isHtm, undefined, option);
            me.tmpStates[ws.state.name] = ws.state;
            return ws;
        };
        me.LayoutState = function (name, params, parentOrOption) {
            var isHtm = isViewRef(name);
            var parent;
            if (!parentOrOption) {
                parent = '';
            } else if (typeof parentOrOption == 'object') {
                parent = parentOrOption.parent || '';
            } else {
                parent = parentOrOption;
            }
            var ws = me.CreateWrappedState(name, params, parent, isHtm, undefined, parentOrOption);
            ws.state['abstract'] = true;
            ws.state.controller = null;
            ws.state.resolve = null;
            me.tmpStates[ws.state.name] = ws.state;
            return ws;
        };
        me.ContentState = function (name, params, parentOrOption) {
            var isHtm = isViewRef(name);
            var parent;
            if (!parentOrOption) {
                parent = me.DefaultLayout;
            } else if (typeof parentOrOption == 'object') {
                parent = parentOrOption.parent || '';
            } else {
                parent = parentOrOption;
            }
            var ws = me.CreateWrappedState(name, params, parent, isHtm, undefined, parentOrOption);
            me.tmpStates[ws.state.name] = ws.state;
            return ws;
        };
        me.MultiViewsState = function (name, params, parentOrOption) {
            var isHtm = isViewRef(name);
            var parent;
            if (!parentOrOption) {
                parent = me.DefaultLayout;
            } else if (typeof parentOrOption == 'object') {
                parent = parentOrOption.parent || '';
            } else {
                parent = parentOrOption;
            }
            var ws = me.CreateWrappedState(name, params, parent, isHtm, undefined, parentOrOption);
            ws.state.name = "/MultiViewsLayout_" + ws.state.name.substring(1, ws.state.name.length);
            me.tmpStates[ws.state.name] = ws.state;
            ws.state['abstract'] = true;
            name = (name.startWith('/') ? '' : me.ns) + name;
            name = isHtm ? name.substring(0, name.indexOf('.')) : name;
            if (name.indexOf('?') > 0) name = name.substring(0, name.indexOf('?'));
            var childState = {
                name: name,
                url: name + (params ? '?' + params : ""),
                params: ws.state.params,
                reloadOnSearch: false,
                parent: ws.state.name,
                views: {}
            }
            ws.state.params = null;
            ws.state.url = ws.state.name;
            me.tmpStates[name] = childState;
            return {
                state: childState,
                BranchView: function (name, params, option) {
                    var isHtm = isViewRef(name);
                    var wrappedview = me.CreateWrappedState(name, params, undefined, isHtm, this.state.name, option);
                    wrappedview.OwnerState = this.state;
                    wrappedview.OwnerState.views[wrappedview.state.name] = wrappedview.state;
                    return wrappedview;
                },
                Layout: ws
            };
        };
        me.RegisterMultiState = function (key, state) {
            me.MultiState[key] = state;
        },
            me.GetMultiState = function (key) {
                return me.MultiState[key];
            },
            me.RawState = function (rawState) {
                var ws = me.CreateWrappedState(rawState);
                me.tmpStates[ws.state.name] = rawState;
                return ws;
            };
        me.RegisterImplicitControllers = function () {
            angular.forEach(me.tmpStates, function (st, name) {
                if (st.controller == ProjectBaseGlobalVar.AutoControllerMarker) {
                    if (st.resolve != null && st.resolve.serverVm)
                        st.controller = ['pb', 'serverVm', '$scope', function (pb, serverVm, $scope) {
                            pb.Super(this, serverVm, $scope);
                        }];
                    else
                        st.controller = ['pb', '$scope', function (pb, $scope) {
                            pb.Super(this, {}, $scope);
                        }];
                }
                if (st.views) {
                    angular.forEach(st.views, function (viewconfig, name) {
                        if (viewconfig.controller == ProjectBaseGlobalVar.AutoControllerMarker) {
                            if (viewconfig.resolve != null && viewconfig.resolve.serverVm)
                                viewconfig.controller = ['pb', 'serverVm', '$scope', function (pb, serverVm, $scope) {
                                    pb.Super(this, serverVm, $scope);
                                }];
                            else
                                viewconfig.controller = ['pb', '$scope', function (pb, $scope) {
                                    pb.Super(this, {}, $scope);
                                }];
                        }
                        var fn = viewconfig.controller;
                        var f = angular.isArray(fn) ? fn[fn.length - 1] : fn;
                        if (f) {
                            name = st.name + ".view_" + name;
                            f.prototype._DefinedBy = f.prototype._DefinedBy || name;//name controller after view name
                        }
                    });
                } else {
                    var fn = st.controller;
                    var f = angular.isArray(fn) ? fn[fn.length - 1] : fn;
                    if (f) {
                        f.prototype._DefinedBy = f.prototype._DefinedBy || st.name;//name controller after state name
                    }
                }
            });
        };
        me.RegisterStates = function (stateProvider) {
            angular.forEach(me.tmpStates, function (st, name) {
                stateProvider.state(st);
            });
            //me.tmpStates={};
            States = me.tmpStates;
        };
    };
    //end-----------PB_Global_Definer-----------------------------------------------
}(String, angular));//end pack
