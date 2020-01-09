using PersonalTracker.Finances.Models.Categories;
using PersonalTracker.Finances.Models.Data;
using PersonalTracker.Fuel.Models;
using PersonalTracker.Lenses.Models;
using PersonalTracker.Media.Models.MediaTypes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonalTracker.Models.Database
{
    /// <summary>Represents all required database interaction methods regardless of implementation.</summary>
    internal interface IDatabaseInteraction
    {
        #region Database Interaction

        /// <summary>Creates a database for the current <see cref="User"/>.</summary>
        void VerifyUserDatabaseIntegrity();

        /// <summary>Verifies that the requested database exists and that its file size is greater than zero. If not, it extracts the embedded database file to the local output folder.</summary>
        void VerifyDatabaseIntegrity();

        #endregion Database Interaction

        #region User Management

        /// <summary>Attempts login of a <see cref="User"/>.</summary>
        /// <param name="username">Username attempting to log in</param>
        /// <param name="password">Hashed PBKDF2 password</param>
        /// <returns>True if user exists and passwords match</returns>
        Task<bool> AttemptLogin(string username, string password);

        /// <summary>Creates a new <see cref="User"/>.</summary>
        /// <param name="createUser"><see cref="User"/> to be created</param>
        /// <returns>True if successful</returns>
        Task<bool> CreateUser(User createUser);

        /// <summary>Deletes a <see cref="User"/>.</summary>
        /// <param name="deleteUser"><see cref="User"/> to be deleted</param>
        /// <returns>True if successful</returns>
        Task<bool> DeleteUser(User deleteUser);

        /// <summary>Gets the next index in the User table.</summary>
        /// <returns>Next index in the User table.</returns>
        Task<int> GetNextUserIndex();

        /// <summary>Loads a <see cref="User"/> from the database.</summary>
        /// <param name="username">Username to load</param>
        /// <returns><see cref="User"/></returns>
        Task<User> LoadUser(string username);

        /// <summary>Modifies a <see cref="User"/>.</summary>
        /// <param name="oldUser"><see cref="User"/> to be modified</param>
        /// <param name="newUser"><see cref="User"/></param>
        /// <returns>True if successful</returns>
        Task<bool> ModifyUser(User oldUser, User newUser);

        #endregion User Management

        #region Finances

        #region Load

        /// <summary>Loads all <see cref="Account"/>s.</summary>
        /// <returns>Returns all <see cref="Account"/>s</returns>
        Task<List<Account>> LoadAccounts();

        /// <summary>Loads all <see cref="Account"/> types.</summary>
        /// <returns>Returns all <see cref="Account"/> types</returns>
        Task<List<string>> LoadAccountTypes();

        /// <summary>Loads all <see cref="Category"/>s.</summary>
        /// <returns>Returns all <see cref="Category"/>s</returns>
        Task<List<Category>> LoadCategories();

        /// <summary>Loads all <see cref="CreditScore"/>s from the database.</summary>
        /// <returns>List of all <see cref="CreditScore"/>s</returns>
        Task<List<CreditScore>> LoadCreditScores();

        #endregion Load

        #region Account Manipulation

        /// <summary>Adds an <see cref="Account"/> to the database.</summary>
        /// <param name="account"><see cref="Account"/> to be added</param>
        /// <returns>Returns true if successful</returns>
        Task<bool> AddAccount(Account account);

        /// <summary>Deletes an <see cref="Account"/> from the database.</summary>
        /// <param name="account"><see cref="Account"/> to be deleted</param>
        /// <returns>Returns true if successful</returns>
        Task<bool> DeleteAccount(Account account);

        /// <summary>Renames an <see cref="Account"/> in the database.</summary>
        /// <param name="account"><see cref="Account"/> to be renamed</param>
        /// <param name="newAccountName">New <see cref="Account"/>'s name</param>
        /// <returns>Returns true if successful</returns>
        Task<bool> RenameAccount(Account account, string newAccountName);

        #endregion Account Manipulation

        #region Category Management

        /// <summary>Inserts a new <see cref="Category"/> into the database.</summary>
        /// <param name="selectedCategory">Selected Major <see cref="Category"/></param>
        /// <param name="newName">Name for new <see cref="Category"/></param>
        /// <param name="isMajor">Is the <see cref="Category"/> being added a Major <see cref="Category"/>?</param>
        /// <returns>Returns true if successful.</returns>
        Task<bool> AddCategory(Category selectedCategory, string newName, bool isMajor);

        /// <summary>Renames a <see cref="Category"/> in the database.</summary>
        /// <param name="selectedCategory"><see cref="Category"/> to rename</param>
        /// <param name="newName">New name of the <see cref="Category"/></param>
        /// <param name="oldName">Old name of the <see cref="Category"/></param>
        /// <param name="isMajor">Is the <see cref="Category"/> being renamed a Major <see cref="Category"/>?</param>
        /// <returns>True if successful</returns>
        Task<bool> RenameCategory(Category selectedCategory, string newName, string oldName, bool isMajor);

        /// <summary>Removes a Major <see cref="Category"/> from the database, as well as removes it from all <see cref="FinancialTransaction"/>s which utilize it.</summary>
        /// <param name="selectedCategory">Selected Major Category to delete</param>
        /// <returns>Returns true if operation successful</returns>
        Task<bool> RemoveMajorCategory(Category selectedCategory);

        /// <summary>Removes a Minor <see cref="Category"/> from the database, as well as removes it from all <see cref="FinancialTransaction"/>s which utilize it.</summary>
        /// <param name="selectedCategory">Selected Major <see cref="Category"/></param>
        /// <param name="minorCategory">Selected Minor <see cref="Category"/> to delete</param>
        /// <returns>Returns true if operation successful</returns>
        Task<bool> RemoveMinorCategory(Category selectedCategory, string minorCategory);

        #endregion Category Management

        #region Credit Score Management

        /// <summary>Adds a new <see cref="CreditScore"/> to the database.</summary>
        /// <param name="newScore"><see cref="CreditScore"/> to be added</param>
        /// <returns>True if successful</returns>
        Task<bool> AddCreditScore(CreditScore newScore);

        /// <summary>Deletes a <see cref="CreditScore"/> from the database</summary>
        /// <param name="deleteScore"><see cref="CreditScore"/> to be deleted</param>
        /// <returns>True if successful</returns>
        Task<bool> DeleteCreditScore(CreditScore deleteScore);

        /// <summary>Modifies a <see cref="CreditScore"/> in the database.</summary>
        /// <param name="oldScore">Original <see cref="CreditScore"/></param>
        /// <param name="newScore">Modified <see cref="CreditScore"/></param>
        /// <returns>True if successful</returns>
        Task<bool> ModifyCreditScore(CreditScore oldScore, CreditScore newScore);

        #endregion Credit Score Management

        #region Financial Transaction Management

        /// <summary>Adds a <see cref="FinancialTransaction"/> to an account and the database</summary>
        /// <param name="transaction"><see cref="FinancialTransaction"/> to be added</param>
        /// <param name="account"><see cref="Account"/> the <see cref="FinancialTransaction"/> will be added to</param>
        /// <returns>Returns true if successful</returns>
        Task<bool> AddFinancialTransaction(FinancialTransaction transaction, Account account);

        /// <summary>Deletes a <see cref="FinancialTransaction"/> from the database.</summary>
        /// <param name="transaction"><see cref="FinancialTransaction"/> to be deleted</param>
        /// <param name="account"><see cref="Account"/> the <see cref="FinancialTransaction"/> will be deleted from</param>
        /// <returns>Returns true if successful</returns>
        Task<bool> DeleteFinancialTransaction(FinancialTransaction transaction, Account account);

        /// <summary>Gets the next <see cref="FinancialTransaction"/> ID autoincrement value in the database for the <see cref="FinancialTransaction"/>s table.</summary>
        /// <returns>Next <see cref="FinancialTransaction"/>s ID value</returns>
        Task<int> GetNextFinancialTransactionIndex();

        /// <summary>Modifies the selected <see cref="FinancialTransaction"/> in the database.</summary>
        /// <param name="newTransaction">Transaction to replace the current one in the database</param>
        /// <param name="oldTransaction">Current <see cref="FinancialTransaction"/> in the database</param>
        /// <returns>Returns true if successful</returns>
        Task<bool> ModifyFinancialTransaction(FinancialTransaction newTransaction, FinancialTransaction oldTransaction);

        #endregion Financial Transaction Management

        #endregion Finances

        #region Contact Lens Manipulation

        /// <summary>Adds a new <see cref="Contact"/> insertion to the database.</summary>
        /// <param name="newContact"><see cref="Contact"/> insertion to be added</param>
        Task<bool> AddContact(Contact newContact);

        /// <summary>Loads all <see cref="Contact"/> insertions from the database.</summary>
        /// <returns>All <see cref="Contact"/> insertions</returns>
        Task<List<Contact>> LoadContacts();

        /// <summary>Modifies an existing <see cref="Contact"/> in the database.</summary>
        /// <param name="originalContact"><see cref="Contact"/> to be modified</param>
        /// <param name="newContact"><see cref="Contact"/> with modifications</param>
        /// <returns>True if successful</returns>
        Task<bool> ModifyContact(Contact originalContact, Contact newContact);

        /// <summary>Removes a <see cref="Contact"/> from the database.</summary>
        /// <param name="removeContact"><see cref="Contact"/> to be removed</param>
        /// <returns>True if successful</returns>
        Task<bool> RemoveContact(Contact removeContact);

        #endregion Contact Lens Manipulation

        #region Fuel

        #region Fuel Transaction Management

        /// <summary>Deletes a <see cref="FuelTransaction"/> from the database.</summary>
        /// <param name="deleteTransaction"><see cref="FuelTransaction"/> to be deleted</param>
        /// <returns>Returns true if deletion is successful</returns>
        Task<bool> DeleteFuelTransaction(FuelTransaction deleteTransaction);

        /// <summary>Gets the next <see cref="FuelTransaction"/> ID autoincrement value in the database for the <see cref="Vehicle"/ table.</summary>
        /// <returns>Next <see cref="FuelTransaction"/> ID value</returns>
        Task<int> GetNextFuelTransactionIndex();

        /// <summary>Loads all <see cref="FuelTransaction"/>s associated with a specific <see cref="Vehicle"/.</summary>
        /// <param name="vehicleID"><see cref="Vehicle"/> ID</param>
        /// <returns>Returns all <see cref="FuelTransaction"/>s associated with a specific <see cref="Vehicle"/.</returns>
        Task<List<FuelTransaction>> LoadFuelTransactions(int vehicleID);

        /// <summary>Modifies an existing <see cref="FuelTransaction"/>.</summary>
        /// <param name="oldTransaction">Existing <see cref="FuelTransaction"/></param>
        /// <param name="newTransaction">New <see cref="FuelTransaction"/></param>
        /// <returns>Returns true if modification is successful</returns>
        Task<bool> ModifyFuelTransaction(FuelTransaction oldTransaction, FuelTransaction newTransaction);

        /// <summary>Adds a new <see cref="FuelTransaction"/> to the database.</summary>
        /// <param name="newTransaction"><see cref="FuelTransaction"/> to be added</param>
        /// <returns>Returns true if add is successful</returns>
        Task<bool> NewFuelTransaction(FuelTransaction newTransaction);

        #endregion Fuel Transaction Management

        #region Vehicle Management

        /// <summary>Changes details in the database regarding a <see cref="Vehicle"/>.</summary>
        /// <param name="oldVehicle">Old <see cref="Vehicle"/> details</param>
        /// <param name="newVehicle">New <see cref="Vehicle"/> details</param>
        /// <returns>Returns true if modification is successful</returns>
        Task<bool> ModifyVehicle(Vehicle oldVehicle, Vehicle newVehicle);

        /// <summary>Deletes a <see cref="Vehicle"/> and all associated <see cref="FuelTransaction"/>s from the database.</summary>
        /// <param name="deleteVehicle"><see cref="Vehicle"/> to be deleted</param>
        /// <returns>Returns true if deletion is successful.</returns>
        Task<bool> DeleteVehicle(Vehicle deleteVehicle);

        /// <summary>Gets the next <see cref="Vehicle"/> ID autoincrement value in the database for the Vehicle table.</summary>
        /// <returns>Next <see cref="Vehicle"/> ID value</returns>
        Task<int> GetNextVehicleIndex();

        /// <summary>Loads all <see cref="Vehicle"/> associated with a <see cref="User"/>.</summary>
        /// <returns>All <see cref="Vehicle"/> associated with a <see cref="User"/></returns>
        Task<List<Vehicle>> LoadVehicles();

        /// <summary>Adds a new <see cref="Vehicle"/> to the database.</summary>
        /// <param name="newVehicle"><see cref="Vehicle"/> to be added</param>
        /// <returns>Returns true if the <see cref="Vehicle"/> was successfully added</returns>
        Task<bool> NewVehicle(Vehicle newVehicle);

        #endregion Vehicle Management

        #endregion Fuel

        #region Books

        /// <summary>Deletes a <see cref="Book"/> from the database.</summary>
        /// <param name="deleteBook"><see cref="Book"/> to be deleted</param>
        /// <returns>True if successful</returns>
        Task<bool> DeleteBook(Book deleteBook);

        /// <summary>Loads all <see cref="Book"/>s from the database.</summary>
        /// <returns>All <see cref="Book"/>s</returns>
        Task<List<Book>> LoadBooks();

        /// <summary>Modifies a <see cref="Book"/> in the database.</summary>
        /// <param name="oldBook">Original <see cref="Book"/></param>
        /// <param name="newBook"><see cref="Book"/> to replace original</param>
        /// <returns>True if successful</returns>
        Task<bool> ModifyBook(Book oldBook, Book newBook);

        /// <summary>Saves a new <see cref="Book"/> to the database.</summary>
        /// <param name="newBook"><see cref="Book"/> to be saved</param>
        Task<bool> NewBook(Book newBook);

        #endregion Books

        #region Television

        #region Delete

        /// <summary>Deletes a <see cref="Series"/> from the database.</summary>
        /// <param name="deleteSeries"><see cref="Series"/> to be deleted</param>
        /// <returns>True if successful</returns>
        Task<bool> DeleteSeries(Series deleteSeries);

        #endregion Delete

        #region Load

        /// <summary>Loads all <see cref="Series"/> from the database.</summary>
        /// <returns>All <see cref="Series"/></returns>
        Task<List<Series>> LoadSeries();

        #endregion Load

        #region Save

        /// <summary>Modifies a <see cref="Series"/> in the database.</summary>
        /// <param name="oldSeries">Original <see cref="Series"/></param>
        /// <param name="newSeries"><see cref="Series"/> to replace original</param>
        /// <returns>True if successful</returns>
        Task<bool> ModifySeries(Series oldSeries, Series newSeries);

        /// <summary>Saves a new <see cref="Series"/> to the database.</summary>
        /// <param name="newSeries"><see cref="Series"/> to be saved</param>
        /// <returns>True if successful</returns>
        Task<bool> NewSeries(Series newSeries);

        #endregion Save

        #endregion Television
    }
}