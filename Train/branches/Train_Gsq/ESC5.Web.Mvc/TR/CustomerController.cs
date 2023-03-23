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
        // List Reference
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
    }
}
