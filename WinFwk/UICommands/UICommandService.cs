using System;
using System.Collections.Generic;

using WinFwk.UIMessages;
using WinFwk.UIServices;

namespace WinFwk.UICommands
{
    public class UICommandService : AbstractUIService,
        IMessageListener<UICommandRequest>
    {
        List<AbstractUICommand> commands;
        public override void Init(MessageBus messageBus)
        {
            base.Init(messageBus);
            // Get all the commands and instanciate them
            commands = [];
            List<Type> types = WinFwkHelper.GetDerivedTypes(typeof(AbstractUICommand));
            foreach (Type type in types)
            {
                System.Reflection.ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                if (constructor?.Invoke(null) is AbstractUICommand command)
                {
                    command.InitBus(this.MessageBus);
                    commands.Add(command);
                }
            }
        }

        public void HandleMessage(UICommandRequest message)
        {
            message.Requestor.Accept(commands);
        }
    }
}
