using Newtonsoft.Json;

namespace MakeOrderStu3.Models.QuestionnaireClasses
{
    public class QuestionnaireValueCoding
    {
        [JsonProperty("System")]
        public string System { get; set; }
        [JsonProperty("Code")]
        public string Code { get; set; }
        [JsonProperty("Display")]
        public string Display { get; set; }
    }
}
