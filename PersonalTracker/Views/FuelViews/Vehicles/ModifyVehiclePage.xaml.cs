using Extensions;
using Extensions.DataTypeHelpers;
using Extensions.Enums;
using PersonalTracker.Models;
using PersonalTracker.Models.FuelModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PersonalTracker.Views.FuelViews.Vehicles
{
    /// <summary>Interaction logic for ModifyVehiclePage.xaml</summary>
    public partial class ModifyVehiclePage
    {
        internal Vehicle UnmodifiedVehicle, ModifiedVehicle;

        /// <summary>Resets all TextBoxes to no text.</summary>
        internal void DisplayOriginalVehicle()
        {
            TxtNickname.Text = UnmodifiedVehicle.Nickname;
            TxtMake.Text = UnmodifiedVehicle.Make;
            TxtModel.Text = UnmodifiedVehicle.Model;
            TxtYear.Text = UnmodifiedVehicle.Year.ToString();
        }

        #region Button-Click Methods

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            ModifiedVehicle = new Vehicle(UnmodifiedVehicle.VehicleID, TxtNickname.Text, TxtMake.Text, TxtModel.Text, Int32Helper.Parse(TxtYear.Text), UnmodifiedVehicle.Transactions);
            if (await AppState.ModifyVehicle(UnmodifiedVehicle, ModifiedVehicle))
            {
                AppState.CurrentUser.ModifyVehicle(UnmodifiedVehicle, ModifiedVehicle);
                ClosePage();
            }
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e) => DisplayOriginalVehicle();

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => ClosePage();

        #endregion Button-Click Methods

        #region Text Manipulation

        /// <summary>Handles all the TextBoxes TextChanged events.</summary>
        /// <param name="sender">TextBox whose text has changed</param>
        /// <param name="e">Event</param>
        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            bool enabled = TxtNickname.Text.Length > 0 && TxtMake.Text.Length > 0 && TxtModel.Text.Length > 0 && TxtYear.Text.Length == 4 && (TxtNickname.Text != UnmodifiedVehicle.Nickname || TxtMake.Text != UnmodifiedVehicle.Make || TxtModel.Text != UnmodifiedVehicle.Model || TxtYear.Text != UnmodifiedVehicle.Year.ToString());
            BtnSave.IsEnabled = enabled;
            BtnReset.IsEnabled = enabled;
        }

        private void Txt_GotFocus(object sender, RoutedEventArgs e) => Functions.TextBoxGotFocus(sender);

        private void TxtYear_PreviewKeyDown(object sender, KeyEventArgs e) =>
            Functions.PreviewKeyDown(e, KeyType.Integers);

        #endregion Text Manipulation

        #region Page Manipulation

        /// <summary>Closes the Page.</summary>
        private void ClosePage() => AppState.GoBack();

        public ModifyVehiclePage()
        {
            InitializeComponent();
            TxtNickname.Focus();
        }

        private void ModifyVehiclePage_OnLoaded(object sender, RoutedEventArgs e)
        {
            DisplayOriginalVehicle();
            AppState.CalculateScale(Grid);
        }

        #endregion Page Manipulation
    }
}