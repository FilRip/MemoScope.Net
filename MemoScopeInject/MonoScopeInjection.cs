using System;
using System.Threading;

using EasyHook;

namespace MemoScopeInject
{
    public class MonoScopeInjection
    {
        private readonly InterfaceExchange _interface;

#pragma warning disable IDE0060 // Supprimer le paramètre inutilisé
        public MonoScopeInjection(RemoteHooking.IContext context, string channelName)
#pragma warning restore IDE0060 // Supprimer le paramètre inutilisé
        {
            _interface = RemoteHooking.IpcConnectClient<InterfaceExchange>(channelName);
        }

#pragma warning disable IDE0060 // Supprimer le paramètre inutilisé
        public void Run(RemoteHooking.IContext context, string channelName)
#pragma warning restore IDE0060 // Supprimer le paramètre inutilisé
        {
            while (true)
            {
                try
                {
                    _interface.Ping();
                    Thread.Sleep(10);
                }
                catch (Exception)
                {
                    break;
                }
            }
        }
    }
}
