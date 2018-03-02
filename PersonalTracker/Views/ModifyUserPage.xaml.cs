using PersonalTracker.Models;
using System.Windows;
using System.Windows.Controls;

namespace PersonalTracker.Views
{
    /// <summary>Interaction logic for ModifyUserPage.xaml</summary>
    public partial class ModifyUserPage : Page
    {
        public ModifyUserPage()
        {
            InitializeComponent();
        }

        private void ModifyUserPage_Loaded(object sender, RoutedEventArgs e) => AppState.CalculateScale(Grid);
    }
}