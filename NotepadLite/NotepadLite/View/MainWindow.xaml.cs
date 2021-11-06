using NotepadLite.Presenter;
using NotepadLite.View;
using System;
using System.Windows;
using System.Windows.Input;

namespace NotepadLite
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainWindow
    {
        public event EventHandler<EventArgs> NewFileEvent;
        public event EventHandler<EventArgs> FileOpenEvent;
        public event EventHandler<EventArgs> FileSaveEvent;

        public MainWindow()
        {
            InitializeComponent();

            new MainWindowPresenter(this);
        }

        public void SetTextToEditor(string text)
        {
            textEditor.Text = text;
        }

        private void OnNewMenuClicked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("New");
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnOpenMenuClicked(object sender, ExecutedRoutedEventArgs e)
        {
            FileOpenEvent?.Invoke(this, e);
        }

        private void OnWindowContentRendered(object sender, EventArgs e)
        {
            textEditor.Focus();
        }
    }
}
