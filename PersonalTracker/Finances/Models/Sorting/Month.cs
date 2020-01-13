using PersonalTracker.Finances.Models.Data;
using PersonalTracker.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PersonalTracker.Finances.Models.Sorting
{
    /// <summary>Represents a month to help determine income and expenses of transactions.</summary>
    public class Month : BaseINPC
    {
        private DateTime _monthStart;
        private List<FinancialTransaction> _allTransactions = new List<FinancialTransaction>();

        #region Modifying Properties

        /// <summary>First day of the month</summary>
        public DateTime MonthStart
        {
            get => _monthStart;
            private set
            {
                _monthStart = value;
                NotifyPropertyChanged(nameof(MonthStart));
            }
        }

        #endregion Modifying Properties

        #region Helper Properties

        /// <summary>Collection of all the transactions that occurred in the month</summary>
        public ReadOnlyCollection<FinancialTransaction> AllTransactions => new ReadOnlyCollection<FinancialTransaction>(
            _allTransactions);

        /// <summary>Income for this month</summary>
        public decimal Income => AllTransactions.Where(transaction => transaction.MajorCategory != "Transfer").Sum(transaction => transaction.Inflow);

        /// <summary>Income for this month, formatted to currency</summary>
        public string IncomeToString => Income.ToString("C2");

        /// <summary>Income for this month, formatted to currency, with preceding text</summary>
        public string IncomeToStringWithText => $"Income: {Income:C2}";

        /// <summary>Expenses for this month</summary>
        public decimal Expenses => AllTransactions.Where(transaction => transaction.MajorCategory != "Transfer").Sum(transaction => transaction.Outflow);

        /// <summary>Expenses for this month, formatted to currency</summary>
        public string ExpensesToString => Expenses.ToString("C2");

        /// <summary>Expenses for this month, formatted to currency, with preceding text</summary>
        public string ExpensesToStringWithText => $"Expenses: {Expenses:C2}";

        /// <summary>Last day of the month</summary>
        public DateTime MonthEnd => new DateTime(MonthStart.Year, MonthStart.Month,
            DateTime.DaysInMonth(MonthStart.Year, MonthStart.Month));

        /// <summary>Formatted text representing the year and month</summary>
        public string FormattedMonth => MonthStart.ToString("yyyy/MM");

        #endregion Helper Properties

        #region Transaction Management

        /// <summary>Adds a transaction to this month.</summary>
        /// <param name="transaction">Transaction to be added</param>
        public void AddTransaction(FinancialTransaction transaction)
        {
            _allTransactions.Add(transaction);
            Sort();
            NotifyPropertyChanged(nameof(AllTransactions));
        }

        /// <summary>Modifies a transaction in this account.</summary>
        /// <param name="index">Index of transaction to be modified</param>
        /// <param name="transaction">Transaction to replace current in list</param>
        public void ModifyTransaction(int index, FinancialTransaction transaction) => _allTransactions[index] = transaction;

        /// <summary>Removes a transaction from this month.</summary>
        /// <param name="transaction">Transaction to be added</param>
        public void RemoveTransaction(FinancialTransaction transaction)
        {
            _allTransactions.Remove(transaction);
            NotifyPropertyChanged(nameof(AllTransactions));
        }

        #endregion Transaction Management

        /// <summary>Sorts the List by date, newest to oldest.</summary>
        private void Sort() => _allTransactions = _allTransactions.OrderByDescending(transaction => transaction.Date)
            .ThenByDescending(transaction => transaction.ID).ToList();

        #region Override Operators

        private static bool Equals(Month left, Month right)
        {
            if (left is null && right is null) return true;
            if (left is null ^ right is null) return false;
            return left.MonthStart == right.MonthStart && left.Income == right.Income && left.Expenses == right.Expenses;
        }

        public sealed override bool Equals(object obj) => Equals(this, obj as Month);

        public bool Equals(Month otherMonth) => Equals(this, otherMonth);

        public static bool operator ==(Month left, Month right) => Equals(left, right);

        public static bool operator !=(Month left, Month right) => !Equals(left, right);

        public sealed override int GetHashCode() => base.GetHashCode() ^ 17;

        public sealed override string ToString() => FormattedMonth;

        #endregion Override Operators

        #region Constructors

        /// <summary>Initializes a default instance of Month.</summary>
        public Month()
        {
        }

        /// <summary>Initializes an instance of Month by assigning Properties.</summary>
        /// <param name="monthStart">First day of the month</param>
        /// <param name="transactions">Transactions during this month</param>
        public Month(DateTime monthStart, IEnumerable<FinancialTransaction> transactions)
        {
            MonthStart = monthStart;
            List<FinancialTransaction> newTransactions = new List<FinancialTransaction>();
            newTransactions.AddRange(transactions);
            _allTransactions = newTransactions;
        }

        /// <summary>Replaces this instance of Account with another instance</summary>
        /// <param name="other">Month to replace this instance</param>
        public Month(Month other) : this(other.MonthStart, other.AllTransactions)
        {
        }

        #endregion Constructors
    }
}