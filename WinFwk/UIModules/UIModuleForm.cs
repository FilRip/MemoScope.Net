using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

using BrightIdeasSoftware;

using NLog;

using WeifenLuo.WinFormsUI.Docking;

using WinFwk.UICommands;
using WinFwk.UIMessages;
using WinFwk.UITools.Settings;
using WinFwk.UITools.ToolBar;

namespace WinFwk.UIModules
{
    public partial class UIModuleForm : Form
        , IMessageListener<DockRequest>
        , IMessageListener<StatusMessage>
        , IMessageListener<ActivationRequest>
        , IMessageListener<UISettingsChangedMessage>
        , IMessageListener<CloseRequest>
        , IUICommandRequestor
    {
        static readonly Logger logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);
        private readonly Dictionary<DockContent, UIModule> dicoModules = [];
        protected readonly MessageBus msgBus = new();
        private readonly List<UIToolBarSettings> toolbarSettings = [];
        private readonly Dictionary<Keys, AbstractUICommand> dicoKeys = [];
        private CancellationTokenSource cancellationTokenSource;
        private int nbTasks;
        IEnumerable<AbstractUICommand> commands;
        protected Dictionary<Type, Action<Control, UISettings>> dicoSkinActions = [];
        public StatusStrip StatusBar => statusStrip;
        private readonly object _lockThis = new();

        protected UIModuleForm()
        {
            InitializeComponent();

            msgBus.UiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            msgBus.Subscribe(this);
            mainPanel.ContentAdded += OnContentAdded;
            mainPanel.ContentRemoved += OnContentRemoved;

            toolbarSettings.Add(UIToolBarSettings.Main);
            toolbarSettings.Add(UIToolBarSettings.Help);

            RegisterSkinAction(typeof(Chart), ApplyColorsChart);
            RegisterSkinAction(typeof(ObjectListView), ApplyColorsListView);
            RegisterSkinAction(typeof(UICommandButton), ApplyColorsUICommandButton);
            RegisterSkinAction(typeof(RoundCornerGroupBox), ApplyColorsRoundCornerGroupBox);
            RegisterSkinAction(typeof(PropertyGrid), ApplyColorsPropertyGrid);
        }

        [UISchedulerAttribute]
        void IMessageListener<DockRequest>.HandleMessage(DockRequest message)
        {
            DockModule(message.UIModule, message.DockState);
        }

        [UISchedulerAttribute]
        void IMessageListener<StatusMessage>.HandleMessage(StatusMessage message)
        {
            lock (_lockThis)
            {
                tsslStatusMessage.Text = message.Text;
                switch (message.Status)
                {
                    case StatusType.BeginTask:
                        nbTasks++;
                        tspbProgressBar.Style = ProgressBarStyle.Marquee;
                        tspbProgressBar.MarqueeAnimationSpeed = 30;
                        tspbProgressBar.Visible = true;
                        cancellationTokenSource = message.CancellationTokenSource;
                        tssbCancel.Visible = cancellationTokenSource != null;
                        break;
                    case StatusType.EndTask:
                        nbTasks--;
                        if (nbTasks <= 0)
                        {
                            tspbProgressBar.Style = ProgressBarStyle.Continuous;
                            tspbProgressBar.MarqueeAnimationSpeed = 0;
                            tspbProgressBar.Visible = false;
                            tssbCancel.Visible = false;
                        }
                        break;
                }
            }
        }

        private void SendModuleEventMessage(DockContent content, ModuleEventType moduleEvent)
        {
            if (dicoModules.TryGetValue(content, out UIModule module))
            {
                msgBus.SendMessage(new ModuleEventMessage(module, moduleEvent));
            }
        }

        private void OnActiveContentChanged(object sender, EventArgs e)
        {
            if (mainPanel.ActiveContent is not DockContent dockContent)
            {
                return;
            }

            SendModuleEventMessage(dockContent, ModuleEventType.Activated);
        }

