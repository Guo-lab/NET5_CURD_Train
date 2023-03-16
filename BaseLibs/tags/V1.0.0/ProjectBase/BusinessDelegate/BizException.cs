using ProjectBase.Utils;
using System;

namespace ProjectBase.BusinessDelegate
{
    public class BizException : Exception, IErrorForUser
    {
        public BizException() : base() { }
        public BizException(string msg) : base(msg)
        {
            this.ExceptionKey = msg;
        }

        private string GetExceptionMessage(string exceptionKey, params string[] parameters)
        {
            string message = exceptionKey;
            if (parameters.Length > 0)
            {
                message = string.Format(exceptionKey, parameters);
            }
            return message;
        }
        public BizException(string exceptionKey, params string[] parameters) : base(exceptionKey)
        {
            this.ExceptionKey = GetExceptionMessage(exceptionKey, parameters);
        }

        private string _exceptionKey;
        public string ExceptionKey
        {
            get
            {
                if (string.IsNullOrEmpty(_exceptionKey)) {
                    _exceptionKey = this.GetType().Name;
                }
                return _exceptionKey;
            }
            set { _exceptionKey = value; }
        }
    }
}
