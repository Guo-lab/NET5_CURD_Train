using Microsoft.AspNetCore.Mvc.Rendering;


namespace KendoUIHelper
{
    public abstract class List:ControlBase
    {
        public List(string tagName, string directiveName) :base (tagName,directiveName)
        {
        }

        protected override void WriteControlSpecificAttribute(TagBuilder builder)
        {
            base.WriteControlSpecificAttribute(builder);
            builder.MergeAttribute("k-auto-width", "true");
            if (!string.IsNullOrEmpty(this.Template))
                builder.MergeAttribute("k-template", "'" + this.Template + "'");
            if (!string.IsNullOrEmpty(this.NoDataTemplate))
            {
                builder.MergeAttribute("k-no-data-template", this.NoDataTemplate);
            }
            else
            {
                builder.MergeAttribute("k-no-data-template", "false");
            }
            if (!string.IsNullOrEmpty(this.Enum))
            {
                builder.MergeAttribute("k-data-source", "DictObj." + this.Enum);
                builder.MergeAttribute("k-data-text-field", "'Text'");
                builder.MergeAttribute("k-data-value-field", "'Id'");
            }
            else {
                if (!string.IsNullOrEmpty(this.DataTextField))
                {
                    builder.MergeAttribute("k-data-text-field", "'" + this.DataTextField + "'");
                }
                if (!string.IsNullOrEmpty(this.DataValueField))
                {
                    builder.MergeAttribute("k-data-value-field", "'" + this.DataValueField + "'");
                }
                if (!string.IsNullOrEmpty(this.DataSource))
                {
                    builder.MergeAttribute("k-data-source", this.DataSource);
                }
            }
        }

        public string DataSource { get; set; }
        public string Enum { get; set; }

        public string DataTextField { get; set; }

        public string DataValueField { get; set; }

        public string Template { get; set; }


        public string NoDataTemplate { get; set; }
    }
}
