using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBPM.DomainModel
{
    public enum StepCategoryEnum
    {
        Start = 1,
        Process = 2,
        Branch = 3,
        Finish = 4
    }
    public enum ValueTypeEnum
    {
        String = 1,
        Numeric = 2,
        YesNo = 3,
        DateTime = 4
    }

    
}
