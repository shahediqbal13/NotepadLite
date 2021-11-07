using NotepadLite.View;
using System;
using System.ComponentModel;
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
        public event EventHandler<EventArgs> FileModificationEvent;
        public event EventHandler<EventArgs> WindowClosingEvent;

        public MainWindow()
        {
            InitializeComponent();
        }

        public string WindowTitle { set { Title = value; } }

        public string EditorText
        {
            get
            {
                return textEditor.Text;
            }
            set
            {
                textEditor.TextChanged -= OnTextChanged;
                textEditor.Text = value;
                textEditor.TextChanged += OnTextChanged;
            }
        }

        public bool IsFileModified { get; set; }

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

        private void OnExitMenuClicked(object sender, ExecutedRoutedEventArgs e)
        {
            WindowClosingEvent?.Invoke(this, e);
        }

        private void OnWindowContentRendered(object sender, EventArgs e)
        {
            textEditor.Focus();
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            IsFileModified = true;
            FileModificationEvent?.Invoke(this, e);
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            WindowClosingEvent?.Invoke(this, e);
        }

        public void CloseWindow()
        {
            Closing -= OnWindowClosing;
            Application.Current.Shutdown();
        }
    }
}
