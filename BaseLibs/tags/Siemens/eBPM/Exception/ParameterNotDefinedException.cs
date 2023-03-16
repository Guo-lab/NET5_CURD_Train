using ProjectBase.BusinessDelegate;

namespace eBPM.Exception
{
    public class ParameterNotDefinedException:BizException
    {
        public ParameterNotDefinedException(string parameterName) : base(){
            this.ExceptionKey = "ParameterNotDefinedException:" + parameterName;
        }
        
    }
}
