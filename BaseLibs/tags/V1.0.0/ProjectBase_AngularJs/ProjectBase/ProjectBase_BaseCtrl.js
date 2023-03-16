(function (String, angular) {
    'use strict';

    var pbm = angular.module('projectbase');
    //application-wide logic
    pbm.controller("BaseCtrl", ['$rootScope', '$scope', '$window', '$state', 'pb', 'pbui', '$translate', '$log', 'App_Dict', 'App_MenuData', 'App_FuncTree',
        function ($rootScope, $scope, $window, $state, pb, pbui, $translate, $log, App_Dict, App_MenuData, App_FuncTree) {
            var c = this;
            $rootScope.Dict = App_Dict; //字典常量

            //App_Dict转化成对象
            var ConvertDict = function (lang) {
                $rootScope.DictObj = {};
                var dict = App_Dict[lang];
                App_Dict.Language = lang;
                for (var prop in dict) {
                    if (!angular.isObject(dict)) continue;
                    $rootScope.DictObj[prop] = [];
                    for (var p in dict[prop]) {
                        $rootScope.DictObj[prop].push({
                            Id: dict[prop][p],
                            Text: p
                        })
                    }
                }
            }
            $rootScope.pbvar = {};//应用程序对框架的设置
            $rootScope.pbvar.ShowProcessing = false;
            $rootScope.pbvar.ProcessingRect = {};
            $rootScope.pbvar.App_FuncTree = App_FuncTree;
            c.Lang = 'zh-cn'; //上次翻译使用的语言
            c.tmp = {};
            ConvertDict(c.Lang);
            c.SetLang = function (lang) {
                c.Lang = lang;
                ConvertDict(lang);
            };
            $rootScope.$on('$stateChangeStart',
                function (event, toState, toParams, fromState, fromParams) {
                    pb.AjaxNavRegister(toState, toParams, fromState, fromParams, true);
                    pbui.PutProcessing("_view");
                    pbui.PutProcessing(false);
                });
            $rootScope.$on('$stateChangeError',
                function (event, toState, toParams, fromState, fromParams, error) {
                    event.preventDefault();
                    if (error && error.isRcResult == false) {
                        //already done ExecuteErrorResult
                    } else {
                        $log.error(error.stack);
                    }
                    pbui.PutProcessing(false);
                });
            $rootScope.$on('$stateChangeSuccess',
                function (event, toState, toParams, fromState, fromParams) {
                    c.SyncMenuToState(toState);
                    pb.AjaxNavRegister(toState, toParams, fromState, fromParams);
                    c.TranslateStateName(toState, toParams);
                    pbui.PutProcessing(false);
                });

            c.TranslateStateName = function (state, stateParam) {
                if (state.data.TranslatedName != null && state.data.Lang == c.Lang) return;
                var key = state.name;
                var navTranslate = stateParam.UrlTranslate;
                if (navTranslate) key = key + '?' + navTranslate;
                $translate(key).then(function (translated) {
                    state.data.TranslatedName = translated;
                    state.data.Lang = c.Lang;
                }, function () {
                    var keys = state.name.split('/');
                    var l = keys.length;
                    $translate([keys[l - 1], keys[l - 2]]).then(function (translations) {
                        state.data.TranslatedName = translations[keys[l - 2]] + translations[keys[l - 1]];
                    });
                });
            };
            c.InitMenu = function (allowedFuncList, menuData) {
                var SetMenuItem = function (parentMenuItem, subMenuItem) {
                    var nav = subMenuItem.nav || 'root';

                    var tempSubCode = subMenuItem.funcCode.split('^');
                    var disabled = true;
                    for (var i = 0; i < tempSubCode.length; i++) {
                        if (tempSubCode[i] == "" || fcodes.indexOf(("," + tempSubCode[i] + ",").toLowerCase()) != -1) {
                            disabled = false;
                            break;
                        }
                    }

                    if (disabled == false) {
                        var link = "";
                        if (subMenuItem.pageLink) {
                            link = " ng-href='" + parentMenuItem.pageLink + "' target=_blank"
                        }
                        else {
                            if (subMenuItem.stateParam) {
                                subMenuItem.sref = subMenuItem.stateName + '({' + subMenuItem.stateParam + ',"ajax-nav":"' + nav + '"})';
                            } else {
                                subMenuItem.sref = subMenuItem.stateName + '({' + '"ajax-nav":"' + nav + '"})';
                            }
                            link = " ui-sref='" + subMenuItem.sref + "' ui-sref-opts='{reload:true,inherit:false}'"
                        }

                        var state = $state.get(subMenuItem.stateName);
                        if (state) {
                            state.data.SubMenu = subMenuItem;
                            state.data.Menu = parentMenuItem;
                            state.data.Translate = subMenuItem.stateTranslate;
                        }
                        return {
                            text: "<a style='display: block;' " + link + "><span translate='" + subMenuItem.text + "'></span></a>",
                            encoded: false,
                        };
                    } else {
                        return null;
                    }
                }
                var LoadSubMenu = function (parentMenuItem, topLevel) {
                    var m = null;
                    angular.forEach(parentMenuItem.subMenus, function (subMenuItem) {
                        var item;
                        if (subMenuItem.subMenus) {
                            item = LoadSubMenu(subMenuItem,false);
                        } else {
                            item = SetMenuItem(parentMenuItem, subMenuItem);
                        }
                        if (item != null) {
                            if (m == null) {
                                m = {
                                    text: topLevel?"<span translate='" + parentMenuItem.text + "'></span>":"<a translate='" + parentMenuItem.text + "'></a>",
                                    encoded: false,
                                    items: [item]
                                }
                            } else {
                                m.items.push(item);
                            }
                        }
                    })
                    return m;
                }
                c.allowedFuncList = allowedFuncList;
                c.menuData = menuData || App_MenuData;
                //c.currentSubMenus = null;
                var fcodes = allowedFuncList ? allowedFuncList.toLowerCase() : '';
                c.TopMenu = [];
                c.LeftMenu = [];

                angular.forEach(c.menuData, function (menuitem) {
                    var m=null;
                    if (menuitem.subMenus) {
                        m = LoadSubMenu(menuitem,true);
                    }
                    if (m!=null) {
                        if (menuitem.position == "top") {
                            c.TopMenu.push(m);
                        } else {
                            c.LeftMenu.push(m);
                        }
                    }
                });
            };
            c.SyncMenuToState = function (activeState) {
                //angular.forEach(c.menuData, function (topmenuitem, index) {
                //if (!topmenuitem.status && topmenuitem == activeState.data.Menu) {
                //topmenuitem.status = 'active'; 
                //c.currentSubMenus = topmenuitem.subMenus;
                //}
                //if (topmenuitem.status == 'active' && topmenuitem != activeState.data.Menu) topmenuitem.status = '';
                //angular.forEach(topmenuitem.subMenus, function (submenuitem, subindex) {
                //if (!submenuitem.status && submenuitem == activeState.data.SubMenu) submenuitem.status = 'active';
                //if (submenuitem.status == 'active' && submenuitem != activeState.data.SubMenu) submenuitem.status = '';
                //    }); 
                //});
            };
            c.test = function (any) {
                $window.alert('bingo');
            };
        }]);

}(String, angular));                         //end pack