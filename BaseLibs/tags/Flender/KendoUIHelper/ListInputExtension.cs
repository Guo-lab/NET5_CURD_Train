using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


namespace KendoUIHelper
{
    public static class ListInputExtension
    {

        #region "AutoComplete"
        public static AutoCompleteBuilder KendoAutoCompleteFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return htmlHelper.KendoAutoCompleteFor(expression,null);
        }
        
        public static AutoCompleteBuilder KendoAutoCompleteFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return htmlHelper.KendoAutoCompleteFor(expression,  HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static AutoCompleteBuilder KendoAutoCompleteFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression,  IDictionary<string, object> htmlAttributes)
        {
            return HtmlExtensions.KendoForList<AutoComplete, AutoCompleteBuilder, TModel, TProperty>(htmlHelper, expression, htmlAttributes);
        }
        #endregion

        #region "DropDownList"
        public static DropDownListBuilder KendoDropDownListFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return htmlHelper.KendoDropDownListFor(expression, null);
        }

        public static DropDownListBuilder KendoDropDownListFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return htmlHelper.KendoDropDownListFor(expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static DropDownListBuilder KendoDropDownListFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            return HtmlExtensions.KendoForList<DropDownList, DropDownListBuilder, TModel, TProperty>(htmlHelper, expression, htmlAttributes);
        }
        #endregion

        #region "MultiSelect"
        public static MultiSelectBuilder KendoMultiSelectFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return htmlHelper.KendoMultiSelectFor(expression, null);
        }

        public static MultiSelectBuilder KendoMultiSelectFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return htmlHelper.KendoMultiSelectFor(expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MultiSelectBuilder KendoMultiSelectFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            return HtmlExtensions.KendoForList<MultiSelect, MultiSelectBuilder, TModel, TProperty>(htmlHelper, expression, htmlAttributes);
        }
        #endregion

        #region "ComboBox"
        public static ComboBoxBuilder KendoComboBoxFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return htmlHelper.KendoComboBoxFor(expression, null);
        }

        public static ComboBoxBuilder KendoComboBoxFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return htmlHelper.KendoComboBoxFor(expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static ComboBoxBuilder KendoComboBoxFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            return HtmlExtensions.KendoForList<ComboBox, ComboBoxBuilder, TModel, TProperty>(htmlHelper, expression, htmlAttributes);
        }
        #endregion

    }
}
