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
using SharpArch.NHibernate;
using ProjectBase.Dto;
using ProjectBase.Domain.Transaction;

namespace ESC5.Web.Mvc.TR
{
    [Area("TR")]
    public class Task_InfoController : AppBaseController
    {
        public ICommonBD<Task, int> TaskBD { get; set; }
        public ICommonBD<User, int> UserBD { get; set; }
        public ITransactionHelper  transHelper { get; set; }
        public IUtilQuery UtilQuery { get; set; }

        public ActionResult List()
        {
            var vm = new TaskListVM()
            {
                UserList = UserBD.GetRefList()
            };
            vm.ResultList = GetResultList(vm.Input);
            return ForView(vm);
        }

        public ActionResult Search([Validate] TaskListVM.SearchInput input)
        {
            return ForList(GetResultList(input), input.ListInput.Pager);
        }

        private IList<TaskListVM.ListRow> GetResultList(TaskListVM.SearchInput input)
        {
            var filter = PredicateBuilder.True<Task>();
            if (!string.IsNullOrEmpty(input.Name))
            {
                filter=filter.And(o => o.Name == input.Name);
            }
            if (input.User?.Id != null)
            {
                filter = filter.And(o => o.User!.Id== input.User.Id);
            }
            var list = TaskBD.GetDtoList<TaskListVM.ListRow>(q => q.Distinct(),
                                                        input.ListInput.Pager,
                                                        filter,//,
                                                       ""//input.ListInput.OrderExpression
                                                       ); 
            list.MergeList(new List<string> { "aaa", "bbb" },(src,tar)=>src.StartsWith("a"));
            return list;
        }

        [HttpPost]
        [Transaction]
        public ActionResult Delete(int Id, [Validate] TaskListVM.SearchInput input)
        {
            TaskBD.Delete(Id);
            return Search(input);
        }

        public ActionResult Add()
        {
            var m = CreateTaskEditVM();
            return ForView(m);
        }

        public ActionResult Edit(int Id)
        {
            var m = CreateTaskEditVM();
            //m.Input = Map(TaskBD.Get(Id), m.Input); 取出DO后Map，性能较差
            //m.Input = TaskBD.GetDto<TaskEditVM.EditInput>(o => o.Id == Id);自己写where，性能提高，稍微麻烦，但可以返回null
            m.Input = TaskBD.GetDto<TaskEditVM.EditInput>(Id);//最简单，性能提高，与Get(Id)一样不会返回null而会抛出找不到记录的异常
            return ForView(m);
        }

        private TaskEditVM CreateTaskEditVM()
        {
            return new TaskEditVM()
            {
                UserList = UserBD.GetRefList()
            };
        }

        [HttpPost]
        [Transaction(System.Data.IsolationLevel.ReadUncommitted)]
        public ActionResult Save([Validate] TaskEditVM.EditInput input)
        {
            //Ensure(false,"hhhhh");
            Task task;
            if (input.Id == 0)
            {
                task = new Task();
            }
            else
            {
                task = TaskBD.Get(input.Id);
            }
            task.Code = input.Code;
            task.Name = input.Name;
            task.User = input.User?.ToReferencedDO(UserBD);
            task.MaxItemCount = input.MaxItemCount;
            task.CreateDate = input.CreateDate;
            task.Score = input.Score??0;
            task.Status = input.Status;
            task.Active = input.Active;

            TaskBD.Save(task);

            if (input.Id == 0) return Noop();
            return SaveOk();
        }


        public ActionResult MultiEdit()
        {
            var m = new TaskMultiEditVM();
            m.UserList = UserBD.GetRefList();
            m.Input.Rows.Add(new TaskEditVM.EditInput());
            m.Input.Rows.Add(new TaskEditVM.EditInput());
            return ForView(m);
        }

        public ActionResult MultiSave([Validate] TaskMultiEditVM.MultiEditInput input)
        {
            //一般情况下即使多行一起编辑，也要每行一保存，不是多行一起保存。
            //特殊情况下，才一起保存。比如子表记录少且不能独立于父表记录存在。

            return Noop();
        }

        public ActionResult SectionedInfo(int Id)
        {
            //GetDtoList支持DTO层级嵌套结构

            //var m = TaskBD.GetDto<TaskSectionedInfoVM>(o=>o.Code=="1010",o=>new TaskSectionedInfoVM {
            //    Section1=new TaskSectionedInfoVM.S1
            //    {
            //        Code=o.Code,
            //        Name=o.Name
            //    },
            //    Section2=new TaskSectionedInfoVM.S2
            //    {
            //        CreateDate=o.CreateDate,
            //        Score=o.Score
            //    }
            //});
            var m = TaskBD.GetDto<TaskSectionedInfoVM>(o => o.Code == "1010");
            return ForView(m);
        }
    }
}
