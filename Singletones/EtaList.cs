using MakeOrderStu3.Models.EtaClasses;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MakeOrderStu3.Singletones
{
    internal sealed class EtaList
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static List<EtaResponse> eta;
        private static string Contract { get; set; }
        private static readonly object syncRoot = new object();
        public static List<EtaResponse> Get(string contract)
        {
            if (Contract is null || Contract != contract)
            {
                try
                {
                    Monitor.TryEnter(syncRoot, TimeSpan.FromSeconds(2));
                    SetEta(contract);
                    Contract = contract;
                }
                catch (Exception ex)
                {
                    logger.Error($"{ex}");
                    eta = null;
                }
                finally
                {
                    Monitor.Exit(syncRoot);
                }
            }
            return eta;
        }

        static void SetEta(string contract)
        {
            string address = new Uri(new Uri(GetSettings.Get().BaseUrl), $"/eta?contract={contract}").ToString();
            logger.Debug(address);
            try
            {
                GetHttpClient.Get().DefaultRequestHeaders.Clear();
                GetHttpClient.Get().DefaultRequestHeaders.TryAddWithoutValidation("Authorization", GetToken.Get().Value);
                using (var response = GetHttpClient.Get().GetAsync(address).Result)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string resp = response.Content.ReadAsStringAsync().Result;
                        eta = JsonConvert.DeserializeObject<List<EtaResponse>>(resp);
                        logger.Info($"Got eta by contract {contract}, eta is null == {eta is null}");
                    }
                    else
                    {
                        throw new Exception(response.Content.ReadAsStringAsync().Result + "\n\n" + response.ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error($"Try to get ETA: {ex}");
            }
        }
    }
}
