using System.Collections.Generic;
using System.Runtime.Remoting.Channels.Ipc;

namespace MemoScope.Helpers
{
    internal class EasyHookKeeper
    {
        private static EasyHookKeeper _instance;
        private Dictionary<int, IpcServerChannel> _listHook;

        internal static EasyHookKeeper GetInstance()
        {
            if (_instance == null)
            {
                _instance = new EasyHookKeeper();
                _instance.Init();
            }
            return _instance;
        }

        private void Init()
        {
            _listHook = [];
        }

        internal IpcServerChannel GetProcessChannel(int processId)
        {
            if (_listHook.ContainsKey(processId))
                return _listHook[processId];
            return null;
        }

        internal void SetProcessChannel(int processId, IpcServerChannel channel)
        {
            if (_listHook.ContainsKey(processId))
                _listHook.Remove(processId);
            _listHook.Add(processId, channel);
        }
    }
}
