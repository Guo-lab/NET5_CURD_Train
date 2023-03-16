using ProjectBase.BusinessDelegate;

namespace SyncLib.Exception
{
    public class SyncHandlerNotFoundException : BizException
    {

        public SyncHandlerNotFoundException(string? handlerType) :base()
        {
            this.ExceptionKey = "SyncHandlerNotFound:" + handlerType;
        }
    }
}
