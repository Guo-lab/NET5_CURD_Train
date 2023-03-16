using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KendoUIHelper
{
    public class DropDownListBuilder:ListBuilder<DropDownList,DropDownListBuilder>
        
    {
        public DropDownListBuilder(DropDownList Component):base(Component)
        {

        }
           
        public DropDownListBuilder OptionLabel(string optionLabel)
        {
            this.Component.OptionLabel = optionLabel;
            return this;
        }     
    }
}
