namespace KendoUIHelper
{
    public class ComboBoxBuilderBase<TControl,TBuilder>:SuggestionListBuilder<TControl,TBuilder>
        where TControl : ComboBox
         where TBuilder : ControlBuilderBase<TControl, TBuilder>

    {
        public ComboBoxBuilderBase(TControl Component):base(Component)
        {

        }

        public TBuilder AutoBind(bool autoBind)
        {
            this.Component.AutoBind = autoBind;
            return (this as TBuilder);
        }
        public TBuilder Text(string text)
        {
            this.Component.Text = text;
            return (this as TBuilder);
        }
    }
}
