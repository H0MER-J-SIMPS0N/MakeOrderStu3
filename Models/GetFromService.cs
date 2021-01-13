using Hl7.Fhir.Model;
using MakeOrderStu3.Singletones;
using Newtonsoft.Json;
using NLog;
using System;
using System.Net.Http;
using System.Text;

namespace MakeOrderStu3.Models
{
    public static class GetFromService
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public static string PreanaliticsResult(string bodyForPeanalytics)
        {
            string result;
            string address = new Uri(new Uri(GetSettings.Get().BaseUrl), @"/preanalytics").ToString();
            logger.Info($"Адрес для получения преаналитики: {address}");
            string postData = bodyForPeanalytics;
            logger.Info($"PostData:\r\n{postData}");
            logger.Info($"Token:\r\n{GetToken.Get().Value}");
            try
            {
                GetHttpClient.Get().DefaultRequestHeaders.Clear();
                GetHttpClient.Get().DefaultRequestHeaders.TryAddWithoutValidation("Authorization", GetToken.Get().Value);
                var content = new StringContent(postData, Encoding.UTF8, "application/json");
                using (HttpResponseMessage response = GetHttpClient.Get().PostAsync(address, content).Result)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        result = response.Content.ReadAsStringAsync().Result;
                    }
                    else
                    {
                        throw new Exception(response.Content.ReadAsStringAsync().Result + "\n\n" + response.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                result = "Не удалось выполнить запрос преаналитики по причине:\r\n" + ex.ToString();
            }
            return result;
        }

        public static string PatientUrl(Patient patient)
        {
            string result;
            try
            {
                string url = new Uri(new Uri(GetSettings.Get().BaseUrl), "fhir/Patient/").ToString();
                GetHttpClient.Get().DefaultRequestHeaders.Clear();
                var content = new StringContent(JsonConvert.SerializeObject(patient, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }), Encoding.UTF8, "application/json");
                using (var response = GetHttpClient.Get().PostAsync(url, content).Result)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string answer = response.Content.ReadAsStringAsync().Result;
                        result = url + JsonConvert.DeserializeObject<Patient>(answer).Id;
                        return result;
                    }
                    else
                    {
                        throw new Exception(response.Content.ReadAsStringAsync().Result + "\n\n" + response.ToString());
                    }                        
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Попытка получить пациента по ссылке: {ex}");
                throw new Exception(ex.ToString());
            }
        }

        public static string ResultBundle(string bundleToSend)
        {
            string result;
            string address = new Uri(new Uri(GetSettings.Get().BaseUrl), @"/fhir/$CreateDiagnosticRequest").ToString();
            logger.Info($"Адрес для отправки Bundle: {address}");
            string postData = bundleToSend;
            logger.Info($"PostData:\r\n{postData}");
            logger.Info($"Token:\r\n{GetToken.Get().Value}");
            try
            {
                GetHttpClient.Get().DefaultRequestHeaders.Clear();
                GetHttpClient.Get().DefaultRequestHeaders.TryAddWithoutValidation("Authorization", GetToken.Get().Value);
                var content = new StringContent(postData, Encoding.UTF8, "application/json");
                using (HttpResponseMessage response = GetHttpClient.Get().PostAsync(address, content).Result)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        result = response.Content.ReadAsStringAsync().Result;
                    }
                    else
                    {
                        throw new Exception(response.Content.ReadAsStringAsync().Result + "\n\n" + response.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                result = "Не удалось выполнить отправку заказа по причине:\r\n" + ex.ToString();
            }
            return result;
        }

        public static string Questionnaire(string bodyForPeanalytics)
        {
            string result;
            string address = new Uri(new Uri(GetSettings.Get().BaseUrl), @"/questionnaire").ToString();
            logger.Info($"Адрес для получения преаналитики: {address}");
            string postData = bodyForPeanalytics;
            logger.Info($"PostData:\r\n{postData}");
            logger.Info($"Token:\r\n{GetToken.Get().Value}");
            try
            {
                GetHttpClient.Get().DefaultRequestHeaders.Clear();
                GetHttpClient.Get().DefaultRequestHeaders.TryAddWithoutValidation("Authorization", GetToken.Get().Value);
                var content = new StringContent(postData, Encoding.UTF8, "application/json");
                using (HttpResponseMessage response = GetHttpClient.Get().PostAsync(address, content).Result)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        result = response.Content.ReadAsStringAsync().Result;
                    }
                    else
                    {
                        throw new Exception(response.Content.ReadAsStringAsync().Result + "\n\n" + response.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                result = "Не удалось выполнить запрос опросника по причине:\r\n" + ex.ToString();
            }
            return result;
        }
    }
}
