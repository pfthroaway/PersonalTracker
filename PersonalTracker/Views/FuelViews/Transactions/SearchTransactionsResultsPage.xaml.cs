using PersonalTracker.Models;
using System.Windows;

namespace PersonalTracker.Views.FuelViews.Transactions
{
    /// <summary>Interaction logic for SearchTransactionsResultsPage.xaml</summary>
    public partial class SearchTransactionsResultsPage
    {
        public SearchTransactionsResultsPage() => InitializeComponent();

        private void SearchTransactionsResultsPage_OnLoaded(object sender, RoutedEventArgs e) => AppState.CalculateScale(Grid);
    }
}