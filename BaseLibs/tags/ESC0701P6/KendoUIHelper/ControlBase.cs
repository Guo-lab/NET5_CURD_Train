using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using ProjectBase.Web.Mvc;

namespace KendoUIHelper
{
    public class ControlBase
    {

        private string DirectiveName;
        private string TagName;
        public ControlBase(string tagName, string directiveName)
        {
            this.TagName = tagName;
            this.DirectiveName = directiveName;
        }

        protected virtual void WriteControlSpecificAttribute(TagBuilder builder)
        {

        }

        //Textbox, TextArea is not KendoUI, so only ng-model will work
        protected virtual void WriteNgModel(TagBuilder builder)
        {
            if (!string.IsNullOrEmpty(this.NgModel))
            {
                builder.MergeAttribute("k-ng-model", this.NgModel);
                if (this.Primitive.HasValue && this.Primitive.Value)
                    builder.MergeAttribute("k-value-primitive", "true");
            }
        }
        //Textbox, TextArea is not KendoUI, so only ng-readonly, ng-disabled will work
        protected virtual void WriteReadOnlyDisabled(TagBuilder builder)
        {
            if (!string.IsNullOrEmpty(this.ReadOnly))
            {
                builder.MergeAttribute("k-ng-readonly", this.ReadOnly);
            }
            if (!string.IsNullOrEmpty(this.Disabled))
            {
                builder.MergeAttribute("k-ng-disabled", this.Disabled);
            }
        }

        private string AdjustName(string ngModel)
        {
            string namePrefix = (this.HtmlAttributes!=null && this.HtmlAttributes.ContainsKey("ng-name-prefix")) ? (string)this.HtmlAttributes["ng-name-prefix"] : "c.vm";
            if (!namePrefix.EndsWith("."))
            {
                namePrefix = namePrefix + ".";
            }
            return ngModel.Replace(namePrefix, "").Replace("[", "[{{").Replace("]", "}}]");
        }

        private string GetModelName()
        {
            if (!string.IsNullOrEmpty(this.NgModel))
                return this.NgModel;
            if (this.HtmlAttributes != null && this.HtmlAttributes.ContainsKey("ng-model"))
                return this.HtmlAttributes["ng-model"].ToString();
            return "";
        }
        public virtual string ToHtmlString()
        {
            TagBuilder tagBuilder = new TagBuilder(this.TagName);
            if (this.HtmlAttributes != null)
            {
                foreach (KeyValuePair<string, object> kvp in this.HtmlAttributes)
                {
                    tagBuilder.MergeAttribute(kvp.Key, (kvp.Value?.ToString())??"");
                }
            }

            if (this.DirectiveName != "")
            {
                if (!string.IsNullOrEmpty(this.Id))
                    tagBuilder.MergeAttribute(this.DirectiveName, this.Id);
                else
                    tagBuilder.MergeAttribute(this.DirectiveName, string.Empty);
            }
            if (!string.IsNullOrEmpty(this.Name))
            {
                string modelName = GetModelName();
                if (!string.IsNullOrEmpty(modelName) && modelName.IndexOf("[") > 0)
                    tagBuilder.MergeAttribute("name", AdjustName(modelName));
                else
                    tagBuilder.MergeAttribute("name", this.Name);
            }
            WriteNgModel(tagBuilder);
            if (!string.IsNullOrEmpty(this.Options))
            {
                tagBuilder.MergeAttribute("k-options", this.Options);
            }

            if (this.Events != null)
            {
                foreach (KeyValuePair<string, object> kvp in this.Events)
                {
                    var eventString = kvp.Value.ToString();
                    int leftParentheses = eventString.IndexOf("(");
                    int rightParentheses = eventString.IndexOf(")");
                    if (leftParentheses!=-1 && rightParentheses != -1)
                    {

                        tagBuilder.MergeAttribute("k-on-" + kvp.Key.ToLower(), eventString.Substring (0,leftParentheses) + "(kendoEvent," + eventString.Substring (leftParentheses+1,rightParentheses-leftParentheses-1) + ")");
                    }
                    else
                    {
                        tagBuilder.MergeAttribute("k-on-" + kvp.Key.ToLower(), eventString + "(kendoEvent)");
                    }
                    
                }
            }
            WriteReadOnlyDisabled(tagBuilder);

            WriteControlSpecificAttribute(tagBuilder);
            return tagBuilder.GetString();
        }

        public string Id { get; set; }
        public string Name { get; set; }

        public string NgModel { get; set; }
        public IDictionary<string, object> HtmlAttributes { get; set; }

        public string Options { get; set; }

        public IDictionary<string, object> Events { get; set; }

        public string ReadOnly { get; set; }
        public string Disabled { get; set; }

        public bool? Primitive { get; set; }

    }
}
