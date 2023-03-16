using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace KendoUIHelper
{
    public static class DateInputExtension
    {
        #region "DatePicker"
        public static DatePickerBuilder KendoDatePickerFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return htmlHelper.KendoDatePickerFor(expression,   null);
        }
        
        public static DatePickerBuilder KendoDatePickerFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return htmlHelper.KendoDatePickerFor(expression,  HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static DatePickerBuilder KendoDatePickerFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression,  IDictionary<string, object> htmlAttributes)
        {
            return HtmlExtensions.KendoFor<DatePicker, DatePickerBuilder, TModel, TProperty>(htmlHelper, expression, htmlAttributes);
        }
        #endregion
        #region "DateTimePicker"
        public static DateTimePickerBuilder KendoDateTimePickerFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return htmlHelper.KendoDateTimePickerFor(expression, null);
        }

        public static DateTimePickerBuilder KendoDateTimePickerFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return htmlHelper.KendoDateTimePickerFor(expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static DateTimePickerBuilder KendoDateTimePickerFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            return HtmlExtensions.KendoFor<DateTimePicker, DateTimePickerBuilder, TModel, TProperty>(htmlHelper, expression, htmlAttributes);
        }
        #endregion
        #region "TimePicker"
        public static TimePickerBuilder KendoTimePickerFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return htmlHelper.KendoTimePickerFor(expression, null);
        }

        public static TimePickerBuilder KendoTimePickerFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return htmlHelper.KendoTimePickerFor(expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static TimePickerBuilder KendoTimePickerFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            return HtmlExtensions.KendoFor<TimePicker, TimePickerBuilder, TModel, TProperty>(htmlHelper, expression, htmlAttributes);
        }
        #endregion
    }
}
