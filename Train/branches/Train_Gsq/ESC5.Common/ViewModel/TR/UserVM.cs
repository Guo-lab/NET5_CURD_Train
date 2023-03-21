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
    public class UserListVM // 命名由Controller名 + Action名组成
    {                       // 每个页面对应建一个VM，页面中的部分数据不能对应VM


        // List页面约定包含一个固定命名为ResultList的集合型属性作为列表数据源。
        // 必须初始化集合属性。
        //
        // 每个VM类都约定包含一个固定命名为Input的属性，用于对应页面输入数据。
        // 必须使用无参构建器初始化改属性。
        //
        // Both Reference in Controller.cs
        public IList<ListRow> ResultList { get; set; } = new List<ListRow>();
        public SearchInput Input { get; set; } = new();


        [DisplayName("User")]
        public class SearchInput
        {
            public string? Name { get; set; }

            public int? Age { get; set; }

            public ListInput ListInput { get; set; }= new ()
            {
                OrderExpression = SortStruc<User>.ToString(User.DefaultSort)
            };
        }




        //public SearchInput_2 Input_2 { get; set; } = new();
        //[DisplayName("How Old")]
        //public class SearchInput_2
        //{
        //    public int? Age { get; set; }
        //    public ListInput ListInput { get; set; } = new()
        //    {
        //        OrderExpression = SortStruc<User>.ToString(User.DefaultSort)
        //    };
        //}


        public class ListRow // 为集合元素（即列表行对象）定义内部类。
        {                    // 该类结构对应页面上列表的列设置，数据对应列表的一行。
            public int Id { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
            public DateTime? BirthDate { get; set; }
            public decimal Salary { get; set; }
            public User.RankEnum Rank { get; set; }
            public bool Active { get; set; }

            public string Mood { get; set; }
        }
    }



    /// <summary>
    /// 仅将客户端输入项赋值给服务器端VM。
    /// Bind标记加在类上，表示绑定数据时只绑定指定的属性，这里就是只绑定Input属性，其它属性不绑定。
    /// 如果没有Bind标记，则所有提交到服务器的请求数据都会绑定，这将造成安全隐患，因此必须标记Bind。
    /// </summary>
    [Bind("Input")]
    public class UserEditVM
    {
        public EditInput Input { get; set; } = new EditInput();

        [DisplayName("User")] // 在输入数据类上标记对应的DO类名。此名称与输入类属性名一起组成页面多语言文件中的Key
        public class EditInput
        {
            public int Id { get; set; }
            [StringLength(10)]
            public string Code { get; set; }
            [StringLength(8)]
            public string Name { get; set; }

            [Min(5)]
            [VmInner(nameof(ValAge))] // public bool ValAge later
            public int Age { get; set; }

            public DateTime? BirthDate { get; set; } // 带问号表示非必要项
            public decimal Salary { get; set; }
            public User.RankEnum Rank { get; set; }
            public bool Active { get; set; }


            // ---------- new field ------------
            [StringLength(5)]
            public string Mood { get; set; }

            public bool ValAge()
            {
                return Age > 10;
            }
        }
    }
}


