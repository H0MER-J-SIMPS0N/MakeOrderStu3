using ReactiveUI;

namespace MakeOrderStu3.Models.QuestionnaireClasses
{
    public class QuestionnaireAnswer: ReactiveObject
    {
        public QuestionnaireItem Item { get; private set; }
        public string LinkId { get; private set; }
        public string Text { get; private set; }

        public string Watermark 
        { 
            get
            {
                if (Item?.Type == "integer")
                {
                    return "целое число";
                }
                else if (Item?.Type == "boolean")
                {
                    return "Y или N";
                }
                else if (Item?.Type == "date")
                {
                    return "дата в виде dd-MM-yyyy";
                }
                else if (Item?.Type == "decimal")
                {
                    return "дробное число с десятичной точкой";
                }
                else if (Item?.Type == "choice")
                {                    
                    return string.Empty;
                }
                return "не пустая строка";
            }
        }

        private string validateColor;
        public string ValidateColor
        {
            get => validateColor;
            set
            {
                this.RaiseAndSetIfChanged(ref validateColor, value);
            }
        }

        private object resultValue;
        public object ResultValue
        {
            get => resultValue;
            set
            {
                this.RaiseAndSetIfChanged(ref resultValue, value);
                ValidateColor = Validate.Answer(resultValue, Item.Type) ? "LightGreen" : Item.Required ? "Red" : "Yellow";
            }
        }

        public QuestionnaireAnswer() { }

        public QuestionnaireAnswer(QuestionnaireItem item, string linkId, string text)
        {
            Item = item;
            Text = text;
            LinkId = linkId;
            ValidateColor = Validate.Answer(resultValue, Item.Type) ? "LightGreen" : Item.Required ? "Red" : "Yellow";
        }

        
    }
}
