using PersonalTracker.Models;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

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
            AppState.VerifyDatabaseIntegrity();
        }

        #endregion Window-Manipulation Methods
    }
}