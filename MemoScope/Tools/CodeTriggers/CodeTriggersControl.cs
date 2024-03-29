﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using BrightIdeasSoftware;

using ScintillaNET;

using WinFwk.UIMessages;
using WinFwk.UITools.Log;

namespace MemoScope.Tools.CodeTriggers
{
    public partial class CodeTriggersControl : UserControl
    {
        public List<CodeTrigger> Triggers => triggers;
        public MessageBus MessageBus { get; set; }
        public Func<object, string> CodeGetter { get; set; }
        public Action<List<CodeTrigger>> SaveTriggers { get; set; }
        public Func<List<CodeTrigger>> LoadTriggers { get; set; }
        public int NbTriggers { get; set; }

        private List<CodeTrigger> triggers;
        private CodeTrigger currentTrigger;

        public CodeTriggersControl()
        {
            InitializeComponent();

            dlvTriggers.CheckStatePutter = (rowObject, value) =>
            {
                CodeTrigger codeTrigger = rowObject as CodeTrigger;
                codeTrigger.Active = (value == CheckState.Checked);
                if (currentTrigger != null)
                {
                    cbActive.Checked = currentTrigger.Active;
                }
                return value;
            };
            dlvTriggers.CheckStateGetter = rowObject => ((CodeTrigger)rowObject).Active ? CheckState.Checked : CheckState.Unchecked;
            dlvTriggers.InitColumns<CodeTrigger>();
            InitCodeSyntax();
            Reset();
        }

        private void Reset()
        {
            RefreshTrigger(null);
        }

        public void RefreshTrigger(CodeTrigger trigger)
        {
            this.tbGroup.TextChanged -= this.TbGroup_TextChanged;
            this.tbName.TextChanged -= this.TbName_TextChanged;
            this.cbActive.CheckedChanged -= this.CbActive_CheckedChanged;
            this.tbCode.TextChanged -= this.TbCode_TextChanged;

            if (trigger == null)
            {
                tbGroup.Text = null;
                tbName.Text = null;
                tbCode.Text = null;
                cbActive.Checked = false;
            }
            else
            {
                tbGroup.Text = trigger.Group;
                tbName.Text = trigger.Name;
                tbCode.Text = trigger.Code;
                cbActive.Checked = trigger.Active;
            }
            this.tbGroup.TextChanged += this.TbGroup_TextChanged;
            this.tbName.TextChanged += this.TbName_TextChanged;
            this.cbActive.CheckedChanged += this.CbActive_CheckedChanged;
            this.tbCode.TextChanged += this.TbCode_TextChanged;
        }

        // Code copy/pasted from
        // https://github.com/jacobslusser/ScintillaNET/wiki/Automatic-Syntax-Highlighting
        private void InitCodeSyntax()
        {
            tbCode.Lexer = Lexer.Cpp;
            // Configuring the default style with properties
            // we have common to every lexer style saves time.
            tbCode.StyleResetDefault();
            tbCode.Styles[Style.Default].Font = "Consolas";
            tbCode.Styles[Style.Default].Size = 10;
            tbCode.StyleClearAll();

            // Configure the CPP (C#) lexer styles
            tbCode.Styles[Style.Cpp.Default].ForeColor = Color.Silver;
            tbCode.Styles[Style.Cpp.Comment].ForeColor = Color.FromArgb(0, 128, 0); // Green
            tbCode.Styles[Style.Cpp.CommentLine].ForeColor = Color.FromArgb(0, 128, 0); // Green
            tbCode.Styles[Style.Cpp.CommentLineDoc].ForeColor = Color.FromArgb(128, 128, 128); // Gray
            tbCode.Styles[Style.Cpp.Number].ForeColor = Color.Olive;
            tbCode.Styles[Style.Cpp.Word].ForeColor = Color.Blue;
            tbCode.Styles[Style.Cpp.Word2].ForeColor = Color.Blue;
            tbCode.Styles[Style.Cpp.String].ForeColor = Color.FromArgb(163, 21, 21); // Red
            tbCode.Styles[Style.Cpp.Character].ForeColor = Color.FromArgb(163, 21, 21); // Red
            tbCode.Styles[Style.Cpp.Verbatim].ForeColor = Color.FromArgb(163, 21, 21); // Red
            tbCode.Styles[Style.Cpp.StringEol].BackColor = Color.Pink;
            tbCode.Styles[Style.Cpp.Operator].ForeColor = Color.Purple;
            tbCode.Styles[Style.Cpp.Preprocessor].ForeColor = Color.Maroon;
        }

