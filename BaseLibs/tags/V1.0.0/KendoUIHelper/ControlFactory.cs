using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace KendoUIHelper
{
    public class ControlFactory
    {
        public IHtmlHelper HtmlHelper { get; set; }
        public ControlFactory(IHtmlHelper helper)
        {
            this.HtmlHelper = helper;
        }

        public static TBuilder CreateBuilder<TControl, TBuilder> () 
            where TControl : ControlBase
            where TBuilder : ControlBuilderBase<TControl, TBuilder>
        {
            return (TBuilder)Activator.CreateInstance(typeof (TBuilder),new object[] { (TControl)Activator.CreateInstance(typeof(TControl)) });
        }
        public DatePickerBuilder DatePicker()
        {
            return new DatePickerBuilder(new DatePicker());
        }

        
        public DateTimePickerBuilder DateTimePicker()
        {
            return new DateTimePickerBuilder(new DateTimePicker());
        }

        public TimePickerBuilder TimePicker()
        {
            return new TimePickerBuilder(new TimePicker());
        }

        public AutoCompleteBuilder AutoComplete()
        {
            return new AutoCompleteBuilder(new AutoComplete());
        }

        public DropDownListBuilder DropDownList()
        {
            return new DropDownListBuilder(new DropDownList());
        }

        public ComboBoxBuilder ComboBox()
        {
            return new ComboBoxBuilder(new ComboBox());
        }

        public MultiColumnComboBoxBuilder MultiColumnComboBox()
        {
            return new MultiColumnComboBoxBuilder(new MultiColumnComboBox());
        }

        public MultiSelectBuilder MultiSelect()
        {
            return new MultiSelectBuilder(new MultiSelect());
        }

        public NumericBoxBuilder NumericBox()
        {
            return new NumericBoxBuilder(new NumericBox());
        }

        public TextBoxBuilder TextBox()
        {
            return new TextBoxBuilder(new TextBox());
        }
        public TextAreaBuilder TextArea()
        {
            return new TextAreaBuilder(new TextArea());
        }
    }
}
