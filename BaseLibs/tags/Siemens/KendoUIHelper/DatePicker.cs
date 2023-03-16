using System;
using System.Collections.Generic;
using System.Linq;



namespace KendoUIHelper
{
    public class DatePicker:DatePickerBase
    {
        public DatePicker():base("input","kendo-date-picker")
        { }
        public override string Format { get; set; } = "yyyy-MM-dd";
    }
}
