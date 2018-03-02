using Extensions;
using Extensions.DataTypeHelpers;
using Extensions.Enums;
using PersonalTracker.Models;
using PersonalTracker.Models.FuelModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PersonalTracker.Views.FuelViews.Vehicles
{
    /// <summary>Interaction logic for AddVehiclePage.xaml</summary>
    public partial class AddVehiclePage
    {
        /// <summary>Resets all TextBoxes to no text.</summary>
        private void Reset()
        {
            TxtNickname.Text = "";
            TxtMake.Text = "";
            TxtModel.Text = "";
            TxtYear.Text = "";
        }

        #region Button-Click Methods

        private async void BtnAddVehicle_Click(object sender, RoutedEventArgs e)
        {
            Vehicle newVehicle = new Vehicle(await AppState.GetNextVehicleIndex(), TxtNickname.Text, TxtMake.Text, TxtModel.Text, Int32Helper.Parse(TxtYear.Text), new List<FuelTransaction>());
            if (await AppState.NewVehicle(newVehicle))
            {
                AppState.CurrentUser.AddVehicle(newVehicle);
                ClosePage();
            }
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e) => Reset();

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => ClosePage();

        #endregion Button-Click Methods

        #region Text Manipulation

        private void Integer_TextChanged(object sender, TextChangedEventArgs e)
        {
            Functions.TextBoxTextChanged(sender, KeyType.Integers);
            TextChanged();
        }

        /// <summary>Handles all the TextBoxes Txt_TextChanged events.</summary>
        private void Txt_TextChanged(object sender, TextChangedEventArgs e) => TextChanged();

        private void TextChanged()
        {
            BtnAddVehicle.IsEnabled = TxtNickname.Text.Length > 0 && TxtMake.Text.Length > 0 &&
                                      TxtModel.Text.Length > 0 && TxtYear.Text.Length == 4;
            BtnReset.IsEnabled = TxtNickname.Text.Length > 0 || TxtMake.Text.Length > 0 || TxtModel.Text.Length > 0 ||
                                 TxtYear.Text.Length > 0;
        }

        private void Txt_GotFocus(object sender, RoutedEventArgs e) => Functions.TextBoxGotFocus(sender);

        private void TxtYear_PreviewKeyDown(object sender, KeyEventArgs e) => Functions.PreviewKeyDown(e,
            KeyType.Integers);

        #endregion Text Manipulation

        #region Page Manipulation

        /// <summary>Closes the Page.</summary>
        private void ClosePage() => AppState.GoBack();

        public AddVehiclePage()
        {
            InitializeComponent();
            TxtNickname.Focus();
        }

        private void AddVehiclePage_OnLoaded(object sender, RoutedEventArgs e) => AppState.CalculateScale(Grid);

        #endregion Page Manipulation
    }
}