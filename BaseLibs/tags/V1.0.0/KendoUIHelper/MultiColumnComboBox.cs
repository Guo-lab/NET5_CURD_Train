using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace KendoUIHelper
{
    public class MultiColumnComboBox:ComboBox
    {
        public MultiColumnComboBox():base("kendo-multi-column-combo-box")
        { }

       public string Columns { get; set; }

        protected override void WriteControlSpecificAttribute(TagBuilder builder)
        {
            base.WriteControlSpecificAttribute(builder);
            if (!string.IsNullOrEmpty(this.Columns))
                builder.MergeAttribute("k-columns", this.Columns);
           
        }
    }
}
