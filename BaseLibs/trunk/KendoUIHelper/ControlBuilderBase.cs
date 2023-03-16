using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Html;
using System.IO;
using System.Text.Encodings.Web;

namespace KendoUIHelper
{
    public class ControlBuilderBase<TControl, TBuilder> : IHtmlContent
         where TControl : ControlBase
         where TBuilder : ControlBuilderBase<TControl, TBuilder>
    {
        protected TControl Component
        {
            get; set;
        }


        public ControlBuilderBase(TControl component)
        {
            this.Component = component;
        }
        //public virtual string ToHtmlString()
        //{
        //    return this.Component.ToHtmlString();
        //}

        public TBuilder Name(string name)
        {
            this.Component.Name = name;
            return (this as TBuilder);
        }

        public TBuilder Id(string Id)
        {
            this.Component.Id = Id;
            return (this as TBuilder);
        }
        public TBuilder NgModel(string ngModel, bool primitive)
        {
            this.Component.NgModel = ngModel;
            this.Component.Primitive = primitive;
            return (this as TBuilder);
        }
        public TBuilder NgModel(string ngModel)
        {
            this.Component.NgModel = ngModel;
            return (this as TBuilder);
        }


        public TBuilder Attributes(object attributes)
        {
            return this.Attributes(HtmlHelper.AnonymousObjectToHtmlAttributes(attributes));
        }

        public TBuilder Attributes(IDictionary<string, object> attributes)
        {
            if (this.Component.HtmlAttributes == null)
            {
                this.Component.HtmlAttributes = attributes;
            }
            else
            {
                foreach (var kvp in attributes)
                {
                    if (this.Component.HtmlAttributes.ContainsKey(kvp.Key))
                    {
                        this.Component.HtmlAttributes[kvp.Key] = kvp.Value;
                    }
                    else
                    {
                        this.Component.HtmlAttributes.Add(kvp.Key, kvp.Value);
                    }
                }
            }
            return (this as TBuilder);
        }

        public TBuilder Options(string option)
        {
            this.Component.Options = option;
            return (this as TBuilder);
        }

        public TBuilder Disabled(string disabled)
        {
            this.Component.Disabled = disabled;
            return (this as TBuilder);
        }

        public TBuilder ReadOnly(string readOnly)
        {
            this.Component.ReadOnly = readOnly;
            return (this as TBuilder);
        }

        public TBuilder Events(object events)
        {
            this.Component.Events = HtmlHelper.AnonymousObjectToHtmlAttributes(events);
            return (this as TBuilder);
        }

        public void WriteTo(TextWriter writer, HtmlEncoder encoder)
        {
            writer.Write(this.Component.ToHtmlString());
        }
    }
}