        private void OnContentRemoved(object sender, DockContentEventArgs e)
        {
            if (e.Content is not DockContent dockContent)
            {
                return;
            }

            logger.Debug($"OnContentRemoved: {dockContent.TabText}");
            SendModuleEventMessage(dockContent, ModuleEventType.Removed);
            dicoModules.Remove(dockContent);
        }

        private void OnContentAdded(object sender, DockContentEventArgs e)
        {
            if (e.Content is not DockContent dockContent)
            {
                return;
            }
            SendModuleEventMessage(dockContent, ModuleEventType.Added);
        }

        [UISchedulerAttribute]
        public void HandleMessage(ActivationRequest message)
        {
            foreach (KeyValuePair<DockContent, UIModule> kvp in dicoModules)
            {
                if (kvp.Value == message.Module)
                {
                    kvp.Key.DockHandler.Activate();
                }
            }
        }

        public virtual void HandleMessage(UISettingsChangedMessage message)
        {
            ApplyColors(this, message.UiSettings);
            ApplyColors(mainPanel, message.UiSettings);
        }

        [UISchedulerAttribute]
        protected DockContent DockModule(UIModule uiModule, DockState dockState = DockState.Document, bool allowclose = true)
        {
            ApplyColors(uiModule, UISettings.Instance);

            DockContent content = BuildContent(uiModule, allowclose);
            content.Show(mainPanel, dockState);
            return content;
        }

        [UISchedulerAttribute]
#pragma warning disable IDE0060 // Supprimer le paramètre inutilisé
        protected DockContent DockModule(UIModule uiModule, DockContent parentContent, DockAlignment dockAlignment = DockAlignment.Top, bool allowclose = true)
#pragma warning restore IDE0060 // Supprimer le paramètre inutilisé
        {
            ApplyColors(uiModule, UISettings.Instance);

            DockContent content = BuildContent(uiModule, allowclose);
            content.Show(parentContent.Pane, DockAlignment.Top, 0.5);
            return content;
        }

        private DockContent BuildContent(UIModule uiModule, bool allowclose)
        {
            uiModule.InitBus(msgBus);
            DockContent content = UIModuleHelper.BuildDockContent(uiModule, allowclose);
            if (uiModule.Icon != null)
            {
                content.Icon = System.Drawing.Icon.FromHandle(uiModule.Icon.GetHicon());
            }
            else
            {
                content.Icon = null;
                content.ShowIcon = false;
            }
            dicoModules[content] = uiModule;
            return content;
        }

        protected void InitToolBars()
        {
            mainPanel.DockTopPortion = 120;
            this.msgBus.SendMessage(new UICommandRequest(this));
            // Link keyboard shortcuts to commands
            if (commands == null)
            {
                return;
            }
            foreach (AbstractUICommand command in commands)
            {
                if (command.Shortcut != Keys.None)
                {
                    dicoKeys[command.Shortcut] = command;
                }
            }

            // Group commands by toolbar name
            Dictionary<string, IGrouping<string, AbstractUICommand>> dicoCommands = commands.GroupBy(command => command.Group).ToDictionary(commandGroup => commandGroup.Key);

            // Create a default ui settings for toolbar missing some settings
            Dictionary<string, UIToolBarSettings> dicoSettings = toolbarSettings.ToDictionary(setting => setting.Name);
            foreach (string commandGroup in dicoCommands.Select(kvp => kvp.Key))
            {
                if (!dicoSettings.ContainsKey(commandGroup))
                {
                    // Create default settings for toolbar
                    dicoSettings.Add(commandGroup, new UIToolBarSettings(commandGroup, 0, null));
                }
            }

            // Order the toolbars by priority 
            DockContent firstToolbar = null;
            foreach (UIToolBarSettings setting in dicoSettings.Values.OrderBy(setting => setting.Priority))
            {
                if (dicoCommands.TryGetValue(setting.Name, out IGrouping<string, AbstractUICommand> commandGroup))
                {
                    UIToolbar toolbar = new();
                    toolbar.InitBus(msgBus);
                    toolbar.Init(setting.Icon, commandGroup);
                    DockContent content = DockModule(toolbar, setting.DockState, false);
                    if (setting.MainToolbar)
                    {
                        firstToolbar = content;
                    }
                }
            }

            firstToolbar?.DockHandler.Activate();
        }