        private void TsbNewTrigger_Click(object sender, EventArgs e)
        {
            CreateTrigger();
            RefreshTriggers();
        }

        private void CreateTrigger()
        {
            currentTrigger = new CodeTrigger { Name = $"#{NbTriggers++:00}", Group = "Default" };
            triggers.Add(currentTrigger);
        }

        public void RefreshTriggers()
        {
            dlvTriggers.Objects = triggers;
            dlvTriggers.ShowGroups = true;
            dlvTriggers.BuildGroups(nameof(CodeTrigger.Group), SortOrder.Ascending);
            if (currentTrigger != null)
            {
                dlvTriggers.SelectedObject = currentTrigger;
                RefreshTrigger(currentTrigger);
            }
        }

        private void TsbSaveAllTriggers_Click(object sender, EventArgs e)
        {
            var triggersToSave = new List<CodeTrigger>(triggers.Select(t => t.Clone()));
            SaveTriggers(triggersToSave);
        }

        private void TsbCloneTrigger_Click(object sender, EventArgs e)
        {
            ListView.SelectedIndexCollection selectedTriggers = dlvTriggers.SelectedIndices;
            foreach (int idx in selectedTriggers)
            {
                CodeTrigger trig = triggers[idx];
                var newTrig = trig.Clone();
                triggers.Add(newTrig);
            }
            RefreshTriggers();
        }

        private void TsbDeleteTrigger_Click(object sender, EventArgs e)
        {
            ListView.SelectedIndexCollection selectedTriggers = dlvTriggers.SelectedIndices;
            foreach (int idx in selectedTriggers)
            {
                CodeTrigger trig = triggers[idx];
                triggers.Remove(trig);
                if (trig == currentTrigger)
                {
                    currentTrigger = null;
                    Reset();
                }
            }
            RefreshTriggers();
        }

        private void DlvTriggers_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentTrigger = dlvTriggers.SelectedObject as CodeTrigger;

            if (currentTrigger != null)
            {
                tbGroup.Text = currentTrigger.Group;
                tbName.Text = currentTrigger.Name;
                tbCode.Text = currentTrigger.Code;
                cbActive.Checked = currentTrigger.Active;
            }
            else
            {
                Reset();
            }
        }

        private void ProcessTriggers_Load(object sender, EventArgs e)
        {
            if (LoadTriggers != null)
            {
                triggers = LoadTriggers();
                NbTriggers = triggers.Count;
                RefreshTriggers();
            }
        }

        private void TbName_TextChanged(object sender, EventArgs e)
        {
            if (currentTrigger == null)
            {
                CreateTrigger();
            }
            currentTrigger.Name = tbName.Text;
            RefreshTriggers();
        }


        private void TbGroup_TextChanged(object sender, EventArgs e)
        {
            if (currentTrigger == null)
            {
                CreateTrigger();
            }
            currentTrigger.Group = tbGroup.Text;
            RefreshTriggers();
        }

        private void TbCode_TextChanged(object sender, EventArgs e)
        {
            if (currentTrigger == null)
            {
                CreateTrigger();
            }

            currentTrigger.Code = tbCode.Text;
            RefreshTriggers();
        }

        private void TbCode_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void TbCode_DragDrop(object sender, DragEventArgs e)
        {
            if (currentTrigger == null)
            {
                CreateTrigger();
                RefreshTriggers();
            }

            if (e.Data is not OLVDataObject data)
            {
                return;
            }
            foreach (object obj in data.ModelObjects)
            {
                if (tbCode.Text == null)
                {
                    tbCode.Text = CodeGetter(obj);
                }
                else
                {
                    tbCode.Text = tbCode.Text.Insert(tbCode.SelectionStart, CodeGetter(obj));
                }
            }
        }

        protected void Log(string text, LogLevelType logLevel = LogLevelType.Info)
        {
            MessageBus.SendMessage(new LogMessage(this, text, logLevel));
        }

        private void CbActive_CheckedChanged(object sender, EventArgs e)
        {
            if (currentTrigger != null)
            {
                currentTrigger.Active = cbActive.Checked;
            }
            RefreshTriggers();
        }
    }
}