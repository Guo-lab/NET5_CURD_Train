using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using ProjectBase.Web.Mvc;
using ProjectBase.AutoMapper;
using ESC5.Domain.DomainModel.TR;
using ProjectBase.Utils;
using ProjectBase.Web.Mvc.Validation;
using ProjectBase.Domain;
using System.Linq.Expressions;
using ProjectBase.Dto;


namespace ESC5.Common.ViewModel.TR
{
    [Bind("Input")]
    public class CustomerList_ListByDateVM
    {
        // In Controller, return result
        public IList<ListRow> ResultList { get; set; } = new List<ListRow>();
        public class ListRow
        {
            public int Id { get; set; }
            public string Email { get; set; }
            public string Name_ { get; set; }
            public DORef<User, int>? User { get; set; }
            public int Gender { get; set; }
            public DateTime? RegisterDate { get; set; }
            public decimal Spending { get; set; }
            public Customer.RankEnum Vip { get; set; }
            public bool Active { get; set; }

            // GetDtoList方法用于从数据库查询指定的列数据。
            // 缺省情况下DTO类的属性就是要查询的列，按照属性名对应到字段名。
            // 如果无法按名对应，可使用SelectorMap显式指定。
            // 1.在DTO类里定义静态变量固定命名为SelectorMap。
            //   SelectorMap支持DTO继承。
            // 2.[SelectorIgnore] 标记用于指定属性不从数据库取得值，此标记支持继承，
            //   即如果基类定义的属性，子类里不想从数据库获取，则可以在子类里重写该属性并标记上SelectorIgnore。
            //   集合类型的属性不用标记Ignore，因为框架本来就会忽略它。
            public static IDictionary<string, Expression<Func<Customer, object?>>>
                SelectorMap = new Dictionary<string, Expression<Func<Customer, object?>>> {
                {
                        // five chars
                        // 其中应包含一个字符型列，但只从数据库取字段值的前5个字符。
                        nameof(Name_),o=>o.Name_.Substring(0,5)
                },
                //{nameof(Score),o=>Decimal.Parse("12.00")},
            };
        }

        // ------------------- Search -----------------
        // In cshtml to gain input and in Controller to update
        public SearchInput Input { get; set; } = new();

        [DisplayName("Customer")]
        public class SearchInput
        {
            public DateTime? RegisterDate { get; set; }
            public ListInput ListInput { get; set; } = new()
            {
                OrderExpression = SortStruc<Customer>.ToString(Customer.DefaultSort)
            };
        }

    }





    [Bind("Input")]
    public class CustomerList_ListApprovedVM
    {
        public IList<ListRow> ResultList { get; set; } = new List<ListRow>();
        public class ListRow
        {
            public int Id { get; set; }
            public string Email { get; set; }
            public string Name_ { get; set; }
            public DORef<User, int>? User { get; set; }
            public int Gender { get; set; }
            public DateTime? RegisterDate { get; set; }
            public decimal Spending { get; set; }
            public Customer.RankEnum Vip { get; set; }
            public bool Active { get; set; }
            public static IDictionary<string, Expression<Func<Customer, object?>>>
                SelectorMap = new Dictionary<string, Expression<Func<Customer, object?>>> {
                {
                        // five chars
                        nameof(Name_),o=>o.Name_.Substring(0,5)
                },
            };
        }

        // ------------------- Search -----------------
        // In cshtml to gain input and in Controller to update
        public SearchInput Input { get; set; } = new();

        [DisplayName("Customer")]
        public class SearchInput
        {
            // Fixed Search conditions
            public ListInput ListInput { get; set; } = new()
            {
                OrderExpression = SortStruc<Customer>.ToString(Customer.DefaultSort)
            };
        }
    }
}
