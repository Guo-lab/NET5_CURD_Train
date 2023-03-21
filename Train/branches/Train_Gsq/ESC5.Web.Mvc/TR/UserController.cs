using Microsoft.AspNetCore.Mvc;
using ProjectBase.Utils;
using ProjectBase.Web.Mvc;
using ProjectBase.Web.Mvc.Validation;
using ProjectBase.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ESC5.Common.ViewModel.TR;
using ESC5.Domain.DomainModel.TR;
using ESC5.WebCommon;

namespace ESC5.Web.Mvc.TR // TR module
{
    [Area("TR")] // 标记所属模块,用于Url对应到Controller类
    public class UserController : AppBaseController // 以Controller为类名后缀，并继承AppBaseController或其子类
    {


        public ICommonBD<User, int> UserBD { get; set; } // Base Domain 声明所需要的BD（会被自动注入），ICommandBD泛型接口不需要定义可直接使用



        // Action方法指的是public的 返回类型固定为IActionResult的方法
        // 每个Action对应一个http请求。
        // 定义清单页面对应的Action.
        // 此Action为页面提供初始绑定数据源--VM数据，即进入页面时客户端就会请求此Action得到VM数据。
        public ActionResult List()
        {
            var vm = new UserListVM();
            vm.ResultList = GetResultList(vm.Input);
            // "使用ForView方法返回VM类型的数据。此VM类型必须与页面上@model声明的类型一致。
            // 注意：只有存在与Action同名的页面和同名的VM类时，才可以也必须使用ForView。
            // 即ForView方法的意义就是为与所在Action(List)的同名页面(List.cshtml)提供同名VM数据(UserListVM)."
            return ForView(vm);
        }


        /// 为清单页面的查询功能定义查询Action.需要前端提交查询条件作为输入参数。
        public ActionResult Search([Validate] UserListVM.SearchInput input)
        {
            // 只提供列表部分
            return ForList(GetResultList(input), input.ListInput.Pager);
        }


        /// <summary>
        /// 由于页面初始时(List action)和查询时(Search action)通常都需要执行查询逻辑，因此定义一个私有方法
        /// </summary>
        /// <param name="input"> 查询条件 </param>
        /// <returns> 查询结果--行集合 </returns>
        private IList<UserListVM.ListRow> GetResultList(UserListVM.SearchInput input)
        {
            var filter = PredicateBuilder.True<User>();
            if (!string.IsNullOrEmpty(input.Name))
            {
                filter=filter.And(o => o.Name == input.Name); // 使用lambda表达式构造 HB 查询条件
            }
            //-------- Add filter condition --------

            if (input.Age != null)
            {
                filter = filter.And(o => o.Age == input.Age);
            }

            //var list = UserBD.GetDtoList<UserListVM.ListRow>(
            //    input.ListInput.Pager,
            //    filter,
            //    input.ListInput.OrderExpression
            //);
            var list =UserBD.GetDtoList<UserListVM.ListRow>(
                input?.ListInput.Pager,
                filter,
                input?.ListInput.OrderExpression
            );
            //---------------------------------------


            // 返回DTO类型元素集合
            return list;
        }



        [HttpPost]    // 限制请求类型（不标记表示不限制）,对数据库进行写操作的action必须限制为HttpPost
        [Transaction] // 标记事务（以action方法为事务单元），对数据库进行写操作的action必须标记为事务才会提交。
        public ActionResult Delete(int Id, [Validate] UserListVM.SearchInput input)
        {
            UserBD.Delete(Id);
            return Search(input);
        }



        public ActionResult Add()
        {
            var m = new UserEditVM();
            return ForView(m);
        }

        public ActionResult Edit(int Id)
        {
            var m = new UserEditVM
            {
                // 直接从数据库取DTO数据
                Input = UserBD.GetDto<UserEditVM.EditInput>(Id)
            };
            return ForView(m);
        }



        [HttpPost]
        [Transaction]
        public ActionResult Save([Validate] UserEditVM.EditInput input) // 对输入数据进行服务器端验证需加标记Validate.只有验证通过才会进入Action，否则自动返回错误信息
        {
            User user;
            if (input.Id == 0) // 判断是否新增
            {
                user = new User();
            }
            else
            {
                user = UserBD.Get(input.Id); // 获取DO对象用于修改数据
            }
            user.Code = input.Code;
            user.Name = input.Name;
            user.Age = input.Age;
            user.BirthDate = input.BirthDate;
            user.Salary = input.Salary;
            user.Rank = input.Rank;
            user.Active = input.Active;

            user.Mood = input.Mood;


            UserBD.Save(user); // 调用BD的方法执行业务逻辑（数据库约束错误会被自动处理）

            if (input.Id == 0) return Noop(); // 对新增成功的可返回Noop结果（相当于void调用）。

            return SaveOk(); // 对保存成功的可返回服务器消息--保存成功
        }

    }
}
