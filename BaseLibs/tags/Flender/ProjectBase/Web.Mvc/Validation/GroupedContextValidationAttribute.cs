using System.ComponentModel.DataAnnotations;

namespace ProjectBase.Web.Mvc.Validation
{
    public abstract class GroupedContextValidationAttribute : GroupedValidationAttribute
    {
        public string ClientPropPathPrefix { get; set; } = "c.vm.Input.";
        protected sealed override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            bool? isValid = false;
            if (ShouldVal(validationContext)) {
                isValid = IsValid(validationContext, value);
            }
            return !isValid.Value ? base.IsValid(value, validationContext) : ValidationResult.Success;
        }

        public sealed override bool IsValid(object value)
        {
            return false;
        }
        protected abstract bool IsValid(ValidationContext validationContext, object value);
    }
}
