using PersonalTracker.Models;
using System.Windows;
using System.Windows.Controls;

namespace PersonalTracker.Views
{
    /// <summary>Interaction logic for ModifyUserPage.xaml</summary>
    public partial class ModifyUserPage : Page
    {
        //TODO Fully implement modification of User credentials, perhaps via an Admin interface. If adding an Admin interface, add an auditing system to see modifications.

        public ModifyUserPage() => InitializeComponent();

        private void ModifyUserPage_Loaded(object sender, RoutedEventArgs e) => AppState.CalculateScale(Grid);
    }
}