using log4net;
using NotepadLite.Util;
using NotepadLite.View;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NotepadLite.Presenter
{
    public class MainWindowPresenter
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MainWindowPresenter));

        private static readonly string FileFilter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

        private readonly IMainWindow _view;
        private static string currentFile;

        public MainWindowPresenter(IMainWindow view)
        {
            _view = view;
            Initialize();
        }

        private void Initialize()
        {
            _view.NewFileEvent += OnNewFileCreateRequest;
            _view.FileOpenEvent += OnFileOpenRequest;
            _view.FileSaveEvent += OnFileSaveRequest;
        }

        private async void OnNewFileCreateRequest(object sender, EventArgs e)
        {
            try
            {
                if (!await CanCreateOrOpenNewFile())
                    return;

                _view.EditorText = string.Empty;

                //if (!_view.IsFileModified)
                //{
                //    _view.EditorText = string.Empty;
                //    return;
                //}

                //var message = "Do you want to save current file?";
                //var result = MessageBox.Show(message, "Warning",
                //                 MessageBoxButtons.YesNoCancel,
                //                 MessageBoxIcon.Warning);

                //if (result == DialogResult.Cancel)
                //    return;

                //if (result == DialogResult.No)
                //{
                //    _view.EditorText = string.Empty;
                //    return;
                //}

                //var fileName = GetFileNameToSave();
                //if (!string.IsNullOrEmpty(fileName))
                //{
                //    await SaveFile(fileName);
                //    _view.EditorText = string.Empty;
                //}
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private async void OnFileOpenRequest(object sender, EventArgs e)
        {
            try
            {
                if (!await CanCreateOrOpenNewFile())
                    return;

                using (var openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = FileFilter;
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        ViewUtil.ShowWaitCursor(true);
                        currentFile = openFileDialog.FileName;

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
                if (string.IsNullOrWhiteSpace(_view.EditorText))
                    return;

                var fileName = GetFileNameToSave();
                if (!string.IsNullOrEmpty(fileName))
                {
                    await SaveFile(fileName);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private async Task SaveFile(string fileName)
        {
            try
            {
                using (var sw = new StreamWriter(fileName))
                {
                    ViewUtil.ShowWaitCursor(true);
                    await sw.WriteAsync(_view.EditorText);
                    _view.IsFileModified = false;
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

        private string GetFileNameToSave()
        {
            try
            {
                if (!string.IsNullOrEmpty(currentFile))
                    return currentFile;

                var saveFileDialog = new SaveFileDialog
                {
                    Filter = FileFilter,
                    Title = "Save File"
                };
                saveFileDialog.ShowDialog();

                currentFile = saveFileDialog.FileName;
                return currentFile;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return string.Empty;
            }
        }

        private async Task<bool> CanCreateOrOpenNewFile()
        {
            if (string.IsNullOrWhiteSpace(_view.EditorText))
                return true;

            if (!_view.IsFileModified)
                return true;

            var message = "Do you want to save current file?";
            var result = MessageBox.Show(message, "Warning",
                             MessageBoxButtons.YesNoCancel,
                             MessageBoxIcon.Warning);

            if (result == DialogResult.Cancel)
                return false;

            if (result == DialogResult.No)
                return true;

            var fileName = GetFileNameToSave();
            if (!string.IsNullOrEmpty(fileName))
            {
                await SaveFile(fileName);
                return true;
            }

            return false;
        }
    }
}
