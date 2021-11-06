using log4net;
using NotepadLite.Util;
using NotepadLite.View;
using System;
using System.IO;
using System.Windows.Forms;

namespace NotepadLite.Presenter
{
    public class MainWindowPresenter
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MainWindowPresenter));

        private static readonly string FileFilter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

        private readonly IMainWindow _view;

        public MainWindowPresenter(IMainWindow view)
        {
            _view = view;
            Initialize();
        }

        private void Initialize()
        {
            _view.FileOpenEvent += OnFileOpenRequest;
            _view.FileSaveEvent += OnFileSaveRequest;
        }

        private async void OnFileOpenRequest(object sender, EventArgs e)
        {
            try
            {
                using (var openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = FileFilter;
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        ViewUtil.ShowWaitCursor(true);

                        var fileStream = openFileDialog.OpenFile();
                        using (var reader = new StreamReader(fileStream))
                        {
                            var content = await reader.ReadToEndAsync();
                            _view.EditorText = content;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            finally
            {
                ViewUtil.ShowWaitCursor(false);
            }
        }

        private async void OnFileSaveRequest(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_view.EditorText))
                {
                    return;
                }

                var saveFileDialog = new SaveFileDialog
                {
                    Filter = FileFilter,
                    Title = "Save File"
                };
                saveFileDialog.ShowDialog();

                if (saveFileDialog.FileName != "")
                {
                    using (var sw = new StreamWriter(saveFileDialog.FileName))
                    {
                        await sw.WriteAsync(_view.EditorText);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}
