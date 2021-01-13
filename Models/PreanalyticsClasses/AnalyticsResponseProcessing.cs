using Newtonsoft.Json;

namespace MakeOrderStu3.Models.PreanalyticsClasses
{
    public class AnalyticsResponseProcessing
    {
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
