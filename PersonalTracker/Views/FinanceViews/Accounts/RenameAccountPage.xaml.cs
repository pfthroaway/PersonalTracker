using PersonalTracker.Models;
using PersonalTracker.Models.FinanceModels.Data;
using System.Windows;

namespace PersonalTracker.Views.FinanceViews.Accounts
{
    /// <summary>Interaction logic for RenameAccountWindow.xaml</summary>
    public partial class RenameAccountPage
    {
        private Account _selectedAccount;

        internal void LoadAccountName(Account currentAccount)
        {
            _selectedAccount = currentAccount;
            TxtAccountName.Text = _selectedAccount.Name;
        }

        #region Button-Click Methods

        private async void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (TxtAccountName.Text != _selectedAccount.Name)
            {
                if (await AppState.RenameAccount(_selectedAccount, TxtAccountName.Text))
                    ClosePage();
                else
                    AppState.DisplayNotification("Unable to process account name change.", "Personal Tracker");
            }
            else
                AppState.DisplayNotification("The account name can't be changed to what it already is.", "Personal Tracker");
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => ClosePage();

        #endregion Button-Click Methods

        #region Page-Manipulation Methods

        /// <summary>Closes the Page.</summary>
        private void ClosePage() => AppState.GoBack();

        public RenameAccountPage() => InitializeComponent();

        #endregion Page-Manipulation Methods
    }
}