using ProjectBase.BusinessDelegate;

namespace eBPM.Exception
{
    public class ExpressionCompileException:BizException
    {
        public ExpressionCompileException(string errorMessage) : base(){
            this.ExceptionKey = "ExpressionCompileException:" + errorMessage;
        }
        
    }
}
