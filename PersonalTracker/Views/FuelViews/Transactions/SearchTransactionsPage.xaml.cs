using PersonalTracker.Models;
using System.Windows;

namespace PersonalTracker.Views.FuelViews.Transactions
{
    /// <summary> Interaction logic for SearchTransactionsPage.xaml </summary>
    public partial class SearchTransactionsPage
    {
        public SearchTransactionsPage() => InitializeComponent();

        private void SearchTransactionsPage_OnLoaded(object sender, RoutedEventArgs e) => AppState.CalculateScale(Grid);
    }
}