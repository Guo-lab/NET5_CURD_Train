using ProjectBase.Web.Mvc.ValueInFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBase.Application
{
    public interface IModelBindingUtil
    {
        Task<Tuple<object, string[]>> BindModelWithValueInFile(object context, string fileName, IValueInFileParser fileParser, object modelInitValue, string modelName);
    }

}
