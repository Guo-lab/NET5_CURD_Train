using ProjectBase.BusinessDelegate;

namespace eBPM.Exception
{
    public class NoBranchSatisfiedException : BizException
    {
        public NoBranchSatisfiedException(string stepName) : base(){
            this.ExceptionKey = "NoBranchSatisfiedException:" + stepName;
        }
        
    }
}
