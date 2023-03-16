using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBase.Web.Mvc.Validation
{
    public abstract class ValidatableObject : System.ComponentModel.DataAnnotations.IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var msg = Validate();
            yield return msg == null?ValidationResult.Success:new ValidationResult(msg);
        }
        public abstract string Validate();
    }
}
