using PersonalTracker.Finances.Models.Categories;
using PersonalTracker.Finances.Models.Data;
using PersonalTracker.Finances.Models.Sorting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace PersonalTracker.Finances.Models
{
    /// <summary>Represents a collection of a <see cref="User"/>'s financial <see cref="Account"/>s and <see cref="FinancialTransaction"/>s.</summary>
    public class AllFinances : INotifyPropertyChanged
    {
        private List<Account> _allAccounts = new List<Account>();
        private List<string> _allAccountTypes = new List<string>();
        private List<Category> _allCategories = new List<Category>();
        private List<FinancialTransaction> _allTransactions = new List<FinancialTransaction>();
        private List<Month> _allMonths = new List<Month>();
        private List<Year> _allYears = new List<Year>();

        /// <summary>List of <see cref="Account"/>s owned by a <see cref="User"/>.</summary>
        public List<Account> AllAccounts
        {
            get => _allAccounts;
            set
            {
                _allAccounts = value;
                OnPropertyChanged("AllAccounts");
            }
        }

        /// <summary>List of <see cref="Account"/> types owned by a <see cref="User"/>.</summary>
        public List<string> AllAccountTypes
        {
            get => _allAccountTypes;
            set
            {
                _allAccountTypes = value;
                OnPropertyChanged("AllAccounts");
            }
        }

        /// <summary>List of a <see cref="User"/>'s <see cref="Category"/>s.</summary>
        public List<Category> AllCategories
        {
            get => _allCategories;
            set
            {
                _allCategories = value;
                OnPropertyChanged("AllAccounts");
            }
        }

        /// <summary>List of a <see cref="User"/>'s <see cref="FinancialTransaction"/>s.</summary>
        public List<FinancialTransaction> AllTransactions
        {
            get => _allTransactions;
            set
            {
                _allTransactions = value;
                OnPropertyChanged("AllAccounts");
            }
        }

        /// <summary>List of <see cref="Month"/>s of <see cref="FinancialTransaction"/>s.</summary>
        public List<Month> AllMonths
        {
            get => _allMonths;
            set
            {
                _allMonths = value;
                OnPropertyChanged("AllAccounts");
            }
        }

        /// <summary>List of <see cref="Year"/>s of <see cref="FinancialTransaction"/>s.</summary>
        public List<Year> AllYears
        {
            get => _allYears;
            set
            {
                _allYears = value;
                OnPropertyChanged("AllAccounts");
            }
        }

        #region Data-Binding

        /// <summary>Event that fires if a Property value has changed so that the UI can properly be updated.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Invokes <see cref="PropertyChangedEventHandler"/> to update the UI when a Property value changes.</summary>
        /// <param name="property">Name of Property whose value has changed</param>
        private void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Data-Binding

        #region Load

        /// <summary>Loads all the <see cref="Month"/>s from AllTransactions.</summary>
        public void LoadMonths()
        {
            AllMonths.Clear();

            if (AllTransactions.Count > 0)
            {
                int months = ((DateTime.Now.Year - AllTransactions[AllTransactions.Count - 1].Date.Year) * 12) + DateTime.Now.Month - AllTransactions[AllTransactions.Count - 1].Date.Month;
                DateTime startMonth = new DateTime(AllTransactions[AllTransactions.Count - 1].Date.Year, AllTransactions[AllTransactions.Count - 1].Date.Month, 1);

                int start = 0;
                do
                {
                    AllMonths.Add(new Month(startMonth.AddMonths(start), new List<FinancialTransaction>()));
                    start++;
                }
                while (start <= months);

                foreach (FinancialTransaction transaction in AllTransactions)
                {
                    AllMonths.Find(month => month.MonthStart <= transaction.Date && transaction.Date <= month.MonthEnd.Date).AddTransaction(transaction);
                }

                AllMonths = AllMonths.OrderByDescending(month => month.FormattedMonth).ToList();
            }
        }

        /// <summary>Loads all the <see cref="Year"/>s from AllTransactions.</summary>
        public void LoadYears()
        {
            AllYears.Clear();

            if (AllTransactions.Count > 0)
            {
                int years = (DateTime.Now.Year - AllTransactions[AllTransactions.Count - 1].Date.Year);
                DateTime startYear = new DateTime(AllTransactions[AllTransactions.Count - 1].Date.Year, 1, 1);

                int start = 0;
                do
                {
                    AllYears.Add(new Year(startYear.AddYears(start), new List<FinancialTransaction>()));
                    start++;
                }
                while (start <= years);

                foreach (FinancialTransaction transaction in AllTransactions)
                {
                    AllYears.Find(year => year.YearStart <= transaction.Date && transaction.Date <= year.YearEnd.Date).AddTransaction(transaction);
                }

                AllYears = AllYears.OrderByDescending(year => year.FormattedYear).ToList();
            }
        }

        #endregion Load

        #region Constructors

        /// <summary>Initializes an instance of <see cref="AllFinances"/> by assigning values to Properties.</summary>
        /// <param name="allAccounts">All <see cref="Account"/>s</param>
        /// <param name="allAccountTypes">All <see cref="Account"/> types</param>
        /// <param name="allCategories">All <see cref="Category"/>s</param>
        public AllFinances(List<Account> allAccounts, List<string> allAccountTypes, List<Category> allCategories)
        {
            AllAccounts = new List<Account>(allAccounts);
            AllAccountTypes = new List<string>(allAccountTypes);
            AllCategories = new List<Category>(allCategories);

            AllTransactions = new List<FinancialTransaction>();
            foreach (Account account in AllAccounts)
                foreach (FinancialTransaction trans in account.AllTransactions)
                    AllTransactions.Add(trans);

            AllAccountTypes.Sort();
            AllTransactions = AllTransactions.OrderByDescending(transaction => transaction.Date).ThenByDescending(transaction => transaction.ID).ToList();
            LoadMonths();
            LoadYears();
        }

        /// <summary>Replaces this instance of <see cref="AllFinances"/> with another instance.</summary>
        /// <param name="other">Instance of <see cref="AllFinances"/> to replace this instance</param>
        public AllFinances(AllFinances other) : this(other.AllAccounts, other.AllAccountTypes, other.AllCategories)
        {
        }

        #endregion Constructors
    }
}