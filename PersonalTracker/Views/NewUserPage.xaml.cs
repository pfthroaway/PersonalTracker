using Extensions;
using Extensions.Encryption;
using PersonalTracker.Models;
using System.Windows;
using System.Windows.Controls;

namespace PersonalTracker.Views
{
    /// <summary>Interaction logic for NewUserPage.xaml</summary>
    public partial class NewUserPage : Page
    {
        #region Click

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => ClosePage();

        private async void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (TxtUsername.Text.Length > 0)
            {
                if (PswdPassword.Password.Length >= 4 && PswdConfirmPassword.Password.Length >= 4)
                {
                    if (PswdPassword.Password == PswdConfirmPassword.Password)
                    {
                        User newUser = new User(TxtUsername.Text, PBKDF2.HashPassword(PswdPassword.Password));
                        if (!await AppState.CreateUser(newUser))
                            TxtUsername.Focus();
                        else
                            ClosePage();
                    }
                    else
                    {
                        AppState.DisplayNotification("Please ensure that your passwords match.", "Personal Tracker");
                        PswdPassword.Focus();
                    }
                }
                else
                {
                    AppState.DisplayNotification("Please ensure that your passwords are at least 4 characters in length.", "Personal Tracker");
                    PswdPassword.Focus();
                }
            }
            else
            {
                AppState.DisplayNotification("Please ensure that your username is filled out.", "Personal Tracker");
                TxtUsername.Focus();
            }
        }

        #endregion Click

        #region Page-Manipulation Methods

        /// <summary>Closes the Page.</summary>
        private void ClosePage() => AppState.GoBack();

        public NewUserPage()
        {
            InitializeComponent();
            TxtUsername.Focus();
        }

        private void TxtUsername_GotFocus(object sender, RoutedEventArgs e) => Functions.TextBoxGotFocus(sender);

        private void Pswd_GotFocus(object sender, RoutedEventArgs e) => Functions.PasswordBoxGotFocus(sender);

        #endregion Page-Manipulation Methods
    }
}