using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KendoUIHelper
{
    public class MultiSelectBuilder:ListBuilder<MultiSelect,MultiSelectBuilder>
        
    {
        public MultiSelectBuilder(MultiSelect Component):base(Component)
        {

        }

        public MultiSelectBuilder AutoClose(bool autoClose)
        {
            this.Component.AutoClose = autoClose;
            return this;
        }
        public MultiSelectBuilder TagMode(TagMode tagMode)
        {
            this.Component.TagMode = tagMode;
            return this;
        }
        public MultiSelectBuilder AutoBind(bool autoBind)
        {
            this.Component.AutoBind = autoBind;
            return this;
        }
    }
}
