using System;
using System.Net.Http;
using System.Threading;

namespace MakeOrderStu3.Singletones
{
    internal sealed class GetHttpClient
    {
        private static HttpClient httpClient;
        private static object syncRoot = new object();
        public static HttpClient Get()
        {
            if (httpClient is null)
            {
                try
                {
                    Monitor.TryEnter(syncRoot, TimeSpan.FromSeconds(2));
                    if (httpClient is null)
                    {
                        httpClient = new HttpClient { Timeout = new TimeSpan(0, 3, 0) };
                    }
                }
                catch
                {
                    httpClient = null;
                }
                finally
                {
                    Monitor.Exit(syncRoot);
                }
            }
            return httpClient;
        }
    }
}
