using Extensions;
using Extensions.DataTypeHelpers;
using Extensions.Enums;
using PersonalTracker.Fuel.Models;
using PersonalTracker.Fuel.Views.Vehicles;
using PersonalTracker.Models;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PersonalTracker.Fuel.Views.Transactions
{
    /// <summary>Interaction logic for AddTransaction.xaml</summary>
    public partial class AddTransactionPage
    {
        /// <summary>Previous Page so that its ItemsSource can be updated prior to navigating back to it.</summary>
        internal ManageFuelupsPage PreviousPage { get; set; }

        /// <summary><see cref="Vehicle"/> to which the <see cref="FuelTransaction"/> is being added.</summary>
        internal Vehicle CurrentVehicle { get; set; }

        /// <summary>Attempts to add a <see cref="FuelTransaction"/> to the database.</summary>
        /// <returns>Returns true if successfully added</returns>
        private async Task<bool> AddTransaction()
        {
            FuelTransaction newTransaction = new FuelTransaction(await AppState.GetNextFuelTransactionIndex(),
                CurrentVehicle.VehicleID, DateTimeHelper.Parse(TransactionDate.SelectedDate), TxtStore.Text,
                Int32Helper.Parse(TxtOctane.Text), DecimalHelper.Parse(TxtDistance.Text),
                DecimalHelper.Parse(TxtGallons.Text), DecimalHelper.Parse(TxtPrice.Text),
                DecimalHelper.Parse(TxtOdometer.Text), Int32Helper.Parse(TxtRange.Text));

            // if the odometer or distance weren't both set, determine the distance/odometer so MPG/odometer will be calculated properly
            if (newTransaction.Distance <= 0M)
            {
                if (newTransaction.Odometer > 0M && CurrentVehicle.Transactions.Count > 0)
                    newTransaction.Distance = newTransaction.Odometer - CurrentVehicle.Transactions[0].Odometer;
                else if (newTransaction.Odometer >= 0M && CurrentVehicle.Transactions.Count == 0)
                    newTransaction.Distance = newTransaction.Odometer;
            }
            else if (newTransaction.Odometer <= 0M)
            {
                if (newTransaction.Distance > 0M && CurrentVehicle.Transactions.Count > 0)
                    newTransaction.Odometer = CurrentVehicle.Transactions[0].Odometer + newTransaction.Distance;
                else if (newTransaction.Distance > 0M && CurrentVehicle.Transactions.Count == 0)
                    newTransaction.Odometer = newTransaction.Distance;
            }

            if (!await AppState.NewFuelTransaction(newTransaction)) return false;
            CurrentVehicle.AddTransaction(newTransaction);
            return true;
        }

        /// <summary>Resets all values to default status.</summary>
        private void Reset()
        {
            TxtStore.Text = "";
            TxtOctane.Text = "";
            TxtDistance.Text = "";
            TxtGallons.Text = "";
            TxtPrice.Text = "";
            TxtOdometer.Text = "";
            TxtRange.Text = "";
        }

        /// <summary>Toggles Buttons on the Page.</summary>
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
            else
                AppState.DisplayNotification("Unable to process transaction.", "Personal Tracker");
        }

        private async void BtnSaveAndDuplicate_Click(object sender, RoutedEventArgs e)
        {
            if (!await AddTransaction())
                AppState.DisplayNotification("Unable to process transaction.", "Personal Tracker");
        }

        private async void BtnSaveAndNew_Click(object sender, RoutedEventArgs e)
        {
            if (await AddTransaction())
            {
                Reset();
                TxtStore.Focus();
            }
            else
                AppState.DisplayNotification("Unable to process transaction.", "Personal Tracker");
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e) => Reset();

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => ClosePage();

        #endregion Button-Click Methods

        #region Input Manipulation

        /// <summary>Checks whether or not the Save buttons should be enabled.</summary>
        private void TextChanged() => ToggleButtons(TransactionDate.SelectedDate != null && TxtStore.Text.Length > 0 && TxtOctane.Text.Length > 0 && (TxtDistance.Text.Length > 0 | TxtOdometer.Text.Length > 0) && TxtGallons.Text.Length > 0 && TxtPrice.Text.Length > 0);

        private void Txt_GotFocus(object sender, RoutedEventArgs e) => Functions.TextBoxGotFocus(sender);

        private void Decimal_PreviewKeyDown(object sender, KeyEventArgs e) => Functions.PreviewKeyDown(e, KeyType.Decimals);

        private void Decimal_TextChanged(object sender, TextChangedEventArgs e)
        {
            Functions.TextBoxTextChanged(sender, KeyType.Decimals);
            TextChanged();
        }

        private void Integer_TextChanged(object sender, TextChangedEventArgs e)
        {
            Functions.TextBoxTextChanged(sender, KeyType.Integers);
            TextChanged();
        }

        private void Txt_TextChanged(object sender, TextChangedEventArgs e) => TextChanged();

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => TextChanged();

        private void Integer_PreviewKeyDown(object sender, KeyEventArgs e) => Functions.PreviewKeyDown(e, KeyType.Integers);

        #endregion Input Manipulation

        #region Page-Manipulation Methods

        /// <summary>Closes the Page.</summary>
        private void ClosePage()
        {
            PreviousPage.RefreshItemsSource();
            AppState.GoBack();
        }

        public AddTransactionPage()
        {
            InitializeComponent();
            TxtStore.Focus();
        }

        private void AddTransactionPage_OnLoaded(object sender, RoutedEventArgs e) => DataContext = CurrentVehicle;

        #endregion Page-Manipulation Methods
    }
}