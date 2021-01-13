using Newtonsoft.Json;

namespace MakeOrderStu3.Models.PreanalyticsClasses
{
    public class AnalyticsResponseContainer
    {
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
