using System;
using System.Data.SqlTypes;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using TemPOS.Helpers;
using PosControls;
using PosControls.Interfaces;
using PosModels.Managers;
using TemPOS.Managers;
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for ReportsMenuControl.xaml
    /// </summary>
    public partial class ReportsMenuControl : UserControl
    {
        private RangeType RangeSelection
        {
            get
            {
                if (radioAllDates.IsSelected)
                    return RangeType.AllDates;
                if (radioDateRange.IsSelected)
                    return RangeType.DateRange;
                if (radioDayOfOperationRange.IsSelected)
                    return RangeType.DayOfOperationRange;
                if (radioMonthToDate.IsSelected)
                    return RangeType.MonthToDate;
                if (radioTodayOnly.IsSelected)
                    return RangeType.TodayOnly;
                if (radioYearToDate.IsSelected)
                    return RangeType.YearToDay;
                return RangeType.None;
            }
        }

        public ReportsMenuControl()
        {
            InitializeComponent();
            SetupUserPreferenceForRange();
            Loaded += ReportsMenuControl_Loaded;
        }

        [Obfuscation(Exclude = true)]
        void ReportsMenuControl_Loaded(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Closed += ReportsMenuControl_Closed;
        }
        [Obfuscation(Exclude = true)]

        [Obfuscation(Exclude = true)]
        void ReportsMenuControl_Closed(object sender, EventArgs e)
        {
            int? value = null;

            if (radioTodayOnly.IsSelected)
                value = 0;
            else if (radioDateRange.IsSelected)
                value = 1;
            else if (radioDayOfOperationRange.IsSelected)
                value = 2;
            else if (radioMonthToDate.IsSelected)
                value = 3;
            else if (radioYearToDate.IsSelected)
                value = 4;
            else if (radioAllDates.IsSelected)
                value = 5;

            SettingManager.SetEmployeeSetting(SessionManager.ActiveEmployee.Id,
                "ReportsMenuDefaultRange", value);
        }

        private void SetupUserPreferenceForRange()
        {
            int? defaultRange = SettingManager.GetEmployeeSetting(SessionManager.ActiveEmployee.Id,
                "ReportsMenuDefaultRange").IntValue;
            if (defaultRange == null)
                SetRadioChecked(radioTodayOnly);
            else if (defaultRange.Value == 0)
                SetRadioChecked(radioTodayOnly);
            else if (defaultRange.Value == 1)
                SetRadioChecked(radioDateRange);
            else if (defaultRange.Value == 2)
                SetRadioChecked(radioDayOfOperationRange);
            else if (defaultRange.Value == 3)
                SetRadioChecked(radioMonthToDate);
            else if (defaultRange.Value == 4)
                SetRadioChecked(radioYearToDate);
            else if (defaultRange.Value == 5)
                SetRadioChecked(radioAllDates);
        }

        private bool PromptRange(string fieldName, ref DateTime startDate, ref DateTime endDate)
        {
            if (RangeSelection == RangeType.DayOfOperationRange)
            {
                PosDialogWindow window = DayOfOperationRangeSelectionControl.CreateInDefaultWindow();
                var control = window.DockedControl as DayOfOperationRangeSelectionControl;

                window.Title = fieldName;
                window.ShowDialogForActiveWindow();
                if (window.ClosedByUser)
                    return false;
                startDate = control.StartRange;
                endDate = control.EndRange;
                return true;
            }
            if (RangeSelection == RangeType.DateRange)
            {
                return PosDialogWindow.PromptDateRange(Strings.SelectDateRange,
                    ref startDate, ref endDate);
            }
            if (RangeSelection == RangeType.AllDates)
            {
                startDate = SqlDateTime.MinValue.Value;
                endDate = SqlDateTime.MaxValue.Value;
                return true;
            }
            if (RangeSelection == RangeType.MonthToDate)
            {
                endDate = DateTime.Now;
                startDate = new DateTime(endDate.Year, endDate.Month, 1, 0, 0, 0);
                return true;
            }
            if (RangeSelection == RangeType.YearToDay)
            {
                endDate = DateTime.Now;
                startDate = new DateTime(endDate.Year, 1, 1, 0, 0, 0);
                return true;
            }
            if (RangeSelection == RangeType.TodayOnly)
            {
                endDate = DateTime.Now;
                startDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 0, 0, 0);
                return true;
            }
            return false;
        }

        private void PrintRefunds(DateTime startDate, DateTime endDate)
        {
            PosDialogWindow window = ShowPreparingReportWindow();
            ReportManager.PrintRefunds(startDate, endDate, ReportClosed_EventHandler);
            window.Close();
        }

        private void PrintNoSales(DateTime startDate, DateTime endDate)
        {
            PosDialogWindow window = ShowPreparingReportWindow();
            ReportManager.PrintNoSales(startDate, endDate, ReportClosed_EventHandler);
            window.Close();
        }

        private void PrintSafeDrops(DateTime startDate, DateTime endDate)
        {
            PosDialogWindow window = ShowPreparingReportWindow();
            ReportManager.PrintSafeDrops(startDate, endDate, ReportClosed_EventHandler);
            window.Close();
        }

        private void PrintPayouts(DateTime startDate, DateTime endDate)
        {
            PosDialogWindow window = ShowPreparingReportWindow();
            ReportManager.PrintPayouts(startDate, endDate, ReportClosed_EventHandler);
            window.Close();
        }

        private void PrintAdminisrtrativeVoidReport(DateTime startDate, DateTime endDate)
        {
            PosDialogWindow window = ShowPreparingReportWindow();
            ReportManager.PrintAdministrativeVoids(startDate, endDate,
                ReportClosed_EventHandler);
            window.Close();
        }

        private void PrintSaleTotalsByItemReport(DateTime startDate, DateTime endDate)
        {
            PosDialogWindow window = ShowPreparingReportWindow();
            ReportManager.PrintSaleTotalsByItem(startDate, endDate,
                ReportClosed_EventHandler);
            window.Close();
        }

        private void PrintSalesByEmployeeReport(DateTime startDate, DateTime endDate)
        {
            PosDialogWindow window = ShowPreparingReportWindow();
            ReportManager.PrintSalesByEmployee(startDate, endDate, ReportClosed_EventHandler);
            window.Close();
        }

        private void PrintLaborEmployeeHoursReport(DateTime startDate, DateTime endDate)
        {
            PosDialogWindow window = ShowPreparingReportWindow();
            ReportManager.PrintLaborEmployeeHours(startDate, endDate,
                ReportClosed_EventHandler);
            window.Close();
        }

        private void PrintLaborHourlyTotalsReport(DateTime startDate, DateTime endDate)
        {
            PosDialogWindow window = ShowPreparingReportWindow();
            ReportManager.PrintLaborHourlyTotals(startDate, endDate,
                ReportClosed_EventHandler);
            window.Close();
        }

        private void PrintLaborTimesheetChangesReport(DateTime startDate, DateTime endDate)
        {
            PosDialogWindow window = ShowPreparingReportWindow();
            ReportManager.PrintLaborTimesheetChanges(startDate, endDate,
                ReportClosed_EventHandler);
            window.Close();
        }

        private void PrintSaleTotalsByCategory(DateTime startDate, DateTime endDate)
        {
            PosDialogWindow window = ShowPreparingReportWindow();
            ReportManager.PrintSaleTotalsByCategory(startDate, endDate,
                ReportClosed_EventHandler);
            window.Close();
        }

        private void PrintEmployeeSalesByCategory(DateTime startDate, DateTime endDate)
        {
            PosDialogWindow window = ShowPreparingReportWindow();
            ReportManager.PrintEmployeeSalesByCategory(startDate, endDate,
                ReportClosed_EventHandler);
            window.Close();
        }

        private void PrintEmployeeSalesByItem(DateTime startDate, DateTime endDate)
        {
            PosDialogWindow window = ShowPreparingReportWindow();
            ReportManager.PrintEmployeeSalesByItem(startDate, endDate,
                ReportClosed_EventHandler);
            window.Close();
        }

        private void PrintReturns(DateTime startDate, DateTime endDate)
        {
            PosDialogWindow window = ShowPreparingReportWindow();
            ReportManager.PrintReturns(startDate, endDate, ReportClosed_EventHandler);
            window.Close();
        }

        private void PrintDeposits(DateTime startDate, DateTime endDate)
        {
            PosDialogWindow window = ShowPreparingReportWindow();
            ReportManager.PrintDeposits(startDate, endDate, ReportClosed_EventHandler);
            window.Close();
        }

        private void PrintFloatingDocking(DateTime startDate, DateTime endDate)
        {
            PosDialogWindow window = ShowPreparingReportWindow();
            ReportManager.PrintFloatingDocking(startDate, endDate, ReportClosed_EventHandler);
            window.Close();
        }

        private void PrintCancels(DateTime startDate, DateTime endDate)
        {
            PosDialogWindow window = ShowPreparingReportWindow();
            ReportManager.PrintCancels(startDate, endDate, ReportClosed_EventHandler);
            window.Close();
        }

        private void PrintWasteByItem(DateTime startDate, DateTime endDate)
        {
            PosDialogWindow window = ShowPreparingReportWindow();
            ReportManager.PrintWasteByItem(startDate, endDate, ReportClosed_EventHandler);
            window.Close();
        }

        private void PrintWasteByCategory(DateTime startDate, DateTime endDate)
        {
            PosDialogWindow window = ShowPreparingReportWindow();
            ReportManager.PrintWasteByCategory(startDate, endDate, ReportClosed_EventHandler);
            window.Close();
        }

        private void PrintWasteByIngredient(DateTime startDate, DateTime endDate)
        {
            PosDialogWindow window = ShowPreparingReportWindow();
            ReportManager.PrintWasteByIngredient(startDate, endDate, ReportClosed_EventHandler);
            window.Close();
        }

        private void PrintWasteByHours(DateTime startDate, DateTime endDate)
        {
            // Note: Collapsed, not being used
        }

        private void PrintUsageByIngredient(DateTime startDate, DateTime endDate)
        {
            PosDialogWindow window = ShowPreparingReportWindow();
            ReportManager.PrintUsageByIngredient(startDate, endDate, ReportClosed_EventHandler);
            window.Close();
        }

        private void PrintCurrentInventory(DateTime startDate, DateTime endDate)
        {
            PosDialogWindow window = ShowPreparingReportWindow();
            ReportManager.PrintCurrentInventory(startDate, endDate, ReportClosed_EventHandler);
            window.Close();
        }

        private void PrintInventoryAdjustments(DateTime startDate, DateTime endDate)
        {
            PosDialogWindow window = ShowPreparingReportWindow();
            ReportManager.PrintInventoryAdjustments(startDate, endDate, ReportClosed_EventHandler);
            window.Close();
        }

        private void PrintIngredientSetChanges(DateTime startDate, DateTime endDate)
        {
            PosDialogWindow window = ShowPreparingReportWindow();
            ReportManager.PrintIngredientSetChanges(startDate, endDate, ReportClosed_EventHandler);
            window.Close();
        }

        private void PrintItemIngredientChanges(DateTime startDate, DateTime endDate)
        {
            PosDialogWindow window = ShowPreparingReportWindow();
            ReportManager.PrintItemIngredientChanges(startDate, endDate, ReportClosed_EventHandler);
            window.Close();
        }

        private void PrintPricingChanges(DateTime startDate, DateTime endDate)
        {
            PosDialogWindow window = ShowPreparingReportWindow();
            ReportManager.PrintPricingChanges(startDate, endDate, ReportClosed_EventHandler);
            window.Close();
        }

        private void PrintItemDiscontinuation(DateTime startDate, DateTime endDate)
        {
            PosDialogWindow window = ShowPreparingReportWindow();
            ReportManager.PrintItemAdjustments(startDate, endDate, ReportClosed_EventHandler);
            window.Close();
        }

        private PosDialogWindow ShowPreparingReportWindow()
        {
            ActionNotificationControl notification =
                ActionNotificationControl.Create(Window.GetWindow(this) as PosDialogWindow,
                Strings.PreparingReport, Strings.Notification);
            notification.Show();
            notification.ParentWindow.Closed += ParentWindow_Closed;
            return notification.ParentWindow;
        }

        [Obfuscation(Exclude = true)]
        void ParentWindow_Closed(object sender, EventArgs e)
        {
            var parent = Window.GetWindow(this) as IShadeable;
            if (parent != null) parent.ShowShadingOverlay = true;
        }

        [Obfuscation(Exclude = true)]
        private void ReportClosed_EventHandler(object sender, EventArgs e)
        {
            var parent = Window.GetWindow(this) as IShadeable;
            if (parent != null) parent.ShowShadingOverlay = false;
        }

        [Obfuscation(Exclude = true)]
        private void radioButton_SelectionGained(object sender, EventArgs e)
        {
            SetRadioChecked(sender);
        }

        private void SetRadioChecked(object sender)
        {
            radioAllDates.IsSelected = Equals(sender, radioAllDates);
            radioDateRange.IsSelected = Equals(sender, radioDateRange);
            radioDayOfOperationRange.IsSelected = Equals(sender, radioDayOfOperationRange);
            radioMonthToDate.IsSelected = Equals(sender, radioMonthToDate);
            radioTodayOnly.IsSelected = Equals(sender, radioTodayOnly);
            radioYearToDate.IsSelected = Equals(sender, radioYearToDate);
        }

        #region Button Click Events
        [Obfuscation(Exclude = true)]
        private void buttonSalesTotalsByItem_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = SqlDateTime.MinValue.Value;
            DateTime endDate = SqlDateTime.MaxValue.Value;
            if (!PromptRange(Strings.SelectDateRange, ref startDate, ref endDate))
                return;
            PrintSaleTotalsByItemReport(startDate, endDate);
        }

        [Obfuscation(Exclude = true)]
        private void buttonSalesTotalsByCategory_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = SqlDateTime.MinValue.Value;
            DateTime endDate = SqlDateTime.MaxValue.Value;
            if (!PromptRange(Strings.SelectDateRange, ref startDate, ref endDate))
                return;
            PrintSaleTotalsByCategory(startDate, endDate);
        }

        [Obfuscation(Exclude = true)]
        private void buttonSalesTotalsByEmployee_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = SqlDateTime.MinValue.Value;
            DateTime endDate = SqlDateTime.MaxValue.Value;
            if (!PromptRange(Strings.SelectDateRange, ref startDate, ref endDate))
                return;
            PrintSalesByEmployeeReport(startDate, endDate);
        }
        
        [Obfuscation(Exclude = true)]
        private void buttonEmployeeSalesTotalsByItem_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = SqlDateTime.MinValue.Value;
            DateTime endDate = SqlDateTime.MaxValue.Value;
            if (!PromptRange(Strings.SelectDateRange, ref startDate, ref endDate))
                return;
            PrintEmployeeSalesByItem(startDate, endDate);
        }

        [Obfuscation(Exclude = true)]
        private void buttonEmployeeSalesTotalsByCategory_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = SqlDateTime.MinValue.Value;
            DateTime endDate = SqlDateTime.MaxValue.Value;
            if (!PromptRange(Strings.SelectDateRange, ref startDate, ref endDate))
                return;
            PrintEmployeeSalesByCategory(startDate, endDate);
        }

        [Obfuscation(Exclude = true)]
        private void buttonLaborEmployeeHours_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = SqlDateTime.MinValue.Value;
            DateTime endDate = SqlDateTime.MaxValue.Value;
            if (!PromptRange(Strings.SelectDateRange, ref startDate, ref endDate))
                return;
            PrintLaborEmployeeHoursReport(startDate, endDate);
        }

        [Obfuscation(Exclude = true)]
        private void buttonLaborHourlyTotals_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = SqlDateTime.MinValue.Value;
            DateTime endDate = SqlDateTime.MaxValue.Value;
            if (!PromptRange(Strings.SelectDateRange, ref startDate, ref endDate))
                return;
            PrintLaborHourlyTotalsReport(startDate, endDate);
        }

        [Obfuscation(Exclude = true)]
        private void buttonLaborTimesheetChanges_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = SqlDateTime.MinValue.Value;
            DateTime endDate = SqlDateTime.MaxValue.Value;
            if (!PromptRange(Strings.SelectDateRange, ref startDate, ref endDate))
                return;
            PrintLaborTimesheetChangesReport(startDate, endDate);
        }

        [Obfuscation(Exclude = true)]
        private void buttonCancels_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = SqlDateTime.MinValue.Value;
            DateTime endDate = SqlDateTime.MaxValue.Value;
            if (!PromptRange(Strings.SelectDateRange, ref startDate, ref endDate))
                return;
            PrintCancels(startDate, endDate);
        }

        [Obfuscation(Exclude = true)]
        private void buttonVoids_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = SqlDateTime.MinValue.Value;
            DateTime endDate = SqlDateTime.MaxValue.Value;
            if (!PromptRange(Strings.SelectDateRange, ref startDate, ref endDate))
                return;
            PrintAdminisrtrativeVoidReport(startDate, endDate);
        }

        [Obfuscation(Exclude = true)]
        private void buttonReturns_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = SqlDateTime.MinValue.Value;
            DateTime endDate = SqlDateTime.MaxValue.Value;
            if (!PromptRange(Strings.SelectDateRange, ref startDate, ref endDate))
                return;
            PrintReturns(startDate, endDate);
        }

        [Obfuscation(Exclude = true)]
        private void buttonRefunds_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = SqlDateTime.MinValue.Value;
            DateTime endDate = SqlDateTime.MaxValue.Value;
            if (!PromptRange(Strings.SelectDateRange, ref startDate, ref endDate))
                return;
            PrintRefunds(startDate, endDate);
        }

        [Obfuscation(Exclude = true)]
        private void buttonNoSales_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = SqlDateTime.MinValue.Value;
            DateTime endDate = SqlDateTime.MaxValue.Value;
            if (!PromptRange(Strings.SelectDateRange, ref startDate, ref endDate))
                return;
            PrintNoSales(startDate, endDate);
        }

        [Obfuscation(Exclude = true)]
        private void buttonPayouts_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = SqlDateTime.MinValue.Value;
            DateTime endDate = SqlDateTime.MaxValue.Value;
            if (!PromptRange(Strings.SelectDateRange, ref startDate, ref endDate))
                return;
            PrintPayouts(startDate, endDate);
        }

        [Obfuscation(Exclude = true)]
        private void buttonSafeDrops_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = SqlDateTime.MinValue.Value;
            DateTime endDate = SqlDateTime.MaxValue.Value;
            if (!PromptRange(Strings.SelectDateRange, ref startDate, ref endDate))
                return;
            PrintSafeDrops(startDate, endDate);
        }

        [Obfuscation(Exclude = true)]
        private void buttonRegisterDeposits_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = SqlDateTime.MinValue.Value;
            DateTime endDate = SqlDateTime.MaxValue.Value;
            if (!PromptRange(Strings.SelectDateRange, ref startDate, ref endDate))
                return;
            PrintDeposits(startDate, endDate);
        }

        [Obfuscation(Exclude = true)]
        private void buttonRegisterFloatingDocking_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = SqlDateTime.MinValue.Value;
            DateTime endDate = SqlDateTime.MaxValue.Value;
            if (!PromptRange(Strings.SelectDateRange, ref startDate, ref endDate))
                return;
            PrintFloatingDocking(startDate, endDate);
        }

        [Obfuscation(Exclude = true)]
        private void buttonInventoryWasteByItem_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = SqlDateTime.MinValue.Value;
            DateTime endDate = SqlDateTime.MaxValue.Value;
            if (!PromptRange(Strings.SelectDateRange, ref startDate, ref endDate))
                return;
            PrintWasteByItem(startDate, endDate);
        }

        [Obfuscation(Exclude = true)]
        private void buttonInventoryWasteByCategory_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = SqlDateTime.MinValue.Value;
            DateTime endDate = SqlDateTime.MaxValue.Value;
            if (!PromptRange(Strings.SelectDateRange, ref startDate, ref endDate))
                return;
            PrintWasteByCategory(startDate, endDate);
        }

        [Obfuscation(Exclude = true)]
        private void buttonInventoryWasteByIngredient_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = SqlDateTime.MinValue.Value;
            DateTime endDate = SqlDateTime.MaxValue.Value;
            if (!PromptRange(Strings.SelectDateRange, ref startDate, ref endDate))
                return;
            PrintWasteByIngredient(startDate, endDate);
        }

        [Obfuscation(Exclude = true)]
        private void buttonInventoryWasteByHours_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = SqlDateTime.MinValue.Value;
            DateTime endDate = SqlDateTime.MaxValue.Value;
            if (!PromptRange(Strings.SelectDateRange, ref startDate, ref endDate))
                return;
            PrintWasteByHours(startDate, endDate);
        }

        [Obfuscation(Exclude = true)]
        private void buttonInventoryUsageByIngredient_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = SqlDateTime.MinValue.Value;
            DateTime endDate = SqlDateTime.MaxValue.Value;
            if (!PromptRange(Strings.SelectDateRange, ref startDate, ref endDate))
                return;
            PrintUsageByIngredient(startDate, endDate);
        }

        [Obfuscation(Exclude = true)]
        private void buttonInventoryCurrentInventory_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = SqlDateTime.MinValue.Value;
            DateTime endDate = SqlDateTime.MaxValue.Value;
            // Note: startDate and endDate are ignored, as this is always getting
            //       the current value.
            PrintCurrentInventory(startDate, endDate);
        }

        [Obfuscation(Exclude = true)]
        private void buttonLogInventoryAdjustments_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = SqlDateTime.MinValue.Value;
            DateTime endDate = SqlDateTime.MaxValue.Value;
            if (!PromptRange(Strings.SelectDateRange, ref startDate, ref endDate))
                return;
            PrintInventoryAdjustments(startDate, endDate);
        }

        [Obfuscation(Exclude = true)]
        private void buttonLogIngredientSetChanges_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = SqlDateTime.MinValue.Value;
            DateTime endDate = SqlDateTime.MaxValue.Value;
            if (!PromptRange(Strings.SelectDateRange, ref startDate, ref endDate))
                return;
            PrintIngredientSetChanges(startDate, endDate);
        }

        [Obfuscation(Exclude = true)]
        private void buttonLogItemIngredientChanges_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = SqlDateTime.MinValue.Value;
            DateTime endDate = SqlDateTime.MaxValue.Value;
            if (!PromptRange(Strings.SelectDateRange, ref startDate, ref endDate))
                return;
            PrintItemIngredientChanges(startDate, endDate);
        }

        [Obfuscation(Exclude = true)]
        private void buttonLogPricingChanges_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = SqlDateTime.MinValue.Value;
            DateTime endDate = SqlDateTime.MaxValue.Value;
            if (!PromptRange(Strings.SelectDateRange, ref startDate, ref endDate))
                return;
            PrintPricingChanges(startDate, endDate);
        }

        [Obfuscation(Exclude = true)]
        private void buttonLogItemDiscontinue_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = SqlDateTime.MinValue.Value;
            DateTime endDate = SqlDateTime.MaxValue.Value;
            if (!PromptRange(Strings.SelectDateRange, ref startDate, ref endDate))
                return;
            PrintItemDiscontinuation(startDate, endDate);
        }
        #endregion

        public static PosDialogWindow CreateInDefaultWindow()
        {
            var control = new ReportsMenuControl();
            return new PosDialogWindow(control, Strings.ReportsMenu, 770, 590);
        }

    }

    public enum RangeType
    {
        AllDates,
        DateRange,
        DayOfOperationRange,
        MonthToDate,
        None,
        TodayOnly,
        YearToDay
    }
}
