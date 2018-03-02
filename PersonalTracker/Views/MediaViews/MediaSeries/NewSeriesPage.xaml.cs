using Extensions;
using Extensions.DataTypeHelpers;
using Extensions.Enums;
using PersonalTracker.Models;
using PersonalTracker.Models.MediaModels.Enums;
using PersonalTracker.Models.MediaModels.MediaTypes;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PersonalTracker.Views.MediaViews.MediaSeries
{
    /// <summary>Interaction logic for NewSeriesPage.xaml</summary>
    public partial class NewSeriesPage
    {
        /// <summary>Checks whether the Save buttons should be enabled.</summary>
        private void CheckButtons()
        {
            bool enabled = TxtName.Text.Length > 0 && DatePremiere.Text.Length > 0 && TxtRating.Text.Length > 0 && TxtSeasons.Text.Length > 0 && TxtEpisodes.Text.Length > 0 && CmbStatus.SelectedIndex >= 0;
            BtnSaveExit.IsEnabled = enabled;
            BtnSaveNew.IsEnabled = enabled;
        }

        /// <summary>Resets all controls to empty.</summary>
        private void Reset()
        {
            TxtName.Text = "";
            DatePremiere.Text = "";
            TxtRating.Text = "";
            TxtSeasons.Text = "";
            TxtEpisodes.Text = "";
            CmbStatus.SelectedIndex = -1;
            TxtChannel.Text = "";
            DateFinale.Text = "";
            CmbDay.SelectedIndex = -1;
            TxtTime.Text = "";
            TxtReturnDate.Text = "";
        }

        /// <summary>Saves the current series.</summary>
        private async Task Save()
        {
            Series newSeries = new Series(TxtName.Text.Trim(), DateTimeHelper.Parse(DatePremiere.SelectedDate), DecimalHelper.Parse(TxtRating.Text.Trim()), Int32Helper.Parse(TxtSeasons.Text.Trim()), Int32Helper.Parse(TxtEpisodes.Text.Trim()), (SeriesStatus)CmbStatus.SelectedIndex, TxtChannel.Text.Trim(), DateTimeHelper.Parse(DateFinale.SelectedDate), CmbDay.SelectedIndex >= 0 ? (DayOfWeek)CmbDay.SelectedIndex : DayOfWeek.Sunday, DateTimeHelper.Parse(TxtTime.Text.Trim()), TxtReturnDate.Text.Trim());
            await AppState.NewSeries(newSeries);
        }

        #region Text/Selection Changed

        private void TxtTextChanged(object sender, TextChangedEventArgs e) => CheckButtons();

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => CheckButtons();

        private void DecimalTextChanged(object sender, TextChangedEventArgs e)
        {
            Functions.TextBoxTextChanged(sender, KeyType.Decimals);
            CheckButtons();
        }

        private void IntTextChanged(object sender, TextChangedEventArgs e)
        {
            Functions.TextBoxTextChanged(sender, KeyType.Integers);
            CheckButtons();
        }

        private void CmbSelectionChanged(object sender, SelectionChangedEventArgs e) => CheckButtons();

        #endregion Text/Selection Changed

        #region Click

        private void BtnBack_Click(object sender, RoutedEventArgs e) => ClosePage();

        private void BtnReset_Click(object sender, RoutedEventArgs e) => Reset();

        private async void BtnSaveNew_Click(object sender, RoutedEventArgs e)
        {
            await Save();
            Reset();
        }

        private async void BtnSaveExit_Click(object sender, RoutedEventArgs e)
        {
            await Save();
            ClosePage();
        }

        #endregion Click

        #region PreviewKeyDown

        private void Decimal_PreviewKeyDown(object sender, KeyEventArgs e) => Functions.PreviewKeyDown(e, KeyType.Decimals);

        private void Integer_PreviewKeyDown(object sender, KeyEventArgs e) => Functions.PreviewKeyDown(e, KeyType.Integers);

        private void Date_PreviewKeyDown(object sender, KeyEventArgs e)
        {
        }

        #endregion PreviewKeyDown

        #region GotFocus

        private void Txt_GotFocus(object sender, RoutedEventArgs e) => Functions.TextBoxGotFocus(sender);

        #endregion GotFocus

        #region Page-Manipulation Methods

        /// <summary>Closes the Page.</summary>
        private void ClosePage() => AppState.GoBack();

        /// <summary>Initializes a new instance of NewSeriesPage.</summary>
        public NewSeriesPage()
        {
            InitializeComponent();
            TxtName.Focus();
        }

        private void NewSeriesPage_OnLoaded(object sender, RoutedEventArgs e) => AppState.CalculateScale(Grid);

        #endregion Page-Manipulation Methods
    }
}