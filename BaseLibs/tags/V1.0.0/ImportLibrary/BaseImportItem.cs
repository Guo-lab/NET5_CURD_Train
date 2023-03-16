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
        public string ErrorMessage { get; set; }
        public ImportLineResult? Result { get; set; } 
    }
}
