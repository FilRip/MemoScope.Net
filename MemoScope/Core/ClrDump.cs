using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

using MemoScope.Core.Cache;
using MemoScope.Core.Dac;
using MemoScope.Core.Data;
using MemoScope.Core.ProcessInfo;

using Microsoft.Diagnostics.Runtime;

using NLog;

using WinFwk.UIMessages;
using WinFwk.UIModules;
using WinFwk.UITools.Log;

using ClrObject = MemoScope.Core.Data.ClrObject;

namespace MemoScope.Core
{
    public class ClrDump : IClrDump, IDisposable
    {
        static readonly Logger logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);

        private static int n = 0;
        public int Id { get; }
        public ClrRuntime Runtime { get; set; }
        public DataTarget Target { get; }
        public string DumpPath { get; }
        public ClrHeap Heap => Runtime.Heap;

        public MessageBus MessageBus { get; }
        public ClrDumpInfo ClrDumpInfo { get; }

        public IList<ClrSegment> Segments => Runtime.Heap.Segments;
        public List<ClrMemoryRegion> Regions()
        {
            return Runtime.EnumerateMemoryRegions().ToList();
        }
        public IList<ClrModule> Modules => Runtime.Modules;

        public List<ClrHandle> Handles()
        {
            return Runtime.EnumerateHandles().ToList();
        }
        public List<ulong> FinalizerQueueObjectAddresses()
        {
            return Runtime.EnumerateFinalizerQueueObjectAddresses().ToList();
        }
        public IEnumerable<IGrouping<ClrType, ulong>> FinalizerQueueObjectAddressesByType => Runtime.EnumerateFinalizerQueueObjectAddresses().GroupBy(address => GetObjectType(address));
        public IList<ClrThread> Threads => Runtime.Threads;
        public ClrThreadPool ThreadPool => Runtime.ThreadPool;
        public List<ClrType> AllTypes()
        {
            return Heap.EnumerateTypes().ToList();
        }

        public Dictionary<int, ThreadProperty> ThreadProperties
        {
            get
            {
                if (threadProperties == null)
                {
                    InitThreadProperties();
                }
                return threadProperties;
            }
        }

        public IEnumerable<ClrRoot> EnumerateClrRoots => Runtime.Heap.EnumerateRoots();

        Dictionary<int, ThreadProperty> threadProperties;
        private readonly SingleThreadWorker worker;
        private ClrDumpCache cache;

        public static int CurrentN()
        {
            return n++;
        }

        public ClrDump(DataTarget target, string dumpPath, MessageBus msgBus)
        {
            Id = CurrentN();
            Target = target;
            DumpPath = dumpPath;
            MessageBus = msgBus;
            worker = new SingleThreadWorker(dumpPath);
            worker.Run(InitRuntime, OnError);

            ClrDumpInfo = ClrDumpInfo.Load(dumpPath);
        }

        private void OnError(Exception ex)
        {
            MessageBus.Log(this, "Failed to initRuntime: " + DumpPath, ex);
        }

        public void InitCache(CancellationToken token)
        {
            cache = new ClrDumpCache(this);
            cache.Init(token);
        }

        private void InitRuntime()
        {
            MessageBus.Log(this, "InitRuntime: " + DumpPath);
            using var locator = DacFinderFactory.CreateDactFinder("DacSymbols");
            var clrVersion = Target.ClrVersions[0];
            var dacFile = locator.FindDac(clrVersion);
            Runtime = clrVersion.CreateRuntime(dacFile);
        }

        public List<ClrType> GetTypes()
        {
            List<ClrType> t = worker.Eval(() => t = AllTypes());
            return t;
        }

        internal void Destroy()
        {
            Dispose();
            cache.Destroy();
        }

