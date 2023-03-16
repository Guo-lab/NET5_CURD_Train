using ProjectBase.BusinessDelegate;

namespace eBPM.Exception
{
    public class ProcessorFinderProgramNotFoundException : BizException
    {
        public ProcessorFinderProgramNotFoundException(string fullName) : base(){
            this.ExceptionKey = "ProcessorFinderProgramNotFoundException:" + fullName;
        }
        
    }
}
