using MakeOrderStu3.Models;
using System;
using System.Threading;

namespace MakeOrderStu3.Singletones
{
    internal sealed class GetToken
    {
        private static Token token;

        private static object syncRoot = new object();
        public static Token Get()
        {
            if (GetSettings.Get() is null)
            {
                token = null;
                return token;
            }
            else if (token is null)
            {
                try
                {
                    Monitor.TryEnter(syncRoot, TimeSpan.FromSeconds(2));
                    if (token is null)
                    {
                        token = new Token(GetSettings.Get());
                    }
                    else if (token.EndTime < DateTime.Now)
                    {
                        token = new Token(GetSettings.Get());
                    }
                }
                catch
                {
                    token = null;
                }
                finally
                {
                    Monitor.Exit(syncRoot);
                }
            }
            else if (token.StatusCode == "exception")
            {
                try
                {
                    Monitor.TryEnter(syncRoot, TimeSpan.FromSeconds(2));
                    token = new Token(GetSettings.Get());
                }
                catch
                {
                    token = null;
                }
                finally
                {
                    Monitor.Exit(syncRoot);
                }
            }
            else if (token.EndTime < DateTime.Now)
            {
                try
                {
                    Monitor.TryEnter(syncRoot, TimeSpan.FromSeconds(2));
                    token = new Token(GetSettings.Get());
                }
                catch
                {
                    token = null;
                }
                finally
                {
                    Monitor.Exit(syncRoot);
                }
            }
            return token;
        }
    }
}
