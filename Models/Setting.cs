using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MakeOrderStu3.Models
{
    public class Setting
    {
        #region Fields and Properties
        [JsonProperty("TokenAddress")]
        public string TokenAddress { get; private set; }

        [JsonProperty("Data")]
        public Dictionary<string, string> Data { get; private set; }

        [JsonProperty("BaseUrl")]
        public string BaseUrl { get; private set; }
        #endregion

        #region Constructor
        public Setting(string tokenAddress, Dictionary<string, string> data, string baseUrl)
        {
            TokenAddress = tokenAddress;
            Data = data;
            BaseUrl = baseUrl;
        }
        #endregion
    }
}
