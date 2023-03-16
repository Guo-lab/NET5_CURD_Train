using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;



namespace KendoUIHelper
{
    public class TextBox:ControlBase
    {
        public TextBox():base("input", "")
        { }

        public TextBox(string tagName):base (tagName,"")
        {

        }

        protected override void WriteNgModel(TagBuilder builder)
        {
            if (!string.IsNullOrEmpty(this.NgModel))
            {
                builder.MergeAttribute("ng-model", this.NgModel);
            }
        }

        protected override void WriteReadOnlyDisabled(TagBuilder builder)
        {
            if (!string.IsNullOrEmpty(this.ReadOnly))
            {
                builder.MergeAttribute("ng-readonly", this.ReadOnly);
            }
            if (!string.IsNullOrEmpty(this.Disabled))
            {
                builder.MergeAttribute("ng-disabled", this.Disabled);
            }
        }
        protected override void WriteControlSpecificAttribute(TagBuilder builder)
        {
            base.WriteControlSpecificAttribute(builder);

            if (HtmlAttributes==null || !HtmlAttributes.ContainsKey("class"))
                builder.MergeAttribute("class", "k-textbox");
            builder.MergeAttribute("type", "text");
            if (HtmlAttributes == null || !HtmlAttributes.ContainsKey("ng-model-options"))
            {
                builder.MergeAttribute("ng-model-options", "{ updateOn: 'blur' }");
            }
            builder.MergeAttribute("ng-trim", "false");
            builder.MergeAttribute("pb-trim", "true");
        }

    }
}
