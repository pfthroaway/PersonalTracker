using PersonalTracker.Models;
using PersonalTracker.Views.FuelViews;
using PersonalTracker.Views.LensesViews;
using PersonalTracker.Views.MediaViews;
using System.ComponentModel;
using System.Windows;

namespace PersonalTracker.Views
{
    /// <summary>Interaction logic for TrackerPage.xaml</summary>
    public partial class TrackerPage : INotifyPropertyChanged
    {
        private string _welcomeMessage;

        /// <summary>Message to be displayed on the screen above the Buttons.</summary>
        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set
            {
                _welcomeMessage = value;
                OnPropertyChanged(nameof(WelcomeMessage));
            }
        }

        /// <summary>Loads everything about the current <see cref="User"/>.</summary>
        internal async void LoadUser() => await AppState.LoadCurrentUser();

        #region Data-Binding

        /// <summary>Event that fires if a Property value has changed so that the UI can properly be updated.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Invokes <see cref="PropertyChangedEventHandler"/> to update the UI when a Property value changes.</summary>
        /// <param name="property">Name of Property whose value has changed</param>
        private void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Data-Binding

        #region Click

        private void BtnFinances_Click(object sender, RoutedEventArgs e) => AppState.Navigate(new FinanceViews.FinancesPage());

        private void BtnFuel_Click(object sender, RoutedEventArgs e) => AppState.Navigate(new ViewAccountPage());

        private void BtnLenses_Click(object sender, RoutedEventArgs e) => AppState.Navigate(new LensesPage());

        private void BtnMedia_Click(object sender, RoutedEventArgs e) => AppState.Navigate(new MediaPage());

        private void BtnChangePassword_Click(object sender, RoutedEventArgs e) => AppState.Navigate(new ModifyUserPage());

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            ClosePage();
            AppState.CurrentUser = null;
            AppState.CurrentUserDatabaseName = "";
            AppState.CurrentUserConnection = "";
        }

        #endregion Click

        #region Page-Manipulation Methods

        /// <summary>Closes the Page.</summary>
        private void ClosePage() => AppState.GoBack();

        public TrackerPage()
        {
            InitializeComponent();
            BtnFinances.Focus();
            DataContext = this;
            LoadUser();
        }

        private void TrackerPage_Loaded(object sender, RoutedEventArgs e)
        {
            AppState.CalculateScale(Grid);

            WelcomeMessage = $"Welcome, {AppState.CurrentUser.Username}!";
        }

        #endregion Page-Manipulation Methods
    }
}