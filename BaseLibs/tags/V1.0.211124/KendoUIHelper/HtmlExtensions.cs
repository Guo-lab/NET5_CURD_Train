using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ProjectBase.Web.Mvc.Angular;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace KendoUIHelper
{
    public static class HtmlExtensions
    {
        private static string Attr_VMPrefix = "NetNgArch_Attr_VMPrefix";

        public static ControlFactory Factory(this IHtmlHelper helper)
        {
            return new ControlFactory(helper);
        }

        #region "Kendo for"

        public static TControlBuilder KendoForList<TControl, TControlBuilder, TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
            where TControl : ControlBase
            where TControlBuilder : ControlBuilderBase<TControl, TControlBuilder>
        {
            htmlAttributes = HtmlExtensions.AddClientValidation(htmlHelper, expression, htmlAttributes);
            bool NoDotId = htmlAttributes.ContainsKey("no-dot-Id");
            bool primitive = htmlAttributes.ContainsKey("primitive");
            if (NoDotId)
            {
                htmlAttributes.Remove("no-dot-Id");
            }

            string modelName = HtmlExtensions.GetModelName(htmlHelper, expression);
            TControlBuilder builder = ControlFactory.CreateBuilder<TControl, TControlBuilder>();

            if (!htmlAttributes.ContainsKey("ng-model"))
            {
                builder.NgModel(HtmlExtensions.GetNgModel(htmlHelper, modelName), primitive);
            }

            if (!NoDotId)
            {
                modelName = modelName + ".Id";
            }

            return builder.Name(modelName).Attributes(htmlAttributes);
        }
        public static TControlBuilder KendoFor<TControl, TControlBuilder, TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
            where TControl : ControlBase
            where TControlBuilder : ControlBuilderBase<TControl, TControlBuilder>
        {

            htmlAttributes = HtmlExtensions.AddClientValidation(htmlHelper, expression, htmlAttributes);
            string modelName = HtmlExtensions.GetModelName(htmlHelper, expression);

            TControlBuilder builder = ControlFactory.CreateBuilder<TControl, TControlBuilder>();
            if (!htmlAttributes.ContainsKey("ng-model"))
            {
                builder.NgModel(HtmlExtensions.GetNgModel(htmlHelper, modelName));
            }

            return builder.Name(modelName).Attributes(htmlAttributes);
        }
        #endregion

        public static string GetNgModel(this IHtmlHelper htmlHelper, string name)
        {
            return htmlHelper.ViewContext.ViewData[Attr_VMPrefix] + "." + name;
        }


        public static string GetModelName<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var expresionProvider = HtmlHelperUtil.GetExpressionProvider(htmlHelper);
            return expresionProvider.GetExpressionText(expression);
        }

        public static IDictionary<string, object> AddClientValidation<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            var expresionProvider = HtmlHelperUtil.GetExpressionProvider(htmlHelper);
            if (htmlAttributes == null)
            {
                htmlAttributes = new Dictionary<string, object>() { };
            }
            ModelMetadata meta = HtmlHelperUtil.MetaFromExpression(expresionProvider, htmlHelper, expression);
            if (meta == null)
            {
                return htmlAttributes;
            }

            var metaProvider=htmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(IModelMetadataProvider)) as IModelMetadataProvider;
            var valAttributes = new Dictionary<string, string>() { };
            var validators = meta.ValidatorMetadata.Where(o=>o is IClientModelValidator);
            foreach(var validator in validators)
            {
                (validator as IClientModelValidator).AddValidation(new ClientModelValidationContext(new ActionContext(), meta, metaProvider, valAttributes));
            }
            foreach (var pair in valAttributes)
            {
                htmlAttributes[pair.Key] = pair.Value;
            }
            
            return HtmlHelperExtension.AddHtmlValidationAttributes(meta, htmlAttributes);
        }
    }
}
