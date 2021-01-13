using Newtonsoft.Json;

namespace MakeOrderStu3.Models.PreanalyticsClasses
{
    public class AnalyticsResponseCollection
    {
        [JsonProperty("quantity")]
        public int Quantity { get; set; }
        [JsonProperty("method")]
        public string Method { get; set; }
    }
}
