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
    public class Task_ListController : AppBaseController
    {
        public ICommonBD<Task, int> TaskBD { get; set; }
        public ICommonBD<User, int> UserBD { get; set; }
        public ITransactionHelper TransactionHelper { get; set; }
        public IUtilQuery UtilQuery { get; set; }
        public ActionResult ListByDate()
        {
           var vm = new Task_ListListByDateVM();
            //此处演示：缺省查询条件
            vm.Input.CreateDate = DateTime.Today;
            vm.ResultList = GetResultListByDate(vm.Input);
            return ForView(vm);
        }

        public ActionResult SearchByDate([Validate] Task_ListListByDateVM.SearchInput input)
        {
            return ForList(GetResultListByDate(input), input.ListInput.Pager);
        }

        private IList<Task_ListListByDateVM.ListRow> GetResultListByDate(Task_ListListByDateVM.SearchInput input)
        {
            Expression<Func<Task,bool>>? filter =null;
            if (input.CreateDate != null)
            {
                //此处演示：查询条件表达式简单写法
                filter = o => o.CreateDate==input.CreateDate;
            }
            return TaskBD.GetDtoList<Task_ListListByDateVM.ListRow>(input.ListInput.Pager,
                                                        filter,
                                                       input.ListInput.OrderExpression
                                                       );

        }

        public ActionResult ApprovedList()
        {
            var vm = new Task_ListApprovedListVM();
            vm.ResultList = GetResultListApproved(vm.Input);
            return ForView(vm);
        }

        private IList<Task_ListApprovedListVM.ListRow> GetResultListApproved(Task_ListApprovedListVM.SearchInput input)
        {
            //此处演示： 固定查询条件
            Expression<Func<Task, bool>>? filter = o=>o.User!=null;
            return TaskBD.GetDtoList<Task_ListApprovedListVM.ListRow>(input.ListInput.Pager,
                                                        filter,
                                                       input.ListInput.OrderExpression
                                                       );

        }


        //----------------以下部分不是培训。仅用于研究
        #region "非示例代码。test insert 性能"
        [Transaction]
        public ActionResult TestMultiInsert()
        {
            var session = NHibernateSession.Current;
            var begin = DateTime.Now;
            var start = 1000;
            for (var k = 1; k <= 50; k++)
            {
                //TransactionHelper.DoInTrans(() =>
                //{
                for (var i = 0; i <= 9; i++)
                {
                    var task = new Task()
                    {
                        Code = (start + i + k * 10).ToString(),
                        Name = (start + i + k * 10).ToString(),
                        User = UserBD.Get(1),
                        Active = true,
                        CreateDate = DateTime.Now,
                        MaxItemCount = i + k * 10,
                        Status = Task.StatusEnum.OnGoing,
                        Score = DateTime.Now.Ticks

                    };
                    TaskBD.Save(task);
                    if (i == 0)
                    {
                        //  TransactionHelper.FlushAndClearSession();

                        // session.Flush();
                        // session.Clear();
                    }
                }
                //      }, true);
            }

            return RcJson(DateTime.Now - begin);
        }

        [Transaction]
        public ActionResult TestMultiInsertAssignId()
        {
            var session = NHibernateSession.Current;
            var begin = DateTime.Now;
            var start = 1000;
            for (var k = 1; k <= 50; k++)
            {
                for (var i = 0; i <= 9; i++)
                {
                    var task = new Task()
                    {
                        Code = (start + i + k * 10).ToString(),
                        Name = (start + i + k * 10).ToString(),
                        User = UserBD.Get(1),
                        Active = true,
                        CreateDate = DateTime.Now,
                        MaxItemCount = i + k * 10,
                        Status = Task.StatusEnum.OnGoing,
                        Score = DateTime.Now.Ticks

                    };
                    //     task.SetAssignedIdTo(start + i + k * 10);
                    TaskBD.Save(task);


                }
            }

            return RcJson(DateTime.Now - begin);
        }
        public ActionResult TestMultiInsertCollection()
        {
            var task = new Task()
            {
                Code = (111).ToString(),
                Name = (111).ToString(),
                User = UserBD.Get(1),
                Active = true,
                CreateDate = DateTime.Now,
                MaxItemCount = 111,
                Status = Task.StatusEnum.OnGoing,
                Score = DateTime.Now.Ticks

            };
            //   task.SetAssignedIdTo(111);
            TaskItem? t = null;

            var begin = DateTime.Now;
            var start = 1000;
            for (var k = 1; k <= 50; k++)
            {
                //    for (var i = 0; i <= 9; i++)
                //    {
                //        var item = new TaskItem()
                //        {
                //            Task=task,
                //            ItemNo = (start + i + k * 10).ToString()
                //        };
                //        task.TaskItems.Add(item);
                //        t = item;
                //    }
            }
            TaskBD.Save(task);
            return RcJson(t!.Id);
        }

        #endregion

        private void ResearchOfCollectionSubQuery()
        {
            var dao = TaskBD;
            dao.Query().Where(o => o.TaskItems.Count() > 0).ToList();
            //Count-->count 
            dao.Query().Where(o => o.TaskItems.Where(y => y.ItemNo == "").Count() > 0).ToList();
            dao.Query().Where(o => o.TaskItems.Count(y => y.ItemNo == "") > 0).ToList();
            //select task0_.Id as col_0_0_
            //from Train_Rainy.dbo.TR_Task task0_ where
            //          (select (count(*)) from Train_Rainy.dbo.TR_TaskItem taskitems1_
            //                              where task0_.Id=taskitems1_.TaskId and taskitems1_.ItemNo=@p0)>@p1

            //All--->not exists  +  not  否定之否定
            dao.Query().Where(o => o.TaskItems.All(y => y.ItemNo.Length < 10)).ToList();
            //select task0_.Id as col_0_0_
            //from Train_Rainy.dbo.TR_Task task0_ where
            //      not (exists (select taskitems1_.Id from Train_Rainy.dbo.TR_TaskItem taskitems1_
            //                              where task0_.Id=taskitems1_.TaskId and  not (len(taskitems1_.ItemNo)<@p0)))

            //Any--->exists
            dao.Query().Where(o => o.TaskItems.Any(y => y.ItemNo != null)).Select(o => o.Id).ToList();
            //select task0_.Id as col_0_0_
            //from Train_Rainy.dbo.TR_Task task0_
            //      where exists (select taskitems1_.Id from Train_Rainy.dbo.TR_TaskItem taskitems1_
            //                              where task0_.Id=taskitems1_.TaskId and (taskitems1_.ItemNo is not null))

            //Max--->聚合
            dao.Query().Where(o => o.TaskItems.Max(y => y.ItemNo.Length) == o.MaxItemCount).ToList();
            //--select task0_.Id as id1_0_, task0_.Code as code2_0_, task0_.Name as name3_0_, task0_.UserId as userid4_0_, task0_.MaxItemCount as maxitemcount5_0_, task0_.CreateDate as createdate6_0_, task0_.Score as score7_0_, task0_.Status as status8_0_, task0_.Active as active9_0_, task0_.Code as formula0_
            //--from Train_Rainy.dbo.TR_Task task0_ where
            //          (select max(taskitems1_.ItemNo) from Train_Rainy.dbo.TR_TaskItem taskitems1_
            //              where task0_.Id = taskitems1_.TaskId)= @p0

            //Single--->返回单值的子查询用于条件表达式
            dao.Query().Where(o => o.TaskItems.Where(y => y.ItemNo == "1").Select(y => y.ItemNo).Single() == "").Select(o => o.Id).ToList();
            //select task0_.Id as id1_0_,
            //from Train_Rainy.dbo.TR_Task task0_ where
            //          (select taskitems1_.ItemNo from Train_Rainy.dbo.TR_TaskItem taskitems1_
            //              where task0_.Id=taskitems1_.TaskId and taskitems1_.ItemNo=@p0)=@p1


            //不支持First
            // var tasks = dao.Query().Where(o => o.TaskItems.First().ItemNo=="1").ToList();报错

            //Take(1).Single--->select top 1  用于支持First效果
            dao.Query().Where(o => o.TaskItems.Where(y => y.ItemNo == "1").Select(y => y.ItemNo).Take(1).Single() == "").Select(o => o.Id).ToList();
            //select task0_.Id as col_0_0_
            //from Train_Rainy.dbo.TR_Task task0_ where
            //      (select TOP(@p0) taskitems1_.ItemNo from Train_Rainy.dbo.TR_TaskItem taskitems1_
            //                  where task0_.Id = taskitems1_.TaskId and taskitems1_.ItemNo = @p1)= @p2
        }
    }
}
