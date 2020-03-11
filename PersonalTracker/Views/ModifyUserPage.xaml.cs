using Extensions;
using Extensions.Encryption;
using PersonalTracker.Models;
using System.Windows;
using System.Windows.Controls;

namespace PersonalTracker.Views
{
    /// <summary>Interaction logic for ModifyUserPage.xaml</summary>
    public partial class ModifyUserPage : Page
    {
        //TODO Fully implement modification of User credentials, perhaps via an Admin interface. If adding an Admin interface, add an auditing system to see modifications.

        #region Button-Click Methods

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (Argon2.ValidatePassword(AppState.CurrentUser.Password, PswdCurrentPassword.Password))
            {
                if (PswdNewPassword.Password.Length >= 4 && PswdConfirmPassword.Password.Length >= 4)
                {
                    if (PswdNewPassword.Password == PswdConfirmPassword.Password)
                    {
                        if (PswdCurrentPassword.Password != PswdNewPassword.Password)
                        {
                            AppState.CurrentUser.Password = Argon2.HashPassword(PswdNewPassword.Password);
                            AppState.ModifyUser(AppState.CurrentUser);
                            AppState.DisplayNotification("Successfully changed password.", "Sulimn");
                            AppState.GoBack();
                        }
                        else
                            AppState.DisplayNotification("The new password can't be the same as the current password.", "Sulimn");
                    }
                    else
                        AppState.DisplayNotification("Please ensure the new passwords match.", "Sulimn");
                }
                else
                    AppState.DisplayNotification("Your password must be at least 4 characters.", "Sulimn");
            }
            else
                AppState.DisplayNotification("Invalid current password.", "Sulimn");
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => AppState.GoBack();

        #endregion Button-Click Methods

        #region Page-Manipulation Methods

        public ModifyUserPage() => InitializeComponent();

        private void PswdChanged(object sender, RoutedEventArgs e) => BtnSubmit.IsEnabled = PswdCurrentPassword.Password.Length > 0 && PswdNewPassword.Password.Length > 0 && PswdConfirmPassword.Password.Length > 0;

        private void Pswd_GotFocus(object sender, RoutedEventArgs e) => Functions.PasswordBoxGotFocus(sender);

        #endregion Page-Manipulation Methods
    }
}