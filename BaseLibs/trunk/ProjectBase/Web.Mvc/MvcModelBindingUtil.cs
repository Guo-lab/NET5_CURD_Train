using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using ProjectBase.Web.Mvc.ValueInFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectBase.Application;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ProjectBase.Web.Mvc
{
    public class MvcModelBindingUtil: IModelBindingUtil
    {
        public IModelMetadataProvider MetadataProvider { get; set; }
        public IModelBinderFactory ModelBinderFactory { get; set; }
        public IObjectModelValidator ObjectValidator { get; set; }
        public async Task<Tuple<object, string[]>> BindModelWithValueInFile(object context, string fileName, IValueInFileParser fileParser, object modelInitValue, string modelName)
        {
            var controllerContext = (ControllerContext)context;
            var meta = MetadataProvider.GetMetadataForType(modelInitValue.GetType());
            var valueProvider = new ValueInFileValueProvider(new ValueProviderFactoryContext(controllerContext), fileName, fileParser);
            var binder = ModelBinderFactory.CreateBinder(new ModelBinderFactoryContext { Metadata = meta });
            var bindingContext = DefaultModelBindingContext.CreateBindingContext(controllerContext,
                valueProvider,
                meta,
                BindingInfo.GetBindingInfo(new object[] { }, meta),
                modelName);

            await binder.BindModelAsync(bindingContext);
            var importItems = bindingContext.Result.Model;
            ObjectValidator.Validate(controllerContext, null, modelName, importItems);

            var msgs =new List<string>();
            var modelStates = bindingContext.ModelState.Values;
            foreach (var modelState in modelStates)
            {
                foreach (var modelError in modelState.Errors)
                {
                    string errorText = modelError.ErrorMessage;
                    if (string.IsNullOrEmpty(errorText) && modelError.Exception != null)
                    {
                        errorText = modelError.Exception.InnerException == null ?
                            modelError.Exception.Message :
                            modelError.Exception.InnerException.Message;
                    }
                    if (!string.IsNullOrEmpty(errorText))
                    {
                        msgs.Add(errorText);
                    }
                }
            }

            return Tuple.Create(importItems, msgs.ToArray());
        }
    }
}
