using PersonalTracker.Finances.Models.Enums;
using PersonalTracker.Models;
using System;

namespace PersonalTracker.Finances.Models.Data
{
    public class CreditScore : BaseINPC
    {
        private DateTime _date;
        private string _source;
        private int _score;
        private Providers _provider;
        private bool _fico;

        #region Modifying Properties

        /// <summary>Date credit score received.</summary>
        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                NotifyPropertyChanged(nameof(Date), nameof(DateToString));
            }
        }

        /// <summary>Source of credit score.</summary>
        public string Source
        {
            get => _source;
            set
            {
                _source = value;
                NotifyPropertyChanged(nameof(Source));
            }
        }

        /// <summary>Score given by provider.</summary>
        public int Score
        {
            get => _score;
            set
            {
                _score = value;
                NotifyPropertyChanged(nameof(Score));
            }
        }

        /// <summary>Credit report company providing the score to the source.</summary>
        internal Providers Provider
        {
            get => _provider;
            set
            {
                _provider = value;
                NotifyPropertyChanged(nameof(Provider), nameof(ProviderToString));
            }
        }

        /// <summary>Is this score a FICO score?</summary>
        public bool FICO
        {
            get => _fico;
            set
            {
                _fico = value;
                NotifyPropertyChanged(nameof(FICO), nameof(FICOToString));
            }
        }

        #endregion Modifying Properties

        #region Helper Properties

        /// <summary>Date credit score received, formatted.</summary>
        public string DateToString => Date != DateTime.MinValue ? Date.ToString("yyyy/MM/dd") : "";

        /// <summary>Credit report company providing the score to the source, formatted.</summary>
        public string ProviderToString => Date != DateTime.MinValue ? Provider.ToString() : "";

        /// <summary>Is this score a FICO score? formatted.</summary>
        public string FICOToString => Date != DateTime.MinValue
            ? FICO
                ? "FICO"
                : "Not FICO"
            : "";

        #endregion Helper Properties

        #region Override Operators

        private static bool Equals(CreditScore left, CreditScore right)
        {
            if (left is null && right is null) return true;
            if (left is null ^ right is null) return false;
            return left.Date == right.Date && string.Equals(left.Source, right.Source, StringComparison.OrdinalIgnoreCase) && left.Score == right.Score && left.Provider == right.Provider && left.FICO == right.FICO;
        }

        public sealed override bool Equals(object obj) => Equals(this, obj as CreditScore);

        public bool Equals(CreditScore otherCreditScore) => Equals(this, otherCreditScore);

        public static bool operator ==(CreditScore left, CreditScore right) => Equals(left, right);

        public static bool operator !=(CreditScore left, CreditScore right) => !Equals(left, right);

        public sealed override int GetHashCode() => base.GetHashCode() ^ 17;

        public sealed override string ToString() => $"{DateToString} - {Source} - {Score}";

        #endregion Override Operators

        #region Constructors

        /// <summary>Initializes a default instance of CreditScore.</summary>
        public CreditScore()
        {
        }

        /// <summary>Initializes an instance of CreditScore by assigning Properties.</summary>
        /// <param name="date">Date credit score received</param>
        /// <param name="source">Source of credit score</param>
        /// <param name="score">Score given by provider.</param>
        /// <param name="provider">Credit report company providing the score to the source</param>
        /// <param name="fico">Is this score a FICO score?</param>
        public CreditScore(DateTime date, string source, int score, Providers provider, bool fico)
        {
            Date = date;
            Source = source;
            Score = score;
            Provider = provider;
            FICO = fico;
        }

        /// <summary>Replaces this instance of CreditScore with another instance.</summary>
        /// <param name="other">Instance of CreditScore to replace this instance</param>
        public CreditScore(CreditScore other) : this(other.Date, other.Source, other.Score, other.Provider, other.FICO)
        {
        }

        #endregion Constructors
    }
}