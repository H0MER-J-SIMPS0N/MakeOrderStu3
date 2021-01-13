using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MakeOrderStu3.Models
{
    public class SpecimenNomenclature
    {
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("specimen_code")]
        public long SpecimenCode { get; set; }
        [JsonProperty("specimen_name")]
        public string SpecimenName { get; set; }
        [JsonProperty("bodysite_code")]
        public string BodysiteCode { get; set; }
        [JsonProperty("bodysite_name")]
        public string BodysiteName { get; set; }
        [JsonProperty("container_type")]
        public string ContainerType { get; set; }
        [JsonProperty("container_name")]
        public string ContainerName { get; set; }

        public override string ToString()
        {
            return $"description: {Description}\n" +
                   $"specimen_code: {SpecimenCode}\n" +
                   $"specimen_name: {SpecimenName}\n" +
                   $"bodysite_code: {BodysiteCode}\n" +
                   $"bodysite_name: {BodysiteName}\n" +
                   $"container_type: {ContainerType}\n" +
                   $"container_name: {ContainerName}";
        }        
    }
}
