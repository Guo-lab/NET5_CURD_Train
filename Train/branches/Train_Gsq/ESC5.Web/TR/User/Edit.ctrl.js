def.ns = "/TR/User/";


// Define a state: /TR/User/Edit �漰��һ��cshtml��һ��Edit Action
def.ContentState('Edit', 'Id').Controller(['pb', 'serverVm', '$scope', function (pb, serverVm, $scope) {
    var c = pb.Super(this, serverVm, $scope);

    c.Save_click=function () {
        c.AjaxSubmit('c.frmEdit', null, {method:'post'}, function (r) {
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
            ValAge: function (modelValue, viewValue) {
                var value = modelValue || viewValue;
                return value > 10;
            }
        });
    }
}]);


// ����һ��state��������ʹ����һ��state��controller����
// Add���state��ʹ��Edit��ҳ���Edit ��controller��
// ���������Ǵӷ����� Add Action��url��Ӧ TR/User/Add��ȡ�á�
def.ContentState('Add').WithController('Edit');

