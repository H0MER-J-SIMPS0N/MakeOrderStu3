using Newtonsoft.Json;

namespace MakeOrderStu3.Models.QuestionnaireClasses
{
    public class QuestionnaireOption
    {
        [JsonProperty("valueCoding")]
        public QuestionnaireValueCoding ValueCoding { get; set; }
    }
}
