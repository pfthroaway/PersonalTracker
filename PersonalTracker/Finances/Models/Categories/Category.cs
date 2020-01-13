using PersonalTracker.Models;
using System.Collections.Generic;
using System.Linq;

namespace PersonalTracker.Finances.Models.Categories
{
    /// <summary>Represents a category of transactions.</summary>
    public class Category : BaseINPC
    {
        private string _name;
        private List<string> _minorCategories = new List<string>();

        #region Properties

        /// <summary>Name of major category</summary>
        public string Name
        {
            get => _name;
            set { _name = value; NotifyPropertyChanged(nameof(Name)); }
        }

        /// <summary>List of minor categories related to the major category</summary>
        public List<string> MinorCategories
        {
            get => _minorCategories;
            private set { _minorCategories = value; NotifyPropertyChanged(nameof(MinorCategories)); }
        }

        #endregion Properties

        /// <summary>Sorts the minor categories alphabetically.</summary>
        internal void Sort()
        {
            if (MinorCategories.Count > 0)
            {
                MinorCategories = new List<string>(MinorCategories.OrderBy(category => category).ToList());
                NotifyPropertyChanged(nameof(MinorCategories));
            }
        }

        public override string ToString() => Name;

        #region Constructors

        /// <summary>Initializes a default instance of Category.</summary>
        public Category()
        {
        }

        /// <summary>Initializes an instance of Category by assigning Properties.</summary>
        /// <param name="name">Name of major category</param>
        /// <param name="minorCategories">List of minor categories related to the major category</param>
        public Category(string name, List<string> minorCategories)
        {
            Name = name;
            MinorCategories = minorCategories;
        }

        #endregion Constructors
    }
}