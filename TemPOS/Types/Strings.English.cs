using System.Collections.Generic;
using System.Reflection;

namespace TemPOS.Types
{
    [Obfuscation(Exclude = true)]
    public partial class Strings
    {
        public static readonly Dictionary<string, string> English = new Dictionary<string, string>();

        public static void InitializeEnglish()
        {
            #region General
            English.Add("Warning", "Warning");
            English.Add("Error", "Error");
            English.Add("Notification", "Notification");
            English.Add("DeleteConfirmation", "Delete Confirmation");
            English.Add("Help", "Help");
            English.Add("Exception", "Exception");
            English.Add("Yes", "Yes");
            English.Add("No", "No");
            English.Add("All", "All");
            English.Add("None", "None");
            English.Add("Set", "Set");
            English.Add("Add", "Add");
            English.Add("Remove", "Remove");
            English.Add("Increase", "Increase");
            English.Add("Decrease", "Decrease");
            English.Add("PermissionDenied", "Permission Denied");
            #endregion

            #region Common
            English.Add("Name", "Name");
            English.Add("Update", "Update");
            English.Add("Description", "Description");
            English.Add("Amount", "Amount");
            English.Add("IsActive", "Is Active");
            English.Add("CancelChanges", "Cancel Changes");
            English.Add("SelectAll", "Select All");
            English.Add("Ticket", "Ticket");
            English.Add("TicketItem", "Ticket Item");
            English.Add("Confirmation", "Confirmation");
            English.Add("FirstName", "First Name");
            English.Add("LastName", "Last Name");
            English.Add("MiddleInitial", "Middle Initial");
            English.Add("AddressLine1", "Address (Line 1)");
            English.Add("AddressLine2", "Address (Line 2)");
            English.Add("City", "City");
            English.Add("State", "State");
            English.Add("PostalCode", "Zip Code");
            English.Add("EMailAddress", "E-Mail Address");
            English.Add("Password", "Password");
            English.Add("IsEnabled", "Is Enabled");
            English.Add("Enabled", "Enabled");
            English.Add("Port", "Port");
            English.Add("Save", "Save");
            English.Add("Open", "Open");
            English.Add("Closed", "Closed");
            English.Add("Employee", "Employee");
            English.Add("Canceled", "Canceled");
            English.Add("Print", "Print");

            // Ticket
            English.Add("LateCancel", "Late Cancel");
            English.Add("Void", "Void");
            English.Add("PrivilegedDiscounts", "Privileged Discounts");
            English.Add("DeliveryDriverDispatching", "Delivery Driver Dispatching");
            English.Add("ChangeOwnerEmployee", "Change Owner Employee");

            // Register
            English.Add("Cashout", "Cash Out");
            English.Add("StartUp", "Start-Up");
            English.Add("Payout", "Payout");
            English.Add("Drop", "Drop");
            English.Add("Deposit", "Deposit");
            English.Add("NoSale", "No Sale");
            English.Add("Refund", "Refund");
            English.Add("Return", "Return");
            English.Add("Report", "Report");
            English.Add("CloseOut", "Close-Out");
            English.Add("DeliveryDriverBankrolling", "Delivery Driver Bankrolling");
            English.Add("UseAnyRegisterDrawer", "Use Any Register Drawer");

            // Manager
            English.Add("ReportsMenu", "Reports Menu");
            English.Add("StartOfDay", "Start-Of-Day");
            English.Add("EndOfDay", "End-Of-Day");
            English.Add("ManagerAlerts", "Manager Alerts");
            English.Add("SystemSetup", "System Setup");
            English.Add("EmployeeSetup", "Employee Setup");
            English.Add("EmployeeScheduling", "Employee Scheduling (Future)");
            English.Add("EmployeeTimekeeping", "Employee Timekeeping");
            English.Add("CustomerSetup", "Customer Setup (Future)");
            English.Add("VendorSetup", "Vendor Setup (Future)");
            English.Add("OverrideDeliveryRestrictions", "Override Delivery Restrictions (Future)");
            English.Add("AdministrativeCommandConsole", "Administrative Command Console");
            English.Add("ExitProgram", "Exit Program");

            #endregion

            #region Languages
            English.Add("LanguageLanguage", "Language");
            English.Add("LanguageEnglish", "English");
            English.Add("LanguageSpanish", "Spanish");
            English.Add("LanguageFrench", "French");
            English.Add("LanguageItalian", "Italian");
            English.Add("LanguageGerman", "German");
            English.Add("LanguageDutch", "Dutch");
            English.Add("LanguagePortuguese", "Portuguese");
            #endregion

            #region ChangePasswordControl
            English.Add("PasswordIncorrect", "Your old password is not correct.");
            English.Add("NewPasswordsMismatch", "Your new passwords do not match.");
            English.Add("NewPasswordToShort", "You new password must be at least five characters in length.");
            English.Add("ChangePasswordWindowTitle", "Change Password");
            English.Add("ChangePasswordOldPassword", "Old Password");
            English.Add("ChangePasswordNewPassword1", "New Password");
            English.Add("ChangePasswordNewPassword2", "New Password (Retype)");
            #endregion

            #region Command Shell Strings
            English.Add("ShellWindowTitle", "Command Console");
            English.Add("ShellResetSystem", "Are you sure you want to reset the entire system and exit the application?");
            English.Add("ShellResetSystemTitle", "Confirm System Reset");
            English.Add("ShellDeleteLog", "Are you sure you want to delete the log file?");
            English.Add("ShellNoLogFileExists", "No log file exists");
            English.Add("ShellHelpClearLine1", "Clears the console");
            English.Add("ShellHelpClearLine2", "Command: clear");
            English.Add("ShellHelpLogLine1", "Delete or show the log file");
            English.Add("ShellHelpLogLine2", "Command: log [delete | show]");
            English.Add("ShellHelpSqlQueryLine1", "Execute a SQL statement that returns a results set");
            English.Add("ShellHelpSqlQueryLine2", "Command: sqlquery [SQL STATEMENT]");
            English.Add("ShellHelpSqlNonQueryLine1", "Execute a SQL statement without returning a results set");
            English.Add("ShellHelpSqlNonQueryLine2", "Command: sqlnonquery [SQL STATEMENT]");
            English.Add("ShellSqlNonQueryResultSuccess", "Command executed successfully");
            English.Add("ShellSqlNonQueryResultFailed", "Command failed to execute");
            English.Add("ShellHelpStoreSettingLine1", "Sets or gets a store setting value");
            English.Add("ShellHelpStoreSettingLine2", "Command: storesetting [NAME] [DATA_TYPE] [VALUE]");
            English.Add("ShellHelpStoreSettingLine3", "Data Types: string (default), int, double, datetime");
            English.Add("ShellStoreSettingNotSet", "No value set for ");
            English.Add("ShellSeatingStatusOn", "Occasion selection is on.");
            English.Add("ShellSeatingStatusOff", "Occasion selection is off.");
            English.Add("ShellNoPrintersFound", "No printers found");
            English.Add("ShellKeyboardLockStatusOn", "Keyboard lock is on.");
            English.Add("ShellKeyboardLockStatusOff", "Keyboard lock is off.");
            English.Add("ShellHelpUsage", "Usage: ");
            #endregion

            #region CancelMadeUnmadeControl
            English.Add("CancelControlReopen", "Reopen");
            English.Add("CancelControlCancelMade", "Cancel    Made");
            English.Add("CancelControlCancelUnmade", "Cancel Unmade");
            English.Add("CancelControlVoid", "Void");
            English.Add("CancelControlDontCancel", "Don't     Cancel");
            English.Add("CancelControlDontRefund", "Cancel Refund");
            #endregion

            #region CategoryEditorControl
            English.Add("CategoryEditorDisplayIndex", "Display Index");
            English.Add("CategoryEditorDoNotDisplay", "Do not display");
            English.Add("CategoryEditorNameInvalid", "Please enter a valid name");
            English.Add("CategoryEditorDisplayIndexInvalid", "Please select a display index value");
            #endregion

            #region CouponCategorySelectionControl
            English.Add("CategoryAllCategories", "All Categories");
            English.Add("CategorySelectedCategories", "Selected Categories");
            English.Add("CategoryAddCategory", "Add Category");
            English.Add("CategoryRemoveCategory", "Remove Category");
            #endregion

            #region CouponEditorDetailsControl
            English.Add("CouponEditorAmountAsPercentage", "Specify Amount as Percentage");
            English.Add("CouponEditorMatching", "Item and Category Matching");
            English.Add("CouponEditorMatchAll", "Apply To All Matching Items on Ticket");
            English.Add("CouponEditorThirdPartyCompensation", "Third Party Compensation");
            English.Add("CouponEditorCouponValueLimit", "Coupon Value Limit");
            English.Add("CouponEditorLimitPerTicket", "Limit Number Per Ticket");
            English.Add("CouponEditorCategory", "Category: ");
            English.Add("CouponEditorExcludeAllExceptFor", "Exclude all, except for...");
            English.Add("CouponEditorIncludeAllExceptFor", "Include all, except for...");
            English.Add("CouponEditorInvlaidLimitPerTicket", "Please enter a valid limit per ticket, or leave the field empty.");
            English.Add("CouponEditorInvalidAmountLimit","Please enter a valid coupon amount limit, or leave the field empty.");
            English.Add("CouponEditorInvalidName", "Please enter a valid name.");
            English.Add("CouponEditorInvalidAmount", "Please enter a valid amount.");
            English.Add("CouponEditorInvalidType", "Please select if this is a fixed or percentage amount.");
            #endregion

            #region CouponItemSelectionControl
            English.Add("ItemAllItems", "All Items");
            English.Add("ItemSelectedItems", "Selected Items");
            English.Add("ItemAddItem", "Add       Item");
            English.Add("ItemRemoveItem", "Remove Item");
            #endregion

            #region CouponMaintenanceControl
            English.Add("CouponProperties", "Coupon Properties");
            English.Add("AreYouSureYouWantToDeleteTheSelectedCoupon", "Are you sure you want to delete the selected coupon?");
            English.Add("CouponSetupNewCoupon", "New Coupon");
            English.Add("CouponSetupDeleteCoupon", "Delete Coupon");
            English.Add("CouponSetupUpdateCoupon", "Update Coupon");
            #endregion

            #region DayOfOperationRangeSelectionControl
            English.Add("SelectedRange", "Selected Range");
            English.Add("DayOfOperationStartingDay", "Starting Day");
            English.Add("DayOfOperationEndingDay", "Ending Day");
            English.Add("DayOfOperationSelectSpecifiedDays", "Select Specified Days");
            English.Add("DayOfOperationSelectThisYear", "Select This Year");
            #endregion

            #region DeviceSelectionControl
            English.Add("DeviceSelectionTab1", "Bump Bars");
            English.Add("DeviceSelectionTab2", "Card Scanners");
            English.Add("DeviceSelectionTab3", "Cash Drawers");
            English.Add("DeviceSelectionTab4", "Coin Despenser");
            English.Add("DeviceSelectionTab5", "Receipt Printers");
            #endregion

            #region DiscountEditorControl
            English.Add("DiscountEditorAmountAsPercentage", "Specify Amount as Percentage");
            English.Add("DiscountEditorApplyTo", "Apply To");
            English.Add("DiscountEditorRequiresPermission", "Requires Discount Security Permission");
            English.Add("DiscountEditorPromptForAmount", "Prompt for Amount");
            #endregion

            #region DiscountMaintenanceControl
            English.Add("Discounts", "Discounts");
            English.Add("DiscountProperties", "Discount Properties");
            English.Add("AreYouSureYouWantToDeleteTheSelectedDiscount", "Are you sure you want to delete the selected discount?");
            English.Add("DiscountEditorAddDiscount", "New Discount");
            English.Add("DiscountEditorDeleteDiscount", "Delete Discount");
            English.Add("DiscountEditorUpdateDiscount", "Update Discount");
            #endregion

            #region EmployeeClockInControl and EmployeeClockOutControl
            English.Add("ClockInJobs", "Jobs");
            English.Add("ClockIn", "Clock-In");
            English.Add("ClockOutDeclareTips", "Declare Tips");
            English.Add("ClockOut", "Clock-Out");
            English.Add("ClockOutTips", "Tips ");
            #endregion

            #region EmployeeEditorControl
            English.Add("Employees", "Employees");
            English.Add("EmployeeProperties", "Employee Properties");
            English.Add("EmployeeEditorAddEmployee", "Add Employee");
            English.Add("EmployeeEditorEditJobs", "Edit Job Choices");
            English.Add("EmployeeEditorTerminateEmployee", "Terminate Employee");
            English.Add("EmployeeEditorRemoveEmployee", "Remove Employee");
            English.Add("EmployeeEditorUpdateEmployee", "Update Employee");
            English.Add("EmployeeEditorRehireEmployee", "Rehire Employee");
            English.Add("EmployeeEditorConfirmRehire", "Are you sure you want to rehire the selected employee?");
            English.Add("EmployeeEditorCantTerminateSelf", "You can not terminate yourself!");
            English.Add("EmployeeEditorConfirmTerminate", "Are you sure you want to terminate the selected employee?");
            English.Add("EmployeeEditorTerminateFirst", "An employee must be terminated before being removed");
            English.Add("EmployeeEditorConfirmRemove", "Are you sure you want to remove the selected employee?");
            English.Add("EmployeeEditorPasswordWarning", "You have not setup a password for this employee, they will not be able to login.");
            #endregion

            #region EmployeeEditorDetailsControl
            English.Add("TheZipCode", "The Zip Code \"");
            English.Add("WasNotFoundInTheDatabaseDoYouWantToAddItToTheDatabase", "\" was not found in the database. Do you want to add it to the database?");
            English.Add("InvalidZipCode", "Invalid Zip Code");
            English.Add("DoesntMatchTheCitystateInformationInTheDatabaseDoYouWantToUpdateTheDatabase", "\" doesn't match the city/state information in the database. Do you want to update the database?");
            English.Add("CityZipCodeMismatch", "City-Zip Code Mismatch");
            English.Add("EnterAValidValueForFirstName", "Enter a valid value for first name.");
            English.Add("InvalidName", "Invalid Name");
            English.Add("EnterAValidValueForLastName", "Enter a valid value for last name.");
            English.Add("EnterAValidValueForCity", "Enter a valid value for city.");
            English.Add("InvalidCity", "Invalid City");
            English.Add("EnterAValidValueForState", "Enter a valid value for state.");
            English.Add("InvalidState", "Invalid State");
            English.Add("EnterAValidValueForZipCode", "Enter a valid value for zip code.");
            English.Add("TheCity", "The city \"");
            English.Add("TheState", "The state \"");
            English.Add("EmployeeEditorTaxId", "Federal Tax ID (Social Security #)");
            English.Add("EmployeeEditorTicketPermissions", "Ticket Permissions");
            English.Add("EmployeeEditorRegisterPermissions", "Register Permissions");
            English.Add("EmployeeEditorManagerPermissions", "Manager Permissions");
            English.Add("EmployeeEditorPhoneNumbers", "Phone Numbers");
            #endregion

            #region EmployeeJobEditorControl
            English.Add("EmployeeEditorCanDeclareTips", "Can Declare Tips");
            English.Add("EmployeeEditorCanTakeDeliveries", "Can Take Deliveries");
            #endregion

            #region EmployeeJobMaintenanceControl
            English.Add("EmployeeJobs", "Employee Jobs");
            English.Add("EmployeeJobProperties", "Employee Job Properties");
            English.Add("EmployeeJobEditorNewJob", "New     Job");
            English.Add("EmployeeJobEditorUpdateJob", "Update    Job");
            #endregion

            #region EmployeeJobSelectionControl
            English.Add("AvaliableJobs", "Avaliable Jobs");
            English.Add("SelectedJobs", "Selected Jobs");
            English.Add("EmployeeJobEditorAddJob", "Add   Job");
            English.Add("EmployeeJobEditorEditPayRate", "Edit Pay Rate");
            English.Add("EmployeeJobEditorRemoveJob", "Remove  Job");
            English.Add("EmployeeJobEditorPayRate", "Pay Rate");
            English.Add("EmployeeJobEditorSelectJobsFor", "Select Jobs for");
            #endregion

            #region ExitControl
            English.Add("ExitLockWorkstation", "Lock Workstation");
            English.Add("ExitLogoffWindows", "Logoff Windows");
            English.Add("ExitShutdownWindows", "Shutdown Windows");
            English.Add("ExitRestartWindows", "Restart Windows");
            English.Add("ExitHibernate", "Hibernate");
            English.Add("ExitSuspend", "Suspend");
            English.Add("ExitRestartProgram", "Restart Program");
            English.Add("ExitExitProgramAndSql", "Exit  Program   and SQL");
            English.Add("ExitExitProgram", "Exit  Program");
            English.Add("ExitStoppingSql", "Stopping SQL Services...");
            English.Add("ExitExitTemPos", "Exit TemPOS");
            #endregion

            #region FutureTimeEditControl
            English.Add("FutureTimeSetTime", "Set Time");
            English.Add("FutureTimeMakeNow", "Make Now");
            English.Add("FutureTimeTooEarly", "You can not set a future time any earlier than today at ");
            English.Add("FutureTimeTooEarlyError", "Too Early Error");
            English.Add("FutureTime", "Future Time");
            #endregion

            #region GeneralSettingsBrushSetupControl
            English.Add("BrushesApplication", "Application");
            English.Add("BrushesBordersEnabled", "Borders - Enabled");
            English.Add("BrushesBordersDisabled", "Borders - Disabled");
            English.Add("BrushesButtonEnabled", "Button - Enabled");
            English.Add("BrushesButtonDisabled", "Button - Disabled");
            English.Add("BrushesButtonEnabledSelected", "Button - Enabled & Selected");
            English.Add("BrushesButtonDisabledSelected", "Button - Disabled & Selected");
            English.Add("BrushesCaret", "Caret");
            English.Add("BrushesCheckBoxEnabled", "CheckBox - Enabled");
            English.Add("BrushesCheckBoxDisabled", "CheckBox - Disabled");
            English.Add("BrushesCheckBoxEnabledSelected", "CheckBox - Enabled & Selected");
            English.Add("BrushesCheckBoxDisabledSelected", "CheckBox - Disabled & Selected");
            English.Add("BrushesComboBoxEnabled", "ComboBox - Enabled");
            English.Add("BrushesComboBoxDisabled", "ComboBox - Disabled");
            English.Add("BrushesLabelEnabled", "Label - Enabled");
            English.Add("BrushesLabelDisabled", "Label - Disabled");
            English.Add("BrushesListItemEnabled", "List Item - Enabled");
            English.Add("BrushesListItemDisabled", "List Item - Disabled");
            English.Add("BrushesListItemEnabledSelected", "List Item - Enabled & Selected");
            English.Add("BrushesListItemDisabledSelected", "List Item - Disabled & Selected");
            English.Add("BrushesRadioButtonEnabled", "RadioButton - Enabled");
            English.Add("BrushesRadioButtonDisabled", "RadioButton - Disabled");
            English.Add("BrushesRadioButtonEnabledSelected", "RadioButton - Enabled & Selected");
            English.Add("BrushesRadioButtonDisabledSelected", "RadioButton - Disabled & Selected");
            English.Add("BrushesTabButtonEnabled", "Tab Button - Enabled");
            English.Add("BrushesTabButtonDisabled", "Tab Button - Disabled");
            English.Add("BrushesTabButtonEnabledSelected", "Tab Button - Enabled & Selected");
            English.Add("BrushesTabButtonDisabledSelected", "Tab Button - Disabled & Selected");
            English.Add("BrushesTextBoxEnabled", "TextBox - Enabled");
            English.Add("BrushesTextBoxDisabled", "TextBox - Disabled");
            English.Add("BrushesWindowTitleBar", "Window Title Bar");
            English.Add("BrushesForegroundColors", "Foreground Colors");
            English.Add("BrushesBackgroundColors", "Background Colors");
            #endregion

            #region GeneralSettingsGeneralPreferencesControl
            English.Add("BlockTaskManagerAccess", "Block Task Manager Access");
            English.Add("Test", "Test");
            English.Add("TestCompletedSuccessfully", "Test completed successfully");
            English.Add("TestPassed", "Test Passed");
            English.Add("ServerIsNotRunning", "Server is not running");
            English.Add("TestFailed", "Test Failed");
            English.Add("ClientIsNotRunning", "Client is not running");
            English.Add("DoYouWantToInstallTheTaskManagerAccessService", "Do you want to install the Task Manager Access service?");
            English.Add("InstallService", "Install Service");
            English.Add("DoYouWantToStartTheTaskManagerAccessService", "Do you want to start the Task Manager Access service?");
            English.Add("StartService", "Start Service");
            English.Add("SettingsClientMessageBroadcastServer", "Client Message Broadcast Server");
            English.Add("SettingsTest", "Test");
            English.Add("SettingsGeneralOptions", "General Options");
            English.Add("SettingsUseSeating", "Use seating");
            English.Add("SettingsForceWasteOnVoid", "Force waste on voids");
            English.Add("SettingsRestrictKeyboard", "Restrict keyboard");
            English.Add("SettingsWeatherConditions", "Weather Conditions");
            English.Add("SettingsPostalCode", "Zip Code");
            English.Add("SettingsAutoLogoutOptions", "Auto-Logout Options");
            English.Add("SettingsAutoLogoutTimeout", "Timeout (Seconds)");
            English.Add("SettingsAutoLogoutDisable", "Disable");
            English.Add("SettingsAutoLogoutDisableOrderEntry", "In order entry");
            English.Add("SettingsAutoLogoutDisableDialogs", "For dialog windows");
            English.Add("SettingsAutoLogoutDisablePasswordChange", "On password changes");
            English.Add("SettingsLocalSettings", "Local Settings");
            English.Add("SettingsKioskMode", "Kiosk Mode");
            English.Add("SettingsKioskModeWarning", "Changing kiosk mode will require you restart TemPOS for changes to take effect.");
            English.Add("SettingsLogoutOnPlaceOrder", "Logout on Place Order");
            #endregion

            #region GeneralSettingsUpdateControl
            English.Add("SettingsOptions", "Options");
            English.Add("SettingsAutoUpdate", "Auto-Update");
            English.Add("SettingsServer", "Server");
            English.Add("SettingsVersionCheck", "Version Check");
            English.Add("SettingsUpdateNow", "Update Now");
            English.Add("UpdateDownloadError", "Error: The downloaded update was corrupted. Update was canceled");
            English.Add("UpdateConnected", "Connected");
            English.Add("UpdateFailedToConnect", "Failed to connected to update server");
            English.Add("UpdateDisconnected", "Disconnected");
            English.Add("UpdateAuthenticated", "Authenticated");
            English.Add("UpdateNewestVersion", "Newest version is");
            English.Add("UpdateReceived", "Update Received");
            English.Add("UpdateTemposUpdater", "TemPOS Updater");
            #endregion

            #region IngredientAmountControl
            English.Add("MeasurementUnit", "Measurement Unit");
            English.Add("UnmeasuredUnits", "Unmeasured Units");
            English.Add("WeightPound", "Weight: Pound");
            English.Add("WeightOunce", "Weight: Ounce");
            English.Add("WeightGram", "Weight: Gram");
            English.Add("WeightMilligram", "Weight: Milligram");
            English.Add("WeightKilogram", "Weight: Kilogram");
            English.Add("VolumeGallon", "Volume: Gallon");
            English.Add("VolumeQuart", "Volume: Quart");
            English.Add("VolumePint", "Volume: Pint");
            English.Add("VolumeCup", "Volume: Cup");
            English.Add("VolumeTablespoon", "Volume: Tablespoon");
            English.Add("VolumeTeaspoon", "Volume: Teaspoon");
            English.Add("VolumeLiter", "Volume: Liter");
            English.Add("VolumeFluidOunce", "Volume: Fluid Ounce");
            English.Add("VolumeMilliliter", "Volume: Milliliter");
            English.Add("VolumeKiloliter", "Volume: Kiloliter");
            #endregion

            #region IngredientEditorDetailsControl
            English.Add("IngredientEditorIncreaseByAmount", "Increase By Amount");
            English.Add("IngredientEditorIncreaseByRecipe", "Increase By Recipe");
            English.Add("IngredientEditorDecreaseByAmount", "Decrease By Amount");
            English.Add("IngredientEditorDecreaseByRecipe", "Decrease By Recipe");
            English.Add("IngredientEditorNoYieldError", "You can not increase or decrease by recipe, until you have specified a recipe yield (located on the ingredient preparation tab)");
            English.Add("IngredientEditorConvert1", "Would you like to convert the inventory amount ");
            English.Add("IngredientEditorConvert2", "(and the recipe yield) ");
            English.Add("IngredientEditorConvert3", "from the old measurement units to the new measurement units?");
            English.Add("IngredientEditorUpdateInventory", "Update Inventory");
            English.Add("IngredientEditorPrintedName", "Printed Name");
            English.Add("IngredientEditorInventoryAmount", "Inventory Amount");
            English.Add("IngredientEditorMeasuringUnit", "Measuring Unit");
            English.Add("IngredientEditorCostPerUnit", "Cost Per Unit");
            English.Add("IngredientEditorParAmount", "PAR Amount");
            English.Add("IngredientEditorIncrease", "Increase");
            English.Add("IngredientEditorDecrease", "Decrease");
            #endregion

            #region IngredientEditorPreparationControl
            English.Add("PreparationCanNotBeModifiedDuringADayOfOperationCompleteAnEndOfDayToModify", "Preparation can not be modified during a day-of-operation. Complete an end-of-day to modify.");
            English.Add("S", "s");
            English.Add("IngredientEditorPrepared", "This is a prepared ingredient");
            English.Add("IngredientEditorAvailable", "Available Ingredients");
            English.Add("IngredientEditorCurrent", "Current Ingredients");
            English.Add("IngredientEditorRecipeYield", "Recipe Yield");
            English.Add("IngredientEditorUnits", "Units");
            English.Add("IngredientEditorAddIngredient", "Add Ingredient");
            English.Add("IngredientEditorEditIngredient", "Edit   Amount");
            English.Add("IngredientEditorRemoveIngredient", "Remove Ingredient");
            English.Add("IngredientEditorAmount", "Amount: ");
            English.Add("IngredientEditorWarningUncheck", "If you uncheck this option, the ingredients used to prepare this ingredient will no longer be associated with the preparation of this ingredient.");
            English.Add("IngredientEditorEditAmount", "Edit Ingredient Amount");
            English.Add("IngredientEditorConfirmRemove", "Are you sure you want remove the selected ingredient from the recipe?");
            English.Add("IngredientEditorEditRecipeYield", "Edit Recipe Yield");
            #endregion

            #region InventoryEditorControl
            English.Add("InventoryIncreaseByAmount", "Increase By Amount");
            English.Add("InventoryIncreaseByRecipe", "Increase By Recipe");
            English.Add("InventoryDecreaseByAmount", "Decrease By Amount");
            English.Add("InventoryDecreaseByRecipe", "Decrease By Recipe");
            English.Add("InventorySetAmount", "Set Inventory Amount");
            English.Add("InventoryError", "You can not increase or decrease by recipe, until you have specified a recipe yield (located on the ingredient preparation tab)");
            English.Add("InventoryEdit", "Edit Inventory");
            #endregion

            #region ItemEditorControl
            English.Add("ItemDetails", "Item Details");
            English.Add("ItemsInGroup", "Items In Group");
            English.Add("ItemIngredients", "Item Ingredients");
            English.Add("ItemOptions", "Item Options");
            English.Add("SpecialPricing", "Special Pricing");
            English.Add("ItemEditorErrorNoCategory", "You have not selected a category for this item.");
            English.Add("ItemEditorInvalidCategory", "Invalid Category");
            English.Add("ItemEditorErrorNoName", "Item name is blank.");
            English.Add("ItemEditorErrorExistingName", "That item name is already being used.");
            English.Add("ItemEditorInvalidName", "Invalid Name");
            #endregion

            #region ItemEditorDetailsControl
            English.Add("ItemEditorInvalidPrice", "Invalid Price");
            English.Add("ItemEditorInvalidTaxSetting", "Invalid Tax Setting");
            English.Add("ItemEditorCategory", "Category");
            English.Add("ItemEditorFullName", "Full Name");
            English.Add("ItemEditorButtonName", "POS Button Name");
            English.Add("ItemEditorPrice", "Price");
            English.Add("ItemEditorPrintDestination", "Automatic Print Destination");
            English.Add("ItemEditorTax", "Tax");
            English.Add("ItemEditorIsReturnable", "Is Returnable");
            English.Add("ItemEditorIsFired", "Is Fired");
            English.Add("ItemEditorIsTaxExemptable", "Is Tax Exemptable");
            English.Add("ItemEditorAvailableForSale", "Available For Sale");
            English.Add("ItemEditorIsOutOfStock", "Is Out-of-Stock");
            English.Add("ItemEditorIsGrouping", "Is Grouping");
            English.Add("ItemEditorJournal", "Journal");
            English.Add("ItemEditorReceipt", "Receipt");
            English.Add("ItemEditorKitchen1", "Kitchen 1");
            English.Add("ItemEditorKitchen2", "Kitchen 2");
            English.Add("ItemEditorKitchen3", "Kitchen 3");
            English.Add("ItemEditorBar1", "Bar 1");
            English.Add("ItemEditorBar2", "Bar 2");
            English.Add("ItemEditorBar3", "Bar 3");
            #endregion

            #region ItemEditorGroupingControl
            English.Add("ItemEditorQuantity", "Quantity: ");
            English.Add("ItemEditorErrorDemo", "You can only group two items in the demo version.");
            English.Add("ItemEditorDemoRestriction", "Demo Restriction");
            English.Add("ItemEditorEditQuantity", "Edit Quantity");
            English.Add("ItemEditorConfirmRemove", "Are you sure you want remove the selected item from the included items?");
            English.Add("ItemEditorAvailableItems", "Available Items");
            English.Add("ItemEditorIncludedItems", "Included Items");
            English.Add("ItemEditorErrorStartOfDay", "Item groupings can not be modified during a day-of-operation. Complete an end-of-day to modify.");
            English.Add("ItemEditorAddItem", "Add      Item");
            English.Add("ItemEditorEditQuantityButton", "Edit  Quantity");
            English.Add("ItemEditorRemoveItem", "Remove Item");
            #endregion

            #region ItemEditorIngredientsControl
            English.Add("ItemIngredientsCanNotBeModifiedDuringADayOfOperationCompleteAnEndOfDayToModify", "Item ingredients can not be modified during a day-of-operation. Complete an end-of-day to modify.");
            English.Add("ItemEditorAmount", "Amount: ");
            English.Add("ItemEditorIngredientAmount", "Item Ingredient Amount");
            English.Add("ItemEditorEditIngredient", "Edit Item Ingredient");
            English.Add("ItemEditorConfirmIngredientRemove", "Are you sure you want remove the selected ingredient from the recipe?");
            English.Add("ItemEditorAvailableIngredients", "Available Ingredients");
            English.Add("ItemEditorCurrentIngredients", "Current Ingredients");
            English.Add("ItemEditorAddIngredient", "Add Ingredient");
            English.Add("ItemEditorEditIngredientAmount", "Edit   Amount");
            English.Add("ItemEditorRemoveIngredient", "Remove Ingredient");
            #endregion

            #region ItemEditorOptionSetControl
            English.Add("ItemOptionsCanNotBeModifiedDuringADayOfOperationCompleteAnEndOfDayToModify", "Item options can not be modified during a day-of-operation. Complete an end-of-day to modify.");
            English.Add("ItemEditorOptionSet1", "Option Set 1");
            English.Add("ItemEditorOptionSet2", "Option Set 2");
            English.Add("ItemEditorOptionSet3", "Option Set 3");
            #endregion

            #region ItemEditorSpecialPricingControl
            English.Add("ItemEditorListboxText1", ", From: ");
            English.Add("ItemEditorListboxText2", " to ");
            English.Add("ItemEditorDay", "Day");
            English.Add("ItemEditorStartTime", "Start Time");
            English.Add("ItemEditorEndTime", "End Time");
            English.Add("ItemEditorMinDiscount", "Minimum For Discount");
            English.Add("ItemEditorMaxDiscount", "Maximum Discounted");
            English.Add("ItemEditorDiscountPrice", "Discount Price");
            #endregion

            #region ItemMaintenanceControl
            English.Add("ItemProperties", "Item Properties");
            English.Add("ItemOptionProperties", "Item Option Properties");
            English.Add("CategoryProperties", "Category Properties");
            English.Add("IngredientProperties", "Ingredient Properties");
            English.Add("Items", "Items");
            English.Add("Ingredients", "Ingredients");
            English.Add("Categories", "Categories");
            English.Add("ItemOptionSets", "Item Option Sets");
            English.Add("ItemSetupFind", "Find");
            English.Add("ItemSetupEditItemOptions", "Edit Item Options");
            English.Add("ItemSetupFindNext", "Find Next");
            English.Add("ItemSetupView", "View");
            English.Add("ItemSetupItems", "Items");
            English.Add("ItemSetupItemsInCategory", "Items in Category: \"");
            English.Add("ItemSetupCategories", "Categories");
            English.Add("ItemSetupIngredients", "Ingredients");
            English.Add("ItemSetupItemOptionSets", "Item Option Sets");
            English.Add("ItemSetup", "Item Setup");
            English.Add("ItemSetupUpdateItem", "Update   Item");
            English.Add("ItemSetupAddItem", "Add      Item");
            English.Add("ItemSetupDeleteItem", "Delete   Item");
            English.Add("ItemSetupUpdateCategory", "Update Category");
            English.Add("ItemSetupAddCategory", "Add Category");
            English.Add("ItemSetupDeleteCategory", "Delete Category");
            English.Add("ItemSetupUpdateIngredient", "Update Ingredient");
            English.Add("ItemSetupAddIngredient", "Add Ingredient");
            English.Add("ItemSetupUpdateOptionSet", "Update Option Set");
            English.Add("ItemSetupAddOptionSet", "Add     Option Set");
            English.Add("ItemSetupDeleteOptionSet", "Delete Option Set");
            English.Add("ItemSetupNotifyNoAdditialItemsFound", "No additional matches found.");
            English.Add("ItemSetupSearchItems", "Search Items");
            English.Add("ItemSetupNotifyNoItemsFound", "No items found containing");
            English.Add("ItemSetupConfirmDeleteOptionSet", "Are you sure you want to delete the selected item option set?");
            English.Add("ItemSetupConfirmDeleteCategory", "Are you sure you want to delete the selected category and all items in the category?");
            English.Add("ItemSetupConfirmDeleteItem", "Are you sure you want to delete the selected item?");
            English.Add("ItemSetupValidationError", "Validation Error");
            #endregion

            #region ItemMaintenanceViewContextMenuControl
            English.Add("ItemSetupAllCategories", "All Categories");
            English.Add("ItemSetupAllItemsInCategory", "All Items in Selected Category");
            English.Add("ItemSetupAllItems", "All Items");
            English.Add("ItemSetupAllItemOptionSets", "All Item Option Sets");
            English.Add("ItemSetupAllIngredients", "All Ingredients");
            #endregion

            #region ItemOptionEditorControl
            English.Add("Ingredient", "Ingredient");
            English.Add("ItemSetupItemRecursionError", "You can not use an item that is already using this item option");
            English.Add("ItemSetupIngredient", "Ingredient");
            English.Add("ItemSetupItem", "Item");
            English.Add("ItemSetupCostForExtra", "Cost For Extra");
            English.Add("ItemSetupUses", "Uses");
            English.Add("ItemSetupNothing", "Nothing");
            #endregion

            #region ItemOptionMaintenanceControl
            English.Add("ItemSetupConfirmDeleteItemOption", "Are you sure you want to delete the selected option?");
            English.Add("ItemSetupOptionsEdit", "Options Edit - ");
            English.Add("ItemSetupNewOption", "New  Option");
            English.Add("ItemSetupDeleteOption", "Delete Option");
            English.Add("ItemSetupUpdateOption", "Update Option");
            #endregion

            #region ItemOptionSetEditorControl
            English.Add("ItemSetupErrorSetNeedsName", "You must specifiy a name for this option set.");
            English.Add("ItemSetupItemSetName", "Item Set Name");
            English.Add("ItemSetupMinimumRequiredOptions", "Minimum Required Options");
            English.Add("ItemSetupNumberOfFreeOptions", "Number of Free Options");
            English.Add("ItemSetupMaxAllowedOptions", "Maximum Allowed Options");
            English.Add("ItemSetupCostPerExtraOption", "Cost Per Extra Option");
            English.Add("ItemSetupPizzaToppingStyle", "Pizza Topping Style");
            #endregion

            #region LoginControl
            English.Add("LoginScanIdCard", "Scan Identification Card");
            English.Add("Login", "Login");
            English.Add("LoginSingletonException", "Singleton Exception");
            English.Add("LoginAdminUserCreated", "Adminstrative User Created");
            English.Add("LoginAdminInfo1", "        User: ");
            English.Add("LoginAdminInfo2", "    Password: ");
            English.Add("LoginLoginIncorrect", "Login Incorrect");
            English.Add("LoginWarningJobs", "You have no job assignments. Please report this to your manager.");
            English.Add("LoginError", "Clock-In Warning");
            English.Add("LoginEnterLogin", "Enter Login");
            #endregion

            #region OrderEntryControl
            English.Add("NonOrderCommands", "Non-Order Commands");
            English.Add("OrderCommands", "Order Commands");
            English.Add("Options", "Options");
            English.Add("Customers", "Customers");
            English.Add("GiftCards", " Gift Cards");
            English.Add("Register", "Register");
            English.Add("Vendors", "Vendors");
            English.Add("VendorItems", "Vendor Items");
            English.Add("VendorOrders", "Vendor Orders");
            English.Add("Map", "Map");
            English.Add("HideButtons", "Hide Buttons");
            English.Add("ShowButtons", "Show Buttons");
            English.Add("ShowMyTickets", "Show My Tickets");
            English.Add("ShowMyOpenTickets", "Show My Open Tickets");
            English.Add("ShowAllTickets", "Show All Tickets");
            English.Add("ShowAllOpenTickets", "Show All Open Tickets");
            English.Add("ShowClosedTickets", "Show Closed Tickets");
            English.Add("ShowCanceledTickets", "Show Canceled Tickets");
            English.Add("ShowTodaysTickets", "Show Todays Tickets");
            English.Add("CreateTicket", "Create Ticket");
            English.Add("Reports", "Reports");
            English.Add("OrderEntryCancelItem", "Cancel Item");
            English.Add("OrderEntryVoidItem", "Void Item");
            English.Add("OrderEntrySpecialInstructions", "Special Instructions");
            English.Add("OrderEntrySystemFunctions", "System Functions");
            English.Add("OrderEntrySetup", "Setup");
            English.Add("OrderEntryRegisterFunctions", "Register Functions");
            English.Add("OrderEntryCoupon", "Coupon");
            English.Add("OrderEntryDiscount", "Discount");
            English.Add("OrderEntryChangeOccasion", "Change Occasion");
            English.Add("OrderEntryFutureTime", "Future Time");
            English.Add("OrderEntryConvertToParty", "Convert To Party");
            English.Add("OrderEntryManageParty", "Manage   Party");
            English.Add("OrderEntryTicketComment", "Ticket Comment");
            English.Add("OrderEntryTaxExemption", "Tax Exemption");
            English.Add("OrderEntryCashOut", "Cash-Out");
            English.Add("OrderEntryCancelTicket", "Cancel   Ticket");
            English.Add("OrderEntryVoidTicket", "Void      Ticket");
            English.Add("OrderEntryProcessReturns", "Process Returns");
            English.Add("OrderEntryPlaceOrder", "Place     Order");
            English.Add("OrderEntryCloseTicket", "Close     Ticket");
            English.Add("OrderEntryReports", "Reports");
            English.Add("OrderEntryChangePassword", "Change Password");
            English.Add("OrderEntryClockOut", "Clock-Out");
            English.Add("OrderEntryCommandConsole", "Command Console");
            English.Add("OrderEntryChangeLanguage", "Change Language");
            English.Add("OrderEntryLogout", "Logout");
            English.Add("OrderEntryExit", "Exit");
            English.Add("OrderEntryEnterTaxExemptionId", "Enter Tax Exemption Id");
            English.Add("OrderEntrySubTotal", "Subtotal: ");
            English.Add("OrderEntryErrorTicketAlreadyOpen", "This ticket you are trying to open is currently open on another terminal.");
            English.Add("OrderEntryTicketLocked", "Ticket Locked");
            English.Add("OrderEntryReturnTotalIs", "Return Total is ");
            English.Add("OrderEntryReturnTotal", "Return Total");
            English.Add("OrderEntryConfirmCancelChanges", "Are you sure you want to cancel all changes?");
            English.Add("OrderEntryFutureTimeUpdateMessage", "You have changed the future time for this ticket. Would you like to update the other tickets in this party to the same future time?");
            English.Add("OrderEntryUpdatePartyTickets", "Update Party Tickets");
            English.Add("OrderEntryShowingAllTickets", "Showing All Tickets");
            English.Add("OrderEntryShowingAllTicketsToday", "Showing All Tickets Today");
            English.Add("OrderEntryShowingAllOpenTickets", "Showing All Open Tickets");
            English.Add("OrderEntryShowingAllCanceledTickets", "Showing All Canceled Tickets");
            English.Add("OrderEntryShowingAllClosedTickets", "Showing All Closed Tickets");
            English.Add("OrderEntryShowingAllDispatchedTickets", "Showing Dispatched Tickets");
            English.Add("OrderEntryShowingYourOpenTickets", "Showing Your Open Tickets");
            English.Add("OrderEntryShowingFutureTickets", "Showing Future Order Tickets");
            English.Add("OrderEntryShowingTicketsFrom", "Showing Tickets from ");
            English.Add("OrderEntryTo", " to ");
            English.Add("OrderEntryCatering", "Catering");
            English.Add("OrderEntryDelivery", "Delivery");
            English.Add("OrderEntryDineIn", "Dine-In");
            English.Add("OrderEntryDriveThru", "Drive-Thru");
            English.Add("OrderEntryCarryout", "Carryout");
            English.Add("OrderEntryPersonalSettings", "Personal Settings");
            #endregion

            #region OrderEntryFunctionsControl
            English.Add("OrderEntryDispatchDriver", "Dispatch Driver");
            English.Add("OrderEntryStartOfDay", "Start Of Day");
            English.Add("OrderEntryEndOfDay", "End Of Day");
            English.Add("OrderEntryEndOfYear", "End Of Year");
            English.Add("OrderEntryEditTimeSheet", "Edit Timesheet");
            English.Add("OrderEntryEditInventory", "Edit Inventory");
            #endregion

            #region OrderEntryReceiptTape
            English.Add("TicketItems", "Ticket Items");
            English.Add("SelectedTicketItemCommands", "Selected Ticket Item Commands");
            English.Add("OrderEntryIncreaseQuantity", "Increase Quantity");
            English.Add("OrderEntryButtonSetQuantity", "Set      Quantity");
            English.Add("OrderEntryDecreaseQuantity", "Decrease Quantity");
            English.Add("OrderEntryButtonCancelItem", "Cancel     Item");
            English.Add("OrderEntryButtonVoidItem", "Void         Item");
            English.Add("OrderEntryButtonReturnItem", "Return     Item");
            English.Add("OrderEntrySpecialInstructionsFor", "Special Instuctions for \"");
            English.Add("OrderEntryNoCancelPermission", "You do not have permission to cancel this ticket item");
            English.Add("OrderEntryNoDecreasePermission", "You do not have permission to decrease the quantity of this ticket item");
            #endregion

            #region OrderEntryRegisterMenuControl
            English.Add("RegisterMenuStartDrawer", "Start Register Drawer");
            English.Add("RegisterMenuNoSale", "No Sale");
            English.Add("RegisterMenuDeposit", "Deposit");
            English.Add("RegisterMenuSafeDrop", "Safe Drop");
            English.Add("RegisterMenuPayout", "Payout");
            English.Add("RegisterMenuFloat", "Float");
            English.Add("RegisterMenuDock", "Dock");
            English.Add("RegisterMenuReport", "Report");
            English.Add("RegisterMenuCloseOut", "Close-Out");
            English.Add("RegisterMenuPreparingReport", "Preparing Report...");
            English.Add("RegisterMenuRunStartOfDay", "You'll need to run Start-Of-Day, before starting a register drawer.");
            English.Add("RegisterMenuUnableToProceed", "Unable to Proceed");
            English.Add("RegisterMenuDepositAmount", "Deposit Amount");
            English.Add("RegisterMenuDropAmount", "Drop Amount");
            English.Add("RegisterMenuCantDropThatMuch", "You do not have that much in the register.");
            English.Add("RegisterMenuInvalidAmount", "Invalid Amount");
            English.Add("RegisterMenuPayoutReason", "Payout Reason");
            English.Add("RegisterMenuPayoutAmount", "Payout Amount");
            English.Add("RegisterMenuConfirmFloat", "Are you sure you want to float your register drawer?");
            English.Add("RegisterMenuNotifyFloat", "Your register drawer is now floating.");
            English.Add("RegisterMenuNotFloating", "You do not have a floating register drawer.");
            English.Add("RegisterMenuDrawerIsNowDocked", "Your floating register drawer has been docked.");
            English.Add("RegisterMenuConfirmDrawerClose", "Are you sure to want to close-out the active register drawer?");
            #endregion

            #region OrderEntrySetupControl
            English.Add("SetupGeneralSettings", "General Settings");
            English.Add("SetupCategoriesAndItems", "Categories and Items");
            English.Add("SetupCouponsAndDiscounts", "Coupons and Discounts");
            English.Add("SetupRoomsAndSeatings", "Rooms and Seatings (Occasions)");
            English.Add("SetupEmployeesAndJobs", "Employees and Jobs");
            English.Add("SetupTaxes", "Taxes");
            English.Add("SetupHardware", "Hardware");
            English.Add("SetupItemSetup", "Item Setup");
            English.Add("SetupCoupons", "Coupons");
            English.Add("SetupDiscounts", "Discounts");
            English.Add("SetupCouponAndDiscountSetup", "Coupon and Discount Setup");
            English.Add("SetupRoomSetup", "Room Setup");
            English.Add("SetupEmployees", "Employees");
            English.Add("SetupEmployeeJobs", "Employee Jobs");
            English.Add("SetupEmployeeSetup", "Employee Setup");
            English.Add("SetupTaxSetup", "Tax Setup");
            English.Add("SetupHardwareSetup", "Hardware Setup");
            #endregion

            #region OrderEntryTicketDetailsControl
            English.Add("MapDestination", "Map Destination");
            English.Add("SelectedTicketDetails", "Selected Ticket Details");
            English.Add("SelectedTicketCommands", "Selected Ticket Commands");
            English.Add("OrderEntryFireEntrees", "Fire   Entrées");
            English.Add("OrderEntryUnCancel", "UnCancel");
            English.Add("OrderEntryChangeEmployee", "Change Employee");
            English.Add("OrderEntryCancel", "Cancel");
            English.Add("OrderEntryRefund", "Refund");
            English.Add("OrderEntryMapDestination", "Map Destination");
            English.Add("OrderEntrySelectedTicketDetails", "Selected Ticket Details");
            English.Add("OrderEntryTicketNumber", "Ticket Number");
            English.Add("OrderEntryStatus", "Status");
            English.Add("OrderEntrySeating", "Seating");
            English.Add("OrderEntryCreatedOn", "Created On");
            English.Add("OrderEntryReadyTime", "Ready Time");
            English.Add("OrderEntryClosedTime", "Closed Time");
            English.Add("OrderEntryOrderNumber", ", Order #");
            English.Add("OrderEntryStartTime", "Start Time");
            English.Add("OrderEntrySuspendedOrder", "Suspended Order");
            English.Add("OrderEntryActiveFuture", "Active (Future Order)");
            English.Add("OrderEntryActiveEntrees", "Active (Unfired Entrées)");
            English.Add("OrderEntryActive", "Active");
            English.Add("OrderEntryServed", "Served");
            #endregion

            #region Personal Settings
            English.Add("SettingsPersonalSettings", "Personal Settings");
            English.Add("SettingsTemperatureScale", "Temperature Scale");
            English.Add("SettingsFahrenheit", "Fahrenheit");
            English.Add("SettingsCelsius", "Celsius");
            English.Add("SettingsKelvin", "Kelvin");
            #endregion

            #region OrderEntryCommands
            English.Add("View", "View");
            English.Add("YouHaveChangedTheFutureTimeForThisTicketWouldYou", "You have changed the future time for this ticket. Would you ");
            English.Add("LikeToUpdateTheOtherTicketsInThisPartyToTheSameFutureTime", "like to update the other tickets in this party to the same future time?");
            English.Add("UpdatePartyTickets", "Update Party Tickets");
            English.Add("EnterTaxExemptionId", "Enter Tax Exemption Id");
            English.Add("ThereIsNoRegisterDrawerStarted", "There is no register drawer started.");
            English.Add("TicketCashout", "Ticket Cash-Out");
            English.Add("YouDoNotHavePermissionToUseTheCurrentRegisterDrawer", "You do not have permission to use the current register drawer.");
            English.Add("ThereAreNoItemsOnThisTicket", "There are no items on this ticket.");
            English.Add("ThisTicketYouAreTryingToCashoutIsCurrentlyOpenOnAnotherTerminal", "This ticket you are trying to cash-out is currently open on another terminal.");
            English.Add("TicketLocked", "Ticket Locked");
            English.Add("PrintingIsDisabledInDemoVersion", "Printing is disabled in demo version");
            English.Add("Disabled", "Disabled");
            English.Add("ChangeEmployee", "Change Employee");
            English.Add("RefundedTotalIs", "Refunded Total is ");
            English.Add("RefundTotal", "Refund Total");
            #endregion

            #region ManagerAlert
            English.Add("ManagerAlert", "Manager Alert");
            #endregion

            #region PosHelper
            English.Add("PermissionRequiredEnterPassword", "Permission Required - Enter Password");
            English.Add("AreYouSureYouWantToReturnTheSelectedTicketItems", "Are you sure you want to return the selected ticket item(s)?");
            English.Add("ReturnTicketItems", "Return Ticket Items");
            English.Add("RefundTicket", "Refund Ticket");
            English.Add("CancelTicket", "Cancel Ticket");
            English.Add("CancelTicketItem", "Cancel Ticket Item");
            English.Add("AreYouSureYouWantToCancelThisTicket", "Are you sure you want to cancel this ticket?");
            English.Add("AreYouSureYouWantToCancelThisTicketItem", "Are you sure you want to cancel this ticket item?");
            English.Add("AreYouSureYouWantToUncancelThisTicket", "Are you sure you want to uncancel this ticket?");
            English.Add("UncancelTicket", "Uncancel Ticket");
            English.Add("AreYouSureYouWantToUncancelThisTicketItem", "Are you sure you want to uncancel this ticket item?");
            English.Add("UncancelTicketItem", "Uncancel Ticket Item");
            English.Add("AreYouSureYouWantToVoidThisTicket", "Are you sure you want to void this ticket?");
            English.Add("VoidTicket", "Void Ticket");
            English.Add("AreYouSureYouWantToVoidThisTicketItem", "Are you sure you want to void this ticket item?");
            English.Add("YouCantVoidATicketThatIsCashedoutTheTicketMustBeRefundedFirst", "You can't void a ticket that is cashed-out. The ticket must be refunded first.");
            English.Add("VoidError", "Void Error");
            English.Add("YouCanNotRunTheEndofdayReportUntilAllOpenTicketsAreClosedout", "You can not run the End-Of-Day report until all open tickets are closed-out.");
            English.Add("EndofdayReport", "End-Of-Day Report");
            English.Add("YouCanNotRunTheEndofdayReportUntilYouCloseoutAllActiveRegisterDrawers", "You can not run the End-Of-Day report until you close-out all active register drawers.");
            English.Add("TheEndOfDayReportShouldOnlyBeRunAtTheEndOfTheDay", "The 'End Of Day' report should only be run at the end of the day. ");
            English.Add("AreYouSureYouWantToRunThisReport", "Are you sure you want to run this report?");
            English.Add("TheEndOfYearReportShouldOnlyBeRunAtTheBeginningOfANewYear", "The 'End Of Year' report should only be run at the beginning of a new year. ");
            #endregion

            #region SqlServerSetup
            English.Add("AboutToInstallDatabase", "About to install database");
            English.Add("DatabaseInstallationException", "Database Installation Exception");
            English.Add("None2", "(None)");
            English.Add("DatabasePatchException", "Database Patch Exception - ");
            #endregion

            #region TaskManagerServiceHelper
            English.Add("FailedToInstallTheTaskManagerAccessService", "Failed to install the Task Manager Access service. ");
            English.Add("ServiceInstallationError", "Service Installation Error");
            English.Add("FailedToStartTheTaskManagerAccessService", "Failed to start the Task Manager Access service.");
            English.Add("ServiceStartError", "Service Start Error");
            #endregion

            #region DeviceManager
            English.Add("Bar1", "Bar #1");
            English.Add("Bar2", "Bar #2");
            English.Add("Bar3", "Bar #3");
            English.Add("InitializationException", "Initialization Exception");
            English.Add("Local", "Local");
            English.Add("Journal", "Journal");
            English.Add("Kitchen1", "Kitchen #1");
            English.Add("Kitchen2", "Kitchen #2");
            English.Add("Kitchen3", "Kitchen #3");
            English.Add("Unused", "Unused");
            #endregion

            #region PrinterManager
            English.Add("TaxTotal", "Tax Total");
            English.Add("ReturnTotal", "Return Total");
            English.Add("Order", "Order #");
            English.Add("TicketNumber", "Ticket #");
            English.Add("Table", "Table: ");
            English.Add("TicketVoid", "*** Ticket Void ***");
            English.Add("TicketRefund", "*** Ticket Refund ***");
            English.Add("TicketCancelMade", "*** Ticket Cancel Made ***");
            English.Add("TicketCancelUnmade", "*** Ticket Cancel Unmade ***");
            English.Add("TicketItemVoid", "*** Ticket Item Void ***");
            English.Add("TicketItemReturn", "*** Ticket Item Return ***");
            English.Add("FutureOrder", "*** Future Order ***");
            English.Add("MakeNow", "*** Make Now ***");
            English.Add("CancelMade", "Cancel Made");
            English.Add("CancelUnmade", "Cancel Unmade");
            English.Add("Hold", "[Hold]");
            English.Add("Fired", "[Fired]");
            English.Add("Make", "[Make]");
            English.Add("Changed", "[Changed]");
            English.Add("Coupons", "Coupons");
            English.Add("Subtotal", "Subtotal");
            English.Add("Tax", "Tax");
            English.Add("Total", "Total");
            English.Add("Unhandled", "Unhandled: ");
            #endregion

            #region RegisterManager
            English.Add("CanNotDetermineTheRegisterIDCheckNetworkSetup", "Can not determine the register ID, check network setup.");
            English.Add("ThereAreNoPhysicalCashRegisterDrawersSetup", "There are no physical cash register drawers setup.");
            English.Add("BothDrawersAre", "Both drawers are ");
            English.Add("TheRegisterDrawerIs", "The register drawer is ");
            English.Add("AlreadyBeingUsedBy", "already being used by ");
            English.Add("OtherEmployees", "other employees.");
            English.Add("AnotherEmployee", "another employee.");
            #endregion

            #region ReportManager
            English.Add("EndOfDayReport", "End-Of-Day Report");
            English.Add("TotalGrossSales", "Total Gross Sales");
            English.Add("TaxCollected", "Tax Collected");
            English.Add("TaxExemptSalesTotal", "Tax Exempt Sales Total");
            English.Add("NumberOfTickets", "Number of Tickets");
            English.Add("TotalOfCoupons", "Total of Coupons");
            English.Add("TotalOfDiscounts", "Total of Discounts");
            English.Add("TotalOfRefunds", "Total of Refunds");
            English.Add("TotalOfReturns", "Total of Returns");
            English.Add("TotalOfPayouts", "Total of Payouts");
            English.Add("EndOfYearReport", "End-Of-Year Report");
            English.Add("TotalSalesByItem", "Total Sales By Item");
            English.Add("TotalSalesByItemFor", "Total Sales By Item for '");
            English.Add("From", "From: ");
            English.Add("To", "To: ");
            English.Add("ItemQuantitySalesRevenueIngredientCost", "Item   Quantity   Sales Revenue   Ingredient Cost");
            English.Add("TotalOfGrossSales", "Total of Gross Sales    ");
            English.Add("TotalSalesByCategory", "Total Sales By Category");
            English.Add("TotalSalesByCategoryFor", "Total Sales By Category for '");
            English.Add("CategoryQuantitySalesRevenue", "Category   Quantity   Sales Revenue");
            English.Add("TotalSalesByEmployee", "Total Sales By Employee");
            English.Add("EmployeeSalesRevenue", "Employee   Sales Revenue");
            English.Add("EmployeeSalesByCategory", "Employee Sales by Category");
            English.Add("EmployeeSalesByItem", "Employee Sales by Item");
            English.Add("EmployeeHours", "Employee Hours");
            English.Add("EmployeeJobHours", "Employee   Job   Hours");
            English.Add("HourlyLaborTotals", "Hourly Labor Totals");
            English.Add("HourlyLaborTotal", "Hourly Labor Total");
            English.Add("TimeOfDayLaborHoursCostOfLabor", "Time of Day   Labor Hours   Cost of Labor");
            English.Add("TimesheetChangeLog", "Timesheet Change Log");
            English.Add("EmployeeTimesheetChangeLog", "Employee Timesheet Change Log");
            English.Add("DeletedTimesheetEntryFor", "\' deleted timesheet entry for \'");
            English.Add("ToriginalStartTimeWas", "\tOriginal Start-Time was \'");
            English.Add("ToriginalEndTimeWas", "\tOriginal End-Time was \'");
            English.Add("ToriginalJobWas", "\tOriginal Job was \'");
            English.Add("ToriginalTipsWas", "\tOriginal Tips was \'");
            English.Add("ToriginalDriverCompensationWas", "\tOriginal Driver Compensation was \'");
            English.Add("ChangedTimesheetEntryFor", "\' changed timesheet entry for \'");
            English.Add("TtimesheetEntryIdIs", "\tTimesheet Entry Id is \'");
            English.Add("RegisterReport", "Register Report");
            English.Add("RegisterReportForRegisterDrawer", "Register report for register drawer ");
            English.Add("Time", "Time: ");
            English.Add("StartingAmount", "Starting Amount");
            English.Add("EndingAmount", "Ending Amount");
            English.Add("DrawerAmount", "Drawer Amount");
            English.Add("DepositAmount", "Deposit Amount");
            English.Add("SafeDropAmount", "Safe Drop Amount");
            English.Add("NumberOfCoupons", "Number of Coupons");
            English.Add("NumberOfDiscounts", "Number of Discounts");
            English.Add("NumberOfRefunds", "Number of Refunds");
            English.Add("NumberOfReturns", "Number of Returns");
            English.Add("NumberOfPayouts", "Number of Payouts");
            English.Add("NumberOfNoSales", "Number of No-Sales");
            English.Add("VoidTransactionReport", "Void Transaction Report");
            English.Add("EmployeeTimeTicketNumberAmount", "Employee   Time   Ticket #   Amount");
            English.Add("ReturnTransactionReport", "Return Transaction Report");
            English.Add("EmployeeTimeTicketNumberNumberItemAmount", "Employee   Time   Ticket #   #   Item   Amount");
            English.Add("RefundTransactionReport", "Refund Transaction Report");
            English.Add("EmployeeTimeTicketNumberRegisterNumberTypeAmount", "Employee   Time   Ticket #   Register #   Type   Amount");
            English.Add("Reopened", "Reopened   ");
            English.Add("Voided", "Voided   ");
            English.Add("NoSaleTransactionReport", "No-Sale Transaction Report");
            English.Add("EmployeeTimeRegisterNumber", "Employee   Time   Register #   ");
            English.Add("SafeDropTransactionReport", "Safe Drop Transaction Report");
            English.Add("EmployeeTimeRegisterNumberAmount", "Employee   Time   Register #   Amount");
            English.Add("PayoutTransactionReport", "Payout Transaction Report");
            English.Add("EmployeeTimeRegisterNumberReasonAmount", "Employee   Time   Register #   Reason   Amount");
            English.Add("RegisterDepositTransactionReport", "Register Deposit Transaction Report");
            English.Add("TicketItemCancelTransactionReport", "Ticket Item Cancel Transaction Report");
            English.Add("EmployeeTimeTicketNumberItemQuantityAmount", "Employee   Time   Ticket #   Item   Quantity   Amount");
            English.Add("Unmade", "(Unmade)");
            English.Add("FloatingDockingTransactionReport", "Floating & Docking Transaction Report");
            English.Add("EmployeeUndockTimeDockTimeSourceDestination", "Employee   Undock Time   Dock Time   Source   Destination");
            English.Add("Undocked", "(Undocked)");
            English.Add("ItemAdjustmentReport", "Item Adjustment Report");
            English.Add("WhenEmployeeTypeItemItemOptionSet", "When   Employee   Type   Item   Item Option Set");
            English.Add("AddedItem", "Added Item   ");
            English.Add("DeletedItem", "Deleted Item   ");
            English.Add("OptionSetAdd", "Option Set Add   ");
            English.Add("OptionSetDelete", "Option Set Delete   ");
            English.Add("ItemPriceChangeReport", "Item Price Change Report");
            English.Add("WhenEmployeeItemChangeOriginalValueNewValue", "When   Employee   Item   Change   Original Value   New Value");
            English.Add("RegularPrice", "Regular Price   ");
            English.Add("SpecialPrice", "Special Price   ");
            English.Add("Added", "Added");
            English.Add("Removed", "Removed");
            English.Add("DayOfWeek", "Day of Week");
            English.Add("StartTime", "Start Time");
            English.Add("EndTime", "End Time");
            English.Add("IngredientRecipeAdjustmentReport", "Ingredient Recipe Adjustment Report");
            English.Add("WhenEmployeeIngredientOriginalQuantityNewQuantityDescription", "When   Employee   Ingredient   Original Quantity   New Quantity   Description");
            English.Add("MeasurementChanged", "[Measurement Changed]   ");
            English.Add("YieldChanged", "[Yield Changed]   ");
            English.Add("InventoryAdjustmentsReport", "Inventory Adjustments Report");
            English.Add("WhenEmployeeOriginalQuantityNewQuantityIngredient", "When   Employee   Original Quantity   New Quantity   Ingredient");
            English.Add("ItemRecipeAdjustmentReport", "Item Recipe Adjustment Report");
            English.Add("WhenEmployeeItemOriginalQuantityNewQuantityIngredient", "When   Employee   Item   Original Quantity   New Quantity   Ingredient");
            English.Add("CurrentInventoryReport", "Current Inventory Report");
            English.Add("IngredientQuantity", "Ingredient   Quantity");
            English.Add("WasteByIngredientReport", "Waste by Ingredient Report");
            English.Add("IngredientQuantityCost", "Ingredient   Quantity   Cost");
            English.Add("UsageByIngredientReport", "Usage by Ingredient Report");
            English.Add("WasteByItemReport", "Waste by Item Report");
            English.Add("ItemQuantityItemCostIngredientCost", "Item   Quantity   Item Cost   Ingredient Cost");
            English.Add("WasteByCategoryReport", "Waste by Category Report");
            English.Add("CategoryQuantityItemCostIngredientCost", "Category   Quantity   Item Cost   Ingredient Cost");
            #endregion

            #region UserControlManager
            English.Add("YouAreNotUsingWindowsVistaOrNewerWindowsKeyboardProtectionIsDisabled", "You are not using Windows Vista or newer Windows. Keyboard protection is disabled.");
            #endregion

            #region CouponEditorControl
            English.Add("CouponDetails", "Coupon Details");
            English.Add("CouponCategories", "Coupon Categories");
            English.Add("CouponItems", "Coupon Items");
            English.Add("SelectCategories", "Select Categories");
            English.Add("SelectItems", "Select Items");
            #endregion

            #region OrderEntryItemSelectionControl
            English.Add("YouCanNotAddMoreThan3TicketItemsToATicketInTheDemoVersion", "You can not add more than 3 ticket items to a ticket in the demo version");
            English.Add("DemoRestriction", "Demo Restriction");
            #endregion

            #region OrderEntryOrderCommandsControl
            English.Add("ConvertToParty", "Convert To Party");
            English.Add("ManageParty", "Manage Party");
            #endregion

            #region OrderEntryTicketSelectionControl
            English.Add("TicketFilter", "Ticket    Filter");
            English.Add("OccasionFilter", "Occasion Filter");
            English.Add("Tickets", "Tickets");
            English.Add("TicketCommands", "Ticket Commands");
            English.Add("SelectDateRange", "Select Date Range");
            English.Add("YouCanNotCreateTicketsUntilStartOfDayHasBeenCompleted", "You can not create tickets until Start-Of-Day has been completed");
            English.Add("RequiresStartOfDay", "Requires Start-Of-Day");
            English.Add("Party", ", Party: ");
            English.Add("CreateTime", "Create Time: ");
            English.Add("Comment", "Comment: ");
            #endregion

            #region PartyEditControl
            English.Add("Delete", "Delete");
            English.Add("PartyHostsName", "Party Host's Name");
            English.Add("PartySize", "Party Size");
            English.Add("Notes", "Notes");
            English.Add("InvitedGuestList", "Invited Guest List");
            English.Add("InvitesName", "Invite's Name");
            English.Add("EditParty", "Edit Party");
            #endregion

            #region PartyManagementControl
            English.Add("AddTicket", "Add    Ticket");
            English.Add("RemoveTicket", "Remove Ticket");
            English.Add("ChangeSeating", "Change Seating");
            English.Add("EditPartyInfo", "Edit    Party     Info");
            English.Add("SelectAllItems", "Select      All      Items");
            English.Add("UnselectAllSelected", "Unselect  All  Selected");
            English.Add("SingleTicket", "Single Ticket");
            English.Add("CurrentTicket", "Current Ticket");
            English.Add("ChangeTicket", "Change Ticket");
            English.Add("ThisTicketIsOpenSomewhereElseItCanNotBeModified", "This ticket is open somewhere else, it can not be modified");
            English.Add("ThePartyInformationForThisTicketIsCurrentlyBeingModifiedSomewhereElse", "The party information for this ticket is currently being modified somewhere else.");
            English.Add("PartyInformationLocked", "Party Information Locked");
            English.Add("YouCanOnlyHave2PartyTicketsInTheDemoVersion", "You can only have 2 party tickets in the Demo version.");
            English.Add("TheSelectedTicketCanNotBeRemovedBecauseItAlreadyOpenedSomewhereElse", "The selected ticket can not be removed, because it already opened somewhere else.");
            English.Add("YouCanNotRemoveATicketThatHasItemsOnItFirstTransferThoseItemsToADifferentTicket", "You can not remove a ticket that has items on it. First transfer those items to a different ticket.");
            English.Add("TicketNotEmpty", "Ticket Not Empty");
            English.Add("AreYouSureYouWantToConvertThisPartyBackIntoASingleTicket", "Are you sure you want to convert this party, back into a single ticket?");
            English.Add("ConfirmSingleTicket", "Confirm Single Ticket");
            English.Add("OneOrMoreOfTheTicketsInThisPartyIsCurrentlyBeingModifiedSomewhereElseCanNotChangeToSingleTicket", "One or more of the ticket(s) in this party is currently being modified somewhere else. Can not change to single ticket.");
            English.Add("YouCanNotAddMoreThan3TicketItemsToASingleTicketInTheDemoVersionAdditionalTicketItemsWillBeRemoved", "You can not add more than 3 ticket items to a single ticket in the Demo version. Additional ticket items will be removed.");
            English.Add("TheDestinationTicketIsOpenSomewhereElseItCanNotHaveItemsTransferedToIt", "The destination ticket is open somewhere else, it can not have items transfered to it.");
            English.Add("YouCanNotMoveAnyMoreTicketItemsToTheDestinationTicketTheDemoVersionIsLimitedTo3TicketItemsPerTicket", "You can not move any more ticket items to the destination ticket. The Demo version is limited to 3 ticket items per ticket.");
            English.Add("NotAllTheTicketItemsWereMovedTheDemoVersionLimitedTo3TicketItemsPerTicket", "Not all the ticket items were moved. The Demo version limited to 3 ticket items per ticket.");
            #endregion

            #region RegisterDrawerStartControl
            English.Add("StartDrawer", "Start Drawer");
            English.Add("YouHaveNotSpecifiedAStartingAmount", "You have not specified a starting amount");
            English.Add("StartUpRegister", "Start-Up Register");
            #endregion

            #region ReportsMenuControl
            English.Add("InventoryAdjustments", "Inventory Adjustments");
            English.Add("IngredientRecipeAdjustments", "Ingredient Recipe Adjustments");
            English.Add("ItemAdjustments", "Item Adjustments");
            English.Add("ItemRecipeAdjustments", "Item Recipe Adjustments");
            English.Add("TodayOnly", "Today Only");
            English.Add("SpecifyDates", "Specify Dates");
            English.Add("OperationalDays", "Operational Days");
            English.Add("MonthToDate", "Month To Date");
            English.Add("YearToDate", "Year To Date");
            English.Add("AllDates", "All Dates");
            English.Add("TotalSales", "Total Sales");
            English.Add("SalesByItem", "Sales by Item");
            English.Add("SalesByCategory", "Sales by Category");
            English.Add("SalesByEmployee", "Sales by Employee");
            English.Add("CostOfSales", "Cost of Sales");
            English.Add("EmployeeSales", "Employee Sales");
            English.Add("TicketTransactions", "Ticket Transactions");
            English.Add("Cancels", "Cancels");
            English.Add("Voids", "Voids");
            English.Add("Returns", "Returns");
            English.Add("Refunds", "Refunds");
            English.Add("RegisterTransactions", "Register Transactions");
            English.Add("NoSales", "No Sales");
            English.Add("Payouts", "Payouts");
            English.Add("SafeDrops", "Safe Drops");
            English.Add("Deposits", "Deposits");
            English.Add("FloatingDocking", "Floating & Docking");
            English.Add("Labor", "Labor");
            English.Add("HourlyTotals", "Hourly Totals");
            English.Add("Inventory", "Inventory");
            English.Add("IngredientUsage", "Ingredient Usage");
            English.Add("CurrentInventory", "Current Inventory");
            English.Add("Waste", "Waste");
            English.Add("WasteByItem", "Waste by Item");
            English.Add("WasteByCategory", "Waste by Category");
            English.Add("WasteByIngredient", "Waste by Ingredient");
            English.Add("WasteByHour", "Waste by Hour");
            English.Add("Logging", "Logging");
            English.Add("PriceChanges", "Price Changes");
            English.Add("TimesheetChanges", "Timesheet Changes");
            English.Add("RangeOptions", "Range Options");
            English.Add("PreparingReport", "Preparing Report...");
            #endregion

            #region SeatingDineInControl
            English.Add("SelectSeating", "Select Seating");
            English.Add("SEATINGISNOTSETUP", "SEATING IS NOT SETUP");
            #endregion

            #region SeatingMaintenanceControl
            English.Add("NewRoom", "New    Room");
            English.Add("EditSeatings", "Edit Seatings");
            English.Add("DeleteRoom", "Delete Room");
            English.Add("UpdateRoom", "Update Room");
            English.Add("SeatingProperties", "Seating Properties");
            English.Add("RoomProperties", "Room Properties");
            English.Add("Rooms", "Rooms");
            English.Add("AddRoom", "Add Room");
            English.Add("RoomSetup", "Room Setup");
            English.Add("Seatings", "Seatings");
            English.Add("AddSeating", "Add Seating");
            English.Add("DeleteSeating", "Delete Seating");
            English.Add("UpdateSeating", "Update Seating");
            English.Add("EditRooms", "Edit Rooms");
            English.Add("SeatingSetupRoom", "Seating Setup [Room: ");
            English.Add("AreYouSureYouWantToDeleteTheSelectedRoomAndAllItsSeatings", "Are you sure you want to delete the selected room and all it's seatings?");
            English.Add("AreYouSureYouWantToDeleteTheSelectedRoom", "Are you sure you want to delete the selected room?");
            English.Add("ConfirmDeletion", "Confirm Deletion");
            English.Add("AreYouSureYouWantToDeleteTheSelectedSeating", "Are you sure you want to delete the selected seating?");
            English.Add("DineIn", "Dine-In");
            English.Add("DriveThru", "Drive-Thru");
            English.Add("Delivery", "Delivery");
            English.Add("Carryout", "Carryout");
            English.Add("Catering", "Catering");
            English.Add("OccasionSelectionIsCurrentlyDisabledDoYouWantToEnableIt", "Occasion selection is currently disabled. Do you want to enable it?");
            English.Add("EnableOccasionSelection", "Enable Occasion Selection");
            English.Add("OccasionSelectionWasDisabledBecauseNoRoomsExist", "Occasion selection was disabled because no rooms exist.");
            English.Add("OccasionSelectionDisabled", "Occasion Selection Disabled");
            #endregion

            #region SeatingRoomEditorControl
            English.Add("TicketType", "Ticket Type");
            English.Add("UnsupportedTickettype", "Unsupported TicketType");
            #endregion

            #region SeatingSelectionControl
            English.Add("SelectOccasion", "Select Occasion");
            English.Add("ChangeOccasion", "Change Occasion");
            English.Add("OccasionSelection", "Occasion Selection");
            #endregion

            #region StartupWindow
            English.Add("Tempos", "TemPOS");
            English.Add("Version10", "Version 1.0");
            English.Add("Build", "Build");
            English.Add("Version", "Version ");
            English.Add("Revision", ", Revision ");
            English.Add("StartingApplication", "Starting application");
            English.Add("ExitingApplicationDueToAnUnhandledException", "Exiting application (due to an unhandled exception)");
            English.Add("UnhandledException", "Unhandled Exception: ");
            English.Add("LocalsettingEditor", "LocalSetting editor");
            English.Add("YouHaveNotEnteredAValidCompanyNameAndorSerialNumber", "You have not entered a valid company name and/or serial number.");
            English.Add("CanNotConnectToTheTemposUpdateServerToVerifyYourSerialNumberPleaseCheckYourInternetConnectionIfYouAreConnectedToTheInternetPleaseTryAgainLaterTheUpdateServerIsDownForMaintenance", "Can not connect to the TemPOS update server to verify your serial number. Please check your Internet connection. If you are connected to the Internet, please try again later, the update server is down for maintenance.");
            English.Add("ConnectionFailed", "Connection Failed");
            English.Add("TheSQLServiceAndSQLBrowserServiceAreNotRunningWouldYouLikeToStartThem", "The SQL service and SQL Browser service are not running. Would you like to start them?");
            English.Add("TheSQLServiceIsNotRunningWouldYouLikeToStartIt", "The SQL service is not running. Would you like to start it?");
            English.Add("TheSQLBrowserServiceIsNotRunningWouldYouLikeToStartIt", "The SQL Browser service is not running. Would you like to start it?");
            English.Add("StartingSQLServices", "Starting SQL Services...");
            English.Add("CouldNotStartTheSQLServiceWouldYouLikeToContinue", "Could not start the SQL service. Would you like to continue?");
            English.Add("DatabaseConnectionTimeout", "Database Connection Timeout");
            English.Add("InstallingSQLDatabase", "Installing SQL Database...");
            English.Add("Information", "Information");
            English.Add("TheDatabaseDesignCurrentlyBeingUsedIsIncorrectForThisVersionOfTempos", "The database design currently being used is incorrect for this version of TemPOS.");
            English.Add("StartupError", "Startup Error");
            #endregion

            #region SystemSettingsEditorControl
            English.Add("Exit", "Exit");
            English.Add("CompanyName", "Company Name");
            English.Add("SerialNumber", "Serial Number");
            English.Add("DatabaseServer", "Database Server");
            English.Add("DatabaseLogin", "Database Login");
            English.Add("DatabasePassword", "Database Password");
            English.Add("DatabaseName", "Database Name");
            English.Add("SystemSettings", "System Settings");
            #endregion

            #region TaxEditorControl
            English.Add("TaxDescription", "Tax Description");
            English.Add("TaxPercentage", "Tax Percentage");
            English.Add("PleaseEnterAValidPercentageValue", "Please enter a valid percentage value");
            English.Add("EmptyString", "Empty string");
            English.Add("PleaseEnterAValidDescription", "Please enter a valid description");
            #endregion

            #region TicketCashoutControl
            English.Add("TaxExemption", "Tax Exemption");
            English.Add("PrintReceipt", "Print Receipt");
            English.Add("TicketDiscounts", "Ticket Discounts");
            English.Add("TicketCoupons", "Ticket Coupons");
            English.Add("DisabledInDemoVersion", "Disabled in Demo Version");
            English.Add("CashOutTicket", "Cash-Out Ticket");
            #endregion

            #region TicketCashoutPaymentControl
            English.Add("Cash", "Cash");
            English.Add("Check", "Check");
            English.Add("CreditCard", "Credit    Card");
            English.Add("GiftCard", "Gift        Card");
            English.Add("SubTotal", "Sub Total");
            English.Add("AmountPayed", "Amount Payed");
            English.Add("AmountDue", "Amount Due");
            English.Add("ChangeDue", "Change Due");
            #endregion

            #region TicketCouponControl
            English.Add("ApplyCoupon", "Apply Coupon");
            English.Add("ClearSelectedCoupon", "Clear Selected Coupon");
            English.Add("AvailableCoupons", "Available Coupons");
            English.Add("AppliedCoupons", "Applied Coupons");
            English.Add("YouNeedToSelectATicketItemToUseThisCouponOn", "You need to select a ticket item to use this coupon on");
            English.Add("ThatCouponCanNotBeAppliedToAnyItemsOnThisTicket", "That coupon can not be applied to any items on this ticket.");
            #endregion

            #region TicketDeliveryDispatchControl
            English.Add("DispatchDriver", "Dispatch Driver");
            English.Add("Drivers", "Drivers");
            English.Add("Deliveries", "Deliveries");
            English.Add("OrderNumber", "Order #");
            English.Add("DeliveryDriverDispatch", "Delivery Driver Dispatch");
            #endregion

            #region TicketDiscountControl
            English.Add("ApplyDiscount", "Apply Discount");
            English.Add("ClearSelectedDiscount", "Clear Selected Discount");
            English.Add("AvailableDiscounts", "Available Discounts");
            English.Add("AppliedDiscounts", "Applied Discounts");
            English.Add("YouDoNotHavePermissionToApplyThisDiscount", "You do not have permission to apply this discount.");
            English.Add("EnterDiscountPercentage", "Enter Discount Percentage");
            English.Add("EnterDiscountAmount", "Enter Discount Amount");
            #endregion

            #region TimesheetEditorControl
            English.Add("DeleteRecord", "Delete Record");
            English.Add("SaveChange", "Save Change");
            English.Add("Job", "Job");
            English.Add("DeclaredTips", "Declared Tips");
            English.Add("DriverCompensation", "Driver Compensation");
            English.Add("YouCanNotSetAStartTimeThatOccursAfterTheEndTime", "You can not set a start time that occurs after the end time.");
            English.Add("ValidationError", "Validation Error");
            English.Add("YouCanNotSetAnEndTimeThatOccursBeforeTheStartTime", "You can not set an end time that occurs before the start time.");
            English.Add("DeclareTips", "Declare Tips");
            English.Add("AreYouSureYouWantToDeleteThisRecordThisActionCanNotBeUndone", "Are you sure you want to delete this record. This action can not be undone.");
            English.Add("Confirm", "Confirm");
            English.Add("TheTimesSpecifiedWouldOverlapAnExistingShift", "The times specified would overlap an existing shift");
            English.Add("EditEntry", "Edit Entry");
            #endregion

            #region TimesheetMaintenanceControl
            English.Add("ThisRecordIsLockedAndCanNotBeChanged", "This record is locked and can not be changed.");
            English.Add("EntryLocked", "Entry Locked");
            English.Add("ThisRecordIsYourCurrentClockInAndCanNotBeChangedUntilYouClockedOut", "This record is your current clock-in, and can not be changed until you clocked-out.");
            English.Add("EmployeeNotClockedOut", "Employee Not Clocked-Out");
            English.Add("ThisEmployeeMustClockOutBeforeYouCanEditWouldYouLikeToClockOutThisEmployeeRightNow", "This employee must clock-out before you can edit. Would you like to clock-out this employee right now?");
            English.Add("TimesheetMaintenance", "Timesheet Maintenance");
            #endregion

            #region DeliveryMaintenanceControl
            English.Add("TODODeliveryMaintenanceControl", "TODO: Delivery Maintenance Control");
            #endregion

            #region DeviceChoiceControl
            English.Add("DisabledInDemo", "Disabled in Demo");
            English.Add("POSPrinter", "POS Printer");
            English.Add("PrintOutput", "Print Output");
            English.Add("CashDrawer", "Cash Drawer");
            English.Add("Scanner", "Scanner");
            English.Add("CoinDispenser", "Coin Dispenser");
            English.Add("BumpBar", "Bump Bar");
            #endregion

            #region EmployeeScheduleEditorControl
            English.Add("TODOScheduleEditorControl", "TODO: Schedule Editor Control");
            #endregion

            #region PersonalSettingsControl
            English.Add("Debug", "Debug");
            English.Add("ShowMyOpen", "Show My Open");
            English.Add("ShowAllOpen", "Show All Open");
            English.Add("ShowFutureOrders", "Show Future Orders");
            English.Add("ShowClosed", "Show Closed");
            English.Add("ShowAllDay", "Show All Day");
            English.Add("ShowRange", "Show Range");
            English.Add("ShowAll", "Show All");
            English.Add("DefaultTicketFilter", "Default Ticket Filter");
            #endregion

            #region PrintOptionEditorControl
            English.Add("AddLocation", "Add Location");
            English.Add("EditLocation", "Edit Location");
            English.Add("RemoveLocation", "Remove Location");
            English.Add("PrinterGroupName", "Printer Group Name");
            English.Add("PrintLocations", "Print Locations");
            #endregion

            #region SeatingEditorControl
            English.Add("Capacity", "Capacity");
            #endregion

            #region SeatingPersonInformationControl
            English.Add("StartTicket", "Start  Ticket");
            English.Add("Occasion", "Occasion");
            English.Add("PhoneNumber", "Phone Number");
            English.Add("StreetAddressLine1", "Street Address (Line 1)");
            English.Add("StreetAddressLine2", "Street Address (Line 2)");
            English.Add("Zipcode", "Zipcode");
            English.Add("CityState", "City / State");
            #endregion

            #region TaxMaintenanceControl
            English.Add("NewTax", "New     Tax");
            English.Add("UpdateTax", "Update Tax");
            English.Add("Taxes", "Taxes");
            English.Add("TaxProperties", "Tax Properties");
            #endregion

            #region TicketFilterControl
            English.Add("ShowCanceled", "Show Canceled");
            English.Add("ShowDispatched", "Show Dispatched");
            #endregion

            #region TicketTypeFilterControl
            English.Add("NoneDisplayAll", "None (Display All)");
            #endregion

            #region GeneralSettingsControl
            English.Add("GeneralSettings", "General Settings");
            English.Add("Brushes", "Brushes");
            English.Add("SoftwareUpdates", "Software Updates");
            #endregion

            #region IngredientEditorControl
            English.Add("IngredientDetails", "Ingredient Details");
            English.Add("IngredientPreparation", "Ingredient Preparation");
            #endregion

            #region EmployeeScheduleMaintenanceControl
            English.Add("New", "New");
            #endregion

            #region OrderEntryChangeEmployeeControl
            English.Add("ChangeOwner", "Change Owner");
            #endregion

            #region PrintOptionMaintenanceControl
            English.Add("NewItem", "New Item");
            English.Add("UpdateItem", "Update Item");
            English.Add("CloseWindow", "Close Window");
            #endregion
        }
    }
}
