using System;

namespace NotepadLite.View
{
    public interface IMainWindow
    {
        event EventHandler<EventArgs> NewFileEvent;
        event EventHandler<EventArgs> FileOpenEvent;
        event EventHandler<EventArgs> FileSaveEvent;

        string EditorText { get; set; }
    }
}
