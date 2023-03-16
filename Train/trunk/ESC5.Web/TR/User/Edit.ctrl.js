def.ns = "/TR/User/";
def.ContentState('Edit', 'Id').Controller(['pb', 'serverVm', '$scope', function (pb, serverVm, $scope) {
    var c = pb.Super(this, serverVm, $scope);

    c.Save_click=function () {
        c.AjaxSubmit('c.frmEdit', null, {method:'post'}, function (r) {
            if (r.IsNoop) {
                c.NavBack();
            }
        });
    }
    c.Back_click = function () {
        c.NavBack();
    }
    c.OnRegisterVmValidator = function () {
        pb.RegisterVmValidator(c.frmEdit,
            {
                ValAge: function (modelValue, viewValue) {
                    var value = modelValue || viewValue;
                    return value > 10;
                }
            });
    }
}]);
def.ContentState('Add').WithController('Edit');

