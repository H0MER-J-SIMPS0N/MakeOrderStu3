using MakeOrderStu3.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.IO;
using System.Threading;

namespace MakeOrderStu3.Singletones
{
    internal sealed class GetSettings
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly object syncRoot = new object();
        private static Setting settings;
        public static Setting Get()
        {
            if (settings is null)
            {
                string settingsText = string.Empty;
                try
                {
                    Monitor.TryEnter(syncRoot, TimeSpan.FromSeconds(2));
                    settingsText = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "settings.json"));
                }
                catch (Exception ex)
                {
                    logger.Error($"Не удалось прочитать файл настроек {Path.Combine(Directory.GetCurrentDirectory(), "settings.json")} по причине:\r\n" + ex.ToString());
                }
                finally
                {
                    Monitor.Exit(syncRoot);
                }
                if (!string.IsNullOrEmpty(settingsText))
                {
                    try
                    {
                        settings = JsonConvert.DeserializeObject<Setting>(settingsText);
                        logger.Info($"BaseUrl - {settings.BaseUrl}; TokenAddress - {settings.TokenAddress}");
                    }
                    catch (Exception ex) { logger.Error($"Не удалось десериализовать данные из файла настроек{Path.Combine(Directory.GetCurrentDirectory(), "settings.json")} по причине:\r\n" + ex.ToString()); }
                }
            }
            return settings;
        }
    }
}
