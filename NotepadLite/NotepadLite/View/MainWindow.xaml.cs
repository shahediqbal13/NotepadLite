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

        public string EditorText
        {
            get
            {
                return textEditor.Text;
            }
            set
            {
                textEditor.Text = value;
            }
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnNewMenuClicked(object sender, RoutedEventArgs e)
        {
            NewFileEvent?.Invoke(this, e);
        }

        private void OnOpenMenuClicked(object sender, ExecutedRoutedEventArgs e)
        {
            FileOpenEvent?.Invoke(this, e);
        }

        private void OnSaveMenuClicked(object sender, ExecutedRoutedEventArgs e)
        {
            FileSaveEvent?.Invoke(this, e);
        }

        private void OnWindowContentRendered(object sender, EventArgs e)
        {
            textEditor.Focus();
        }
    }
}
