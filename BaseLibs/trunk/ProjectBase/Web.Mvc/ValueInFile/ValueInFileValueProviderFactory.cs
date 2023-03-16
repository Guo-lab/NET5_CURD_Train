using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectBase.Web.Mvc.ValueInFile
{
    public class ValueInFileValueProviderFactory : IValueProviderFactory
    {
        public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            var actionParams = ((ControllerActionDescriptor)context.ActionContext.ActionDescriptor).MethodInfo.GetParameters();
            var fileNameParam = actionParams.Where(o => o.GetCustomAttributes(false).OfType<ValueInFileAttribute>().SingleOrDefault() != null).FirstOrDefault();
            if (fileNameParam != null)
            {
                var resourceName = fileNameParam.GetCustomAttributes(false).OfType<ValueInFileAttribute>().Single().FileParserResourceName;
                context.ValueProviders.Add(new ValueInFileValueProvider(context, fileNameParam.Name, resourceName));
            }
            return Task.CompletedTask;
        }
    }
}