using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace KendoUIHelper
{
    public class DropDownList:List
    {
        public DropDownList():base("select", "kendo-drop-down-list")
        { }

        protected override void WriteControlSpecificAttribute(TagBuilder builder)
        {
            base.WriteControlSpecificAttribute(builder);
            
            if (!string.IsNullOrEmpty (this.OptionLabel))
                builder.MergeAttribute("k-option-label", "'" + this.OptionLabel + "'|translate");

        }

        public string OptionLabel { get; set; }
    }
}
