using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportLibrary
{
    public enum ImportLineResult
    {
        Succeed = 0,
        Warning,
        Failed
    }
    public class BaseImportItem
    {
        public string Key { get; set; }
        public string ErrorMessage { get; set; }
        public ImportLineResult? Result { get; set; } 

        public BaseImportItem()
        {
            this.Key = Guid.NewGuid().ToString();
            this.Result = ImportLineResult.Succeed;
        }
    }
}
