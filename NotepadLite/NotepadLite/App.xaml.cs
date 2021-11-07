using log4net;
using NotepadLite.Presenter;
using NotepadLite.Util;
using System;
using System.Windows;

namespace NotepadLite
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(App));

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
                var presenter = new MainWindowPresenter(mainWindow);
                if (e.Args != null && e.Args.Length == 1)
                {
                    if (FileUtil.FileExists(e.Args[0]))
                    {
                        await presenter.ReadFileAndSetTextToEditor(e.Args[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
    }
}
