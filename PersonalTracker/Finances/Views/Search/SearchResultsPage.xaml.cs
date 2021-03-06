﻿using Extensions;
using Extensions.ListViewHelp;
using PersonalTracker.Finances.Models.Data;
using PersonalTracker.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace PersonalTracker.Finances.Views.Search
{
    /// <summary>Interaction logic for SearchResultsWindow.xaml</summary>
    public partial class SearchResultsPage : INotifyPropertyChanged
    {
        private List<FinancialTransaction> _allTransactions;
        private ListViewSort _sort = new ListViewSort();

        public string TransactionCount => $"Transaction Count: {_allTransactions.Count}";

        #region Data-Binding

        /// <summary>Event that fires if a Property value has changed so that the UI can properly be updated.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Invokes <see cref="PropertyChangedEventHandler"/> to update the UI when a Property value changes.</summary>
        /// <param name="property">Name of Property whose value has changed</param>
        private void NotifyPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Data-Binding

        internal void LoadWindow(List<FinancialTransaction> matchingTransactions)
        {
            _allTransactions = matchingTransactions.OrderByDescending(transaction => transaction.Date)
                .ThenByDescending(transaction => transaction.ID).ToList();
            LVTransactions.ItemsSource = _allTransactions;
        }

        #region Click

        private void BtnBack_Click(object sender, RoutedEventArgs e) => ClosePage();

        private void LVTransactionsColumnHeader_Click(object sender, RoutedEventArgs e) => _sort =
            Functions.ListViewColumnHeaderClick(sender, _sort, LVTransactions, "#CCCCCC");

        #endregion Click

        #region Page-Manipulation Methods

        /// <summary>Closes the Page.</summary>
        private void ClosePage() => AppState.GoBack();

        public SearchResultsPage() => InitializeComponent();

        #endregion Page-Manipulation Methods
    }
}