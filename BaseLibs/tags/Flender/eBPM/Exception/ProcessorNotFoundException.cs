using ProjectBase.BusinessDelegate;

namespace eBPM.Exception
{
    public class ProcessorNotFoundException : BizException
    {
        public ProcessorNotFoundException(string stepName) : base(){
            this.ExceptionKey = "ProcessorNotFoundException:" + stepName;
        }
        
    }
}
