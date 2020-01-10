﻿using System;
using System.ComponentModel;

namespace PersonalTracker.Lenses.Models
{
    /// <summary>Represents a contact lens.</summary>
    public class Contact : INotifyPropertyChanged
    {
        private DateTime _date, _replacementDate;
        private Side _side;

        #region Modifying Properties

        /// <summary>Date on which the contact was inserted.</summary>
        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged("Date");
                OnPropertyChanged("DateToString");
            }
        }

        /// <summary>Date on which the contact needs to be replaced.</summary>
        public DateTime ReplacementDate
        {
            get => _replacementDate;
            set
            {
                _replacementDate = value;
                OnPropertyChanged("ReplacementDate");
                OnPropertyChanged("ReplacementDateToString");
            }
        }

        /// <summary>Side on which the contact was inserted.</summary>
        public Side Side
        {
            get => _side;
            set
            {
                _side = value;
                OnPropertyChanged("Side");
            }
        }

        #endregion Modifying Properties

        #region Helper Properties

        /// <summary>Date on which the contact was inserted, formatted.</summary>
        public string DateToString => Date.ToString("yyyy/MM/dd");

        /// <summary>Date on which the contact needs to be replaced, formatted.</summary>
        public string ReplacementDateToString => ReplacementDate.ToString("yyyy/MM/dd");

        /// <summary>Side on which the contact was inserted, formatted.</summary>
        public string SideToString => Side.ToString();

        #endregion Helper Properties

        #region Data-Binding

        /// <summary>Event that fires if a Property value has changed so that the UI can properly be updated.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Invokes <see cref="PropertyChangedEventHandler"/> to update the UI when a Property value changes.</summary>
        /// <param name="property">Name of Property whose value has changed</param>
        private void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Data-Binding

        #region Override Operators

        private static bool Equals(Contact left, Contact right)
        {
            if (left is null && right is null) return true;
            if (left is null ^ right is null) return false;
            return DateTime.Equals(left.Date, right.Date) && left.Side == right.Side && DateTime.Equals(left.ReplacementDate, right.ReplacementDate);
        }

        public override bool Equals(object obj) => Equals(this, obj as Contact);

        public bool Equals(Contact otherContact) => Equals(this, otherContact);

        public static bool operator ==(Contact left, Contact right) => Equals(left, right);

        public static bool operator !=(Contact left, Contact right) => !Equals(left, right);

        public override int GetHashCode() => base.GetHashCode() ^ 17;

        public override string ToString() => $"{DateToString} - {Side}";

        #endregion Override Operators

        #region Constructors

        /// <summary>Initializes a default instance of Contact.</summary>
        public Contact()
        {
        }

        /// <summary>Initializes an instance of Contact by assigning Properties.</summary>
        /// <param name="date">Date on which the contact was inserted.</param>
        /// <param name="side">Side on which the contact was inserted.</param>
        /// <param name="replacementDate">Date on which the contact should be replaced.</param>
        public Contact(DateTime date, Side side, DateTime replacementDate)
        {
            Date = date;
            Side = side;
            ReplacementDate = replacementDate;
        }

        /// <summary>Replaces this instance of Contact with another instance</summary>
        /// <param name="other">Contact to replace this instance</param>
        public Contact(Contact other) : this(other.Date, other.Side, other.ReplacementDate)
        {
        }

        #endregion Constructors
    }
}