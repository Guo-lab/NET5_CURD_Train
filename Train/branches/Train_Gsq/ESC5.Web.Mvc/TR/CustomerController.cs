using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using ProjectBase.Utils;
using ProjectBase.Web.Mvc;
using ProjectBase.Web.Mvc.Validation;
using ProjectBase.Domain;
using System.Linq.Expressions;
using ESC5.Common.ViewModel.TR;
using ESC5.Domain.DomainModel.TR;
using ESC5.WebCommon;

using SharpArch.NHibernate;
using ProjectBase.Dto;
using ProjectBase.Domain.Transaction;




namespace ESC5.Web.Mvc.TR
{

    // !!!
    [Area("TR")]
    public class CustomerController : AppBaseController
    {
        public ICommonBD<Customer, int> CustomerBD { get; set; }
        // -------- Search User --------
        public ICommonBD<User, int> UserBD { get; set; }
        // -----------------------------
        
        // -----------
        // ------------------- Action ----------------
        // -----------
        public ActionResult List()
        {
            var vm = new CustomerListVM()
            {
                // FK User get DORef IList from UserBD 
                UserList = UserBD.GetRefList()
            };
            vm.ResultList = GetResultList(vm.Input);
            // ProjectBase BaseController
            return ForView(vm);
        }

        public ActionResult Search([Validate] CustomerListVM.SearchInput input)
        {
            // 只提供列表部分
            return ForList(GetResultList(input), input.ListInput.Pager);
        }
        private IList<CustomerListVM.ListRow> GetResultList(CustomerListVM.SearchInput input)
        {
            var filter = PredicateBuilder.True<Customer>();
            if (!string.IsNullOrEmpty(input.Name_))
            {
                filter = filter.And(o => o.Name_ == input.Name_); // 使用lambda表达式构造 HB 查询条件
            }
            if (input.Gender != null)
            {
                filter = filter.And(o => o.Gender == input.Gender);
            }
            if (input.User?.Id != null)
            {
                filter = filter.And(o => o.User!.Id == input.User.Id);
            }
            // GetDtoList方法用于从数据库查询指定的列数据
            var list = CustomerBD.GetDtoList<CustomerListVM.ListRow>(
                q => q.Distinct(),
                input.ListInput.Pager,
                filter,
                input.ListInput.OrderExpression
            );
            // 返回DTO类型元素集合, <list.MergeList();>
            return list;
        }





        [HttpPost]    // 限制请求类型（不标记表示不限制）,对数据库进行写操作的action必须限制为HttpPost
        [Transaction] // 标记事务（以action方法为事务单元），对数据库进行写操作的action必须标记为事务才会提交。
        public ActionResult Delete(int Id, [Validate] CustomerListVM.SearchInput input)
        {
            CustomerBD.Delete(Id);
            return Search(input);
        }


        // -----------------------------------------------
        // List Ref
        public ActionResult Add()
        {
            var m = new CustomerEditVM()
            {
                UserList = UserBD.GetRefList()
            };
            return ForView(m);
        }
        // UserList = UserBD.GetRefList() need to be passed
        public ActionResult Edit(int Id)
        {
            var m = new CustomerEditVM()
            {
                UserList = UserBD.GetRefList()
            };
            m.Input = CustomerBD.GetDto<CustomerEditVM.EditInput>(Id);
            return ForView(m);
        }
        // ------------------------------------------------



        [HttpPost]
        [Transaction(System.Data.IsolationLevel.ReadUncommitted)]
        public ActionResult Save([Validate] CustomerEditVM.EditInput input) // 对输入数据进行服务器端验证需加标记Validate.只有验证通过才会进入Action，否则自动返回错误信息
        {
            Customer customer;
            if (input.Id == 0) // 判断是否新增
            {
                customer = new Customer();
            }
            else
            {
                customer = CustomerBD.Get(input.Id); // 获取DO对象用于修改数据
            }
            customer.Email = input.Email;
            customer.Name_ = input.Name_;
            customer.Gender = input.Gender;
            customer.RegisterDate = input.RegisterDate;
            customer.Spending = input.Spending;
            customer.Vip = input.Vip;
            customer.Active = input.Active;

            customer.User = input.User?.ToReferencedDO(UserBD);


            CustomerBD.Save(customer); // 调用BD的方法执行业务逻辑（数据库约束错误会被自动处理）

            if (input.Id == 0) return Noop(); // 对新增成功的可返回Noop结果（相当于void调用）。
            return SaveOk(); // 对保存成功的可返回服务器消息--保存成功
        }


        // ----------- For other List Search Actions ----------
        // ----------- (1) Search by Date
        public ActionResult ListByDate()
        {
            var vm = new CustomerList_ListByDateVM();
            // 缺省查询条件, Default
            vm.Input.RegisterDate = DateTime.Today;
            vm.ResultList = GetResultListByDate(vm.Input);
            return ForView(vm);
        }
        // 如何跳转到这里的
        // @using (Html.KendoForm("c.frmSearch", "SearchByDate"))
        // and
        // c.Search_click()
        public ActionResult SearchByDate([Validate] CustomerList_ListByDateVM.SearchInput input)
        {
            // no need to allocate new vm
            // var vm = new CustomerList_ListByDateVM();
            IList<CustomerList_ListByDateVM.ListRow> ResultList = GetResultListByDate(input);
            // ForList: BaseController
            return ForList(ResultList, input.ListInput.Pager);
        }
        private IList<CustomerList_ListByDateVM.ListRow> GetResultListByDate(CustomerList_ListByDateVM.SearchInput input)
        {
            Expression<Func<Customer, bool>>? filter = null;
            if (input.RegisterDate != null)
            {
                // 查询条件表达式简单写法
                filter = o => o.RegisterDate == input.RegisterDate;
            }
            return CustomerBD.GetDtoList<CustomerList_ListByDateVM.ListRow>(
                input.ListInput.Pager,
                filter,
                input.ListInput.OrderExpression
            );
        }


        // ----------- (2) Search by whether be approved
        // Fixed search
        public ActionResult ListApproved()
        {
            var vm = new CustomerList_ListApprovedVM();
            vm.ResultList = GetResultListApproved(vm.Input);
            return ForView(vm);
        }
        private IList<CustomerList_ListApprovedVM.ListRow> GetResultListApproved(CustomerList_ListApprovedVM.SearchInput input)
        {
            // var filter = PredicateBuilder.True<Customer>();
            Expression<Func<Customer, bool>>? filter = null;
            filter = o => o.User != null;
            return CustomerBD.GetDtoList<CustomerList_ListApprovedVM.ListRow>(
                input.ListInput.Pager,
                filter,
                input.ListInput.OrderExpression
            );
        }

    }
}
