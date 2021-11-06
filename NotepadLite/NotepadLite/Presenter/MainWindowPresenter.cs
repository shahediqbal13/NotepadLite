using log4net;
using NotepadLite.View;
using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;

namespace NotepadLite.Presenter
{
    public class MainWindowPresenter
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MainWindowPresenter));

        private IMainWindow _view;

        public MainWindowPresenter(IMainWindow view)
        {
            _view = view;
            Initialize();
        }

        private void Initialize()
        {
            _view.FileOpenEvent += OnFileOpenRequest;
        }

        private async void OnFileOpenRequest(object sender, EventArgs e)
        {
            try
            {
                using (var openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                        var fileStream = openFileDialog.OpenFile();
                        using (var reader = new StreamReader(fileStream))
                        {
                            var content = await reader.ReadToEndAsync();
                            _view.SetTextToEditor(content);
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
                Mouse.OverrideCursor = null;
            }
        }
    }
}
