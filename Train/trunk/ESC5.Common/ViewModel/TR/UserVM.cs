using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ProjectBase.Web.Mvc;
using ProjectBase.AutoMapper;
using ESC5.Domain.DomainModel.TR;
using System;
using ProjectBase.Utils;
using ProjectBase.Web.Mvc.Validation;
using ProjectBase.Dto;

namespace ESC5.Common.ViewModel.TR
{
    [Bind("Input")]
    public class UserListVM
    {
        public IList<ListRow> ResultList { get; set; } = new List<ListRow>();
        public SearchInput Input { get; set; } = new();

        [DisplayName("User")]
        public class SearchInput
        {
            public string? Name { get; set; }
            public ListInput ListInput { get; set; }= new ()
            {
                OrderExpression = SortStruc<User>.ToString(User.DefaultSort)
            };
        }
        public class ListRow
        {
            public int Id { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
            public DateTime? BirthDate { get; set; }
            public decimal Salary { get; set; }
            public User.RankEnum Rank { get; set; }
            public bool Active { get; set; }
        }
    }

    [Bind("Input")]
    public class UserEditVM
    {
        public EditInput Input { get; set; } = new EditInput();

        [DisplayName("User")]
        public class EditInput
        {
            public int Id { get; set; }
            [StringLength(10)]
            public string Code { get; set; }
            [StringLength(8)]
            public string Name { get; set; }
            [Min(5)]
            [VmInner(nameof(ValAge))]
            public int Age { get; set; }
            public DateTime? BirthDate { get; set; }
            public decimal Salary { get; set; }
            public User.RankEnum Rank { get; set; }
            public bool Active { get; set; }


            public bool ValAge()
            {
                return Age > 10;
            }
        }
    }
}


