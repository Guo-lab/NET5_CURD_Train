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

    // ��֤ViewModelע��ʱ ����ˢ�� ע��Age > 10
    c.OnRegisterVmValidator = function () {
        pb.RegisterVmValidator(c.frmEdit,
            {
                ValGender: function (modelValue, viewValue) {
                    var value = modelValue || viewValue;
                    return value >= 0;
                },
                ValDate: function (modelValue, v) {
                    console.log(c.vm.Input.RegisterDate);
                    var value = modelValue || v;
                    console.log(typeof value, value);
                    // ����
                    // VmInner ����������֤����������������Կ����Ⱥ�˳��
                    // ���˳����Ҫ��Ҳ���Էֳɶ��VmInner��ÿ������ֻ��֤һ������
                    return value != null;
                },

                ShouldValidateGroups: function () {
                    if (c.vm.Input.Gender > 1) {
                        return "_D,More";
                    }
                    return null;
                },

                // ��Ȼ���԰�������֤�߼���д��������֤��
                // ֻ����֤�߼�ȷʵ�漰����ֶ��Ҳ��׷ָ��ʱ�򣬲�ʹ��������֤��
                // ������ֻ֤�������ֶ���֤��ͨ����Ż�ִ�С�
                VmFormValidate: function () {
                    if (c.vm.Input.Email.Length + c.vm.Input.Name_.Length > c.vm.Input.Spending && !c.vm.Input.Active && c.vm.Input.RegisterDate <= DateTime.Now.AddDays(-9)) {
                        // Hints(Prompts) while failing to verify
                        return "FormData_invalid";
                    }
                    return null;
                }
            });
    }
}]);


def.ContentState('Add').WithController('Edit');







// --------------------------- Multi State ------------------------
def.ContentState('MultiEdit').Controller(['pb', 'serverVm', '$scope', function (pb, serverVm, $scope) {
    var c = pb.Super(this, serverVm, $scope);

    c.OnRegisterVmValidator = function () {
        pb.RegisterVmValidator('c.frmEdit', {
            ValGender: function(modelValue, v, index) {
                console.log(modelValue);
                console.log(c.vm.Input.Rows[index].Gender);
                return modelValue >= 0;
            },
            ValDate: function(modelValue, v, index) {
                console.log(c.vm.Input.Rows[index].RegisterDate);
                var value = modelValue;
                return value != null;
            },
            ShouldValidateGroups: function(index) {
                if (c.vm.Input.Rows[index].Gender > 1) {
                    return "_D,More";
                }
                return null;
            },
            VmFormValidate: function (index) {
                if (c.vm.Input.Rows[index].Email.Length + c.vm.Input.Rows[index].Name_.Length > c.vm.Input.Rows[index].Spending && !c.vm.Input.Rows[index].Active && c.vm.Input.Rows[index].RegisterDate <= DateTime.Now.AddDays(-9)) {
                    return "FormData_invalid";
                }
                return null;
            },
        }
        , c);
    };
    c.Save_click = function (formIndex) {
        c.AjaxSubmit('c.frmEdit', null, { formIndex: formIndex }, function (r) {
            if (r.IsNoop) {
                pb.AjaxNavBack();
            }
        });
    };
    c.MultiSave_click = function () {
        // url: public ActionResult MultiSave([Validate] CustomerMultiEditVM.MultiEditInput input) { return Noop(); }
        c.AjaxSubmit('c.frmEdit', null, { url: 'MultiSave' }, function (r) {
            if (r.IsNoop) {
                pb.AjaxNavBack();
            }
        });
    };

    c.Back_click = function () {
        pb.AjaxNavBack();
    };
}]);



