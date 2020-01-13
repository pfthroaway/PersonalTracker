using Extensions;
using PersonalTracker.Finances.Models;
using PersonalTracker.Finances.Models.Categories;
using PersonalTracker.Finances.Models.Data;
using PersonalTracker.Fuel.Models;
using PersonalTracker.Lenses.Models;
using PersonalTracker.Media.Models;
using PersonalTracker.Media.Models.MediaTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PersonalTracker.Models
{
    /// <summary>Represents a User in the Personal Tracker.</summary>
    public class User : BaseINPC
    {
        #region Fields

        private int _userID;
        private string password, username;
        private AllFinances _finances = new AllFinances(new List<Account>(), new List<string>(), new List<Category>());
        private List<Contact> _lenses = new List<Contact>();
        private List<Vehicle> _vehicles = new List<Vehicle>();
        private AllMedia _media = new AllMedia(new List<Series>());

        #endregion Fields

        #region Modifying Properties

        /// <summary>The <see cref="User"/>'s ID.</summary>
        public int UserID
        {
            get => _userID;
            set
            {
                _userID = value;
                NotifyPropertyChanged(nameof(UserID), nameof(UserIDToString));
            }
        }

        /// <summary>The <see cref="User"/>'s login name.</summary>
        public string Username
        {
            get => username;
            set
            {
                username = value;
                NotifyPropertyChanged(nameof(Username));
            }
        }

        /// <summary>The <see cref="User"/>'s hashed PBKDF2 password.</summary>
        public string Password
        {
            get => password;
            set
            {
                password = value;
                NotifyPropertyChanged(nameof(Password));
            }
        }

        /// <summary>The <see cref="User"/>'s <see cref="Personal Tracker"/>.</summary>
        public AllFinances Finances
        {
            get => _finances;
            set
            {
                _finances = value;
                NotifyPropertyChanged(nameof(Finances));
            }
        }

        /// <summary>The <see cref="User"/>'s <see cref="Media"/>.</summary>
        public AllMedia Media
        {
            get => _media;
            set
            {
                _media = value;
                NotifyPropertyChanged(nameof(Media));
            }
        }

        #endregion Modifying Properties

        #region Helper Properties

        /// <summary>The <see cref="User"/>'s ID, formatted to string.</summary>
        public string UserIDToString => UserID.ToString().PadLeft(5, '0');

        /// <summary>List of all Vehicles the User owns.</summary>
        public ReadOnlyCollection<Vehicle> Vehicles => new ReadOnlyCollection<Vehicle>(_vehicles);

        /// <summary>The <see cref="User"/>'s <see cref="ContactLenses"/>.</summary>
        public ReadOnlyCollection<Contact> Lenses => new ReadOnlyCollection<Contact>(_lenses);

        #endregion Helper Properties

        #region Contact Lenses Management

        /// <summary>Adds a new contact insertion to the <see cref="User"/>'s lenses.</summary>
        /// <param name="newContact">Contact insertion to be added</param>
        public void AddContactLens(Contact newContact)
        {
            _lenses.Add(newContact);
            UpdateContactLenses();
        }

        /// <summary>Modifies an existing <see cref="Contact"/> in the <see cref="User"/>'s lenses.</summary>
        /// <param name="originalContact"><see cref="Contact"/> to be modified</param>
        /// <param name="newContact"><see cref="Contact"/> with modifications</param>
        public void ModifyContactLens(Contact originalContact, Contact newContact)
        {
            _lenses.Replace(originalContact, newContact);
            UpdateContactLenses();
        }

        /// <summary>Removes a <see cref="Contact"/> entry in the <see cref="User"/>'s lenses.</summary>
        /// <param name="removeContact"><see cref="Contact"/> insertion to be added</param>
        public void RemoveContactLens(Contact removeContact)
        {
            _lenses.Remove(removeContact);
            UpdateContactLenses();
        }

        /// <summary>Sorts and updates the <see cref="User"/>'s <see cref="Contact"/> collection.</summary>
        private void UpdateContactLenses()
        {
            _lenses = _lenses.OrderByDescending(contact => contact.Date).ThenBy(contact => contact.SideToString).ToList();
            NotifyPropertyChanged(nameof(Lenses));
        }

        /// <summary>Assigns a collection of <see cref="Contact"/> lenses to a <see cref="User"/>.</summary>
        /// <param name="lenses">Collection of <see cref="Contact"/> lenses to be assigned</param>
        public void SetLenses(IEnumerable<Contact> lenses)
        {
            List<Contact> newContacts = new List<Contact>();
            newContacts.AddRange(lenses);
            _lenses = newContacts;
        }

        #endregion Contact Lenses Management

        #region Vehicle Management

        /// <summary>Adds a Vehicle to the list of vehicles.</summary>
        /// <param name="newVehicle">Vehicle to be removed</param>
        public void AddVehicle(Vehicle newVehicle)
        {
            _vehicles.Add(newVehicle);
            _vehicles = _vehicles.OrderBy(vehicle => vehicle.Nickname).ToList();
            NotifyPropertyChanged(nameof(Vehicles));
        }

        /// <summary>Modifies a Vehicle in the list of Vehicles.</summary>
        /// <param name="oldVehicle">Original Vehicle to be replaced</param>
        /// <param name="newVehicle">New Vehicle to replace old</param>
        public void ModifyVehicle(Vehicle oldVehicle, Vehicle newVehicle)
        {
            _vehicles.Replace(oldVehicle, newVehicle);
            _vehicles = _vehicles.OrderBy(trans => trans.Nickname).ToList();
            NotifyPropertyChanged(nameof(Vehicles));
        }

        /// <summary>Removes a Vehicle from the list of vehicles.</summary>
        /// <param name="vehicle">Vehicle to be removed</param>
        public void RemoveVehicle(Vehicle vehicle)
        {
            _vehicles.Remove(vehicle);
            NotifyPropertyChanged(nameof(Vehicles));
        }

        /// <summary>Assigns a collection of <see cref="Vehicle"/>s to a <see cref="User"/>.</summary>
        /// <param name="vehicles">Collection of <see cref="Vehicle"/>s to be assigned</param>
        public void SetVehicles(IEnumerable<Vehicle> vehicles)
        {
            List<Vehicle> newVehicles = new List<Vehicle>();
            newVehicles.AddRange(vehicles);
            _vehicles = newVehicles;
        }

        #endregion Vehicle Management

        #region Override Operators

        private static bool Equals(User left, User right)
        {
            if (left is null && right is null) return true;
            if (left is null ^ right is null) return false;
            return left.UserID == right.UserID && string.Equals(left.Username, right.Username, StringComparison.OrdinalIgnoreCase) && string.Equals(left.Password, right.Password) && left.Finances == right.Finances && left.Vehicles == right.Vehicles && left.Media == right.Media && left.Lenses == right.Lenses;
        }

        public sealed override bool Equals(object obj) => Equals(this, obj as User);

        public bool Equals(User other) => Equals(this, other);

        public static bool operator ==(User left, User right) => Equals(left, right);

        public static bool operator !=(User left, User right) => !Equals(left, right);

        public sealed override int GetHashCode() => base.GetHashCode() ^ 17;

        public sealed override string ToString() => $"{UserID} : {Username}";

        #endregion Override Operators

        #region Constructors

        /// <summary>Constructs an instance of <see cref="User"/> by assigning values to some Properties.</summary>
        /// <param name="username">The <see cref="User"/>'s login name.</param>
        /// <param name="password">The <see cref="User"/>'s hashed PBKDF2 password.</param>
        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }

        /// <summary>Constructs an instance of <see cref="User"/> by assigning values to some Properties.</summary>
        /// <param name="userID">The <see cref="User"/>'s ID.</param>
        /// <param name="username">The <see cref="User"/>'s login name.</param>
        /// <param name="password">The <see cref="User"/>'s hashed PBKDF2 password.</param>
        public User(int userID, string username, string password) : this(username, password) => UserID = userID;

        /// <summary>Constructs an instance of <see cref="User"/> by assigning values to all Properties.</summary>
        /// <param name="userID">The <see cref="User"/>'s ID.</param>
        /// <param name="username">The <see cref="User"/>'s login name.</param>
        /// <param name="password">The <see cref="User"/>'s hashed PBKDF2 password.</param>
        /// <param name="finances">The <see cref="User"/>'s <see cref="Personal Tracker"</param>
        /// <param name="vehicles">The <see cref="User"/>'s <see cref="Vehicle"/>s.</param>
        /// <param name="lenses">The <see cref="User"/>'s <see cref="Contact"/> lenses.</param>
        /// <param name="media">The <see cref="User"/>'s <see cref="Media"/>.</param>
        public User(int userID, string username, string password, AllFinances finances, IEnumerable<Vehicle> vehicles, IEnumerable<Contact> lenses, AllMedia media)
        {
            UserID = userID;
            Username = username;
            Password = password;
            Finances = finances;
            SetLenses(lenses);
            SetVehicles(vehicles);
            Media = media;
        }

        /// <summary>Constructs an instance of <see cref="User"/> by copying another <see cref="User"/>'s values.</summary>
        /// <param name="other"><see cref="User"/> to copy values from</param>
        public User(User other) : this(other.UserID, other.Username, other.Password, other.Finances, other.Vehicles, other.Lenses, other.Media)
        {
        }

        #endregion Constructors
    }
}