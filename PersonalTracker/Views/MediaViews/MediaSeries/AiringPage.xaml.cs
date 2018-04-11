using Extensions;
using Extensions.ListViewHelp;
using PersonalTracker.Models;
using PersonalTracker.Models.MediaModels.Enums;
using PersonalTracker.Models.MediaModels.MediaTypes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PersonalTracker.Views.MediaViews.MediaSeries
{
    /// <summary>Interaction logic for AiringPage.xaml</summary>
    public partial class AiringPage : INotifyPropertyChanged
    {
        private ListViewSort _sort = new ListViewSort();
        private List<Series> _series = new List<Series>();
        private Series _selectedSeries = new Series();

        #region Data-Binding

        /// <summary>Event that fires if a Property value has changed so that the UI can properly be updated.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Invokes <see cref="PropertyChangedEventHandler"/> to update the UI when a Property value changes.</summary>
        /// <param name="property">Name of Property whose value has changed</param>
        private void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Data-Binding

        /// <summary>Refreshes the ListView's ItemsSource.</summary>
        internal void RefreshItemsSource()
        {
            _series = AppState.CurrentUser.Media.AllSeries.ToList().FindAll(series => series.Status == SeriesStatus.Airing)
              .OrderBy(series => series.Day).ThenBy(series => series.Time).ThenBy(series => series.Name).ToList();
            LVSeries.ItemsSource = _series;
            LVSeries.Items.Refresh();
            DataContext = _selectedSeries;
        }

        /// <summary>Toggles Buttons on the Page.</summary>
        /// <param name="toggle">Toggle</param>
        private void ToggleButtons(bool toggle)
        {
            BtnModifySeries.IsEnabled = toggle;
            BtnDeleteSeries.IsEnabled = toggle;
        }

        #region Click Methods

        private void LVSeriesColumnHeader_Click(object sender, RoutedEventArgs e) => _sort = Functions.ListViewColumnHeaderClick(sender, _sort, LVSeries, "#CCCCCC");

        private void BtnNewSeries_Click(object sender, RoutedEventArgs e) => AppState.Navigate(new NewSeriesPage());

        private void BtnModifySeries_Click(object sender, RoutedEventArgs e) => AppState.Navigate(new ModifySeriesPage { SelectedSeries = _selectedSeries });

        private async void BtnDeleteSeries_Click(object sender, RoutedEventArgs e)
        {
            if (await AppState.DeleteSeries(_selectedSeries))
                RefreshItemsSource();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e) => ClosePage();

        #endregion Click Methods

        #region Window-Manipulation Methods

        /// <summary>Closes the Page.</summary>
        private void ClosePage() => AppState.GoBack();

        public AiringPage()
        {
            InitializeComponent();
            LVSeries.ItemsSource = _series;
        }

        private void LVSeries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedSeries = LVSeries.SelectedIndex >= 0 ? (Series)LVSeries.SelectedItem : new Series();

            ToggleButtons(LVSeries.SelectedIndex >= 0);
            DataContext = _selectedSeries;
        }

        private void AiringPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            AppState.CalculateScale(Grid);
            RefreshItemsSource();
        }

        #endregion Window-Manipulation Methods
    }
}