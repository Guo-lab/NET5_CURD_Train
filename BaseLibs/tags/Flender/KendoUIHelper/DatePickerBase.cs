using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace KendoUIHelper
{
    public class DatePickerBase : ControlBase
    {
        public DatePickerBase(string tagName, string directiveName) :base (tagName,directiveName)
        {
        }

        protected override void WriteControlSpecificAttribute(TagBuilder builder)
        {
            base.WriteControlSpecificAttribute(builder);
            if (!string.IsNullOrEmpty(this.Format))
                builder.MergeAttribute("k-format", "'" + this.Format + "'");
            if (this.Max.HasValue)
            {
                builder.MergeAttribute("k-max", "'" + this.Max.ToString() + "'");
            }
            if (!string.IsNullOrEmpty(this.MaxString))
            {
                builder.MergeAttribute("k-max", this.MaxString);
            }
            if (this.Min.HasValue)
            {
                builder.MergeAttribute("k-min", "'" + this.Min.Value.ToString() + "'");
            }
            if (!string.IsNullOrEmpty(this.MinString))
            {
                builder.MergeAttribute("k-min", this.MinString);
            }
            if (!string.IsNullOrEmpty(this.Start))
            {
                builder.MergeAttribute("k-start", "'" + this.Start + "'");
            }
            if (!string.IsNullOrEmpty(this.Depth))
            {
                builder.MergeAttribute("k-depth", "'" + this.Depth + "'");
            }
        }

        public string Start { get; set; }
        public string Depth { get; set; }

        public virtual string Format { get; set; }
        public string MaxString { get; set; }
        public DateTime? Max { get; set; }
        public string MinString { get; set; }
        public DateTime? Min { get; set; }

        
    }
}
