using System.Windows.Input;

namespace NotepadLite.Util
{
    public static class ViewUtil
    {
        public static void ShowWaitCursor(bool show)
        {
            if (show)
            {
                Mouse.OverrideCursor = Cursors.Wait;
            }
            else
            {
                Mouse.OverrideCursor = null;
            }
        }

        public static string GetWindowTitle(string fileName = StringValues.UntitledFileName, string fileModifiedIcon = "")
        {
            return $"{fileModifiedIcon}{fileName} - Notepad Lite";
        }
    }
}