        private bool disposeValue;
        public bool IsDisposed
        {
            get { return disposeValue; }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                MessageBus.Log(this, $"{nameof(Dispose)}: " + DumpPath);
                logger.Debug("Cache dispose");
                cache.Dispose();
                logger.Debug("Runtime.DataTarget.Dispose");
                Run(() => Runtime?.DataTarget?.Dispose());
                logger.Debug("Worker.Dispose");
                worker.Dispose();
                disposeValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public ClrType GetClrType(string typeName)
        {
            ClrType t = worker.Eval(() => t = Heap.EnumerateTypes().FirstOrDefault(clrType => clrType.Name == typeName));
            return t;
        }

        public List<ClrTypeStats> GetTypeStats()
        {
            var stats = cache.LoadTypeStat();
            return stats;
        }

        public List<ulong> GetInstances(ClrType type)
        {
            int typeId = cache.GetTypeId(type.Name);
            return GetInstances(typeId);
        }

        public List<ulong> GetInstances(int typeId)
        {
            var instances = cache.LoadInstances(typeId);
            return instances;
        }

        public IEnumerable<ulong> EnumerateInstances(ClrType type)
        {
            int typeId = cache.GetTypeId(type.Name);
            return cache.EnumerateInstances(typeId);
        }

        public int CountInstances(ClrType type)
        {
            int typeId = cache.GetTypeId(type.Name);
            return cache.CountInstances(typeId);
        }

        public ClrType GetType(ulong methodTable)
        {
            var type = Eval(() => Heap.GetTypeByMethodTable(methodTable));
            return type;
        }

        public ClrType GetType(string typeName)
        {
            var type = Eval(() => Heap.GetTypeByName(typeName));
            return type;
        }

        public bool IsString(ClrType type)
        {
            var res = Eval(() => type.IsString);
            return res;
        }
        public bool IsPrimitive(ClrType type)
        {
            var res = Eval(() => type.IsPrimitive);
            return res;
        }

        public T Eval<T>(Func<T> func)
        {
            if (worker.Active)
            {
                return worker.Eval(func);
            }
            else
            {
                throw new InvalidOperationException($"{Id}: can't run action because worker is not active !");
            }
        }

        public void Run(Action action)
        {
            if (worker.Active)
            {
                worker.Run(action);
            }
            else
            {
                throw new InvalidOperationException($"{Id}: can't run action because worker is not active !");
            }
        }

        public object GetSimpleValue(ulong address, ClrType type)
        {
            var obj = Eval(() => GetSimpleValueImpl(address, type));
            return obj;
        }

        private object GetSimpleValueImpl(ulong address, ClrType type)
        {
            if (SimpleValueHelper.IsSimpleValue(type))
            {
                var value = SimpleValueHelper.GetSimpleValue(address, type, false);
                return value;
            }

            return address;
        }

        public object GetFieldValue(ulong address, ClrType type, List<ClrInstanceField> fields)
        {
            var obj = Eval(() => GetFieldValueImpl(address, type, fields));
            return obj;
        }
        public object GetFieldValue(ulong address, ClrType type, List<string> fieldNames)
        {
            var obj = Eval(() => GetFieldValueImpl(address, type, fieldNames));
            return obj;
        }
        public object GetFieldValue(ulong address, ClrType type, ClrInstanceField field)
        {
            var obj = Eval(() => GetFieldValueImpl(address, type, field));
            return obj;
        }

        internal List<FieldInfo> GetFieldInfos(ClrType type)
        {
            List<FieldInfo> fieldNames = Eval(() => GetFieldNamesImpl(type));
            return fieldNames;
        }

        private List<FieldInfo> GetFieldNamesImpl(ClrType type)
        {
            var fieldNames = type.Fields.Select(f => new FieldInfo(f.Name, f.Type)).ToList();
            if (type.IsInterface || type.IsAbstract)
            {
                foreach (var someType in Heap.EnumerateTypes())
                {
                    if (type.IsInterface && someType.Interfaces.Any(interf => interf.Name == type.Name))
                    {
                        fieldNames.AddRange(GetFieldNamesImpl(someType));
                    }
                    if (type.IsAbstract && someType.BaseType == type)
                    {
                        fieldNames.AddRange(GetFieldNamesImpl(someType));
                    }
                }
            }
            return fieldNames.Distinct().ToList();
        }

        public object GetFieldValueImpl(ulong address, ClrType type, List<ClrInstanceField> fields)
        {
            ClrObject obj = new(address, type);

            for (int i = 0; i < fields.Count; i++)
            {
                var field = fields[i];
                obj = obj[field];
                if (obj.IsNull)
                {
                    return null;
                }
            }

            return obj.HasSimpleValue ? obj.SimpleValue : obj.Address;
        }
        public object GetFieldValueImpl(ulong address, ClrType type, List<string> fieldNames)
        {
            ClrObject obj = new(address, type);

            for (int i = 0; i < fieldNames.Count; i++)
            {
                var fieldName = fieldNames[i];
                ClrInstanceField field = obj.GetField(fieldName);
                if (field == null)
                {
                    return null;
                }

                obj = obj[field];
                if (obj.IsNull)
                {
                    return null;
                }
            }

            return obj.HasSimpleValue ? obj.SimpleValue : obj.Address;
        }

        public object GetFieldValueImpl(ulong address, ClrType type, ClrInstanceField field)
        {
            ClrObject obj = new(address, type);
            var fieldValue = obj[field];
            if (fieldValue.IsNull)
            {
                return null;
            }

            return fieldValue.HasSimpleValue ? fieldValue.SimpleValue : fieldValue.Address;
        }

        public IEnumerable<ulong> EnumerateReferers(ulong address)
        {
            return cache.EnumerateReferers(address);
        }

        public List<ulong> GetReferers(ulong address)
        {
            var referers = cache.LoadReferers(address);
            return referers;
        }

        public bool HasReferers(ulong address)
        {
            var hasReferers = cache.CountReferers(address) > 0;
            return hasReferers;
        }

        public int CountReferers(ulong address)
        {
            var count = cache.CountReferers(address);
            return count;
        }

        public string GetObjectTypeName(ulong address)
        {
            var name = worker.Eval(() =>
            {
                var clrType = Heap.GetObjectType(address);
                return clrType?.Name;
            });
            return name;
        }

        public ClrType GetObjectType(ulong address)
        {
            var clrType = worker.Eval(() => GetObjectTypeImpl(address));
            return clrType;
        }

        public ClrType GetObjectTypeImpl(ulong address)
        {
            var clrType = Heap.GetObjectType(address);
            return clrType;
        }

        private void InitThreadProperties()
        {
            ClrType threadType = GetClrType(typeof(Thread).FullName);
            var threadsInstances = GetInstances(threadType);
            var nameField = threadType.GetFieldByName("m_Name");
            var priorityField = threadType.GetFieldByName("m_Priority");
            var idField = threadType.GetFieldByName("m_ManagedThreadId");

            threadProperties = [];
            foreach (ulong threadAddress in threadsInstances)
            {
                string name = (string)GetFieldValue(threadAddress, threadType, nameField);
                int priority = (int)GetFieldValue(threadAddress, threadType, priorityField);
                int id = idField != null ? (int)GetFieldValue(threadAddress, threadType, idField) : 0;
                threadProperties[id] = new ThreadProperty
                {
                    Address = threadAddress,
                    ManagedId = id,
                    Priority = priority,
                    Name = name
                };
            }
        }

        public ulong ReadHeapPointer(ulong address)
        {
            Heap.ReadPointer(address, out ulong value);
            return value;
        }

        public ulong ReadRuntimePointer(ulong address)
        {
            Runtime.ReadPointer(address, out ulong value);
            return value;
        }

        public ClrMethod GetMethodByHandle(ulong methodDescriptorPtr)
        {
            var meth = Runtime.GetMethodByHandle(methodDescriptorPtr);
            return meth;
        }

        // Find the field in instance at address that references refAddress
        public string GetFieldNameReference(ulong refAddress, ulong address, bool prefixWithType = false)
        {
            return Eval(() => GetFieldNameReferenceImpl(refAddress, address, prefixWithType));
        }

        public string GetFieldNameReferenceImpl(ulong refAddress, ulong address, bool prefixWithType)
        {
            ClrType type = GetObjectTypeImpl(address);
            if (type == null)
            {
                return "Unknown";
            }
            string fieldName = "???";
            ClrObject obj = new(address, type);
            if (type.IsArray)
            {
                fieldName = "[ ? ]";
                var length = type.GetArrayLength(address);
                for (int i = 0; i < length; i++)
                {
                    if (obj[i].Address == refAddress)
                    {
                        fieldName = $"[ {i} ]";
                    }
                }
            }
            else
            {
                foreach (var field in type.Fields)
                {
                    switch (field.ElementType)
                    {
                        case ClrElementType.Struct:
                        case ClrElementType.String:
                        case ClrElementType.Array:
                        case ClrElementType.SZArray:
                        case ClrElementType.Object:
                            var fieldValue = obj[field];
                            if (fieldValue.Address == refAddress)
                            {
                                fieldName = field.Name;
                            }
                            break;
                    }
                }
            }
            if (prefixWithType)
            {
                fieldName = $"{fieldName}@{type.Name}";
            }

            return fieldName;
        }

#pragma warning disable CS0612 // Le type ou le membre est obsol�te
        public List<BlockingObject> GetBlockingObjects()
        {
            List<BlockingObject> blockingObjects = [];
            CancellationTokenSource source = new();
            var token = source.Token;
            MessageBus.BeginTask("Looking for blocking objects...", source);

            int currentN = 0;
            foreach (var obj in Runtime.Heap.EnumerateBlockingObjects())
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                currentN++;
                if (currentN % 512 == 0)
                {
                    MessageBus.Status($"Looking for blocking objects: {blockingObjects.Count:###,###,###,##0}");
                }
                blockingObjects.Add(obj);
            }
            if (token.IsCancellationRequested)
            {
                MessageBus.EndTask($"Blocking objects (cancelled): {blockingObjects.Count:###,###,###,##0} found.");
            }
            else
            {
                MessageBus.EndTask($"Blocking objects: {blockingObjects.Count:###,###,###,##0} found.");
            }
            return blockingObjects;
        }
#pragma warning restore CS0612 // Le type ou le membre est obsol�te

