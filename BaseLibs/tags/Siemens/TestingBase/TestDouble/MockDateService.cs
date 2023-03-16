using System;
using E2biz.Utility.General;

namespace TestingBase.TestDouble
{
    public class MockDateService : IDateService
    {
        private DateTime mockDate;
  
       
        public DateTime Now
        {
            get
            {
                return mockDate ;
            }
        }

        public DateTime Today
        {
            get
            {
                return mockDate.Date;
            }
        }
        public void SetToday(DateTime mockValue)
        {
            mockDate = mockValue;
        }
        public void SetNow(DateTime mockValue)
        {
            mockDate = mockValue;
        }
    }
}
