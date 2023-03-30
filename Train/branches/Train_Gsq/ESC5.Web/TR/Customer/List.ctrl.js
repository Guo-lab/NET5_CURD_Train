// dir Customer
def.ns = "/TR/Customer/";


// ���������ڲ�������Ҫ���������г��ֵ�ҳ���Ӧ�� State ʹ�� ContentState ����������
// State/controller define, in Controller: ������ı�����������Ҫ�������̶������� pb, serverVm, $scope(Angular)
// pb�ǿ���ṩ�ķ���serverVmΪ����ṩ������--�ӷ��������ص����ݶ���(����ṩ�ӷ��������ͻ��˵����ݰ�)
//
// pbui Confirm
def.ContentState('List').Controller(['pb', 'serverVm', '$scope', 'pbui', function (pb, serverVm, $scope, pbui) {

    var c = pb.Super(this, serverVm, $scope);

    // ��c�¶����¼�������
    c.Search_click = function () {
        // kendo grid �ڿ��ָ��֧���£�ʹ�ñ�����(c.grdUser)��AjaxRead�������ύ������ڱ���������ѯ��������ҳ����������ݣ�
        // ���ӷ�������ȡ��ѯ������ݡ�
        c.grdCustomer.AjaxRead();
    };

    c.Delete_click_AjaxSubmit = function (id, name) {
        // function r ���崦����Ӧ������ݵ� function. ����r�Ƿ��������ص����ݣ���RcResult����
        c.AjaxSubmit(
            'c.frmSearch',
            'Id=' + id,
            {
                url: 'Delete',
                confirm: 'ConfirmDelete',
                confirmSingle: name
            },
            function (r) {
                // ��������ɾ����᷵�����²�ѯ�Ľ������ ������Ӧʹ��Bind�������������ݸ��°󶨵�����
                c.grdCustomer.Bind(r.data);
        });
    }

    // pbui Delete Confirm
    c.Delete_click = function (id, name) {
        pbui.Confirm('ConfirmDelete', name).then(confirmed => {
            if (confirmed) {
                c.AjaxSubmit('c.frmSearch', 'Id=' + id, {url: 'Delete'}, function (r) {
                    c.grdCustomer.Bind(r.data);
                });
            }
        });
    };


    // OnNavBackRefresh�����м���� !!���ص���ǰҳ��ʱ������!!
    // ��ʱ��ǰҳ���ϰ󶨵��ǻ��������
    // ͨ����д�˷���������ִ��ˢ�����ݵ��߼�
    c.OnNavBackRefresh = function () {
        c.Search_click();
    };
}]);





// -------------------------------------------------------------
// ---------------------- Other List Actions -------------------
// -------------------------------------------------------------
// ListByDate Defined in Controller 
// A third VM
def.ContentState('ListByDate').Controller(['pb', 'serverVm', '$scope', function (pb, serverVm, $scope) {
    var c = pb.Super(this, serverVm, $scope);

    c.Search_click = function () {
        //c.CallAction('TestMultiInsert', null, function (r) {
        //    console.log(r.data);
        //});TestMultiInsertStateless  TestMultiInsertAssignId
        //c.CallAction('TestMultiInsert', null, function (r) {
        //    console.log(r.data);
        //});

        c.grdCustomer3.AjaxRead();
    };

    c.OnNavBackRefresh = function () {
        c.Search_click();
    };
}]);

// ListApproved
def.ContentState('ListApproved').Controller(['pb', 'serverVm', '$scope', function (pb, serverVm, $scope) {
    var c = pb.Super(this, serverVm, $scope);

    c.Search_click = function () {
        c.grdCustomer2.AjaxRead();
    };

    c.OnNavBackRefresh = function () {
        c.Search_click();
    };
}]);



def.ContentState('SectionedInfo').Controller(['pb', 'serverVm', '$scope', function (pb, serverVm, $scope) {
    var c = pb.Super(this, serverVm, $scope);
}]);