        private void ApplyTheme(DockPanel mainPanel, UISettings uiSettings)
        {
            switch (uiSettings.Theme)
            {
                case Themes.VS2003:
                    mainPanel.Theme = new VS2003Theme();
                    break;
                case Themes.VS2005:
                    mainPanel.Theme = new VS2005Theme();
                    break;
                case Themes.VS2012Blue:
                    mainPanel.Theme = new VS2012BlueTheme();
                    break;
                case Themes.VS2012Dark:
                    mainPanel.Theme = new VS2012DarkTheme();
                    break;
                case Themes.VS2013Dark:
                    mainPanel.Theme = new VS2013DarkTheme();
                    break;
                case Themes.VS2013Light:
                    mainPanel.Theme = new VS2013LightTheme();
                    break;
                case Themes.VS2015Blue:
                    mainPanel.Theme = new VS2015BlueTheme();
                    break;
                case Themes.VS2015Dark:
                    mainPanel.Theme = new VS2015DarkTheme();
                    break;
                case Themes.VS2015Light:
                    mainPanel.Theme = new VS2015LightTheme();
                    break;
            }
        }

        private static void ApplyColors(DockPanel dockPanel, UISettings uiSettings)
        {
            if (uiSettings == null)
            {
                return;
            }

            DockPanelSkin skin = dockPanel.Theme.Skin;
            if (skin == null)
            {
                return;
            }

            DockPaneStripSkin dpStrip = skin.DockPaneStripSkin;

            DockPaneStripGradient docGrad = dpStrip.DocumentGradient;
            docGrad.ActiveTabGradient = uiSettings.ActiveTabGradient.TabGradient;
            docGrad.DockStripGradient = uiSettings.DockStripGradient.TabGradient;
            docGrad.InactiveTabGradient = uiSettings.InactiveTabGradient.TabGradient;

            DockPaneStripToolWindowGradient toolGrad = dpStrip.ToolWindowGradient;
            toolGrad.ActiveTabGradient = uiSettings.ActiveTabGradient.TabGradient;
            toolGrad.DockStripGradient = uiSettings.DockStripGradient.TabGradient;
            toolGrad.InactiveTabGradient = uiSettings.InactiveTabGradient.TabGradient;

            toolGrad.ActiveCaptionGradient = uiSettings.ActiveCaptionGradient.TabGradient;
            toolGrad.InactiveCaptionGradient = uiSettings.InactiveCaptionGradient.TabGradient;
            dockPanel.Refresh();
        }

        public void RegisterSkinAction(Type type, Action<Control, UISettings> action)
        {
            dicoSkinActions[type] = action;
        }

        protected void ApplyColors(Control control, UISettings uiSettings)
        {
            Color backgroundColor = uiSettings.BackgroundColor;
            Color foregroundColor = uiSettings.ForegroundColor;
            if (backgroundColor.IsEmpty || foregroundColor.IsEmpty || backgroundColor.A == 0 || foregroundColor.A == 0)
            {
                return;
            }
            control.BackColor = backgroundColor;
            control.ForeColor = foregroundColor;

            Type controlType = control.GetType();
            while (controlType != typeof(object))
            {
                if (dicoSkinActions.TryGetValue(controlType, out Action<Control, UISettings> skinAction))
                {
                    skinAction(control, uiSettings);
                }
                controlType = controlType.BaseType;
            }
            foreach (Control child in control.Controls)
            {
                ApplyColors(child, uiSettings);
            }

            control.Refresh();
        }

