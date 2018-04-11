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
    /// <summary>Interaction logic for ModifySeriesPage.xaml</summary>
    public partial class ModifySeriesPage
    {
        /// <summary>The <see cref="Series"/> that is currently being modified.</summary>
        internal Series SelectedSeries { get; set; }

        /// <summary>Checks whether the Save buttons should be enabled.</summary>
        private void CheckButton() => BtnSave.IsEnabled = TxtName.Text.Length > 0 && DatePremiere.Text.Length > 0 && TxtRating.Text.Length > 0 && TxtSeasons.Text.Length > 0 && TxtEpisodes.Text.Length > 0 && CmbStatus.SelectedIndex >= 0 && (TxtName.Text != SelectedSeries.Name || DatePremiere.Text != SelectedSeries.PremiereDateToString || DecimalHelper.Parse(TxtRating.Text) != SelectedSeries.Rating || Int32Helper.Parse(TxtSeasons.Text) != SelectedSeries.Seasons || Int32Helper.Parse(TxtEpisodes.Text) != SelectedSeries.Episodes || (SeriesStatus)CmbStatus.SelectedIndex != SelectedSeries.Status);

        /// <summary>Resets all controls to original <see cref="Series"/> values.</summary>
        private void Reset()
        {
            TxtName.Text = SelectedSeries.Name;
            DatePremiere.Text = SelectedSeries.PremiereDateToString;
            TxtRating.Text = SelectedSeries.Rating.ToString();
            TxtSeasons.Text = SelectedSeries.Seasons.ToString();
            TxtEpisodes.Text = SelectedSeries.Episodes.ToString();
            CmbStatus.SelectedItem = SelectedSeries.Status.ToString();
            TxtChannel.Text = SelectedSeries.Channel;
            DateFinale.Text = SelectedSeries.FinaleDateToString;
            CmbDay.SelectedItem = SelectedSeries.Day.ToString();
            TxtTime.Text = SelectedSeries.TimeToString;
            TxtReturnDate.Text = SelectedSeries.ReturnDate;
        }

        /// <summary>Saves the current series.</summary>
        private async Task Save()
        {
            Series newSeries = new Series(TxtName.Text.Trim(), DateTimeHelper.Parse(DatePremiere.SelectedDate), DecimalHelper.Parse(TxtRating.Text.Trim()), Int32Helper.Parse(TxtSeasons.Text.Trim()), Int32Helper.Parse(TxtEpisodes.Text.Trim()), (SeriesStatus)CmbStatus.SelectedIndex, TxtChannel.Text.Trim(), DateTimeHelper.Parse(DateFinale.SelectedDate), CmbDay.SelectedIndex >= 0 ? (DayOfWeek)CmbDay.SelectedIndex : DayOfWeek.Sunday, DateTimeHelper.Parse(TxtTime.Text.Trim()), TxtReturnDate.Text.Trim());
            await AppState.ModifySeries(SelectedSeries, newSeries);
        }

        #region Text/Selection Changed

        private void TxtTextChanged(object sender, TextChangedEventArgs e) => CheckButton();

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => CheckButton();

        private void DecimalTextChanged(object sender, TextChangedEventArgs e)
        {
            Functions.TextBoxTextChanged(sender, KeyType.Decimals);
            CheckButton();
        }

        private void IntTextChanged(object sender, TextChangedEventArgs e)
        {
            Functions.TextBoxTextChanged(sender, KeyType.Integers);
            CheckButton();
        }

        private void CmbSelectionChanged(object sender, SelectionChangedEventArgs e) => CheckButton();

        #endregion Text/Selection Changed

        #region Click

        private void BtnBack_Click(object sender, RoutedEventArgs e) => ClosePage();

        private void BtnReset_Click(object sender, RoutedEventArgs e) => Reset();

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            await Save();
            ClosePage();
        }

        #endregion Click

        #region PreviewKeyDown

        private void Decimal_PreviewKeyDown(object sender, KeyEventArgs e) => Functions.PreviewKeyDown(e, KeyType.Decimals);

        private void Integer_PreviewKeyDown(object sender, KeyEventArgs e) => Functions.PreviewKeyDown(e, KeyType.Integers);

        #endregion PreviewKeyDown

        #region GotFocus

        private void Txt_GotFocus(object sender, RoutedEventArgs e) => Functions.TextBoxGotFocus(sender);

        #endregion GotFocus

        #region Window-Manipulation Methods

        /// <summary>Closes the Page.</summary>
        private void ClosePage() => AppState.GoBack();

        public ModifySeriesPage()
        {
            InitializeComponent();
            TxtName.Focus();
            CmbStatus.Items.Add("Airing");
            CmbStatus.Items.Add("Ended");
            CmbStatus.Items.Add("Hiatus");
            CmbStatus.Items.Add("Nope");
            CmbDay.Items.Add("Sunday");
            CmbDay.Items.Add("Monday");
            CmbDay.Items.Add("Tuesday");
            CmbDay.Items.Add("Wednesday");
            CmbDay.Items.Add("Thursday");
            CmbDay.Items.Add("Friday");
            CmbDay.Items.Add("Saturday");
        }

        private void ModifySeriesPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            AppState.CalculateScale(Grid);
            Reset();
        }

        #endregion Window-Manipulation Methods
    }
}