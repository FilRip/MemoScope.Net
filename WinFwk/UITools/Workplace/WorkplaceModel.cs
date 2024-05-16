using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using NLog;

using WinFwk.UIMessages;
using WinFwk.UIModules;

namespace WinFwk.UITools.Workplace
{
    public class WorkplaceModel
    {
        static readonly Logger logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);
        internal Dictionary<UIModule, List<UIModule>> modules = [];
        public List<UIModule> RootModules { get; } = [];
        private MessageBus MessageBus { get; set; }

        public void Init(MessageBus messageBus)
        {
            MessageBus = messageBus;
        }

        public void Add(UIModule uiModule)
        {
            if (uiModule.UIModuleParent == null)
            {
                return;
            }
            if (uiModule.UIModuleParent == uiModule)
            {
                RootModules.Add(uiModule);
            }
            else
            {
                if (!modules.TryGetValue(uiModule.UIModuleParent, out List<UIModule> children))
                {
                    children = [];
                    modules[uiModule.UIModuleParent] = children;
                }
                children.Add(uiModule);

                modules[uiModule] = [];
            }
        }

        public string GetName(object rowObject)
        {
            return ((UIModule)rowObject).Name;
        }

        public string GetSummary(object rowObject)
        {
            return ((UIModule)rowObject).Summary;
        }

        public object GetIcon(object rowObject)
        {
            return ((UIModule)rowObject).Icon;
        }

        public bool HasChild(object o)
        {
            return modules.ContainsKey((UIModule)o);
        }

        public List<UIModule> GetChildren(object o)
        {
            if (o is not UIModule uiModule)
            {
#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
                return null;
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null
            }
            modules.TryGetValue(uiModule, out List<UIModule> children);
            int count = children == null ? 0 : children.Count;
            logger.Debug($"{nameof(GetChildren)}: parent: {uiModule.Name}, children: {count}");
            return children;
        }

        public void Remove(UIModule module)
        {
            logger.Debug($"{nameof(Remove)}: {module.Name}");
            if (RootModules.Remove(module))
            {
                logger.Debug($"{nameof(Remove)}: removed root module {module.Name}");
            }

            if (modules.ContainsKey(module))
            {
                bool b = modules.Remove(module);
                logger.Debug($"{nameof(Remove)}: removed module {module.Name}: {b}");
            }


            List<UIModule> parentChildren = GetChildren(module.UIModuleParent);
            if (parentChildren != null && parentChildren.Any())
            {
                parentChildren.Remove(module);
            }
        }

        public void CloseModule(UIModule module)
        {
            List<UIModule> children = GetChildren(module);
            int count = children == null ? 0 : children.Count;
            logger.Debug($"{nameof(CloseModule)}: {module.Name}, children: {count}");
            if (children != null && children.Any())
            {
                foreach (UIModule child in children)
                {
                    logger.Debug($"RequestCloseModule: Child={child.Name}");
                    MessageBus.SendMessage(new CloseRequest(child));
                }
            }

            logger.Debug($"{nameof(CloseModule)}: Remove/Close {module.Name}");
            Remove(module);
            module.Close();
            MessageBus.SendMessage(new CloseRequest(module));
        }
    }
}