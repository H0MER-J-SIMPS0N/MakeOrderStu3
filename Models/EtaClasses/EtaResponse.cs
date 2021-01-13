using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MakeOrderStu3.Models.EtaClasses
{
    internal sealed class EtaResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("time")]
        public int Time { get; set; }
        [JsonProperty("caption")]
        public string Caption { get; set; }
        [JsonProperty("delayDuration")]
        public DelayDuration DelayDuration { get; set; }
        [JsonProperty("effectivePeriod")]
        public EffectivePeriod EffectivePeriod { get; set; }
    }
}
