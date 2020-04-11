using Extensions;
using Extensions.ListViewHelp;
using PersonalTracker.Media.Models.Enums;
using PersonalTracker.Media.Models.MediaTypes;
using PersonalTracker.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PersonalTracker.Media.Views.MediaSeries
{
    /// <summary>Interaction logic for TelevisionPage.xaml</summary>
    public partial class TelevisionPage : Page, INotifyPropertyChanged
    {
        private ListViewSort _sort = new ListViewSort();
        private List<Series> _series = new List<Series>();
        private Series _selectedSeries = new Series();

        #region Data-Binding

        /// <summary>Event that fires if a Property value has changed so that the UI can properly be updated.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Invokes <see cref="PropertyChangedEventHandler"/> to update the UI when a Property value changes.</summary>
        /// <param name="property">Name of Property whose value has changed</param>
        private void NotifyPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Data-Binding

        /// <summary>Refreshes the ListView's ItemsSource.</summary>
        internal void RefreshItemsSource(List<Series> refreshSeries)
        {
            if (TxtSearch.Text.Length > 0)
            {
                _series = ChkAll.IsChecked.Value ? AppState.CurrentUser.Media.AllSeries.ToList() : refreshSeries;
                _series = _series.FindAll(series => series.Name.IndexOf(TxtSearch.Text, StringComparison.InvariantCultureIgnoreCase) >= 0).OrderBy(series => series.Name).ToList();
            }
            else
                _series = refreshSeries;

            LVTelevision.ItemsSource = _series;
            LVTelevision.Items.Refresh();
        }

        #region Filter & Search

        private void Rad_Click(object sender, RoutedEventArgs e) => CheckRadio();

        private void CheckRadio()
        {
            if (RadAll.IsChecked.Value)
                RefreshItemsSource(AppState.CurrentUser.Media.AllSeries.ToList().OrderBy(series => series.Name).ToList());
            else if (RadAiring.IsChecked.Value)
                RefreshItemsSource(_series = AppState.CurrentUser.Media.AllSeries.ToList().FindAll(series => series.Status == SeriesStatus.Airing).OrderBy(series => series.Day).ThenBy(series => series.Time).ThenBy(series => series.Name).ToList());
            else if (RadEnded.IsChecked.Value)
                RefreshItemsSource(_series = AppState.CurrentUser.Media.AllSeries.ToList().FindAll(series => series.Status == SeriesStatus.Ended).OrderBy(series => series.Name).ToList());
            else if (RadHiatus.IsChecked.Value)
                RefreshItemsSource(_series = AppState.CurrentUser.Media.AllSeries.ToList().FindAll(series => series.Status == SeriesStatus.Hiatus).OrderBy(series => series.Name).ToList());
            else if (RadNope.IsChecked.Value)
                RefreshItemsSource(_series = AppState.CurrentUser.Media.AllSeries.ToList().FindAll(series => series.Status == SeriesStatus.Nope).OrderBy(series => series.Name).ToList());
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtSearch.Text.Length == 0)
                CheckRadio();
        }

        #endregion Filter & Search

        #region Click

        private void LVTelevision_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedSeries = LVTelevision.SelectedIndex >= 0 ? (Series)LVTelevision.SelectedItem : new Series();
            BtnMoreInfo.IsEnabled = LVTelevision.SelectedIndex >= 0;
        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e) => _sort = Functions.ListViewColumnHeaderClick(sender, _sort, LVTelevision, "#CCCCCC");

        private void BtnSearch_Click(object sender, RoutedEventArgs e) => CheckRadio();

        private void BtnAddSeries_Click(object sender, RoutedEventArgs e) => AppState.Navigate(new NewSeriesPage());

        private void BtnMoreInfo_Click(object sender, RoutedEventArgs e) => AppState.Navigate(new ModifySeriesPage { SelectedSeries = _selectedSeries });

        private void BtnBack_Click(object sender, RoutedEventArgs e) => AppState.GoBack();

        #endregion Click

        #region Page-Manipulation Methods

        public TelevisionPage()
        {
            InitializeComponent();
            LVTelevision.ItemsSource = _series;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) => CheckRadio();

        #endregion Page-Manipulation Methods
    }
}