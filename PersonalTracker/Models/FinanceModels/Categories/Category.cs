using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace PersonalTracker.Models.FinanceModels.Categories
{
    /// <summary>Represents a category of transactions.</summary>
    public class Category : INotifyPropertyChanged
    {
        private string _name;
        private List<string> _minorCategories = new List<string>();

        #region Properties

        /// <summary>Name of major category</summary>
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged("Name"); }
        }

        /// <summary>List of minor categories related to the major category</summary>
        public List<string> MinorCategories
        {
            get => _minorCategories;
            private set { _minorCategories = value; OnPropertyChanged("MinorCategories"); }
        }

        #endregion Properties

        #region Data-Binding

        /// <summary>Event that fires if a Property value has changed so that the UI can properly be updated.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Invokes <see cref="PropertyChangedEventHandler"/> to update the UI when a Property value changes.</summary>
        /// <param name="property">Name of Property whose value has changed</param>
        private void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Data-Binding

        /// <summary>Sorts the minor categories alphabetically.</summary>
        internal void Sort()
        {
            if (MinorCategories.Count > 0)
            {
                MinorCategories = new List<string>(MinorCategories.OrderBy(category => category).ToList());
                OnPropertyChanged("MinorCategories");
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