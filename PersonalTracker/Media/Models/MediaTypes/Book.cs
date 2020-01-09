using System.ComponentModel;

namespace PersonalTracker.Media.Models.MediaTypes
{
    /// <summary>Represents a book.</summary>
    internal class Book : INotifyPropertyChanged
    {
        private string _name, _author, _series;
        private decimal _rating;
        private int _year;

        #region Modifying Properties

        /// <summary>Name of the <see cref="Book"/>.</summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        /// <summary>Author of the <see cref="Book"/>.</summary>
        public string Author
        {
            get => _author;
            set
            {
                _author = value;
                OnPropertyChanged("Author");
            }
        }

        /// <summary>Series of the <see cref="Book"/>.</summary>
        public string Series
        {
            get => _series;
            set
            {
                _series = value;
                OnPropertyChanged("Series");
            }
        }

        /// <summary>Rating of the <see cref="Book"/>.</summary>
        public decimal Rating { get => _rating; set => _rating = value; }

        /// <summary>Year the <see cref="Book"/> was released.</summary>
        public int Year
        {
            get => _year;
            set
            {
                _year = value;
                OnPropertyChanged("Year");
            }
        }

        #endregion Modifying Properties

        #region Data-Binding

        /// <summary>Event that fires if a Property value has changed so that the UI can properly be updated.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Invokes <see cref="PropertyChangedEventHandler"/> to update the UI when a Property value changes.</summary>
        /// <param name="property">Name of Property whose value has changed</param>
        private void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Data-Binding

        #region Constructors

        /// <summary>Initializes a default instance of <see cref="Book"/>.</summary>
        public Book()
        {
        }

        /// <summary>Initializes an instance of <see cref="Book"/> by assigning values to Properties.</summary>
        /// <param name="name">Author of the <see cref="Book"/>.</param>
        /// <param name="author">Author of the <see cref="Book"/>.</param>
        /// <param name="series">Series of the <see cref="Book"/>.</param>
        /// <param name="rating">Rating of the <see cref="Book"/>.</param>
        /// <param name="year">Year the <see cref="Book"/> was released.</param>
        public Book(string name, string author, string series, decimal rating, int year)
        {
            Name = name;
            Author = author;
            Series = series;
            Rating = rating;
            Year = year;
        }

        /// <summary>Replaces this instance of <see cref="Book"/> with another instance.</summary>
        /// <param name="other">Instance of <see cref="Book"/> to replace this instance</param>
        public Book(Book other) : this(other.Name, other.Author, other.Series, other.Rating, other.Year)
        {
        }

        #endregion Constructors
    }
}