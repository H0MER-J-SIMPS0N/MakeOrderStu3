using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MakeOrderStu3.Models.PreanalyticsClasses
{
    public class AnalyticsResponseSpecimen
    {
        [JsonIgnore]
        public long Label { get; set; }
        [JsonProperty("code")]
        public long Code { get; set; }
        [JsonProperty("collection")]
        public AnalyticsResponseCollection Collection { get; set; }
        [JsonProperty("processing")]
        public List<AnalyticsResponseProcessing> Processing { get; set; }
        [JsonProperty("container")]
        public List<AnalyticsResponseContainer> Container { get; set; }
        [JsonProperty("guids")]
        public List<string> Guids { get; set; }
        [JsonProperty("supportingInfo")]
        public string SupportingInfo { get; set; }
    }
}
