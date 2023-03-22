def.ns = "/TR/Customer/";


def.ContentState('Edit', 'Id').Controller(['pb', 'serverVm', '$scope', function (pb, serverVm, $scope) {
    var c = pb.Super(this, serverVm, $scope);

    c.Save_click = function () {
        c.AjaxSubmit('c.frmEdit', null, { method: 'post' }, function (r) {
            // ���ڷ��������ص���������Ϳ���ʹ�÷�����������ж�
            // �˴�IsNoop��Save Action���߼���ӦΪ�����ɹ�
            if (r.IsNoop) {
                c.NavBack();
            }
        });
    }

    c.Back_click = function () {
        // �л������мʽ����·���е���һstate��������ع����ɿ��֧��
        c.NavBack();
    }

    // ��֤ViewModelע��ʱ ����ˢ�� ע��Age > 10
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

