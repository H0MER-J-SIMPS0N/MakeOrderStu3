using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MakeOrderStu3.Models
{
    public sealed class OrderPosition
    {
        [JsonProperty("Id")]
        public string Id { get; private set; }
        [JsonProperty("NomenclaturePosition")]
        public Nomenclature NomenclaturePosition { get; set; }

        public OrderPosition()
        {

        }

        public OrderPosition(string id, Nomenclature nomenclaturePosition)
        {
            Id = id;
            NomenclaturePosition = nomenclaturePosition;
        }
        public OrderPosition(Nomenclature nomenclature)
        {
            Id = Guid.NewGuid().ToString();
            NomenclaturePosition = nomenclature;
        }

        public override string ToString()
        {
            return $"{Id} - {NomenclaturePosition}";
        }

    } 
}
