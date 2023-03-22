def.ns = "/TR/Customer/";


// ���������ڲ�������Ҫ���������г��ֵ�ҳ���Ӧ�� State ʹ�� ContentState ����������
// State/controller define, in Controller: ������ı�����������Ҫ�������̶������� pb, serverVm, $scope(Angular)
// pb�ǿ���ṩ�ķ���serverVmΪ����ṩ������--�ӷ��������ص����ݶ���(����ṩ�ӷ��������ͻ��˵����ݰ�)
def.ContentState('List', 't').Controller(['pb', 'serverVm', '$scope', function (pb, serverVm, $scope) {

    var c = pb.Super(this, serverVm, $scope);

    // ��c�¶����¼�������
    c.Search_click = function () {
        // kendo grid �ڿ��ָ��֧���£�ʹ�ñ�����(c.grdUser)��AjaxRead�������ύ������ڱ���������ѯ��������ҳ����������ݣ�
        // ���ӷ�������ȡ��ѯ������ݡ�
        c.grdUser.AjaxRead();
    };
    c.Delete_click = function (id, name) {
        // function r ���崦����Ӧ������ݵ� function. ����r�Ƿ��������ص����ݣ���RcResult����
        c.AjaxSubmit('c.frmSearch', 'Id=' + id, { url: 'Delete', confirm: 'ConfirmDelete', confirmSingle: name }, function (r) {
            // ��������ɾ����᷵�����²�ѯ�Ľ������ ������Ӧʹ��Bind�������������ݸ��°󶨵�����
            c.grdUser.Bind(r.data);
        });
    }
    // OnNavBackRefresh�����м���� !!���ص���ǰҳ��ʱ������!!
    // ��ʱ��ǰҳ���ϰ󶨵��ǻ��������
    // ͨ����д�˷���������ִ��ˢ�����ݵ��߼�
    c.OnNavBackRefresh = function () {
        c.Search_click();
    }
}]);


