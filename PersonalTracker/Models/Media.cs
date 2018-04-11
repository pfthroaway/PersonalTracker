using Extensions;
using PersonalTracker.Models.MediaModels.MediaTypes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace PersonalTracker.Models
{
    internal class Media : INotifyPropertyChanged
    {
        private List<Series> _allSeries = new List<Series>();

        public ReadOnlyCollection<Series> AllSeries => new ReadOnlyCollection<Series>(_allSeries);

        #region Data-Binding

        /// <summary>Event that fires if a Property value has changed so that the UI can properly be updated.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Invokes <see cref="PropertyChangedEventHandler"/> to update the UI when a Property value changes.</summary>
        /// <param name="property">Name of Property whose value has changed</param>
        private void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Data-Binding

        #region Series Management

        /// <summary>Assigns a collection of <see cref="Series"/> to the <see cref="User"/>'s <see cref="Media"/>.</summary>
        /// <param name="series">Collection of <see cref="Series"/> to be assigned</param>
        internal void AssignSeries(IEnumerable<Series> series)
        {
            _allSeries = new List<Series>();

            List<Series> newSeries = new List<Series>();
            newSeries.AddRange(series);
            _allSeries = newSeries;
            if (AllSeries.Count > 0)
                UpdateSeries();
        }

        /// <summary>Adds a new <see cref="Series"/> to the collection.</summary>
        /// <param name="newSeries"><see cref="Series"/> to be saved</param>
        internal void AddSeries(Series newSeries)
        {
            _allSeries.Add(newSeries);
            UpdateSeries();
        }

        /// <summary>Deletes a <see cref="Series"/> from the collection.</summary>
        /// <param name="deleteSeries"><see cref="Series"/> to be deleted</param>
        internal void DeleteSeries(Series deleteSeries) => _allSeries.Remove(deleteSeries);

        /// <summary>Modifies a <see cref="Series"/>.</summary>
        /// <param name="oldSeries">Original <see cref="Series"/></param>
        /// <param name="newSeries"><see cref="Series"/> to replace original</param>
        internal void ModifySeries(Series oldSeries, Series newSeries)
        {
            _allSeries.Replace(oldSeries, newSeries);
            UpdateSeries();
        }

        /// <summary>Updates the collection of <see cref="Series"/>.</summary>
        private void UpdateSeries()
        {
            _allSeries = _allSeries.OrderBy(series => series.Name).ToList();
            OnPropertyChanged("AllSeries");
        }

        #endregion Series Management

        #region Constructors

        public Media(IEnumerable<Series> series) => AssignSeries(series);

        #endregion Constructors
    }
}