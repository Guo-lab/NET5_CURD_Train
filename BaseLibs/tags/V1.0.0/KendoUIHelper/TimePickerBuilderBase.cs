using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KendoUIHelper
{
    public class TimePickerBuilderBase<TControl,TBuilder>:DatePickerBuilderBase<TControl, TBuilder>
        where TControl : TimePickerBase
        where TBuilder : ControlBuilderBase<TControl, TBuilder>
    {
        public TimePickerBuilderBase(TControl Component):base(Component)
        {

        }

        public TBuilder Interval(int interval)
        {
            this.Component.Interval = interval;
            return (this as TBuilder);
        }

        
    }
}
