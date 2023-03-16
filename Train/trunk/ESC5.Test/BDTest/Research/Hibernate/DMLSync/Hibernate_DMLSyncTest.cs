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

namespace BDTest.Research.Hibernate.DMLSync
{
    [Collection("BD")]
    public class Hibernate_DMLSyncTest : BDTestBase
    {
        static Hibernate_DMLSyncTest()
        {
            // RegTestee<IBD, BD>();
        }

        // public BD testee;
        public Hibernate_DMLSyncTest(PerClassContext perClassContext, ITestOutputHelper output) : base(perClassContext, output) { }

        protected override void OnSetTestee(object value)
        {
            // testee = (value as BD)!;
        }
        protected override void BeforeClass()
        {
            LoadSqlData("TR_Task");
        }
        protected override void BeforeMethod()
        {
            dao = GetDao<Task, int>();
        }

        private IGenericDaoWithTypedId<Task, int> dao;

        [Fact]
        public void DMLSync_Update_NoSync()
        {
            //Given
            var task = dao.GetOneByQuery(o => true, false);
            Assert.NotEqual(-246, task.Score);

            //When            
            dao.Query().Update(o => new Task { Score = -246 });

            //Then
            Assert.NotEqual(-246, task.Score);//说明DML写入数据库的数据，不会自动与Session缓存同步
        }

        [Fact]
        public void DMLSync_UpdateAndGetEntity_NoSync()
        {
            //Given
            var task = dao.GetOneByQuery(o => true, false);

            //When            
            dao.Query().Update(o => new Task { Score = -246 });
            task = dao.GetOneByQuery(o => true, false);

            //Then
            Assert.NotEqual(-246, task.Score);//说明GetOneByQuery不重新查询而是使用了Session缓存
        }

        [Fact]
        public void DMLSync_UpdateAndSelect_OK()
        {
            //Given

            //When            
            dao.Query().Update(o => new Task { Score = -246 });
            var score = dao.Query().Select(o => o.Score).Distinct().Single();

            //Then
            Assert.Equal(-246, score);//说明DML操作结果可以通过查询数据库得到
        }
        [Fact]
        public void DMLSync_UpdateAndRefresh_Sync()
        {
            //Given
            var task = dao.GetOneByQuery(o => true, false);

            //When            
            dao.Query().Update(o => new Task { Score = -246 });
            dao.Refresh(task);

            //Then
            Assert.Equal(-246, task.Score);//说明可以手动进行同步
        }
    }
}
