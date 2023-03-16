using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Html;

namespace ProjectBase.Web.Mvc.Angular
{
    public static class RadioButtonExtension
    {
        public static HtmlString NgRadioButtonFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object value)
        {
            return NgRadioButtonFor(htmlHelper, expression, value,null, null /* htmlAttributes */);
        }
        public static HtmlString NgRadioButtonFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object value,string cssClass)
        {
            return NgRadioButtonFor(htmlHelper, expression, value,cssClass, null /* htmlAttributes */);
        }
        public static HtmlString NgRadioButtonFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object value, string cssClass, object htmlAttributes)
        {
            return NgRadioButtonFor(htmlHelper, expression, value,cssClass, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static HtmlString NgRadioButtonFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object value, string cssClass, IDictionary<string, object> htmlAttributes)
        {
            var ngModelExplicit = htmlAttributes != null && htmlAttributes.ContainsKey("ng-model");
            htmlAttributes = HtmlHelperExtension.AddNgModel(htmlHelper, expression, htmlAttributes);
            if (!String.IsNullOrEmpty(cssClass)) htmlAttributes["class"] = cssClass;
            if(value==null)
            {
                value = "";
			    htmlAttributes["ng-value"]= "null";
		    }else if(value is bool)
            {
                htmlAttributes["ng-value"] = value.ToString().ToLower();
            }
            else
            {
                htmlAttributes["ng-value"] = "'" + value + "'";
            }
            return HtmlHelperExtension.AdjustName(htmlHelper.RadioButtonFor(expression, value, htmlAttributes), htmlAttributes, ngModelExplicit);
        }
      

    }
}
