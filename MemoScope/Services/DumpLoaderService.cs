using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;

using MemoScope.Core;
using MemoScope.Modules.Explorer;

using Microsoft.Diagnostics.Runtime;

using WinFwk.UIMessages;
using WinFwk.UIServices;
using WinFwk.UITools.Log;

namespace MemoScope.Services
{
    public class DumpLoaderService : AbstractUIService, IMessageListener<OpenDumpRequest>
    {
        private readonly TaskFactory fact;

        public DumpLoaderService()
        {
            var sched = new LimitedConcurrencyLevelTaskScheduler(1);
            fact = new TaskFactory(sched);
        }

        public void HandleMessage(OpenDumpRequest openDumpRequest)
        {
            CancellationTokenSource source = new();
            var token = source.Token;
            foreach (string fileInfo in openDumpRequest.FileInfos.Where(fi => fi != null).Select(f => f.FullName))
            {
                fact.StartNew(() =>
                {
                    BeginTask("Loading file: " + fileInfo, source);
                    DataTarget target = null;
                    try
                    {
                        target = DataTarget.LoadCrashDump(fileInfo);

                        if ((Environment.Is64BitProcess && target.PointerSize != 8)
                        || (!Environment.Is64BitProcess && target.PointerSize != 4))
                        {
                            throw new InvalidOperationException($"Wrong architecture ! Dumpfile : {target.PointerSize * 8} bits, Environment.Is64BitProcess : {Environment.Is64BitProcess}");
                        }

                        var clrDump = new ClrDump(target, fileInfo, MessageBus);
                        clrDump.InitCache(token);
                        if (token.IsCancellationRequested)
                        {
                            clrDump.Destroy();
                            EndTask($"File NOT loaded: {fileInfo}");
                        }
                        else
                        {
                            MessageBus.SendMessage(new ClrDumpLoadedMessage(clrDump));
                            EndTask($"File loaded: {fileInfo}");
                        }
                    }
                    catch (Exception ex)
                    {
                        string msg = $"Failed to load dump file: {fileInfo}";
                        EndTask(msg);
                        MessageBus.Log(this, msg, ex);
                        target?.Dispose();
                        throw;
                    }
                });
            }
        }
    }
}
