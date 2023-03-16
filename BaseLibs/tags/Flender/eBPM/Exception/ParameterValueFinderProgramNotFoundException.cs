using ProjectBase.BusinessDelegate;

namespace eBPM.Exception
{
    public class ParameterValueFinderProgramNotFoundException : BizException
    {
        public ParameterValueFinderProgramNotFoundException(string fullName) : base(){
            this.ExceptionKey = "ParameterValueFinderProgramNotFoundException:" + fullName;
        }
        
    }
}
