using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using NLog;

using WinFwk.UIMessages;
using WinFwk.UIModules;

namespace WinFwk.UITools.Workplace
{
    public partial class WorkplaceModule : UIModule,
        IMessageListener<SummaryChangedMessage>,
        IMessageListener<ModuleEventMessage>
    {
        private static readonly Logger logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);
        private readonly WorkplaceModel model;
        protected IEnumerable<UIModule> SelectedModules => tlvModules.CheckedObjects.OfType<UIModule>();

        public WorkplaceModule()
        {
            UIModuleParent = null;
            InitializeComponent();
            Name = "Workplace";
            Icon = Properties.Resources.globe_place;
            model = new WorkplaceModel();
            colName.AspectGetter = model.GetName;
            colName.ImageGetter = model.GetIcon;
            colSummary.AspectGetter = model.GetSummary;

            tlvModules.CanExpandGetter = model.HasChild;
            tlvModules.ChildrenGetter = model.GetChildren;
        }

        public void Init(MessageBus messageBus)
        {
            model.Init(messageBus);
        }

        public void HandleMessage(ModuleEventMessage message)
        {
            logger.Debug($"ModuleEventMessage: {message.ModuleEvent}, {message.Module.Name} / {message.Module.Summary}");
            switch (message.ModuleEvent)
            {
                case ModuleEventType.Added:
                    model.Add(message.Module);
                    break;
                case ModuleEventType.Removed:
                    tlvModules.UncheckObject(message.Module);
                    model.CloseModule(message.Module);
                    UIModule parent = message.Module.UIModuleParent;
                    if (parent != null)
                    {
                        tlvModules.RefreshObject(parent);
                    }

                    if (message.Module.UIModuleParent != null)
                    {
                        MessageBus.SendMessage(new ActivationRequest(message.Module.UIModuleParent));
                    }
                    break;
                case ModuleEventType.Activated:
                    break;
                default:
#pragma warning disable S3928 // Parameter names used into ArgumentException constructors should match an existing one 
                    throw new ArgumentOutOfRangeException(nameof(message.ModuleEvent));
#pragma warning restore S3928 // Parameter names used into ArgumentException constructors should match an existing one 
            }
            System.Collections.IList checkedObjects = tlvModules.CheckedObjects;
            tlvModules.ClearObjects();
            tlvModules.Roots = model.RootModules;
            tlvModules.CheckedObjects = checkedObjects;
            tlvModules.ExpandAll();
            tlvModules.SelectedObject = message.Module;
        }

        public void HandleMessage(SummaryChangedMessage message)
        {
            tlvModules.RefreshObject(message.Module);
        }

        private void TlvModules_SelectionChanged(object sender, EventArgs e)
        {
            if (tlvModules.SelectedObject is UIModule module)
            {
                MessageBus.SendMessage(new ActivationRequest(module));
            }
        }

        public override bool Closable()
        {
            return false;
        }

        private void CloseModulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (UIModule module in tlvModules.CheckedObjects.OfType<UIModule>())
            {
                MessageBus.SendMessage(new CloseRequest(module));
            }
        }
    }
}
