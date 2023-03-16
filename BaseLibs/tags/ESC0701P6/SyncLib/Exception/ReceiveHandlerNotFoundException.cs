using ProjectBase.BusinessDelegate;

namespace SyncLib.Exception
{
    public class ReceiveHandlerNotFoundException : BizException
    {

        public ReceiveHandlerNotFoundException(string? handlerType):base()
        {
            this.ExceptionKey = "ReceiveHandlerNotFound:" + handlerType;
        }
    }
}
