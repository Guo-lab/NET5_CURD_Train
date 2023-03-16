using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportLibrary.Validate
{
    public interface IImportValidateController<T> where T : BaseImportItem
    {
        IEnumerable<T> ImportItems { get;set;}
        void StartValidate();
    }
}
