using System.Threading;

using WinFwk.UIMessages;

namespace WinFwk.UIModules
{
    public enum StatusType { BeginTask, EndTask, Text }
    public class StatusMessage(string text, StatusType status = StatusType.Text) : AbstractUIMessage
    {
        public StatusType Status { get; } = status;
        public string Text { get; } = text;
        public CancellationTokenSource CancellationTokenSource { get; } = null;

        public StatusMessage(string text, CancellationTokenSource cancellationTokenSource) : this(text, StatusType.BeginTask)
        {
            CancellationTokenSource = cancellationTokenSource;
        }

        public override string ToString()
        {
            return $"Status: {Status}, Text: '{Text}'";
        }
    }

    public static class StatusMessageHelper
    {
        public static void Status(this MessageBus msgBus, string text, StatusType status = StatusType.Text)
        {
            msgBus.SendMessage(new StatusMessage(text, status));
        }
        public static void BeginTask(this MessageBus msgBus, string text, CancellationTokenSource cancellationTokenSource = null)
        {
            msgBus.SendMessage(new StatusMessage(text, cancellationTokenSource));
        }
        public static void EndTask(this MessageBus msgBus, string text)
        {
            Status(msgBus, text, StatusType.EndTask);
        }
    }
}
