using Extensions;
using Extensions.ListViewHelp;
using PersonalTracker.Models;
using PersonalTracker.Models.FuelModels;
using PersonalTracker.Views.FuelViews.Transactions;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PersonalTracker.Views.FuelViews.Vehicles
{
    /// <summary>Interaction logic for ViewVehicleTransactionsPage.xaml</summary>
    public partial class ManageFuelupsPage
    {
        private Vehicle _currentVehicle;
        private ListViewSort _sort = new ListViewSort();
        private FuelTransaction _selectedTransaction = new FuelTransaction();

        /// <summary>The currently selected Vehicle.</summary>
        internal Vehicle CurrentVehicle
        {
            get => _currentVehicle;
            set
            {
                _currentVehicle = value;
                OnPropertyChanged("CurrentVehicle");
            }
        }

        /// <summary>Refreshes the ListView's ItemsSource.</summary>
        internal void RefreshItemsSource()
        {
            CurrentVehicle = AppState.CurrentUser.Vehicles.ToList().Find(vehicle => vehicle.VehicleID == CurrentVehicle.VehicleID);
            LVTransactions.ItemsSource = CurrentVehicle.Transactions;
            LVTransactions.Items.Refresh();
            DataContext = CurrentVehicle;
        }

        #region Data-Binding

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this,
            new PropertyChangedEventArgs(property));

        #endregion Data-Binding

        #region Click

        private void BtnAddFuelup_Click(object sender, RoutedEventArgs e) => AppState.Navigate(
            new AddTransactionPage { PreviousPage = this, CurrentVehicle = CurrentVehicle });

        private void BtnBack_Click(object sender, RoutedEventArgs e) => ClosePage();

        private async void BtnDeleteTransaction_Click(object sender, RoutedEventArgs e)
        {
            if (AppState.YesNoNotification("Are you sure you want to delete this Transaction? This action cannot be undone.",
                "Fuel Tracker"))
                if (await AppState.DeleteFuelTransaction(_selectedTransaction))
                {
                    CurrentVehicle.RemoveTransaction(_selectedTransaction);
                    RefreshItemsSource();
                }
        }

        private void BtnModifyTransaction_Click(object sender, RoutedEventArgs e) => AppState.Navigate(
            new ModifyTransactionPage
            {
                UnmodifiedTransaction = _selectedTransaction,
                ModifiedTransaction = new FuelTransaction(_selectedTransaction),
                CurrentVehicle = CurrentVehicle
            });

        private void BtnSearchTransactions_Click(object sender, RoutedEventArgs e)
        {
            //TODO Implement Searching Fuel Transactions
        }

        private void LVTransactions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BtnModifyTransaction.IsEnabled = LVTransactions.SelectedIndex >= 0;
            BtnDeleteTransaction.IsEnabled = LVTransactions.SelectedIndex >= 0;
            _selectedTransaction = LVTransactions.SelectedIndex >= 0
                ? (FuelTransaction)LVTransactions.SelectedItem
                : new FuelTransaction();
        }

        private void LVTransactionsColumnHeader_Click(object sender, RoutedEventArgs e) => _sort = Functions.ListViewColumnHeaderClick(sender, _sort, LVTransactions, "#CCCCCC");

        #endregion Click

        #region Page-Manipulation Methods

        /// <summary>Closes the Page.</summary>
        private void ClosePage() => AppState.GoBack();

        public ManageFuelupsPage() => InitializeComponent();

        private void ManageVehiclePage_OnLoaded(object sender, RoutedEventArgs e)
        {
            AppState.CalculateScale(Grid);
            RefreshItemsSource();
        }

        #endregion Page-Manipulation Methods
    }
}