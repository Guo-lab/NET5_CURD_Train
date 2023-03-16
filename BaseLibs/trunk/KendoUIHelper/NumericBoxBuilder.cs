using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KendoUIHelper
{
    public class NumericBoxBuilder:ControlBuilderBase<NumericBox, NumericBoxBuilder>
        
    {
        public NumericBoxBuilder(NumericBox Component):base(Component)
        {

        }

        public NumericBoxBuilder Min(decimal min)
        {
            this.Component.Min = min;
            return this;
        }
        public NumericBoxBuilder Max(decimal max)
        {
            this.Component.Max = max;
            return this;
        }

        public NumericBoxBuilder MinString(string min)
        {
            this.Component.MinString = min;
            return this;
        }
        public NumericBoxBuilder MaxString(string max)
        {
            this.Component.MaxString = max;
            return this;
        }

        public NumericBoxBuilder Step(decimal step)
        {
            this.Component.Step = step;
            return this;
        }

        public NumericBoxBuilder Scale(int scale)
        {
            this.Component.Scale = scale;
            return this;
        }

        public NumericBoxBuilder Format(string format)
        {
            this.Component.Format = format;
            return this;
        }

    }
}
