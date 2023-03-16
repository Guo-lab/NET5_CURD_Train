using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KendoUIHelper
{
    public class MultiColumnComboBoxBuilder:ComboBoxBuilderBase<MultiColumnComboBox,MultiColumnComboBoxBuilder>
        
    {
        public MultiColumnComboBoxBuilder(MultiColumnComboBox Component):base(Component)
        {

        }
        public MultiColumnComboBoxBuilder Columns(string columns)
        {
            this.Component.Columns = columns;
            return this;
        }

    }
}
