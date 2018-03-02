using Extensions;
using Extensions.Enums;
using PersonalTracker.Models.Database;
using PersonalTracker.Models.FinanceModels;
using PersonalTracker.Models.FinanceModels.Categories;
using PersonalTracker.Models.FinanceModels.Data;
using PersonalTracker.Models.FinanceModels.Sorting;
using PersonalTracker.Models.FuelModels;
using PersonalTracker.Models.LensesModels;
using PersonalTracker.Models.MediaModels.MediaTypes;
using PersonalTracker.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PersonalTracker.Models
{
    internal static class AppState
    {
        #region User

        internal static User CurrentUser;

        internal static string CurrentUserDatabaseName, CurrentUserConnection;

        #endregion User

        #region Database Interaction

        private static readonly SQLiteDatabaseInteraction DatabaseInteraction = new SQLiteDatabaseInteraction();

        /// <summary>Verifies that the requested database exists and that its file size is greater than zero. If not, it extracts the embedded database file to the local output folder.</summary>
        public static void VerifyDatabaseIntegrity() => DatabaseInteraction.VerifyDatabaseIntegrity();

        #endregion Database Interaction

        #region Navigation

        /// <summary>Instance of MainWindow currently loaded</summary>
        internal static MainWindow MainWindow { get; set; }

        /// <summary>Width of the Page currently being displayed in the MainWindow</summary>
        internal static double CurrentPageWidth { get; set; }

        /// <summary>Height of the Page currently being displayed in the MainWindow</summary>
        internal static double CurrentPageHeight { get; set; }

        /// <summary>Calculates the scale needed for the MainWindow.</summary>
        /// <param name="grid">Grid of current Page</param>
        internal static void CalculateScale(Grid grid)
        {
            CurrentPageHeight = grid.ActualHeight;
            CurrentPageWidth = grid.ActualWidth;
            MainWindow.CalculateScale();

            Page newPage = MainWindow.MainFrame.Content as Page;
            if (newPage != null)
                newPage.Style = (Style)MainWindow.FindResource("PageStyle");
        }

        /// <summary>Navigates to selected Page.</summary>
        /// <param name="newPage">Page to navigate to.</param>
        internal static void Navigate(Page newPage) => MainWindow.MainFrame.Navigate(newPage);

        /// <summary>Navigates to the previous Page.</summary>
        internal static void GoBack()
        {
            if (MainWindow.MainFrame.CanGoBack)
                MainWindow.MainFrame.GoBack();
        }

        #endregion Navigation

        #region User Management

        /// <summary>Attempts login of a <see cref="User"/>.</summary>
        /// <param name="username">Username attempting to log in</param>
        /// <param name="password">Hashed PBKDF2 password</param>
        /// <returns>True if user exists and passwords match</returns>
        public static Task<bool> AttemptLogin(string username, string password) => DatabaseInteraction.AttemptLogin(username, password);

        /// <summary>Creates a new <see cref="User"/>.</summary>
        /// <param name="createUser"><see cref="User"/> to be created</param>
        /// <returns>True if successful</returns>
        public static Task<bool> CreateUser(User createUser) => DatabaseInteraction.CreateUser(createUser);

        /// <summary>Deletes a <see cref="User"/>.</summary>
        /// <param name="deleteUser"><see cref="User"/> to be deleted</param>
        /// <returns>True if successful</returns>
        public static Task<bool> DeleteUser(User deleteUser) => DatabaseInteraction.DeleteUser(deleteUser);

        /// <summary>Gets the next index in the User table.</summary>
        /// <returns>Next index in the User table.</returns>
        public static Task<int> GetNextUserIndex() => DatabaseInteraction.GetNextUserIndex();

        public static async Task LoadCurrentUser()
        {
            DatabaseInteraction.VerifyUserDatabaseIntegrity();
            CurrentUser.SetLenses(new List<Contact>(await DatabaseInteraction.LoadContacts()));
            CurrentUser.SetVehicles(new List<Vehicle>(await DatabaseInteraction.LoadVehicles()));
            CurrentUser.Media = new Media(await DatabaseInteraction.LoadSeries());
            CurrentUser.Finances = new Finances(await DatabaseInteraction.LoadAccounts(), await DatabaseInteraction.LoadAccountTypes(), await DatabaseInteraction.LoadCategories());
        }

        /// <summary>Loads a <see cref="User"/> from the database.</summary>
        /// <param name="username">Username to load</param>
        /// <returns><see cref="User"/></returns>
        public static Task<User> LoadUser(string username) => DatabaseInteraction.LoadUser(username);

        /// <summary>Modifies a <see cref="User"/>.</summary>
        /// <param name="oldUser"><see cref="User"/> to be modified</param>
        /// <param name="newUser"><see cref="User"/></param>
        /// <returns>True if successful</returns>
        public static Task<bool> ModifyUser(User oldUser, User newUser) => DatabaseInteraction.ModifyUser(oldUser, newUser);

        /// <summary>Assigns information about the <see cref="User"/>'s database file in <see cref="AppState"/>.</summary>
        internal static void SetUserDatabaseInformation()
        {
            CurrentUserDatabaseName = $"{CurrentUser.UserIDToString}.sqlite";
            CurrentUserConnection = $"Data Source = {CurrentUserDatabaseName}; foreign keys = true; Version = 3;";
        }

        #endregion User Management

        #region Finances

        #region Account Manipulation

        /// <summary>Adds an account to the database.</summary>
        /// <param name="newAccount">Account to be added</param>
        /// <returns>Returns true if successful</returns>
        internal static async Task<bool> AddAccount(Account newAccount)
        {
            bool success = false;
            if (await DatabaseInteraction.AddAccount(newAccount))
            {
                CurrentUser.Finances.AllAccounts.Add(newAccount);
                CurrentUser.Finances.AllAccounts = CurrentUser.Finances.AllAccounts.OrderBy(account => account.Name).ToList();
                CurrentUser.Finances.AllTransactions.Add(newAccount.AllTransactions[0]);
                CurrentUser.Finances.AllTransactions = CurrentUser.Finances.AllTransactions.OrderByDescending(transaction => transaction.Date).ThenByDescending(transaction => transaction.ID).ToList();
                success = true;
            }

            return success;
        }

        /// <summary>Deletes an account from the database.</summary>
        /// <param name="account">Account to be deleted</param>
        /// <returns>Returns true if successful</returns>
        internal static async Task<bool> DeleteAccount(Account account)
        {
            bool success = false;
            if (await DatabaseInteraction.DeleteAccount(account))
            {
                foreach (FinancialTransaction transaction in account.AllTransactions)
                    CurrentUser.Finances.AllTransactions.Remove(transaction);

                CurrentUser.Finances.AllAccounts.Remove(account);
                success = true;
            }

            return success;
        }

        /// <summary>Renames an account in the database.</summary>
        /// <param name="account">Account to be renamed</param>
        /// <param name="newAccountName">New account's name</param>
        /// <returns>Returns true if successful</returns>
        internal static async Task<bool> RenameAccount(Account account, string newAccountName)
        {
            bool success = false;
            string oldAccountName = account.Name;
            if (await DatabaseInteraction.RenameAccount(account, newAccountName))
            {
                account.Name = newAccountName;

                foreach (FinancialTransaction transaction in CurrentUser.Finances.AllTransactions)
                {
                    if (transaction.Account == oldAccountName)
                        transaction.Account = newAccountName;
                }
                success = true;
            }

            return success;
        }

        #endregion Account Manipulation

        #region Category Management

        /// <summary>Inserts a new Category into the database.</summary>
        /// <param name="selectedCategory">Selected Major Category</param>
        /// <param name="newName">Name for new Category</param>
        /// <param name="isMajor">Is the category being added a Major Category?</param>
        /// <returns>Returns true if successful.</returns>
        internal static async Task<bool> AddCategory(Category selectedCategory, string newName, bool isMajor)
        {
            bool success = false;
            if (await DatabaseInteraction.AddCategory(selectedCategory, newName, isMajor))
            {
                if (isMajor)
                {
                    CurrentUser.Finances.AllCategories.Add(new Category(newName, new List<string>()));
                }
                else
                    selectedCategory.MinorCategories.Add(newName);

                CurrentUser.Finances.AllCategories = CurrentUser.Finances.AllCategories.OrderBy(category => category.Name).ToList();
                success = true;
            }

            return success;
        }

        /// <summary>Rename a category in the database.</summary>
        /// <param name="selectedCategory">Category to rename</param>
        /// <param name="newName">New name of the Category</param>
        /// <param name="oldName">Old name of the Category</param>
        /// <param name="isMajor">Is the category being renamed a Major Category?</param>
        /// <returns></returns>
        internal static async Task<bool> RenameCategory(Category selectedCategory, string newName, string oldName, bool isMajor)
        {
            bool success = false;
            if (await DatabaseInteraction.RenameCategory(selectedCategory, newName, oldName, isMajor))
            {
                if (isMajor)
                {
                    selectedCategory = CurrentUser.Finances.AllCategories.Find(category => category.Name == selectedCategory.Name);
                    selectedCategory.Name = newName;
                    CurrentUser.Finances.AllTransactions.Select(transaction => transaction.MajorCategory == oldName ? newName : oldName).ToList();
                }
                else
                {
                    selectedCategory = CurrentUser.Finances.AllCategories.Find(category => category.Name == selectedCategory.Name);
                    selectedCategory.MinorCategories.Remove(oldName);
                    selectedCategory.MinorCategories.Add(newName);
                    CurrentUser.Finances.AllTransactions.Select(transaction => transaction.MinorCategory == oldName ? newName : oldName).ToList();
                }

                CurrentUser.Finances.AllCategories = CurrentUser.Finances.AllCategories.OrderBy(category => category.Name).ToList();
                success = true;
            }

            return success;
        }

        /// <summary>Removes a Major Category from the database, as well as removes it from all Transactions which utilize it.</summary>
        /// <param name="selectedCategory">Selected Major Category to delete</param>
        /// <returns>Returns true if operation successful</returns>
        internal static async Task<bool> RemoveMajorCategory(Category selectedCategory)
        {
            bool success = false;
            if (await DatabaseInteraction.RemoveMajorCategory(selectedCategory))
            {
                foreach (FinancialTransaction transaction in CurrentUser.Finances.AllTransactions)
                {
                    if (transaction.MajorCategory == selectedCategory.Name)
                    {
                        transaction.MajorCategory = "";
                        transaction.MinorCategory = "";
                    }
                }

                CurrentUser.Finances.AllCategories.Remove(CurrentUser.Finances.AllCategories.Find(category => category.Name == selectedCategory.Name));
                success = true;
            }

            return success;
        }

        /// <summary>Removes a Major Category from the database, as well as removes it from all Transactions which utilize it.</summary>
        /// <param name="selectedCategory">Selected Major Category</param>
        /// <param name="minorCategory">Selected Minor Category to delete</param>
        /// <returns>Returns true if operation successful</returns>
        internal static async Task<bool> RemoveMinorCategory(Category selectedCategory, string minorCategory)
        {
            bool success = false;
            if (await DatabaseInteraction.RemoveMinorCategory(selectedCategory, minorCategory))
            {
                foreach (FinancialTransaction transaction in CurrentUser.Finances.AllTransactions)
                {
                    if (transaction.MajorCategory == selectedCategory.Name && transaction.MinorCategory == minorCategory)
                        transaction.MinorCategory = "";
                }

                selectedCategory = CurrentUser.Finances.AllCategories.Find(category => category.Name == selectedCategory.Name);
                selectedCategory.MinorCategories.Remove(minorCategory);
                success = true;
            }

            return success;
        }

        #endregion Category Management

        #region Credit Score Management

        /// <summary>Loads all credit scores from the database.</summary>
        /// <returns>List of all credit scores</returns>
        public static Task<List<CreditScore>> LoadCreditScores() => DatabaseInteraction.LoadCreditScores();

        /// <summary>Adds a new credit score to the database.</summary>
        /// <param name="newScore">Score to be added</param>
        /// <returns>True if successful</returns>
        public static Task<bool> AddCreditScore(CreditScore newScore) =>
            DatabaseInteraction.AddCreditScore(newScore);

        /// <summary>Deletes a credit score from the database</summary>
        /// <param name="deleteScore">Score to be deleted</param>
        /// <returns>True if successful</returns>
        public static Task<bool> DeleteCreditScore(CreditScore deleteScore) =>
            DatabaseInteraction.DeleteCreditScore(deleteScore);

        /// <summary>Modifies a credit score in the database.</summary>
        /// <param name="oldScore">Original score</param>
        /// <param name="newScore">Modified score</param>
        /// <returns>True if successful</returns>
        public static Task<bool> ModifyCreditScore(CreditScore oldScore, CreditScore newScore) =>
            DatabaseInteraction.ModifyCreditScore(oldScore, newScore);

        #endregion Credit Score Management

        #region Transaction Manipulation

        /// <summary>Adds a transaction to an account and the database</summary>
        /// <param name="transaction">Transaction to be added</param>
        /// <param name="account">Account the transaction will be added to</param>
        /// <returns>Returns true if successful</returns>
        internal static async Task<bool> AddFinancialTransaction(FinancialTransaction transaction, Account account)
        {
            bool success = false;
            if (await DatabaseInteraction.AddFinancialTransaction(transaction, account))
            {
                if (CurrentUser.Finances.AllMonths.Any(month => month.MonthStart <= transaction.Date && transaction.Date <= month.MonthEnd.Date))
                    CurrentUser.Finances.AllMonths.Find(month => month.MonthStart <= transaction.Date && transaction.Date <= month.MonthEnd.Date).AddTransaction(transaction);
                else
                {
                    Month newMonth = new Month(new DateTime(transaction.Date.Year, transaction.Date.Month, 1), new List<FinancialTransaction>());
                    newMonth.AddTransaction(transaction);
                    AppState.CurrentUser.Finances.AllMonths.Add(newMonth);
                }

                CurrentUser.Finances.AllMonths = CurrentUser.Finances.AllMonths.OrderByDescending(month => month.FormattedMonth).ToList();
                success = true;
            }
            else
                DisplayNotification("Unable to process transaction.", "Finances");

            return success;
        }

        /// <summary>Gets the next Transaction ID autoincrement value in the database for the Transactions table.</summary>
        /// <returns>Next Transactions ID value</returns>
        public static Task<int> GetNextFinancialTransactionIndex() => DatabaseInteraction.GetNextFinancialTransactionIndex();

        /// <summary>Modifies the selected Transaction in the database.</summary>
        /// <param name="newTransaction">Transaction to replace the current one in the database</param>
        /// <param name="oldTransaction">Current Transaction in the database</param>
        /// <returns>Returns true if successful</returns>
        internal static async Task<bool> ModifyFinancialTransaction(FinancialTransaction newTransaction, FinancialTransaction oldTransaction)
        {
            bool success = false;
            if (await DatabaseInteraction.ModifyFinancialTransaction(newTransaction, oldTransaction))
            {
                CurrentUser.Finances.AllTransactions[CurrentUser.Finances.AllTransactions.IndexOf(oldTransaction)] = newTransaction;
                CurrentUser.Finances.AllTransactions = CurrentUser.Finances.AllTransactions.OrderByDescending(transaction => transaction.Date).ThenByDescending(transaction => transaction.ID).ToList();
                CurrentUser.Finances.LoadMonths();
                CurrentUser.Finances.LoadYears();
                success = true;
            }

            return success;
        }

        /// <summary>Deletes a transaction from the database.</summary>
        /// <param name="transaction">Transaction to be deleted</param>
        /// <param name="account">Account the transaction will be deleted from</param>
        /// <returns>Returns true if successful</returns>
        internal static async Task<bool> DeleteFinancialTransaction(FinancialTransaction transaction, Account account)
        {
            bool success = false;
            if (await DatabaseInteraction.DeleteFinancialTransaction(transaction, account))
            {
                CurrentUser.Finances.AllTransactions.Remove(transaction);
                success = true;
            }

            return success;
        }

        #endregion Transaction Manipulation

        #endregion Finances

        #region Contact Lens Manipulation

        /// <summary>Adds a new <see cref="Contact"/> insertion to the database.</summary>
        /// <param name="newContact"><see cref="Contact"/> insertion to be added</param>
        internal static async Task<bool> AddContact(Contact newContact)
        {
            if (await DatabaseInteraction.AddContact(newContact))
            {
                CurrentUser.AddContactLens(newContact);
                return true;
            }
            return false;
        }

        #endregion Contact Lens Manipulation

        #region Transaction Management

        /// <summary>Gets the next TransactionID autoincrement value in the database for the <see cref="FuelTransaction"/> table.</summary>
        /// <returns>Next TransactionID value</returns>
        public static Task<int> GetNextFuelTransactionIndex() => DatabaseInteraction.GetNextFuelTransactionIndex();

        /// <summary>Deletes a <see cref="FuelTransaction"/> from the database.</summary>
        /// <param name="deleteTransaction"><see cref="FuelTransaction"/> to be deleted</param>
        /// <returns>Returns true if deletion successful</returns>
        public static Task<bool> DeleteFuelTransaction(FuelTransaction deleteTransaction) => DatabaseInteraction
            .DeleteFuelTransaction(deleteTransaction);

        /// <summary>Loads all <see cref="FuelTransaction"/>s associated with a specific Vehicle.</summary>
        /// <param name="vehicleID"><see cref="Vehicle"/> ID</param>
        /// <returns>Returns all Transactions associated with a specific <see cref="Vehicle"/>.</returns>
        public static Task<List<FuelTransaction>> LoadFuelTransactions(int vehicleID) => DatabaseInteraction
            .LoadFuelTransactions(vehicleID);

        /// <summary>Modifies an existing <see cref="FuelTransaction"/>.</summary>
        /// <param name="oldTransaction">Existing <see cref="FuelTransaction"/></param>
        /// <param name="newTransaction">New <see cref="FuelTransaction"/></param>
        /// <returns>Returns true if modification successful</returns>
        public static Task<bool> ModifyFuelTransaction(FuelTransaction oldTransaction, FuelTransaction newTransaction) =>
            DatabaseInteraction.ModifyFuelTransaction(oldTransaction, newTransaction);

        /// <summary>Adds a new <see cref="FuelTransaction"/> to the database.</summary>
        /// <param name="newTransaction"><see cref="FuelTransaction"/> to be added</param>
        /// <returns>Returns true if add successful</returns>
        public static Task<bool> NewFuelTransaction(FuelTransaction newTransaction) => DatabaseInteraction
            .NewFuelTransaction(newTransaction);

        #endregion Transaction Management

        #region Vehicle Management

        /// <summary>Gets the next VehicleID autoincrement value in the database for the <see cref="Vehicle"/> table.</summary>
        /// <returns>Next VehicleID value</returns>
        public static Task<int> GetNextVehicleIndex() => DatabaseInteraction.GetNextVehicleIndex();

        /// <summary>Deletes a <see cref="Vehicle"/> and all associated <see cref="FuelTransaction"/>s from the database.</summary>
        /// <param name="deleteVehicle"><see cref="Vehicle"/> to be deleted</param>
        /// <returns>Returns true if deletion is successful.</returns>
        public static Task<bool> DeleteVehicle(Vehicle deleteVehicle) =>
            DatabaseInteraction.DeleteVehicle(deleteVehicle);

        /// <summary>Loads all <see cref="Vehicle"/>s associated with a User.</summary>
        /// <returns>All <see cref="Vehicle"/>s associated with a User</returns>
        public static Task<List<Vehicle>> LoadVehicles() => DatabaseInteraction.LoadVehicles();

        /// <summary>Changes details in the database regarding a <see cref="Vehicle"/>.</summary>
        /// <param name="oldVehicle">Old <see cref="Vehicle"/> details</param>
        /// <param name="newVehicle">New <see cref="Vehicle"/> details</param>
        /// <returns>Returns true if modification successful</returns>
        public static Task<bool> ModifyVehicle(Vehicle oldVehicle, Vehicle newVehicle) =>
            DatabaseInteraction.ModifyVehicle(oldVehicle, newVehicle);

        /// <summary>Adds a new <see cref="Vehicle"/> to the database.</summary>
        /// <param name="newVehicle"><see cref="Vehicle"/> to be added</param>
        /// <returns>Returns whether the <see cref="Vehicle"/> was successfully added</returns>
        public static Task<bool> NewVehicle(Vehicle newVehicle) =>
            DatabaseInteraction.NewVehicle(newVehicle);

        #endregion Vehicle Management

        #region Television

        #region Delete

        /// <summary>Deletes a <see cref="Series"/> from the database.</summary>
        /// <param name="deleteSeries"><see cref="Series"/> to be deleted</param>
        /// <returns>True if successful</returns>
        public static async Task<bool> DeleteSeries(Series deleteSeries)
        {
            if (YesNoNotification($"Are you sure you want to delete {deleteSeries.Name}? This action cannot be undone.",
              "Media Tracker"))
            {
                if (await DatabaseInteraction.DeleteSeries(deleteSeries))
                {
                    CurrentUser.Media.DeleteSeries(deleteSeries);
                    return true;
                }
                DisplayNotification($"Unable to delete {deleteSeries.Name}.", "Media Tracker");
            }
            return false;
        }

        #endregion Delete

        #region Load

        internal static async Task LoadAll() => await LoadSeries();

        /// <summary>Loads all <see cref="Series"/> from the database.</summary>
        /// <returns>All Series</returns>
        public static async Task LoadSeries() => CurrentUser.Media.AssignSeries(await DatabaseInteraction.LoadSeries());

        #endregion Load

        #region Save

        /// <summary>Modifies a <see cref="Series"/> in the database.</summary>
        /// <param name="oldSeries">Original <see cref="Series"/></param>
        /// <param name="newSeries"><see cref="Series"/> to replace original</param>
        /// <returns>True if successful</returns>
        internal static async Task<bool> ModifySeries(Series oldSeries, Series newSeries)
        {
            if (await DatabaseInteraction.ModifySeries(oldSeries, newSeries))
            {
                CurrentUser.Media.ModifySeries(oldSeries, newSeries);
                return true;
            }
            DisplayNotification("Unable to modify television series.", "Media Tracker");
            return false;
        }

        /// <summary>Saves a new <see cref="Series"/> to the database.</summary>
        /// <param name="newSeries"><see cref="Series"/> to be saved</param>
        /// <returns>True if successful</returns>
        internal static async Task<bool> NewSeries(Series newSeries)
        {
            if (await DatabaseInteraction.NewSeries(newSeries))
            {
                CurrentUser.Media.AddSeries(newSeries);
                return true;
            }
            DisplayNotification("Unable to add new television series.", "Media Tracker");
            return false;
        }

        #endregion Save

        #endregion Television

        #region Notification Management

        /// <summary>Displays a new Notification in a thread-safe way.</summary>
        /// <param name="message">Message to be displayed</param>
        /// <param name="title">Title of the Notification window</param>
        internal static void DisplayNotification(string message, string title) => Application.Current.Dispatcher.Invoke(
            () => new Notification(message, title, NotificationButtons.OK, MainWindow).ShowDialog());

        /// <summary>Displays a new Notification in a thread-safe way and retrieves a boolean result upon its closing.</summary>
        /// <param name="message">Message to be displayed</param>
        /// <param name="title">Title of the Notification window</param>
        /// <returns>Returns value of clicked button on Notification.</returns>
        internal static bool YesNoNotification(string message, string title) => Application.Current.Dispatcher.Invoke(() => (new Notification(message, title, NotificationButtons.YesNo, MainWindow).ShowDialog() == true));

        #endregion Notification Management
    }
}