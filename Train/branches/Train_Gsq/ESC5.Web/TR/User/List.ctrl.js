/// 开头声明指出此js文件中 def 对象的方法的参数中 state 名要加的前缀。
/// 按规范就是 Uri 共用部分。
/// 下面定义 State 时就不用每个 Url 都重复写一遍一样的部分了。
def.ns = "/TR/User/";


// 在整个窗口布局中主要内容区域中呈现的页面对应的 State 使用 ContentState 方法来定义
// State/controller define, in Controller: 依赖项的变量名，至少要有三个固定依赖项 pb, serverVm, $scope(Angular)
// pb是框架提供的服务，serverVm为框架提供的数据--从服务器返回的数据对象(框架提供从服务器到客户端的数据绑定)
def.ContentState('List', 't').Controller(['pb', 'serverVm', '$scope', function (pb, serverVm, $scope) {

    // 继承框架提供的类似基类controller的功能
    // c指向运行时controller对象自身，c.vm = serverVm，同时支持面包屑式导航路径功能。
    // [使用c对象来组织方法与数据] 即方法名和数据名都要带有c.前缀。
    var c = pb.Super(this, serverVm, $scope);

    // 在c下定义事件处理方法
    c.Search_click = function () {
        // kendo grid 在框架指令支持下，使用表格对象(c.grdUser)的AjaxRead方法来提交表格所在表单（包含查询条件、分页、排序等数据）
        // 并从服务器获取查询结果数据。
        c.grdUser.AjaxRead();
    };
    c.Delete_click = function (id, name) {

        // 来提交表单并处理响应结果
        // function r 定义处理相应结果数据的 function. 参数r是服务器返回的数据，是RcResult类型
        c.AjaxSubmit('c.frmSearch', 'Id=' + id, { url: 'Delete', confirm: 'ConfirmDelete', confirmSingle: name }, function (r) {

            // 服务器在删除后会返回重新查询的结果数据
            // 表格对象应使用Bind方法来将该数据更新绑定到自身
            c.grdUser.Bind(r.data);
        });
    }

    // On开头的方法是框架支持的钩子(生命周期)方法
    // OnNavBackRefresh在面包屑导航返回到当前页面时被调用。
    // 此时当前页面上绑定的是缓存的数据
    // 通过编写此方法，可以执行刷新数据的逻辑
    c.OnNavBackRefresh = function () {
        c.Search_click();
    }
}]);


