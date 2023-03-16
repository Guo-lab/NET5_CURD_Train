using ProjectBase.BusinessDelegate;

namespace eBPM.Exception
{
    public class InvalidStatusException : BizException
    {
        public InvalidStatusException(string status) : base(){
            this.ExceptionKey = "InvalidStatusException:" + status;
        }
        
    }
}
