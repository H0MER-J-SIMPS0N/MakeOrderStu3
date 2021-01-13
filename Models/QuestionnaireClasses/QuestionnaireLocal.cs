using Newtonsoft.Json;

namespace MakeOrderStu3.Models.QuestionnaireClasses
{
    public class QuestionnaireLocal
    {
        [JsonProperty("resourceType")]
        public string ResourceType { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("item")]
        public QuestionnaireItem Item { get; set; }
    }
}
