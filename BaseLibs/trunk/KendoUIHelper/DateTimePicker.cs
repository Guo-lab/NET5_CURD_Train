using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace KendoUIHelper
{
    public class DateTimePicker:TimePickerBase
    {
        public DateTimePicker():base("input","kendo-date-time-picker")
        { }
        public override string Format { get; set; } = "yyyy-MM-dd HH:mm:ss";
    }
}
