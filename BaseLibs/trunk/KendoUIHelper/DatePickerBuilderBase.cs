using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace KendoUIHelper
{
    public class DatePickerBuilderBase<TControl, TBuilder> : ControlBuilderBase<TControl, TBuilder>
         where TControl : DatePickerBase
         where TBuilder : ControlBuilderBase<TControl,TBuilder>
    {
        public DatePickerBuilderBase(TControl Component):base(Component)
        {

        }

        public TBuilder Start(string start)
        {
            this.Component.Start = start;
            return (this as TBuilder);
        }

        public TBuilder Depth(string depth)
        {
            this.Component.Depth = depth;
            return (this as TBuilder);
        }

        public TBuilder Format(string format)
        {
            this.Component.Format = format;
            return (this as TBuilder);
        }

        public TBuilder Max(DateTime max)
        {
            this.Component.Max = max;
            return (this as TBuilder);
        }
        public TBuilder MaxString(string max)
        {
            this.Component.MaxString = max;
            return (this as TBuilder);
        }

        public TBuilder Min(DateTime min)
        {
            this.Component.Min = min;
            return (this as TBuilder);
        }

        public TBuilder MinString(string min)
        {
            this.Component.MinString = min;
            return (this as TBuilder);
        }


    }
}
