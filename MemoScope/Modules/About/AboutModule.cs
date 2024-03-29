﻿using System.Windows.Forms;

using WinFwk.UIModules;

namespace MemoScope.Modules.About
{
    public partial class AboutModule : UIModule
    {
        public AboutModule()
        {
            InitializeComponent();
            Icon = Properties.Resources.help_small;

            Summary = "About the application";
            tbVersion.Text = string.Format("v {0}", Application.ProductVersion);
        }

        private void LinkGitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start($"https://github.com/fremag/MemoScope.Net");
        }

        private void LinkWiki_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start($"https://github.com/fremag/MemoScope.Net/wiki");
        }
    }
}