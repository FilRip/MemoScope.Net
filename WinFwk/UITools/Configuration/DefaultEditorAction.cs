using System;
using System.Drawing;

namespace WinFwk.UITools.Configuration
{
    public class DefaultEditorAction(string text, Image icon, Action action) : IEditorAction
    {
        private readonly Action Action = action;

        public string Text { get; } = text;
        public Image Icon { get; } = icon;

        public void DoWork()
        {
            Action();
        }
    }
}
