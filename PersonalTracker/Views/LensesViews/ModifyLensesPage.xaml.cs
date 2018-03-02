using PersonalTracker.Models;
using PersonalTracker.Models.LensesModels;
using System.Windows;
using System.Windows.Controls;

namespace PersonalTracker.Views.LensesViews
{
    /// <summary>Interaction logic for ModifyContactPage.xaml</summary>
    public partial class ModifyContactPage : Page
    {
        private Contact _originalContact, _modifiedContact;

        private void LoadContact(Contact loadContact)
        {
            _originalContact = loadContact;
            _modifiedContact = new Contact(loadContact);
            InsertionDate.SelectedDate = _originalContact.Date;
            ReplacementDate.SelectedDate = _originalContact.ReplacementDate;
            CmbSide.SelectedItem = _originalContact.SideToString;
        }

        internal ModifyContactPage(Contact selectedContact)
        {
            InitializeComponent();
            CmbSide.Items.Add("Left");
            CmbSide.Items.Add("Right");
            LoadContact(selectedContact);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) => AppState.CalculateScale(Grid);
    }
}