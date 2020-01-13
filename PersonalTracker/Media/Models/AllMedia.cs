using Extensions;
using PersonalTracker.Media.Models.MediaTypes;
using PersonalTracker.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PersonalTracker.Media.Models
{
    public class AllMedia : BaseINPC
    {
        private List<Series> _allSeries = new List<Series>();

        public ReadOnlyCollection<Series> AllSeries => new ReadOnlyCollection<Series>(_allSeries);

        #region Series Management

        /// <summary>Assigns a collection of <see cref="Series"/> to the <see cref="User"/>'s <see cref="Media"/>.</summary>
        /// <param name="series">Collection of <see cref="Series"/> to be assigned</param>
        public void AssignSeries(IEnumerable<Series> series)
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
        public void AddSeries(Series newSeries)
        {
            _allSeries.Add(newSeries);
            UpdateSeries();
        }

        /// <summary>Deletes a <see cref="Series"/> from the collection.</summary>
        /// <param name="deleteSeries"><see cref="Series"/> to be deleted</param>
        public void DeleteSeries(Series deleteSeries) => _allSeries.Remove(deleteSeries);

        /// <summary>Modifies a <see cref="Series"/>.</summary>
        /// <param name="oldSeries">Original <see cref="Series"/></param>
        /// <param name="newSeries"><see cref="Series"/> to replace original</param>
        public void ModifySeries(Series oldSeries, Series newSeries)
        {
            _allSeries.Replace(oldSeries, newSeries);
            UpdateSeries();
        }

        /// <summary>Updates the collection of <see cref="Series"/>.</summary>
        private void UpdateSeries()
        {
            _allSeries = _allSeries.OrderBy(series => series.Name).ToList();
            NotifyPropertyChanged(nameof(AllSeries));
        }

        #endregion Series Management

        #region Constructors

        public AllMedia(IEnumerable<Series> series) => AssignSeries(series);

        #endregion Constructors
    }
}