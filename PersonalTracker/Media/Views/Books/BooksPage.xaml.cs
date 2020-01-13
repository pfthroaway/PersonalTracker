using System.ComponentModel;
using System.Windows.Controls;

namespace PersonalTracker.Media.Views.MediaBooks
{
    /// <summary>Interaction logic for BooksPage.xaml</summary>
    public partial class BooksPage : Page, INotifyPropertyChanged
    {
        #region Data-Binding

        /// <summary>Event that fires if a Property value has changed so that the UI can properly be updated.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Invokes <see cref="PropertyChangedEventHandler"/> to update the UI when a Property value changes.</summary>
        /// <param name="property">Name of Property whose value has changed</param>
        private void NotifyPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Data-Binding

        #region Page-Manipulation Methods

        public BooksPage() => InitializeComponent();

        #endregion Page-Manipulation Methods
    }
}