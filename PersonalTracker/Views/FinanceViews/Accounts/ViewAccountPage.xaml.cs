using Extensions;
using Extensions.ListViewHelp;
using PersonalTracker.Models;
using PersonalTracker.Models.FinanceModels.Data;
using PersonalTracker.Views.FinanceViews.Search;
using PersonalTracker.Views.FinanceViews.Transactions;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace PersonalTracker.Views.FinanceViews.Accounts
{
    /// <summary>Interaction logic for ViewAccountWindow.xaml</summary>
    public partial class ViewAccountPage : INotifyPropertyChanged
    {
        private Account _selectedAccount;
        private FinancialTransaction _selectedTransaction;
        private ListViewSort _sort = new ListViewSort();

        #region Data-Binding

        /// <summary>Event that fires if a Property value has changed so that the UI can properly be updated.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Invokes <see cref="PropertyChangedEventHandler"/> to update the UI when a Property value changes.</summary>
        /// <param name="property">Name of Property whose value has changed</param>
        private void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Data-Binding

        /// <summary>Loads the Account that was selected.</summary>
        /// <param name="account">Selected Account</param>
        internal void LoadAccount(Account account)
        {
            _selectedAccount = account;
            LVTransactions.ItemsSource = _selectedAccount.AllTransactions;
            DataContext = _selectedAccount;
        }

        /// <summary>Refreshes the ListView's ItemsSource.</summary>
        internal void RefreshItemsSource()
        {
            _selectedAccount = AppState.CurrentUser.Finances.AllAccounts.Find(account => account.Name == _selectedAccount.Name);
            LVTransactions.ItemsSource = _selectedAccount.AllTransactions;
            LVTransactions.Items.Refresh();
            DataContext = _selectedAccount;
        }

        #region Button-Click Methods

        private void BtnBack_Click(object sender, RoutedEventArgs e) => ClosePage();

        private async void BtnDeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            if (AppState.YesNoNotification("Are you sure you want to delete this account? All transactions associated with it will be lost forever!", "Personal Tracker"))
            {
                if (await AppState.DeleteAccount(_selectedAccount))
                    ClosePage();
            }
        }

        private async void BtnDeleteTransaction_Click(object sender, RoutedEventArgs e)
        {
            if (AppState.YesNoNotification("Are you sure you want to delete this transaction? All data associated with it will be lost forever!", "Personal Tracker"))
            {
                _selectedAccount.RemoveTransaction(_selectedTransaction);
                if (await AppState.DeleteFinancialTransaction(_selectedTransaction, _selectedAccount))
                {
                    LVTransactions.UnselectAll();
                    RefreshItemsSource();
                }
            }
        }

        private void BtnModifyTransaction_Click(object sender, RoutedEventArgs e)
        {
            ModifyTransactionPage modifyTransactionWindow = new ModifyTransactionPage();
            modifyTransactionWindow.SetCurrentTransaction(_selectedTransaction, _selectedAccount);
            AppState.Navigate(modifyTransactionWindow);
        }

        private void BtnNewTransaction_Click(object sender, RoutedEventArgs e)
        {
            AppState.Navigate(new NewTransactionPage());
            LVTransactions.ItemsSource = null;
        }

        private void BtnNewTransfer_Click(object sender, RoutedEventArgs e)
        {
            AppState.Navigate(new NewTransferPage());
            LVTransactions.ItemsSource = null;
        }

        private void BtnRenameAccount_Click(object sender, RoutedEventArgs e)
        {
            RenameAccountPage renameAccountWindow = new RenameAccountPage();
            renameAccountWindow.LoadAccountName(_selectedAccount);
            AppState.Navigate(renameAccountWindow);
        }

        private void BtnSearchTransactions_Click(object sender, RoutedEventArgs e) => AppState.Navigate(
            new SearchTransactionsPage());

        #endregion Button-Click Methods

        #region Page-Manipulation Methods

        /// <summary>Closes the Page.</summary>
        private void ClosePage() => AppState.GoBack();

        public ViewAccountPage() => InitializeComponent();

        private void ViewAccountPage_Loaded(object sender, RoutedEventArgs e) => RefreshItemsSource();

        private void LVTransactionsColumnHeader_Click(object sender, RoutedEventArgs e) => _sort = Functions.ListViewColumnHeaderClick(sender, _sort, LVTransactions, "#CCCCCC");

        private void LVTransactions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LVTransactions.SelectedIndex >= 0)
            {
                _selectedTransaction = (FinancialTransaction)LVTransactions.SelectedValue;
                BtnDeleteTransaction.IsEnabled = true;
                BtnModifyTransaction.IsEnabled = true;
            }
            else
            {
                _selectedTransaction = new FinancialTransaction();
                BtnDeleteTransaction.IsEnabled = false;
                BtnModifyTransaction.IsEnabled = false;
            }
        }

        #endregion Page-Manipulation Methods
    }
}