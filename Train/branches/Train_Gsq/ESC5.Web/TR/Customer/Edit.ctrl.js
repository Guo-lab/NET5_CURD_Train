def.ns = "/TR/Customer/";


def.ContentState('Edit', 'Id').Controller(['pb', 'serverVm', '$scope', function (pb, serverVm, $scope) {
    var c = pb.Super(this, serverVm, $scope);

    c.Save_click = function () {

        //---------- Former 
        //c.AjaxSubmit('c.frmEdit', null, { method: 'post' }, function (r) {
        //    if (r.IsNoop) {
        //        c.NavBack();
        //    }
        //});
        //
        //
        //---------- Whether to confirm
        c.AjaxSubmit('c.frmEdit', null, { confirm: '' }, function (r) {
            if (r.IsNoop) {
                pb.AjaxNavBack();
            }
        });
        //c.AjaxSubmit('c.frmEdit', null, { confirm: 'ConfirmSave' }, function (r) {
        //    if (r.IsNoop) {
        //        pb.AjaxNavBack();
        //    }
        //});
        
    }

    c.Back_click = function () {
        pb.AjaxNavBack();
    }

    // 验证ViewModel注册时 钩子刷新 注册Age > 10
    c.OnRegisterVmValidator = function () {
        pb.RegisterVmValidator(c.frmEdit,
            {
                ValGender: function (modelValue, viewValue) {
                    var value = modelValue || viewValue;
                    return value >= 0;
                },
                VmFormValidate: function () {
                    return null;
                }
            });
    }
}]);


def.ContentState('Add').WithController('Edit');

