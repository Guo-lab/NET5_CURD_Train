using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace KendoUIHelper
{
    public class TimePickerBase:DatePickerBase
    {
        public TimePickerBase(string tagName, string directiveName) :base (tagName,directiveName)
        {
        }
        protected override void WriteControlSpecificAttribute(TagBuilder builder)
        {
            base.WriteControlSpecificAttribute(builder);
            if (this.Interval.HasValue)
                builder.MergeAttribute("k-interval", this.Interval.Value.ToString ());
        }

        public int? Interval { get; set; }
        public override string Format { get; set; } = "HH:mm:ss";
    }
}
