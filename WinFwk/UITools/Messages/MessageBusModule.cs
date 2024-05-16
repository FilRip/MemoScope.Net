using System;
using System.Linq;

using BrightIdeasSoftware;

using WinFwk.UIMessages;
using WinFwk.UIModules;

namespace WinFwk.UITools.Messages
{
    public partial class MessageBusModule : UIModule
    {
        public MessageBusModule()
        {
            InitializeComponent();
        }

        public override void Init()
        {
            Name = "MessageBus";
            Icon = Properties.Resources.small_chart_organisation;
            dtlvMessageTypes.InitData<AbstractMessageInformation>();
            dtlvMessageTypes.SelectedIndexChanged += OnSelectedIndexChanged;
        }

        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (dtlvMessageTypes.SelectedObject is SubscriberInformation subscriberInfo)
            {
                pgObject.SelectedObject = subscriberInfo.Instance;
            }
        }

        public override void PostInit()
        {
            System.Collections.Generic.IEnumerable<MessageTypeInformation> types = MessageBus.GetMessageTypes().Select(testc => new MessageTypeInformation(testc, MessageBus));
            dtlvMessageTypes.SetObjects(types);
        }
    }

    public abstract class AbstractMessageInformation(Type type) : TreeNodeInformationAdapter<AbstractMessageInformation>
    {
        Type Type { get; set; } = type;

        [OLVColumn]
        public string Name => Type.Name;
    }

    public class MessageTypeInformation : AbstractMessageInformation
    {
        public MessageTypeInformation(Type type, MessageBus bus) : base(type)
        {
            Children = bus.GetSubscribers(type).Select(sub => new SubscriberInformation(sub)).OfType<AbstractMessageInformation>().ToList();
        }

        public override bool CanExpand => Children.Any();
    }

    public class SubscriberInformation(object instance) : AbstractMessageInformation(instance.GetType())
    {
        public object Instance { get; } = instance;
    }
}
