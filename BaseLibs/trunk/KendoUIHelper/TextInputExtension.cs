using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


namespace KendoUIHelper
{
    public static class TextInputExtension
    {

        #region "TextBox"
        public static TextBoxBuilder KendoTextBoxFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return htmlHelper.KendoTextBoxFor(expression,null);
        }
        
        public static TextBoxBuilder KendoTextBoxFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return htmlHelper.KendoTextBoxFor(expression,  HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static TextBoxBuilder KendoTextBoxFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression,  IDictionary<string, object> htmlAttributes)
        {
            return HtmlExtensions.KendoFor<TextBox, TextBoxBuilder, TModel, TProperty>(htmlHelper, expression, htmlAttributes);

        }
        #endregion

        #region "TextArea"
        public static TextAreaBuilder KendoTextAreaFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return htmlHelper.KendoTextAreaFor(expression, null);
        }

        public static TextAreaBuilder KendoTextAreaFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return htmlHelper.KendoTextAreaFor(expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static TextAreaBuilder KendoTextAreaFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            return HtmlExtensions.KendoFor<TextArea, TextAreaBuilder, TModel, TProperty>(htmlHelper, expression, htmlAttributes);
        }
        #endregion

        #region "NumericBox"
        public static NumericBoxBuilder KendoNumericBoxFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return htmlHelper.KendoNumericBoxFor(expression, null);
        }

        public static NumericBoxBuilder KendoNumericBoxFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return htmlHelper.KendoNumericBoxFor(expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static NumericBoxBuilder KendoNumericBoxFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            return HtmlExtensions.KendoFor<NumericBox, NumericBoxBuilder, TModel, TProperty>(htmlHelper, expression, htmlAttributes);
        }
        #endregion

    }
}
