using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace KendoUIHelper
{
    public enum KendoFilter
    {
        StartsWith,
        EndsWith,
        Contains,
    }
    public class SuggestionList:List
    {
        public SuggestionList(string tagName,string directiveName):base(tagName, directiveName)
        { }

        protected override void WriteControlSpecificAttribute(TagBuilder builder)
        {
            base.WriteControlSpecificAttribute(builder);
            
            int nMinLength = this.MinLength.HasValue ? this.MinLength.Value : 1;
            builder.MergeAttribute("k-min-length", nMinLength.ToString());
            builder.MergeAttribute("k-enforce-min-length", "true");
            if (this.Filter.HasValue)
                builder.MergeAttribute("k-filter", "'" + this.Filter.Value.ToString().ToLower() + "'");

        }

        public int? MinLength { get; set; }
        public KendoFilter? Filter { get; set; }
    }
}
