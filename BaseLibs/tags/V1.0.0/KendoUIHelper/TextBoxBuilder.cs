namespace KendoUIHelper
{
    public class TextBoxBuilder:ControlBuilderBase<TextBox,TextBoxBuilder>
        
    {
        public TextBoxBuilder(TextBox Component):base(Component)
        {

        }

        public TextBoxBuilder SubmitOnEnter(string onEnter) {
            this.Component.SubmitOnEnter = onEnter;
            return this;
        }
    }
}
