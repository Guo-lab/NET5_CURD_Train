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
using ProjectBase.Dto;
using ProjectBase.Domain; // DORef 
using System.Linq.Expressions;



namespace ESC5.Common.ViewModel.TR
{

    [Bind("Input")]
    public class CustomerListVM
    {
        public IList<ListRow> ResultList { get; set; } = new List<ListRow>();
        public SearchInput Input { get; set; } = new();

        // ------------- DORef -----------------
        // List -> GetRefList()
        public IList<DORef<User, int>> UserList { get; set; }


        [DisplayName("Customer")]
        public class SearchInput
        {
            #region------------ Filter Search ---------------------
            public string? Name_ { get; set; }
            public int? Gender { get; set; }
            // ----------- User Filter -----------
            public DORef<User, int>? User { get; set; }
            #endregion ---------------------------------------------


            // ListInput new Pager with or without PageSize
            public ListInput ListInput { get; set; } = new()
            {
                OrderExpression = SortStruc<Customer>.ToString(Customer.DefaultSort)
            };
        }

        public class ListRow // 为集合元素（即列表行对象）定义内部类。
        {                    // 该类结构对应页面上列表的列设置，数据对应列表的一行。
            public int Id { get; set; }
            public string Email { get; set; }
            public string Name_ { get; set; }

            // --------- DORef ---------
            public DORef<User, int>? User { get; set; }
            //public string? UserName { get; set; }


            public int Gender { get; set; }
            public DateTime? RegisterDate { get; set; }
            public decimal Spending { get; set; }

            //public int Vip { get; set; }
            public Customer.RankEnum Vip { get; set; }
            public bool Active { get; set; }


        }
    }





    [Bind("Input")]
    public class CustomerEditVM
    {
        public EditInput Input { get; set; } = new EditInput();

        // ----- DORef -----
        public IList<DORef<User, int>> UserList { get; set; } = new List<DORef<User, int>>();



        [DisplayName("Customer")]
        public class EditInput
        {
            public int Id { get; set; }

            [StringLength(50)]
            public string Email { get; set; }
            [StringLength(10)]
            public string Name_ { get; set; }


            // -------- DORef ---------
            public DORef<User, int>? User { get; set; }


            [VmInner(nameof(ValGender))] // public bool Val later
            public int Gender { get; set; }


            //[Max("2099-03-01")]
            //[VmInner(nameof(ValDate))]
            public DateTime? RegisterDate { get; set; }
            public decimal Spending { get; set; }

            // ------------ Enum Fail -> OK -------------
            //public int Vip { get; set; }
            public Customer.RankEnum Vip { get; set; }
            public bool Active { get; set; }




            public bool ValGender()
            {
                return Gender > -1;
            }
            //public bool ValDate()
            //{

            //}
        }
    }

}
