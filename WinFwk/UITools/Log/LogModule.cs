using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using BrightIdeasSoftware;

using NLog;
using NLog.Targets;

using WinFwk.UIMessages;
using WinFwk.UIModules;
using WinFwk.UITools.Settings;

namespace WinFwk.UITools.Log
{
    public partial class LogModule : UIModule
        , IMessageListener<LogMessage>
    {
        private readonly LogModel model = [];

        public LogModule()
        {
            InitializeComponent();
            colTimeStamp.AspectGetter = model.GetTimeStamp;
            colLogLevel.AspectGetter = model.GetLogLevel;
            colText.AspectGetter = model.GetText;
            colException.AspectGetter = model.GetException;
            dlvLogMessages.CellClick += OnCellClick;

            Icon = Properties.Resources.small_file_extension_log;
            Summary = "Logs";

            Icon appIcon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location);
            notifyIcon.Icon = appIcon;
            notifyIcon.Text = $"{Application.ProductName} ({Application.ProductVersion})";
            notifyIcon.Visible = true;
        }

        [UISchedulerAttribute]
        public void HandleMessage(LogMessage message)
        {
            Logger logger = LogManager.GetLogger(message.LoggerName);
            switch (message.LogLevel)
            {
                case LogLevelType.Debug:
                    logger.Debug(message.Text);
                    break;
                case LogLevelType.Info:
                    logger.Info(message.Text);
                    break;
                case LogLevelType.Warn:
                    logger.Warn(message.Text);
                    break;
                case LogLevelType.Error:
                    logger.Error(message.Text);
                    MessageBox.Show(message.Text, "Error !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case LogLevelType.Exception:
                    MessageBox.Show($"{message.Text}{Environment.NewLine}Exception: {message.Exception.Message}", "Exception !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    logger.Error(message.Exception, message.Text);
                    break;
                case LogLevelType.Notify:
                    logger.Info(message.Text);
                    notifyIcon.BalloonTipTitle = notifyIcon.Text;
                    notifyIcon.BalloonTipText = string.IsNullOrEmpty(message.Text) ? "Hello !" : message.Text;
                    notifyIcon.ShowBalloonTip(1000);
                    break;
                default:
#pragma warning disable S3928 // Parameter names used into ArgumentException constructors should match an existing one 
                    throw new ArgumentOutOfRangeException(nameof(message.LogLevel));
#pragma warning restore S3928 // Parameter names used into ArgumentException constructors should match an existing one 
            }
            model.Add(message);
            Summary = message.Text;
            dlvLogMessages.SetObjects(model);
        }

        private void OnCellClick(object sender, CellClickEventArgs e)
        {
            if (e.Item == null || e.Item.RowObject == null)
            {
                return;
            }

            LogMessage logMessage = model.GetObject(e.Item.RowObject);
            if (logMessage == null)
            {
                return;
            }

            switch (e.ClickCount)
            {
                case 1:
                    Summary = logMessage.Text;
                    break;
                case 2:
                    LogMessageViewerModule viewerModule = new() { UIModuleParent = this };
                    viewerModule.Init(logMessage);
                    RequestDockModule(viewerModule);
                    break;
            }

        }

        private void FdlvLogMessages_FormatCell(object sender, FormatCellEventArgs e)
        {
            if (e.Column != colLogLevel)
                return;

            LogLevelType logLevelType = (LogLevelType)e.SubItem.ModelValue;
            e.SubItem.BackColor = GetLogLevelColor(logLevelType);
            e.SubItem.ForeColor = GetLogLevelForeColor(logLevelType);
        }

        private static Color GetLogLevelColor(LogLevelType logLevel)
        {
            return logLevel switch
            {
                LogLevelType.Debug => UISettings.Instance.DebugColor,
                LogLevelType.Info => UISettings.Instance.InfoColor,
                LogLevelType.Warn => UISettings.Instance.WarnColor,
                LogLevelType.Error => UISettings.Instance.ErrorColor,
                LogLevelType.Exception => UISettings.Instance.ExceptionColor,
                LogLevelType.Notify => UISettings.Instance.NotifyColor,
                _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null),
            };
        }

        private static Color GetLogLevelForeColor(LogLevelType logLevel)
        {
            return logLevel switch
            {
                LogLevelType.Debug => UISettings.Instance.DebugForeColor,
                LogLevelType.Info => UISettings.Instance.InfoForeColor,
                LogLevelType.Warn => UISettings.Instance.WarnForeColor,
                LogLevelType.Error => UISettings.Instance.ErrorForeColor,
                LogLevelType.Exception => UISettings.Instance.ExceptionForeColor,
                LogLevelType.Notify => UISettings.Instance.NotifyForeColor,
                _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null),
            };
        }

        private void BtnOpenLogFile_Click(object sender, EventArgs e)
        {
            foreach (FileTarget fileTarget in LogManager.Configuration.AllTargets.OfType<FileTarget>())
            {
                LogEventInfo logEventInfo = new() { TimeStamp = DateTime.Now };
                string fileName = fileTarget.FileName.Render(logEventInfo);
                Process.Start(fileName);
            }
        }

        public override bool Closable()
        {
            return false;
        }
    }
}
