using Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace PersonalTracker.Models.FuelModels
{
    /// <summary>Represents a Vehicle owned by a User.</summary>
    public class Vehicle : INotifyPropertyChanged, IEnumerable<FuelTransaction>, IEquatable<Vehicle>
    {
        private string _nickname, _make, _model;
        private int _vehicleID, _year;
        private List<FuelTransaction> _transactions;

        #region Modifying Properties

        /// <summary>Vehicle ID</summary>
        public int VehicleID
        {
            get => _vehicleID;
            set { _vehicleID = value; OnPropertyChanged("VehicleID"); }
        }

        /// <summary>Vehicle nickname</summary>
        public string Nickname
        {
            get => _nickname;
            set { _nickname = value; OnPropertyChanged("Nickname"); }
        }

        /// <summary>Brand of car</summary>
        public string Make
        {
            get => _make;
            set { _make = value; OnPropertyChanged("Make"); }
        }

        /// <summary>Model of car</summary>
        public string Model
        {
            get => _model;
            set { _model = value; OnPropertyChanged("Model"); }
        }

        /// <summary>Model year of car</summary>
        public int Year
        {
            get => _year;
            set { _year = value; OnPropertyChanged("Year"); }
        }

        #endregion Modifying Properties

        #region Helper Properties

        /// <summary>List of Transactions associated with the current Vehicle.</summary>
        public ReadOnlyCollection<FuelTransaction> Transactions => new ReadOnlyCollection<FuelTransaction>(_transactions);

        /// <summary>Miles per gallon</summary>
        public decimal MPG => Transactions.Count > 0 ? Transactions.Sum(trans => trans.MPG) / Transactions.Count : 0;

        /// <summary>Miles per gallon, formatted with three decimal places.</summary>
        public string MPGToString => MPG.ToString("N3");

        /// <summary>Miles per gallon, formatted with three decimal places, with preceding text.</summary>
        public string MPGToStringWithText => $"MPG: {MPGToString}";

        /// <summary>Total amount of money spent on fuel for this Vehicle.</summary>
        public decimal TotalCost => Transactions.Count > 0 ? Transactions.Sum(trans => trans.TotalPrice) : 0;

        /// <summary>Total amount of money spent on fuel for this Vehicle, formatted.</summary>
        public string TotalCostToString => TotalCost.ToString("C2");

        /// <summary>Total amount of money spent on fuel for this Vehicle, formatted, with text.</summary>
        public string TotalCostToStringWithText => $"Total Cost of Fuel: {TotalCostToString}";

        /// <summary>Average distance per fuel-up for this Vehicle.</summary>
        public decimal AverageDistance => Transactions.Count > 0
            ? Transactions.Sum(trans => trans.Distance) / Transactions.Count
            : 0;

        /// <summary>Average distance per fuel-up for this Vehicle, formatted.</summary>
        public string AverageDistanceToString => AverageDistance.ToString("N2");

        /// <summary>Average distance per fuel-up for this Vehicle, formatted, with text.</summary>
        public string AverageDistanceToStringWithText => $"Average distance between fuel-ups: {AverageDistanceToString}";

        /// <summary>Average gallons per fuel-up for this Vehicle.</summary>
        public decimal AverageGallons => Transactions.Count > 0
            ? Transactions.Sum(trans => trans.Gallons) / Transactions.Count
            : 0;

        /// <summary>Average gallons per fuel-up for this Vehicle, formatted.</summary>
        public string AverageGallonsToString => AverageGallons.ToString("N2");

        /// <summary>Average gallons per fuel-up for this Vehicle, formatted, with text.</summary>
        public string AverageGallonsToStringWithText => $"Average gallons per fuel-up: {AverageGallonsToString}";

        /// <summary>Average price per gallon per fuel-up for this Vehicle.</summary>
        public decimal AveragePrice => Transactions.Count > 0
            ? Transactions.Sum(trans => trans.Price) / Transactions.Count
            : 0;

        /// <summary>Average price per gallon per fuel-up for this Vehicle, formatted.</summary>
        public string AveragePriceToString => AveragePrice.ToString("C2");

        /// <summary>Average price per gallon per fuel-up for this Vehicle, formatted, with text.</summary>
        public string AveragePriceToStringWithText => $"Average price per gallon per fuel-up: {AveragePriceToString}";

        /// <summary>Average total price per fuel-up for this Vehicle.</summary>
        public decimal AverageTotalPrice => Transactions.Count > 0
            ? TotalCost / Transactions.Count
            : 0;

        /// <summary>Average total price per fuel-up for this Vehicle, formatted.</summary>
        public string AverageTotalPriceToString => AverageTotalPrice.ToString("C2");

        /// <summary>Average total price per fuel-up for this Vehicle, formatted, with text.</summary>
        public string AverageTotalPriceToStringWithText => $"Average price per fuel-up: {AverageTotalPriceToString}";

        #endregion Helper Properties

        #region Data-Binding

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this,
            new PropertyChangedEventArgs(property));

        #endregion Data-Binding

        #region Transaction Management

        /// <summary>Adds a Transaction to the list of Transactions.</summary>
        /// <param name="transaction">Transaction to be added</param>
        internal void AddTransaction(FuelTransaction transaction)
        {
            _transactions.Add(transaction);
            Sort();
            UpdateProperties();
        }

        /// <summary>Modifies a Transaction in the list of Transactions.</summary>
        /// <param name="oldTransaction">Original Transaction to be replaced</param>
        /// <param name="newTransaction">New Transaction to replace old</param>
        internal void ModifyTransaction(FuelTransaction oldTransaction, FuelTransaction newTransaction)
        {
            _transactions.Replace(oldTransaction, newTransaction);
            Sort();
            UpdateProperties();
        }

        /// <summary>Removes a Transaction from the list of Transactions.</summary>
        /// <param name="transaction">Transaction to be removed</param>
        internal void RemoveTransaction(FuelTransaction transaction)
        {
            _transactions.Remove(transaction);
            UpdateProperties();
        }

        /// <summary>Sorts Transactions by date.</summary>
        private void Sort()
        {
            if (_transactions.Count > 0)
                _transactions = _transactions.OrderByDescending(trans => trans.TranscationID).ThenBy(trans => trans.Date).ToList();
        }

        /// <summary>Updates data-binding for MPG-related Properties</summary>
        private void UpdateProperties()
        {
            OnPropertyChanged("Transactions");
            OnPropertyChanged("MPG");
            OnPropertyChanged("MPGToString");
            OnPropertyChanged("MPGToStringWithText");
            OnPropertyChanged("TotalCost");
            OnPropertyChanged("TotalCostToString");
            OnPropertyChanged("TotalCostToStringWithText");
            OnPropertyChanged("AverageDistance");
            OnPropertyChanged("AverageDistanceToString");
            OnPropertyChanged("AverageDistanceToStringWithText");
            OnPropertyChanged("AverageGallons");
            OnPropertyChanged("AverageGallonsToString");
            OnPropertyChanged("AverageGallonsToStringWithText");
            OnPropertyChanged("AveragePrice");
            OnPropertyChanged("AveragePriceToString");
            OnPropertyChanged("AveragePriceToStringWithText");
            OnPropertyChanged("AverageTotalPrice");
            OnPropertyChanged("AverageTotalPriceToString");
            OnPropertyChanged("AverageTotalPriceToStringWithText");
        }

        #endregion Transaction Management

        #region Enumerators

        public IEnumerator<FuelTransaction> GetEnumerator() => Transactions.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion Enumerators

        #region Override Operators

        private static bool Equals(Vehicle left, Vehicle right)
        {
            if (ReferenceEquals(null, left) && ReferenceEquals(null, right)) return true;
            if (ReferenceEquals(null, left) ^ ReferenceEquals(null, right)) return false;
            return left.VehicleID == right.VehicleID && string.Equals(left.Nickname, right.Nickname, StringComparison.OrdinalIgnoreCase) && string.Equals(left.Make, right.Make, StringComparison.OrdinalIgnoreCase) && string.Equals(left.Model, right.Model, StringComparison.OrdinalIgnoreCase) && left.Year == right.Year && left.Transactions.Count == right.Transactions.Count && !left.Transactions.Except(right.Transactions).Any();
        }

        public override bool Equals(object obj) => Equals(this, obj as Vehicle);

        public bool Equals(Vehicle other) => Equals(this, other);

        public static bool operator ==(Vehicle left, Vehicle right) => Equals(left, right);

        public static bool operator !=(Vehicle left, Vehicle right) => !Equals(left, right);

        public override int GetHashCode() => base.GetHashCode() ^ 17;

        public override string ToString() => $"{VehicleID} - {Nickname}";

        #endregion Override Operators

        #region Constructors

        /// <summary>Initializes a default instance of Vehicle.</summary>
        public Vehicle()
        {
        }

        /// <summary>Initializes an instance of Vehicle by assigning Property values.</summary>
        /// <param name="vehicleID">Vehicle ID</param>
        /// <param name="userID">User ID of Vehicle owner</param>
        /// <param name="nickname">Vehicle nickname</param>
        /// <param name="make">Vehicle make</param>
        /// <param name="model">Vehicle model</param>
        /// <param name="year">Vehicle year</param>
        /// <param name="transactions">Fuel-up transactions for Vehicle</param>
        public Vehicle(int vehicleID, string nickname, string make, string model, int year, IEnumerable<FuelTransaction> transactions)
        {
            VehicleID = vehicleID;
            Nickname = nickname;
            Make = make;
            Model = model;
            Year = year;

            List<FuelTransaction> newTransactions = new List<FuelTransaction>();
            newTransactions.AddRange(transactions);
            _transactions = newTransactions;
        }

        /// <summary>Replaces this instance of Vehicle with another instance.</summary>
        /// <param name="other">Instance of Vehicle to replace this instance</param>
        public Vehicle(Vehicle other) : this(other.VehicleID, other.Nickname, other.Make, other.Model, other.Year, other.Transactions)
        {
        }

        #endregion Constructors
    }
}