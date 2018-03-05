using PersonalTracker.Models.MediaModels.Enums;
using System;
using System.ComponentModel;

namespace PersonalTracker.Models.MediaModels.MediaTypes
{
    /// <summary>Represents a television series.</summary>
    internal class Series : INotifyPropertyChanged
    {
        private string _name, _channel, _returnDate;
        private int _seasons, _episodes;
        private decimal _rating;
        private SeriesStatus _status;
        private DayOfWeek _day;
        private DateTime _time, _premiereDate, _finaleDate;

        #region Modifying Properties

        /// <summary>Name of the Series.</summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        /// <summary>Date the series first aired.</summary>
        public DateTime PremiereDate
        {
            get => _premiereDate;
            set { _premiereDate = value; OnPropertyChanged("PremiereDate"); }
        }

        /// <summary>Rating for the Series.</summary>
        public decimal Rating
        {
            get => _rating;
            set
            {
                _rating = value;
                OnPropertyChanged("Rating");
            }
        }

        /// <summary>Number of seasons the series aired.</summary>
        public int Seasons
        {
            get => _seasons;
            set
            {
                _seasons = value;
                OnPropertyChanged("Seasons");
            }
        }

        /// <summary>Total number of episodes of the series.</summary>
        public int Episodes
        {
            get => _episodes;
            set
            {
                _episodes = value;
                OnPropertyChanged("Episodes");
            }
        }

        /// <summary>Currenty status of the series.</summary>
        public SeriesStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        /// <summary>Channel the series airs on.</summary>
        public string Channel
        {
            get => _channel;
            set
            {
                _channel = value;
                OnPropertyChanged("Channel");
            }
        }

        /// <summary>Date the series last aired.</summary>
        public DateTime FinaleDate
        {
            get => _finaleDate;
            set { _finaleDate = value; OnPropertyChanged("FinaleDate"); }
        }

        /// <summary>Day of the week the series airs.</summary>
        public DayOfWeek Day
        {
            get => _day;
            set
            {
                _day = value;
                OnPropertyChanged("Day");
            }
        }

        /// <summary>Time the series currently airs.</summary>
        public DateTime Time
        {
            get => _time;
            set { _time = value; OnPropertyChanged("Time"); }
        }

        /// <summary>Date the series will return on.</summary>
        public string ReturnDate
        {
            get => _returnDate;
            set
            {
                _returnDate = value;
                OnPropertyChanged("ReturnDate");
            }
        }

        #endregion Modifying Properties

        #region Helper Properties

        /// <summary>Rating for the Series, with text.</summary>
        public string RatingToString => Rating != decimal.MinValue && Rating != 0 ? $"My Rating: {Rating:N1}" : "";

        /// <summary>Number of seasons the series aired, with text.</summary>
        public string SeasonsToString => Seasons != 0 ? $"Seasons: {Seasons}" : "";

        /// <summary>Total number of episodes of the series.</summary>
        public string EpisodesToString => Episodes != 0 ? $"Episodes: {Episodes}" : "";

        /// <summary>Time the series currently airs, formatted.</summary>
        public string TimeToString => (!string.IsNullOrEmpty(Name) && Status == SeriesStatus.Airing) || Status == SeriesStatus.Hiatus ? Time.ToString("hh:mm tt") : "";

        /// <summary>Day of week, formatted.</summary>
        public string DayToString => (!string.IsNullOrEmpty(Name) && Status == SeriesStatus.Airing) || Status == SeriesStatus.Hiatus ? Day.ToString() : "";

        /// <summary>Date the series first aired, formatted to string.</summary>
        public string PremiereDateToString => PremiereDate != DateTime.MinValue ? $"{PremiereDate:yyyy/MM/dd}" : "";

        /// <summary>Date the series first aired, formatted to string, with preceding text.</summary>
        public string PremiereDateToStringWithText => !string.IsNullOrEmpty(PremiereDateToString) ? $"Premiere: {PremiereDateToString}" : "";

        /// <summary>Date the series last aired, formatted to string.</summary>
        public string FinaleDateToString => FinaleDate != DateTime.MinValue && Status == SeriesStatus.Ended ? $"{FinaleDate:yyyy/MM/dd}" : "";

        /// <summary>Date the series last aired, formatted to string, with preceding text.</summary>
        public string FinaleDateToStringWithText => !string.IsNullOrEmpty(FinaleDateToString) ? $"Finale: {FinaleDateToString}" : "";

        /// <summary>Date the series will return on, with preceding text.</summary>
        public string ReturnDateWithText => !string.IsNullOrWhiteSpace(ReturnDate) ? $"Returning: {ReturnDate}" : "";

        #endregion Helper Properties

        #region Data-Binding

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Data-Binding

        #region Override Operators

        private static bool Equals(Series left, Series right)
        {
            if (ReferenceEquals(null, left) && ReferenceEquals(null, right)) return true;
            if (ReferenceEquals(null, left) ^ ReferenceEquals(null, right)) return false;
            return string.Equals(left.Name, right.Name, StringComparison.OrdinalIgnoreCase) && DateTime.Equals(left.Time, right.Time) && left.Rating == right.Rating && string.Equals(left.Channel, right.Channel, StringComparison.OrdinalIgnoreCase) && string.Equals(left.ReturnDate, right.ReturnDate, StringComparison.OrdinalIgnoreCase) && left.Seasons == right.Seasons && left.Episodes == right.Episodes && left.Status == right.Status && left.Day == right.Day && DateTime.Equals(left.PremiereDate, right.PremiereDate) && DateTime.Equals(left.FinaleDate, right.FinaleDate);
        }

        public sealed override bool Equals(object obj) => Equals(this, obj as Series);

        public bool Equals(Series otherSeries) => Equals(this, otherSeries);

        public static bool operator ==(Series left, Series right) => Equals(left, right);

        public static bool operator !=(Series left, Series right) => !Equals(left, right);

        public sealed override int GetHashCode() => base.GetHashCode() ^ 17;

        public sealed override string ToString() => Name;

        #endregion Override Operators

        #region Constructors

        /// <summary>Initalizes a default instance of Series.</summary>
        public Series()
        {
        }

        /// <summary>Initializes an instance of Series by assigning values to Properties.</summary>
        /// <param name="name">Name of the Series</param>
        /// <param name="time">Release date of the Series</param>
        /// <param name="rating">Rating for the Series</param>
        /// <param name="startDate"></param>
        /// <param name="seasons"></param>
        /// <param name="episodes"></param>
        /// <param name="status"></param>
        /// <param name="channel"></param>
        /// <param name="endDate"></param>
        /// <param name="day"></param>
        /// <param name="returnDate"></param>
        public Series(string name, DateTime startDate, decimal rating, int seasons, int episodes, SeriesStatus status, string channel, DateTime endDate, DayOfWeek day, DateTime time, string returnDate)
        {
            Name = name;
            PremiereDate = startDate;
            Rating = rating;
            Seasons = seasons;
            Episodes = episodes;
            Status = status;
            Channel = channel;
            FinaleDate = endDate;
            Day = day;
            Time = time;
            ReturnDate = returnDate;
        }

        /// <summary>Replaces this instance of Series with another instance.</summary>
        /// <param name="other">Instance of Series to replace this instance</param>
        public Series(Series other) : this(other.Name, other.PremiereDate, other.Rating, other.Seasons, other.Episodes,
            other.Status, other.Channel, other.FinaleDate, other.Day, other.Time, other.ReturnDate)
        {
        }

        #endregion Constructors
    }
}