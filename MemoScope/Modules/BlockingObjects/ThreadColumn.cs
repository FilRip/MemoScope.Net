using System.Windows.Forms;

using BrightIdeasSoftware;

using MemoScope.Core;

namespace MemoScope.Modules.BlockingObjects
{
    internal class ThreadColumn : OLVColumn
    {
        private readonly ThreadProperty thread;

        public ThreadColumn(ThreadProperty thread)
        {
            this.thread = thread;
            ImageGetter = GetData;
            Name = $"THREAD_{thread.ManagedId} - {thread.Name}";
            Text = $"{thread.ManagedId} - {thread.Name}";
            ToolTipText = Text;
            TextAlign = HorizontalAlignment.Center;
        }

        private object GetData(object rowObject)
        {
            if (rowObject is not BlockingObjectInformation blockingObjectInfo)
            {
                return null;
            }
            if (blockingObjectInfo.OwnersId.Contains(thread.ManagedId))
            {
                return Properties.Resources._lock_small;
            }
            if (blockingObjectInfo.WaitersId.Contains(thread.ManagedId))
            {
                return Properties.Resources.hourglass;
            }
            return null;
        }
    }
}