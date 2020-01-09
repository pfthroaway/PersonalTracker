using Extensions;
using Extensions.ListViewHelp;
using PersonalTracker.Finances.Models.Data;
using PersonalTracker.Finances.Views.Accounts;
using PersonalTracker.Finances.Views.Categories;
using PersonalTracker.Finances.Views.Credit;
using PersonalTracker.Finances.Views.Reports;
using PersonalTracker.Finances.Views.Transactions;
using PersonalTracker.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PersonalTracker.Finances.Views
{
    /// <summary>Interaction logic for FinancesPage.xaml</summary>
    public partial class FinancesPage
    {
        //TODO Fix Finances implementation to use ReadOnlyCollections.

        private List<Account> _allAccounts;

        private ListViewSort _sort = new ListViewSort();

        /// <summary>Refreshes the ListView's ItemsSource.</summary>
        internal void RefreshItemsSource()
        {
            _allAccounts = AppState.CurrentUser.Finances.AllAccounts;
            LVAccounts.ItemsSource = _allAccounts;
            LVAccounts.Items.Refresh();
        }

        #region Click Methods

        private void BtnBack_Click(object sender, RoutedEventArgs e) => AppState.GoBack();

        private void BtnNewAccount_Click(object sender, RoutedEventArgs e) => AppState.Navigate(new NewAccountPage());

        private void LVAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e) => BtnViewTransactions.IsEnabled = LVAccounts.SelectedIndex >= 0;

        private void BtnManageCategories_Click(object sender, RoutedEventArgs e) => AppState.Navigate(new ManageCategoriesPage());

        private void BtnMonthlyReport_Click(object sender, RoutedEventArgs e) => AppState.Navigate(new MonthlyReportPage());

        private void BtnViewAccount_Click(object sender, RoutedEventArgs e)
        {
            Account selectedAccount = (Account)LVAccounts.SelectedValue;
            ViewAccountPage viewAccountWindow = new ViewAccountPage();
            viewAccountWindow.LoadAccount(selectedAccount);
            AppState.Navigate(viewAccountWindow);
        }

        private void BtnViewCreditScores_Click(object sender, RoutedEventArgs e) =>
            AppState.Navigate(new CreditScorePage());

        private void BtnViewAllTransactions_Click(object sender, RoutedEventArgs e) => AppState.Navigate(new ViewAllTransactionsPage());

        private void LVAccountsColumnHeader_Click(object sender, RoutedEventArgs e) => _sort = Functions.ListViewColumnHeaderClick(sender, _sort, LVAccounts, "#CCCCCC");

        #endregion Click Methods

        #region Page-Manipulation Methods

        public FinancesPage() => InitializeComponent();

        private void MainPage_Loaded(object sender, RoutedEventArgs e) => RefreshItemsSource();

        #endregion Page-Manipulation Methods
    }
}