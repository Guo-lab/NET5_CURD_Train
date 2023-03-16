using ProjectBase.BusinessDelegate;

namespace eBPM.Exception
{
    public class ProcessResultNotDefinedException : BizException
    {
        public ProcessResultNotDefinedException(string action) : base(){
            this.ExceptionKey = "ProcessResultNotDefinedException:" + action;
        }
        
    }
}
