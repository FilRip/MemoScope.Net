using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WinFwk.UIMessages
{
    public class MessageBus
    {
        private readonly Dictionary<Type, List<object>> dicoSubscribers = [];
        private TaskScheduler uiScheduler;
        private TaskFactory taskFactory;

        public event Action<Exception, object> ExceptionRaised;
        public event Action<UIMessageInfo> MessageSent;

        public TaskScheduler UiScheduler
        {
            get { return uiScheduler; }
            set
            {
                uiScheduler = value;
                taskFactory = new TaskFactory(value);
            }
        }

        public IEnumerable<Type> GetMessageTypes() => dicoSubscribers.Keys;

        public static IEnumerable<Type> GetMessageTypes(object subscriber)
        {
            List<Type> types = WinFwkHelper.GetGenericInterfaceArguments(subscriber, typeof(IMessageListener<>));
            return types;
        }

        public List<object> GetSubscribers(Type msgType)
        {
            if (!dicoSubscribers.TryGetValue(msgType, out List<object> subscribers))
            {
                subscribers = [];
                dicoSubscribers.Add(msgType, subscribers);
            }

            return subscribers;
        }

        public void Subscribe(object subscriber)
        {
            if (subscriber == null)
            {
                return;
            }

            IEnumerable<Type> messageTypes = GetMessageTypes(subscriber);
            foreach (Type msgType in messageTypes)
            {
                List<object> subscribers = GetSubscribers(msgType);
                if (!subscribers.Contains(subscriber))
                {
                    subscribers.Add(subscriber);
                }
            }
        }

        public void Unsubscribe(object subscriber)
        {
            if (subscriber == null)
            {
                return;
            }

            IEnumerable<Type> messageTypes = GetMessageTypes(subscriber);
            foreach (Type msgType in messageTypes)
            {
                List<object> subscribers = GetSubscribers(msgType);
                if (subscribers.Contains(subscriber))
                {
                    subscribers.Remove(subscriber);
                }
            }
        }

        public void SendMessage<T>(T message) where T : AbstractUIMessage
        {
            MessageSent?.Invoke(new UIMessageInfo(message));

            List<object> subscribers = GetSubscribers(typeof(T));
            if (subscribers.Count == 0)
            {
                return;
            }

            Type interfaceType = typeof(IMessageListener<T>);
            foreach (object subscriber in subscribers)
            {
                if (subscriber is not IMessageListener<T> sub)
                {
                    // this should not happen !
                    continue;
                }

                Type subscriberType = subscriber.GetType();
                InterfaceMapping map = subscriberType.GetInterfaceMap(interfaceType);
                bool sched = false;
                foreach (MethodInfo m in map.TargetMethods)
                {
                    IEnumerable<Attribute> x = m.GetCustomAttributes(typeof(UISchedulerAttribute));
                    sched = x.Any();
                }

                try
                {
                    if (sched)
                    {
                        taskFactory.StartNew(() =>
                        {
                            sub.HandleMessage(message);
                        });
                    }
                    else
                    {
                        sub.HandleMessage(message);
                    }
                }
                catch (Exception e)
                {
                    ExceptionRaised?.Invoke(e, sub);
                }
            }
        }
    }
}