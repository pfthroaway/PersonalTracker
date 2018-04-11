using PersonalTracker.Models;
using PersonalTracker.Views.MediaViews.MediaSeries;
using System.Windows;

namespace PersonalTracker.Views.MediaViews
{
    /// <summary> Interaction logic for MediaPage.xaml </summary>
    public partial class MediaPage
    {
        //TODO Implement other types of Media: film, books, and music.
        //TODO Make Media Tracker more easily navigated.

        #region Click

        private void BtnBack_Click(object sender, RoutedEventArgs e) => AppState.GoBack();

        #region Television

        private void BtnTelevisionAll_Click(object sender, RoutedEventArgs e) => AppState.Navigate(new AllTelevisionPage());

        private void BtnTelevisionAiring_Click(object sender, RoutedEventArgs e) => AppState.Navigate(new AiringPage());

        private void BtnTelevisionHiatus_Click(object sender, RoutedEventArgs e) => AppState.Navigate(new HiatusPage());

        private void BtnTelevisionEnded_Click(object sender, RoutedEventArgs e) => AppState.Navigate(new EndedPage());

        private void BtnTelevisionAddNew_Click(object sender, RoutedEventArgs e) => AppState.Navigate(new NewSeriesPage());

        #endregion Television

        #region Film

        private void BtnFilmReleased_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BtnFilmUpcoming_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BtnFilmAddNew_Click(object sender, RoutedEventArgs e)
        {
        }

        #endregion Film

        #endregion Click

        public MediaPage() => InitializeComponent();

        private void MainPage_OnLoaded(object sender, RoutedEventArgs e) => AppState.CalculateScale(Grid);
    }
}