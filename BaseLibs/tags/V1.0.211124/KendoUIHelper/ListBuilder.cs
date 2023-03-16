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
            return (this as TBuilder);
        }

        public TBuilder NoDataTemplate(string template)
        {
            this.Component.NoDataTemplate = template;
            return (this as TBuilder);
        }
    }
}
