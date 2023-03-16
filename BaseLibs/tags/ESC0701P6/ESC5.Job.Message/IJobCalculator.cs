using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESC5.Job.Message
{
    //计算待处理事项，每个Entity有自己的实现类
    public interface IJobCalculator
    {
        void RefreshJob(int Id);
    }
    public interface IJobCalculatorFactory
    {
        IJobCalculator GetJobCalculator(string orderType);
    }

}
