using Extensions;
using Extensions.DataTypeHelpers;
using Extensions.Enums;
using PersonalTracker.Finances.Models.Categories;
using PersonalTracker.Finances.Models.Data;
using PersonalTracker.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PersonalTracker.Finances.Views.Transactions
{
    /// <summary>Interaction logic for NewTransactionWindow.xaml</summary>
    public partial class NewTransactionPage : INotifyPropertyChanged
    {
        private readonly List<Account> _allAccounts = AppState.CurrentUser.Finances.AllAccounts;
        private readonly List<Category> _allCategories = AppState.CurrentUser.Finances.AllCategories;
        private Category _selectedCategory = new Category();
        private Account _selectedAccount = new Account();

        #region Data-Binding

        /// <summary>Event that fires if a Property value has changed so that the UI can properly be updated.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Invokes <see cref="PropertyChangedEventHandler"/> to update the UI when a Property value changes.</summary>
        /// <param name="property">Name of Property whose value has changed</param>
        private void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Data-Binding

        /// <summary>Attempts to add a Transaction to the database.</summary>
        /// <returns>Returns true if successfully added</returns>
        private async Task<bool> AddTransaction()
        {
            FinancialTransaction newTransaction = new FinancialTransaction(await AppState.GetNextFinancialTransactionIndex(), DateTimeHelper.Parse(TransactionDate.SelectedDate),
                TxtPayee.Text, CmbMajorCategory.SelectedValue.ToString(), CmbMinorCategory.SelectedValue.ToString(),
                TxtMemo.Text, DecimalHelper.Parse(TxtOutflow.Text), DecimalHelper.Parse(TxtInflow.Text),
                _selectedAccount.Name);
            _selectedAccount.AddTransaction(newTransaction);
            AppState.CurrentUser.Finances.AllTransactions.Add(newTransaction);

            return await AppState.AddFinancialTransaction(newTransaction, _selectedAccount);
        }

        /// <summary>Resets all values to default status.</summary>
        private void Reset()
        {
            TransactionDate.Text = "";
            CmbMajorCategory.SelectedIndex = -1;
            CmbMinorCategory.SelectedIndex = -1;
            TxtMemo.Text = "";
            TxtPayee.Text = "";
            TxtInflow.Text = "";
            TxtOutflow.Text = "";
            CmbAccount.SelectedIndex = -1;
        }

        /// <summary>Toggles Buttons on the Window.</summary>
        /// <param name="enabled">Should Button be enabled?</param>
        private void ToggleButtons(bool enabled)
        {
            BtnSaveAndDone.IsEnabled = enabled;
            BtnSaveAndDuplicate.IsEnabled = enabled;
            BtnSaveAndNew.IsEnabled = enabled;
        }

        #region Button-Click Methods

        private async void BtnSaveAndDone_Click(object sender, RoutedEventArgs e)
        {
            if (await AddTransaction())
                ClosePage();
        }

        private async void BtnSaveAndDuplicate_Click(object sender, RoutedEventArgs e) => await AddTransaction();

        private async void BtnSaveAndNew_Click(object sender, RoutedEventArgs e)
        {
            if (await AddTransaction())
            {
                Reset();
                CmbMinorCategory.Focus();
            }
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e) => Reset();

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => ClosePage();

        #endregion Button-Click Methods

        #region Text/Selection Changed

        /// <summary>Checks whether or not the Save buttons should be enabled.</summary>
        private void TextChanged() => ToggleButtons(TransactionDate.SelectedDate != null && CmbMajorCategory.SelectedIndex >= 0 && CmbMinorCategory.SelectedIndex >= 0 && TxtPayee.Text.Length > 0 && (TxtInflow.Text.Length > 0 || TxtOutflow.Text.Length > 0) && CmbAccount.SelectedIndex >= 0);

        private void Txt_TextChanged(object sender, TextChangedEventArgs e) => TextChanged();

        private void TxtInOutflow_TextChanged(object sender, TextChangedEventArgs e)
        {
            Functions.TextBoxTextChanged(sender, KeyType.Decimals);
            TextChanged();
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => TextChanged();

        private void CmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbMajorCategory.SelectedIndex >= 0)
            {
                CmbMinorCategory.IsEnabled = true;
                _selectedCategory = (Category)CmbMajorCategory.SelectedValue;
                CmbMinorCategory.ItemsSource = _selectedCategory.MinorCategories;
            }
            else
            {
                CmbMinorCategory.IsEnabled = false;
                _selectedCategory = new Category();
                CmbMinorCategory.ItemsSource = _selectedCategory.MinorCategories;
            }

            TextChanged();
        }

        private void CmbAccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbAccount.SelectedIndex >= 0)
                _selectedAccount = (Account)CmbAccount.SelectedValue;
            else
                _selectedAccount = new Account();
            TextChanged();
        }

        #endregion Text/Selection Changed

        #region Page-Manipulation Methods

        /// <summary>Closes the Page.</summary>
        private void ClosePage() => AppState.GoBack();

        public NewTransactionPage()
        {
            InitializeComponent();
            CmbAccount.ItemsSource = _allAccounts;
            CmbMajorCategory.ItemsSource = _allCategories;
            CmbMinorCategory.ItemsSource = _selectedCategory.MinorCategories;
        }

        private void TxtInflowOutflow_PreviewKeyDown(object sender, KeyEventArgs e) => Functions.PreviewKeyDown(e, KeyType.Decimals);

        private void Txt_GotFocus(object sender, RoutedEventArgs e) => Functions.TextBoxGotFocus(sender);

        #endregion Page-Manipulation Methods
    }
}