        public List<ClrRoot> GetClrRoots()
        {
            List<ClrRoot> clrRoots = [];
            CancellationTokenSource source = new();
            var token = source.Token;
            MessageBus.BeginTask("Looking for ClrRoots...", source);

            int currentN = 0;
            foreach (var obj in Runtime.Heap.EnumerateRoots())
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                currentN++;
                if (currentN % 512 == 0)
                {
                    MessageBus.Status($"Looking for ClrRoots: {clrRoots.Count:###,###,###,##0}");
                }
                clrRoots.Add(obj);
            }
            if (token.IsCancellationRequested)
            {
                MessageBus.EndTask($"ClrRoots (cancelled): {clrRoots.Count:###,###,###,##0} found.");
            }
            else
            {
                MessageBus.EndTask($"ClrRoots : {clrRoots.Count:###,###,###,##0} found.");
            }
            return clrRoots;
        }

        public bool HasField(ClrType clrType)
        {
            if (clrType.IsPrimitive)
            {
                return false;
            }
            if (clrType.Fields.Any())
            {
                return true;
            }

            if (clrType.IsInterface && clrType.Methods.Any(meth => meth.Name.StartsWith("get_")))
            {
                return true;
            }

            return false;
        }


    }

    public class ThreadProperty
    {
        public ulong Address { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public int ManagedId { get; set; }
    }

    public sealed class FieldInfo(string name, ClrType fieldType) : IEquatable<FieldInfo>, IEqualityComparer<FieldInfo>
    {
        public string Name { get; } = name;
        public ClrType FieldType { get; } = fieldType;

        public bool Equals(FieldInfo fieldInfo)
        {
            return fieldInfo.Name == Name && fieldInfo.FieldType.Name == FieldType.Name;
        }
        public override bool Equals(object obj)
        {
            return ((IEquatable<FieldInfo>)this).Equals((FieldInfo)obj);
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode() * 37 + FieldType.Name.GetHashCode();
        }

        public bool Equals(FieldInfo x, FieldInfo y)
        {
            if (x == null && y == null)
                return true;
            if (x == null)
                return false;
            return x.Equals(y);
        }

        public int GetHashCode(FieldInfo obj)
        {
            return obj.GetHashCode();
        }
    }
}