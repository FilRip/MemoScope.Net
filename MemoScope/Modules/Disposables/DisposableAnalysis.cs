﻿using System.Collections.Generic;
using System.Threading;

using MemoScope.Core;

using WinFwk.UIModules;

namespace MemoScope.Modules.Disposables
{
    public static class DisposableAnalysis
    {
        internal static List<DisposableTypeInformation> GetDisposableTypeInformations(ClrDump clrDump)
        {
            CancellationTokenSource token = new();
            clrDump.MessageBus.BeginTask("Analyzing IDisposable types...", token);

            List<DisposableTypeInformation> result = [];

            foreach (var type in clrDump.AllTypes())
            {
                clrDump.MessageBus.Status($"Analyzing type: {type.Name}");
                if (token.IsCancellationRequested)
                {
                    clrDump.MessageBus.EndTask("Analyzing IDisposable types: cancelled.");
                    return result;
                }

                foreach (var interf in type.Interfaces)
                {
                    if (interf.Name == typeof(System.IDisposable).FullName)
                    {
                        clrDump.MessageBus.Status($"Analyzing IDisposable type: counting instances for {type.Name}");
                        int nb = clrDump.CountInstances(type);
                        result.Add(new DisposableTypeInformation(type, nb));
                    }
                }
            }
            clrDump.MessageBus.EndTask("IDisposable types analyzed.");
            return result;
        }
    }
}
