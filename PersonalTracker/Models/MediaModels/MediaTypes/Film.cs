using System;
using System.ComponentModel;

namespace PersonalTracker.Models.MediaModels.MediaTypes
{
    /// <summary>Represents a film.</summary>
    internal class Film : INotifyPropertyChanged
    {
        private string _name;
        private DateTime _released;
        private decimal _rating;

        #region Modifying Properties

        /// <summary>Name of the Film.</summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        /// <summary>Release date of the Film.</summary>
        public DateTime Released
        {
            get => _released;
            set { _released = value; OnPropertyChanged("Released"); }
        }

        /// <summary>Rating for the Film.</summary>
        public decimal Rating
        {
            get => _rating;
            set
            {
                _rating = value;
                OnPropertyChanged("Rating");
            }
        }

        #endregion Modifying Properties

        #region Helper Properties

        /// <summary>Release date of the Film, formatted to string.</summary>
        public string ReleasedToString => Released.ToString("yyyy/MM/dd");

        #endregion Helper Properties

        #region Data-Binding

        /// <summary>Event that fires if a Property value has changed so that the UI can properly be updated.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Invokes <see cref="PropertyChangedEventHandler"/> to update the UI when a Property value changes.</summary>
        /// <param name="property">Name of Property whose value has changed</param>
        private void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Data-Binding

        #region Override Operators

        private static bool Equals(Film left, Film right)
        {
            if (ReferenceEquals(null, left) && ReferenceEquals(null, right)) return true;
            if (ReferenceEquals(null, left) ^ ReferenceEquals(null, right)) return false;
            return string.Equals(left.Name, right.Name, StringComparison.OrdinalIgnoreCase) && DateTime.Equals(left.Released, right.Released) && left.Rating == right.Rating;
        }

        public sealed override bool Equals(object obj) => Equals(this, obj as Film);

        public bool Equals(Film otherFilm) => Equals(this, otherFilm);

        public static bool operator ==(Film left, Film right) => Equals(left, right);

        public static bool operator !=(Film left, Film right) => !Equals(left, right);

        public sealed override int GetHashCode() => base.GetHashCode() ^ 17;

        public sealed override string ToString() => $"{Name} ({Released:yyyy})";

        #endregion Override Operators

        #region Constructors

        /// <summary>Initalizes a default instance of Film.</summary>
        public Film()
        {
        }

        /// <summary>Initializes an instance of Film by assigning values to Properties.</summary>
        /// <param name="name">Name of the Film</param>
        /// <param name="released">Release date of the Film</param>
        /// <param name="rating">Rating for the Film</param>
        public Film(string name, DateTime released, decimal rating)
        {
            Name = name;
            Released = released;
            Rating = rating;
        }

        /// <summary>Replaces this instance of Film with another instance.</summary>
        /// <param name="other">Instance of Film to replace this instance</param>
        public Film(Film other) : this(other.Name, other.Released, other.Rating)
        {
        }

        #endregion Constructors
    }
}