using System;

namespace WinFwk.UICommands
{

    public class UIDataProviderAdapter<T>(Func<T> dataProvider) : IUIDataProvider<T>
    {
        private readonly Func<T> dataProvider = dataProvider;

        public T Data
        {
            get
            {
                return dataProvider();
            }
        }
    }
}
