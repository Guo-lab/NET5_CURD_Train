using Microsoft.AspNetCore.Mvc.Rendering;

namespace KendoUIHelper
{
    public class ComboBox:SuggestionList
    {
        public ComboBox():base("select","kendo-combo-box")
        { }

        public ComboBox(string directiveName):base("select",directiveName)
        {

        }
        protected override void WriteControlSpecificAttribute(TagBuilder builder)
        {
            base.WriteControlSpecificAttribute(builder);
            if (this.AutoBind.HasValue)
                builder.MergeAttribute("k-auto-bind", this.AutoBind.Value.ToString().ToLower());
            if (!string.IsNullOrEmpty (this.Text))
                builder.MergeAttribute("k-text", this.Text);
        }

        public bool? AutoBind { get; set; }
        public string Text { get; set; }
    }
}
