using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace KendoUIHelper
{
    public class ListBuilder<TControl, TBuilder> : ControlBuilderBase<TControl, TBuilder>
         where TControl : List
         where TBuilder : ControlBuilderBase<TControl,TBuilder>
    {
        public ListBuilder(TControl Component):base(Component)
        {

        }

        

        public TBuilder DataSource(string dataSource)
        {
            this.Component.DataSource = dataSource;
            return (this as TBuilder);
        }

        public TBuilder DataTextField(string dataTextField)
        {
            this.Component.DataTextField = dataTextField;
            return (this as TBuilder);
        }

        public TBuilder DataValueField(string dataValueField)
        {
            this.Component.DataValueField = dataValueField;
            return (this as TBuilder);
        }

        public TBuilder Template(string template)
        {
            this.Component.Template = template;
            return (this as TBuilder);
        }

        public TBuilder Enum(string eNum)
        {
            this.Component.Enum = eNum;
            this.Component.Primitive = true;
            if (Component.Name!=null && Component.Name.EndsWith(".Id"))
            {
                Component.Name = Component.Name.Substring(0, Component.Name.Length - 3);
            }
            if (Component.NgModel != null && Component.NgModel.EndsWith(".Id"))
            {
                Component.NgModel = Component.NgModel.Substring(0, Component.NgModel.Length - 3);
            }
            return (this as TBuilder);
        }

        public TBuilder NoDataTemplate(string template)
        {
            this.Component.NoDataTemplate = template;
            return (this as TBuilder);
        }
        public TBuilder ForSimpleType()
        {
            this.Component.Primitive = true;
            Component.DataTextField = null;
            Component.DataValueField = null;
            if (Component.Name != null && Component.Name.EndsWith(".Id"))
            {
                Component.Name = Component.Name.Substring(0, Component.Name.Length - 3);
            }
            if (Component.NgModel != null && Component.NgModel.EndsWith(".Id"))
            {
                Component.NgModel = Component.NgModel.Substring(0, Component.NgModel.Length - 3);
            }
            return (this as TBuilder);
        }
    }
}
