using ProjectBase.BusinessDelegate;

namespace eBPM.Exception
{
    public class ProcessTypeNotDefinedException : BizException
    {
        public ProcessTypeNotDefinedException(string processType) : base(){
            this.ExceptionKey = "ProcessTypeNotDefinedException:" + processType;
        }
        
    }
}
