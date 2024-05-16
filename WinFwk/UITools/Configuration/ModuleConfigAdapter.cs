namespace WinFwk.UITools.Configuration
{
    public class ModuleConfigAdapter : IModuleConfig
    {
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
