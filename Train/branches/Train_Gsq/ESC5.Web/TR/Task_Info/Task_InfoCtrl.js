def.ns = "/TR/Task_Info/";






def.ContentState('List').Controller(['pb', 'serverVm', '$scope','pbui', function (pb, serverVm, $scope,pbui) {
    var c = pb.Super(this, serverVm, $scope);

    c.Search_click = function () {
        c.grdTask.AjaxRead();
    };
    c.Delete_click = function (id, name) {
        pbui.Confirm('ConfirmDelete',name).then(confirmed => {
            if (confirmed) {
                c.AjaxSubmit('c.frmSearch', 'Id=' + id, null, function (r) {
                    c.grdTask.Bind(r.data);
                });
            }
        });
        c.AjaxSubmit(
            'c.frmSearch',
            'Id=' + id,
            {
                url: 'Delete',
                confirm: 'ConfirmDelete',
                confirmMulti: c.vm.Input.ListInput.SelectedValues
            },
            function (r) {
            c.grdTask.Bind(r.data);
        });
    };
    c.OnNavBackRefresh = function () {
        c.Search_click();
    };
}]);















def.ContentState('Edit', 'Id').Controller(['pb', 'serverVm', '$scope', function (pb, serverVm, $scope) {
    var c = pb.Super(this, serverVm, $scope);

    c.OnRegisterVmValidator = function () {
        pb.RegisterVmValidator(c.frmEdit, {
            ValScore: function (modelValue, v) {
                return c.vm.Input.Score > 0;
            },
            ShouldValidateGroups: function () {
                if (c.vm.Input.MaxItemCount > 10) {
                    return "_D,A";
                }
                return "A";
            },
            VmFormValidate:function () {
                //if (c.vm.Input.MaxItemCount < 100 && c.vm.Input.Score > 10 || c.vm.Input.Active && c.vm.Input.Score < 0) return null;
                //return "FormDataInValid";
                return null;
            }
        });
    };
    c.Save_click = function () {
        c.AjaxSubmit('c.frmEdit', null, null, function (r) {
            if (r.IsNoop) {
                pb.AjaxNavBack();
            }
        });
    };
    c.Back_click = function () {
        pb.AjaxNavBack();
    };
}]);







def.ContentState('Add').WithController('Edit');







def.ContentState('MultiEdit').Controller(['pb', 'serverVm', '$scope', function (pb, serverVm, $scope) {
    var c = pb.Super(this, serverVm, $scope);

    c.OnRegisterVmValidator = function () {
        pb.RegisterVmValidator('c.frmEdit', {
            ValScore: function (modelValue, v, index) {
                console.log(modelValue);
                console.log(c.vm.Input.Rows[index].Score);
                return modelValue > 0;
            },
            ShouldValidateGroups: function (index) {
                if (c.vm.Input.Rows[index].MaxItemCount > 10) {
                    return "_D,A";
                }
                return "A";
            },
            VmFormValidate: function (index) {
                //if (c.vm.Input.Rows[index].MaxItemCount < 100 && c.vm.Input.Rows[index].Score > 10 || c.vm.Input.Rows[index].Active && c.vm.Input.Rows[index].Score < 0) return null;
                //return "FormDataInValid";
                return null;
            }
        },c);
    };
    c.Save_click = function (formIndex) {
        c.AjaxSubmit('c.frmEdit', null, { formIndex: formIndex}, function (r) {
            if (r.IsNoop) {
                pb.AjaxNavBack();
            }
        });
    };
    c.MultiSave_click = function () {
        c.AjaxSubmit('c.frmEdit', null, { url: 'MultiSave'}, function (r) {
            if (r.IsNoop) {
                pb.AjaxNavBack();
            }
        });
    };
    c.Back_click = function () {
        pb.AjaxNavBack();
    };
}]);

