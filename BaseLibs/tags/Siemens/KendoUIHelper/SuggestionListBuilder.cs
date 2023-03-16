using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KendoUIHelper
{
    public class SuggestionListBuilder<TControl,TBuilder>:ListBuilder<TControl, TBuilder>
        where TControl : SuggestionList
        where TBuilder : ControlBuilderBase<TControl, TBuilder>
    {
        public SuggestionListBuilder(TControl Component):base(Component)
        {

        }

        public TBuilder Filter(KendoFilter filter)
        {
            this.Component.Filter = filter;
            return (this as TBuilder);
        }

        public TBuilder MinLength(int minLength)
        {
            this.Component.MinLength = minLength;
            return (this as TBuilder);
        }

        
    }
}
