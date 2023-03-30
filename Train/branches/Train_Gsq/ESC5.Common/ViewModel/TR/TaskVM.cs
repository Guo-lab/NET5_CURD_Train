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

using System.Diagnostics;


namespace ESC5.Common.ViewModel.TR
{
    [Bind("Input")]
    public class TaskListVM
    {
        public IList<ListRow> ResultList { get; set; } = new List<ListRow>();
        public SearchInput Input { get; set; } = new();

        /// <summary>
        /// DORef类型使用：
        //// 1.Controller中取下拉列表数据源XxxBD.GetRefList()
        //// 通常下拉列表数据用于选择一个对象引用，BD.GetRefList() 为此种场景返回集合，元素为类型是DORef的对象，其包含Id和RefText两个属性，后者用于文字显示。
        //// 2.从DORef向DO赋值
        //// 3.从DO向DORef赋值
        //// 4.DORef类型的查询条件处理（判断输入，与构建lamda）
        /// </summary>
        public IList<DORef<User,int>> UserList { get; set; }



        [DisplayName("Task")]
        public class SearchInput
        {
            public string? Name { get; set; }
            public DORef<User, int>? User { get; set; }
            public ListInput ListInput { get; set; }= new ()
            {
                OrderExpression = SortStruc<Task>.ToString(Task.DefaultSort)
            };
        }
        public class ListRow: ISelfMerger<string>
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

            // 从Domain到VM的属性赋值，可以使用ISelfMapper(AutoMapper)，但反向赋值不可以。
            // GetDtoList方法可以直接从数据库选取对应的数据向VM赋值。
            // 
            // 对于上面两种方法得到的VM，可能有部分属性为自动赋值，此时可以使用ISelfMerger来完成进一步赋值
            [SelectorIgnore]
            public string Haha { get; set; }
            [SelectorIgnore]
            public string Hello { get; set; }
            [SelectorIgnore]
            public IList<string> HelloList { get; set; }

            public void Merge()
            {
                Haha = "hahahaha" + Name;
                Debug.WriteLine(Haha, "Haha in Merge");
            }
            public void Merge(BaseSelfMergerContext context)
            {
                Haha = "hahahaha" + context.ToString();
                Debug.WriteLine(Haha, "Haha in Merge Context");
            }

            public void MergeVar<TVariantSource>(TVariantSource src)
            {
                Hello = src!.ToString()+ "Merge<TSource>";
                if(src is DORef<User,int> user)
                {
                    Hello = user.RefText;
                }
                Debug.WriteLine(Hello, "Hello in MergeVar");
            }
            public void Merge(string src)
            {
                Hello = src+ "Merge";
                Debug.WriteLine(Hello, "Hello in Merge");
            }
            public void MergeList(IEnumerable<string> src)
            {
                HelloList = src.ToList();
                Debug.WriteLine(HelloList, "HelloList"); // HelloList: System.Collections.Generic.List`1[System.String]
            }



            public static IDictionary<string, Expression<Func<Task, object?>>> SelectorMap = new Dictionary<string, Expression<Func<Task, object?>>> {
                {nameof(Name),o=>o.Name.Substring(0,5)},
              //  {nameof(Score),o=>12},
              //  {nameof(CreateDate),o=>null},
            };
        }
    }

    [Bind("Input")]
    public class TaskEditVM
    {
        public EditInput Input { get; set; } = new EditInput();
        public IList<DORef<User, int>> UserList { get; set; } = new List<DORef<User, int>>();

        [DisplayName("Task")]
        public class EditInput : ValidatableObject,IValidateWhen
        {
            public int Id { get; set; }

            [StringLength(10)]
            public string Code { get; set; }
            
            [StringLength(8,Groups ="A")]
            public string Name { get; set; }
            
            public DORef<User,int>? User { get; set; }
            
            [ValidateWhen]
            public int MaxItemCount { get; set; }
            
            [Max("2099-03-01")]
            public DateTime? CreateDate { get; set; }
            
            [DecimalFormat(5,3)]
            [Range(-100,100)]
            [Required(Groups ="_A")]
            [VmInner(nameof(ValScore))]
            public decimal? Score { get; set; }
            
            public Task.StatusEnum Status { get; set; }
            public bool Active { get; set; }

            public bool ValScore()
            {
                return Score > 0;
            }
            public string ShouldValidateGroups()
            {
                if (MaxItemCount > 10)
                {
                    return "_D,A";
                }
                return "A";
            }

            public override string? Validate()
            {
                //if (MaxItemCount < 100 && Score > 10 || Active && Score > 1) return null;
                //return "FormDataInValid";
                return null;
            }

        }
    }
}


