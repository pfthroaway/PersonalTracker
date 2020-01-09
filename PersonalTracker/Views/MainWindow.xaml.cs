using PersonalTracker.Models;
using System.Windows;

namespace PersonalTracker.Views
{
    /// <summary>Interaction logic for MainWindow.xaml</summary>
    public partial class MainWindow : Window
    {
        #region Window-Manipulation Methods

        public MainWindow() => InitializeComponent();

        private void WindowMain_Loaded(object sender, RoutedEventArgs e)
        {
            AppState.MainWindow = this;
            AppState.FileManagement();
        }

        #endregion Window-Manipulation Methods
    }
}