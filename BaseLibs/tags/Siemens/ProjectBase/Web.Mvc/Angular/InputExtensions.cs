using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Html;

namespace ProjectBase.Web.Mvc.Angular{


    public static class InputExtensions {

        // Hidden
        public static HtmlString NgHiddenFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return NgHiddenFor(htmlHelper, expression, null /* htmlAttributes */);
        }

        public static HtmlString NgHiddenFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return NgHiddenFor(htmlHelper, expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static HtmlString NgHiddenFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            var ngModelExplicit = htmlAttributes != null && htmlAttributes.ContainsKey("ng-model");
            htmlAttributes = HtmlHelperExtension.AddNgModel(htmlHelper, expression, htmlAttributes);
            HtmlHelperExtension.AdjustNameInFormArray(htmlHelper, htmlAttributes);
            var value = "{{" + htmlAttributes["ng-model"] + "}}";
            //去掉ng-model以便生成的html不带此此指令
            htmlAttributes.Remove("ng-model");
            htmlAttributes.Remove("type");
            var expresionProvider = HtmlHelperUtil.GetExpressionProvider(htmlHelper);
            var html = htmlHelper.Hidden(expresionProvider.GetExpressionText(expression), value, htmlAttributes);
            return HtmlHelperExtension.AdjustName(html, htmlAttributes, ngModelExplicit);
        }

  // CheckBox

        public static HtmlString NgCheckBoxFor<TModel>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression)
        {
            return NgCheckBoxFor(htmlHelper, expression,null,null, null /* htmlAttributes */);
        }
        public static HtmlString NgCheckBoxFor<TModel>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression, string cssClass)
        {
            return NgCheckBoxFor(htmlHelper, expression, cssClass,null,null /* htmlAttributes */);
        }
        public static HtmlString NgCheckBoxFor<TModel>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression, string cssClass, string value, object htmlAttributes)
        {
            return NgCheckBoxFor(htmlHelper, expression, cssClass, value, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static HtmlString NgCheckBoxFor<TModel>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression, string cssClass, string value, IDictionary<string, object> htmlAttributes)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
             var ngModelExplicit = htmlAttributes!=null&&htmlAttributes.ContainsKey("ng-model");
            htmlAttributes = HtmlHelperExtension.AddNgModel(htmlHelper, expression, htmlAttributes);
            if (!String.IsNullOrEmpty(cssClass)) htmlAttributes["class"] = cssClass;
            if (value != null)
            {
                htmlAttributes["ng-true-value"] = "'" + value + "'";
            }
            HtmlHelperExtension.AdjustNameInFormArray(htmlHelper, htmlAttributes);
            return HtmlHelperExtension.AdjustName(htmlHelper.CheckBoxFor(expression, htmlAttributes), htmlAttributes, ngModelExplicit);
        }

        // Password

        public static HtmlString NgPasswordFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return NgPasswordFor(htmlHelper, expression,null, null /* htmlAttributes */);
        }
        public static HtmlString NgPasswordFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string cssClass)
        {
            return NgPasswordFor(htmlHelper, expression, cssClass, null /* htmlAttributes */);
        }
        public static HtmlString NgPasswordFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string cssClass, object htmlAttributes)
        {
            return NgPasswordFor(htmlHelper, expression,cssClass, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static HtmlString NgPasswordFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string cssClass, IDictionary<string, object> htmlAttributes)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            var ngModelExplicit = htmlAttributes != null && htmlAttributes.ContainsKey("ng-model");
            htmlAttributes=HtmlHelperExtension.AddClientValidation(htmlHelper, expression, htmlAttributes);
            if (!String.IsNullOrEmpty(cssClass)) htmlAttributes["class"] = cssClass;
            if (!htmlAttributes.ContainsKey("ng-model-options"))
            {
                htmlAttributes["ng-model-options"] = "{ updateOn: 'blur' }";
            }
            HtmlHelperExtension.AdjustNameInFormArray(htmlHelper, htmlAttributes);
            return HtmlHelperExtension.AdjustName(htmlHelper.PasswordFor(expression, htmlAttributes), htmlAttributes, ngModelExplicit);
        }


        // TextBox

        public static HtmlString NgTextBoxFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return htmlHelper.NgTextBoxFor(expression,null, null);
        }
        public static HtmlString NgTextBoxFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string cssClass)
        {
            return htmlHelper.NgTextBoxFor(expression,cssClass, null);
        }

         public static HtmlString NgTextBoxFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string cssClass, object htmlAttributes)
         {
            return htmlHelper.NgTextBoxFor(expression,cssClass, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }
        public static HtmlString NgTextBoxFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string cssClass, IDictionary<string, object> htmlAttributes)
        {
            var ngModelExplicit = htmlAttributes != null && htmlAttributes.ContainsKey("ng-model");
            htmlAttributes = HtmlHelperExtension.AddClientValidation(htmlHelper, expression, htmlAttributes);
            if (!String.IsNullOrEmpty(cssClass)) htmlAttributes["class"] = cssClass;
            if (htmlAttributes.ContainsKey("type") && (String)htmlAttributes["type"] == "date")
            {
                htmlAttributes.Remove("type");
                htmlAttributes["uib-datepicker-popup"] = "yyyy-MM-dd";
                if (!htmlAttributes.ContainsKey("pb-datepicker-button"))
                {
                    htmlAttributes["pb-datepicker-button"] = "yes";
                }
            }
            else
            if (!htmlAttributes.ContainsKey("ng-model-options"))
            {
                htmlAttributes["ng-model-options"] = "{ updateOn: 'blur' }";
            }
            htmlAttributes["ng-trim"] = "false";
            htmlAttributes["pb-trim"] = "true";
            HtmlHelperExtension.AdjustNameInFormArray(htmlHelper, htmlAttributes);
            return HtmlHelperExtension.AdjustName(HtmlHelperInputExtensions.TextBoxFor(htmlHelper, expression, htmlAttributes), htmlAttributes, ngModelExplicit);
        }

      
    }
}
