using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace KendoUIHelper
{
    public enum TagMode
    {
        Single,
        Multiple
    }
    public class MultiSelect:List
    {
        public MultiSelect():base("select", "kendo-multi-select")
        { }

        protected override void WriteControlSpecificAttribute(TagBuilder builder)
        {
            base.WriteControlSpecificAttribute(builder);
            if (this.AutoBind.HasValue)
                builder.MergeAttribute("k-auto-bind", this.AutoBind.Value.ToString().ToLower());
            if (this.AutoClose.HasValue)
                builder.MergeAttribute("k-auto-close", this.AutoClose.Value.ToString ().ToLower());
            if (this.TagMode.HasValue)
                builder.MergeAttribute("k-tag-mode", "'" + this.TagMode.Value.ToString().ToLower() + "'");
        }

        public bool? AutoClose { get; set; }
        public bool? AutoBind { get; set; }
        public TagMode? TagMode { get; set; }       
    }
}
