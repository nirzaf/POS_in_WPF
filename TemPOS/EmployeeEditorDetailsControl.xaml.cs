using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using TemPOS.Types;
using PosControls;
using PosControls.Interfaces;
using PosModels;
using PosModels.Managers;
using PosModels.Types;
using Strings = TemPOS.Types.Strings;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for EmployeeEditorDetailsControl.xaml
    /// </summary>
    public partial class EmployeeEditorDetailsControl : UserControl
    {
        private bool _haltEvents;
        private Employee _selectedEmployee;

        [Obfuscation(Exclude = true)]
        public event EventHandler DetailsChanged;

        public string Password
        {
            get;
            private set;
        }

        public Employee SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                _haltEvents = true;
                textBoxPhone1.Text = textBoxPhone2.Text = textBoxPhone3.Text =
                    textBoxPhone4.Text = textBoxPhone5.Text = textBoxPhone6.Text = "";
                if (value != null)
                {
                    SelectedPerson = PersonManager.GetPerson(value.PersonId);
                    if (value.ScanCodeData != null)
                        Password = Encoding.UTF8.GetString(value.ScanCodeData);
                    else
                        Password = null;
                    textBoxFirstName.Text = SelectedPerson.FirstName;
                    if (SelectedPerson.MiddleInitial != null)
                        textBoxMiddleInitial.Text = "" + SelectedPerson.MiddleInitial.Value;
                    else
                        textBoxMiddleInitial.Text = null;
                    textBoxLastName.Text = SelectedPerson.LastName;
                    textBoxAddress1.Text = SelectedPerson.AddressLine1;
                    textBoxAddress2.Text = SelectedPerson.AddressLine2;
                    textBoxEMailAddress.Text = SelectedPerson.EMailAddress;
                    textBoxFederalTaxId.Text = SelectedEmployee.FederalTaxId;
                    ZipCode zipCode = ZipCode.Get(SelectedPerson.ZipCodeId);
                    if (zipCode != null)
                    {
                        ZipCodeCity city = ZipCodeCity.Get(zipCode.CityId);
                        ZipCodeState state = ZipCodeState.Get(city.StateId);

                        textBoxAddressCity.Text = city.City;
                        textBoxAddressState.Text = state.Abbreviation;
                        textBoxAddressZipCode.Text = zipCode.PostalCode.ToString("D5");
                    }
                    else
                    {
                        textBoxAddressCity.Text = "";
                        textBoxAddressState.Text = "";
                        textBoxAddressZipCode.Text = "";
                    }
                    checkBoxPermT1.IsSelected = value.HasPermission(Permissions.LateCancel);
                    checkBoxPermT2.IsSelected = value.HasPermission(Permissions.Void);
                    checkBoxPermT3.IsSelected = value.HasPermission(Permissions.RegisterDiscounts);
                    checkBoxPermT4.IsSelected = value.HasPermission(Permissions.DriverDispatch);
                    checkBoxPermT5.IsSelected = value.HasPermission(Permissions.ChangeTicketOwner);
                    checkBoxPermR1.IsSelected = value.HasPermission(Permissions.Cashout);
                    checkBoxPermR2.IsSelected = value.HasPermission(Permissions.RegisterStart);
                    checkBoxPermR3.IsSelected = value.HasPermission(Permissions.RegisterPayout);
                    checkBoxPermR4.IsSelected = value.HasPermission(Permissions.RegisterDrop);
                    checkBoxPermR5.IsSelected = value.HasPermission(Permissions.RegisterNoSale);
                    checkBoxPermR6.IsSelected = value.HasPermission(Permissions.RegisterRefund);
                    checkBoxPermR7.IsSelected = value.HasPermission(Permissions.RegisterReturn);
                    checkBoxPermR8.IsSelected = value.HasPermission(Permissions.RegisterReport);
                    checkBoxPermR9.IsSelected = value.HasPermission(Permissions.RegisterClose);
                    checkBoxPermR10.IsSelected = value.HasPermission(Permissions.RegisterDeposit);
                    checkBoxPermR11.IsSelected = value.HasPermission(Permissions.DriverBankrolling);
                    checkBoxPermR12.IsSelected = value.HasPermission(Permissions.UseAnyRegisterDrawer);
                    checkBoxPermM1.IsSelected = value.HasPermission(Permissions.ReportsMenu);
                    checkBoxPermM2.IsSelected = value.HasPermission(Permissions.SystemMaintenance);
                    checkBoxPermM3.IsSelected = value.HasPermission(Permissions.EmployeeMaintenance);
                    checkBoxPermM4.IsSelected = value.HasPermission(Permissions.EmployeeScheduleMaintenance);
                    checkBoxPermM5.IsSelected = value.HasPermission(Permissions.EmployeeTimesheetMaintenance);
                    checkBoxPermM6.IsSelected = value.HasPermission(Permissions.CustomerMaintenance);
                    checkBoxPermM7.IsSelected = value.HasPermission(Permissions.VendorMaintenance);
                    checkBoxPermM8.IsSelected = value.HasPermission(Permissions.InventoryAdjustments);
                    checkBoxPermM9.IsSelected = value.HasPermission(Permissions.OverrideDeliveryRestriction);
                    checkBoxPermM10.IsSelected = value.HasPermission(Permissions.ExitProgram);
                    checkBoxPermM11.IsSelected = value.HasPermission(Permissions.CommandShell);
                    checkBoxPermM12.IsSelected = value.HasPermission(Permissions.StartOfDay);
                    checkBoxPermM13.IsSelected = value.HasPermission(Permissions.EndOfDay);
                    checkBoxPermM14.IsSelected = value.HasPermission(Permissions.ManagerAlerts);

                    if (SelectedPerson.PhoneNumberId1 > 0)
                        textBoxPhone1.Text = PhoneNumber.Get(SelectedPerson.PhoneNumberId1).Number;
                    if (SelectedPerson.PhoneNumberId2 > 0)
                    {
                        buttonAddPhone2.Visibility = Visibility.Collapsed;
                        InsertRow(stackPanelPhone2);
                        textBoxPhone2.Text = PhoneNumber.Get(SelectedPerson.PhoneNumberId2).Number;
                    }
                    if (SelectedPerson.PhoneNumberId3 > 0)
                    {
                        buttonAddPhone3.Visibility = Visibility.Collapsed;
                        InsertRow(stackPanelPhone3);
                        textBoxPhone3.Text = PhoneNumber.Get(SelectedPerson.PhoneNumberId3).Number;
                    }
                    if (SelectedPerson.PhoneNumberId4 > 0)
                    {
                        buttonAddPhone4.Visibility = Visibility.Collapsed;
                        InsertRow(stackPanelPhone4);
                        textBoxPhone4.Text = PhoneNumber.Get(SelectedPerson.PhoneNumberId4).Number;
                    }
                    if (SelectedPerson.PhoneNumberId5 > 0)
                    {
                        buttonAddPhone5.Visibility = Visibility.Collapsed;
                        InsertRow(stackPanelPhone5);
                        textBoxPhone5.Text = PhoneNumber.Get(SelectedPerson.PhoneNumberId5).Number;
                    }
                    if (SelectedPerson.PhoneNumberId6 > 0)
                    {
                        InsertRow(stackPanelPhone6);
                        textBoxPhone6.Text = PhoneNumber.Get(SelectedPerson.PhoneNumberId6).Number;
                    }
                }
                else
                {
                    SelectedPerson = null;
                    Password = null;
                    textBoxFirstName.Text = "";
                    textBoxMiddleInitial.Text = "";
                    textBoxLastName.Text = "";
                    textBoxAddress1.Text = "";
                    textBoxAddress2.Text = "";
                    textBoxAddressCity.Text = "";
                    textBoxAddressState.Text = "";
                    textBoxAddressZipCode.Text = "";
                    textBoxEMailAddress.Text = "";
                    textBoxFederalTaxId.Text = "";
                    checkBoxPermT3.IsSelected = 
                        checkBoxPermT1.IsSelected = checkBoxPermT2.IsSelected =
                        checkBoxPermR1.IsSelected = checkBoxPermT4.IsSelected = checkBoxPermT5.IsSelected =
                        checkBoxPermR2.IsSelected = checkBoxPermR3.IsSelected = checkBoxPermR4.IsSelected = 
                        checkBoxPermR5.IsSelected = checkBoxPermR6.IsSelected = checkBoxPermR7.IsSelected = 
                        checkBoxPermR8.IsSelected = checkBoxPermR9.IsSelected = checkBoxPermR12.IsSelected =
                        checkBoxPermR10.IsSelected = checkBoxPermR11.IsSelected = checkBoxPermM1.IsSelected = 
                        checkBoxPermM2.IsSelected = checkBoxPermM3.IsSelected = checkBoxPermM4.IsSelected =
                        checkBoxPermM5.IsSelected = checkBoxPermM6.IsSelected = checkBoxPermM7.IsSelected =
                        checkBoxPermM8.IsSelected = checkBoxPermM9.IsSelected = checkBoxPermR10.IsSelected =
                        checkBoxPermM10.IsSelected = checkBoxPermM11.IsSelected = checkBoxPermM12.IsSelected =
                        checkBoxPermM13.IsSelected = checkBoxPermM14.IsSelected = false;
                }
                if (String.IsNullOrWhiteSpace(textBoxPhone6.Text) &&
                    (stackPanelPhone6.Visibility == Visibility.Visible))
                    RemoveRow(stackPanelPhone6);
                if (String.IsNullOrWhiteSpace(textBoxPhone5.Text) && 
                    (stackPanelPhone5.Visibility == Visibility.Visible))
                    RemoveRow(stackPanelPhone5);
                if (String.IsNullOrWhiteSpace(textBoxPhone4.Text) && 
                    (stackPanelPhone4.Visibility == Visibility.Visible))
                    RemoveRow(stackPanelPhone4);
                if (String.IsNullOrWhiteSpace(textBoxPhone3.Text) && 
                    (stackPanelPhone3.Visibility == Visibility.Visible))
                    RemoveRow(stackPanelPhone3);
                if (String.IsNullOrWhiteSpace(textBoxPhone2.Text) && 
                    (stackPanelPhone2.Visibility == Visibility.Visible))
                    RemoveRow(stackPanelPhone2);
                _haltEvents = false;
            }
        }

        public Person SelectedPerson
        {
            get;
            private set;
        }

        public EmployeeEditorDetailsControl()
        {
            Password = null;
            InitializeComponent();
            InsertRow(stackPanelPhone1);
        }

        [Obfuscation(Exclude = true)]
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // ToDo: Future Permissions
            stackPanelPermission15.Visibility = Visibility.Collapsed;
            stackPanelPermission17.Visibility = Visibility.Collapsed;
            stackPanelPermission18.Visibility = Visibility.Collapsed;
            stackPanelPermission20.Visibility = Visibility.Collapsed;
