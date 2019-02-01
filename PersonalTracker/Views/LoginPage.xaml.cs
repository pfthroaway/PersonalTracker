using Extensions;
using PersonalTracker.Models;
using System.Windows;
using System.Windows.Controls;

namespace PersonalTracker.Views
{
    /// <summary>Interaction logic for LoginPage.xaml</summary>
    public partial class LoginPage : Page
    {
        #region Click

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (await AppState.AttemptLogin(TxtUsername.Text, PswdPassword.Password))
            {
                AppState.CurrentUser = await AppState.LoadUser(TxtUsername.Text);
                AppState.SetUserDatabaseInformation();
                AppState.Navigate(new TrackerPage());
                TxtUsername.Clear();
                PswdPassword.Clear();
                TxtUsername.Focus();
            }
            else
                AppState.DisplayNotification("Invalid credentials. Please try again.", "Personal Tracker");
        }

        private void BtnNewUser_Click(object sender, RoutedEventArgs e) => AppState.Navigate(new NewUserPage());

        #endregion Click

        #region Page-Manipulation Methods

        public LoginPage()
        {
            InitializeComponent();
            TxtUsername.Focus();
        }

        private void TxtUsername_GotFocus(object sender, RoutedEventArgs e) => Functions.TextBoxGotFocus(sender);

        private void PswdPassword_GotFocus(object sender, RoutedEventArgs e) => Functions.PasswordBoxGotFocus(sender);

        #endregion Page-Manipulation Methods
    }
}