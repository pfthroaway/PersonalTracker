using PersonalTracker.Models.FuelModels;
using PersonalTracker.Models.LensesModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonalTracker.Models.Database
{
    internal interface IDatabaseInteraction
    {
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

        #region Contact Lens Manipulation

        /// <summary>Adds a new contact insertion to the database.</summary>
        /// <param name="newContact">Contact insertion to be added</param>
        Task<bool> AddContact(Contact newContact);

        /// <summary>Loads all contact insertions from the database.</summary>
        /// <returns>All contact insertions</returns>
        Task<List<Contact>> LoadContacts();

        /// <summary>Modifies an existing contact in the database.</summary>
        /// <param name="originalContact">Contact to be modified</param>
        /// <param name="newContact">Contact with modifications</param>
        Task<bool> ModifyContact(Contact originalContact, Contact newContact);

        /// <summary>Removes a contact from the database.</summary>
        /// <param name="removeContact">Contact to be removed</param>
        Task<bool> RemoveContact(Contact removeContact);

        #endregion Contact Lens Manipulation

        #region Fuel

        #region Fuel Transaction Management

        /// <summary>Deletes a Transaction from the database.</summary>
        /// <param name="deleteTransaction">Transaction to be deleted</param>
        /// <returns>Returns true if deletion successful</returns>
        Task<bool> DeleteFuelTransaction(FuelTransaction deleteTransaction);

        /// <summary>Gets the next TransactionID autoincrement value in the database for the Vehicle table.</summary>
        /// <returns>Next TransactionID value</returns>
        Task<int> GetNextFuelTransactionIndex();

        /// <summary>Loads all Transactions associated with a specific Vehicle.</summary>
        /// <param name="vehicleID">Vehicle ID</param>
        /// <returns>Returns all Transactions associated with a specific Vehicle.</returns>
        Task<List<FuelTransaction>> LoadFuelTransactions(int vehicleID);

        /// <summary>Modifies an existing Transaction.</summary>
        /// <param name="oldTransaction">Existing Transaction</param>
        /// <param name="newTransaction">New Transaction</param>
        /// <returns>Returns true if modification successful</returns>
        Task<bool> ModifyFuelTransaction(FuelTransaction oldTransaction, FuelTransaction newTransaction);

        /// <summary>Adds a new Transaction to the database.</summary>
        /// <param name="newTransaction">Transaction to be added</param>
        /// <returns>Returns true if add successful</returns>
        Task<bool> NewFuelTransaction(FuelTransaction newTransaction);

        #endregion Fuel Transaction Management

        #region Vehicle Management

        /// <summary>Changes details in the database regarding a Vehicle.</summary>
        /// <param name="oldVehicle">Old Vehicle details</param>
        /// <param name="newVehicle">New Vehicle details</param>
        /// <returns>Returns true if modification successful</returns>
        Task<bool> ModifyVehicle(Vehicle oldVehicle, Vehicle newVehicle);

        /// <summary>Deletes a Vehicle and all associated Transactions from the database.</summary>
        /// <param name="deleteVehicle">Vehicle to be deleted</param>
        /// <returns>Returns true if deletion is successful.</returns>
        Task<bool> DeleteVehicle(Vehicle deleteVehicle);

        /// <summary>Gets the next VehicleID autoincrement value in the database for the Vehicle table.</summary>
        /// <returns>Next VehicleID value</returns>
        Task<int> GetNextVehicleIndex();

        /// <summary>Loads all Vehicles associated with a User.</summary>
        /// <returns>All Vehicles associated with a User</returns>
        Task<List<Vehicle>> LoadVehicles();

        /// <summary>Adds a new Vehicle to the database.</summary>
        /// <param name="newVehicle">Vehicle to be added</param>
        /// <returns>Returns whether the Vehicle was successfully added</returns>
        Task<bool> NewVehicle(Vehicle newVehicle);

        #endregion Vehicle Management

        #endregion Fuel
    }
}