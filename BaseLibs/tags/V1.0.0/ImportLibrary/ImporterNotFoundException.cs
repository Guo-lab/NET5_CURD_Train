using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectBase.BusinessDelegate;

namespace ImportLibrary
{
    public class ImporterNotFoundException:BizException
    {
        public ImporterNotFoundException(string type) : base()
        {
            this.ExceptionKey = "ImporterNotFound:" + type;
        }
    }
}
