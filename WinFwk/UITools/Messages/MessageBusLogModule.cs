using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using BrightIdeasSoftware;

using WinFwk.UIMessages;
using WinFwk.UIModules;

namespace WinFwk.UITools.Messages
{
    public partial class MessageBusLogModule : UIModule
    {
        readonly BufferedSource<UIMessageInfo> dataSource;
        readonly TaskFactory taskFactory;
        public MessageBusLogModule()
        {
            InitializeComponent();
            dataSource = new BufferedSource<UIMessageInfo>(dlvMessages);
            dlvMessages.InitColumns<UIMessageInfo>();
            dlvMessages.VirtualListDataSource = dataSource;
            taskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());
        }

        public override void Init()
        {
            Name = "MessageBusLog";
            Icon = Properties.Resources.small_raw_access_logs;
            dlvMessages.SelectedIndexChanged += OnSelectedIndexChanged;
            MessageBus.MessageSent += OnMessageSent;
        }

        private void OnMessageSent(UIMessageInfo msgInfo)
        {
            dataSource.AddObject(msgInfo);
            taskFactory.StartNew(() => dlvMessages.UpdateVirtualListSize());
        }

        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (dlvMessages.SelectedObject is UIMessageInfo msgInfo)
            {
                pgObject.SelectedObject = msgInfo.Message;
            }
        }
    }

    public class BufferedSource<T>(VirtualObjectListView listView) : AbstractVirtualListDataSource(listView)
    {
        private List<T> Buffer { get; set; } = [];

        public override object GetNthObject(int n)
        {
            return Buffer[n];
        }

        public override int GetObjectCount()
        {
            return Buffer.Count;
        }

        internal void AddObject(T obj)
        {
            Buffer.Insert(0, obj);
            while (Buffer.Count > 2000)
            {
                Buffer.RemoveAt(Buffer.Count - 1);
            }
        }
    }
}
