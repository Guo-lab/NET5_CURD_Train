using System;
using ESC5.Domain.DomainModel.GN;
using Moq;
using ProjectBase.Domain;
using ProjectBase.Utils;
using TestingBase.TestBase;
using Xunit;
using Xunit.Abstractions;

namespace ESC5.Test.BDIntegrateTest.GN
{
    [Collection("BDIntegrate")]
    public class UserSomeFlowTest : BDIntegrateTestBase
    {
        public UserSomeFlowTest(PerClassContext perClassContext, ITestOutputHelper output) : base(perClassContext, output) { }

        public ICommonBD<User, int> testee;

        protected override void BeforeClass()
        {
            testee = Resolve<ICommonBD<User, int>>(); //从容器获得被测对象
            //LoadExcelData("Test_PO.xlsx");
        }

        [Fact]
        public void SomeFlow_Ok_Ok()
        {
            //Arrange/Given
            var user = new User();
            user.Code = "abc";

            //Act/When
            testee.Save(user);

            //Assert/Then
            Assert.Equal(1, user.Id);
        }

    }
}
