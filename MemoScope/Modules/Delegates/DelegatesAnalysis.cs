﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using MemoScope.Core;
using MemoScope.Core.Data;

using Microsoft.Diagnostics.Runtime;

using WinFwk.UIModules;

using ClrObject = MemoScope.Core.Data.ClrObject;

namespace MemoScope.Modules.Delegates
{
    // Thanks to Alois Kraus for his article about delegate internals:
    // http://geekswithblogs.net/akraus1/archive/2012/05/20/149699.aspx
    public static class DelegatesAnalysis
    {
        const string TargetFieldName = "_target";
        const string InvocationCountFieldName = "_invocationCount";
        const string MethodPtrFieldName = "_methodPtr";
        const string InvocationListFieldName = "_invocationList";

        public static bool IsDelegateType(ClrDumpType clrDumpType)
        {
            var delegateType = clrDumpType.ClrDump.GetClrType(typeof(MulticastDelegate).FullName);
            return clrDumpType.BaseType != null && clrDumpType.BaseType.ClrType == delegateType;
        }

        public static List<ClrType> GetDelegateTypes(ClrDump clrDump)
        {
            CancellationTokenSource token = new();
            clrDump.MessageBus.BeginTask("Analyzing delegate types...", token);
            List<ClrType> delegates = [];
            var delegateType = clrDump.GetClrType(typeof(MulticastDelegate).FullName);

            foreach (var type in clrDump.AllTypes())
            {
                clrDump.MessageBus.Status($"Analyzing delegate type: {type.Name}");
                if (token.IsCancellationRequested)
                {
                    break;
                }

                if (type.BaseType != null && type.BaseType == delegateType)
                {
                    clrDump.MessageBus.Status($"Analyzing delegate type: counting instances for {type.Name}");
                    int nb = clrDump.CountInstances(type);
                    if (nb > 0)
                    {
                        delegates.Add(type);
                    }
                }
            }

            clrDump.MessageBus.EndTask("Delegate types analyzed.");
            return delegates.GroupBy(t => t.Name).Select(g => g.First()).ToList();
        }

        internal static IEnumerable<ClrObject> EnumerateHandlers(ClrObject clrObject)
        {
            ClrObject target = clrObject[TargetFieldName];
            ulong targetAddress = target.Address;

            if (targetAddress != clrObject.Address)
            {
                yield return clrObject;
            }
            else
            {

                var invocCount = clrObject[InvocationCountFieldName];
                var v = invocCount.SimpleValue;
                var invocCountValue = (long)v;
                var invocList = clrObject[InvocationListFieldName];
                if (!invocList.IsNull)
                {
                    for (int i = 0; i < invocCountValue; i++)
                    {
                        var targetObject = invocList[i];
                        yield return targetObject;
                    }
                }
            }
        }

        internal static List<DelegateTargetInformation> GetDelegateTargetInformations(ClrDumpObject clrDumpObject)
        {
            var targetInformations = new List<DelegateTargetInformation>();
            ClrObject clrObject = clrDumpObject.ClrObject;
            ClrDump clrDump = clrDumpObject.ClrDump;
            foreach (var handlerObject in EnumerateHandlers(clrObject))
            {
                var target = handlerObject[TargetFieldName];
                var methInfo = GetDelegateMethod(clrDump, handlerObject, target);
                var targetInfo = new DelegateTargetInformation(target.Address, new ClrDumpType(clrDump, target.Type), methInfo);
                targetInformations.Add(targetInfo);
            }

            return targetInformations;
        }

        private static ClrMethod GetDelegateMethod(ClrDump clrDump, ClrObject handler, ClrObject target)
        {
            _ = target.Type;
            var methPtrObj = handler[MethodPtrFieldName];
            var methPtrLong = (long)methPtrObj.SimpleValue;
            var methPtr = (ulong)methPtrLong;
            var methInfo = GetDelegateMethod(methPtr, clrDump);
            return methInfo;
        }

        // Thanks to Lee Culver and Jeff Cyr
        // for the code:
        // https://github.com/Microsoft/clrmd/issues/35
        public static ClrMethod GetDelegateMethod(ulong methodPtr, ClrDump clrDump)
        {
            ulong magicPtr = methodPtr + 5;
            ulong magicValue1 = clrDump.ReadHeapPointer(magicPtr + 1);
            ulong magicValue2 = clrDump.ReadHeapPointer(magicPtr + 2);

            ulong mysticPtr = magicPtr + 8 * (magicValue2 & 0xFF) + 3;
            ulong mysticOffset = 8 * (magicValue1 & 0xFF);

            ulong mysticValue = clrDump.ReadRuntimePointer(mysticPtr);
            ulong methodDescriptorPtr = mysticValue + mysticOffset;

            ClrMethod method = clrDump.GetMethodByHandle(methodDescriptorPtr);
            return method;
        }

        public static long CountTargets(ClrDump clrDump, ClrType clrType)
        {
            long count = 0;
            var targetField = clrType.GetFieldByName(TargetFieldName);
            var invocCountField = clrType.GetFieldByName(InvocationCountFieldName);

            foreach (ulong address in clrDump.EnumerateInstances(clrType))
            {
                count += CountTargets(address, clrType, targetField, invocCountField);
            }
            return count;
        }

