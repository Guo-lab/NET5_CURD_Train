def.ns = "/TR/Task_List/";


def.ContentState('ListByDate').Controller(['pb', 'serverVm', '$scope', function (pb, serverVm, $scope) {
    var c = pb.Super(this, serverVm, $scope);

    c.Search_click = function () {
        //c.CallAction('TestMultiInsert', null, function (r) {
        //    console.log(r.data);
        //});TestMultiInsertStateless  TestMultiInsertAssignId
        c.CallAction('TestMultiInsert', null, function (r) {
            console.log(r.data);
        });
        
       // c.grdTask.AjaxRead();
    };
}]);


def.ContentState('ApprovedList').Controller(['pb', 'serverVm', '$scope', function (pb, serverVm, $scope) {
    var c = pb.Super(this, serverVm, $scope);
}]);


