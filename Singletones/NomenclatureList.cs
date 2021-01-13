using MakeOrderStu3.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MakeOrderStu3.Singletones
{
    internal sealed class NomenclatureList
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static List<Nomenclature> Nomenclatures;
        private static string Contract { get; set; }
        private static object syncRoot = new object();
        public static List<Nomenclature> Get(string contract)
        {
            if (Contract is null || Contract != contract)
            {
                try
                {
                    Monitor.TryEnter(syncRoot, TimeSpan.FromSeconds(2));
                    SetNomenclature(contract);
                    Contract = contract;
                    EtaList.Get(contract);
                }
                catch (Exception ex)
                {
                    logger.Error($"{ex}");
                    throw new Exception($"{ex}");
                }
                finally
                {
                    Monitor.Exit(syncRoot);
                }
            }            
            return Nomenclatures;
        }

        static void SetNomenclature(string contract)
        {
            string address = new Uri(new Uri(GetSettings.Get().BaseUrl), $"/nomenclature?contract={contract}").ToString();
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
                        Nomenclatures = JsonConvert.DeserializeObject<List<Nomenclature>>(resp);
                        logger.Info($"Got nomenclature by contract {contract}, nomenclature is null == {Nomenclatures is null}");
                    }
                    else
                    {
                        throw new Exception(response.Content.ReadAsStringAsync().Result + "\n\n" + response.ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error($"Try to get NOMENCLATURE STU3");
                throw new Exception($"{ex}");
            }
        }
    }
}