#if DEBUG
            stackPanelPermission24.Visibility = Visibility.Collapsed;
#endif
            // Disable certain buttons for the KeyboardEntryControl oftextBoxMiddleInitial
            textBoxMiddleInitial.ContextMenuInitialized += textBoxMiddleInitial_ContextMenuInitialized;
        }

        [Obfuscation(Exclude = true)]
        void textBoxMiddleInitial_ContextMenuInitialized(object sender, EventArgs e)
        {
            KeyboardEntryControl control = textBoxMiddleInitial.GetKeyboardControl();
            control.SpaceButton.IsEnabled = false;
            control.LeftButton.IsEnabled = false;
            control.RightButton.IsEnabled = false;
            control.CapsLockButton.IsEnabled = false;
            control.ShiftLockButton.IsEnabled = false;
            control.SoftCapsLockButton.IsEnabled = false;
        }

        #region Private methods
        private void InsertRow(StackPanel stackPanel)
        {
            RowDefinition row = new RowDefinition
            {
                Height = new GridLength(35, GridUnitType.Star)
            };
            //row.MinHeight = 40;
            gridControl.Height += 35;
            gridControl.RowDefinitions.Insert(9, row);

            stackPanel.Visibility = Visibility.Visible;
            if (labelPhoneNumber.Visibility != Visibility.Visible)
            {
                labelPhoneNumber.Visibility = Visibility.Visible;
                IncrementRow(labelPhoneNumber);
            }
            IncrementRow(textBoxEMailAddress);
            IncrementRow(textBoxFederalTaxId);
            IncrementRow(labelFederalTaxId);
            IncrementRow(labelEmailAddress);
            IncrementRow(labelManagerPermissions);
            IncrementRow(labelPassword);
            IncrementRow(labelRegisterPermissions);
            IncrementRow(labelTicketPermissions);
            if (stackPanelPhone2.Visibility == Visibility.Hidden)
                IncrementRow(stackPanelPhone1);
            if (stackPanelPhone3.Visibility == Visibility.Hidden)
                IncrementRow(stackPanelPhone2);
            if (stackPanelPhone4.Visibility == Visibility.Hidden)
                IncrementRow(stackPanelPhone3);
            if (stackPanelPhone5.Visibility == Visibility.Hidden)
                IncrementRow(stackPanelPhone4);
            if (stackPanelPhone6.Visibility == Visibility.Hidden)
                IncrementRow(stackPanelPhone5);
            IncrementRow(stackPanelPhone6);
            IncrementRow(stackPanelRegisterPermissions);
            IncrementRow(stackPanelManagerPermissions);
            IncrementRow(stackPanelTicketPermissions);
            IncrementRow(stackPanelPermission1);
            IncrementRow(stackPanelPermission2);
            IncrementRow(stackPanelPermission3);
            IncrementRow(stackPanelPermission4);
            IncrementRow(stackPanelPermission5);
            IncrementRow(stackPanelPermission6);
            IncrementRow(stackPanelPermission7);
            IncrementRow(stackPanelPermission8);
            IncrementRow(stackPanelPermission9);
            IncrementRow(stackPanelPermission10);
            IncrementRow(stackPanelPermission11);
            IncrementRow(stackPanelPermission12);
            IncrementRow(stackPanelPermission13);
            IncrementRow(stackPanelPermission14);
            IncrementRow(stackPanelPermission15);
            IncrementRow(stackPanelPermission16);
            IncrementRow(stackPanelPermission17);
            IncrementRow(stackPanelPermission18);
            IncrementRow(stackPanelPermission19);
            IncrementRow(stackPanelPermission20);
            IncrementRow(stackPanelPermission21);
            IncrementRow(stackPanelPermission22);
            IncrementRow(stackPanelPermission23);
            IncrementRow(stackPanelPermission24);
            IncrementRow(stackPanelPermission25);
            IncrementRow(stackPanelPermission26);
            IncrementRow(stackPanelPermission27);
            IncrementRow(stackPanelPermission28);
            IncrementRow(stackPanelPermission29);
            IncrementRow(stackPanelPermission30);
            IncrementRow(stackPanelPermission31);
            IncrementRow(buttonSetPassword);

            gridControl.UpdateLayout();
        }

        private void IncrementRow(UIElement control)
        {
            Grid.SetRow(control, Grid.GetRow(control) + 1);
        }

        private void DecrementRow(UIElement control)
        {
            Grid.SetRow(control, Grid.GetRow(control) - 1);
        }

        private void RemoveRow(StackPanel removedStackPanel)
        {
            StackPanel lowestStackPanel = GetLowestStackPanel();
            if (Equals(lowestStackPanel, stackPanelPhone1))
            {
                textBoxPhone1.Text = null;
                GetTextBlockButton(lowestStackPanel, Types.Strings.Add).Visibility = Visibility.Visible;
                return; // Don't remove the only row
            }
            gridControl.Height -= 35;
            gridControl.RowDefinitions.RemoveAt(GetStackPanelRowIndex(lowestStackPanel));
            UpdateText(removedStackPanel);
            lowestStackPanel.Visibility = Visibility.Hidden;

            // New last row
            lowestStackPanel = GetLowestStackPanel();
            GetTextBlockButton(lowestStackPanel, Types.Strings.Add).Visibility = Visibility.Visible;

            DecrementRow(textBoxEMailAddress);
            DecrementRow(labelEmailAddress);
            DecrementRow(textBoxFederalTaxId);
            DecrementRow(labelFederalTaxId);
            DecrementRow(labelManagerPermissions);
            DecrementRow(labelPassword);
            DecrementRow(labelRegisterPermissions);
            DecrementRow(labelTicketPermissions);

            if (stackPanelPhone2.Visibility == Visibility.Hidden)
                DecrementRow(stackPanelPhone2);
            if (stackPanelPhone3.Visibility == Visibility.Hidden)
                DecrementRow(stackPanelPhone3);
            if (stackPanelPhone4.Visibility == Visibility.Hidden)
                DecrementRow(stackPanelPhone4);
            if (stackPanelPhone5.Visibility == Visibility.Hidden)
                DecrementRow(stackPanelPhone5);
            if (stackPanelPhone6.Visibility == Visibility.Hidden)
                DecrementRow(stackPanelPhone6);

            DecrementRow(stackPanelRegisterPermissions);
            DecrementRow(stackPanelManagerPermissions);
            DecrementRow(stackPanelTicketPermissions);
            DecrementRow(stackPanelPermission1);
            DecrementRow(stackPanelPermission2);
            DecrementRow(stackPanelPermission3);
            DecrementRow(stackPanelPermission4);
            DecrementRow(stackPanelPermission5);
            DecrementRow(stackPanelPermission6);
            DecrementRow(stackPanelPermission7);
            DecrementRow(stackPanelPermission8);
            DecrementRow(stackPanelPermission9);
            DecrementRow(stackPanelPermission10);
            DecrementRow(stackPanelPermission11);
            DecrementRow(stackPanelPermission12);
            DecrementRow(stackPanelPermission13);
            DecrementRow(stackPanelPermission14);
            DecrementRow(stackPanelPermission15);
            DecrementRow(stackPanelPermission16);
            DecrementRow(stackPanelPermission17);
            DecrementRow(stackPanelPermission18);
            DecrementRow(stackPanelPermission19);
            DecrementRow(stackPanelPermission20);
            DecrementRow(stackPanelPermission21);
            DecrementRow(stackPanelPermission22);
            DecrementRow(stackPanelPermission23);
            DecrementRow(stackPanelPermission24);
            DecrementRow(stackPanelPermission25);
            DecrementRow(stackPanelPermission26);
            DecrementRow(stackPanelPermission27);
            DecrementRow(stackPanelPermission28);
            DecrementRow(stackPanelPermission29);
            DecrementRow(stackPanelPermission30);
            DecrementRow(stackPanelPermission31);
            DecrementRow(buttonSetPassword);

            gridControl.UpdateLayout();
        }

        private int GetStackPanelRowIndex(StackPanel stackPanel)
        {
            if (Equals(stackPanelPhone6, stackPanel))
                return 12;
            if (Equals(stackPanelPhone5, stackPanel))
                return 11;
            if (Equals(stackPanelPhone4, stackPanel))
                return 10;
            if (Equals(stackPanelPhone3, stackPanel))
                return 9;
            if (Equals(stackPanelPhone2, stackPanel))
                return 8;
            return 7;
        }

        private void UpdateText(StackPanel removedStackPanel)
        {

            if (Equals(removedStackPanel, stackPanelPhone1))
            {
                textBoxPhone1.Text = textBoxPhone2.Text;
                textBoxPhone2.Text = textBoxPhone3.Text;
                textBoxPhone3.Text = textBoxPhone4.Text;
                textBoxPhone4.Text = textBoxPhone5.Text;
                textBoxPhone5.Text = textBoxPhone6.Text;
            }
            if (Equals(removedStackPanel, stackPanelPhone2))
            {
                textBoxPhone2.Text = textBoxPhone3.Text;
                textBoxPhone3.Text = textBoxPhone4.Text;
                textBoxPhone4.Text = textBoxPhone5.Text;
                textBoxPhone5.Text = textBoxPhone6.Text;
            }
            if (Equals(removedStackPanel, stackPanelPhone3))
            {
                textBoxPhone3.Text = textBoxPhone4.Text;
                textBoxPhone4.Text = textBoxPhone5.Text;
                textBoxPhone5.Text = textBoxPhone6.Text;
            }
            if (Equals(removedStackPanel, stackPanelPhone4))
            {
                textBoxPhone4.Text = textBoxPhone5.Text;
                textBoxPhone5.Text = textBoxPhone6.Text;
            }
            if (Equals(removedStackPanel, stackPanelPhone5))
            {
                textBoxPhone5.Text = textBoxPhone6.Text;
            }
            textBoxPhone6.Text = "";
        }

        private CustomTextBox GetCustomTextBox(StackPanel stackPanel)
        {
            foreach (UIElement child in stackPanel.Children)
            {
                if (child is CustomTextBox)
                    return child as CustomTextBox;
            }
            return null;
        }

        private TextBlockButton GetTextBlockButton(StackPanel stackPanel, string text)
        {
            return stackPanel.Children.OfType<TextBlockButton>()
                .Select(child => child)
                .FirstOrDefault(pushButton => pushButton.Text.Equals(text));
        }

        private StackPanel GetLowestStackPanel()
        {
            if (stackPanelPhone6.Visibility == Visibility.Visible)
                return stackPanelPhone6;
            if (stackPanelPhone5.Visibility == Visibility.Visible)
                return stackPanelPhone5;
            if (stackPanelPhone4.Visibility == Visibility.Visible)
                return stackPanelPhone4;
            if (stackPanelPhone3.Visibility == Visibility.Visible)
                return stackPanelPhone3;
            if (stackPanelPhone2.Visibility == Visibility.Visible)
                return stackPanelPhone2;
            return stackPanelPhone1;
        }

        private ZipCode GetZipCode()
        {
            if (!PassNullCheck())
                return null;
            /*
            ZipCodeState state = GetZipCodeState();
            if (state == null)
                return null;
            ZipCodeCity city = GetZipCodeCity(state);
            if (city == null)
                return null;
             */
            // Will not generate an exception, PassNullCheck, verified this stringValue
            int postalCode = Convert.ToInt32(textBoxAddressZipCode.Text);
            ZipCode zipCode = ZipCode.Get(postalCode);
            //ZipCode[] allCityCodes = ZipCode.GetAll(city.Id);
            if (zipCode == null)
            {
                DialogButton button =
                    PosDialogWindow.ShowDialog(Types.Strings.TheZipCode + textBoxAddressZipCode.Text +
                    Types.Strings.WasNotFoundInTheDatabaseDoYouWantToAddItToTheDatabase,
                    Types.Strings.InvalidZipCode, DialogButtons.YesNo);
                if (button == DialogButton.Yes)
                {
                    // Add the ZipCode
                }
            }
            else
            {
                //if (!ContainsValidZipCode(allCityCodes, zipCode))
                {
                    // ToDo: Add support for this
                /*
                    DialogButton button =
                        ShowDialogMessage(Strings.TheZipCode + textBoxAddressZipCode.Text +
                        Strings.DoesntMatchTheCitystateInformationInTheDatabaseDoYouWantToUpdateTheDatabase,
                        Strings.CityZipCodeMismatch, DialogButtons.YesNo);
                    if (button == DialogButton.Yes)
                    {
                        // ToDo: Update the ZipCodeCityId for this ZipCode
                    }
                    else
                 */
                    {
                        // Update the text fields instead
                        ZipCodeCity city = ZipCodeCity.Get(zipCode.CityId);
                        textBoxAddressCity.Text = city.City;
                        textBoxAddressState.Text = ZipCodeState.Get(city.StateId).Abbreviation;
                    }
                }
            }
            return zipCode;
        }

        private bool PassNullCheck()
        {
            if (String.IsNullOrEmpty(textBoxFirstName.Text) ||
                String.IsNullOrWhiteSpace(textBoxFirstName.Text))
            {
                PosDialogWindow.ShowDialog(Types.Strings.EnterAValidValueForFirstName, Types.Strings.InvalidName, DialogButtons.Ok);
                return false;
            }
            if (String.IsNullOrEmpty(textBoxLastName.Text) ||
                String.IsNullOrWhiteSpace(textBoxLastName.Text))
            {
                PosDialogWindow.ShowDialog(Types.Strings.EnterAValidValueForLastName, Types.Strings.InvalidName, DialogButtons.Ok);
                return false;
            }
            /*
            if (String.IsNullOrEmpty(textBoxAddressCity.Text) ||
                String.IsNullOrWhiteSpace(textBoxAddressCity.Text))
            {
                ShowDialogMessage(Strings.EnterAValidValueForCity, Strings.InvalidCity, DialogButtons.Ok);
                return false;
            }
            if (String.IsNullOrEmpty(textBoxAddressState.Text) ||
                String.IsNullOrWhiteSpace(textBoxAddressState.Text))
            {
                ShowDialogMessage(Strings.EnterAValidValueForState, Strings.InvalidState, DialogButtons.Ok);
                return false;
            }
            */
            bool failedConversion = false;
            try
            {
                Convert.ToInt32(textBoxAddressZipCode.Text);
            }
            catch
            {
                failedConversion = true;
            }
            if (String.IsNullOrEmpty(textBoxAddressZipCode.Text) ||
                String.IsNullOrWhiteSpace(textBoxAddressZipCode.Text) ||
                failedConversion)
            {
                PosDialogWindow.ShowDialog(Types.Strings.EnterAValidValueForZipCode, Types.Strings.InvalidZipCode, DialogButtons.Ok);
                return false;
            }
            return true;
        }

        [Obsolete("Unused - Needs Strings Support")]
        private bool ContainsValidZipCode(ZipCode[] allCityCodes, ZipCode zipCode)
        {
            foreach (ZipCode cityZipCode in allCityCodes)
            {
                if (zipCode.PostalCode == cityZipCode.PostalCode)
                    return true;
            }
            return false;
        }

        [Obsolete("Unused - Needs Strings Support")]
        private ZipCodeCity GetZipCodeCity(ZipCodeState state)
        {
            ZipCodeCity city = ZipCodeCity.GetByName(state.Id, textBoxAddressCity.Text);
            if (city == null)
            {
                DialogButton button =
                    PosDialogWindow.ShowDialog(Types.Strings.TheCity + textBoxAddressCity.Text +
                    Types.Strings.WasNotFoundInTheDatabaseDoYouWantToAddItToTheDatabase,
                    Types.Strings.InvalidCity, DialogButtons.YesNo);
                if (button == DialogButton.Yes)
                {
                    // Add the city
                }
            }
            return city;
        }

        [Obsolete("Unused - Needs Strings Support")]
        private ZipCodeState GetZipCodeState()
        {
            ZipCodeState state = ZipCodeState.GetByName(textBoxAddressState.Text);
            if (state == null)
            {
                DialogButton button =
                    PosDialogWindow.ShowDialog(Types.Strings.TheState + textBoxAddressState.Text +
                    Types.Strings.WasNotFoundInTheDatabaseDoYouWantToAddItToTheDatabase,
                    Types.Strings.InvalidState, DialogButtons.YesNo);
                if (button == DialogButton.Yes)
                {
                    // Add the state
                }
            }
            return state;
        }

        private Permissions[] GetSelectedPermissions()
        {
            List<Permissions> permissions = new List<Permissions>();
            if (checkBoxPermT1.IsSelected)
                permissions.Add(Permissions.LateCancel);
            if (checkBoxPermT2.IsSelected)
                permissions.Add(Permissions.Void);
            if (checkBoxPermT3.IsSelected)
                permissions.Add(Permissions.RegisterDiscounts);
            if (checkBoxPermT4.IsSelected)
                permissions.Add(Permissions.DriverDispatch);
            if (checkBoxPermT5.IsSelected)
                permissions.Add(Permissions.ChangeTicketOwner);
            if (checkBoxPermR1.IsSelected)
                permissions.Add(Permissions.Cashout);
            if (checkBoxPermR2.IsSelected)
                permissions.Add(Permissions.RegisterStart);
            if (checkBoxPermR3.IsSelected)
                permissions.Add(Permissions.RegisterPayout);
            if (checkBoxPermR4.IsSelected)
                permissions.Add(Permissions.RegisterDrop);
            if (checkBoxPermR5.IsSelected)
                permissions.Add(Permissions.RegisterNoSale);
            if (checkBoxPermR6.IsSelected)
                permissions.Add(Permissions.RegisterRefund);
            if (checkBoxPermR7.IsSelected)
                permissions.Add(Permissions.RegisterReturn);
            if (checkBoxPermR8.IsSelected)
                permissions.Add(Permissions.RegisterReport);
            if (checkBoxPermR9.IsSelected)
                permissions.Add(Permissions.RegisterClose);
            if (checkBoxPermR10.IsSelected)
                permissions.Add(Permissions.RegisterDeposit);
            if (checkBoxPermR11.IsSelected)
                permissions.Add(Permissions.DriverBankrolling);
            if (checkBoxPermR12.IsSelected)
                permissions.Add(Permissions.UseAnyRegisterDrawer);
            if (checkBoxPermM1.IsSelected)
                permissions.Add(Permissions.ReportsMenu);
            if (checkBoxPermM2.IsSelected)
                permissions.Add(Permissions.SystemMaintenance);
            if (checkBoxPermM3.IsSelected)
                permissions.Add(Permissions.EmployeeMaintenance);
            if (checkBoxPermM4.IsSelected)
                permissions.Add(Permissions.EmployeeScheduleMaintenance);
            if (checkBoxPermM5.IsSelected)
                permissions.Add(Permissions.EmployeeTimesheetMaintenance);
            if (checkBoxPermM6.IsSelected)
                permissions.Add(Permissions.CustomerMaintenance);
            if (checkBoxPermM7.IsSelected)
                permissions.Add(Permissions.VendorMaintenance);
            if (checkBoxPermM8.IsSelected)
                permissions.Add(Permissions.InventoryAdjustments);
            if (checkBoxPermM9.IsSelected)
                permissions.Add(Permissions.OverrideDeliveryRestriction);
            if (checkBoxPermM10.IsSelected)
                permissions.Add(Permissions.ExitProgram);
            if (checkBoxPermM11.IsSelected)
                permissions.Add(Permissions.CommandShell);
            if (checkBoxPermM12.IsSelected)
                permissions.Add(Permissions.StartOfDay);
            if (checkBoxPermM13.IsSelected)
                permissions.Add(Permissions.EndOfDay);
            if (checkBoxPermM14.IsSelected)
                permissions.Add(Permissions.ManagerAlerts);
            return permissions.ToArray();
        }

        #endregion

        #region Public methods
        public Employee Add()
        {
            ZipCode zipCode = GetZipCode();
            if (zipCode == null)
                return null;
            int[] phoneNumbers = GetPhoneNumbers();

            // Add Employee
            Permissions[] permissions = GetSelectedPermissions();
            SelectedPerson = PersonManager.Add(
                textBoxFirstName.Text,
                (!string.IsNullOrEmpty(textBoxMiddleInitial.Text) ?
                textBoxMiddleInitial.Text[0] : (char?)null),
                textBoxLastName.Text,
                textBoxAddress1.Text,
                textBoxAddress2.Text,
                zipCode.PostalCode,
                phoneNumbers[0],
                phoneNumbers[1],
                phoneNumbers[2],
                phoneNumbers[3],
                phoneNumbers[4],
                phoneNumbers[5],
                textBoxEMailAddress.Text);
            SelectedEmployee = EmployeeManager.AddEmployee(
                SelectedPerson.Id,
                DateTime.Now,
                permissions, Password, textBoxFederalTaxId.Text);
            return SelectedEmployee;
        }

        private int[] GetPhoneNumbers()
        {
            string[] textLines = new [] {
                textBoxPhone1.Text,
                textBoxPhone2.Text,
                textBoxPhone3.Text,
                textBoxPhone4.Text,
                textBoxPhone5.Text,
                textBoxPhone6.Text };
            int[] phoneNumberIds = new [] { 0, 0, 0, 0, 0, 0 };

            for (int i = 0; i < 6; i++)
            {
                if (!String.IsNullOrEmpty(textLines[i]) &&
                    !String.IsNullOrWhiteSpace(textLines[i]))
                {
                    PhoneNumber phoneNumber =
                        PhoneNumber.Get(textLines[i]) ?? PhoneNumber.Add(textLines[i], null);
                    phoneNumberIds[i] = phoneNumber.Id;
                }
                else
                    break;
            }
            return phoneNumberIds;
        }

        public bool Update()
        {
            if ((SelectedEmployee == null) || (SelectedPerson == null))
                return false;
            int[] phoneNumbers = GetPhoneNumbers();
            ZipCode zipCode = GetZipCode();
            if (zipCode == null)
                return false;
            SelectedPerson.SetFirstName(textBoxFirstName.Text);
            SelectedPerson.SetLastName(textBoxLastName.Text);
            char? middleInitial = ((!String.IsNullOrEmpty(textBoxMiddleInitial.Text)) ?
                textBoxMiddleInitial.Text[0] : (char?)null);
            SelectedPerson.SetPhoneNumberId1(phoneNumbers[0]);
            SelectedPerson.SetPhoneNumberId2(phoneNumbers[1]);
            SelectedPerson.SetPhoneNumberId3(phoneNumbers[2]);
            SelectedPerson.SetPhoneNumberId4(phoneNumbers[3]);
            SelectedPerson.SetPhoneNumberId5(phoneNumbers[4]);
            SelectedPerson.SetPhoneNumberId6(phoneNumbers[5]);
            SelectedPerson.SetMiddleInitial(middleInitial);
            SelectedPerson.SetAddressLine1(textBoxAddress1.Text);
            SelectedPerson.SetAddressLine2(textBoxAddress2.Text);
            SelectedPerson.SetZipCodeId(zipCode.PostalCode);
            SelectedPerson.SetEMailAddress(textBoxEMailAddress.Text);
            SelectedEmployee.SetPermission(Permissions.LateCancel, checkBoxPermT1.IsSelected);
            SelectedEmployee.SetPermission(Permissions.Void, checkBoxPermT2.IsSelected);
            SelectedEmployee.SetPermission(Permissions.RegisterDiscounts, checkBoxPermT3.IsSelected);
            SelectedEmployee.SetPermission(Permissions.DriverDispatch, checkBoxPermT4.IsSelected);
            SelectedEmployee.SetPermission(Permissions.ChangeTicketOwner, checkBoxPermT5.IsSelected);
            SelectedEmployee.SetPermission(Permissions.Cashout, checkBoxPermR1.IsSelected);
            SelectedEmployee.SetPermission(Permissions.RegisterStart, checkBoxPermR2.IsSelected);
            SelectedEmployee.SetPermission(Permissions.RegisterPayout, checkBoxPermR3.IsSelected);
            SelectedEmployee.SetPermission(Permissions.RegisterDrop, checkBoxPermR4.IsSelected);
            SelectedEmployee.SetPermission(Permissions.RegisterNoSale, checkBoxPermR5.IsSelected);
            SelectedEmployee.SetPermission(Permissions.RegisterRefund, checkBoxPermR6.IsSelected);
            SelectedEmployee.SetPermission(Permissions.RegisterReturn, checkBoxPermR7.IsSelected);
            SelectedEmployee.SetPermission(Permissions.RegisterReport, checkBoxPermR8.IsSelected);
            SelectedEmployee.SetPermission(Permissions.RegisterClose, checkBoxPermR9.IsSelected);
            SelectedEmployee.SetPermission(Permissions.RegisterDeposit, checkBoxPermR10.IsSelected);
            SelectedEmployee.SetPermission(Permissions.DriverBankrolling, checkBoxPermR11.IsSelected);
            SelectedEmployee.SetPermission(Permissions.UseAnyRegisterDrawer, checkBoxPermR12.IsSelected);
            SelectedEmployee.SetPermission(Permissions.ReportsMenu, checkBoxPermM1.IsSelected);
            SelectedEmployee.SetPermission(Permissions.SystemMaintenance, checkBoxPermM2.IsSelected);
            SelectedEmployee.SetPermission(Permissions.EmployeeMaintenance, checkBoxPermM3.IsSelected);
            SelectedEmployee.SetPermission(Permissions.EmployeeScheduleMaintenance, checkBoxPermM4.IsSelected);
            SelectedEmployee.SetPermission(Permissions.EmployeeTimesheetMaintenance, checkBoxPermM5.IsSelected);
            SelectedEmployee.SetPermission(Permissions.CustomerMaintenance, checkBoxPermM6.IsSelected);
            SelectedEmployee.SetPermission(Permissions.VendorMaintenance, checkBoxPermM7.IsSelected);
            SelectedEmployee.SetPermission(Permissions.InventoryAdjustments, checkBoxPermM8.IsSelected);
            SelectedEmployee.SetPermission(Permissions.OverrideDeliveryRestriction, checkBoxPermM9.IsSelected);
            SelectedEmployee.SetPermission(Permissions.ExitProgram, checkBoxPermM10.IsSelected);
            SelectedEmployee.SetPermission(Permissions.CommandShell, checkBoxPermM11.IsSelected);
            SelectedEmployee.SetPermission(Permissions.StartOfDay, checkBoxPermM12.IsSelected);
            SelectedEmployee.SetPermission(Permissions.EndOfDay, checkBoxPermM13.IsSelected);
            SelectedEmployee.SetPermission(Permissions.ManagerAlerts, checkBoxPermM14.IsSelected);
            SelectedEmployee.SetScanCodeData(Password);
            SelectedEmployee.SetFederalTaxId(textBoxFederalTaxId.Text);
            return (SelectedPerson.Update() && SelectedEmployee.Update());
        }
        #endregion

        #region Event Handling
        private void DoDetailsChanged()
        {
            if ((DetailsChanged != null) && !_haltEvents)
                DetailsChanged.Invoke(this, new EventArgs());
        }

        [Obfuscation(Exclude = true)]
        private void buttonWriteToCard_Selected(object sender, EventArgs e)
        {
            
        }

        [Obfuscation(Exclude = true)]
        private void checkBoxPerm_SelectionChanged(object sender, EventArgs e)
        {
            DoDetailsChanged();
        }

        [Obfuscation(Exclude = true)]
        private void textBox_TextChanged(object sender, RoutedEventArgs e)
        {
            DoDetailsChanged();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxAddressZipCode_TextChanged(object sender, RoutedEventArgs e)
        {
            DoDetailsChanged();
            if (_haltEvents || string.IsNullOrEmpty(textBoxAddressZipCode.Text) || (textBoxAddressZipCode.Text.Length != 5))
                return;
            ZipCode zipCode = GetZipCode();
            if (zipCode != null)
            {
                ZipCodeCity zipCodeCity = ZipCodeCity.Get(zipCode.CityId);
                if (zipCodeCity != null)
                {
                    ZipCodeState zipCodeState = ZipCodeState.Get(zipCodeCity.StateId);
                    if (zipCodeState != null)
                    {
                        textBoxAddressCity.Text = zipCodeCity.City;
                        textBoxAddressState.Text = zipCodeState.Abbreviation;
                        if (textBoxAddressZipCode.ContextMenu != null)
                            textBoxAddressZipCode.ContextMenu.IsOpen = false;
                    }
                }
            }
        }

        [Obfuscation(Exclude = true)]
        private void textBoxMiddleInitial_TextChanged(object sender, RoutedEventArgs e)
        {
            if (textBoxMiddleInitial.Text == " ")
            {
                textBoxMiddleInitial.TextChanged -= textBoxMiddleInitial_TextChanged;
                textBoxMiddleInitial.Text = null;
                textBoxMiddleInitial.TextChanged += textBoxMiddleInitial_TextChanged;
            }
            DoDetailsChanged();
        }

        [Obfuscation(Exclude = true)]
        private void buttonSetPassword_Click(object sender, RoutedEventArgs e)
        {
            string tempPassword = PosDialogWindow.PromptKeyboard(Types.Strings.Password, "", true, ShiftMode.None);
            if (tempPassword != null)
            {
                Password = tempPassword;
                DoDetailsChanged();
            }
        }

        [Obfuscation(Exclude = true)]
        private void buttonTicketAllNone_Click(object sender, RoutedEventArgs e)
        {
            bool isSelected = Equals(sender, buttonTicketAll);
            checkBoxPermT1.IsSelected =
                checkBoxPermT2.IsSelected =
                checkBoxPermT3.IsSelected =
                checkBoxPermT4.IsSelected =
                checkBoxPermT5.IsSelected =
                isSelected;
        }

        [Obfuscation(Exclude = true)]
        private void buttonRegisterAllNone_Click(object sender, RoutedEventArgs e)
        {
            bool isSelected = Equals(sender, buttonRegisterAll);
            checkBoxPermR1.IsSelected =
                checkBoxPermR2.IsSelected =
                checkBoxPermR3.IsSelected =
                checkBoxPermR4.IsSelected =
                checkBoxPermR5.IsSelected =
                checkBoxPermR6.IsSelected =
                checkBoxPermR7.IsSelected =
                checkBoxPermR8.IsSelected =
                checkBoxPermR9.IsSelected =
                checkBoxPermR10.IsSelected =
                checkBoxPermR11.IsSelected =
                checkBoxPermR12.IsSelected =
                isSelected;
        }

        [Obfuscation(Exclude = true)]
        private void buttonManagerAllNone_Click(object sender, RoutedEventArgs e)
        {
            bool isSelected = Equals(sender, buttonManagerAll);
            checkBoxPermM1.IsSelected =
                checkBoxPermM2.IsSelected =
                checkBoxPermM3.IsSelected =
                checkBoxPermM4.IsSelected =
                checkBoxPermM5.IsSelected =
                checkBoxPermM6.IsSelected =
                checkBoxPermM7.IsSelected =
                checkBoxPermM8.IsSelected =
                checkBoxPermM9.IsSelected =
                checkBoxPermM10.IsSelected =
                checkBoxPermM11.IsSelected =
                checkBoxPermM12.IsSelected =
                checkBoxPermM13.IsSelected =
                checkBoxPermM14.IsSelected =
                    isSelected;
        }

        [Obfuscation(Exclude = true)]
        private void buttonAddPhone2_Click(object sender, RoutedEventArgs e)
        {
            TextBlockButton pushButton = sender as TextBlockButton;
            if (pushButton != null && pushButton.Text.Equals(Types.Strings.Add))
            {
                InsertRow(stackPanelPhone2);
                buttonAddPhone2.Visibility = Visibility.Collapsed;
            }
            else
            {
                RemoveRow(stackPanelPhone1);
            }
        }

        [Obfuscation(Exclude = true)]
        private void buttonAddPhone3_Click(object sender, RoutedEventArgs e)
        {
            TextBlockButton pushButton = sender as TextBlockButton;
            if (pushButton != null && pushButton.Text.Equals(Types.Strings.Add))
            {
                InsertRow(stackPanelPhone3);
                buttonAddPhone3.Visibility = Visibility.Collapsed;
            }
            else
            {
                RemoveRow(stackPanelPhone2);
            }
        }

        [Obfuscation(Exclude = true)]
        private void buttonAddPhone4_Click(object sender, RoutedEventArgs e)
        {
            TextBlockButton pushButton = sender as TextBlockButton;
            if (pushButton != null && pushButton.Text.Equals(Types.Strings.Add))
            {
                InsertRow(stackPanelPhone4);
                buttonAddPhone4.Visibility = Visibility.Collapsed;
            }
            else
            {
                RemoveRow(stackPanelPhone3);
            }
        }

        [Obfuscation(Exclude = true)]
        private void buttonAddPhone5_Click(object sender, RoutedEventArgs e)
        {
            TextBlockButton pushButton = sender as TextBlockButton;
            if (pushButton != null && pushButton.Text.Equals(Types.Strings.Add))
            {
                InsertRow(stackPanelPhone5);
                buttonAddPhone5.Visibility = Visibility.Collapsed;
            }
            else
            {
                RemoveRow(stackPanelPhone4);
            }
        }

        [Obfuscation(Exclude = true)]
        private void buttonAddPhone6_Click(object sender, RoutedEventArgs e)
        {
            TextBlockButton pushButton = sender as TextBlockButton;
            if (pushButton != null && pushButton.Text.Equals(Types.Strings.Add))
            {
                InsertRow(stackPanelPhone6);
                buttonAddPhone6.Visibility = Visibility.Collapsed;
            }
            else
            {
                RemoveRow(stackPanelPhone5);
            }
        }

        [Obfuscation(Exclude = true)]
        private void buttonRemovePhone6_Click(object sender, RoutedEventArgs e)
        {
            RemoveRow(stackPanelPhone6);
        }
        #endregion
    }
}
