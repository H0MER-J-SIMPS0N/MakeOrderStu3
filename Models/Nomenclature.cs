using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using ReactiveUI;

namespace MakeOrderStu3.Models
{
    [Serializable]
    public class Nomenclature: ReactiveObject
    {
        [JsonIgnore]
        private string id;
        [JsonProperty("id")]
        public string Id
        {
            get => id;
            set => this.RaiseAndSetIfChanged(ref id, value);
        }
        [JsonIgnore]
        private string caption;
        [JsonProperty("caption")]
        public string Caption
        {
            get => caption;
            set => this.RaiseAndSetIfChanged(ref caption, value);
        }
        [JsonIgnore]
        private bool allowMultipleItems;
        [JsonProperty("allow_multiple_items")]
        public bool AllowMultipleItems
        {
            get => allowMultipleItems;
            set => this.RaiseAndSetIfChanged(ref allowMultipleItems, value);
        }
        [JsonIgnore]
        private string labId;
        [JsonProperty("lab_id")]
        public string LabId
        {
            get => labId;
            set => this.RaiseAndSetIfChanged(ref labId, value);
        }
        [JsonIgnore]
        private string labCaption;
        [JsonProperty("lab_caption")]
        public string LabCaption
        {
            get => labCaption;
            set => this.RaiseAndSetIfChanged(ref labCaption, value);
        }
        [JsonIgnore]
        private string group;
        [JsonProperty("group")]
        public string Group
        {
            get => group;
            set => this.RaiseAndSetIfChanged(ref group, value);
        }
        [JsonIgnore]
        private string description;
        [JsonProperty("description")]
        public string Description
        {
            get => description;
            set => this.RaiseAndSetIfChanged(ref description, value);
        }
        [JsonIgnore]
        private List<string> patientPreparation;
        [JsonProperty("patient_preparation")]
        public List<string> PatientPreparation
        {
            get => patientPreparation;
            set => this.RaiseAndSetIfChanged(ref patientPreparation, value);
        }
        [JsonIgnore]
        private bool multipleSpecimen;
        [JsonProperty("multiple_specimen")]
        public bool MultipleSpecimen
        {
            get => multipleSpecimen;
            set => this.RaiseAndSetIfChanged(ref multipleSpecimen, value);
        }
        private List<SpecimenNomenclature> specimen;
        [JsonProperty("specimen")]
        public List<SpecimenNomenclature> Specimen
        {
            get => specimen;
            set => this.RaiseAndSetIfChanged(ref specimen, value);
        }
        [JsonIgnore]
        private List<SpecimenNomenclature> requiredSpecimen;
        [JsonProperty("required_specimen")]
        public List<SpecimenNomenclature> RequiredSpecimen
        {
            get => requiredSpecimen;
            set => this.RaiseAndSetIfChanged(ref requiredSpecimen, value);
        }
        [JsonIgnore]
        private double price;
        [JsonProperty("price")]
        public double Price
        {
            get => price;
            set => this.RaiseAndSetIfChanged(ref price, value);
        }

        [JsonIgnore]
        private List<SpecimenNomenclature> selectedSpecimens;
        [JsonIgnore]
        public List<SpecimenNomenclature> SelectedSpecimens
        {
            get => selectedSpecimens;
            set => this.RaiseAndSetIfChanged(ref selectedSpecimens, value);
        }     
        
        public Nomenclature()
        {
            SelectedSpecimens = new List<SpecimenNomenclature>();
        }

        public override string ToString() => $"{LabId} ({Id}) - {Caption}";
        
    }
}
