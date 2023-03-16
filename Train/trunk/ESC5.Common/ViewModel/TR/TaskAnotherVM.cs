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
using ProjectBase.Domain;
using System.Linq.Expressions;
using ProjectBase.Dto;

namespace ESC5.Common.ViewModel.TR
{
    [Bind("Input")]
    public class Task_ListListByDateVM
    {
        public IList<ListRow> ResultList { get; set; } = new List<ListRow>();
        public SearchInput Input { get; set; } = new();

        [DisplayName("Task")]
        public class SearchInput
        {
            public DateTime? CreateDate { get; set; }
            public ListInput ListInput { get; set; }= new ()
            {
                OrderExpression = SortStruc<Task>.ToString(Task.DefaultSort)
            };
        }
        public class ListRow
        {
            public int Id { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public DORef<User, int>? User { get; set; }
            public string? UserName { get; set; }
            public int MaxItemCount { get; set; }
            public DateTime? CreateDate { get; set; }
            public decimal Score { get; set; }
            public Task.StatusEnum Status { get; set; }
            public bool Active { get; set; }

            public static IDictionary<string, Expression<Func<Task, object?>>> SelectorMap = new Dictionary<string, Expression<Func<Task, object?>>> {
                {nameof(Name),o=>o.Name.Substring(0,5)},
        //        {nameof(Score),o=>Decimal.Parse("12.00")},
            };
        }
    }

    public class Task_ListApprovedListVM
    {
        public IList<ListRow> ResultList { get; set; } = new List<ListRow>();
        public SearchInput Input { get; set; } = new();
        public class SearchInput
        {
            public ListInput ListInput { get; set; } = new()
            {
                OrderExpression = SortStruc<Task>.ToString(Task.DefaultSort)
            };
        }
        public class ListRow
        {
            public int Id { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public DORef<User, int>? User { get; set; }
            public string? UserName { get; set; }
            public int MaxItemCount { get; set; }
            public DateTime? CreateDate { get; set; }
            public decimal Score { get; set; }
            public Task.StatusEnum Status { get; set; }
            public bool Active { get; set; }

            public static IDictionary<string, Expression<Func<Task, object?>>> SelectorMap = new Dictionary<string, Expression<Func<Task, object?>>> {
                {nameof(Name),o=>o.Name.Substring(0,5)}
            };
        }
    }
    public class TaskSectionedInfoVM
    {
        [SelectorCascade]
        public S1 Section1 { get; set; }

        [SelectorCascade]
        public S2 Section2 { get; set; }
        public decimal Score { get; set; }

        public class S1
        {
            public string Code { get; set; }
            public string Name { get; set; }

            [SelectorCascade]
            public S2 Section12 { get; set; }

            public static IDictionary<string, Expression<Func<Task, object?>>> SelectorMap = new Dictionary<string, Expression<Func<Task, object?>>> {
                {nameof(Code),o=>o.Name}
            };
        }
        public class S2
        {
            public DateTime? CreateDate { get; set; }
            public decimal Score { get; set; }
        }

    }
}


