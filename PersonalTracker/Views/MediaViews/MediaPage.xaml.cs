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

        private void BtnBooks_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BtnFilms_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BtnMusic_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BtnTelevision_Click(object sender, RoutedEventArgs e) => AppState.Navigate(new TelevisionPage());

        #endregion Click

        public MediaPage() => InitializeComponent();
    }
}