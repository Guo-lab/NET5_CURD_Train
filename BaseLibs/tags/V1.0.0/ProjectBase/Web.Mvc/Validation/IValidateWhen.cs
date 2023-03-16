using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBase.Web.Mvc.Validation
{
    public interface IValidateWhen
    {
        //bool ShouldVal(ValidationContext validationContext) {
        //    return true;
        //}
        string ShoulValGroups(ValidationContext validationContext);
    }
}
