(function (String, angular) {
    'use strict';

    var pbm = angular.module('projectbase', ['ui.router', 'ui.bootstrap', 'jcs-autoValidate', 'pascalprecht.translate', 'ngCookies', 'ngAnimate']);
    pbm.config(['$httpProvider', function ($httpProvider) {
        if (!$httpProvider.defaults.headers.common) {
            $httpProvider.defaults.headers.common = {};
        }
        $httpProvider.defaults.headers.common["Cache-Control"] = "no-cache";
        $httpProvider.defaults.headers.common.Pragma = "no-cache";
    }]);
      //pbm.factory('ResponseCheckerForUiRouter', function($q,$window,$location) {
    //    return {
    //      'response': function(response) {
    //        return response;
    //      }
    //    };
    //  });
    //begin--------------------------------------------------------------------------------------------
    pbm.directive("pbInitvm", function () {//create an vm property for the element's controller instance
        var directiveDefinitionObject = {
            priority: 0,
            restrict: 'A',
            compile: function (tElement, tAttrs, transclude) {
                return {
                    pre: function (scope, element, attrs) {
                        element.controller().vm = {};
                        if (attrs["pbInitvm"] != "") {
                            angular.copy(angular.fromJson(attrs["pbInitvm"]), element.controller().vm);
                        }
                    },
                    post: function postLink(scope, iElement, iAttrs, controller) {

                    }
                };
            }
        };
        return directiveDefinitionObject;
    });
    //begin--------------------------------------------------------------------------------------------
    pbm.directive("pbInitData", function () {//create an vm property for the element's controller instance
        var directiveDefinitionObject = {
            priority: 0,
            restrict: 'A',
            compile: function (tElement, tAttrs, transclude) {
                return {
                    pre: function (scope, element, attrs) {
                        if (attrs["pbInitData"] != "" && attrs["pbInitVar"]) {
                            scope[attrs["pbInitVar"]]._initData = angular.fromJson(attrs["pbInitData"]);
                        }
                    },
                    post: function postLink(scope, iElement, iAttrs, controller) {

                    }
                };
            }
        };
        return directiveDefinitionObject;
    });
    //begin-----------------------缺省情况下ajax-url属性将form,a,button元素自动绑定点击事件为ajax提交---------------------------------------------------------------------
    pbm.directive("ajaxUrl", ['pb', '$rootScope', function factory(pb, $rootScope) {
        var directiveDefinitionObject = {
            priority: 0,
            restrict: 'A',
            compile: function compile(tElement, tAttrs, transclude) {
                return {
                    pre: function preLink(scope, iElement, iAttrs, controller) {
                    },
                    post: function postLink(scope, iElement, iAttrs, controller) {
                        if (iAttrs['pbFunc'] || iElement.prop('tagName') == 'FORM') {
                        } else {
                            var funccode = iAttrs['ajaxUrl'];
                            if (funccode == '') {
                                var f = pb.ParentForm(iElement);
                                funccode = f.attr('ajax-url');
                            }
                            if (funccode && funccode != '') {
                                var pos0 = funccode.lastIndexOf('?');
                                if (pos0 > 0) funccode = funccode.substring(0, pos0);
                                var keys = funccode.split('/');
                                var l = keys.length;
                                funccode = keys[l - 2] + '.' + keys[l - 1];
                                //iElement.attr('pb-func',funccode);can't be this way
                                $rootScope.pbvar.App_FuncTree.SetElementStatusByFunc(iElement, funccode);
                            }
                        }

                        if (iAttrs["ngClick"] || iAttrs["ngSubmit"]) return;
                        var submitFunc = function () {
                            pb.AjaxSubmit(iElement);
                        };
                        if (iElement.prop('tagName') == 'FORM') {
                            var kendoForm = iElement.data().kendoValidator;
                            if (kendoForm) {
                                pb.KendoBindValidate(scope, kendoForm);
                            }
                        } else if (iElement.prop('tagName') == 'BUTTON' || iElement.prop('tagName') == 'A') {
                            iElement.bind('click', submitFunc);
                        }
                    }
                };
            }
        };
        return directiveDefinitionObject;
    }]);

    //begin-------func onto element-------------------------------------------------------------------------------------
    pbm.directive("pbFunc", ['$rootScope', function ($rootScope) {
        var directiveDefinitionObject = {
            priority: 100,//after ajax-url
            restrict: 'A',
            link: function (scope, iElement, iAttrs) {
                $rootScope.pbvar.App_FuncTree.SetElementStatusByFunc(iElement, iAttrs['pbFunc']);
            }
        };
        return directiveDefinitionObject;
    }]);
    pbm.directive("uiSref", ['pb', '$rootScope', function factory(pb, $rootScope) {
        var directiveDefinitionObject = {
            priority: 0,
            restrict: 'A',
            compile: function compile(tElement, tAttrs, transclude) {
                return {
                    pre: function preLink(scope, iElement, iAttrs, controller) {
                    },
                    post: function postLink(scope, iElement, iAttrs, controller) {
                        if (iAttrs['pbFunc']) {
                        } else {
                            var funccode = iAttrs['uiSref'];
                            if (funccode && funccode != '') {
                                var pos0 = funccode.lastIndexOf('(');
                                if (pos0 > 0) funccode = funccode.substring(0, pos0);
                                var keys = funccode.split('/');
                                var l = keys.length;
                                funccode = keys[l - 2] + '.' + keys[l - 1];
                                $rootScope.pbvar.App_FuncTree.SetElementStatusByFunc(iElement, funccode);
                            }
                        }
                    }
                };
            }
        };
        return directiveDefinitionObject;
    }]);
    //end-------func onto element---------------------------------------------------------------
    pbm.directive("pbGroup", ['pb', function (pb) {
        var directiveDefinitionObject = {
            priority: 0,
            restrict: "A",
            require: ["^form", "ngModel"],
            link: function (scope, ele, attrs, ctrls) {
                if (!attrs.name) return;
                var formController = ctrls[0], ngModelController = ctrls[1];
                formController.PB_Group = formController.PB_Group || {};
                formController.PB_Group[attrs["pbGroup"]] = formController.PB_Group[attrs["pbGroup"]] || {};
                formController.PB_Group[attrs["pbGroup"]][attrs.name] = ele;
            }
        };
        return directiveDefinitionObject;
    }]);
    //begin---------filter-----------------------------------------------------------------------------------
    pbm.filter("Dict", ['$filter', 'App_Dict', function ($filter, App_Dict) {
        return function (value, dictName) {
            var obj = App_Dict[App_Dict.Language][dictName];
            for (var label in obj) {
                if (obj[label] == value) return label;
            }
            return value;
        };
    }]);
    pbm.filter('urlEncode', function () {
        return function (input) {
            if (input) {
                return window.encodeURIComponent(input);
            }
            return "";
        }
    });
    //将列表中加上一项：项数据为javascript对象{Id:1,RefText:'label'}}.只控件中加一项，原列表数据不受影响。
    pbm.filter("RefSelectEnforceMatch", ['$filter', 'App_PlugIn', function ($filter, App_PlugIn) {
        //var newlist = null;
        return function (listvalue, itemobject,property) {
            if (property == undefined) {
                property = "Id";
            }
            if (itemobject == undefined || App_PlugIn.RefListContainItem(listvalue, itemobject,property)) {
                return listvalue;
            }
            var newlist = angular.copy(listvalue);
            newlist[listvalue.length] = itemobject;
            return newlist;
        };
    }]);
    //将列表中加上一项或多项：项数据为javascript对象{labelasprop:valueasvalue}.只控件中加项，原列表数据不受影响。
    pbm.filter("SelectAddOptions", ['$filter', 'App_PlugIn', function ($filter, App_PlugIn) {
        //var newlist = null;
        return function (listvalue, itemobject) {
            //if (newlist) return newlist;
            var newlist = angular.copy(listvalue);
            if (!angular.isObject(itemobject)) {
                var opnobj = App_PlugIn.GetMoreOptionsDefault();
                for (var prop in opnobj) {
                    if (itemobject == opnobj[prop]) {
                        itemobject = {};
                        itemobject[prop] = opnobj[prop];
                        break;
                    }
                }
            }
            for (var prop in itemobject) {
                newlist[prop] = itemobject[prop];
            }
            return newlist;
        };
    }]);
    pbm.filter("Display", ['$filter', 'App_Dict', function ($filter, App_Dict) {//统一显示处理的接口
        return function (value, dictNameOrFormatOrFracSize, begin, end) {
            if (angular.isUndefined(value)) return '';
            if ((angular.isNumber(value) || typeof value == 'boolean') && dictNameOrFormatOrFracSize ) {
                if (angular.isString(dictNameOrFormatOrFracSize) && App_Dict[App_Dict.Language][dictNameOrFormatOrFracSize]) {//second param is dictname
                    return $filter('Dict')(value, dictNameOrFormatOrFracSize);
                } else if (angular.isNumber(dictNameOrFormatOrFracSize)) {
                    return $filter('number')(value, dictNameOrFormatOrFracSize);
                }
            }
            if (angular.isNumber(value) && !dictNameOrFormatOrFracSize && value % 1 != 0) {
                return $filter('number')(value, 2);//小数值默认2位小数位
            }
            if (value == true) return App_Dict[App_Dict.Language].TrueDisplay;
            if (value == false) return App_Dict[App_Dict.Language].FalseDisplay;
            if (angular.isString(value) && begin) return value.substring(begin, end);
            if (value && angular.isDefined(value.RefText)) {
                return value.RefText== null?'': value.RefText;
            }
            //now I assume it's a date value
            if (!dictNameOrFormatOrFracSize) {
                dictNameOrFormatOrFracSize = 'yyyy-MM-dd';
            } else if (dictNameOrFormatOrFracSize.toLowerCase() == 'dt' || dictNameOrFormatOrFracSize.toLowerCase() == 'datetime') {
                dictNameOrFormatOrFracSize = 'yyyy-MM-dd HH:mm:ss';
            } else if (dictNameOrFormatOrFracSize.toLowerCase() == 't' || dictNameOrFormatOrFracSize.toLowerCase() == 'time'){
                dictNameOrFormatOrFracSize = 'HH:mm:ss';
            }
            return $filter('date')(value, dictNameOrFormatOrFracSize);
        };
    }]);
    pbm.filter('trustAsHtml', function ($sce) {
        return function (html) {
            return $sce.trustAsHtml(html);
        };
    });
    pbm.filter('trustAsUrl', function ($sce) {
        return function (url) {
            return $sce.trustAsResourceUrl(url);
        };
    });
	pbm.filter('PercentValue', function () {
        return function (o) {
            if (o != undefined && /(^(-)*\d+\.\d*$)|(^(-)*\d+$)/.test(o)) {
                var v = parseFloat(o);
                return Number(Math.round(v * 10000) / 100).toFixed(0) + "%";
            } else {
                return "undefined";
            }
        }
    });
    pbm.filter('VerToChar', function () {
        return function (version) {
            return String.fromCharCode(64 + version);
        }
    });
    pbm.filter("ReIndex", ['$filter', function ($filter) {
        return function (index, array) {
            var cnt = index;
            for (var i = 0; i < cnt; i++) {
                if (angular.isUndefined(array[i])) index = index - 1;
            }
            return index;
        };
    }]);
    //begin-------pb-required=label-------------------------------------------------------------------------------------
    pbm.directive("pbRequired", [function () {
        var directiveDefinitionObject = {
            priority: 10000,
            // transclude: false,
            restrict: 'A',
            compile: function (tElement, tAttrs, transclude) {
                if (tAttrs['pbRequired'] != 'label') return;
                tElement.append('<span class="required">&nbsp;*&nbsp;</span>');
            }
        };
        return directiveDefinitionObject;
    }]);

     pbm.factory('pbTranslate', ['$q', 'defaultErrorMessageResolver', '$translate', function ($q, defaultErrorMessageResolver, $translate) {
        var TranslateVal = function (errorType, el) {
            var defer = $q.defer();
            var p = defaultErrorMessageResolver.resolve(errorType, el);
            errorType = errorType || '';
            var translatekey = el.attr('translatekey');
            var msgKey4Field = errorType + '_' + translatekey;
            var attrName = errorType.toLowerCase() + '-params';
            if (attrName.substr(0, 2) == 'pb') {
                attrName = 'pb-' + attrName.substring(2);
            }
            var params = el.attr(attrName);
            p.then(function (automsg) {
                var errorMsg = automsg;
                $translate([translatekey, errorMsg, msgKey4Field], params).then(function (translated) {
                    if (translated[msgKey4Field] != msgKey4Field) {
                        defer.resolve(translated[msgKey4Field]);
                    } else {
                        defer.resolve(translated[translatekey] + " " + translated[errorMsg]);
                    }
                }, function (notTranslatedMsgKey) {
                    defer.resolve(notTranslatedMsgKey[translatekey] + " " + notTranslatedMsgKey[errorMsg]);
                });
            });

            return defer.promise;
        };
        return {
            TranslateVal: TranslateVal
        };
    }]);


    function GetParamFromUrl(url, name) {
        if (url.indexOf('?') < 0) return null;
        var qs = url.split('?')[1];
        var pos0 = qs.indexOf(name + '=');
        if (pos0 < 0) return null;
        var pos1 = qs.indexOf('&', pos0);
        if (pos1 < 0) return qs.substr(pos0 + name.length + 1);
        return qs.substring(pos0 + name.length + 1, pos1);
    }
    String.prototype.endWith = function (s) {
        if (s == null || s == "" || this.length == 0 || s.length > this.length)
            return false;
        if (this.substring(this.length - s.length) == s)
            return true;
        else
            return false;
    };

    String.prototype.startWith = function (s) {
        if (s == null || s == "" || this.length == 0 || s.length > this.length)
            return false;
        if (this.substr(0, s.length) == s)
            return true;
        else
            return false;
    };

    //begin和end都是yyyy-mm-dd格式的日期字符串
    function DateDiff(begin, end) {
        var beginArr = begin.split("-");
        var endArr = end.split("-");
        var beginRDate = new Date(beginArr[0], beginArr[1], beginArr[2]);
        var endRDate = new Date(endArr[0], endArr[1], endArr[2]);
        var result = (endRDate - beginRDate) / (24 * 60 * 60 * 1000);
        return result;
        //这样得到的result即为两个日期之间相差的天数。
    };
    String.prototype.getBytesLength = function () {
        return this.replace(/[^\x00-\xff]/gi, "--").length;
    };
	
    Date.prototype.Format = function (fmt) {
        var o = {
            "M+": this.getMonth() + 1,                 //月份 
            "d+": this.getDate(),                    //日 
            "h+": this.getHours(),                   //小时 
            "m+": this.getMinutes(),                 //分 
            "s+": this.getSeconds(),                 //秒 
            "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
            "S": this.getMilliseconds()             //毫秒 
        };
        if (/(y+)/.test(fmt))
            fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
        for (var k in o)
            if (new RegExp("(" + k + ")").test(fmt))
                fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        return fmt;
    };
    Date.prototype.addMonths = function (m) {
        var date = new Date(this.valueOf());
        var d = this.getDate();
        date.setMonth(this.getMonth() + m);

        if (date.getDate() < d)
            this.setDate(0);
        return date;
    };
    Date.prototype.addDays = function (days) {
        var date = new Date(this.valueOf());
        date.setDate(date.getDate() + days);
        return date;
    }

    Number.prototype.round = function (decimalPlaces) {
        var p = Math.pow(10, decimalPlaces);
        return Math.round(this * p) / p;
    }
	
    Array.prototype.returnStr = function (arrayName) {
        var s = ""
        for (var i = 0; i < this.length; i++) {
            s += arrayName + "[" + i + "]=" + this[i] + "&";
        }
        s = s.substring(0, s.length - 1);
        return s;
    };
    Array.prototype.includes = function (searchElement, fromIndex) {
        fromIndex = fromIndex || 0;
        for (var i = fromIndex; i < this.length; i++) {
            if (this[i] == searchElement) return true;
        }
        return false;
    };
    Array.prototype.includesStringIgnoreCase = function (searchElement, fromIndex) {
        fromIndex = fromIndex || 0;
        for (var i = fromIndex; i < this.length; i++) {
            if (this[i].toLowerCase() == searchElement.toLowerCase()) return true;
        }
        return false;
    };
    //------------------------------------------------------------------------------->


}(String, angular));                         //end pack