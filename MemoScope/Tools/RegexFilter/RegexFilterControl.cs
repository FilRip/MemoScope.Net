using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MemoScope.Tools.RegexFilter
{
    public partial class RegexFilterControl : UserControl
    {
        Regex regex;
        public event Action<Regex> RegexApplied;
        public event Action RegexCancelled;

        public RegexFilterControl()
        {
            InitializeComponent();
        }

        private void TbRegex_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    Apply();
                    break;
                case Keys.Escape:
                    Cancel();
                    break;
            }
        }

        private void Cancel()
        {
            RegexCancelled?.Invoke();
            tbRegex.BackColor = Color.LightGray;
        }

        private void Apply()
        {
            try
            {
                if (cbIgnoreCase.Checked)
                {
                    regex = new Regex(tbRegex.Text);
                }
                else
                {
                    regex = new Regex(tbRegex.Text, RegexOptions.IgnoreCase);
                }

                RegexApplied?.Invoke(regex);
                tbRegex.BackColor = Color.LightGreen;
            }
            catch (ArgumentException)
            {
                Cancel();
                tbRegex.BackColor = Color.Orange;
            }
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            Apply();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        private void CbIgnoreCase_CheckedChanged(object sender, EventArgs e)
        {
            Apply();
        }
    }
}
