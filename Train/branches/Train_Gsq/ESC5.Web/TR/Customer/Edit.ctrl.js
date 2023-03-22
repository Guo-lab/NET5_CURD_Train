def.ns = "/TR/Customer/";


def.ContentState('Edit', 'Id').Controller(['pb', 'serverVm', '$scope', function (pb, serverVm, $scope) {
    var c = pb.Super(this, serverVm, $scope);

    c.Save_click = function () {
        c.AjaxSubmit('c.frmEdit', null, { method: 'post' }, function (r) {
            // 对于服务器返回的命令的类型可以使用方便的属性来判断
            // 此处IsNoop与Save Action中逻辑对应为新增成功
            if (r.IsNoop) {
                c.NavBack();
            }
        });
    }

    c.Back_click = function () {
        // 切换回面包屑式导航路径中的上一state。这个返回功能由框架支持
        c.NavBack();
    }

    // 验证ViewModel注册时 钩子刷新 注册Age > 10
    c.OnRegisterVmValidator = function () {
        pb.RegisterVmValidator(c.frmEdit,
            {
                ValGender: function (modelValue, viewValue) {
                    var value = modelValue || viewValue;
                    return value >= 0;
                }
            });
    }
}]);


def.ContentState('Add').WithController('Edit');

