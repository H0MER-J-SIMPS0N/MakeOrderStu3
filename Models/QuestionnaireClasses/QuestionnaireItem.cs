using Newtonsoft.Json;
using System.Collections.Generic;

namespace MakeOrderStu3.Models.QuestionnaireClasses
{
    public class QuestionnaireItem
    {
        [JsonProperty("linkId")]
        public string LinkId { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("required")]
        public bool Required { get; set; }
        [JsonProperty("repeats")]
        public bool Repeats { get; set; }
        [JsonProperty("option")]
        public List<QuestionnaireOption> Options { get; set; }
        [JsonProperty("item")]
        public List<QuestionnaireItem> Items { get; set; }
    }
}
