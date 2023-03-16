using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq.Expressions;

namespace ProjectBase.Web.Mvc.Angular
{

    public static class LabelExtensions {

         public static HtmlString NgLabelFor<TModel, TValue>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression,  string cssClass="e2-label") {
            var expresionProvider = HtmlHelperUtil.GetExpressionProvider(html);
            return LabelHelper(html,
                               HtmlHelperUtil.MetaFromExpression(expresionProvider,html,expression),
                               expresionProvider.GetExpressionText(expression),
                               cssClass);
        }

        internal static HtmlString LabelHelper(IHtmlHelper html, ModelMetadata metadata, string htmlFieldName, string cssClass = null) {
            TagBuilder tag = new TagBuilder("label");
            tag.Attributes.Add("for", TagBuilder.CreateSanitizedId(html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName),"-"));
            tag.InnerHtml.SetContent("{{'" + metadata.DisplayName + "'|translate}}");
            if (metadata.IsRequired) tag.Attributes.Add("pb-required", "label");
            tag.Attributes.Add("class", cssClass);
            return new HtmlString(HtmlHelperUtil.ToString(tag));
        }
    }
}
