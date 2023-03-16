//<-----------------App setup-----------------------
//<--------------框架上的应用定义的全局变量，方便于脚本中引用各组件,仅此两个，
//不要再定义其他的全局变量，需要时应定义命名对象

var app = angular.module("app", ['kendo.directives', 'projectbase', 'uploader', 'ngCookies']);
//--------------Config StatesDefiner and Define Common States -------------------------------->
var def = new PB_Global_StateDefiner(app);
def.SetUrlContextPrefix('/Primary01');
def.SetUrlMappingPrefix('');
def.LayoutState('/Shared/ContentLayout.htm');
def.SetDefaultLayout('/Shared/ContentLayout');
//--------------------------------------------------->
//<-----------保存应用级别的全部全局变量与配置--------------------------
app.constant('App_DefaultState', '/Home/ShowLogin');
app.value('AppConst', {
    DefaultRuleGroup: '_D',
    AlwaysRuleGroup: '_A'
});
//----------------------config components-------------------------------------------------------------------------------
app.config(['$stateProvider', '$urlRouterProvider', '$httpProvider', '$translateProvider', 'pbProvider', 'App_DefaultState','ProjectBaseGlobalVar',
    function ($stateProvider, $urlRouterProvider, $httpProvider, $translateProvider, pbProvider, App_DefaultState, ProjectBaseGlobalVar) {
        $httpProvider.defaults.headers.common["X-Requested-With"] = "XMLHttpRequest";
        $httpProvider.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
        // $httpProvider.interceptors.push('ResponseCheckerForUiRouter');

        $translateProvider.useSanitizeValueStrategy('escape');
        $translateProvider.useLocalStorage();
        var lang = window.localStorage.lang || 'zh-CN';
        $translateProvider.preferredLanguage(lang);
        kendo.culture(lang);
        $translateProvider.useStaticFilesLoader({
            prefix: '../Scripts/lang/',
            suffix: '.json'
        });

        $urlRouterProvider.otherwise(App_DefaultState);
        def.RegisterStates($stateProvider);

        pbProvider.SetAjaxNavRootState('/Home/MainFrameLoggedIn');
        ProjectBaseGlobalVar.HttpErrorMinus1_Redirect = App_DefaultState;
    }]);
app.run(['validator', 'myCustomErrorMessageResolver', 'myCustomElementModifier', 'defaultErrorMessageResolver', '$rootScope', '$state', '$stateParams', 'App_FuncTree','AppConst','pb',
    function (validator, myCustomErrorMessageResolver, myCustomElementModifier, defaultErrorMessageResolver, $rootScope, $state, $stateParams, App_FuncTree, AppConst,pb) {
        validator.registerDomModifier(myCustomElementModifier.key, myCustomElementModifier);
        validator.setDefaultElementModifier(myCustomElementModifier.key);
        validator.defaultFormValidationOptions.forceValidation = true;
        defaultErrorMessageResolver.setI18nFileRootPath("../Scripts/lang");
        defaultErrorMessageResolver.setCulture("zh-cn");
        //validator.setErrorMessageResolver(defaultErrorMessageResolver.resolve);
        validator.setErrorMessageResolver(myCustomErrorMessageResolver.resolve);
        $rootScope.$state = $state;
        $rootScope.$stateParams = $stateParams;
    }]);

