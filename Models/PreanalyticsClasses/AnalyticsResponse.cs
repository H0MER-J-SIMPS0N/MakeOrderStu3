using Newtonsoft.Json;

namespace MakeOrderStu3.Models.PreanalyticsClasses
{
    public class AnalyticsResponse
    {
        [JsonProperty("specimens")]
        public AnalyticsResponseSpecimen[] Specimens { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
        }
    }
}
