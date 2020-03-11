using PersonalTracker.Models;

namespace PersonalTracker.Media.Models.MediaTypes
{
    /// <summary>Represents a book.</summary>
    internal class Book : BaseINPC
    {
        private string _name, _author, _series;
        private decimal _number, _rating;
        private int _year;

        #region Modifying Properties

        /// <summary>Name of the <see cref="Book"/>.</summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyPropertyChanged(nameof(Name));
            }
        }

        /// <summary>Author of the <see cref="Book"/>.</summary>
        public string Author
        {
            get => _author;
            set
            {
                _author = value;
                NotifyPropertyChanged(nameof(Author));
            }
        }

        /// <summary>Series of the <see cref="Book"/>.</summary>
        public string Series
        {
            get => _series;
            set
            {
                _series = value;
                NotifyPropertyChanged(nameof(Series));
            }
        }

        /// <summary>Number of the <see cref="Book"/> in the series.</summary>
        public decimal Number
        {
            get => _number;
            set { _number = value; NotifyPropertyChanged(nameof(Number)); }
        }

        /// <summary>Rating of the <see cref="Book"/>.</summary>
        public decimal Rating
        {
            get => _rating;
            set { _rating = value; NotifyPropertyChanged(nameof(Rating)); }
        }

        /// <summary>Year the <see cref="Book"/> was released.</summary>
        public int Year
        {
            get => _year;
            set
            {
                _year = value;
                NotifyPropertyChanged(nameof(Year));
            }
        }

        #endregion Modifying Properties

        #region Constructors

        /// <summary>Initializes a default instance of <see cref="Book"/>.</summary>
        public Book()
        {
        }

        /// <summary>Initializes an instance of <see cref="Book"/> by assigning values to Properties.</summary>
        /// <param name="name">Author of the <see cref="Book"/>.</param>
        /// <param name="author">Author of the <see cref="Book"/>.</param>
        /// <param name="series">Series of the <see cref="Book"/>.</param>
        /// <param name="number">Number of the <see cref="Book"/> in the series.</param>
        /// <param name="rating">Rating of the <see cref="Book"/>.</param>
        /// <param name="year">Year the <see cref="Book"/> was released.</param>
        public Book(string name, string author, string series, decimal number, decimal rating, int year)
        {
            Name = name;
            Author = author;
            Series = series;
            Number = number;
            Rating = rating;
            Year = year;
        }

        /// <summary>Replaces this instance of <see cref="Book"/> with another instance.</summary>
        /// <param name="other">Instance of <see cref="Book"/> to replace this instance</param>
        public Book(Book other) : this(other.Name, other.Author, other.Series, other.Number, other.Rating, other.Year)
        {
        }

        #endregion Constructors
    }
}