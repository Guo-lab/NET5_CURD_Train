def.ns = "/TR/User/";
def.ContentState('List','t').Controller(['pb', 'serverVm', '$scope', function (pb, serverVm, $scope) {
    var c = pb.Super(this, serverVm, $scope);

    c.Search_click = function () {
        c.grdUser.AjaxRead();
    };
    c.Delete_click = function (id, name) {
        c.AjaxSubmit('c.frmSearch', 'Id=' + id, { url: 'Delete', confirm: 'ConfirmDelete', confirmSingle: name }, function (r) {
            c.grdUser.Bind(r.data);
        });
    }
    c.OnNavBackRefresh = function () {
        c.Search_click();
    }
}]);


