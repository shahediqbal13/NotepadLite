using System;

namespace NotepadLite.View
{
    public interface IMainWindow
    {
        event EventHandler<EventArgs> NewFileEvent;
        event EventHandler<EventArgs> FileOpenEvent;
        event EventHandler<EventArgs> FileSaveEvent;
        event EventHandler<EventArgs> FileChangeEvent;

        string WindowTitle { set; }
        string EditorText { get; set; }
        bool IsFileModified { get; set; }
    }
}
