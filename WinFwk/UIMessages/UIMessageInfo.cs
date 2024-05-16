using System;

using BrightIdeasSoftware;

namespace WinFwk.UIMessages
{
    public class UIMessageInfo(AbstractUIMessage message)
    {
        public AbstractUIMessage Message { get; } = message;

        [OLVColumn(AspectToStringFormat = "{0:HH:mm:ss.fff}")]
        public DateTime TimeStamp { get; } = DateTime.Now;

        [OLVColumn]
        public string Type => Message.GetType().Name;

        [OLVColumn(FillsFreeSpace = true)]
        public string Text => Message.ToString();
    }
}
