using PersonalTracker.Models;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PersonalTracker.Media.Views.MediaBooks
{
    /// <summary>Interaction logic for NewBookPage.xaml</summary>
    public partial class NewBookPage : Page
    {
        private void Reset()
        {
        }

        /// <summary>Saves the current <see cref="Book"/>.</summary>
        private async Task Save()
        {
            //Series newSeries = new Series(TxtName.Text.Trim(), DateTimeHelper.Parse(DatePremiere.SelectedDate), DecimalHelper.Parse(TxtRating.Text.Trim()), Int32Helper.Parse(TxtSeasons.Text.Trim()), Int32Helper.Parse(TxtEpisodes.Text.Trim()), (SeriesStatus)CmbStatus.SelectedIndex, TxtChannel.Text.Trim(), DateTimeHelper.Parse(DateFinale.SelectedDate), CmbDay.SelectedIndex >= 0 ? (DayOfWeek)CmbDay.SelectedIndex : DayOfWeek.Sunday, DateTimeHelper.Parse(TxtTime.Text.Trim()), TxtReturnDate.Text.Trim());
            //await AppState.NewSeries(newSeries);
        }

        #region Click

        private void BtnBack_Click(object sender, RoutedEventArgs e) => AppState.GoBack();

        private void BtnReset_Click(object sender, RoutedEventArgs e) => Reset();

        private async void BtnSaveNew_Click(object sender, RoutedEventArgs e)
        {
            await Save();
            Reset();
        }

        private async void BtnSaveExit_Click(object sender, RoutedEventArgs e)
        {
            await Save();
            AppState.GoBack();
        }

        #endregion Click

        #region Page-Manipulation Methods

        public NewBookPage() => InitializeComponent();

        #endregion Page-Manipulation Methods
    }
}