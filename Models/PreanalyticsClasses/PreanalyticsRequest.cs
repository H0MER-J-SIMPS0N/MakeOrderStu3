using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MakeOrderStu3.Models.PreanalyticsClasses
{
    public sealed class PreanalyticsRequest
    {
        [JsonProperty("contract")]
        public string ContractCode { get; set; }
        [JsonProperty("includetransportcontainer")]
        public string IncludeTransportContainer { get; set; }
        [JsonProperty("analyticsrequests")]
        public List<AnalyticsRequest> AnalyticsRequests { get; set; }
    }
}
