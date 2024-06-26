﻿using System.Drawing;
using System.Drawing.Drawing2D;

namespace WinFwk.UITools.Settings.Skins
{
    public class DefaultSkin : AbstractSkin
    {
        public override string Name => "__Default__";

        public override void Apply(UISettings settings)
        {
            settings.AlternateRowColor = SystemColors.Window;
            settings.BackgroundColor = SystemColors.Window;
            settings.ForegroundColor = SystemColors.WindowText;

            settings.HeaderBackColor = SystemColors.Menu;
            settings.HeaderForeColor = SystemColors.MenuText;

            settings.DockStripGradient.StartColor = SystemColors.Control;
            settings.DockStripGradient.EndColor = SystemColors.Control;
            settings.DockStripGradient.LinearGradientMode = LinearGradientMode.Vertical;
            settings.DockStripGradient.TextColor = SystemColors.ControlText;

            settings.ActiveTabGradient.StartColor = SystemColors.ControlLightLight;
            settings.ActiveTabGradient.EndColor = SystemColors.ControlLightLight;
            settings.ActiveTabGradient.LinearGradientMode = LinearGradientMode.Vertical;
            settings.ActiveTabGradient.TextColor = SystemColors.ActiveCaptionText;

            settings.InactiveTabGradient.StartColor = SystemColors.ControlLight;
            settings.InactiveTabGradient.EndColor = SystemColors.ControlLight;
            settings.InactiveTabGradient.LinearGradientMode = LinearGradientMode.Vertical;
            settings.InactiveTabGradient.TextColor = SystemColors.InactiveCaptionText;

            settings.ActiveCaptionGradient.StartColor = SystemColors.GradientActiveCaption;
            settings.ActiveCaptionGradient.EndColor = SystemColors.ActiveCaption;
            settings.ActiveCaptionGradient.LinearGradientMode = LinearGradientMode.Vertical;
            settings.ActiveCaptionGradient.TextColor = SystemColors.ActiveCaptionText;

            settings.InactiveCaptionGradient.StartColor = SystemColors.GradientInactiveCaption;
            settings.InactiveCaptionGradient.EndColor = SystemColors.InactiveCaption;
            settings.InactiveCaptionGradient.LinearGradientMode = LinearGradientMode.Vertical;
            settings.InactiveCaptionGradient.TextColor = SystemColors.InactiveCaptionText;

            settings.TitleBackColor = SystemColors.Control;
            settings.TitleForeColor = SystemColors.ActiveCaptionText;

            settings.LineColor = SystemColors.Control;
            settings.SelectedItemColor = SystemColors.Highlight;
            settings.SelectedItemTextColor = SystemColors.HighlightText;

        }
    }
}
