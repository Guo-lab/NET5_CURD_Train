/// ��ͷ����ָ����js�ļ��� def ����ķ����Ĳ����� state ��Ҫ�ӵ�ǰ׺��
/// ���淶���� Uri ���ò��֡�
/// ���涨�� State ʱ�Ͳ���ÿ�� Url ���ظ�дһ��һ���Ĳ����ˡ�
def.ns = "/TR/User/";


// ���������ڲ�������Ҫ���������г��ֵ�ҳ���Ӧ�� State ʹ�� ContentState ����������
// State/controller define, in Controller: ������ı�����������Ҫ�������̶������� pb, serverVm, $scope(Angular)
// pb�ǿ���ṩ�ķ���serverVmΪ����ṩ������--�ӷ��������ص����ݶ���(����ṩ�ӷ��������ͻ��˵����ݰ�)
def.ContentState('List', 't').Controller(['pb', 'serverVm', '$scope', function (pb, serverVm, $scope) {

    // �̳п���ṩ�����ƻ���controller�Ĺ���
    // cָ������ʱcontroller��������c.vm = serverVm��ͬʱ֧�����мʽ����·�����ܡ�
    // [ʹ��c��������֯����������] ������������������Ҫ����c.ǰ׺��
    var c = pb.Super(this, serverVm, $scope);

    // ��c�¶����¼�������
    c.Search_click = function () {
        // kendo grid �ڿ��ָ��֧���£�ʹ�ñ�����(c.grdUser)��AjaxRead�������ύ������ڱ���������ѯ��������ҳ����������ݣ�
        // ���ӷ�������ȡ��ѯ������ݡ�
        c.grdUser.AjaxRead();
    };
    c.Delete_click = function (id, name) {

        // ���ύ����������Ӧ���
        // function r ���崦����Ӧ������ݵ� function. ����r�Ƿ��������ص����ݣ���RcResult����
        c.AjaxSubmit('c.frmSearch', 'Id=' + id, { url: 'Delete', confirm: 'ConfirmDelete', confirmSingle: name }, function (r) {

            // ��������ɾ����᷵�����²�ѯ�Ľ������
            // ������Ӧʹ��Bind�������������ݸ��°󶨵�����
            c.grdUser.Bind(r.data);
        });
    }

    // On��ͷ�ķ����ǿ��֧�ֵĹ���(��������)����
    // OnNavBackRefresh�����м�������ص���ǰҳ��ʱ�����á�
    // ��ʱ��ǰҳ���ϰ󶨵��ǻ��������
    // ͨ����д�˷���������ִ��ˢ�����ݵ��߼�
    c.OnNavBackRefresh = function () {
        c.Search_click();
    }
}]);


