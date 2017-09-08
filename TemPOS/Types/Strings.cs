using System;
using System.Reflection;
using TemPOS.Helpers;
using PosControls.Types;
using System.Linq;
using PosModels.Types;

namespace TemPOS.Types
{
    [Obfuscation(Exclude = true)]
    public partial class Strings : StringsCore
    {
        #region Code
        #region Licensed Access Only / Static Initializer
        static Strings()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(Strings).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to link to this application");
            }
#endif
            InitializeEnglish();
            InitializeFrench();
            InitializeGerman();
            InitializeItalian();
            InitializeSpanish();
            InitializeDutch();
            InitializePortuguese();
        }
        #endregion

        private static string GetString(string name)
        {
            try
            {
                switch (Language)
                {
#if DEBUG
                    case Languages.Debug:
                        return "(" + name + ")";
#endif
                    case Languages.English:
                        return English[name];
                    case Languages.Spanish:
                        return Spanish[name];
                    case Languages.French:
                        return French[name];
                    case Languages.Italian:
                        return Italian[name];
                    case Languages.German:
                        return German[name];
                    case Languages.Dutch:
                        return Dutch[name];
                    case Languages.Portuguese:
                        return Portuguese[name];
                    default:
                        throw new Exception("Invalid Language");
                }
            }
            catch
            {
                try
                {
                    return English[name];
                }
                catch
                {
                    return "(" + name + ")";
                }
            }
        }
        #endregion

        #region General
        public static string Warning
        {
            get { return GetString("Warning"); }
        }

        public static string Error
        {
            get { return GetString("Error"); }
        }

        public static string Notification
        {
            get { return GetString("Notification"); }
        }

        public static string DeleteConfirmation
        {
            get { return GetString("DeleteConfirmation"); }
        }

        public static string Help
        {
            get { return GetString("Help"); }
        }

        public static string Exception
        {
            get { return GetString("Exception"); }
        }

        public static string Yes
        {
            get { return GetString("Yes"); }
        }

        public static string No
        {
            get { return GetString("No"); }
        }

        public static string All
        {
            get { return GetString("All"); }
        }

        public static string None
        {
            get { return GetString("None"); }
        }

        public static string Set
        {
            get { return GetString("Set"); }
        }

        public static string Add
        {
            get { return GetString("Add"); }
        }

        public static string Remove
        {
            get { return GetString("Remove"); }
        }

        public static string Increase
        {
            get { return GetString("Increase"); }
        }

        public static string Decrease
        {
            get { return GetString("Decrease"); }
        }

        public static string PermissionDenied
        {
            get { return GetString("PermissionDenied"); }
        }
        #endregion

        #region Common
        public static string Name
        {
            get { return GetString("Name"); }
        }

        public static string Update
        {
            get { return GetString("Update"); }
        }

        public static string Description
        {
            get { return GetString("Description"); }
        }

        public static string Amount
        {
            get { return GetString("Amount"); }
        }

        public static string IsActive
        {
            get { return GetString("IsActive"); }
        }

        public static string CancelChanges
        {
            get { return GetString("CancelChanges"); }
        }

        public static string Port
        {
            get { return GetString("Port"); }
        }

        public static string Save
        {
            get { return GetString("Save"); }
        }

        public static string Print
        {
            get { return GetString("Print"); }
        }

        public static string Ticket
        {
            get { return GetString("Ticket"); }
        }

        public static string TicketItem
        {
            get { return GetString("TicketItem"); }
        }

        public static string SelectAll
        {
            get { return GetString("SelectAll"); }
        }

        public static string Confirmation
        {
            get { return GetString("Confirmation"); }
        }

        public static string IsEnabled
        {
            get { return GetString("IsEnabled"); }
        }

        public static string Enabled
        {
            get { return GetString("Enabled"); }
        }

        public static string FirstName
        {
            get { return GetString("FirstName"); }
        }

        public static string LastName
        {
            get { return GetString("LastName"); }
        }

        public static string MiddleInitial
        {
            get { return GetString("MiddleInitial"); }
        }

        public static string AddressLine1
        {
            get { return GetString("AddressLine1"); }
        }

        public static string AddressLine2
        {
            get { return GetString("AddressLine2"); }
        }

        public static string City
        {
            get { return GetString("City"); }
        }

        public static string State
        {
            get { return GetString("State"); }
        }

        public static string PostalCode
        {
            get { return GetString("PostalCode"); }
        }

        public static string EMailAddress
        {
            get { return GetString("EMailAddress"); }
        }

        public static string Password
        {
            get { return GetString("Password"); }
        }

        public static string LateCancel
        {
            get { return GetString("LateCancel"); }
        }

        public static string Open
        {
            get { return GetString("Open"); }        
        }

        public static string Closed
        {
            get { return GetString("Closed"); }
        }
        
        public static string Employee
        {
            get { return GetString("Employee"); }
        }
        
        public static string Canceled
        {
            get { return GetString("Canceled"); }
        }

        public static string Void
        {
            get { return GetString("Void"); }
        }

        public static string PrivilegedDiscounts
        {
            get { return GetString("PrivilegedDiscounts"); }
        }

        public static string DeliveryDriverDispatching
        {
            get { return GetString("DeliveryDriverDispatching"); }
        }

        public static string ChangeOwnerEmployee
        {
            get { return GetString("ChangeOwnerEmployee"); }
        }

        public static string Cashout
        {
            get { return GetString("Cashout"); }
        }

        public static string StartUp
        {
            get { return GetString("StartUp"); }
        }

        public static string Payout
        {
            get { return GetString("Payout"); }
        }

        public static string Drop
        {
            get { return GetString("Drop"); }
        }

        public static string Deposit
        {
            get { return GetString("Deposit"); }
        }

        public static string NoSale
        {
            get { return GetString("NoSale"); }
        }

        public static string Refund
        {
            get { return GetString("Refund"); }
        }

        public static string Return
        {
            get { return GetString("Return"); }
        }

        public static string Report
        {
            get { return GetString("Report"); }
        }

        public static string CloseOut
        {
            get { return GetString("CloseOut"); }
        }

        public static string DeliveryDriverBankrolling
        {
            get { return GetString("DeliveryDriverBankrolling"); }
        }

        public static string UseAnyRegisterDrawer
        {
            get { return GetString("UseAnyRegisterDrawer"); }
        }

        public static string ReportsMenu
        {
            get { return GetString("ReportsMenu"); }
        }

        public static string StartOfDay
        {
            get { return GetString("StartOfDay"); }
        }

        public static string EndOfDay
        {
            get { return GetString("EndOfDay"); }
        }

        public static string ManagerAlerts
        {
            get { return GetString("ManagerAlerts"); }
        }

        public static string SystemSetup
        {
            get { return GetString("SystemSetup"); }
        }

        public static string EmployeeSetup
        {
            get { return GetString("EmployeeSetup"); }
        }

        public static string EmployeeScheduling
        {
            get { return GetString("EmployeeScheduling"); }
        }

        public static string EmployeeTimekeeping
        {
            get { return GetString("EmployeeTimekeeping"); }
        }

        public static string CustomerSetup
        {
            get { return GetString("CustomerSetup"); }
        }

        public static string VendorSetup
        {
            get { return GetString("VendorSetup"); }
        }

        public static string InventoryAdjustments
        {
            get { return GetString("InventoryAdjustments"); }
        }

        public static string OverrideDeliveryRestrictions
        {
            get { return GetString("OverrideDeliveryRestrictions"); }
        }

        public static string AdministrativeCommandConsole
        {
            get { return GetString("AdministrativeCommandConsole"); }
        }

        public static string ExitProgram
        {
            get { return GetString("ExitProgram"); }
        }
        #endregion

        #region Languages
        public static string LanguageLanguage
        {
            get { return GetString("LanguageLanguage"); }
        }
        public static string LanguageEnglish
        {
            get { return GetString("LanguageEnglish"); }
        }
        public static string LanguageSpanish
        {
            get { return GetString("LanguageSpanish"); }
        }
        public static string LanguageFrench
        {
            get { return GetString("LanguageFrench"); }
        }
        public static string LanguageItalian
        {
            get { return GetString("LanguageItalian"); }
        }
        public static string LanguageGerman
        {
            get { return GetString("LanguageGerman"); }
        }
        public static string LanguageDutch
        {
            get { return GetString("LanguageDutch"); }
        }
        public static string LanguagePortuguese
        {
            get { return GetString("LanguagePortuguese"); }
        }
        #endregion

        #region ChangePasswordControl
        public static string PasswordIncorrect
        {
            get { return GetString("PasswordIncorrect"); }
        }

        public static string NewPasswordsMismatch
        {
            get { return GetString("NewPasswordsMismatch"); }
        }

        public static string NewPasswordToShort
        {
            get { return GetString("NewPasswordToShort"); }
        }

        public static string ChangePasswordWindowTitle
        {
            get { return GetString("ChangePasswordWindowTitle"); }
        }

        public static string ChangePasswordOldPassword
        {
            get { return GetString("ChangePasswordOldPassword"); }
        }

        public static string ChangePasswordNewPassword1
        {
            get { return GetString("ChangePasswordNewPassword1"); }
        }

        public static string ChangePasswordNewPassword2
        {
            get { return GetString("ChangePasswordNewPassword2"); }
        }
        #endregion

        #region CommandShellControl
        public static string ShellWindowTitle
        {
            get { return GetString("ShellWindowTitle"); }
        }

        public static string ShellResetSystem
        {
            get { return GetString("ShellResetSystem"); }
        }

        public static string ShellResetSystemTitle
        {
            get { return GetString("ShellResetSystemTitle"); }
        }

        public static string ShellDeleteLog
        {
            get { return GetString("ShellDeleteLog"); }
        }

        public static string ShellNoLogFileExists
        {
            get { return GetString("ShellNoLogFileExists"); }
        }

        public static string ShellHelpClearLine1
        {
            get { return GetString("ShellHelpClearLine1"); }
        }

        public static string ShellHelpClearLine2
        {
            get { return GetString("ShellHelpClearLine2"); }
        }

        public static string ShellHelpLogLine1
        {
            get { return GetString("ShellHelpLogLine1"); }
        }

        public static string ShellHelpLogLine2
        {
            get { return GetString("ShellHelpLogLine2"); }
        }

        public static string ShellHelpSqlQueryLine1
        {
            get { return GetString("ShellHelpSqlQueryLine1"); }
        }

        public static string ShellHelpSqlQueryLine2
        {
            get { return GetString("ShellHelpSqlQueryLine2"); }
        }

        public static string ShellHelpSqlNonQueryLine1
        {
            get { return GetString("ShellHelpSqlNonQueryLine1"); }
        }

        public static string ShellHelpSqlNonQueryLine2
        {
            get { return GetString("ShellHelpSqlNonQueryLine2"); }
        }

        public static string ShellSqlNonQueryResultSuccess
        {
            get { return GetString("ShellSqlNonQueryResultSuccess"); }
        }

        public static string ShellSqlNonQueryResultFailed
        {
            get { return GetString("ShellSqlNonQueryResultFailed"); }
        }

        public static string ShellHelpStoreSettingLine1
        {
            get { return GetString("ShellHelpStoreSettingLine1"); }
        }

        public static string ShellHelpStoreSettingLine2
        {
            get { return GetString("ShellHelpStoreSettingLine2"); }
        }

        public static string ShellHelpStoreSettingLine3
        {
            get { return GetString("ShellHelpStoreSettingLine3"); }
        }

        public static string ShellStoreSettingNotSet
        {
            get { return GetString("ShellStoreSettingNotSet"); }
        }

        public static string ShellSeatingStatusOn
        {
            get { return GetString("ShellSeatingStatusOn"); }
        }

        public static string ShellSeatingStatusOff
        {
            get { return GetString("ShellSeatingStatusOff"); }
        }

        public static string ShellNoPrintersFound
        {
            get { return GetString("ShellNoPrintersFound"); }
        }

        public static string ShellKeyboardLockStatusOn
        {
            get { return GetString("ShellKeyboardLockStatusOn"); }
        }

        public static string ShellKeyboardLockStatusOff
        {
            get { return GetString("ShellKeyboardLockStatusOff"); }
        }

        public static string ShellHelpUsage
        {
            get { return GetString("ShellHelpUsage"); }
        }
        #endregion

        #region CancelMadeUnmadeControl
        public static string CancelControlReopen
        {
            get { return GetString("CancelControlReopen"); }
        }

        public static string CancelControlCancelMade
        {
            get { return GetString("CancelControlCancelMade"); }
        }

        public static string CancelControlCancelUnmade
        {
            get { return GetString("CancelControlCancelUnmade"); }
        }

        public static string CancelControlVoid
        {
            get { return GetString("CancelControlVoid"); }
        }

        public static string CancelControlDontCancel
        {
            get { return GetString("CancelControlDontCancel"); }
        }

        public static string CancelControlDontRefund
        {
            get { return GetString("CancelControlDontRefund"); }
        }
        #endregion

        #region CategoryEditorControl
        public static string CategoryEditorDisplayIndex
        {
            get { return GetString("CategoryEditorDisplayIndex"); }
        }

        public static string CategoryEditorDoNotDisplay
        {
            get { return GetString("CategoryEditorDoNotDisplay"); }
        }

        public static string CategoryEditorNameInvalid
        {
            get { return GetString("CategoryEditorNameInvalid"); }
        }

        public static string CategoryEditorDisplayIndexInvalid
        {
            get { return GetString("CategoryEditorDisplayIndexInvalid"); }
        }
        #endregion

        #region CouponCategorySelectionControl
        public static string CategoryAllCategories
        {
            get { return GetString("CategoryAllCategories"); }
        }

        public static string CategorySelectedCategories
        {
            get { return GetString("CategorySelectedCategories"); }
        }

        public static string CategoryAddCategory
        {
            get { return GetString("CategoryAddCategory"); }
        }

        public static string CategoryRemoveCategory
        {
            get { return GetString("CategoryRemoveCategory"); }
        }
        #endregion

        #region CouponEditorDetailsControl
        public static string CouponEditorAmountAsPercentage
        {
            get { return GetString("CouponEditorAmountAsPercentage"); }
        }

        public static string CouponEditorMatching
        {
            get { return GetString("CouponEditorMatching"); }
        }

        public static string CouponEditorMatchAll
        {
            get { return GetString("CouponEditorMatchAll"); }
        }

        public static string CouponEditorThirdPartyCompensation
        {
            get { return GetString("CouponEditorThirdPartyCompensation"); }
        }

        public static string CouponEditorCouponValueLimit
        {
            get { return GetString("CouponEditorCouponValueLimit"); }
        }

        public static string CouponEditorLimitPerTicket
        {
            get { return GetString("CouponEditorLimitPerTicket"); }
        }

        public static string CouponEditorCategory
        {
            get { return GetString("CouponEditorCategory"); }
        }

        public static string CouponEditorExcludeAllExceptFor
        {
            get { return GetString("CouponEditorExcludeAllExceptFor"); }
        }

        public static string CouponEditorIncludeAllExceptFor
        {
            get { return GetString("CouponEditorIncludeAllExceptFor"); }
        }

        public static string CouponEditorInvlaidLimitPerTicket
        {
            get { return GetString("CouponEditorInvlaidLimitPerTicket"); }
        }

        public static string CouponEditorInvalidAmountLimit
        {
            get { return GetString("CouponEditorInvalidAmountLimit"); }
        }

        public static string CouponEditorInvalidName
        {
            get { return GetString("CouponEditorInvalidName"); }
        }

        public static string CouponEditorInvalidAmount
        {
            get { return GetString("CouponEditorInvalidAmount"); }
        }

        public static string CouponEditorInvalidType
        {
            get { return GetString("CouponEditorInvalidType"); }
        }
        #endregion

        #region CouponItemSelectionControl
        public static string ItemAllItems
        {
            get { return GetString("ItemAllItems"); }
        }

        public static string ItemSelectedItems
        {
            get { return GetString("ItemSelectedItems"); }
        }

        public static string ItemAddItem
        {
            get { return GetString("ItemAddItem"); }
        }

        public static string ItemRemoveItem
        {
            get { return GetString("ItemRemoveItem"); }
        }
        #endregion

        #region CouponMaintenanceControl
        public static string CouponProperties
        {
            get { return GetString("CouponProperties"); }
        }
        public static string AreYouSureYouWantToDeleteTheSelectedCoupon
        {
            get { return GetString("AreYouSureYouWantToDeleteTheSelectedCoupon"); }
        }
        public static string CouponSetupNewCoupon
        {
            get { return GetString("CouponSetupNewCoupon"); }
        }

        public static string CouponSetupDeleteCoupon
        {
            get { return GetString("CouponSetupDeleteCoupon"); }
        }

        public static string CouponSetupUpdateCoupon
        {
            get { return GetString("CouponSetupUpdateCoupon"); }
        }
        #endregion

        #region DayOfOperationRangeSelectionControl
        public static string SelectedRange
        {
            get { return GetString("SelectedRange"); }
        }
        public static string DayOfOperationStartingDay
        {
            get { return GetString("DayOfOperationStartingDay"); }
        }

        public static string DayOfOperationEndingDay
        {
            get { return GetString("DayOfOperationEndingDay"); }
        }

        public static string DayOfOperationSelectSpecifiedDays
        {
            get { return GetString("DayOfOperationSelectSpecifiedDays"); }
        }

        public static string DayOfOperationSelectThisYear
        {
            get { return GetString("DayOfOperationSelectThisYear"); }
        }
        #endregion

        #region DeviceSelectionControl
        public static string DeviceSelectionTab1
        {
            get { return GetString("DeviceSelectionTab1"); }
        }

        public static string DeviceSelectionTab2
        {
            get { return GetString("DeviceSelectionTab2"); }
        }

        public static string DeviceSelectionTab3
        {
            get { return GetString("DeviceSelectionTab3"); }
        }

        public static string DeviceSelectionTab4
        {
            get { return GetString("DeviceSelectionTab4"); }
        }

        public static string DeviceSelectionTab5
        {
            get { return GetString("DeviceSelectionTab5"); }
        }
        #endregion

        #region DiscountEditorControl
        public static string DiscountEditorAmountAsPercentage
        {
            get { return GetString("DiscountEditorAmountAsPercentage"); }
        }

        public static string DiscountEditorApplyTo
        {
            get { return GetString("DiscountEditorApplyTo"); }
        }

        public static string DiscountEditorRequiresPermission
        {
            get { return GetString("DiscountEditorRequiresPermission"); }
        }

        public static string DiscountEditorPromptForAmount
        {
            get { return GetString("DiscountEditorPromptForAmount"); }
        }
        #endregion

        #region DiscountMaintenanceControl
        public static string Discounts
        {
            get { return GetString("Discounts"); }
        }
        public static string DiscountProperties
        {
            get { return GetString("DiscountProperties"); }
        }
        public static string AreYouSureYouWantToDeleteTheSelectedDiscount
        {
            get { return GetString("AreYouSureYouWantToDeleteTheSelectedDiscount"); }
        }
        public static string DiscountEditorAddDiscount
        {
            get { return GetString("DiscountEditorAddDiscount"); }
        }

        public static string DiscountEditorDeleteDiscount
        {
            get { return GetString("DiscountEditorDeleteDiscount"); }
        }

        public static string DiscountEditorUpdateDiscount
        {
            get { return GetString("DiscountEditorUpdateDiscount"); }
        }
        #endregion

        #region EmployeeClickInControl
        public static string ClockIn
        {
            get { return GetString("ClockIn"); }
        }

        public static string ClockInJobs
        {
            get { return GetString("ClockInJobs"); }
        }

        public static string ClockOut
        {
            get { return GetString("ClockOut"); }
        }

        public static string ClockOutDeclareTips
        {
            get { return GetString("ClockOutDeclareTips"); }
        }

        public static string ClockOutTips
        {
            get { return GetString("ClockOutTips"); }
        }
        #endregion

        #region EmployeeEditorControl
        public static string Employees
        {
            get { return GetString("Employees"); }
        }
        public static string EmployeeProperties
        {
            get { return GetString("EmployeeProperties"); }
        }
        public static string EmployeeEditorAddEmployee
        {
            get { return GetString("EmployeeEditorAddEmployee"); }
        }

        public static string EmployeeEditorEditJobs
        {
            get { return GetString("EmployeeEditorEditJobs"); }
        }

        public static string EmployeeEditorTerminateEmployee
        {
            get { return GetString("EmployeeEditorTerminateEmployee"); }
        }

        public static string EmployeeEditorRemoveEmployee
        {
            get { return GetString("EmployeeEditorRemoveEmployee"); }    
        }

        public static string EmployeeEditorUpdateEmployee
        {
            get { return GetString("EmployeeEditorUpdateEmployee"); }
        }

        public static string EmployeeEditorRehireEmployee
        {
            get { return GetString("EmployeeEditorRehireEmployee"); }
        }

        public static string EmployeeEditorConfirmRehire
        {
            get { return GetString("EmployeeEditorConfirmRehire"); }
        }

        public static string EmployeeEditorCantTerminateSelf
        {
            get { return GetString("EmployeeEditorCantTerminateSelf"); }
        }

        public static string EmployeeEditorConfirmTerminate
        {
            get { return GetString("EmployeeEditorConfirmTerminate"); }
        }

        public static string EmployeeEditorTerminateFirst
        {
            get { return GetString("EmployeeEditorConfirmTerminate"); }
        }

        public static string EmployeeEditorConfirmRemove
        {
            get { return GetString("EmployeeEditorConfirmRemove"); }
        }

        public static string EmployeeEditorPasswordWarning
        {
            get { return GetString("EmployeeEditorPasswordWarning"); }
        }
        #endregion

        #region EmployeeEditorDetailsControl
        public static string TheZipCode
        {
            get { return GetString("TheZipCode"); }
        }
        public static string WasNotFoundInTheDatabaseDoYouWantToAddItToTheDatabase
        {
            get { return GetString("WasNotFoundInTheDatabaseDoYouWantToAddItToTheDatabase"); }
        }
        public static string InvalidZipCode
        {
            get { return GetString("InvalidZipCode"); }
        }
        public static string DoesntMatchTheCitystateInformationInTheDatabaseDoYouWantToUpdateTheDatabase
        {
            get { return GetString("DoesntMatchTheCitystateInformationInTheDatabaseDoYouWantToUpdateTheDatabase"); }
        }
        public static string CityZipCodeMismatch
        {
            get { return GetString("CityZipCodeMismatch"); }
        }
        public static string EnterAValidValueForFirstName
        {
            get { return GetString("EnterAValidValueForFirstName"); }
        }
        public static string InvalidName
        {
            get { return GetString("InvalidName"); }
        }
        public static string EnterAValidValueForLastName
        {
            get { return GetString("EnterAValidValueForLastName"); }
        }
        public static string EnterAValidValueForCity
        {
            get { return GetString("EnterAValidValueForCity"); }
        }
        public static string InvalidCity
        {
            get { return GetString("InvalidCity"); }
        }
        public static string EnterAValidValueForState
        {
            get { return GetString("EnterAValidValueForState"); }
        }
        public static string InvalidState
        {
            get { return GetString("InvalidState"); }
        }
        public static string EnterAValidValueForZipCode
        {
            get { return GetString("EnterAValidValueForZipCode"); }
        }
        public static string TheCity
        {
            get { return GetString("TheCity"); }
        }
        public static string TheState
        {
            get { return GetString("TheState"); }
        }
        public static string EmployeeEditorTaxId
        {
            get { return GetString("EmployeeEditorTaxId"); }
        }

        public static string EmployeeEditorTicketPermissions
        {
            get { return GetString("EmployeeEditorTicketPermissions"); }
        }

        public static string EmployeeEditorRegisterPermissions
        {
            get { return GetString("EmployeeEditorRegisterPermissions"); }
        }

        public static string EmployeeEditorManagerPermissions
        {
            get { return GetString("EmployeeEditorManagerPermissions"); }
        }

        public static string EmployeeEditorPhoneNumbers
        {
            get { return GetString("EmployeeEditorPhoneNumbers"); }
        }
        #endregion

        #region EmployeeJobEditorControl
        public static string EmployeeEditorCanDeclareTips
        {
            get { return GetString("EmployeeEditorCanDeclareTips"); }
        }

        public static string EmployeeEditorCanTakeDeliveries
        {
            get { return GetString("EmployeeEditorCanTakeDeliveries"); }
        }
        #endregion

        #region EmployeeJobMaintenanceControl
        public static string EmployeeJobs
        {
            get { return GetString("EmployeeJobs"); }
        }
        public static string EmployeeJobProperties
        {
            get { return GetString("EmployeeJobProperties"); }
        }
        public static string EmployeeJobEditorNewJob
        {
            get { return GetString("EmployeeJobEditorNewJob"); }
        }

        public static string EmployeeJobEditorUpdateJob
        {
            get { return GetString("EmployeeJobEditorUpdateJob"); }
        }
        #endregion

        #region EmployeeJobSelectionControl
        public static string AvaliableJobs
        {
            get { return GetString("AvaliableJobs"); }
        }
        public static string SelectedJobs
        {
            get { return GetString("SelectedJobs"); }
        }
        public static string EmployeeJobEditorAddJob
        {
            get { return GetString("EmployeeJobEditorAddJob"); }
        }

        public static string EmployeeJobEditorEditPayRate
        {
            get { return GetString("EmployeeJobEditorEditPayRate"); }
        }

        public static string EmployeeJobEditorRemoveJob
        {
            get { return GetString("EmployeeJobEditorRemoveJob"); }
        }

        public static string EmployeeJobEditorPayRate
        {
            get { return GetString("EmployeeJobEditorPayRate"); }
        }

        public static string EmployeeJobEditorSelectJobsFor
        {
            get { return GetString("EmployeeJobEditorSelectJobsFor"); }
        }
        #endregion

        #region ExitControl
        public static string ExitLockWorkstation
        {
            get { return GetString("ExitLockWorkstation"); }
        }

        public static string ExitLogoffWindows
        {
            get { return GetString("ExitLogoffWindows"); }
        }

        public static string ExitShutdownWindows
        {
            get { return GetString("ExitShutdownWindows"); }
        }

        public static string ExitRestartWindows
        {
            get { return GetString("ExitRestartWindows"); }
        }

        public static string ExitHibernate
        {
            get { return GetString("ExitHibernate"); }
        }

        public static string ExitSuspend
        {
            get { return GetString("ExitSuspend"); }
        }

        public static string ExitRestartProgram
        {
            get { return GetString("ExitRestartProgram"); }
        }

        public static string ExitExitProgramAndSql
        {
            get { return GetString("ExitExitProgramAndSql"); }
        }

        public static string ExitExitProgram
        {
            get { return GetString("ExitExitProgram"); }
        }

        public static string ExitStoppingSql
        {
            get { return GetString("ExitStoppingSql"); }
        }

        public static string ExitExitTemPos
        {
            get { return GetString("ExitExitTemPos"); }
        }


        #endregion

        #region FutureTimeEditControl
        public static string FutureTimeSetTime
        {
            get { return GetString("FutureTimeSetTime"); }
        }

        public static string FutureTimeMakeNow
        {
            get { return GetString("FutureTimeMakeNow"); }
        }

        public static string FutureTimeTooEarly
        {
            get { return GetString("FutureTimeTooEarly"); }
        }

        public static string FutureTimeTooEarlyError
        {
            get { return GetString("FutureTimeTooEarlyError"); }
        }

        public static string FutureTime
        {
            get { return GetString("FutureTime"); }
        }
        #endregion

        #region GeneralSettingsBrushSetupControl
        public static string BrushesApplication
        {
            get { return GetString("BrushesApplication"); }
        }

        public static string BrushesBordersEnabled
        {
            get { return GetString("BrushesBordersEnabled"); }
        }

        public static string BrushesBordersDisabled
        {
            get { return GetString("BrushesBordersDisabled"); }
        }

        public static string BrushesButtonEnabled
        {
            get { return GetString("BrushesButtonEnabled"); }
        }

        public static string BrushesButtonDisabled
        {
            get { return GetString("BrushesButtonDisabled"); }
        }

        public static string BrushesButtonEnabledSelected
        {
            get { return GetString("BrushesButtonEnabledSelected"); }
        }

        public static string BrushesButtonDisabledSelected
        {
            get { return GetString("BrushesButtonDisabledSelected"); }
        }

        public static string BrushesCaret
        {
            get { return GetString("BrushesCaret"); }
        }

        public static string BrushesCheckBoxEnabled
        {
            get { return GetString("BrushesCheckBoxEnabled"); }
        }

        public static string BrushesCheckBoxDisabled
        {
            get { return GetString("BrushesCheckBoxDisabled"); }
        }

        public static string BrushesCheckBoxEnabledSelected
        {
            get { return GetString("BrushesCheckBoxEnabledSelected"); }
        }

        public static string BrushesCheckBoxDisabledSelected
        {
            get { return GetString("BrushesCheckBoxDisabledSelected"); }
        }

        public static string BrushesComboBoxEnabled
        {
            get { return GetString("BrushesComboBoxEnabled"); }
        }

        public static string BrushesComboBoxDisabled
        {
            get { return GetString("BrushesComboBoxDisabled"); }
        }

        public static string BrushesLabelEnabled
        {
            get { return GetString("BrushesLabelEnabled"); }
        }

        public static string BrushesLabelDisabled
        {
            get { return GetString("BrushesLabelDisabled"); }
        }

        public static string BrushesListItemEnabled
        {
            get { return GetString("BrushesListItemEnabled"); }
        }

        public static string BrushesListItemDisabled
        {
            get { return GetString("BrushesListItemDisabled"); }
        }

        public static string BrushesListItemEnabledSelected
        {
            get { return GetString("BrushesListItemEnabledSelected"); }
        }

        public static string BrushesListItemDisabledSelected
        {
            get { return GetString("BrushesListItemDisabledSelected"); }
        }

        public static string BrushesRadioButtonEnabled
        {
            get { return GetString("BrushesRadioButtonEnabled"); }
        }

        public static string BrushesRadioButtonDisabled
        {
            get { return GetString("BrushesRadioButtonDisabled"); }
        }

        public static string BrushesRadioButtonEnabledSelected
        {
            get { return GetString("BrushesRadioButtonEnabledSelected"); }
        }

        public static string BrushesRadioButtonDisabledSelected
        {
            get { return GetString("BrushesRadioButtonDisabledSelected"); }
        }

        public static string BrushesTabButtonEnabled
        {
            get { return GetString("BrushesTabButtonEnabled"); }
        }

        public static string BrushesTabButtonDisabled
        {
            get { return GetString("BrushesTabButtonDisabled"); }
        }

        public static string BrushesTabButtonEnabledSelected
        {
            get { return GetString("BrushesTabButtonEnabledSelected"); }
        }

        public static string BrushesTabButtonDisabledSelected
        {
            get { return GetString("BrushesTabButtonDisabledSelected"); }
        }

        public static string BrushesTextBoxEnabled
        {
            get { return GetString("BrushesTextBoxEnabled"); }
        }

        public static string BrushesTextBoxDisabled
        {
            get { return GetString("BrushesTextBoxDisabled"); }
        }

        public static string BrushesWindowTitleBar
        {
            get { return GetString("BrushesWindowTitleBar"); }
        }

        public static string BrushesForegroundColors
        {
            get { return GetString("BrushesForegroundColors"); }
        }

        public static string BrushesBackgroundColors
        {
            get { return GetString("BrushesBackgroundColors"); }
        }
        #endregion

        #region GeneralSettingsGeneralPreferencesControl
        public static string BlockTaskManagerAccess
        {
            get { return GetString("BlockTaskManagerAccess"); }
        }
        public static string Test
        {
            get { return GetString("Test"); }
        }
        public static string TestCompletedSuccessfully
        {
            get { return GetString("TestCompletedSuccessfully"); }
        }
        public static string TestPassed
        {
            get { return GetString("TestPassed"); }
        }
        public static string ServerIsNotRunning
        {
            get { return GetString("ServerIsNotRunning"); }
        }
        public static string TestFailed
        {
            get { return GetString("TestFailed"); }
        }
        public static string ClientIsNotRunning
        {
            get { return GetString("ClientIsNotRunning"); }
        }
        public static string DoYouWantToInstallTheTaskManagerAccessService
        {
            get { return GetString("DoYouWantToInstallTheTaskManagerAccessService"); }
        }
        public static string InstallService
        {
            get { return GetString("InstallService"); }
        }
        public static string DoYouWantToStartTheTaskManagerAccessService
        {
            get { return GetString("DoYouWantToStartTheTaskManagerAccessService"); }
        }
        public static string StartService
        {
            get { return GetString("StartService"); }
        }
        public static string SettingsClientMessageBroadcastServer
        {
            get { return GetString("SettingsClientMessageBroadcastServer"); }
        }

        public static string SettingsTest
        {
            get { return GetString("SettingsTest"); }
        }

        public static string SettingsGeneralOptions
        {
            get { return GetString("SettingsGeneralOptions"); }
        }

        public static string SettingsUseSeating
        {
            get { return GetString("SettingsUseSeating"); }
        }

        public static string SettingsForceWasteOnVoid
        {
            get { return GetString("SettingsForceWasteOnVoid"); }
        }

        public static string SettingsRestrictKeyboard
        {
            get { return GetString("SettingsRestrictKeyboard"); }
        }

        public static string SettingsWeatherConditions
        {
            get { return GetString("SettingsWeatherConditions"); }
        }

        public static string SettingsPostalCode
        {
            get { return GetString("SettingsPostalCode"); }
        }

        public static string SettingsAutoLogoutOptions
        {
            get { return GetString("SettingsAutoLogoutOptions"); }
        }

        public static string SettingsAutoLogoutTimeout
        {
            get { return GetString("SettingsAutoLogoutTimeout"); }
        }

        public static string SettingsAutoLogoutDisable
        {
            get { return GetString("SettingsAutoLogoutDisable"); }
        }

        public static string SettingsAutoLogoutDisableOrderEntry
        {
            get { return GetString("SettingsAutoLogoutDisableOrderEntry"); }
        }

        public static string SettingsAutoLogoutDisableDialogs
        {
            get { return GetString("SettingsAutoLogoutDisableDialogs"); }
        }

        public static string SettingsAutoLogoutDisablePasswordChange
        {
            get { return GetString("SettingsAutoLogoutDisablePasswordChange"); }
        }

        public static string SettingsLocalSettings
        {
	        get { return GetString("SettingsLocalSettings"); }
        }

        public static string SettingsKioskMode
        {
	        get { return GetString("SettingsKioskMode"); }
        }

        public static string SettingsKioskModeWarning
        {
	        get { return GetString("SettingsKioskModeWarning"); }
        }

        public static string SettingsLogoutOnPlaceOrder
        {
            get { return GetString("SettingsLogoutOnPlaceOrder"); }
        }
        #endregion

        #region GeneralSettingsUpdateControl
        public static string SettingsOptions
        {
            get { return GetString("SettingsOptions"); }
        }

        public static string SettingsAutoUpdate
        {
            get { return GetString("SettingsAutoUpdate"); }
        }

        public static string SettingsServer
        {
            get { return GetString("SettingsServer"); }
        }

        public static string SettingsVersionCheck
        {
            get { return GetString("SettingsVersionCheck"); }
        }

        public static string SettingsUpdateNow
        {
            get { return GetString("SettingsUpdateNow"); }
        }

        public static string UpdateDownloadError
        {
            get { return GetString("UpdateDownloadError"); }
        }

        public static string UpdateConnected
        {
            get { return GetString("UpdateConnected"); }
        }

        public static string UpdateFailedToConnect
        {
            get { return GetString("UpdateFailedToConnect"); }
        }

        public static string UpdateDisconnected
        {
            get { return GetString("UpdateDisconnected"); }
        }

        public static string UpdateAuthenticated
        {
            get { return GetString("UpdateAuthenticated"); }
        }

        public static string UpdateNewestVersion
        {
            get { return GetString("UpdateNewestVersion"); }
        }

        public static string UpdateReceived
        {
            get { return GetString("UpdateReceived"); }
        }

        public static string UpdateTemposUpdater
        {
            get { return GetString("UpdateTemposUpdater"); }
        }
        #endregion

        #region IngredientAmountControl
        public static string MeasurementUnit
        {
            get { return GetString("MeasurementUnit"); }
        }

        public static string UnmeasuredUnits
        {
            get { return GetString("UnmeasuredUnits"); }
        }

        public static string WeightPound
        {
            get { return GetString("WeightPound"); }
        }

        public static string WeightOunce
        {
            get { return GetString("WeightOunce"); }
        }

        public static string WeightGram
        {
            get { return GetString("WeightGram"); }
        }

        public static string WeightMilligram
        {
            get { return GetString("WeightMilligram"); }
        }

        public static string WeightKilogram
        {
            get { return GetString("WeightKilogram"); }
        }

        public static string VolumeGallon
        {
            get { return GetString("VolumeGallon"); }
        }

        public static string VolumeQuart
        {
            get { return GetString("VolumeQuart"); }
        }

        public static string VolumePint
        {
            get { return GetString("VolumePint"); }
        }

        public static string VolumeCup
        {
            get { return GetString("VolumeCup"); }
        }

        public static string VolumeTablespoon
        {
            get { return GetString("VolumeTablespoon"); }
        }

        public static string VolumeTeaspoon
        {
            get { return GetString("VolumeTeaspoon"); }
        }

        public static string VolumeLiter
        {
            get { return GetString("VolumeLiter"); }
        }

        public static string VolumeFluidOunce
        {
            get { return GetString("VolumeFluidOunce"); }
        }

        public static string VolumeMilliliter
        {
            get { return GetString("VolumeMilliliter"); }
        }

        public static string VolumeKiloliter
        {
            get { return GetString("VolumeKiloliter"); }
        }
        #endregion

        #region IngredientEditorDetailsControl
        public static string IngredientEditorIncreaseByAmount
        {
            get { return GetString("IngredientEditorIncreaseByAmount"); }
        }

        public static string IngredientEditorIncreaseByRecipe
        {
            get { return GetString("IngredientEditorIncreaseByRecipe"); }
        }

        public static string IngredientEditorDecreaseByAmount
        {
            get { return GetString("IngredientEditorDecreaseByAmount"); }
        }

        public static string IngredientEditorDecreaseByRecipe
        {
            get { return GetString("IngredientEditorDecreaseByRecipe"); }
        }

        public static string IngredientEditorNoYieldError
        {
            get { return GetString("IngredientEditorNoYieldError"); }
        }

        public static string IngredientEditorConvert1
        {
            get { return GetString("IngredientEditorConvert1"); }
        }

        public static string IngredientEditorConvert2
        {
            get { return GetString("IngredientEditorConvert2"); }
        }

        public static string IngredientEditorConvert3
        {
            get { return GetString("IngredientEditorConvert3"); }
        }

        public static string IngredientEditorUpdateInventory
        {
            get { return GetString("IngredientEditorUpdateInventory"); }
        }

        public static string IngredientEditorPrintedName
        {
            get { return GetString("IngredientEditorPrintedName"); }
        }

        public static string IngredientEditorInventoryAmount
        {
            get { return GetString("IngredientEditorInventoryAmount"); }
        }

        public static string IngredientEditorMeasuringUnit
        {
            get { return GetString("IngredientEditorMeasuringUnit"); }
        }

        public static string IngredientEditorCostPerUnit
        {
            get { return GetString("IngredientEditorCostPerUnit"); }
        }

        public static string IngredientEditorParAmount
        {
            get { return GetString("IngredientEditorParAmount"); }
        }

        public static string IngredientEditorIncrease
        {
            get { return GetString("IngredientEditorIncrease"); }
        }

        public static string IngredientEditorDecrease
        {
            get { return GetString("IngredientEditorDecrease"); }
        }
        #endregion

        #region IngredientEditorPreparationControl
        public static string PreparationCanNotBeModifiedDuringADayOfOperationCompleteAnEndOfDayToModify
        {
            get { return GetString("PreparationCanNotBeModifiedDuringADayOfOperationCompleteAnEndOfDayToModify"); }
        }
        public static string S
        {
            get { return GetString("S"); }
        }
        public static string IngredientEditorPrepared
        {
            get { return GetString("IngredientEditorPrepared"); }
        }

        public static string IngredientEditorAvailable
        {
            get { return GetString("IngredientEditorAvailable"); }
        }

        public static string IngredientEditorCurrent
        {
            get { return GetString("IngredientEditorCurrent"); }
        }

        public static string IngredientEditorRecipeYield
        {
            get { return GetString("IngredientEditorRecipeYield"); }
        }

        public static string IngredientEditorUnits
        {
            get { return GetString("IngredientEditorUnits"); }
        }

        public static string IngredientEditorAddIngredient
        {
            get { return GetString("IngredientEditorAddIngredient"); }
        }

        public static string IngredientEditorEditIngredient
        {
            get { return GetString("IngredientEditorEditIngredient"); }
        }

        public static string IngredientEditorRemoveIngredient
        {
            get { return GetString("IngredientEditorRemoveIngredient"); }
        }

        public static string IngredientEditorAmount
        {
            get { return GetString("IngredientEditorAmount"); }
        }

        public static string IngredientEditorWarningUncheck
        {
            get { return GetString("IngredientEditorWarningUncheck"); }
        }

        public static string IngredientEditorEditAmount
        {
            get { return GetString("IngredientEditorEditAmount"); }
        }

        public static string IngredientEditorConfirmRemove
        {
            get { return GetString("IngredientEditorConfirmRemove"); }
        }

        public static string IngredientEditorEditRecipeYield
        {
            get { return GetString("IngredientEditorEditRecipeYield"); }
        }
        #endregion

        #region InventoryEditorControl
        public static string InventoryIncreaseByAmount
        {
            get { return GetString("InventoryIncreaseByAmount"); }
        }

        public static string InventoryIncreaseByRecipe
        {
            get { return GetString("InventoryIncreaseByRecipe"); }
        }

        public static string InventoryDecreaseByAmount
        {
            get { return GetString("InventoryDecreaseByAmount"); }
        }

        public static string InventoryDecreaseByRecipe
        {
            get { return GetString("InventoryDecreaseByRecipe"); }
        }

        public static string InventorySetAmount
        {
            get { return GetString("InventorySetAmount"); }
        }

        public static string InventoryError
        {
            get { return GetString("InventoryError"); }
        }

        public static string InventoryEdit
        {
            get { return GetString("InventoryEdit"); }
        }
        #endregion

        #region ItemEditorControl
        public static string ItemDetails
        {
            get { return GetString("ItemDetails"); }
        }
        public static string ItemsInGroup
        {
            get { return GetString("ItemsInGroup"); }
        }
        public static string ItemIngredients
        {
            get { return GetString("ItemIngredients"); }
        }
        public static string ItemOptions
        {
            get { return GetString("ItemOptions"); }
        }
        public static string SpecialPricing
        {
            get { return GetString("SpecialPricing"); }
        }
        public static string ItemEditorErrorNoCategory
        {
            get { return GetString("ItemEditorErrorNoCategory"); }
        }

        public static string ItemEditorInvalidCategory
        {
            get { return GetString("ItemEditorInvalidCategory"); }
        }

        public static string ItemEditorErrorNoName
        {
            get { return GetString("ItemEditorErrorNoName"); }
        }

        public static string ItemEditorErrorExistingName
        {
            get { return GetString("ItemEditorErrorExistingName"); }
        }

        public static string ItemEditorInvalidName
        {
            get { return GetString("ItemEditorInvalidName"); }
        }
        #endregion

        #region ItemEditorDetailsControl
        public static string ItemEditorInvalidPrice
        {
            get { return GetString("ItemEditorInvalidPrice"); }
        }
        public static string ItemEditorInvalidTaxSetting
        {
            get { return GetString("ItemEditorInvalidTaxSetting"); }
        }
        public static string ItemEditorCategory
        {
            get { return GetString("ItemEditorCategory"); }
        }
        public static string ItemEditorFullName
        {
            get { return GetString("ItemEditorFullName"); }
        }
        public static string ItemEditorButtonName
        {
            get { return GetString("ItemEditorButtonName"); }
        }
        public static string ItemEditorPrice
        {
            get { return GetString("ItemEditorPrice"); }
        }
        public static string ItemEditorPrintDestination
        {
            get { return GetString("ItemEditorPrintDestination"); }
        }
        public static string ItemEditorTax
        {
            get { return GetString("ItemEditorTax"); }
        }
        public static string ItemEditorIsReturnable
        {
            get { return GetString("ItemEditorIsReturnable"); }
        }
        public static string ItemEditorIsFired
        {
            get { return GetString("ItemEditorIsFired"); }
        }
        public static string ItemEditorIsTaxExemptable
        {
            get { return GetString("ItemEditorIsTaxExemptable"); }
        }
        public static string ItemEditorAvailableForSale
        {
            get { return GetString("ItemEditorAvailableForSale"); }
        }
        public static string ItemEditorIsOutOfStock
        {
            get { return GetString("ItemEditorIsOutOfStock"); }
        }
        public static string ItemEditorIsGrouping
        {
            get { return GetString("ItemEditorIsGrouping"); }
        }
        public static string ItemEditorJournal
        {
            get { return GetString("ItemEditorJournal"); }
        }
        public static string ItemEditorReceipt
        {
            get { return GetString("ItemEditorReceipt"); }
        }
        public static string ItemEditorKitchen1
        {
            get { return GetString("ItemEditorKitchen1"); }
        }
        public static string ItemEditorKitchen2
        {
            get { return GetString("ItemEditorKitchen2"); }
        }
        public static string ItemEditorKitchen3
        {
            get { return GetString("ItemEditorKitchen3"); }
        }
        public static string ItemEditorBar1
        {
            get { return GetString("ItemEditorBar1"); }
        }
        public static string ItemEditorBar2
        {
            get { return GetString("ItemEditorBar2"); }
        }
        public static string ItemEditorBar3
        {
            get { return GetString("ItemEditorBar3"); }
        }
        #endregion

        #region ItemEditorGroupingControl
        public static string ItemEditorQuantity
        {
            get { return GetString("ItemEditorQuantity"); }
        }
        public static string ItemEditorErrorDemo
        {
            get { return GetString("ItemEditorErrorDemo"); }
        }
        public static string ItemEditorDemoRestriction
        {
            get { return GetString("ItemEditorDemoRestriction"); }
        }
        public static string ItemEditorEditQuantity
        {
            get { return GetString("ItemEditorEditQuantity"); }
        }
        public static string ItemEditorConfirmRemove
        {
            get { return GetString("ItemEditorConfirmRemove"); }
        }
        public static string ItemEditorAvailableItems
        {
            get { return GetString("ItemEditorAvailableItems"); }
        }
        public static string ItemEditorIncludedItems
        {
            get { return GetString("ItemEditorIncludedItems"); }
        }
        public static string ItemEditorErrorStartOfDay
        {
            get { return GetString("ItemEditorErrorStartOfDay"); }
        }
        public static string ItemEditorAddItem
        {
            get { return GetString("ItemEditorAddItem"); }
        }
        public static string ItemEditorEditQuantityButton
        {
            get { return GetString("ItemEditorEditQuantityButton"); }
        }
        public static string ItemEditorRemoveItem
        {
            get { return GetString("ItemEditorRemoveItem"); }
        }
        #endregion

        #region ItemEditorIngredientsControl
        public static string ItemIngredientsCanNotBeModifiedDuringADayOfOperationCompleteAnEndOfDayToModify
        {
            get { return GetString("ItemIngredientsCanNotBeModifiedDuringADayOfOperationCompleteAnEndOfDayToModify"); }
        }
        public static string ItemEditorAmount
        {
            get { return GetString("ItemEditorAmount"); }
        }
        public static string ItemEditorIngredientAmount
        {
            get { return GetString("ItemEditorIngredientAmount"); }
        }
        public static string ItemEditorEditIngredient
        {
            get { return GetString("ItemEditorEditIngredient"); }
        }
        public static string ItemEditorConfirmIngredientRemove
        {
            get { return GetString("ItemEditorConfirmIngredientRemove"); }
        }
        public static string ItemEditorAvailableIngredients
        {
            get { return GetString("ItemEditorAvailableIngredients"); }
        }
        public static string ItemEditorCurrentIngredients
        {
            get { return GetString("ItemEditorCurrentIngredients"); }
        }
        public static string ItemEditorAddIngredient
        {
            get { return GetString("ItemEditorAddIngredient"); }
        }
        public static string ItemEditorEditIngredientAmount
        {
            get { return GetString("ItemEditorEditIngredientAmount"); }
        }
        public static string ItemEditorRemoveIngredient
        {
            get { return GetString("ItemEditorRemoveIngredient"); }
        }
        #endregion

        #region ItemEditorOptionSetControl
        public static string ItemOptionsCanNotBeModifiedDuringADayOfOperationCompleteAnEndOfDayToModify
        {
            get { return GetString("ItemOptionsCanNotBeModifiedDuringADayOfOperationCompleteAnEndOfDayToModify"); }
        }
        public static string ItemEditorOptionSet1
        {
            get { return GetString("ItemEditorOptionSet1"); }
        }
        public static string ItemEditorOptionSet2
        {
            get { return GetString("ItemEditorOptionSet2"); }
        }
        public static string ItemEditorOptionSet3
        {
            get { return GetString("ItemEditorOptionSet3"); }
        }
        #endregion

        #region ItemEditorSpecialPricingControl
        public static string ItemEditorListboxText1
        {
            get { return GetString("ItemEditorListboxText1"); }
        }
        public static string ItemEditorListboxText2
        {
            get { return GetString("ItemEditorListboxText2"); }
        }
        public static string ItemEditorDay
        {
            get { return GetString("ItemEditorDay"); }
        }
        public static string ItemEditorStartTime
        {
            get { return GetString("ItemEditorStartTime"); }
        }
        public static string ItemEditorEndTime
        {
            get { return GetString("ItemEditorEndTime"); }
        }
        public static string ItemEditorMinDiscount
        {
            get { return GetString("ItemEditorMinDiscount"); }
        }
        public static string ItemEditorMaxDiscount
        {
            get { return GetString("ItemEditorMaxDiscount"); }
        }
        public static string ItemEditorDiscountPrice
        {
            get { return GetString("ItemEditorDiscountPrice"); }
        }
        #endregion

        #region ItemMaintenanceControl
        public static string Delete
        {
            get { return GetString("Delete"); }
        }
        public static string Items
        {
            get { return GetString("Items"); }
        }
        public static string ItemProperties
        {
            get { return GetString("ItemProperties"); }
        }
        public static string ItemOptionProperties
        {
            get { return GetString("ItemOptionProperties"); }
        }
        public static string CategoryProperties
        {
            get { return GetString("CategoryProperties"); }
        }
        public static string IngredientProperties
        {
            get { return GetString("IngredientProperties"); }
        }
        public static string Ingredients
        {
            get { return GetString("Ingredients"); }
        }
        public static string Categories
        {
            get { return GetString("Categories"); }
        }
        public static string ItemOptionSets
        {
            get { return GetString("ItemOptionSets"); }
        }
        public static string ItemSetupFind
        {
            get { return GetString("ItemSetupFind"); }
        }
        public static string ItemSetupEditItemOptions
        {
            get { return GetString("ItemSetupEditItemOptions"); }
        }
        public static string ItemSetupFindNext
        {
            get { return GetString("ItemSetupFindNext"); }
        }
        public static string ItemSetupView
        {
            get { return GetString("ItemSetupView"); }
        }
        public static string ItemSetupItems
        {
            get { return GetString("ItemSetupItems"); }
        }
        public static string ItemSetupItemsInCategory
        {
            get { return GetString("ItemSetupItemsInCategory"); }
        }
        public static string ItemSetupCategories
        {
            get { return GetString("ItemSetupCategories"); }
        }
        public static string ItemSetupIngredients
        {
            get { return GetString("ItemSetupIngredients"); }
        }
        public static string ItemSetupItemOptionSets
        {
            get { return GetString("ItemSetupItemOptionSets"); }
        }
        public static string ItemSetup
        {
            get { return GetString("ItemSetup"); }
        }
        public static string ItemSetupUpdateItem
        {
            get { return GetString("ItemSetupUpdateItem"); }
        }
        public static string ItemSetupAddItem
        {
            get { return GetString("ItemSetupAddItem"); }
        }
        public static string ItemSetupDeleteItem
        {
            get { return GetString("ItemSetupDeleteItem"); }
        }
        public static string ItemSetupAddCategory
        {
            get { return GetString("ItemSetupAddCategory"); }
        }
        public static string ItemSetupUpdateCategory
        {
            get { return GetString("ItemSetupUpdateCategory"); }
        }
        public static string ItemSetupDeleteCategory
        {
            get { return GetString("ItemSetupDeleteCategory"); }
        }
        public static string ItemSetupUpdateIngredient
        {
            get { return GetString("ItemSetupUpdateIngredient"); }
        }
        public static string ItemSetupAddIngredient
        {
            get { return GetString("ItemSetupAddIngredient"); }
        }
        public static string ItemSetupUpdateOptionSet
        {
            get { return GetString("ItemSetupUpdateOptionSet"); }
        }
        public static string ItemSetupAddOptionSet
        {
            get { return GetString("ItemSetupAddOptionSet"); }
        }
        public static string ItemSetupDeleteOptionSet
        {
            get { return GetString("ItemSetupDeleteOptionSet"); }
        }
        public static string ItemSetupNotifyNoAdditialItemsFound
        {
            get { return GetString("ItemSetupNotifyNoAdditialItemsFound"); }
        }
        public static string ItemSetupSearchItems
        {
            get { return GetString("ItemSetupSearchItems"); }
        }
        public static string ItemSetupNotifyNoItemsFound
        {
            get { return GetString("ItemSetupNotifyNoItemsFound"); }
        }
        public static string ItemSetupConfirmDeleteOptionSet
        {
            get { return GetString("ItemSetupConfirmDeleteOptionSet"); }
        }
        public static string ItemSetupConfirmDeleteCategory
        {
            get { return GetString("ItemSetupConfirmDeleteCategory"); }
        }
        public static string ItemSetupConfirmDeleteItem
        {
            get { return GetString("ItemSetupConfirmDeleteItem"); }
        }
        public static string ItemSetupValidationError
        {
            get { return GetString("ItemSetupValidationError"); }
        }
        #endregion

        #region ItemMaintenanceViewContextMenuControl
        public static string ItemSetupAllCategories
        {
            get { return GetString("ItemSetupAllCategories"); }
        }
        public static string ItemSetupAllItemsInCategory
        {
            get { return GetString("ItemSetupAllItemsInCategory"); }
        }
        public static string ItemSetupAllItems
        {
            get { return GetString("ItemSetupAllItems"); }
        }
        public static string ItemSetupAllItemOptionSets
        {
            get { return GetString("ItemSetupAllItemOptionSets"); }
        }
        public static string ItemSetupAllIngredients
        {
            get { return GetString("ItemSetupAllIngredients"); }
        }
        #endregion

        #region ItemOptionEditorControl
        public static string Ingredient
        {
            get { return GetString("Ingredient"); }
        }
        public static string ItemSetupItemRecursionError
        {
            get { return GetString("ItemSetupItemRecursionError"); }
        }
        public static string ItemSetupIngredient
        {
            get { return GetString("ItemSetupIngredient"); }
        }
        public static string ItemSetupItem
        {
            get { return GetString("ItemSetupItem"); }
        }
        public static string ItemSetupCostForExtra
        {
            get { return GetString("ItemSetupCostForExtra"); }
        }
        public static string ItemSetupUses
        {
            get { return GetString("ItemSetupUses"); }
        }
        public static string ItemSetupNothing
        {
            get { return GetString("ItemSetupNothing"); }
        }
        #endregion

        #region ItemOptionMaintenanceControl
        public static string ItemSetupConfirmDeleteItemOption
        {
            get { return GetString("ItemSetupConfirmDeleteItemOption"); }
        }
        public static string ItemSetupOptionsEdit
        {
            get { return GetString("ItemSetupOptionsEdit"); }
        }
        public static string ItemSetupNewOption
        {
            get { return GetString("ItemSetupNewOption"); }
        }
        public static string ItemSetupDeleteOption
        {
            get { return GetString("ItemSetupDeleteOption"); }
        }
        public static string ItemSetupUpdateOption
        {
            get { return GetString("ItemSetupUpdateOption"); }
        }
        #endregion

        #region ItemOptionSetEditorControl
        public static string ItemSetupErrorSetNeedsName
        {
            get { return GetString("ItemSetupErrorSetNeedsName"); }
        }
        public static string ItemSetupItemSetName
        {
            get { return GetString("ItemSetupItemSetName"); }
        }
        public static string ItemSetupMinimumRequiredOptions
        {
            get { return GetString("ItemSetupMinimumRequiredOptions"); }
        }
        public static string ItemSetupNumberOfFreeOptions
        {
            get { return GetString("ItemSetupNumberOfFreeOptions"); }
        }
        public static string ItemSetupMaxAllowedOptions
        {
            get { return GetString("ItemSetupMaxAllowedOptions"); }
        }
        public static string ItemSetupCostPerExtraOption
        {
            get { return GetString("ItemSetupCostPerExtraOption"); }
        }
        public static string ItemSetupPizzaToppingStyle
        {
            get { return GetString("ItemSetupPizzaToppingStyle"); }
        }
        #endregion

        #region LoginControl
        public static string LoginScanIdCard
        {
            get { return GetString("LoginScanIdCard"); }
        }
        public static string Login
        {
            get { return GetString("Login"); }
        }
        public static string LoginSingletonException
        {
            get { return GetString("LoginSingletonException"); }
        }
        public static string LoginAdminUserCreated
        {
            get { return GetString("LoginAdminUserCreated"); }
        }
        public static string LoginAdminInfo1
        {
            get { return GetString("LoginAdminInfo1"); }
        }
        public static string LoginAdminInfo2
        {
            get { return GetString("LoginAdminInfo2"); }
        }
        public static string LoginLoginIncorrect
        {
            get { return GetString("LoginLoginIncorrect"); }
        }
        public static string LoginWarningJobs
        {
            get { return GetString("LoginWarningJobs"); }
        }
        public static string LoginError
        {
            get { return GetString("LoginError"); }
        }
        public static string LoginEnterLogin
        {
            get { return GetString("LoginEnterLogin"); }
        }
        #endregion

        #region OrderEntryControl
        public static string NonOrderCommands
        {
            get { return GetString("NonOrderCommands"); }
        }
        public static string OrderCommands
        {
            get { return GetString("OrderCommands"); }
        }
        public static string Options
        {
            get { return GetString("Options"); }
        }
        public static string Customers
        {
            get { return GetString("Customers"); }
        }
        public static string GiftCards
        {
            get { return GetString("GiftCards"); }
        }
        public static string Register
        {
            get { return GetString("Register"); }
        }
        public static string Vendors
        {
            get { return GetString("Vendors"); }
        }
        public static string VendorItems
        {
            get { return GetString("VendorItems"); }
        }
        public static string VendorOrders
        {
            get { return GetString("VendorOrders"); }
        }
        public static string Map
        {
            get { return GetString("Map"); }
        }
        public static string HideButtons
        {
            get { return GetString("HideButtons"); }
        }
        public static string ShowButtons
        {
            get { return GetString("ShowButtons"); }
        }
        public static string ShowMyTickets
        {
            get { return GetString("ShowMyTickets"); }
        }
        public static string ShowMyOpenTickets
        {
            get { return GetString("ShowMyOpenTickets"); }
        }
        public static string ShowAllTickets
        {
            get { return GetString("ShowAllTickets"); }
        }
        public static string ShowAllOpenTickets
        {
            get { return GetString("ShowAllOpenTickets"); }
        }
        public static string ShowClosedTickets
        {
            get { return GetString("ShowClosedTickets"); }
        }
        public static string ShowCanceledTickets
        {
            get { return GetString("ShowCanceledTickets"); }
        }
        public static string ShowTodaysTickets
        {
            get { return GetString("ShowTodaysTickets"); }
        }
        public static string CreateTicket
        {
            get { return GetString("CreateTicket"); }
        }
        public static string Reports
        {
            get { return GetString("Reports"); }
        }
        public static string OrderEntryCancelItem
        {
            get { return GetString("OrderEntryCancelItem"); }
        }
        public static string OrderEntryVoidItem
        {
            get { return GetString("OrderEntryVoidItem"); }
        }
        public static string OrderEntrySpecialInstructions
        {
            get { return GetString("OrderEntrySpecialInstructions"); }
        }
        public static string OrderEntrySystemFunctions
        {
            get { return GetString("OrderEntrySystemFunctions"); }
        }
        public static string OrderEntrySetup
        {
            get { return GetString("OrderEntrySetup"); }
        }
        public static string OrderEntryRegisterFunctions
        {
            get { return GetString("OrderEntryRegisterFunctions"); }
        }
        public static string OrderEntryCoupon
        {
            get { return GetString("OrderEntryCoupon"); }
        }
        public static string OrderEntryCancel
        {
            get { return GetString("OrderEntryCancel"); }
        }
        public static string OrderEntryDiscount
        {
            get { return GetString("OrderEntryDiscount"); }
        }
        public static string OrderEntryChangeOccasion
        {
            get { return GetString("OrderEntryChangeOccasion"); }
        }
        public static string OrderEntryFutureTime
        {
            get { return GetString("OrderEntryFutureTime"); }
        }
        public static string OrderEntryConvertToParty
        {
            get { return GetString("OrderEntryConvertToParty"); }
        }
        public static string OrderEntryManageParty
        {
            get { return GetString("OrderEntryManageParty"); }
        }
        public static string OrderEntryTicketComment
        {
            get { return GetString("OrderEntryTicketComment"); }
        }
        public static string OrderEntryTaxExemption
        {
            get { return GetString("OrderEntryTaxExemption"); }
        }
        public static string OrderEntryCashOut
        {
            get { return GetString("OrderEntryCashOut"); }
        }
        public static string OrderEntryCancelTicket
        {
            get { return GetString("OrderEntryCancelTicket"); }
        }
        public static string OrderEntryVoidTicket
        {
            get { return GetString("OrderEntryVoidTicket"); }
        }
        public static string OrderEntryProcessReturns
        {
            get { return GetString("OrderEntryProcessReturns"); }
        }
        public static string OrderEntryPlaceOrder
        {
            get { return GetString("OrderEntryPlaceOrder"); }
        }
        public static string OrderEntryCloseTicket
        {
            get { return GetString("OrderEntryCloseTicket"); }
        }
        public static string OrderEntryReports
        {
            get { return GetString("OrderEntryReports"); }
        }
        public static string OrderEntryPersonalSettings
        {
            get { return GetString("OrderEntryPersonalSettings"); }
        }
        public static string OrderEntryChangePassword
        {
            get { return GetString("OrderEntryChangePassword"); }
        }
        public static string OrderEntryChangeLanguage
        {
            get { return GetString("OrderEntryChangeLanguage"); }
        }
        public static string OrderEntryClockOut
        {
            get { return GetString("OrderEntryClockOut"); }
        }
        public static string OrderEntryCommandConsole
        {
            get { return GetString("OrderEntryCommandConsole"); }
        }
        public static string OrderEntryLogout
        {
            get { return GetString("OrderEntryLogout"); }
        }
        public static string OrderEntryExit
        {
            get { return GetString("OrderEntryExit"); }
        }
        public static string OrderEntryEnterTaxExemptionId
        {
            get { return GetString("OrderEntryEnterTaxExemptionId"); }
        }
        public static string OrderEntrySubTotal
        {
            get { return GetString("OrderEntrySubTotal"); }
        }
        public static string OrderEntryErrorTicketAlreadyOpen
        {
            get { return GetString("OrderEntryErrorTicketAlreadyOpen"); }
        }
        public static string OrderEntryTicketLocked
        {
            get { return GetString("OrderEntryTicketLocked"); }
        }
        public static string OrderEntryReturnTotalIs
        {
            get { return GetString("OrderEntryReturnTotalIs"); }
        }
        public static string OrderEntryReturnTotal
        {
            get { return GetString("OrderEntryReturnTotal"); }
        }
        public static string OrderEntryConfirmCancelChanges
        {
            get { return GetString("OrderEntryConfirmCancelChanges"); }
        }
        public static string OrderEntryFutureTimeUpdateMessage
        {
            get { return GetString("OrderEntryFutureTimeUpdateMessage"); }
        }
        public static string OrderEntryUpdatePartyTickets
        {
            get { return GetString("OrderEntryUpdatePartyTickets"); }
        }
        public static string OrderEntryShowingAllTickets
        {
            get { return GetString("OrderEntryShowingAllTickets"); }
        }
        public static string OrderEntryShowingAllTicketsToday
        {
            get { return GetString("OrderEntryShowingAllTicketsToday"); }
        }
        public static string OrderEntryShowingAllOpenTickets
        {
            get { return GetString("OrderEntryShowingAllOpenTickets"); }
        }
        public static string OrderEntryShowingAllCanceledTickets
        {
            get { return GetString("OrderEntryShowingAllCanceledTickets"); }
        }
        public static string OrderEntryShowingAllClosedTickets
        {
            get { return GetString("OrderEntryShowingAllClosedTickets"); }
        }
        public static string OrderEntryShowingAllDispatchedTickets
        {
            get { return GetString("OrderEntryShowingAllDispatchedTickets"); }
        }
        public static string OrderEntryShowingYourOpenTickets
        {
            get { return GetString("OrderEntryShowingYourOpenTickets"); }
        }
        public static string OrderEntryShowingFutureTickets
        {
            get { return GetString("OrderEntryShowingFutureTickets"); }
        }
        public static string OrderEntryShowingTicketsFrom
        {
            get { return GetString("OrderEntryShowingTicketsFrom"); }
        }
        public static string OrderEntryTo
        {
            get { return GetString("OrderEntryTo"); }
        }
        public static string OrderEntryCatering
        {
            get { return GetString("OrderEntryCatering"); }
        }
        public static string OrderEntryDelivery
        {
            get { return GetString("OrderEntryDelivery"); }
        }
        public static string OrderEntryDineIn
        {
            get { return GetString("OrderEntryDineIn"); }
        }
        public static string OrderEntryDriveThru
        {
            get { return GetString("OrderEntryDriveThru"); }
        }
        public static string OrderEntryCarryout
        {
            get { return GetString("OrderEntryCarryout"); }
        }
        #endregion

        #region OrderEntryFunctionsControl
        public static string OrderEntryDispatchDriver
        {
            get { return GetString("OrderEntryDispatchDriver"); }
        }
        public static string OrderEntryStartOfDay
        {
            get { return GetString("OrderEntryStartOfDay"); }
        }
        public static string OrderEntryEndOfDay
        {
            get { return GetString("OrderEntryEndOfDay"); }
        }
        public static string OrderEntryEndOfYear
        {
            get { return GetString("OrderEntryEndOfYear"); }
        }
        public static string OrderEntryEditTimeSheet
        {
            get { return GetString("OrderEntryEditTimeSheet"); }
        }
        public static string OrderEntryEditInventory
        {
            get { return GetString("OrderEntryEditInventory"); }
        }
        #endregion

        #region OrderEntryReceiptTape
        public static string TicketItems
        {
            get { return GetString("TicketItems"); }
        }
        public static string SelectedTicketItemCommands
        {
            get { return GetString("SelectedTicketItemCommands"); }
        }
        public static string OrderEntryIncreaseQuantity
        {
            get { return GetString("OrderEntryIncreaseQuantity"); }
        }
        public static string OrderEntryButtonSetQuantity
        {
            get { return GetString("OrderEntryButtonSetQuantity"); }
        }
        public static string OrderEntryDecreaseQuantity
        {
            get { return GetString("OrderEntryDecreaseQuantity"); }
        }
        public static string OrderEntryButtonCancelItem
        {
            get { return GetString("OrderEntryButtonCancelItem"); }
        }
        public static string OrderEntryButtonVoidItem
        {
            get { return GetString("OrderEntryButtonVoidItem"); }
        }
        public static string OrderEntryButtonReturnItem
        {
            get { return GetString("OrderEntryButtonReturnItem"); }
        }
        public static string OrderEntrySpecialInstructionsFor
        {
            get { return GetString("OrderEntrySpecialInstructionsFor"); }
        }
        public static string OrderEntryNoCancelPermission
        {
            get { return GetString("OrderEntryNoCancelPermission"); }
        }
        public static string OrderEntryNoDecreasePermission
        {
            get { return GetString("OrderEntryNoDecreasePermission"); }
        }
        #endregion

        #region OrderEntryRegisterMenuControl
        public static string RegisterMenuStartDrawer
        {
            get { return GetString("RegisterMenuStartDrawer"); }
        }
        public static string RegisterMenuNoSale
        {
            get { return GetString("RegisterMenuNoSale"); }
        }
        public static string RegisterMenuDeposit
        {
            get { return GetString("RegisterMenuDeposit"); }
        }
        public static string RegisterMenuSafeDrop
        {
            get { return GetString("RegisterMenuSafeDrop"); }
        }
        public static string RegisterMenuPayout
        {
            get { return GetString("RegisterMenuPayout"); }
        }
        public static string RegisterMenuFloat
        {
            get { return GetString("RegisterMenuFloat"); }
        }
        public static string RegisterMenuDock
        {
            get { return GetString("RegisterMenuDock"); }
        }
        public static string RegisterMenuReport
        {
            get { return GetString("RegisterMenuReport"); }
        }
        public static string RegisterMenuCloseOut
        {
            get { return GetString("RegisterMenuCloseOut"); }
        }
        public static string RegisterMenuPreparingReport
        {
            get { return GetString("RegisterMenuPreparingReport"); }
        }
        public static string RegisterMenuRunStartOfDay
        {
            get { return GetString("RegisterMenuRunStartOfDay"); }
        }
        public static string RegisterMenuUnableToProceed
        {
            get { return GetString("RegisterMenuUnableToProceed"); }
        }
        public static string RegisterMenuDepositAmount
        {
            get { return GetString("RegisterMenuDepositAmount"); }
        }
        public static string RegisterMenuDropAmount
        {
            get { return GetString("RegisterMenuDropAmount"); }
        }
        public static string RegisterMenuCantDropThatMuch
        {
            get { return GetString("RegisterMenuCantDropThatMuch"); }
        }
        public static string RegisterMenuInvalidAmount
        {
            get { return GetString("RegisterMenuInvalidAmount"); }
        }
        public static string RegisterMenuPayoutReason
        {
            get { return GetString("RegisterMenuPayoutReason"); }
        }
        public static string RegisterMenuPayoutAmount
        {
            get { return GetString("RegisterMenuPayoutAmount"); }
        }
        public static string RegisterMenuConfirmFloat
        {
            get { return GetString("RegisterMenuConfirmFloat"); }
        }
        public static string RegisterMenuNotifyFloat
        {
            get { return GetString("RegisterMenuNotifyFloat"); }
        }
        public static string RegisterMenuNotFloating
        {
            get { return GetString("RegisterMenuNotFloating"); }
        }
        public static string RegisterMenuDrawerIsNowDocked
        {
            get { return GetString("RegisterMenuDrawerIsNowDocked"); }
        }
        public static string RegisterMenuConfirmDrawerClose
        {
            get { return GetString("RegisterMenuConfirmDrawerClose"); }
        }
        #endregion

        #region OrderEntrySetupControl
        public static string SetupGeneralSettings
        {
            get { return GetString("SetupGeneralSettings"); }
        }
        public static string SetupCategoriesAndItems
        {
            get { return GetString("SetupCategoriesAndItems"); }
        }
        public static string SetupCouponsAndDiscounts
        {
            get { return GetString("SetupCouponsAndDiscounts"); }
        }
        public static string SetupRoomsAndSeatings
        {
            get { return GetString("SetupRoomsAndSeatings"); }
        }
        public static string SetupEmployeesAndJobs
        {
            get { return GetString("SetupEmployeesAndJobs"); }
        }
        public static string SetupTaxes
        {
            get { return GetString("SetupTaxes"); }
        }
        public static string SetupHardware
        {
            get { return GetString("SetupHardware"); }
        }
        public static string SetupItemSetup
        {
            get { return GetString("SetupItemSetup"); }
        }
        public static string SetupCoupons
        {
            get { return GetString("SetupCoupons"); }
        }
        public static string SetupDiscounts
        {
            get { return GetString("SetupDiscounts"); }
        }
        public static string SetupCouponAndDiscountSetup
        {
            get { return GetString("SetupCouponAndDiscountSetup"); }
        }
        public static string SetupRoomSetup
        {
            get { return GetString("SetupRoomSetup"); }
        }
        public static string SetupEmployees
        {
            get { return GetString("SetupEmployees"); }
        }
        public static string SetupEmployeeJobs
        {
            get { return GetString("SetupEmployeeJobs"); }
        }
        public static string SetupEmployeeSetup
        {
            get { return GetString("SetupEmployeeSetup"); }
        }
        public static string SetupTaxSetup
        {
            get { return GetString("SetupTaxSetup"); }
        }
        public static string SetupHardwareSetup
        {
            get { return GetString("SetupHardwareSetup"); }
        }
        #endregion

        #region OrderEntryTicketDetailsControl
        public static string MapDestination
        {
            get { return GetString("MapDestination"); }
        }
        public static string SelectedTicketDetails
        {
            get { return GetString("SelectedTicketDetails"); }
        }
        public static string SelectedTicketCommands
        {
            get { return GetString("SelectedTicketCommands"); }
        }
        public static string OrderEntryFireEntrees
        {
            get { return GetString("OrderEntryFireEntrees"); }
        }
        public static string OrderEntryUnCancel
        {
            get { return GetString("OrderEntryUnCancel"); }
        }
        public static string OrderEntryChangeEmployee
        {
            get { return GetString("OrderEntryChangeEmployee"); }
        }
        public static string OrderEntryRefund
        {
            get { return GetString("OrderEntryRefund"); }
        }
        public static string OrderEntryMapDestination
        {
            get { return GetString("OrderEntryMapDestination"); }
        }
        public static string OrderEntrySelectedTicketDetails
        {
            get { return GetString("OrderEntrySelectedTicketDetails"); }
        }
        public static string OrderEntryTicketNumber
        {
            get { return GetString("OrderEntryTicketNumber"); }
        }
        public static string OrderEntryStatus
        {
            get { return GetString("OrderEntryStatus"); }
        }
        public static string OrderEntrySeating
        {
            get { return GetString("OrderEntrySeating"); }
        }
        public static string OrderEntryCreatedOn
        {
            get { return GetString("OrderEntryCreatedOn"); }
        }
        public static string OrderEntryReadyTime
        {
            get { return GetString("OrderEntryReadyTime"); }
        }
        public static string OrderEntryClosedTime
        {
            get { return GetString("OrderEntryClosedTime"); }
        }
        public static string OrderEntryOrderNumber
        {
            get { return GetString("OrderEntryOrderNumber"); }
        }
        public static string OrderEntryStartTime
        {
            get { return GetString("OrderEntryStartTime"); }
        }
        public static string OrderEntrySuspendedOrder
        {
            get { return GetString("OrderEntrySuspendedOrder"); }
        }
        public static string OrderEntryActiveFuture
        {
            get { return GetString("OrderEntryActiveFuture"); }
        }
        public static string OrderEntryActiveEntrees
        {
            get { return GetString("OrderEntryActiveEntrees"); }
        }
        public static string OrderEntryActive
        {
            get { return GetString("OrderEntryActive"); }
        }
        public static string OrderEntryServed
        {
            get { return GetString("OrderEntryServed"); }
        }
        #endregion

        #region Personal Settings
        public static string SettingsPersonalSettings
        {
	        get { return GetString("SettingsPersonalSettings"); }
        }
        public static string SettingsTemperatureScale
        {
	        get { return GetString("SettingsTemperatureScale"); }
        }
        public static string SettingsFahrenheit
        {
	        get { return GetString("SettingsFahrenheit"); }
        }
        public static string SettingsCelsius
        {
	        get { return GetString("SettingsCelsius"); }
        }
        public static string SettingsKelvin
        {
	        get { return GetString("SettingsKelvin"); }
        }
        #endregion

        #region OrderEntryCommands
        public static string View
        {
            get { return GetString("View"); }
        }
        public static string YouHaveChangedTheFutureTimeForThisTicketWouldYou
        {
            get { return GetString("YouHaveChangedTheFutureTimeForThisTicketWouldYou"); }
        }
        public static string LikeToUpdateTheOtherTicketsInThisPartyToTheSameFutureTime
        {
            get { return GetString("LikeToUpdateTheOtherTicketsInThisPartyToTheSameFutureTime"); }
        }
        public static string UpdatePartyTickets
        {
            get { return GetString("UpdatePartyTickets"); }
        }
        public static string EnterTaxExemptionId
        {
            get { return GetString("EnterTaxExemptionId"); }
        }
        public static string ThereIsNoRegisterDrawerStarted
        {
            get { return GetString("ThereIsNoRegisterDrawerStarted"); }
        }
        public static string TicketCashout
        {
            get { return GetString("TicketCashout"); }
        }
        public static string YouDoNotHavePermissionToUseTheCurrentRegisterDrawer
        {
            get { return GetString("YouDoNotHavePermissionToUseTheCurrentRegisterDrawer"); }
        }
        public static string ThereAreNoItemsOnThisTicket
        {
            get { return GetString("ThereAreNoItemsOnThisTicket"); }
        }
        public static string ThisTicketYouAreTryingToCashoutIsCurrentlyOpenOnAnotherTerminal
        {
            get { return GetString("ThisTicketYouAreTryingToCashoutIsCurrentlyOpenOnAnotherTerminal"); }
        }
        public static string TicketLocked
        {
            get { return GetString("TicketLocked"); }
        }
        public static string PrintingIsDisabledInDemoVersion
        {
            get { return GetString("PrintingIsDisabledInDemoVersion"); }
        }
        public static string Disabled
        {
            get { return GetString("Disabled"); }
        }
        public static string ChangeEmployee
        {
            get { return GetString("ChangeEmployee"); }
        }
        public static string RefundedTotalIs
        {
            get { return GetString("RefundedTotalIs"); }
        }
        public static string RefundTotal
        {
            get { return GetString("RefundTotal"); }
        }
        #endregion

        #region ManagerAlert
        public static string ManagerAlert
        {
            get { return GetString("ManagerAlert"); }
        }
        #endregion

        #region PosHelper
        public static string StartOfDay2
        {
            get { return GetString("StartOfDay2"); }
        }
        public static string PermissionRequiredEnterPassword
        {
            get { return GetString("PermissionRequiredEnterPassword"); }
        }
        public static string AreYouSureYouWantToReturnTheSelectedTicketItems
        {
            get { return GetString("AreYouSureYouWantToReturnTheSelectedTicketItems"); }
        }
        public static string ReturnTicketItems
        {
            get { return GetString("ReturnTicketItems"); }
        }
        public static string RefundTicket
        {
            get { return GetString("RefundTicket"); }
        }
        public static string CancelTicket
        {
            get { return GetString("CancelTicket"); }
        }
        public static string CancelTicketItem
        {
            get { return GetString("CancelTicketItem"); }
        }
        public static string AreYouSureYouWantToCancelThisTicket
        {
            get { return GetString("AreYouSureYouWantToCancelThisTicket"); }
        }
        public static string AreYouSureYouWantToCancelThisTicketItem
        {
            get { return GetString("AreYouSureYouWantToCancelThisTicketItem"); }
        }
        public static string AreYouSureYouWantToUncancelThisTicket
        {
            get { return GetString("AreYouSureYouWantToUncancelThisTicket"); }
        }
        public static string UncancelTicket
        {
            get { return GetString("UncancelTicket"); }
        }
        public static string AreYouSureYouWantToUncancelThisTicketItem
        {
            get { return GetString("AreYouSureYouWantToUncancelThisTicketItem"); }
        }
        public static string UncancelTicketItem
        {
            get { return GetString("UncancelTicketItem"); }
        }
        public static string AreYouSureYouWantToVoidThisTicket
        {
            get { return GetString("AreYouSureYouWantToVoidThisTicket"); }
        }
        public static string VoidTicket
        {
            get { return GetString("VoidTicket"); }
        }
        public static string AreYouSureYouWantToVoidThisTicketItem
        {
            get { return GetString("AreYouSureYouWantToVoidThisTicketItem"); }
        }
        public static string YouCantVoidATicketThatIsCashedoutTheTicketMustBeRefundedFirst
        {
            get { return GetString("YouCantVoidATicketThatIsCashedoutTheTicketMustBeRefundedFirst"); }
        }
        public static string VoidError
        {
            get { return GetString("VoidError"); }
        }
        public static string YouCanNotRunTheEndofdayReportUntilAllOpenTicketsAreClosedout
        {
            get { return GetString("YouCanNotRunTheEndofdayReportUntilAllOpenTicketsAreClosedout"); }
        }
        public static string EndofdayReport
        {
            get { return GetString("EndofdayReport"); }
        }
        public static string YouCanNotRunTheEndofdayReportUntilYouCloseoutAllActiveRegisterDrawers
        {
            get { return GetString("YouCanNotRunTheEndofdayReportUntilYouCloseoutAllActiveRegisterDrawers"); }
        }
        public static string TheEndOfDayReportShouldOnlyBeRunAtTheEndOfTheDay
        {
            get { return GetString("TheEndOfDayReportShouldOnlyBeRunAtTheEndOfTheDay"); }
        }
        public static string AreYouSureYouWantToRunThisReport
        {
            get { return GetString("AreYouSureYouWantToRunThisReport"); }
        }
        public static string TheEndOfYearReportShouldOnlyBeRunAtTheBeginningOfANewYear
        {
            get { return GetString("TheEndOfYearReportShouldOnlyBeRunAtTheBeginningOfANewYear"); }
        }
        public static string Dailyidoffset
        {
            get { return GetString("Dailyidoffset"); }
        }
        #endregion

        #region SqlServerSetup
        public static string AboutToInstallDatabase
        {
            get { return GetString("AboutToInstallDatabase"); }
        }
        public static string DatabaseInstallationException
        {
            get { return GetString("DatabaseInstallationException"); }
        }
        public static string None2
        {
            get { return GetString("None"); }
        }
        public static string DatabasePatchException
        {
            get { return GetString("DatabasePatchException"); }
        }
        #endregion

        #region TaskManagerServiceHelper
        public static string FailedToInstallTheTaskManagerAccessService
        {
            get { return GetString("FailedToInstallTheTaskManagerAccessService"); }
        }
        public static string ServiceInstallationError
        {
            get { return GetString("ServiceInstallationError"); }
        }
        public static string FailedToStartTheTaskManagerAccessService
        {
            get { return GetString("FailedToStartTheTaskManagerAccessService"); }
        }
        public static string ServiceStartError
        {
            get { return GetString("ServiceStartError"); }
        }
        #endregion

        #region DeviceManager
        public static string Bar1
        {
            get { return GetString("Bar1"); }
        }
        public static string Bar2
        {
            get { return GetString("Bar2"); }
        }
        public static string Bar3
        {
            get { return GetString("Bar3"); }
        }
        public static string InitializationException
        {
            get { return GetString("InitializationException"); }
        }
        public static string Local
        {
            get { return GetString("Local"); }
        }
        public static string Journal
        {
            get { return GetString("Journal"); }
        }
        public static string Kitchen1
        {
            get { return GetString("Kitchen1"); }
        }
        public static string Kitchen2
        {
            get { return GetString("Kitchen2"); }
        }
        public static string Kitchen3
        {
            get { return GetString("Kitchen3"); }
        }
        public static string Unused
        {
            get { return GetString("Unused"); }
        }
        #endregion

        #region PrinterManager
        public static string TaxTotal
        {
            get { return GetString("TaxTotal"); }
        }
        public static string ReturnTotal
        {
            get { return GetString("ReturnTotal"); }
        }
        public static string Order
        {
            get { return GetString("Order"); }
        }
        public static string TicketNumber
        {
            get { return GetString("Ticket"); }
        }
        public static string Table
        {
            get { return GetString("Table"); }
        }
        public static string TicketVoid
        {
            get { return GetString("TicketVoid"); }
        }
        public static string TicketRefund
        {
            get { return GetString("TicketRefund"); }
        }
        public static string TicketCancelMade
        {
            get { return GetString("TicketCancelMade"); }
        }
        public static string TicketCancelUnmade
        {
            get { return GetString("TicketCancelUnmade"); }
        }
        public static string TicketItemVoid
        {
            get { return GetString("TicketItemVoid"); }
        }
        public static string TicketItemReturn
        {
            get { return GetString("TicketItemReturn"); }
        }
        public static string FutureOrder
        {
            get { return GetString("FutureOrder"); }
        }
        public static string MakeNow
        {
            get { return GetString("MakeNow"); }
        }
        public static string CancelMade
        {
            get { return GetString("CancelMade"); }
        }
        public static string CancelUnmade
        {
            get { return GetString("CancelUnmade"); }
        }
        public static string Hold
        {
            get { return GetString("Hold"); }
        }
        public static string Fired
        {
            get { return GetString("Fired"); }
        }
        public static string Make
        {
            get { return GetString("Make"); }
        }
        public static string Changed
        {
            get { return GetString("Changed"); }
        }
        public static string Coupons
        {
            get { return GetString("Coupons"); }
        }
        public static string Subtotal
        {
            get { return GetString("Subtotal"); }
        }
        public static string Tax
        {
            get { return GetString("Tax"); }
        }
        public static string Total
        {
            get { return GetString("Total"); }
        }
        public static string Unhandled
        {
            get { return GetString("Unhandled"); }
        }
        #endregion

        #region RegisterManager
        public static string CanNotDetermineTheRegisterIDCheckNetworkSetup
        {
            get { return GetString("CanNotDetermineTheRegisterIDCheckNetworkSetup"); }
        }
        public static string ThereAreNoPhysicalCashRegisterDrawersSetup
        {
            get { return GetString("ThereAreNoPhysicalCashRegisterDrawersSetup"); }
        }
        public static string BothDrawersAre
        {
            get { return GetString("BothDrawersAre"); }
        }
        public static string TheRegisterDrawerIs
        {
            get { return GetString("TheRegisterDrawerIs"); }
        }
        public static string AlreadyBeingUsedBy
        {
            get { return GetString("AlreadyBeingUsedBy"); }
        }
        public static string OtherEmployees
        {
            get { return GetString("OtherEmployees"); }
        }
        public static string AnotherEmployee
        {
            get { return GetString("AnotherEmployee"); }
        }
        #endregion

        #region ReportManager
        public static string EndOfDayReport
        {
            get { return GetString("EndOfDayReport"); }
        }
        public static string TotalGrossSales
        {
            get { return GetString("TotalGrossSales"); }
        }
        public static string TaxCollected
        {
            get { return GetString("TaxCollected"); }
        }
        public static string TaxExemptSalesTotal
        {
            get { return GetString("TaxExemptSalesTotal"); }
        }
        public static string NumberOfTickets
        {
            get { return GetString("NumberOfTickets"); }
        }
        public static string TotalOfCoupons
        {
            get { return GetString("TotalOfCoupons"); }
        }
        public static string TotalOfDiscounts
        {
            get { return GetString("TotalOfDiscounts"); }
        }
        public static string TotalOfRefunds
        {
            get { return GetString("TotalOfRefunds"); }
        }
        public static string TotalOfReturns
        {
            get { return GetString("TotalOfReturns"); }
        }
        public static string TotalOfPayouts
        {
            get { return GetString("TotalOfPayouts"); }
        }
        public static string EndOfYearReport
        {
            get { return GetString("EndOfYearReport"); }
        }
        public static string TotalSalesByItem
        {
            get { return GetString("TotalSalesByItem"); }
        }
        public static string TotalSalesByItemFor
        {
            get { return GetString("TotalSalesByItemFor"); }
        }
        public static string From
        {
            get { return GetString("From"); }
        }
        public static string To
        {
            get { return GetString("To"); }
        }
        public static string ItemQuantitySalesRevenueIngredientCost
        {
            get { return GetString("ItemQuantitySalesRevenueIngredientCost"); }
        }
        public static string TotalOfGrossSales
        {
            get { return GetString("TotalOfGrossSales"); }
        }
        public static string TotalSalesByCategory
        {
            get { return GetString("TotalSalesByCategory"); }
        }
        public static string TotalSalesByCategoryFor
        {
            get { return GetString("TotalSalesByCategoryFor"); }
        }
        public static string CategoryQuantitySalesRevenue
        {
            get { return GetString("CategoryQuantitySalesRevenue"); }
        }
        public static string TotalSalesByEmployee
        {
            get { return GetString("TotalSalesByEmployee"); }
        }
        public static string EmployeeSalesRevenue
        {
            get { return GetString("EmployeeSalesRevenue"); }
        }
        public static string EmployeeSalesByCategory
        {
            get { return GetString("EmployeeSalesByCategory"); }
        }
        public static string EmployeeSalesByItem
        {
            get { return GetString("EmployeeSalesByItem"); }
        }
        public static string EmployeeHours
        {
            get { return GetString("EmployeeHours"); }
        }
        public static string EmployeeJobHours
        {
            get { return GetString("EmployeeJobHours"); }
        }
        public static string HourlyLaborTotals
        {
            get { return GetString("HourlyLaborTotals"); }
        }
        public static string HourlyLaborTotal
        {
            get { return GetString("HourlyLaborTotal"); }
        }
        public static string TimeOfDayLaborHoursCostOfLabor
        {
            get { return GetString("TimeOfDayLaborHoursCostOfLabor"); }
        }
        public static string TimesheetChangeLog
        {
            get { return GetString("TimesheetChangeLog"); }
        }
        public static string EmployeeTimesheetChangeLog
        {
            get { return GetString("EmployeeTimesheetChangeLog"); }
        }
        public static string DeletedTimesheetEntryFor
        {
            get { return GetString("DeletedTimesheetEntryFor"); }
        }
        public static string ToriginalStartTimeWas
        {
            get { return GetString("ToriginalStartTimeWas"); }
        }
        public static string ToriginalEndTimeWas
        {
            get { return GetString("ToriginalEndTimeWas"); }
        }
        public static string ToriginalJobWas
        {
            get { return GetString("ToriginalJobWas"); }
        }
        public static string ToriginalTipsWas
        {
            get { return GetString("ToriginalTipsWas"); }
        }
        public static string ToriginalDriverCompensationWas
        {
            get { return GetString("ToriginalDriverCompensationWas"); }
        }
        public static string ChangedTimesheetEntryFor
        {
            get { return GetString("ChangedTimesheetEntryFor"); }
        }
        public static string TtimesheetEntryIdIs
        {
            get { return GetString("TtimesheetEntryIdIs"); }
        }
        public static string RegisterReport
        {
            get { return GetString("RegisterReport"); }
        }
        public static string RegisterReportForRegisterDrawer
        {
            get { return GetString("RegisterReportForRegisterDrawer"); }
        }
        public static string Time
        {
            get { return GetString("Time"); }
        }
        public static string StartingAmount
        {
            get { return GetString("StartingAmount"); }
        }
        public static string EndingAmount
        {
            get { return GetString("EndingAmount"); }
        }
        public static string DrawerAmount
        {
            get { return GetString("DrawerAmount"); }
        }
        public static string DepositAmount
        {
            get { return GetString("DepositAmount"); }
        }
        public static string SafeDropAmount
        {
            get { return GetString("SafeDropAmount"); }
        }
        public static string NumberOfCoupons
        {
            get { return GetString("NumberOfCoupons"); }
        }
        public static string NumberOfDiscounts
        {
            get { return GetString("NumberOfDiscounts"); }
        }
        public static string NumberOfRefunds
        {
            get { return GetString("NumberOfRefunds"); }
        }
        public static string NumberOfReturns
        {
            get { return GetString("NumberOfReturns"); }
        }
        public static string NumberOfPayouts
        {
            get { return GetString("NumberOfPayouts"); }
        }
        public static string NumberOfNoSales
        {
            get { return GetString("NumberOfNoSales"); }
        }
        public static string VoidTransactionReport
        {
            get { return GetString("VoidTransactionReport"); }
        }
        public static string EmployeeTimeTicketNumberAmount
        {
            get { return GetString("EmployeeTimeTicketNumberAmount"); }
        }
        public static string ReturnTransactionReport
        {
            get { return GetString("ReturnTransactionReport"); }
        }
        public static string EmployeeTimeTicketNumberNumberItemAmount
        {
            get { return GetString("EmployeeTimeTicketNumberNumberItemAmount"); }
        }
        public static string RefundTransactionReport
        {
            get { return GetString("RefundTransactionReport"); }
        }
        public static string EmployeeTimeTicketNumberRegisterNumberTypeAmount
        {
            get { return GetString("EmployeeTimeTicketNumberRegisterNumberTypeAmount"); }
        }
        public static string Reopened
        {
            get { return GetString("Reopened"); }
        }
        public static string Voided
        {
            get { return GetString("Voided"); }
        }
        public static string NoSaleTransactionReport
        {
            get { return GetString("NoSaleTransactionReport"); }
        }
        public static string EmployeeTimeRegisterNumber
        {
            get { return GetString("EmployeeTimeRegisterNumber"); }
        }
        public static string SafeDropTransactionReport
        {
            get { return GetString("SafeDropTransactionReport"); }
        }
        public static string EmployeeTimeRegisterNumberAmount
        {
            get { return GetString("EmployeeTimeRegisterNumberAmount"); }
        }
        public static string PayoutTransactionReport
        {
            get { return GetString("PayoutTransactionReport"); }
        }
        public static string EmployeeTimeRegisterNumberReasonAmount
        {
            get { return GetString("EmployeeTimeRegisterNumberReasonAmount"); }
        }
        public static string RegisterDepositTransactionReport
        {
            get { return GetString("RegisterDepositTransactionReport"); }
        }
        public static string TicketItemCancelTransactionReport
        {
            get { return GetString("TicketItemCancelTransactionReport"); }
        }
        public static string EmployeeTimeTicketNumberItemQuantityAmount
        {
            get { return GetString("EmployeeTimeTicketNumberItemQuantityAmount"); }
        }
        public static string Unmade
        {
            get { return GetString("Unmade"); }
        }
        public static string FloatingDockingTransactionReport
        {
            get { return GetString("FloatingDockingTransactionReport"); }
        }
        public static string EmployeeUndockTimeDockTimeSourceDestination
        {
            get { return GetString("EmployeeUndockTimeDockTimeSourceDestination"); }
        }
        public static string Undocked
        {
            get { return GetString("Undocked"); }
        }
        public static string ItemAdjustmentReport
        {
            get { return GetString("ItemAdjustmentReport"); }
        }
        public static string WhenEmployeeTypeItemItemOptionSet
        {
            get { return GetString("WhenEmployeeTypeItemItemOptionSet"); }
        }
        public static string AddedItem
        {
            get { return GetString("AddedItem"); }
        }
        public static string DeletedItem
        {
            get { return GetString("DeletedItem"); }
        }
        public static string OptionSetAdd
        {
            get { return GetString("OptionSetAdd"); }
        }
        public static string OptionSetDelete
        {
            get { return GetString("OptionSetDelete"); }
        }
        public static string ItemPriceChangeReport
        {
            get { return GetString("ItemPriceChangeReport"); }
        }
        public static string WhenEmployeeItemChangeOriginalValueNewValue
        {
            get { return GetString("WhenEmployeeItemChangeOriginalValueNewValue"); }
        }
        public static string RegularPrice
        {
            get { return GetString("RegularPrice"); }
        }
        public static string SpecialPrice
        {
            get { return GetString("SpecialPrice"); }
        }
        public static string Added
        {
            get { return GetString("Added"); }
        }
        public static string Removed
        {
            get { return GetString("Removed"); }
        }
        public static string DayOfWeek
        {
            get { return GetString("DayOfWeek"); }
        }
        public static string StartTime
        {
            get { return GetString("StartTime"); }
        }
        public static string EndTime
        {
            get { return GetString("EndTime"); }
        }
        public static string IngredientRecipeAdjustmentReport
        {
            get { return GetString("IngredientRecipeAdjustmentReport"); }
        }
        public static string WhenEmployeeIngredientOriginalQuantityNewQuantityDescription
        {
            get { return GetString("WhenEmployeeIngredientOriginalQuantityNewQuantityDescription"); }
        }
        public static string MeasurementChanged
        {
            get { return GetString("MeasurementChanged"); }
        }
        public static string YieldChanged
        {
            get { return GetString("YieldChanged"); }
        }
        public static string InventoryAdjustmentsReport
        {
            get { return GetString("InventoryAdjustmentsReport"); }
        }
        public static string WhenEmployeeOriginalQuantityNewQuantityIngredient
        {
            get { return GetString("WhenEmployeeOriginalQuantityNewQuantityIngredient"); }
        }
        public static string ItemRecipeAdjustmentReport
        {
            get { return GetString("ItemRecipeAdjustmentReport"); }
        }
        public static string WhenEmployeeItemOriginalQuantityNewQuantityIngredient
        {
            get { return GetString("WhenEmployeeItemOriginalQuantityNewQuantityIngredient"); }
        }
        public static string CurrentInventoryReport
        {
            get { return GetString("CurrentInventoryReport"); }
        }
        public static string IngredientQuantity
        {
            get { return GetString("IngredientQuantity"); }
        }
        public static string WasteByIngredientReport
        {
            get { return GetString("WasteByIngredientReport"); }
        }
        public static string IngredientQuantityCost
        {
            get { return GetString("IngredientQuantityCost"); }
        }
        public static string UsageByIngredientReport
        {
            get { return GetString("UsageByIngredientReport"); }
        }
        public static string WasteByItemReport
        {
            get { return GetString("WasteByItemReport"); }
        }
        public static string ItemQuantityItemCostIngredientCost
        {
            get { return GetString("ItemQuantityItemCostIngredientCost"); }
        }
        public static string WasteByCategoryReport
        {
            get { return GetString("WasteByCategoryReport"); }
        }
        public static string CategoryQuantityItemCostIngredientCost
        {
            get { return GetString("CategoryQuantityItemCostIngredientCost"); }
        }
        #endregion

        #region UserControlManager
        public static string YouAreNotUsingWindowsVistaOrNewerWindowsKeyboardProtectionIsDisabled
        {
            get { return GetString("YouAreNotUsingWindowsVistaOrNewerWindowsKeyboardProtectionIsDisabled"); }
        }
        #endregion

        #region CouponEditorControl
        public static string CouponDetails
        {
            get { return GetString("CouponDetails"); }
        }
        public static string CouponCategories
        {
            get { return GetString("CouponCategories"); }
        }
        public static string CouponItems
        {
            get { return GetString("CouponItems"); }
        }
        public static string SelectCategories
        {
            get { return GetString("SelectCategories"); }
        }
        public static string SelectItems
        {
            get { return GetString("SelectItems"); }
        }
        #endregion

        #region OrderEntryItemSelectionControl
        public static string YouCanNotAddMoreThan3TicketItemsToATicketInTheDemoVersion
        {
            get { return GetString("YouCanNotAddMoreThan3TicketItemsToATicketInTheDemoVersion"); }
        }
        public static string DemoRestriction
        {
            get { return GetString("DemoRestriction"); }
        }
        #endregion

        #region OrderEntryOrderCommandsControl
        public static string ConvertToParty
        {
            get { return GetString("ConvertToParty"); }
        }
        public static string ManageParty
        {
            get { return GetString("ManageParty"); }
        }
        #endregion

        #region OrderEntryTicketSelectionControl
        public static string TicketFilter
        {
            get { return GetString("TicketFilter"); }
        }
        public static string OccasionFilter
        {
            get { return GetString("OccasionFilter"); }
        }
        public static string Tickets
        {
            get { return GetString("Tickets"); }
        }
        public static string TicketCommands
        {
            get { return GetString("TicketCommands"); }
        }
        public static string SelectDateRange
        {
            get { return GetString("SelectDateRange"); }
        }
        public static string YouCanNotCreateTicketsUntilStartOfDayHasBeenCompleted
        {
            get { return GetString("YouCanNotCreateTicketsUntilStartOfDayHasBeenCompleted"); }
        }
        public static string RequiresStartOfDay
        {
            get { return GetString("RequiresStartOfDay"); }
        }
        public static string Party
        {
            get { return GetString("Party"); }
        }
        public static string CreateTime
        {
            get { return GetString("CreateTime"); }
        }
        public static string Comment
        {
            get { return GetString("Comment"); }
        }
        #endregion

        #region PartyEditControl
        public static string PartyHostsName
        {
            get { return GetString("PartyHostsName"); }
        }
        public static string PartySize
        {
            get { return GetString("PartySize"); }
        }
        public static string Notes
        {
            get { return GetString("Notes"); }
        }
        public static string InvitedGuestList
        {
            get { return GetString("InvitedGuestList"); }
        }
        public static string InvitesName
        {
            get { return GetString("InvitesName"); }
        }
        public static string EditParty
        {
            get { return GetString("EditParty"); }
        }
        #endregion

        #region PartyManagementControl
        public static string AddTicket
        {
            get { return GetString("AddTicket"); }
        }
        public static string RemoveTicket
        {
            get { return GetString("RemoveTicket"); }
        }
        public static string ChangeSeating
        {
            get { return GetString("ChangeSeating"); }
        }
        public static string EditPartyInfo
        {
            get { return GetString("EditPartyInfo"); }
        }
        public static string SelectAllItems
        {
            get { return GetString("SelectAllItems"); }
        }
        public static string UnselectAllSelected
        {
            get { return GetString("UnselectAllSelected"); }
        }
        public static string SingleTicket
        {
            get { return GetString("SingleTicket"); }
        }
        public static string CurrentTicket
        {
            get { return GetString("CurrentTicket"); }
        }
        public static string ChangeTicket
        {
            get { return GetString("ChangeTicket"); }
        }
        public static string ThisTicketIsOpenSomewhereElseItCanNotBeModified
        {
            get { return GetString("ThisTicketIsOpenSomewhereElseItCanNotBeModified"); }
        }
        public static string ThePartyInformationForThisTicketIsCurrentlyBeingModifiedSomewhereElse
        {
            get { return GetString("ThePartyInformationForThisTicketIsCurrentlyBeingModifiedSomewhereElse"); }
        }
        public static string PartyInformationLocked
        {
            get { return GetString("PartyInformationLocked"); }
        }
        public static string YouCanOnlyHave2PartyTicketsInTheDemoVersion
        {
            get { return GetString("YouCanOnlyHave2PartyTicketsInTheDemoVersion"); }
        }
        public static string TheSelectedTicketCanNotBeRemovedBecauseItAlreadyOpenedSomewhereElse
        {
            get { return GetString("TheSelectedTicketCanNotBeRemovedBecauseItAlreadyOpenedSomewhereElse"); }
        }
        public static string YouCanNotRemoveATicketThatHasItemsOnItFirstTransferThoseItemsToADifferentTicket
        {
            get { return GetString("YouCanNotRemoveATicketThatHasItemsOnItFirstTransferThoseItemsToADifferentTicket"); }
        }
        public static string TicketNotEmpty
        {
            get { return GetString("TicketNotEmpty"); }
        }
        public static string AreYouSureYouWantToConvertThisPartyBackIntoASingleTicket
        {
            get { return GetString("AreYouSureYouWantToConvertThisPartyBackIntoASingleTicket"); }
        }
        public static string ConfirmSingleTicket
        {
            get { return GetString("ConfirmSingleTicket"); }
        }
        public static string OneOrMoreOfTheTicketsInThisPartyIsCurrentlyBeingModifiedSomewhereElseCanNotChangeToSingleTicket
        {
            get { return GetString("OneOrMoreOfTheTicketsInThisPartyIsCurrentlyBeingModifiedSomewhereElseCanNotChangeToSingleTicket"); }
        }
        public static string YouCanNotAddMoreThan3TicketItemsToASingleTicketInTheDemoVersionAdditionalTicketItemsWillBeRemoved
        {
            get { return GetString("YouCanNotAddMoreThan3TicketItemsToASingleTicketInTheDemoVersionAdditionalTicketItemsWillBeRemoved"); }
        }
        public static string TheDestinationTicketIsOpenSomewhereElseItCanNotHaveItemsTransferedToIt
        {
            get { return GetString("TheDestinationTicketIsOpenSomewhereElseItCanNotHaveItemsTransferedToIt"); }
        }
        public static string YouCanNotMoveAnyMoreTicketItemsToTheDestinationTicketTheDemoVersionIsLimitedTo3TicketItemsPerTicket
        {
            get { return GetString("YouCanNotMoveAnyMoreTicketItemsToTheDestinationTicketTheDemoVersionIsLimitedTo3TicketItemsPerTicket"); }
        }
        public static string NotAllTheTicketItemsWereMovedTheDemoVersionLimitedTo3TicketItemsPerTicket
        {
            get { return GetString("NotAllTheTicketItemsWereMovedTheDemoVersionLimitedTo3TicketItemsPerTicket"); }
        }
        #endregion

        #region RegisterDrawerStartControl
        public static string StartDrawer
        {
            get { return GetString("StartDrawer"); }
        }
        public static string YouHaveNotSpecifiedAStartingAmount
        {
            get { return GetString("YouHaveNotSpecifiedAStartingAmount"); }
        }
        public static string StartUpRegister
        {
            get { return GetString("StartUpRegister"); }
        }
        #endregion

        #region ReportsMenuControl
        public static string IngredientRecipeAdjustments
        {
            get { return GetString("IngredientRecipeAdjustments"); }
        }
        public static string ItemAdjustments
        {
            get { return GetString("ItemAdjustments"); }
        }
        public static string ItemRecipeAdjustments
        {
            get { return GetString("ItemRecipeAdjustments"); }
        }
        public static string TodayOnly
        {
            get { return GetString("TodayOnly"); }
        }
        public static string SpecifyDates
        {
            get { return GetString("SpecifyDates"); }
        }
        public static string OperationalDays
        {
            get { return GetString("OperationalDays"); }
        }
        public static string MonthToDate
        {
            get { return GetString("MonthToDate"); }
        }
        public static string YearToDate
        {
            get { return GetString("YearToDate"); }
        }
        public static string AllDates
        {
            get { return GetString("AllDates"); }
        }
        public static string TotalSales
        {
            get { return GetString("TotalSales"); }
        }
        public static string SalesByItem
        {
            get { return GetString("SalesByItem"); }
        }
        public static string SalesByCategory
        {
            get { return GetString("SalesByCategory"); }
        }
        public static string SalesByEmployee
        {
            get { return GetString("SalesByEmployee"); }
        }
        public static string CostOfSales
        {
            get { return GetString("CostOfSales"); }
        }
        public static string EmployeeSales
        {
            get { return GetString("EmployeeSales"); }
        }
        public static string TicketTransactions
        {
            get { return GetString("TicketTransactions"); }
        }
        public static string Cancels
        {
            get { return GetString("Cancels"); }
        }
        public static string Voids
        {
            get { return GetString("Voids"); }
        }
        public static string Returns
        {
            get { return GetString("Returns"); }
        }
        public static string Refunds
        {
            get { return GetString("Refunds"); }
        }
        public static string RegisterTransactions
        {
            get { return GetString("RegisterTransactions"); }
        }
        public static string NoSales
        {
            get { return GetString("NoSales"); }
        }
        public static string Payouts
        {
            get { return GetString("Payouts"); }
        }
        public static string SafeDrops
        {
            get { return GetString("SafeDrops"); }
        }
        public static string Deposits
        {
            get { return GetString("Deposits"); }
        }
        public static string FloatingDocking
        {
            get { return GetString("FloatingDocking"); }
        }
        public static string Labor
        {
            get { return GetString("Labor"); }
        }
        public static string HourlyTotals
        {
            get { return GetString("HourlyTotals"); }
        }
        public static string Inventory
        {
            get { return GetString("Inventory"); }
        }
        public static string IngredientUsage
        {
            get { return GetString("IngredientUsage"); }
        }
        public static string CurrentInventory
        {
            get { return GetString("CurrentInventory"); }
        }
        public static string Waste
        {
            get { return GetString("Waste"); }
        }
        public static string WasteByItem
        {
            get { return GetString("WasteByItem"); }
        }
        public static string WasteByCategory
        {
            get { return GetString("WasteByCategory"); }
        }
        public static string WasteByIngredient
        {
            get { return GetString("WasteByIngredient"); }
        }
        public static string WasteByHour
        {
            get { return GetString("WasteByHour"); }
        }
        public static string Logging
        {
            get { return GetString("Logging"); }
        }
        public static string PriceChanges
        {
            get { return GetString("PriceChanges"); }
        }
        public static string TimesheetChanges
        {
            get { return GetString("TimesheetChanges"); }
        }
        public static string RangeOptions
        {
            get { return GetString("RangeOptions"); }
        }
        public static string PreparingReport
        {
            get { return GetString("PreparingReport"); }
        }
        #endregion

        #region SeatingDineInControl
        public static string SelectSeating
        {
            get { return GetString("SelectSeating"); }
        }
        public static string SEATINGISNOTSETUP
        {
            get { return GetString("SEATINGISNOTSETUP"); }
        }
        #endregion

        #region SeatingMaintenanceControl
        public static string NewRoom
        {
            get { return GetString("NewRoom"); }
        }
        public static string EditSeatings
        {
            get { return GetString("EditSeatings"); }
        }
        public static string DeleteRoom
        {
            get { return GetString("DeleteRoom"); }
        }
        public static string UpdateRoom
        {
            get { return GetString("UpdateRoom"); }
        }
        public static string SeatingProperties
        {
            get { return GetString("SeatingProperties"); }
        }
        public static string RoomProperties
        {
            get { return GetString("RoomProperties"); }
        }
        public static string Rooms
        {
            get { return GetString("Rooms"); }
        }
        public static string AddRoom
        {
            get { return GetString("AddRoom"); }
        }
        public static string RoomSetup
        {
            get { return GetString("RoomSetup"); }
        }
        public static string Seatings
        {
            get { return GetString("Seatings"); }
        }
        public static string AddSeating
        {
            get { return GetString("AddSeating"); }
        }
        public static string DeleteSeating
        {
            get { return GetString("DeleteSeating"); }
        }
        public static string UpdateSeating
        {
            get { return GetString("UpdateSeating"); }
        }
        public static string EditRooms
        {
            get { return GetString("EditRooms"); }
        }
        public static string SeatingSetupRoom
        {
            get { return GetString("SeatingSetupRoom"); }
        }
        public static string AreYouSureYouWantToDeleteTheSelectedRoomAndAllItsSeatings
        {
            get { return GetString("AreYouSureYouWantToDeleteTheSelectedRoomAndAllItsSeatings"); }
        }
        public static string AreYouSureYouWantToDeleteTheSelectedRoom
        {
            get { return GetString("AreYouSureYouWantToDeleteTheSelectedRoom"); }
        }
        public static string ConfirmDeletion
        {
            get { return GetString("ConfirmDeletion"); }
        }
        public static string AreYouSureYouWantToDeleteTheSelectedSeating
        {
            get { return GetString("AreYouSureYouWantToDeleteTheSelectedSeating"); }
        }
        public static string DineIn
        {
            get { return GetString("Dine-In"); }
        }
        public static string DriveThru
        {
            get { return GetString("DriveThru"); }
        }
        public static string Delivery
        {
            get { return GetString("Delivery"); }
        }
        public static string Carryout
        {
            get { return GetString("Carryout"); }
        }
        public static string Catering
        {
            get { return GetString("Catering"); }
        }
        public static string OccasionSelectionIsCurrentlyDisabledDoYouWantToEnableIt
        {
            get { return GetString("OccasionSelectionIsCurrentlyDisabledDoYouWantToEnableIt"); }
        }
        public static string EnableOccasionSelection
        {
            get { return GetString("EnableOccasionSelection"); }
        }
        public static string OccasionSelectionWasDisabledBecauseNoRoomsExist
        {
            get { return GetString("OccasionSelectionWasDisabledBecauseNoRoomsExist"); }
        }
        public static string OccasionSelectionDisabled
        {
            get { return GetString("OccasionSelectionDisabled"); }
        }
        #endregion

        #region SeatingRoomEditorControl
        public static string TicketType
        {
            get { return GetString("TicketType"); }
        }
        public static string UnsupportedTickettype
        {
            get { return GetString("UnsupportedTickettype"); }
        }
        #endregion

        #region SeatingSelectionControl
        public static string SelectOccasion
        {
            get { return GetString("SelectOccasion"); }
        }
        public static string ChangeOccasion
        {
            get { return GetString("ChangeOccasion"); }
        }
        public static string OccasionSelection
        {
            get { return GetString("OccasionSelection"); }
        }
        #endregion

        #region StartupWindow
        public static string Tempos
        {
            get { return GetString("Tempos"); }
        }
        public static string Version10
        {
            get { return GetString("Version10"); }
        }
        public static string Build
        {
            get { return GetString("Build"); }
        }
        public static string Version
        {
            get { return GetString("Version"); }
        }
        public static string Revision
        {
            get { return GetString("Revision"); }
        }
        public static string StartingApplication
        {
            get { return GetString("StartingApplication"); }
        }
        public static string ExitingApplicationDueToAnUnhandledException
        {
            get { return GetString("ExitingApplicationDueToAnUnhandledException"); }
        }
        public static string UnhandledException
        {
            get { return GetString("UnhandledException"); }
        }
        public static string LocalsettingEditor
        {
            get { return GetString("LocalsettingEditor"); }
        }
        public static string YouHaveNotEnteredAValidCompanyNameAndorSerialNumber
        {
            get { return GetString("YouHaveNotEnteredAValidCompanyNameAndorSerialNumber"); }
        }
        public static string CanNotConnectToTheTemposUpdateServerToVerifyYourSerialNumberPleaseCheckYourInternetConnectionIfYouAreConnectedToTheInternetPleaseTryAgainLaterTheUpdateServerIsDownForMaintenance
        {
            get { return GetString("CanNotConnectToTheTemposUpdateServerToVerifyYourSerialNumberPleaseCheckYourInternetConnectionIfYouAreConnectedToTheInternetPleaseTryAgainLaterTheUpdateServerIsDownForMaintenance"); }
        }
        public static string ConnectionFailed
        {
            get { return GetString("ConnectionFailed"); }
        }
        public static string TheSQLServiceAndSQLBrowserServiceAreNotRunningWouldYouLikeToStartThem
        {
            get { return GetString("TheSQLServiceAndSQLBrowserServiceAreNotRunningWouldYouLikeToStartThem"); }
        }
        public static string TheSQLServiceIsNotRunningWouldYouLikeToStartIt
        {
            get { return GetString("TheSQLServiceIsNotRunningWouldYouLikeToStartIt"); }
        }
        public static string TheSQLBrowserServiceIsNotRunningWouldYouLikeToStartIt
        {
            get { return GetString("TheSQLBrowserServiceIsNotRunningWouldYouLikeToStartIt"); }
        }
        public static string StartingSQLServices
        {
            get { return GetString("StartingSQLServices"); }
        }
        public static string CouldNotStartTheSQLServiceWouldYouLikeToContinue
        {
            get { return GetString("CouldNotStartTheSQLServiceWouldYouLikeToContinue"); }
        }
        public static string DatabaseConnectionTimeout
        {
            get { return GetString("DatabaseConnectionTimeout"); }
        }
        public static string InstallingSQLDatabase
        {
            get { return GetString("InstallingSQLDatabase"); }
        }
        public static string Information
        {
            get { return GetString("Information"); }
        }
        public static string TheDatabaseDesignCurrentlyBeingUsedIsIncorrectForThisVersionOfTempos
        {
            get { return GetString("TheDatabaseDesignCurrentlyBeingUsedIsIncorrectForThisVersionOfTempos"); }
        }
        public static string StartupError
        {
            get { return GetString("StartupError"); }
        }
        #endregion

        #region SystemSettingsEditorControl
        public static string Exit
        {
            get { return GetString("Exit"); }
        }
        public static string CompanyName
        {
            get { return GetString("CompanyName"); }
        }
        public static string SerialNumber
        {
            get { return GetString("SerialNumber"); }
        }
        public static string DatabaseServer
        {
            get { return GetString("DatabaseServer"); }
        }
        public static string DatabaseLogin
        {
            get { return GetString("DatabaseLogin"); }
        }
        public static string DatabasePassword
        {
            get { return GetString("DatabasePassword"); }
        }
        public static string DatabaseName
        {
            get { return GetString("DatabaseName"); }
        }
        public static string SystemSettings
        {
            get { return GetString("SystemSettings"); }
        }
        #endregion

        #region TaxEditorControl
        public static string TaxDescription
        {
            get { return GetString("TaxDescription"); }
        }
        public static string TaxPercentage
        {
            get { return GetString("TaxPercentage"); }
        }
        public static string PleaseEnterAValidPercentageValue
        {
            get { return GetString("PleaseEnterAValidPercentageValue"); }
        }
        public static string EmptyString
        {
            get { return GetString("EmptyString"); }
        }
        public static string PleaseEnterAValidDescription
        {
            get { return GetString("PleaseEnterAValidDescription"); }
        }
        #endregion

        #region TicketCashoutControl
        public static string TaxExemption
        {
            get { return GetString("TaxExemption"); }
        }
        public static string PrintReceipt
        {
            get { return GetString("PrintReceipt"); }
        }
        public static string TicketDiscounts
        {
            get { return GetString("TicketDiscounts"); }
        }
        public static string TicketCoupons
        {
            get { return GetString("TicketCoupons"); }
        }
        public static string DisabledInDemoVersion
        {
            get { return GetString("DisabledInDemoVersion"); }
        }
        public static string CashOutTicket
        {
            get { return GetString("CashOutTicket"); }
        }
        #endregion

        #region TicketCashoutPaymentControl
        public static string Cash
        {
            get { return GetString("Cash"); }
        }
        public static string Check
        {
            get { return GetString("Check"); }
        }
        public static string CreditCard
        {
            get { return GetString("CreditCard"); }
        }
        public static string GiftCard
        {
            get { return GetString("GiftCard"); }
        }
        public static string SubTotal
        {
            get { return GetString("SubTotal"); }
        }
        public static string AmountPayed
        {
            get { return GetString("AmountPayed"); }
        }
        public static string AmountDue
        {
            get { return GetString("AmountDue"); }
        }
        public static string ChangeDue
        {
            get { return GetString("ChangeDue"); }
        }
        #endregion

        #region TicketCouponControl
        public static string ApplyCoupon
        {
            get { return GetString("ApplyCoupon"); }
        }
        public static string ClearSelectedCoupon
        {
            get { return GetString("ClearSelectedCoupon"); }
        }
        public static string AvailableCoupons
        {
            get { return GetString("AvailableCoupons"); }
        }
        public static string AppliedCoupons
        {
            get { return GetString("AppliedCoupons"); }
        }
        public static string YouNeedToSelectATicketItemToUseThisCouponOn
        {
            get { return GetString("YouNeedToSelectATicketItemToUseThisCouponOn"); }
        }
        public static string ThatCouponCanNotBeAppliedToAnyItemsOnThisTicket
        {
            get { return GetString("ThatCouponCanNotBeAppliedToAnyItemsOnThisTicket"); }
        }
        #endregion

        #region TicketDeliveryDispatchControl
        public static string DispatchDriver
        {
            get { return GetString("DispatchDriver"); }
        }
        public static string Drivers
        {
            get { return GetString("Drivers"); }
        }
        public static string Deliveries
        {
            get { return GetString("Deliveries"); }
        }
        public static string OrderNumber
        {
            get { return GetString("OrderNumber"); }
        }
        public static string DeliveryDriverDispatch
        {
            get { return GetString("DeliveryDriverDispatch"); }
        }
        #endregion

        #region TicketDiscountControl
        public static string ApplyDiscount
        {
            get { return GetString("ApplyDiscount"); }
        }
        public static string ClearSelectedDiscount
        {
            get { return GetString("ClearSelectedDiscount"); }
        }
        public static string AvailableDiscounts
        {
            get { return GetString("AvailableDiscounts"); }
        }
        public static string AppliedDiscounts
        {
            get { return GetString("AppliedDiscounts"); }
        }
        public static string YouDoNotHavePermissionToApplyThisDiscount
        {
            get { return GetString("YouDoNotHavePermissionToApplyThisDiscount"); }
        }
        public static string EnterDiscountPercentage
        {
            get { return GetString("EnterDiscountPercentage"); }
        }
        public static string EnterDiscountAmount
        {
            get { return GetString("EnterDiscountAmount"); }
        }
        #endregion

        #region TimesheetEditorControl
        public static string DeleteRecord
        {
            get { return GetString("DeleteRecord"); }
        }
        public static string SaveChange
        {
            get { return GetString("SaveChange"); }
        }
        public static string Job
        {
            get { return GetString("Job"); }
        }
        public static string DeclaredTips
        {
            get { return GetString("DeclaredTips"); }
        }
        public static string DriverCompensation
        {
            get { return GetString("DriverCompensation"); }
        }
        public static string YouCanNotSetAStartTimeThatOccursAfterTheEndTime
        {
            get { return GetString("YouCanNotSetAStartTimeThatOccursAfterTheEndTime"); }
        }
        public static string ValidationError
        {
            get { return GetString("ValidationError"); }
        }
        public static string YouCanNotSetAnEndTimeThatOccursBeforeTheStartTime
        {
            get { return GetString("YouCanNotSetAnEndTimeThatOccursBeforeTheStartTime"); }
        }
        public static string DeclareTips
        {
            get { return GetString("DeclareTips"); }
        }
        public static string AreYouSureYouWantToDeleteThisRecordThisActionCanNotBeUndone
        {
            get { return GetString("AreYouSureYouWantToDeleteThisRecordThisActionCanNotBeUndone"); }
        }
        public static string Confirm
        {
            get { return GetString("Confirm"); }
        }
        public static string TheTimesSpecifiedWouldOverlapAnExistingShift
        {
            get { return GetString("TheTimesSpecifiedWouldOverlapAnExistingShift"); }
        }
        public static string EditEntry
        {
            get { return GetString("EditEntry"); }
        }
        #endregion

        #region TimesheetMaintenanceControl
        public static string ThisRecordIsLockedAndCanNotBeChanged
        {
            get { return GetString("ThisRecordIsLockedAndCanNotBeChanged"); }
        }
        public static string EntryLocked
        {
            get { return GetString("EntryLocked"); }
        }
        public static string ThisRecordIsYourCurrentClockInAndCanNotBeChangedUntilYouClockedOut
        {
            get { return GetString("ThisRecordIsYourCurrentClockInAndCanNotBeChangedUntilYouClockedOut"); }
        }
        public static string EmployeeNotClockedOut
        {
            get { return GetString("EmployeeNotClockedOut"); }
        }
        public static string ThisEmployeeMustClockOutBeforeYouCanEditWouldYouLikeToClockOutThisEmployeeRightNow
        {
            get { return GetString("ThisEmployeeMustClockOutBeforeYouCanEditWouldYouLikeToClockOutThisEmployeeRightNow"); }
        }
        public static string TimesheetMaintenance
        {
            get { return GetString("TimesheetMaintenance"); }
        }
        #endregion

        #region DeliveryMaintenanceControl
        public static string TODODeliveryMaintenanceControl
        {
            get { return GetString("TODODeliveryMaintenanceControl"); }
        }
        #endregion

        #region DeviceChoiceControl
        public static string DisabledInDemo
        {
            get { return GetString("DisabledInDemo"); }
        }
        public static string POSPrinter
        {
            get { return GetString("POSPrinter"); }
        }
        public static string PrintOutput
        {
            get { return GetString("PrintOutput"); }
        }
        public static string CashDrawer
        {
            get { return GetString("CashDrawer"); }
        }
        public static string Scanner
        {
            get { return GetString("Scanner"); }
        }
        public static string CoinDispenser
        {
            get { return GetString("CoinDispenser"); }
        }
        public static string BumpBar
        {
            get { return GetString("BumpBar"); }
        }
        #endregion

        #region EmployeeScheduleEditorControl
        public static string TODOScheduleEditorControl
        {
            get { return GetString("TODOScheduleEditorControl"); }
        }
        #endregion

        #region PersonalSettingsControl
        public static string Debug
        {
            get { return GetString("Debug"); }
        }
        public static string ShowMyOpen
        {
            get { return GetString("ShowMyOpen"); }
        }
        public static string ShowAllOpen
        {
            get { return GetString("ShowAllOpen"); }
        }
        public static string ShowFutureOrders
        {
            get { return GetString("ShowFutureOrders"); }
        }
        public static string ShowClosed
        {
            get { return GetString("ShowClosed"); }
        }
        public static string ShowAllDay
        {
            get { return GetString("ShowAllDay"); }
        }
        public static string ShowRange
        {
            get { return GetString("ShowRange"); }
        }
        public static string ShowAll
        {
            get { return GetString("ShowAll"); }
        }
        public static string DefaultTicketFilter
        {
            get { return GetString("DefaultTicketFilter"); }
        }
        #endregion

        #region PrintOptionEditorControl
        public static string AddLocation
        {
            get { return GetString("AddLocation"); }
        }
        public static string EditLocation
        {
            get { return GetString("EditLocation"); }
        }
        public static string RemoveLocation
        {
            get { return GetString("RemoveLocation"); }
        }
        public static string PrinterGroupName
        {
            get { return GetString("PrinterGroupName"); }
        }
        public static string PrintLocations
        {
            get { return GetString("PrintLocations"); }
        }
        #endregion

        #region SeatingEditorControl
        public static string Capacity
        {
            get { return GetString("Capacity"); }
        }
        #endregion

        #region SeatingPersonInformationControl
        public static string StartTicket
        {
            get { return GetString("StartTicket"); }
        }
        public static string Occasion
        {
            get { return GetString("Occasion"); }
        }
        public static string PhoneNumber
        {
            get { return GetString("PhoneNumber"); }
        }
        public static string StreetAddressLine1
        {
            get { return GetString("StreetAddressLine1"); }
        }
        public static string StreetAddressLine2
        {
            get { return GetString("StreetAddressLine2"); }
        }
        public static string Zipcode
        {
            get { return GetString("Zipcode"); }
        }
        public static string CityState
        {
            get { return GetString("CityState"); }
        }
        #endregion

        #region TaxMaintenanceControl
        public static string NewTax
        {
            get { return GetString("NewTax"); }
        }
        public static string UpdateTax
        {
            get { return GetString("UpdateTax"); }
        }
        public static string Taxes
        {
            get { return GetString("Taxes"); }
        }
        public static string TaxProperties
        {
            get { return GetString("TaxProperties"); }
        }
        #endregion

        #region TicketFilterControl
        public static string ShowCanceled
        {
            get { return GetString("ShowCanceled"); }
        }
        public static string ShowDispatched
        {
            get { return GetString("ShowDispatched"); }
        }
        #endregion

        #region TicketTypeFilterControl
        public static string NoneDisplayAll
        {
            get { return GetString("NoneDisplayAll"); }
        }
        #endregion

        #region GeneralSettingsControl
        public static string GeneralSettings
        {
            get { return GetString("GeneralSettings"); }
        }
        public static string Brushes
        {
            get { return GetString("Brushes"); }
        }
        public static string SoftwareUpdates
        {
            get { return GetString("SoftwareUpdates"); }
        }
        #endregion

        #region IngredientEditorControl
        public static string IngredientDetails
        {
            get { return GetString("IngredientDetails"); }
        }
        public static string IngredientPreparation
        {
            get { return GetString("IngredientPreparation"); }
        }
        #endregion

        #region EmployeeScheduleMaintenanceControl
        public static string New
        {
            get { return GetString("New"); }
        }
        #endregion

        #region OrderEntryChangeEmployeeControl
        public static string ChangeOwner
        {
            get { return GetString("ChangeOwner"); }
        }
        #endregion

        #region PrintOptionMaintenanceControl
        public static string NewItem
        {
            get { return GetString("NewItem"); }
        }
        public static string UpdateItem
        {
            get { return GetString("UpdateItem"); }
        }
        public static string CloseWindow
        {
            get { return GetString("CloseWindow"); }
        }
        #endregion
    }
}
