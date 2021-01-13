using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MakeOrderStu3.Models.EtaClasses
{
    public class DelayDuration
    {
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("unit")]
        public string Unit { get; set; }
    }
}
