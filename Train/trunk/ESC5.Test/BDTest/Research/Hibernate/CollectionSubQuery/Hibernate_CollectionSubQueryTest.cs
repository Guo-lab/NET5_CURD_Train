using ESC5.Domain.DomainModel.TR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestingBase.TestBase;
using Xunit;
using Xunit.Abstractions;
using NHibernate.Linq;
using ProjectBase.Domain;

namespace BDTest.Research.Hibernate.EntityCollectionQuery
{
    [Collection("BD")]
    public class Hibernate_CollectionSubQueryTest : BDTestBase
    {
        static Hibernate_CollectionSubQueryTest()
        {
            // RegTestee<IBD, BD>();
        }

        // public BD testee;
        public Hibernate_CollectionSubQueryTest(PerClassContext perClassContext, ITestOutputHelper output) : base(perClassContext, output) { }

        protected override void OnSetTestee(object value)
        {
            // testee = (value as BD)!;
        }
        protected override void BeforeClass()
        {
            LoadSqlData("TR_Task", "TR_TaskItem");
        }

        private IGenericDaoWithTypedId<Task, int> dao;

        [Fact]
        public void CollectionSubQuery_OK_OK()
        {
            //此例说明使用NHibernate Linq构建查询和利用集合属性构建子查询


            //Given
            var dao = GetDao<Task, int>();
            var taskItemDao = GetDao<TaskItem, int>();
            var t=taskItemDao.GetCountByQuery();

            //When            
            var tasks=dao.Query().Where(o => o.TaskItems.Count()>0).ToList();

            //Count-->count 
            dao.Query().Where(o => o.TaskItems.Where(y => y.ItemNo== "").Count()>0).ToList();
            dao.Query().Where(o => o.TaskItems.Count(y => y.ItemNo == "") > 0).ToList();
            //select task0_.Id as col_0_0_
            //from Train_Rainy.dbo.TR_Task task0_ where
            //          (select (count(*)) from Train_Rainy.dbo.TR_TaskItem taskitems1_
            //                              where task0_.Id=taskitems1_.TaskId and taskitems1_.ItemNo=@p0)>@p1

            //All--->not exists  +  not  否定之否定
            dao.Query().Where(o => o.TaskItems.All(y => y.ItemNo.Length<10)).ToList();
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
