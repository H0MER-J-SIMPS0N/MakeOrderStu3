using Hl7.Fhir.Model;
using MakeOrderStu3.Models;
using MakeOrderStu3.Models.PreanalyticsClasses;
using MakeOrderStu3.Models.QuestionnaireClasses;
using System;
using System.Collections.Generic;

namespace MakeOrderStu3.Singletones
{
    internal sealed class GetOrder
    {
        private static Order order;
        public static Order Get()
        {
            if (order is null)
            {
                order = new Order();
            }
            return order;
        }

        public static Order Get(List<Nomenclature> nomenclatures, string contractCode)
        {
            return order = new Order(nomenclatures, contractCode);
        }

        public static void SetResource(List<SpecimenNomenclature> specimenNomenclatures)
        {
            try
            {
                order.SelectedSpecimens = specimenNomenclatures;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex}");
            }
        }

        public static void AddResource(List<SpecimenNomenclature> specimenNomenclatures)
        {
            try
            {
                order.SelectedSpecimens.AddRange(specimenNomenclatures);
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex}");
            }
        }

        public static void AddResource(SpecimenNomenclature specimenNomenclature)
        {
            try
            {
                order.SelectedSpecimens.Add(specimenNomenclature);
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex}");
            }
        }
        
        public static void AddResource(PreanalyticsRequest preanalyticsRequest)
        {
            try
            {
                order.PreanalyticsRequest = preanalyticsRequest;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex}");
            }
        }

        public static void AddResource(AnalyticsResponse analyticsResponse)
        {
            try
            {
                order.AnalyticsResponse = analyticsResponse;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex}");
            }
        }
        public static void AddResource(Patient patient)
        {
            try
            {
                order.PatientUrl = GetFromService.PatientUrl(patient);
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex}");
            }
        }

        public static void AddResource(QuestionnaireLocal questionnaire)
        {
            try
            {
                order.Questionnaire = questionnaire;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex}");
            }
        }

        public static void AddResource(List<QuestionnaireResponse.ItemComponent> groupResultItems)
        {
            try
            {
                order.GroupResultItems = groupResultItems;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex}");
            }
        }
    }
}
