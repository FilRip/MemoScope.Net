using System;

using WinFwk.UIMessages;

namespace WinFwk.UITools.Log
{
    public enum LogLevelType { Debug, Info, Warn, Error, Exception, Notify }
    public class LogMessage(object sender, string text, LogLevelType logLevel = LogLevelType.Info) : AbstractUIMessage
    {
        public DateTime TimeStamp { get; private set; } = DateTime.Now;
        public LogLevelType LogLevel { get; private set; } = logLevel;
        public string Text { get; private set; } = text;
        public Exception Exception { get; private set; }
        public string LoggerName { get; private set; } = sender.GetType().Name;

        public LogMessage(object sender, string text, Exception exception) : this(sender, text, LogLevelType.Exception)
        {
            Exception = exception;
        }

        public override string ToString()
        {
            return $"Level: ${LogLevel}, Logger: ${LoggerName}, Text: '{Text}'";
        }
    }
}
