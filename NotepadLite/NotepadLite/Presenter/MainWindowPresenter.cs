using log4net;
using NotepadLite.Util;
using NotepadLite.View;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NotepadLite.Presenter
{
    public class MainWindowPresenter
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MainWindowPresenter));

        private static readonly string FileFilter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

        private readonly IMainWindow _view;
        private DialogResult _dialogResult;
        private static string currentFile;

        public MainWindowPresenter(IMainWindow view)
        {
            _view = view;
            Initialize();
        }

        private void Initialize()
        {
            _view.WindowTitle = ViewUtil.GetWindowTitle();

            _view.NewFileEvent += OnNewFileCreateRequest;
            _view.FileOpenEvent += OnFileOpenRequest;
            _view.FileSaveEvent += OnFileSaveRequest;
            _view.FileModificationEvent += OnFileModificationEvent;
            _view.WindowClosingEvent += OnWindowCloseRequest;
        }

        private void OnFileModificationEvent(object sender, EventArgs e)
        {
            if (_view.IsFileModified)
            {
                var fileName = string.IsNullOrEmpty(currentFile) ? StringValues.UntitledFileName : currentFile;
                _view.WindowTitle = ViewUtil.GetWindowTitle(fileName, "*");
            }
        }

        private async void OnNewFileCreateRequest(object sender, EventArgs e)
        {
            try
            {
                if (!await IsCurrentFileSaved())
                    return;

                _view.EditorText = string.Empty;
                currentFile = string.Empty;
                _view.WindowTitle = ViewUtil.GetWindowTitle();
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
                if (!await IsCurrentFileSaved())
                    return;

                using (var openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = FileFilter;
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        currentFile = openFileDialog.FileName;
                        _view.WindowTitle = ViewUtil.GetWindowTitle(currentFile);

                        await ReadFileAndSetTextToEditor(openFileDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
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

        private async void OnWindowCloseRequest(object sender, EventArgs e)
        {
            try
            {
                var fileSaved = await IsCurrentFileSaved();
                if (fileSaved || _dialogResult == DialogResult.No)
                    _view.CloseWindow();

            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public async Task ReadFileAndSetTextToEditor(string fileName)
        {
            try
            {
                ViewUtil.ShowWaitCursor(true);
                var content = await FileUtil.ReadFileAsync(fileName);
                _view.EditorText = content;
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

        private async Task SaveFile(string fileName)
        {
            try
            {
                ViewUtil.ShowWaitCursor(true);
                await FileUtil.WriteToFileAsync(fileName, _view.EditorText);
                _view.IsFileModified = false;
                _view.WindowTitle = ViewUtil.GetWindowTitle(fileName);
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

        private async Task<bool> IsCurrentFileSaved()
        {
            if (string.IsNullOrWhiteSpace(_view.EditorText))
                return true;

            if (!_view.IsFileModified)
                return true;

            var message = "Do you want to save current file?";
            _dialogResult = MessageBox.Show(message, "Warning",
                             MessageBoxButtons.YesNoCancel,
                             MessageBoxIcon.Warning);

            if (_dialogResult == DialogResult.Cancel)
                return false;

            if (_dialogResult == DialogResult.No)
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
