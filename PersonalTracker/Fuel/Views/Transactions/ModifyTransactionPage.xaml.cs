using Extensions;
using Extensions.DataTypeHelpers;
using Extensions.Enums;
using PersonalTracker.Fuel.Models;
using PersonalTracker.Models;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PersonalTracker.Fuel.Views.Transactions
{
    /// <summary>Interaction logic for ModifyTransaction.xaml</summary>
    public partial class ModifyTransactionPage
    {
        internal FuelTransaction UnmodifiedTransaction, ModifiedTransaction;
        internal Vehicle CurrentVehicle;

        /// <summary>Attempts to add a <see cref="FuelTransaction"/> to the database.</summary>
        /// <returns>Returns true if successfully added</returns>
        private async Task<bool> ModifyTransaction()
        {
            ModifiedTransaction = new FuelTransaction(UnmodifiedTransaction.TranscationID,
                UnmodifiedTransaction.VehicleID, DateTimeHelper.Parse(TransactionDate.SelectedDate), TxtStore.Text,
                Int32Helper.Parse(TxtOctane.Text), DecimalHelper.Parse(TxtDistance.Text),
                DecimalHelper.Parse(TxtGallons.Text), DecimalHelper.Parse(TxtPrice.Text),
                DecimalHelper.Parse(TxtOdometer.Text), Int32Helper.Parse(TxtRange.Text));

            if (await AppState.ModifyFuelTransaction(UnmodifiedTransaction, ModifiedTransaction))
            {
                CurrentVehicle.ModifyTransaction(UnmodifiedTransaction, ModifiedTransaction);
                return true;
            }
            return false;
        }

        /// <summary>Displays the original <see cref="FuelTransaction"/>.</summary>
        internal void DisplayOriginalTransaction()
        {
            TransactionDate.SelectedDate = UnmodifiedTransaction.Date;
            TxtStore.Text = UnmodifiedTransaction.Store;
            TxtOctane.Text = UnmodifiedTransaction.Octane.ToString();
            TxtDistance.Text = UnmodifiedTransaction.DistanceToString;
            TxtGallons.Text = UnmodifiedTransaction.GallonsToString;
            TxtPrice.Text = UnmodifiedTransaction.PriceToString;
            TxtOdometer.Text = UnmodifiedTransaction.OdometerToString;
            TxtRange.Text = UnmodifiedTransaction.Range.ToString();
        }

        #region Button-Click Methods

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (await ModifyTransaction())
                ClosePage();
            else
                AppState.DisplayNotification("Unable to modify transaction.", "Personal Tracker");
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e) => DisplayOriginalTransaction();

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => ClosePage();

        #endregion Button-Click Methods

        #region Input Manipulation

        /// <summary>Checks whether or not the Save buttons should be enabled.</summary>
        private void TextChanged()
        {
            bool enabled = TransactionDate.SelectedDate != null && TxtStore.Text.Length > 0 && TxtOctane.Text.Length > 0 && (TxtDistance.Text.Length > 0 | TxtOdometer.Text.Length > 0) && TxtGallons.Text.Length > 0 && TxtPrice.Text.Length > 0 && (TransactionDate.SelectedDate != UnmodifiedTransaction.Date || TxtStore.Text != UnmodifiedTransaction.Store || TxtOctane.Text != UnmodifiedTransaction.Octane.ToString() || TxtDistance.Text != UnmodifiedTransaction.DistanceToString || TxtDistance.Text != UnmodifiedTransaction.DistanceToString || TxtOdometer.Text != UnmodifiedTransaction.OdometerToString || TxtGallons.Text != UnmodifiedTransaction.GallonsToString || TxtPrice.Text != UnmodifiedTransaction.PriceToString);
            BtnSave.IsEnabled = enabled;
            BtnReset.IsEnabled = enabled;
        }

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
        private void ClosePage() => AppState.GoBack();

        public ModifyTransactionPage()
        {
            InitializeComponent();
            TxtStore.Focus();
        }

        private void ModifyTransactionPage_OnLoaded(object sender, RoutedEventArgs e) => DisplayOriginalTransaction();

        #endregion Page-Manipulation Methods
    }
}