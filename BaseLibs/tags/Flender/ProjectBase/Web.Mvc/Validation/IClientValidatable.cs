using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;

namespace ProjectBase.Web.Mvc.Validation
{
    public interface IClientValidatable
    {
        IEnumerable<BaseModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata);
    }
}
