app.factory('App_FuncTree', [function () {
    var allowedFuncList = null;
    var forbiddenElementVisible = false;
    var GetCheckedListString = function (treeModel) {
        var s = [];
        angular.forEach(treeModel.funcList, function (val, idx) {
            if (treeModel.selectedList[idx] == true &&
                (angular.isUndefined(treeModel.disableList[idx]) || treeModel.disableList[idx] == false)) {
                s.push(val);
            }
        });
        s = s.join(',');
        return s;
    };
    var SetElementStatusByFunc = function (element, elementfunccode) {
        if (allowedFuncList != null && allowedFuncList.indexOf(',' + elementfunccode.toLowerCase() + ',') < 0) return;
        if (forbiddenElementVisible) {
            element.prop('disabled', true);
        } else {
            element.css('display', 'none');
        }
    };
    var SetAllowedFuncList = function (fcodes) {
        allowedFuncList = fcodes.toLowerCase();
    };
    var SetForbiddenElementVisible = function (visible) {
        forbiddenElementVisible = visible;
    };
    return {
        GetCheckedListString: GetCheckedListString,
        SetElementStatusByFunc: SetElementStatusByFunc,
        SetAllowedFuncList: SetAllowedFuncList,
        SetForbiddenElementVisible: SetForbiddenElementVisible
    };
}]);
app.factory('App_PlugIn', ['pb', function (pb) {
    var RefListContainItem = function (list, item) {
        if (item != undefined) {
            for (var i = 0; i < list.length; i++) {
                if (list[i].Id == item.Id) {
                    return true;
                }
            }
        }
        return false;
    };
    var GetMoreOptionsDefault = function () {
        return { "全部": -1 };
    };
    return {
        RefListContainItem: RefListContainItem,
        GetMoreOptionsDefault: GetMoreOptionsDefault
    };
}]);
