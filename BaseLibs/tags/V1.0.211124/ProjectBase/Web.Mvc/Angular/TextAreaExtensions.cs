namespace ProjectBase.Web.Mvc.Angular
{
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

    public static class TextAreaExtensions {

        public static HtmlString NgTextAreaFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression) {
            return NgTextAreaFor(htmlHelper, expression, null,null);
        }
        public static HtmlString NgTextAreaFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression,string cssClass)
        {
            return NgTextAreaFor(htmlHelper, expression, cssClass, null);
        }
        public static HtmlString NgTextAreaFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string cssClass, object htmlAttributes)
        {
            return NgTextAreaFor(htmlHelper, expression,cssClass, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static HtmlString NgTextAreaFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string cssClass, IDictionary<string, object> htmlAttributes)
        {
            if (expression == null) {
                throw new ArgumentNullException("expression");
            }
            var ngModelExplicit = htmlAttributes != null && htmlAttributes.ContainsKey("ng-model");
            htmlAttributes = HtmlHelperExtension.AddClientValidation(htmlHelper, expression, htmlAttributes);
            if (!String.IsNullOrEmpty(cssClass)) htmlAttributes["class"] = cssClass;
            if (!htmlAttributes.ContainsKey("ng-model-options"))
            {
                htmlAttributes["ng-model-options"] = "{ updateOn: 'blur' }";
            }
            return HtmlHelperExtension.AdjustName(HtmlHelperInputExtensions.TextAreaFor(htmlHelper,expression, htmlAttributes), htmlAttributes, ngModelExplicit);
        }

        public static HtmlString NgTextAreaFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, int rows, int columns, object htmlAttributes)
        {
            return NgTextAreaFor(htmlHelper, expression, null, rows, columns, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }
        public static HtmlString NgTextAreaFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string cssClass, int rows, int columns, object htmlAttributes)
        {
            return NgTextAreaFor(htmlHelper, expression, cssClass, rows, columns, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static HtmlString NgTextAreaFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string cssClass,int rows, int columns, IDictionary<string, object> htmlAttributes) {
            if (expression == null) {
                throw new ArgumentNullException("expression");
            }
            var ngModelExplicit = htmlAttributes != null && htmlAttributes.ContainsKey("ng-model");
            htmlAttributes = HtmlHelperExtension.AddClientValidation(htmlHelper, expression, htmlAttributes);
            if (!String.IsNullOrEmpty(cssClass)) htmlAttributes["class"] = cssClass;
            if (!htmlAttributes.ContainsKey("ng-model-options"))
            {
                htmlAttributes["ng-model-options"] = "{ updateOn: 'blur' }";
            }
            if (rows > 0)
            {
                htmlAttributes["rows"] = rows;
            }
            if (columns > 0)
            {
                htmlAttributes["cols"] = columns;
            }
            return HtmlHelperExtension.AdjustName(HtmlHelperInputExtensions.TextAreaFor(htmlHelper, expression, htmlAttributes), htmlAttributes, ngModelExplicit);
        }

    }
}
