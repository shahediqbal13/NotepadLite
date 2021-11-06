using log4net;
using NotepadLite.View;
using System;
using System.IO;
using System.Windows.Forms;

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

        private void OnFileOpenRequest(object sender, EventArgs e)
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
                        var fileStream = openFileDialog.OpenFile();
                        using (var reader = new StreamReader(fileStream))
                        {
                            _view.SetTextToEditor(reader.ReadToEnd());
                        }
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
