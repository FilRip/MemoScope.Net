﻿using System.Drawing;
using System.Windows.Forms;

using BrightIdeasSoftware;

using MemoScope.Core;
using MemoScope.Core.Bookmarks;
using MemoScope.Core.Helpers;

using WinFwk.UIMessages;

namespace MemoScope.Modules.Bookmarks
{
    public partial class BookmarkModule : UIClrDumpModule, IMessageListener<BookmarkMessage>
    {
        public BookmarkModule()
        {
            InitializeComponent();
        }

        public void SetUp(ClrDump clrDump)
        {
            ClrDump = clrDump;
            dlvBookmarks.InitColumns<Bookmark>();
            dlvBookmarks.SetUpAddressColumn(this);
            dlvBookmarks.SetUpTypeColumn(this);
            var col = dlvBookmarks[nameof(Bookmark.Comment)];
            col.CellEditUseWholeCell = true;
            dlvBookmarks.CellEditActivation = ObjectListView.CellEditActivateMode.DoubleClick;
            dlvBookmarks.CellEditFinished += (o, e) =>
            {
                ClrDump.ClrDumpInfo.Save();
            };

            var colColor = dlvBookmarks[nameof(Bookmark.Color)];
            dlvBookmarks.FormatCell += (o, e) =>
            {
                if (e.Column != colColor)
                {
                    return;
                }
                var rowObj = e.Model;
                if (rowObj is Bookmark bookmark)
                {
                    e.SubItem.BackColor = bookmark.Color;
                    e.SubItem.Text = bookmark.Color != Color.Empty ? null : "Select Color...";
                }
            };
            var colPick = dlvBookmarks[nameof(Bookmark.ColorPick)];
            colPick.IsButton = true;
            colPick.ButtonSizing = OLVColumn.ButtonSizingMode.CellBounds;

            dlvBookmarks.ButtonClick += (o, e) =>
            {
                var rowObj = e.Model;
                if (rowObj is Bookmark bookmark)
                {
                    ColorDialog colDiag = new()
                    {
                        Color = bookmark.Color
                    };
                    if (colDiag.ShowDialog() == DialogResult.OK)
                    {
                        bookmark.Color = colDiag.Color;
                        ClrDump.ClrDumpInfo.Save();
                    }
                }
            };
            dlvBookmarks.UseCellFormatEvents = true;
            LoadBookmarks();
        }

        private void LoadBookmarks()
        {
            dlvBookmarks.Objects = ClrDump.ClrDumpInfo.Bookmarks;
            dlvBookmarks.ShowGroups = true;
            dlvBookmarks.BuildGroups(nameof(Bookmark.TypeName), SortOrder.Ascending);
        }

        public override void PostInit()
        {
            Icon = Properties.Resources.award_star_gold_blue_small;
            Name = $"#{ClrDump.Id} - Bookmarks";
        }

        public void HandleMessage(BookmarkMessage message)
        {
            LoadBookmarks();
        }
    }
}
