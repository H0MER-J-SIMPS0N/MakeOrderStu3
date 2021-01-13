using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MakeOrderStu3.Models.PreanalyticsClasses
{
    public sealed class AnalyticsRequest
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("guid")]
        public string Guid { get; set; }
        [JsonProperty("specimen_code")]
        public long SpecimenCode { get; set; }
        [JsonProperty("bodycite_code")]
        public string BodyciteCode { get; set; }
        [JsonProperty("container_type")]
        public string ContainerType { get; set; }
    }
}
