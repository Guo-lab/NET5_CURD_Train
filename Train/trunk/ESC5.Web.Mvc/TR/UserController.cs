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

namespace ESC5.Web.Mvc.TR
{
    [Area("TR")]
    public class UserController : AppBaseController
    {
        public ICommonBD<User, int> UserBD { get; set; }
        public ActionResult List()
        {
            var vm = new UserListVM();
            vm.ResultList = GetResultList(vm.Input);
            return ForView(vm);
        }

        public ActionResult Search([Validate] UserListVM.SearchInput input)
        {
            return ForList(GetResultList(input), input.ListInput.Pager);
        }
        private IList<UserListVM.ListRow> GetResultList(UserListVM.SearchInput input)
        {
            var filter = PredicateBuilder.True<User>();
            if (!string.IsNullOrEmpty(input.Name))
            {
                filter=filter.And(o => o.Name == input.Name);
            }
            var list=UserBD.GetDtoList<UserListVM.ListRow>(input.ListInput.Pager,
                                                        filter,
                                                       input.ListInput.OrderExpression);
            return list;
        }

        [HttpPost]
        [Transaction]
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
            var m = new UserEditVM();
            m.Input = UserBD.GetDto<UserEditVM.EditInput>(Id);
            return ForView(m);
        }

        [Transaction]
        public ActionResult Save([Validate] UserEditVM.EditInput input)
        {
            User user;
            if (input.Id == 0)
            {
                user = new User();
            }
            else
            {
                user = UserBD.Get(input.Id);
            }
            user.Code = input.Code;
            user.Name = input.Name;
            user.Age = input.Age;
            user.BirthDate = input.BirthDate;
            user.Salary = input.Salary;
            user.Rank = input.Rank;
            user.Active = input.Active;

            UserBD.Save(user);

            if (input.Id == 0) return Noop();
            return SaveOk();
        }

    }
}
