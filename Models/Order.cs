using Hl7.Fhir.Model;
using MakeOrderStu3.Models.PreanalyticsClasses;
using MakeOrderStu3.Models.QuestionnaireClasses;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace MakeOrderStu3.Models
{
    public sealed class Order
    {
        public string ContractCode { get; set; }
        
        public List<OrderPosition> OrderPositions { get; private set; }

        public List<SpecimenNomenclature> SelectedSpecimens { get; set; }

        private List<SpecimenNomenclature> allSelectedSpecimens = new List<SpecimenNomenclature>();
        public List<SpecimenNomenclature> AllSelectedSpecimens 
        {
            get 
            {
                allSelectedSpecimens.Clear();
                for (int i = 0; i < OrderPositions.Count; i++)
                {
                    allSelectedSpecimens.AddRange(OrderPositions[i].NomenclaturePosition.RequiredSpecimen);
                }
                allSelectedSpecimens.AddRange(SelectedSpecimens);
                return allSelectedSpecimens;
            }
        }

        public PreanalyticsRequest PreanalyticsRequest { get; set; }
        public AnalyticsResponse AnalyticsResponse { get; set; }
        public QuestionnaireLocal Questionnaire { get; set; }
        public List<QuestionnaireResponse.ItemComponent> GroupResultItems { get; set; }
        public string PatientUrl { get; set; }

        public Order() {}
        public Order(List<Nomenclature> nomenclatures, string contractCode)
        {
            OrderPositions = nomenclatures.Select(x => new OrderPosition(x)).ToList();
            ContractCode = contractCode;
        }


        public override string ToString()
        {
            return JsonConvert.SerializeObject(OrderPositions, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
        }

    }
}