        public static long CountTargets(ulong address, ClrType clrType, ClrInstanceField targetField, ClrInstanceField invocCountField)
        {
            ClrObject clrObject = new(address, clrType);
            ClrObject target = clrObject[targetField];
            if (target.Address != address)
            {
                return 1;
            }

            var invocCount = clrObject[invocCountField];
            var value = invocCount.SimpleValue;
            return (long)value;
        }

        internal static List<DelegateInstanceInformation> GetDelegateInstanceInformation(ClrDumpType clrDumpType)
        {
            var clrType = clrDumpType.ClrType;
            var delegateInstanceInformations = new List<DelegateInstanceInformation>();
            var targetField = clrType.GetFieldByName(TargetFieldName);
            var invocCountField = clrType.GetFieldByName(InvocationCountFieldName);

            foreach (var address in clrDumpType.EnumerateInstances())
            {
                var targets = CountTargets(address, clrType, targetField, invocCountField);
                var delegateInstanceInformation = new DelegateInstanceInformation(address, clrDumpType, targets);
                delegateInstanceInformations.Add(delegateInstanceInformation);
            }

            return delegateInstanceInformations;
        }

        internal static List<DelegateTypeInformation> GetDelegateTypeInformations(ClrDump clrDump)
        {
            var types = GetDelegateTypes(clrDump);
            var typeInformations = new List<DelegateTypeInformation>();
            foreach (var type in types)
            {
                var count = clrDump.CountInstances(type);
                var targets = CountTargets(clrDump, type);
                var delegateInformation = new DelegateTypeInformation(type, count, targets);
                typeInformations.Add(delegateInformation);
            }

            return typeInformations;
        }

        public static List<LoneTargetInformation> GetLoneTargetInformations(ClrDump clrDump)
        {
            CancellationTokenSource token = new();
            clrDump.MessageBus.BeginTask("Analyzing lone targets...", token);

            Dictionary<ClrObject, ClrObject> loneTargetAddresses = [];
            // For each instance of every delegate types 
            // let's find all the target objects
            // and select those with only referenced once
            var types = GetDelegateTypes(clrDump);
            foreach (var type in types)
            {
                clrDump.MessageBus.Status($"Analyzing delegate type: {type.Name}");
                if (token.IsCancellationRequested)
                {
                    break;
                }
                int n = 0;
                foreach (var address in clrDump.EnumerateInstances(type))
                {
                    if (n++ % 128 == 0)
                    {
                        clrDump.MessageBus.Status($"Analyzing delegate type: {type.Name}, instance #{n:###,###,###,##0}");
                    }
                    var handlerObject = new ClrObject(address, type);
                    foreach (var subHandlerObject in EnumerateHandlers(handlerObject))
                    {
                        if (token.IsCancellationRequested)
                        {
                            break;
                        }
                        var target = subHandlerObject[TargetFieldName];
                        int count = clrDump.CountReferers(target.Address);
                        if (count == 1)
                        {
                            loneTargetAddresses[target] = subHandlerObject;
                        }
                    }
                }
            }

            List<LoneTargetInformation> loneTargets = [];

            // foreach lone target, in its reference tree, we try to find the first 
            // object that is not a delegate type or an array of object (ie invocationList)
            var delegateType = clrDump.GetClrType(typeof(MulticastDelegate).FullName);
            var arrayObjType = clrDump.GetClrType(typeof(object[]).FullName);
            HashSet<ulong> visited = [];
            foreach (var kvp in loneTargetAddresses)
            {
                var loneTarget = kvp.Key;
                var handler = kvp.Value;
                var methInfo = GetDelegateMethod(clrDump, handler, loneTarget);
                visited.Clear();
                ulong ownerAddress = FindOwner(handler.Address, clrDump, delegateType, arrayObjType, visited);
                ClrObject owner = new(ownerAddress, clrDump.GetObjectType(ownerAddress));
                var loneTargetInformation = new LoneTargetInformation(loneTarget, methInfo, owner);
                loneTargets.Add(loneTargetInformation);
            }
            string status = token.IsCancellationRequested ? "cancelled" : "done";
            clrDump.MessageBus.EndTask($"Analyzing lone targets: {status}. Found: {loneTargets.Count}");
            return loneTargets;
        }

        public static ulong FindOwner(ulong address, ClrDump clrDump, ClrType delegateType, ClrType arrayObjType, HashSet<ulong> visited)
        {
            if (visited.Contains(address))
            {
                return 0;
            }

            var type = clrDump.GetObjectType(address);
            if (type == null)
            {
                return 0;
            }
            if (type != arrayObjType && (type.BaseType == null || type.BaseType != delegateType))
            {
                return address;
            }

            visited.Add(address);
            var refs = clrDump.GetReferers(address);

            foreach (var newAddress in refs)
            {
                var owner = FindOwner(newAddress, clrDump, delegateType, arrayObjType, visited);
                if (owner != 0)
                {
                    return owner;
                }
            }

            return 0;
        }
    }
}
