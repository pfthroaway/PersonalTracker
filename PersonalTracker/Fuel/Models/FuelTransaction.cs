using PersonalTracker.Models;
using System;

namespace PersonalTracker.Fuel.Models
{
    /// <summary>Represents a fuel-up Transaction for a Vehicle.</summary>
    public class FuelTransaction : BaseINPC
    {
        private string _store;
        private int _transcationID, _vehicleID, _octane, _range;
        private decimal _distance, _gallons, _odometer, _price;
        private DateTime _date;

        #region Modifying Properties

        /// <summary>Transaction ID</summary>
        public int TranscationID
        {
            get => _transcationID;
            set { _transcationID = value; NotifyPropertyChanged(nameof(TranscationID)); }
        }

        /// <summary>Vehicle ID</summary>
        public int VehicleID
        {
            get => _vehicleID;
            set
            {
                _vehicleID = value;
                NotifyPropertyChanged(nameof(VehicleID));
            }
        }

        /// <summary>Store where fuel was purchased.</summary>
        public string Store
        {
            get => _store;
            set
            {
                _store = value;
                NotifyPropertyChanged(nameof(Store));
            }
        }

        /// <summary>Date fuel was purchased.</summary>
        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                NotifyPropertyChanged(nameof(Date), nameof(DateToString));
            }
        }

        /// <summary>Octane of fuel purchased.</summary>
        public int Octane
        {
            get => _octane;
            set
            {
                _octane = value;
                NotifyPropertyChanged(nameof(Octane));
            }
        }

        /// <summary>Estimated range vehicle.</summary>
        public int Range
        {
            get => _range;
            set
            {
                _range = value;
                NotifyPropertyChanged(nameof(Range));
            }
        }

        /// <summary>Distance travelled.</summary>
        public decimal Distance
        {
            get => _distance;
            set
            {
                _distance = value;
                NotifyPropertyChanged(nameof(Distance), nameof(DistanceToString), nameof(MPG), nameof(MPGToString));
            }
        }

        /// <summary>Gallons purchased.</summary>
        public decimal Gallons
        {
            get => _gallons;
            set
            {
                _gallons = value;
                NotifyPropertyChanged(nameof(Gallons), nameof(GallonsToString), nameof(MPG), nameof(MPGToString));
            }
        }

        /// <summary>Odometer at fuel-up.</summary>
        public decimal Odometer
        {
            get => _odometer;
            set
            {
                _odometer = value;
                NotifyPropertyChanged(nameof(Odometer), nameof(OdometerToString));
            }
        }

        /// <summary>Price of fuel per gallon.</summary>
        public decimal Price
        {
            get => _price;
            set
            {
                _price = value;
                NotifyPropertyChanged(nameof(Price), nameof(PriceToString), nameof(TotalPrice), nameof(TotalPriceToString));
            }
        }

        #endregion Modifying Properties

        #region Helper Properties

        /// <summary>Date fuel was purchased, formatted.</summary>
        public string DateToString => Date.ToString("yyyy/MM/dd");

        /// <summary>Distance travelled, formatted.</summary>
        public string DistanceToString => Distance.ToString("N1");

        /// <summary>Gallons purchased, formatted.</summary>
        public string GallonsToString => Gallons.ToString("N2");

        /// <summary>Miles per gallon.</summary>
        public decimal MPG => Distance / Gallons;

        /// <summary>Miles per gallon, formatted with three decimal places.</summary>
        public string MPGToString => MPG.ToString("N3");

        /// <summary>Total price of the transaction.</summary>
        public string OdometerToString => Odometer.ToString("N1");

        /// <summary>Total price of fuel, formatted to currency.</summary>
        public string PriceToString => Price.ToString("C3");

        /// <summary>Total price of the transaction.</summary>
        public decimal TotalPrice => Price * Gallons;

        /// <summary>Total price of the transaction, formatted to currency.</summary>
        public string TotalPriceToString => TotalPrice.ToString("C2");

        #endregion Helper Properties

        #region Override Operators

        private static bool Equals(FuelTransaction left, FuelTransaction right)
        {
            if (left is null && right is null) return true;
            if (left is null ^ right is null) return false;
            return DateTime.Equals(left.Date, right.Date) && string.Equals(left.Store, right.Store, StringComparison.OrdinalIgnoreCase) && left.TranscationID == right.TranscationID && left.VehicleID == right.VehicleID && left.Odometer == right.Odometer && left.Range == right.Range && left.Distance == right.Distance && left.Gallons == right.Gallons && left.Odometer == right.Odometer && left.Price == right.Price;
        }

        public override bool Equals(object obj) => Equals(this, obj as FuelTransaction);

        public bool Equals(FuelTransaction other) => Equals(this, other);

        public static bool operator ==(FuelTransaction left, FuelTransaction right) => Equals(left, right);

        public static bool operator !=(FuelTransaction left, FuelTransaction right) => !Equals(left, right);

        public override int GetHashCode() => base.GetHashCode() ^ 17;

        public override string ToString() => $"{DateToString} - {Store}";

        #endregion Override Operators

        #region Constructors

        /// <summary>Initializes a default instance of Transaction.</summary>
        public FuelTransaction()
        {
        }

        /// <summary>Initializes an instance of Transaction by assigning Property values.</summary>
        /// <param name="transactionID">Transaction ID</param>
        /// <param name="vehicleID">Vehicle ID</param>
        /// <param name="date">Date the Transaction occurred</param>
        /// <param name="store">Store where the Transaction occurred</param>
        /// <param name="octane">Octane of fuel purchased</param>
        /// <param name="distance">Distance travelled since last refuelling</param>
        /// <param name="gallons">Gallons of fuel purchased</param>
        /// <param name="price">Price of fuel per gallon</param>
        /// <param name="odometer">Full odometer reading at time of purchase</param>
        /// <param name="range">Estimated range before refuel</param>
        public FuelTransaction(int transactionID, int vehicleID, DateTime date, string store, int octane, decimal distance, decimal gallons, decimal price, decimal odometer, int range)
        {
            TranscationID = transactionID;
            VehicleID = vehicleID;
            Date = date;
            Store = store;
            Octane = octane;
            Distance = distance;
            Gallons = gallons;
            Price = price;
            Odometer = odometer;
            Range = range;
        }

        /// <summary>Replaces this instance of Transaction with another instance.</summary>
        /// <param name="other">Instance of Transaction to replace this instance</param>
        public FuelTransaction(FuelTransaction other) : this(other.TranscationID, other.VehicleID, other.Date, other.Store, other.Octane, other.Distance, other.Gallons, other.Price, other.Odometer, other.Range)
        {
        }

        #endregion Constructors
    }
}