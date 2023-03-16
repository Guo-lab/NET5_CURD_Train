using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ProjectBase.Web.Mvc;
using ProjectBase.Web.Mvc.Angular;
using ProjectBase.Web.Mvc.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace KendoUIHelper
{
    public static class HtmlExtensions
    {
        private static string Attr_VMPrefix = "NetNgArch_Attr_VMPrefix";
        //private static string FormArray_NgModel_DefaultPrefix = "item.";

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
            bool primitive = true;
            if (htmlAttributes.ContainsKey("primitive") && htmlAttributes["primitive"].ToString().Equals("false",StringComparison.OrdinalIgnoreCase))
            {
                primitive = false;
            }
            if (NoDotId)
            {
                htmlAttributes.Remove("no-dot-Id");
            }

            string modelName = HtmlExtensions.GetModelName(htmlHelper, expression);
            TControlBuilder builder = ControlFactory.CreateBuilder<TControl, TControlBuilder>();

            if (!NoDotId)
            {
                modelName = modelName + ".Id";
            }
            if (!htmlAttributes.ContainsKey("ng-model"))
            {
                builder.NgModel(HtmlExtensions.GetNgModel(htmlHelper, modelName), primitive);
            }

            GenerateAttributeVMAnnotation(htmlHelper, expression, builder, htmlAttributes);
            if (HtmlHelperExtension.IsInFormArray(htmlHelper))
            {
                var names = HtmlHelperExtension.KendoAdjustNameInFormArray(htmlHelper, modelName);
                return builder.NgModel(names[0]).Name(names[1]).Attributes(htmlAttributes);
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
            GenerateAttributeVMAnnotation(htmlHelper, expression, builder, htmlAttributes);
            if (HtmlHelperExtension.IsInFormArray(htmlHelper))
            {
                var names= HtmlHelperExtension.KendoAdjustNameInFormArray(htmlHelper, modelName);
                return builder.NgModel(names[0]).Name(names[1]).Attributes(htmlAttributes);
            }
            return builder.Name(modelName).Attributes(htmlAttributes);
        }
        //根据VM中的标记生成相应客户端控件的相关属性（不是验证指令，而是控件的与验证有关的属性，比如非必填的下拉框自动加一个空选项）
        private static void GenerateAttributeVMAnnotation<TControl, TControlBuilder, TModel, TProperty>(IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression,ControlBuilderBase<TControl, TControlBuilder> builder, IDictionary<string, object> htmlAttributes)
            where TControl : ControlBase
            where TControlBuilder : ControlBuilderBase<TControl, TControlBuilder>
        {
            if (builder is DropDownListBuilder && !htmlAttributes.ContainsKey("required"))
            {
                (builder as DropDownListBuilder).OptionLabel();
            }
            else if (builder is DatePickerBuilder || builder is DateTimePickerBuilder)
            {
                var expresionProvider = HtmlHelperUtil.GetExpressionProvider(htmlHelper);
                ModelMetadata meta = HtmlHelperUtil.MetaFromExpression(expresionProvider, htmlHelper, expression);
                var rangeAttr = Attribute.GetCustomAttribute(meta.ContainerType.GetProperty(meta.PropertyName), typeof(RangeAttribute), true) as RangeAttribute;
                if (rangeAttr != null)
                {
                    var b1 = builder as DatePickerBuilder;
                    var b2=builder as DateTimePickerBuilder;
                    if (rangeAttr.Min != null)
                    {
                        object t=b1 != null ?
                            b1.Min(Convert.ToDateTime(rangeAttr.Min))
                        : b2.Min(Convert.ToDateTime(rangeAttr.Min));
                    }
                    if (rangeAttr.Max != null)
                    {
                        object t = b1 != null ?
                            b1.Max(Convert.ToDateTime(rangeAttr.Max))
                        : b2.Max(Convert.ToDateTime(rangeAttr.Max));
                    }
                }
            }
            else if (builder is NumericBoxBuilder)
            {
                var b = builder as NumericBoxBuilder;
                var expresionProvider = HtmlHelperUtil.GetExpressionProvider(htmlHelper);
                ModelMetadata meta = HtmlHelperUtil.MetaFromExpression(expresionProvider, htmlHelper, expression);
                var attr =Attribute.GetCustomAttribute(meta.ContainerType.GetProperty(meta.PropertyName),typeof(DecimalFormatAttribute), true) as DecimalFormatAttribute;
                if (attr != null)
                {
                    b.Scale(attr.Scale);
                    b.Format("n"+ attr.Scale);
                    // b.Precision(attr.Precision);
                }
                else if (meta.ModelType== typeof(decimal) || meta.ModelType == typeof(decimal?))
                {
                    b.Scale(ClientDataTypeModelValidatorProvider.DefaultDecimalScale);
                    b.Format("n" + ClientDataTypeModelValidatorProvider.DefaultDecimalScale);
                }
                else
                {
                    b.Scale(0);
                    b.Format("n0");
                }
                var rangeAttr = Attribute.GetCustomAttribute(meta.ContainerType.GetProperty(meta.PropertyName), typeof(RangeAttribute), true) as RangeAttribute;
                if (rangeAttr != null)
                {
                    if (rangeAttr.Min != null)
                    {
                        b.Min(Convert.ToDecimal(rangeAttr.Min));
                    }
                    if (rangeAttr.Max != null)
                    {
                        b.Max(Convert.ToDecimal(rangeAttr.Max));
                    }
                }
            }
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
