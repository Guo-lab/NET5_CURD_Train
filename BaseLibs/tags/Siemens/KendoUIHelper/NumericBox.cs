using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;



namespace KendoUIHelper
{
    public class NumericBox: ControlBase
    {
        public NumericBox():base("input", "kendo-numeric-text-box")
        { }

        protected override void WriteControlSpecificAttribute(TagBuilder builder)
        {
            base.WriteControlSpecificAttribute(builder);
            if (!string.IsNullOrEmpty (this.Format))
                builder.MergeAttribute("k-format", "'" + this.Format + "'");

            if (this.Min.HasValue)
                builder.MergeAttribute("k-min", this.Min.Value.ToString ());
            if (this.Max.HasValue)
                builder.MergeAttribute("k-max", this.Max.Value.ToString());
            if (!string.IsNullOrWhiteSpace(this.MinString))
               builder.MergeAttribute("k-min", this.MinString.Trim());
            if (!string.IsNullOrWhiteSpace(this.MaxString))
                builder.MergeAttribute("k-max", this.MaxString.Trim());
            if (this.Scale.HasValue )
                builder.MergeAttribute("k-decimals", this.Scale.Value.ToString());
            if (this.Step.HasValue)
                builder.MergeAttribute("k-step", this.Step.Value.ToString());
        }

        public decimal? Min { get; set; }
        public decimal? Max { get; set; }

        public string MinString { get; set; }
        public string MaxString { get; set; }
        public int? Scale { get; set; }
        public string Format { get; set; }
        public decimal? Step { get; set; } = 1;
    }
}
