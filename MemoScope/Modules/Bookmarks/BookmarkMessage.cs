using MemoScope.Core.Data;

using WinFwk.UIMessages;

namespace MemoScope.Modules.Bookmarks
{
    public enum BookmarkAction { Add, Remove, Update }

    public class BookmarkMessage(BookmarkAction action, ClrDumpObject clrDumpObject) : AbstractUIMessage
    {
        public BookmarkAction Action { get; } = action;
        public ClrDumpObject ClrDumpObject { get; } = clrDumpObject;
    }
}