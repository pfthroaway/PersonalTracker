using Extensions;
using Extensions.DatabaseHelp;
using Extensions.DataTypeHelpers;
using Extensions.Encryption;
using PersonalTracker.Models.FinanceModels.Categories;
using PersonalTracker.Models.FinanceModels.Data;
using PersonalTracker.Models.FinanceModels.Enums;
using PersonalTracker.Models.FuelModels;
using PersonalTracker.Models.LensesModels;
using PersonalTracker.Models.MediaModels.Enums;
using PersonalTracker.Models.MediaModels.MediaTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PersonalTracker.Models.Database
{
    internal class SQLiteDatabaseInteraction : IDatabaseInteraction
    {
        private const string _DATABASENAME = "Users.sqlite";
        private readonly string _con = $"Data Source={_DATABASENAME}; foreign keys = TRUE; Version = 3;";

        #region Database Interaction

        [Obsolete]
        /// <summary>Creates a database for the current <see cref="User"/>.</summary>
        public async void CreateCurrentUserDatabase()
        {
            SQLiteConnection.CreateFile(AppState.CurrentUserDatabaseName);

            SQLiteCommand cmd = new SQLiteCommand
            {
                CommandText = "CREATE TABLE `AccountTypes` (`Name` TEXT NOT NULL UNIQUE, PRIMARY KEY(`Name`)); " +
               "CREATE TABLE `Accounts` (`Name` TEXT NOT NULL UNIQUE, `Type` TEXT NOT NULL, `Balance` NUMERIC NOT NULL, PRIMARY KEY(`Name`)); " +
               "CREATE TABLE `CreditScores` (`ID` INTEGER PRIMARY KEY AUTOINCREMENT,`Date` TEXT NOT NULL, `Source` TEXT NOT NULL, `Score` INTEGER NOT NULL, `Provider` TEXT NOT NULL, `FICO` INTEGER NOT NULL); " +
               "CREATE TABLE `MajorCategories` (`Name` TEXT NOT NULL UNIQUE, PRIMARY KEY(`Name`)); " +
               "CREATE TABLE `MinorCategories` ( `MajorCategory` TEXT NOT NULL, `MinorCategory` TEXT NOT NULL, FOREIGN KEY(`MajorCategory`) REFERENCES `MajorCategories`(`Name`) ON UPDATE CASCADE ON DELETE CASCADE, PRIMARY KEY(`MajorCategory`,`MinorCategory`)); " +
               "CREATE TABLE `FinanceTransactions` (`ID` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, `Date` TEXT NOT NULL, `Payee` TEXT NOT NULL, `MajorCategory` TEXT NOT NULL, `MinorCategory` TEXT NOT NULL, `Memo` TEXT, `Outflow` NUMERIC NOT NULL, `Inflow` NUMERIC NOT NULL, `Account` TEXT NOT NULL, FOREIGN KEY(`Account`) REFERENCES `Accounts`(`Name`) ON UPDATE CASCADE ON DELETE CASCADE);" +
               "CREATE TABLE `Contacts` (`Date`	TEXT NOT NULL, `Side` TEXT NOT NULL, `ReplacementDate` TEXT NOT NULL); " +
               "CREATE TABLE `FuelTransactions` (`TransactionID` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, `VehicleID` INTEGER NOT NULL, `Store` TEXT NOT NULL, `Date` TEXT NOT NULL, `Octane`	INTEGER NOT NULL, `Distance` NUMERIC NOT NULL, `Gallons`	NUMERIC NOT NULL, `Price` NUMERIC NOT NULL, `Odometer` NUMERIC NOT NULL, `Range` INTEGER, FOREIGN KEY(`VehicleID`) REFERENCES `Vehicles`(`VehicleID`)); " +
               "CREATE TABLE `Vehicles` (`VehicleID` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, `UserID` INTEGER NOT NULL, `Nickname`	TEXT NOT NULL, `Make` TEXT NOT NULL, `Model` TEXT NOT NULL, `Year` TEXT NOT NULL); " +
               "CREATE TABLE `Books` (`Name` TEXT NOT NULL COLLATE NOCASE, `Author` TEXT NOT NULL COLLATE NOCASE); " +
               "CREATE TABLE `Films` (`Name` TEXT NOT NULL COLLATE NOCASE, `Released` TEXT NOT NULL, `Rating` NUMERIC NOT NULL); " +
               "CREATE TABLE `Music` (`Artist` TEXT NOT NULL COLLATE NOCASE, `Album` TEXT NOT NULL); " +
               "CREATE TABLE `Series` (`Name` TEXT NOT NULL COLLATE NOCASE, `PremiereDate` TEXT, `Rating` NUMERIC, `Seasons` INTEGER, `Episodes` INTEGER, `Status` INTEGER NOT NULL, `Channel` TEXT, `FinaleDate` TEXT, `Day` INTEGER, `Time` TEXT, `ReturnDate` TEXT); "
            };

            await SQLite.ExecuteCommand(AppState.CurrentUserConnection, cmd);
        }

        /// <summary>Creates a database for the current <see cref="User"/>.</summary>
        public void VerifyUserDatabaseIntegrity()
        {
            if (!File.Exists(AppState.CurrentUserDatabaseName))
            {
                Functions.VerifyFileIntegrity(Assembly.GetExecutingAssembly().GetManifestResourceStream($"PersonalTracker.UserDB.sqlite"), "UserDB.sqlite");
                File.Move("UserDB.sqlite", AppState.CurrentUserDatabaseName);
            }
        }

        /// <summary>Verifies that the requested database exists and that its file size is greater than zero. If not, it extracts the embedded database file to the local output folder.</summary>
        public void VerifyDatabaseIntegrity() => Functions.VerifyFileIntegrity(
            Assembly.GetExecutingAssembly().GetManifestResourceStream($"PersonalTracker.{_DATABASENAME}"), _DATABASENAME);

        #endregion Database Interaction

        #region User Management

        /// <summary>Attempts login of a <see cref="User"/>.</summary>
        /// <param name="username">Username attempting to log in</param>
        /// <param name="password">Hashed PBKDF2 password</param>
        /// <returns>True if user exists and passwords match</returns>
        public async Task<bool> AttemptLogin(string username, string password)
        {
            SQLiteCommand cmd = new SQLiteCommand { CommandText = "SELECT * FROM Users WHERE Username = @name" };
            cmd.Parameters.AddWithValue("name", username);

            DataSet ds = await SQLite.FillDataSet(_con, cmd);

            return ds.Tables[0].Rows.Count > 0 && PBKDF2.ValidatePassword(password, ds.Tables[0].Rows[0]["Password"].ToString());
        }

        /// <summary>Creates a new <see cref="User"/>.</summary>
        /// <param name="createUser"><see cref="User"/> to be created</param>
        /// <returns>True if successful</returns>
        public async Task<bool> CreateUser(User createUser)
        {
            SQLiteCommand cmd = new SQLiteCommand
            {
                CommandText = "SELECT * FROM Users WHERE [Username] = @username"
            };
            cmd.Parameters.AddWithValue("@username", createUser.Username);
            DataSet ds = await SQLite.FillDataSet(_con, cmd);

            if (ds.Tables[0].Rows.Count == 0)
            {
                cmd = new SQLiteCommand
                {
                    CommandText = "INSERT INTO Users([Username], [Password]) VALUES(@username, @password)"
                };
                cmd.Parameters.AddWithValue("@username", createUser.Username);
                cmd.Parameters.AddWithValue("@password", createUser.Password);
                return await SQLite.ExecuteCommand(_con, cmd);
            }
            else
            {
                AppState.DisplayNotification("Username has already been taken. Please choose another.", "Personal Tracker");
                return false;
            }
        }

        /// <summary>Deletes a <see cref="User"/>.</summary>
        /// <param name="deleteUser"><see cref="User"/> to be deleted</param>
        /// <returns>True if successful</returns>
        public Task<bool> DeleteUser(User deleteUser)
        {
            SQLiteCommand cmd = new SQLiteCommand
            {
                CommandText = "DELETE FROM Users WHERE [UserID] = @id"
            };
            cmd.Parameters.AddWithValue("@id", deleteUser.UserID);

            return SQLite.ExecuteCommand(_con, cmd);
        }

        /// <summary>Gets the next index in the User table.</summary>
        /// <returns>Next index in the User table.</returns>
        public async Task<int> GetNextUserIndex()
        {
            DataSet ds = await SQLite.FillDataSet(_con, "SELECT * FROM SQLITE_SEQUENCE WHERE name = 'Users'");

            return ds.Tables[0].Rows.Count > 0 ? Int32Helper.Parse(ds.Tables[0].Rows[0]["seq"]) + 1 : 1;
        }

        /// <summary>Loads a <see cref="User"/> from the database.</summary>
        /// <param name="username">Username to load</param>
        /// <returns><see cref="User"/></returns>
        public async Task<User> LoadUser(string username)
        {
            SQLiteCommand cmd = new SQLiteCommand
            {
                CommandText = "SELECT * FROM Users WHERE [Username] = @username"
            };

            cmd.Parameters.AddWithValue("@username", username);
            DataSet ds = await SQLite.FillDataSet(_con, cmd);

            return new User(Int32Helper.Parse(ds.Tables[0].Rows[0]["UserID"]), ds.Tables[0].Rows[0]["Username"].ToString(), ds.Tables[0].Rows[0]["Password"].ToString());
        }

        /// <summary>Modifies a <see cref="User"/>.</summary>
        /// <param name="oldUser"><see cref="User"/> to be modified</param>
        /// <param name="newUser"><see cref="User"/></param>
        /// <returns>True if successful</returns>
        public Task<bool> ModifyUser(User oldUser, User newUser)
        {
            SQLiteCommand cmd = new SQLiteCommand
            {
                CommandText = "UPDATE Users SET [Username] = @name, [Password] = @password WHERE [Username] = @oldName AND [Password] = @oldPassword"
            };

            cmd.Parameters.AddWithValue("@name", newUser.Username);
            cmd.Parameters.AddWithValue("@password", newUser.Password);
            cmd.Parameters.AddWithValue("@oldName", oldUser.Username);
            cmd.Parameters.AddWithValue("@oldPassword", oldUser.Password);

            return SQLite.ExecuteCommand(_con, cmd);
        }

        #endregion User Management

        #region Finances

        #region Load

        /// <summary>Loads all Accounts.</summary>
        /// <returns>Returns all Accounts</returns>
        public async Task<List<Account>> LoadAccounts()
        {
            List<Account> allAccounts = new List<Account>();
            DataSet ds = await SQLite.FillDataSet(AppState.CurrentUserConnection, "SELECT * FROM Accounts");

            if (ds.Tables[0].Rows.Count > 0)
                allAccounts.AddRange(from DataRow dr in ds.Tables[0].Rows select new Account(dr["Name"].ToString(), EnumHelper.Parse<AccountTypes>(dr["Type"].ToString()), new List<FinancialTransaction>()));

            ds = await SQLite.FillDataSet(AppState.CurrentUserConnection, "SELECT * FROM FinancialTransactions");
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Account selectedAccount = allAccounts.Find(account => account.Name == dr["Account"].ToString());

                    FinancialTransaction newTransaction = new FinancialTransaction(Int32Helper.Parse(dr["ID"]),
                        DateTimeHelper.Parse(dr["Date"]), dr["Payee"].ToString(), dr["MajorCategory"].ToString(),
                        dr["MinorCategory"].ToString(), dr["Memo"].ToString(), DecimalHelper.Parse(dr["Outflow"]),
                        DecimalHelper.Parse(dr["Inflow"]), selectedAccount.Name);
                    selectedAccount.AddTransaction(newTransaction);
                }
            }

            allAccounts = allAccounts.OrderBy(account => account.Name).ToList();
            if (allAccounts.Count > 0)
            {
                foreach (Account account in allAccounts)
                    account.Sort();
            }

            return allAccounts;
        }

        /// <summary>Loads all Account types.</summary>
        /// <returns>Returns all Account types</returns>
        public async Task<List<string>> LoadAccountTypes()
        {
            List<string> allAccountTypes = new List<string>();
            DataSet ds = await SQLite.FillDataSet(AppState.CurrentUserConnection, "SELECT * FROM AccountTypes");

            if (ds.Tables[0].Rows.Count > 0)
                allAccountTypes.AddRange(from DataRow dr in ds.Tables[0].Rows select dr["Name"].ToString());

            return allAccountTypes;
        }

        /// <summary>Loads all Categories.</summary>
        /// <returns>Returns all Categories</returns>
        public async Task<List<Category>> LoadCategories()
        {
            List<Category> allCategories = new List<Category>();
            DataSet ds = await SQLite.FillDataSet(AppState.CurrentUserConnection, "SELECT * FROM MajorCategories");

            if (ds.Tables[0].Rows.Count > 0)
                allCategories.AddRange(from DataRow dr in ds.Tables[0].Rows select new Category(dr["Name"].ToString(), new List<string>()));

            ds = await SQLite.FillDataSet(AppState.CurrentUserConnection, "SELECT * FROM MinorCategories");

            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Category selectedCategory = allCategories.Find(category => category.Name == dr["MajorCategory"].ToString());

                    selectedCategory.MinorCategories.Add(dr["MinorCategory"].ToString());
                }
            }

            allCategories = allCategories.OrderBy(category => category.Name).ToList();

            foreach (Category category in allCategories)
                category.Sort();

            return allCategories;
        }

        /// <summary>Loads all credit scores from the database.</summary>
        /// <returns>List of all credit scores</returns>
        public async Task<List<CreditScore>> LoadCreditScores()
        {
            List<CreditScore> scores = new List<CreditScore>();
            DataSet ds = await SQLite.FillDataSet(AppState.CurrentUserConnection, "SELECT * FROM CreditScores");
            if (ds.Tables[0].Rows.Count > 0)
            {
                scores.AddRange(from DataRow dr in ds.Tables[0].Rows select new CreditScore(DateTimeHelper.Parse(dr["Date"]), dr["Source"].ToString(), Int32Helper.Parse(dr["Score"]), EnumHelper.Parse<Providers>(dr["Provider"].ToString()), BoolHelper.Parse(dr["FICO"])));
            }

            return scores.OrderByDescending(score => score.Date).ToList();
        }

        #endregion Load

        #region Account Manipulation

        /// <summary>Adds an account to the database.</summary>
        /// <param name="account">Account to be added</param>
        /// <returns>Returns true if successful</returns>
        public Task<bool> AddAccount(Account account)
        {
            SQLiteCommand cmd = new SQLiteCommand
            {
                CommandText =
                    "INSERT INTO Accounts([Name], [Type], [Balance])VALUES(@name, @type, @balance)"
            };

            cmd.Parameters.AddWithValue("@name", account.Name);
            cmd.Parameters.AddWithValue("@type", account.AccountType);
            cmd.Parameters.AddWithValue("@balance", account.Balance);
            return SQLite.ExecuteCommand(AppState.CurrentUserConnection, cmd);
        }

        /// <summary>Deletes an account from the database.</summary>
        /// <param name="account">Account to be deleted</param>
        /// <returns>Returns true if successful</returns>
        public Task<bool> DeleteAccount(Account account)
        {
            SQLiteCommand cmd = new SQLiteCommand { CommandText = "DELETE FROM Accounts WHERE [Name] = @name" };
            cmd.Parameters.AddWithValue("@name", account.Name);

            return SQLite.ExecuteCommand(AppState.CurrentUserConnection, cmd);
        }

        /// <summary>Renames an account in the database.</summary>
        /// <param name="account">Account to be renamed</param>
        /// <param name="newAccountName">New account's name</param>
        /// <returns>Returns true if successful</returns>
        public Task<bool> RenameAccount(Account account, string newAccountName)
        {
            string oldAccountName = account.Name;
            SQLiteCommand cmd = new SQLiteCommand
            {
                CommandText = "UPDATE Accounts SET [Name] = @newAccountName WHERE [Name] = @oldAccountName"
            };
            cmd.Parameters.AddWithValue("@newAccountName", newAccountName);
            cmd.Parameters.AddWithValue("@oldAccountName", oldAccountName);

            return SQLite.ExecuteCommand(AppState.CurrentUserConnection, cmd);
        }

        #endregion Account Manipulation

        #region Category Management

        /// <summary>Inserts a new Category into the database.</summary>
        /// <param name="selectedCategory">Selected Major Category</param>
        /// <param name="newName">Name for new Category</param>
        /// <param name="isMajor">Is the category being added a Major Category?</param>
        /// <returns>Returns true if successful.</returns>
        public Task<bool> AddCategory(Category selectedCategory, string newName, bool isMajor)
        {
            SQLiteCommand cmd = new SQLiteCommand
            {
                CommandText = isMajor
                    ? "INSERT INTO MajorCategories([Name])VALUES(@majorCategory)"
                    : "INSERT INTO MinorCategories([MajorCategory], [MinorCategory])VALUES(@majorCategory, @minorCategory)"
            };

            if (isMajor)
                cmd.Parameters.AddWithValue("@majorCategory", newName);
            else
            {
                cmd.Parameters.AddWithValue("@majorCategory", selectedCategory.Name);
                cmd.Parameters.AddWithValue("@minorCategory", newName);
            }

            return SQLite.ExecuteCommand(AppState.CurrentUserConnection, cmd);
        }

        /// <summary>Rename a category in the database.</summary>
        /// <param name="selectedCategory">Category to rename</param>
        /// <param name="newName">New name of the Category</param>
        /// <param name="oldName">Old name of the Category</param>
        /// <param name="isMajor">Is the category being renamed a Major Category?</param>
        /// <returns></returns>
        public Task<bool> RenameCategory(Category selectedCategory, string newName, string oldName, bool isMajor)
        {
            SQLiteCommand cmd = new SQLiteCommand
            {
                CommandText = isMajor
                    ? "UPDATE MajorCategories SET [Name] = @newName WHERE [Name] = @oldName; UPDATE MinorCategories SET [MajorCategory] = @newName WHERE [MajorCategory] = @oldName"
                    : "UPDATE MinorCategories SET [MinorCategory] = @newName WHERE [MinorCategory] = @oldName AND [MajorCategory] = @majorCategory"
            };
            cmd.Parameters.AddWithValue("@newName", newName);
            cmd.Parameters.AddWithValue("@oldName", oldName);
            cmd.Parameters.AddWithValue("@majorCategory", selectedCategory.Name);

            return SQLite.ExecuteCommand(AppState.CurrentUserConnection, cmd);
        }

        /// <summary>Removes a Major Category from the database, as well as removes it from all Transactions which utilize it.</summary>
        /// <param name="selectedCategory">Selected Major Category to delete</param>
        /// <returns>Returns true if operation successful</returns>
        public Task<bool> RemoveMajorCategory(Category selectedCategory)
        {
            SQLiteCommand cmd = new SQLiteCommand
            {
                CommandText =
                "DELETE FROM MajorCategories WHERE [Name] = @name; DELETE FROM MinorCategories WHERE [MajorCategory] = @name; UPDATE FinancialTransactions SET [MajorCategory] = @newName AND [MinorCategory] = @newName WHERE [MinorCategory] = @name"
            };
            cmd.Parameters.AddWithValue("@name", selectedCategory.Name);
            cmd.Parameters.AddWithValue("@newName", "");

            return SQLite.ExecuteCommand(AppState.CurrentUserConnection, cmd);
        }

        /// <summary>Removes a Major Category from the database, as well as removes it from all Transactions which utilize it.</summary>
        /// <param name="selectedCategory">Selected Major Category</param>
        /// <param name="minorCategory">Selected Minor Category to delete</param>
        /// <returns>Returns true if operation successful</returns>
        public Task<bool> RemoveMinorCategory(Category selectedCategory, string minorCategory)
        {
            SQLiteCommand cmd = new SQLiteCommand
            {
                CommandText =
                "DELETE FROM MinorCategories WHERE [MajorCategory] = @majorCategory AND [MinorCategory] = @minorCategory; UPDATE FinancialTransactions SET [MinorCategory] = @newMinorName WHERE [MajorCategory] = @majorCategory AND [MinorCategory] = @minorCategory"
            };
            cmd.Parameters.AddWithValue("@majorCategory", selectedCategory.Name);
            cmd.Parameters.AddWithValue("@minorCategory", minorCategory);
            cmd.Parameters.AddWithValue("@newMinorName", "");

            return SQLite.ExecuteCommand(AppState.CurrentUserConnection, cmd);
        }

        #endregion Category Management

        #region Credit Score Management

        /// <summary>Adds a new credit score to the database.</summary>
        /// <param name="newScore">Score to be added</param>
        /// <returns>True if successful</returns>
        public Task<bool> AddCreditScore(CreditScore newScore)
        {
            SQLiteCommand cmd = new SQLiteCommand
            {
                CommandText =
                    "INSERT INTO CreditScores([Date], [Source], [Score], [Provider], [FICO])VALUES(@date, @source, @score, @provider, @fico)"
            };
            cmd.Parameters.AddWithValue("@date", newScore.DateToString);
            cmd.Parameters.AddWithValue("@source", newScore.Source);
            cmd.Parameters.AddWithValue("@score", newScore.Score);
            cmd.Parameters.AddWithValue("@provider", newScore.ProviderToString);
            cmd.Parameters.AddWithValue("@fico", Int32Helper.Parse(newScore.FICO));

            return SQLite.ExecuteCommand(AppState.CurrentUserConnection, cmd);
        }

        /// <summary>Deletes a credit score from the database</summary>
        /// <param name="deleteScore">Score to be deleted</param>
        /// <returns>True if successful</returns>
        public Task<bool> DeleteCreditScore(CreditScore deleteScore)
        {
            SQLiteCommand cmd = new SQLiteCommand
            {
                CommandText =
                    "DELETE FROM CreditScores WHERE [Date] = @date AND [Source] = @source AND [Score] = @score AND [Provider] = @provider AND [FICO] = @fico"
            };
            cmd.Parameters.AddWithValue("@date", deleteScore.DateToString);
            cmd.Parameters.AddWithValue("@source", deleteScore.Source);
            cmd.Parameters.AddWithValue("@score", deleteScore.Score);
            cmd.Parameters.AddWithValue("@provider", deleteScore.ProviderToString);
            cmd.Parameters.AddWithValue("@fico", Int32Helper.Parse(deleteScore.FICO));

            return SQLite.ExecuteCommand(AppState.CurrentUserConnection, cmd);
        }

        /// <summary>Modifies a credit score in the database.</summary>
        /// <param name="oldScore">Original score</param>
        /// <param name="newScore">Modified score</param>
        /// <returns>True if successful</returns>
        public Task<bool> ModifyCreditScore(CreditScore oldScore, CreditScore newScore)
        {
            SQLiteCommand cmd = new SQLiteCommand
            {
                CommandText =
                    "UPDATE CreditScores SET [Date] = @date, [Source] = @source, [Score] = @score, [Provider] = @provider, [FICO] = @fico WHERE [Date] = @oldDate AND [Source] = @oldSource AND [Score] = @oldScore AND [Provider] = @oldProvider AND [FICO] = @oldFico"
            };
            cmd.Parameters.AddWithValue("@date", newScore.DateToString);
            cmd.Parameters.AddWithValue("@source", newScore.Source);
            cmd.Parameters.AddWithValue("@score", newScore.Score);
            cmd.Parameters.AddWithValue("@provider", newScore.ProviderToString);
            cmd.Parameters.AddWithValue("@fico", Int32Helper.Parse(newScore.FICO));
            cmd.Parameters.AddWithValue("@oldDate", oldScore.DateToString);
            cmd.Parameters.AddWithValue("@oldSource", oldScore.Source);
            cmd.Parameters.AddWithValue("@oldScore", oldScore.Score);
            cmd.Parameters.AddWithValue("@oldProvider", oldScore.ProviderToString);
            cmd.Parameters.AddWithValue("@oldFico", Int32Helper.Parse(oldScore.FICO));

            return SQLite.ExecuteCommand(AppState.CurrentUserConnection, cmd);
        }

        #endregion Credit Score Management

        #region Transaction Management

        /// <summary>Adds a transaction to an account and the database</summary>
        /// <param name="transaction">Transaction to be added</param>
        /// <param name="account">Account the transaction will be added to</param>
        /// <returns>Returns true if successful</returns>
        public Task<bool> AddFinancialTransaction(FinancialTransaction transaction, Account account)
        {
            SQLiteCommand cmd = new SQLiteCommand
            {
                CommandText =
                    "INSERT INTO FinancialTransactions([Date], [Payee], [MajorCategory], [MinorCategory], [Memo], [Outflow], [Inflow], [Account]) VALUES(@date, @payee, @majorCategory, @minorCategory, @memo, @outflow, @inflow, @name); UPDATE Accounts SET [Balance] = @balance WHERE [Name] = @name"
            };
            cmd.Parameters.AddWithValue("@date", transaction.DateToString);
            cmd.Parameters.AddWithValue("@payee", transaction.Payee);
            cmd.Parameters.AddWithValue("@majorCategory", transaction.MajorCategory);
            cmd.Parameters.AddWithValue("@minorCategory", transaction.MinorCategory);
            cmd.Parameters.AddWithValue("@memo", transaction.Memo);
            cmd.Parameters.AddWithValue("@outflow", transaction.Outflow);
            cmd.Parameters.AddWithValue("@inflow", transaction.Inflow);
            cmd.Parameters.AddWithValue("@name", transaction.Account);
            cmd.Parameters.AddWithValue("@balance", account.Balance);

            return SQLite.ExecuteCommand(AppState.CurrentUserConnection, cmd);
        }

        /// <summary>Deletes a transaction from the database.</summary>
        /// <param name="transaction">Transaction to be deleted</param>
        /// <param name="account">Account the transaction will be deleted from</param>
        /// <returns>Returns true if successful</returns>
        public Task<bool> DeleteFinancialTransaction(FinancialTransaction transaction, Account account)
        {
            SQLiteCommand cmd = new SQLiteCommand
            {
                CommandText =
                    "DELETE FROM FinancialTransactions WHERE [Date] = @date AND [Payee] = @payee AND [MajorCategory] = @majorCategory AND [MinorCategory] = @minorCategory AND [Memo] = @memo AND [Outflow] = @outflow AND [Inflow] = @inflow AND [Account] = @account"
            };
            cmd.Parameters.AddWithValue("@date", transaction.DateToString);
            cmd.Parameters.AddWithValue("@payee", transaction.Payee);
            cmd.Parameters.AddWithValue("@majorCategory", transaction.MajorCategory);
            cmd.Parameters.AddWithValue("@minorCategory", transaction.MinorCategory);
            cmd.Parameters.AddWithValue("@memo", transaction.Memo);
            cmd.Parameters.AddWithValue("@outflow", transaction.Outflow);
            cmd.Parameters.AddWithValue("@inflow", transaction.Inflow);
            cmd.Parameters.AddWithValue("@account", account.Name);

            return SQLite.ExecuteCommand(AppState.CurrentUserConnection, cmd);
        }

        /// <summary>Gets the next Transaction ID autoincrement value in the database for the Transactions table.</summary>
        /// <returns>Next Transactions ID value</returns>
        public async Task<int> GetNextFinancialTransactionIndex()
        {
            DataSet ds = await SQLite.FillDataSet(AppState.CurrentUserConnection, "SELECT * FROM SQLITE_SEQUENCE WHERE name = 'Transactions'");

            return ds.Tables[0].Rows.Count > 0 ? Int32Helper.Parse(ds.Tables[0].Rows[0]["seq"]) + 1 : 1;
        }

        /// <summary>Modifies the selected Transaction in the database.</summary>
        /// <param name="newTransaction">Transaction to replace the current one in the database</param>
        /// <param name="oldTransaction">Current Transaction in the database</param>
        /// <returns>Returns true if successful</returns>
        public Task<bool> ModifyFinancialTransaction(FinancialTransaction newTransaction, FinancialTransaction oldTransaction)
        {
            SQLiteCommand cmd = new SQLiteCommand
            {
                CommandText = "UPDATE FinancialTransactions SET [Date] = @date, [Payee] = @payee, [MajorCategory] = @majorCategory, [MinorCategory] = @minorCategory, [Memo] = @memo, [Outflow] = @outflow, [Inflow] = @inflow, [Account] = @account WHERE [Date] = @oldDate AND [Payee] = @oldPayee AND [MajorCategory] = @oldMajorCategory AND [MinorCategory] = @oldMinorCategory AND [Memo] = @oldMemo AND [Outflow] = @oldOutflow AND [Inflow] = @oldInflow AND [Account] = @oldAccount"
            };
            cmd.Parameters.AddWithValue("@date", newTransaction.DateToString);
            cmd.Parameters.AddWithValue("@payee", newTransaction.Payee);
            cmd.Parameters.AddWithValue("@majorCategory", newTransaction.MajorCategory);
            cmd.Parameters.AddWithValue("@minorCategory", newTransaction.MinorCategory);
            cmd.Parameters.AddWithValue("@memo", newTransaction.Memo);
            cmd.Parameters.AddWithValue("@outflow", newTransaction.Outflow);
            cmd.Parameters.AddWithValue("@inflow", newTransaction.Inflow);
            cmd.Parameters.AddWithValue("@account", newTransaction.Account);

            cmd.Parameters.AddWithValue("@oldDate", oldTransaction.DateToString);
            cmd.Parameters.AddWithValue("@oldPayee", oldTransaction.Payee);
            cmd.Parameters.AddWithValue("@oldMajorCategory", oldTransaction.MajorCategory);
            cmd.Parameters.AddWithValue("@oldMinorCategory", oldTransaction.MinorCategory);
            cmd.Parameters.AddWithValue("@oldMemo", oldTransaction.Memo);
            cmd.Parameters.AddWithValue("@oldOutflow", oldTransaction.Outflow);
            cmd.Parameters.AddWithValue("@oldInflow", oldTransaction.Inflow);
            cmd.Parameters.AddWithValue("@oldAccount", oldTransaction.Account);

            return SQLite.ExecuteCommand(AppState.CurrentUserConnection, cmd);
        }

        #endregion Transaction Management

        #endregion Finances

        #region Lenses Management

        #region Contact Lens Manipulation

        /// <summary>Adds a new contact insertion to the database.</summary>
        /// <param name="newContact">Contact insertion to be added</param>
        public Task<bool> AddContact(Contact newContact)
        {
            SQLiteCommand cmd = new SQLiteCommand
            {
                CommandText = "INSERT INTO Contacts([Date], [Side], [ReplacementDate])" +
                              "VALUES(@date, @side, @replacementDate)"
            };
            cmd.Parameters.AddWithValue("@date", newContact.DateToString);
            cmd.Parameters.AddWithValue("@side", newContact.SideToString);
            cmd.Parameters.AddWithValue("@replacementDate", newContact.ReplacementDateToString);

            return SQLite.ExecuteCommand(AppState.CurrentUserConnection, cmd);
        }

        /// <summary>Loads all contact insertions from the database.</summary>
        /// <returns>All contact insertions</returns>
        public async Task<List<Contact>> LoadContacts()
        {
            List<Contact> allContacts = new List<Contact>();
            DataSet ds = await SQLite.FillDataSet(AppState.CurrentUserConnection, "SELECT * FROM Contacts");
            if (ds.Tables[0].Rows.Count > 0)
            {
                allContacts.AddRange(from DataRow dr in ds.Tables[0].Rows select new Contact(DateTimeHelper.Parse(dr["Date"]), EnumHelper.Parse<Side>(dr["Side"].ToString()), DateTimeHelper.Parse(dr["ReplacementDate"])));
                allContacts = allContacts.OrderByDescending(contact => contact.Date)
                    .ThenBy(contact => contact.SideToString).ToList();
            }

            return allContacts;
        }

        /// <summary>Modifies an existing contact in the database.</summary>
        /// <param name="originalContact">Contact to be modified</param>
        /// <param name="newContact">Contact with modifications</param>
        public Task<bool> ModifyContact(Contact originalContact, Contact newContact)
        {
            SQLiteCommand cmd = new SQLiteCommand
            {
                CommandText = "Update Contacts SET [Date] = @newDate, [Side] = @newSide, [ReplacementDate] = @newReplacementDate WHERE [Date] = @oldDate AND [Side] = @oldSide AND [ReplacementDate] = @oldReplacementDate"
            };
            cmd.Parameters.AddWithValue("@newDate", newContact.DateToString);
            cmd.Parameters.AddWithValue("@newSide", newContact.SideToString);
            cmd.Parameters.AddWithValue("@newReplacementDate", newContact.ReplacementDateToString);
            cmd.Parameters.AddWithValue("@oldDate", originalContact.DateToString);
            cmd.Parameters.AddWithValue("@oldSide", originalContact.SideToString);
            cmd.Parameters.AddWithValue("@oldReplacementDate", originalContact.ReplacementDateToString);

            return SQLite.ExecuteCommand(AppState.CurrentUserConnection, cmd);
        }

        /// <summary>Removes a contact from the database.</summary>
        /// <param name="removeContact">Contact to be removed</param>
        public Task<bool> RemoveContact(Contact removeContact)
        {
            SQLiteCommand cmd = new SQLiteCommand
            {
                CommandText = "REMOVE FROM Contacts WHERE [Date] = @date AND [Side] = @side AND [ReplacementDate] = @replacementDate"
            };
            cmd.Parameters.AddWithValue("@date", removeContact.DateToString);
            cmd.Parameters.AddWithValue("@side", removeContact.SideToString);
            cmd.Parameters.AddWithValue("@replacementDate", removeContact.ReplacementDateToString);

            return SQLite.ExecuteCommand(AppState.CurrentUserConnection, cmd);
        }

        #endregion Contact Lens Manipulation

        #endregion Lenses Management

        #region Fuel

        #region Fuel Transaction Management

        /// <summary>Deletes a Transaction from the database.</summary>
        /// <param name="deleteTransaction">Transaction to be deleted</param>
        /// <returns>Returns true if deletion successful</returns>
        public Task<bool> DeleteFuelTransaction(FuelTransaction deleteTransaction)
        {
            SQLiteCommand cmd = new SQLiteCommand { CommandText = "DELETE FROM FuelTransactions WHERE [TransactionID] = @transactionID" };
            cmd.Parameters.AddWithValue("@transactionID", deleteTransaction.TranscationID);

            return SQLite.ExecuteCommand(AppState.CurrentUserConnection, cmd);
        }

        /// <summary>Gets the next TransactionID autoincrement value in the database for the Vehicle table.</summary>
        /// <returns>Next TransactionID value</returns>
        public async Task<int> GetNextFuelTransactionIndex()
        {
            DataSet ds = await SQLite.FillDataSet(AppState.CurrentUserConnection, "SELECT * FROM SQLITE_SEQUENCE WHERE name = 'Transactions'");

            return ds.Tables[0].Rows.Count > 0 ? Int32Helper.Parse(ds.Tables[0].Rows[0]["seq"]) + 1 : 1;
        }

        /// <summary>Loads all Transactions associated with a specific Vehicle.</summary>
        /// <param name="vehicleID">Vehicle ID</param>
        /// <returns>Returns all Transactions associated with a specific Vehicle.</returns>
        public async Task<List<FuelTransaction>> LoadFuelTransactions(int vehicleID)
        {
            SQLiteCommand cmd = new SQLiteCommand { CommandText = "SELECT * FROM FuelTransactions WHERE VehicleID = @id" };
            cmd.Parameters.AddWithValue("@id", vehicleID);

            DataSet ds = await SQLite.FillDataSet(AppState.CurrentUserConnection, cmd);

            List<FuelTransaction> transactions = new List<FuelTransaction>();

            if (ds.Tables[0].Rows.Count > 0)
            {
                transactions.AddRange(from DataRow dr in ds.Tables[0].Rows select new FuelTransaction(Int32Helper.Parse(dr["TransactionID"]), Int32Helper.Parse(dr["VehicleID"]), DateTimeHelper.Parse(dr["Date"]), dr["Store"].ToString(), Int32Helper.Parse(dr["Octane"]), DecimalHelper.Parse(dr["Distance"]), DecimalHelper.Parse(dr["Gallons"]), DecimalHelper.Parse(dr["Price"]), DecimalHelper.Parse(dr["Odometer"]), Int32Helper.Parse(dr["Range"])));
                transactions = transactions.OrderByDescending(transaction => transaction.Date).ToList();
            }

            return transactions;
        }

        /// <summary>Modifies an existing Transaction.</summary>
        /// <param name="oldTransaction">Existing Transaction</param>
        /// <param name="newTransaction">New Transaction</param>
        /// <returns>Returns true if modification successful</returns>
        public Task<bool> ModifyFuelTransaction(FuelTransaction oldTransaction, FuelTransaction newTransaction)
        {
            SQLiteCommand cmd = new SQLiteCommand
            {
                CommandText = "UPDATE FuelTransactions SET [VehicleID] = @vehicleID, [Store] = @store, [Date] = @date, [Octane] = @octane, [Distance] = @distance, [Gallons] = @gallons, [Price] = @price, [Odometer] = @odometer, [Range] = @range WHERE [TransactionID] = @transactionID"
            };
            cmd.Parameters.AddWithValue("@vehicleID", newTransaction.VehicleID);
            cmd.Parameters.AddWithValue("@store", newTransaction.Store.Replace("'", "''"));
            cmd.Parameters.AddWithValue("@date", newTransaction.DateToString);
            cmd.Parameters.AddWithValue("@octane", newTransaction.Octane);
            cmd.Parameters.AddWithValue("@distance", newTransaction.Distance);
            cmd.Parameters.AddWithValue("@gallons", newTransaction.Gallons);
            cmd.Parameters.AddWithValue("@price", newTransaction.Price);
            cmd.Parameters.AddWithValue("@odometer", newTransaction.Odometer);
            cmd.Parameters.AddWithValue("@range", newTransaction.Range);
            cmd.Parameters.AddWithValue("@transactionID", newTransaction.TranscationID);

            return SQLite.ExecuteCommand(AppState.CurrentUserConnection, cmd);
        }

        /// <summary>Adds a new Transaction to the database.</summary>
        /// <param name="newTransaction">Transaction to be added</param>
        /// <returns>Returns true if add successful</returns>
        public Task<bool> NewFuelTransaction(FuelTransaction newTransaction)
        {
            SQLiteCommand cmd = new SQLiteCommand { CommandText = "INSERT INTO FuelTransactions([VehicleID], [Store], [Date], [Octane], [Distance], [Gallons], [Price], [Odometer], [Range])VALUES(@id, @store, @date, @octane, @distance, @gallons, @price, @odometer, @range)" };
            cmd.Parameters.AddWithValue("@id", newTransaction.VehicleID);
            cmd.Parameters.AddWithValue("@store", newTransaction.Store);
            cmd.Parameters.AddWithValue("@date", newTransaction.DateToString);
            cmd.Parameters.AddWithValue("@octane", newTransaction.Octane);
            cmd.Parameters.AddWithValue("@distance", newTransaction.Distance);
            cmd.Parameters.AddWithValue("@gallons", newTransaction.Gallons);
            cmd.Parameters.AddWithValue("@price", newTransaction.Price);
            cmd.Parameters.AddWithValue("@odometer", newTransaction.Odometer);
            cmd.Parameters.AddWithValue("@range", newTransaction.Range);
            return SQLite.ExecuteCommand(AppState.CurrentUserConnection, cmd);
        }

        #endregion Fuel Transaction Management

        #region Vehicle Management

        /// <summary>Changes details in the database regarding a Vehicle.</summary>
        /// <param name="oldVehicle">Old Vehicle details</param>
        /// <param name="newVehicle">New Vehicle details</param>
        /// <returns>Returns true if modification successful</returns>
        public Task<bool> ModifyVehicle(Vehicle oldVehicle, Vehicle newVehicle)
        {
            SQLiteCommand cmd = new SQLiteCommand { CommandText = "UPDATE Vehicles SET [Nickname] = @name, [Make] = @make, [Model] = @model, [Year] = @year WHERE [VehicleID] = @vehicleID" };
            cmd.Parameters.AddWithValue("@name", newVehicle.Nickname);
            cmd.Parameters.AddWithValue("@make", newVehicle.Make);
            cmd.Parameters.AddWithValue("@model", newVehicle.Model);
            cmd.Parameters.AddWithValue("@year", newVehicle.Year);
            cmd.Parameters.AddWithValue("@vehicleID", oldVehicle.VehicleID);
            return SQLite.ExecuteCommand(AppState.CurrentUserConnection, cmd);
        }

        /// <summary>Deletes a Vehicle and all associated Transactions from the database.</summary>
        /// <param name="deleteVehicle">Vehicle to be deleted</param>
        /// <returns>Returns true if deletion is successful.</returns>
        public Task<bool> DeleteVehicle(Vehicle deleteVehicle)
        {
            SQLiteCommand cmd = new SQLiteCommand { CommandText = "DELETE FROM FuelTransactions WHERE [VehicleID] = @vehicleID, DELETE FROM Vehicles WHERE [VehicleID] = @vehicleID" };
            cmd.Parameters.AddWithValue("@vehicleID", deleteVehicle.VehicleID);

            return SQLite.ExecuteCommand(AppState.CurrentUserConnection, cmd);
        }

        /// <summary>Gets the next VehicleID autoincrement value in the database for the Vehicle table.</summary>
        /// <returns>Next VehicleID value</returns>
        public async Task<int> GetNextVehicleIndex()
        {
            DataSet ds = await SQLite.FillDataSet(AppState.CurrentUserConnection, "SELECT * FROM SQLITE_SEQUENCE WHERE name = 'Vehicles'");

            if (ds.Tables[0].Rows.Count > 0)
                return Int32Helper.Parse(ds.Tables[0].Rows[0]["seq"]) + 1;
            return 1;
        }

        /// <summary>Loads all Vehicles associated with a User.</summary>
        /// <returns>All Vehicles associated with a User</returns>
        public async Task<List<Vehicle>> LoadVehicles()
        {
            DataSet ds = await SQLite.FillDataSet(AppState.CurrentUserConnection, "SELECT * FROM Vehicles");
            List<Vehicle> vehicles = new List<Vehicle>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dataRow in ds.Tables[0].Rows)
                {
                    int vehicleID = Int32Helper.Parse(dataRow["VehicleID"]);
                    Vehicle newVehicle = new Vehicle(vehicleID,
                        dataRow["Nickname"].ToString(), dataRow["Make"].ToString(), dataRow["Model"].ToString(),
                        Int32Helper.Parse(dataRow["Year"]), await LoadFuelTransactions(vehicleID));
                    vehicles.Add(newVehicle);
                }
                vehicles = vehicles.OrderBy(vehicle => vehicle.Nickname).ToList();
            }
            return vehicles;
        }

        /// <summary>Adds a new Vehicle to the database.</summary>
        /// <param name="newVehicle">Vehicle to be added</param>
        /// <returns>Returns whether the Vehicle was successfully added</returns>
        public Task<bool> NewVehicle(Vehicle newVehicle)
        {
            SQLiteCommand cmd = new SQLiteCommand { CommandText = "INSERT INTO Vehicles([Nickname], [Make], [Model], [Year])VALUES(@name, @make, @model, @year)" };
            cmd.Parameters.AddWithValue("@name", newVehicle.Nickname);
            cmd.Parameters.AddWithValue("@make", newVehicle.Make);
            cmd.Parameters.AddWithValue("@model", newVehicle.Model);
            cmd.Parameters.AddWithValue("@year", newVehicle.Year);
            return SQLite.ExecuteCommand(AppState.CurrentUserConnection, cmd);
        }

        #endregion Vehicle Management

        #endregion Fuel

        #region Television

        #region Delete

        /// <summary>Deletes a Series from the database.</summary>
        /// <param name="deleteSeries">Series to be deleted</param>
        /// <returns>True if successful</returns>
        public async Task<bool> DeleteSeries(Series deleteSeries)
        {
            SQLiteCommand cmd = new SQLiteCommand { CommandText = "DELETE FROM Series WHERE [Name] = @name AND [PremiereDate] = @date" };
            cmd.Parameters.AddWithValue("@name", deleteSeries.Name);
            cmd.Parameters.AddWithValue("@date", deleteSeries.PremiereDateToString);

            return await SQLite.ExecuteCommand(AppState.CurrentUserConnection, cmd);
        }

        #endregion Delete

        #region Load

        /// <summary>Loads all Series from the database.</summary>
        /// <returns>All Series</returns>
        public async Task<List<Series>> LoadSeries()
        {
            List<Series> allSeries = new List<Series>();
            DataSet ds = await SQLite.FillDataSet(AppState.CurrentUserConnection, "SELECT * FROM Series");

            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                    allSeries.Add(new Series(dr["Name"].ToString(), DateTimeHelper.Parse(dr["PremiereDate"]), DecimalHelper.Parse(dr["Rating"]), Int32Helper.Parse(dr["Seasons"]), Int32Helper.Parse(dr["Episodes"]), (SeriesStatus)Int32Helper.Parse(dr["Status"]), dr["Channel"].ToString(), DateTimeHelper.Parse(dr["FinaleDate"]), (DayOfWeek)Int32Helper.Parse(dr["Day"]), DateTimeHelper.Parse(dr["Time"]), dr["ReturnDate"].ToString()));
            }

            return allSeries;
        }

        #endregion Load

        #region Save

        /// <summary>Modifies a Series in the database.</summary>
        /// <param name="oldSeries">Original series</param>
        /// <param name="newSeries">Series to replace original</param>
        /// <returns>True if successful</returns>
        public async Task<bool> ModifySeries(Series oldSeries, Series newSeries)
        {
            SQLiteCommand cmd = new SQLiteCommand { CommandText = "UPDATE Series SET [Name] = @name, [PremiereDate] = @premiereDate, [Rating] = @rating, [Seasons] = @seasons, [Episodes] = @episodes, [Status] = @status, [Channel] = @channel, [FinaleDate] = @finaleDate, [Day] = @day, [Time] = @time, [ReturnDate] = @returnDate WHERE [Name] = @oldName" };
            cmd.Parameters.AddWithValue("@name", newSeries.Name);
            cmd.Parameters.AddWithValue("@premiereDate", newSeries.PremiereDateToString);
            cmd.Parameters.AddWithValue("@rating", newSeries.Rating);
            cmd.Parameters.AddWithValue("@seasons", newSeries.Seasons);
            cmd.Parameters.AddWithValue("@episodes", newSeries.Episodes);
            cmd.Parameters.AddWithValue("@status", Int32Helper.Parse(newSeries.Status));
            cmd.Parameters.AddWithValue("@channel", newSeries.Channel);
            cmd.Parameters.AddWithValue("@finaleDate", newSeries.FinaleDateToString);
            cmd.Parameters.AddWithValue("@day", Int32Helper.Parse(newSeries.Day));
            cmd.Parameters.AddWithValue("@time", newSeries.TimeToString);
            cmd.Parameters.AddWithValue("@returnDate", newSeries.ReturnDate);
            cmd.Parameters.AddWithValue("@oldName", oldSeries.Name);

            return await SQLite.ExecuteCommand(AppState.CurrentUserConnection, cmd);
        }

        /// <summary>Saves a new Series to the database.</summary>
        /// <param name="newSeries">Series to be saved</param>
        /// <returns>True if successful</returns>
        public async Task<bool> NewSeries(Series newSeries)
        {
            SQLiteCommand cmd = new SQLiteCommand { CommandText = "INSERT INTO Series([Name], [PremiereDate], [Rating], [Seasons], [Episodes], [Status], [Channel], [FinaleDate], [Day], [Time], [ReturnDate]) VALUES(@name, @premiereDate, @rating, @seasons, @episodes, @status, @channel, @finaleDate, @day, @time, @returnDate)" };
            cmd.Parameters.AddWithValue("@name", newSeries.Name);
            cmd.Parameters.AddWithValue("@premiereDate", newSeries.PremiereDateToString);
            cmd.Parameters.AddWithValue("@rating", newSeries.Rating);
            cmd.Parameters.AddWithValue("@seasons", newSeries.Seasons);
            cmd.Parameters.AddWithValue("@episodes", newSeries.Episodes);
            cmd.Parameters.AddWithValue("@status", Int32Helper.Parse(newSeries.Status));
            cmd.Parameters.AddWithValue("@channel", newSeries.Channel);
            cmd.Parameters.AddWithValue("@finaleDate", newSeries.FinaleDateToString);
            cmd.Parameters.AddWithValue("@day", Int32Helper.Parse(newSeries.Day));
            cmd.Parameters.AddWithValue("@time", newSeries.TimeToString);
            cmd.Parameters.AddWithValue("@returnDate", newSeries.ReturnDate);

            return await SQLite.ExecuteCommand(AppState.CurrentUserConnection, cmd);
        }

        #endregion Save

        #endregion Television
    }
}