        private static void ApplyColorsChart(Control control, UISettings uiSettings)
        {
            foreach (ChartArea area in ((Chart)control).ChartAreas)
            {
                area.BackColor = uiSettings.BackgroundColor;
            }
        }

        private static void ApplyColorsListView(Control control, UISettings uiSettings)
        {
            if (control is not ObjectListView objListView)
            {
                return;
            }
            // Alternate row
            objListView.UseAlternatingBackColors = uiSettings.UseAlternateRowColor;
            objListView.AlternateRowBackColor = uiSettings.AlternateRowColor;

            // Header
            objListView.HeaderFormatStyle = new HeaderFormatStyle();
            objListView.HeaderFormatStyle.SetBackColor(uiSettings.HeaderBackColor);
            objListView.HeaderFormatStyle.SetForeColor(uiSettings.HeaderForeColor);

            // Selected row
            objListView.SelectedBackColor = uiSettings.SelectedRowBackgroundColor;
            objListView.UnfocusedSelectedBackColor = uiSettings.SelectedRowBackgroundColor;
            objListView.SelectedForeColor = uiSettings.SelectedRowForegroundColor;
            objListView.UnfocusedSelectedForeColor = uiSettings.SelectedRowForegroundColor;
            objListView.OwnerDraw = true;
        }

        private void ApplyColorsUICommandButton(Control control, UISettings uiSetings)
        {
            if (control is not UICommandButton button)
            {
                return;
            }

            button.DisabledTextColor = uiSetings.DisabledTextColor;
        }

        private void ApplyColorsRoundCornerGroupBox(Control control, UISettings uiSetings)
        {
            if (control is not RoundCornerGroupBox groupBox)
            {
                return;
            }

            groupBox.TitleBackColor = uiSetings.TitleBackColor;
            groupBox.TitleForeColor = uiSetings.TitleForeColor;
        }

        private void ApplyColorsPropertyGrid(Control control, UISettings uiSetings)
        {
            if (control is not PropertyGrid propertyGrid)
            {
                return;
            }

            propertyGrid.LineColor = uiSetings.LineColor;
            propertyGrid.SelectedItemWithFocusBackColor = uiSetings.SelectedItemColor;
            propertyGrid.SelectedItemWithFocusForeColor = uiSetings.SelectedItemTextColor;
        }

        private void UIModuleForm_Load(object sender, EventArgs e)
        {
            ApplyTheme(mainPanel, UISettings.Instance);
            Text = string.Format("{0} {1} ({2})", Application.ProductName, Application.ProductVersion, Environment.Is64BitProcess ? "x64" : "x86");
            ApplyColors(mainPanel, UISettings.Instance);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (dicoKeys.TryGetValue(keyData, out AbstractUICommand command))
            {
                command.Run();
            }

            return false;
        }

        public void InitModuleFactory()
        {
            UIModuleFactory.Init(this.msgBus, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void TssbCancel_ButtonClick(object sender, EventArgs e)
        {
            cancellationTokenSource?.Cancel();
        }

        protected void AddToolBar(string name, int priority, Bitmap icon, DockState dockState = DockState.DockTop, bool mainToolbar = false)
        {
            toolbarSettings.Add(new UIToolBarSettings(name, priority, icon, dockState, mainToolbar));
        }

        [UISchedulerAttribute]
        void IMessageListener<CloseRequest>.HandleMessage(CloseRequest message)
        {
            logger.Info($"CloseRequest: {message.Module.Name} / {message.Module.Summary}");
            foreach (KeyValuePair<DockContent, UIModule> kvp in dicoModules)
            {
                UIModule module = kvp.Value;
                if (module == message.Module && module.Closable())
                {
                    logger.Info($"Close: {module.Name} / {module.Summary}");
                    kvp.Key.Close();
                    kvp.Key.DockHandler.Dispose();
                    break;
                }
            }
        }

        public void Accept(IEnumerable<AbstractUICommand> commands)
        {
            this.commands = commands;
        }
    }
}

