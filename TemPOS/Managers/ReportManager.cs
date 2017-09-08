using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using PosControls;
using PosModels;
using PosModels.Managers;
using TemPOS.Types;
using PosModels.Types;
using PosModels.Helpers;
using TemPOS.Helpers;
using Strings = TemPOS.Types.Strings;

namespace TemPOS.Managers
{
    public static class ReportManager
    {
        #region Licensed Access Only
        static ReportManager()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(ReportManager).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to link to this application");
            }
#endif
        }
        #endregion

        #region End-of-Day Report
        public static void PrintEndOfDay(DayOfOperation today,
            EventHandler windowClosedEventHandler = null)
        {
            PosDialogWindow window = ReportViewerControl.CreateInDefaultWindow(
                Types.Strings.EndOfDayReport);
            var control = window.DockedControl as ReportViewerControl;
            if (control == null) return;
            if (windowClosedEventHandler != null)
            {
                if (!today.EndOfDay.HasValue)
                {
                    windowClosedEventHandler(window, new EventArgs());
                    return;
                }
                window.Closed += windowClosedEventHandler;
            }
            if (!today.EndOfDay.HasValue) return;
            Person person = PersonManager.GetPerson(Employee.GetPersonId(SessionManager.ActiveEmployee.Id));
            var todaysTickets = new List<Ticket>(TicketManager.GetRange(today.StartOfDay, today.EndOfDay.Value));
            var nonCanceledTickets = new List<Ticket>(TicketManager.FilterByCanceled(todaysTickets, false));
            var payments = new List<TicketPayment>(TicketPayment.GetAllForTickets(nonCanceledTickets));
            var payouts = new List<RegisterPayout>(RegisterPayout.GetAll(today.StartOfDay, today.EndOfDay.Value));
            var refunds = new List<TicketRefund>(TicketRefund.GetAll(today.StartOfDay, today.EndOfDay.Value));
            var returns = new List<TicketItemReturn>(TicketItemReturn.GetAll(today.StartOfDay, today.EndOfDay.Value));

            double[] sums = TicketManager.GetSumOfTickets(nonCanceledTickets);
            //double totalSales = sums[0];
            double taxExemptSalesTotal = sums[1];
            double couponTotal = TicketManager.GetSumOfCoupons(nonCanceledTickets);
            double discountTotal = TicketManager.GetSumOfDiscounts(nonCanceledTickets);
            double taxes = TicketManager.GetSumOfTaxes(nonCanceledTickets);
            double payoutTotal = PosHelper.GetSumOfPayouts(payouts);
            double refundAmount = PosHelper.GetSumOfRefunds(refunds);
            double returnAmount = PosHelper.GetSumOfReturns(returns);
            double amountOfSales = PosHelper.GetSumOfPayments(payments);
            // ToDo: Use these
            //double amountOfSalesInCash = PosHelper.GetSumOfPayments(payments, PaymentSource.Cash);
            //double amountOfSalesInCredit = PosHelper.GetSumOfPayments(payments, PaymentSource.Credit);
            //double amountOfSalesInCheck = PosHelper.GetSumOfPayments(payments, PaymentSource.Check);
            //double amountOfSalesInGiftCard = PosHelper.GetSumOfPayments(payments, PaymentSource.GiftCard);

            // Header
            control.DefineTable(1);
            control.PrintTableLine(Types.Strings.EndOfDayReport + Environment.NewLine);
            control.PrintTableLine(Types.Strings.Employee + ": " + person.LastName + ", " +
                person.FirstName);
            control.PrintTableLine(Types.Strings.StartOfDay + ": " + today.StartOfDay);
            control.PrintTableLine(Types.Strings.EndOfDay + ": " + today.EndOfDay.Value);

            // Report
            control.DefineTable(2);
            control.PrintTableLine(Types.Strings.TotalGrossSales +
                PrinterManager.FixedNumberOfSpaces(20) + amountOfSales.ToString("C2"));
            control.PrintTableLine(Types.Strings.TaxCollected +
                PrinterManager.FixedNumberOfSpaces(20) + taxes.ToString("C2"));
            control.PrintTableLine(Types.Strings.TaxExemptSalesTotal +
                PrinterManager.FixedNumberOfSpaces(20) + taxExemptSalesTotal.ToString("C2") +
                Environment.NewLine);
            control.PrintTableLine(Types.Strings.NumberOfTickets +
                PrinterManager.FixedNumberOfSpaces(20) + nonCanceledTickets.Count());
            control.PrintTableLine(Types.Strings.TotalOfCoupons +
                PrinterManager.FixedNumberOfSpaces(20) + couponTotal.ToString("C2"));
            control.PrintTableLine(Types.Strings.TotalOfDiscounts +
                PrinterManager.FixedNumberOfSpaces(20) + discountTotal.ToString("C2"));
            control.PrintTableLine(Types.Strings.TotalOfRefunds +
                PrinterManager.FixedNumberOfSpaces(20) + refundAmount.ToString("C2"));
            control.PrintTableLine(Types.Strings.TotalOfReturns +
                PrinterManager.FixedNumberOfSpaces(20) + returnAmount.ToString("C2"));
            control.PrintTableLine(Types.Strings.TotalOfPayouts +
                PrinterManager.FixedNumberOfSpaces(20) + payoutTotal.ToString("C2"));

            window.Topmost = true;
            window.IsClosable = true;
            window.Show();
        }
        #endregion

        #region End-Of-Year Report
        public static void PrintEndOfYear(short year, EventHandler windowClosedEventHandler = null)
        {
            PosDialogWindow window = ReportViewerControl.CreateInDefaultWindow(
                Types.Strings.EndOfYearReport);
            var control = window.DockedControl as ReportViewerControl;
            if (control == null) return;
            if (windowClosedEventHandler != null)
                window.Closed += windowClosedEventHandler;
            Person person = PersonManager.GetPerson(Employee.GetPersonId(SessionManager.ActiveEmployee.Id));
            DayOfOperation firstOfYear = DayOfOperation.GetEarliestInYear(year);
            DayOfOperation lastOfYear = DayOfOperation.GetLatestInYear(year);
            if (!lastOfYear.EndOfDay.HasValue) return;

            DateTime startRange = firstOfYear.StartOfDay;
            DateTime endRange = lastOfYear.EndOfDay.Value;
            var thisYearsTickets = new List<Ticket>(TicketManager.GetRange(startRange, endRange));
            var nonCanceledTickets = new List<Ticket>(TicketManager.FilterByCanceled(thisYearsTickets, false));
            var payments = new List<TicketPayment>(TicketPayment.GetAllForTickets(nonCanceledTickets));
            var payouts = new List<RegisterPayout>(RegisterPayout.GetAll(startRange, endRange));
            var refunds = new List<TicketRefund>(TicketRefund.GetAll(startRange, endRange));
            var returns = new List<TicketItemReturn>(TicketItemReturn.GetAll(startRange, endRange));

            double[] sums = TicketManager.GetSumOfTickets(nonCanceledTickets);
            //double totalSales = sums[0];
            double taxExemptSalesTotal = sums[1];
            double couponTotal = TicketManager.GetSumOfCoupons(nonCanceledTickets);
            double discountTotal = TicketManager.GetSumOfDiscounts(nonCanceledTickets);
            double taxes = TicketManager.GetSumOfTaxes(nonCanceledTickets);
            double payoutTotal = PosHelper.GetSumOfPayouts(payouts);
            double refundAmount = PosHelper.GetSumOfRefunds(refunds);
            double returnAmount = PosHelper.GetSumOfReturns(returns);
            double amountOfSales = PosHelper.GetSumOfPayments(payments);
            // ToDo: Use these
            //double amountOfSalesInCash = PosHelper.GetSumOfPayments(payments, PaymentSource.Cash);
            //double amountOfSalesInCredit = PosHelper.GetSumOfPayments(payments, PaymentSource.Credit);
            //double amountOfSalesInCheck = PosHelper.GetSumOfPayments(payments, PaymentSource.Check);
            //double amountOfSalesInGiftCard = PosHelper.GetSumOfPayments(payments, PaymentSource.GiftCard);

            // Header
            control.DefineTable(1);
            control.PrintTableLine(Types.Strings.EndOfYearReport + Environment.NewLine);
            control.PrintTableLine(Types.Strings.Employee + ": " + person.LastName + ", " +
                person.FirstName);

            // Report
            control.DefineTable(2);
            control.PrintTableLine(Types.Strings.TotalGrossSales +
                PrinterManager.FixedNumberOfSpaces(20) + amountOfSales.ToString("C2"));
            control.PrintTableLine(Types.Strings.TaxCollected +
                PrinterManager.FixedNumberOfSpaces(20) + taxes.ToString("C2"));
            control.PrintTableLine(Types.Strings.TaxExemptSalesTotal +
                PrinterManager.FixedNumberOfSpaces(20) + taxExemptSalesTotal.ToString("C2") +
                Environment.NewLine);
            control.PrintTableLine(Types.Strings.NumberOfTickets +
                PrinterManager.FixedNumberOfSpaces(20) + nonCanceledTickets.Count());
            control.PrintTableLine(Types.Strings.TotalOfCoupons +
                PrinterManager.FixedNumberOfSpaces(20) + couponTotal.ToString("C2"));
            control.PrintTableLine(Types.Strings.TotalOfDiscounts +
                PrinterManager.FixedNumberOfSpaces(20) + discountTotal.ToString("C2"));
            control.PrintTableLine(Types.Strings.TotalOfRefunds +
                PrinterManager.FixedNumberOfSpaces(20) + refundAmount.ToString("C2"));
            control.PrintTableLine(Types.Strings.TotalOfReturns +
                PrinterManager.FixedNumberOfSpaces(20) + returnAmount.ToString("C2"));
            control.PrintTableLine(Types.Strings.TotalOfPayouts +
                PrinterManager.FixedNumberOfSpaces(20) + payoutTotal.ToString("C2"));

            window.Topmost = true;
            window.IsClosable = true;
            window.Show();
        }
        #endregion

        #region Total Sales Reports
        public static void PrintSaleTotalsByItem(DateTime startDate, DateTime endDate,
            EventHandler windowClosedEventHandler = null)
        {
            PosDialogWindow window = ReportViewerControl.CreateInDefaultWindow(
                Types.Strings.TotalSalesByItem);
            var control = window.DockedControl as ReportViewerControl;
            if (windowClosedEventHandler != null)
                window.Closed += windowClosedEventHandler;

            PrintSaleTotalsByItem(startDate, endDate, control);

            window.Topmost = true;
            window.IsClosable = true;
            window.Show();
        }

        private static void PrintSaleTotalsByItem(DateTime startDate, DateTime endDate,
            ReportViewerControl control, Employee employee = null)
        {
            Person person = null;
            if (employee != null)
                person = Person.Get(Employee.GetPersonId(employee.Id));

            control.DefineTable(1);
            if (employee == null)
                control.PrintTableLine(Types.Strings.TotalSalesByItem);
            else
                control.PrintTableLine(Types.Strings.TotalSalesByItemFor + person.LastName + ", " +
                    person.FirstName + "'");
            control.PrintTableLine(Types.Strings.From + startDate);
            control.PrintTableLine(Types.Strings.To + endDate);
            control.DefineTable(new[] {
                    new GridLength(3, GridUnitType.Star),
                    new GridLength(1.25, GridUnitType.Star),
                    new GridLength(1.25, GridUnitType.Star),
                    new GridLength(1.25, GridUnitType.Star),
                    new GridLength(0, GridUnitType.Star)
                });

            control.PrintTableLine(
                Types.Strings.ItemQuantitySalesRevenueIngredientCost,
                0, true);

            double grossSales = 0;
            var countsPerItemIds = new Dictionary<int, int>();
            var salesPerItemIds = new Dictionary<int, double>();
            foreach (TicketItem ticketItem in
                (employee == null) 
                ? TicketItem.GetAll(startDate, endDate) 
                : TicketItem.GetAll(startDate, endDate, employee.Id))
            {
                double totalCost = ticketItem.GetTotalCost();
                grossSales += totalCost;
                if (!countsPerItemIds.Keys.Contains(ticketItem.ItemId))
                {
                    countsPerItemIds.Add(ticketItem.ItemId, ticketItem.Quantity);
                    salesPerItemIds.Add(ticketItem.ItemId, totalCost);
                }
                else
                {
                    countsPerItemIds[ticketItem.ItemId] += ticketItem.Quantity;
                    salesPerItemIds[ticketItem.ItemId] += totalCost;
                }
            }
            foreach (int itemId in countsPerItemIds.Keys)
            {
                Item item = Item.Get(itemId);
                control.PrintTableLine(item.FullName + "   " +
                    countsPerItemIds[itemId] + "   " +
                    salesPerItemIds[itemId].ToString("C2") + "   " +
                    (item.GetCostOfIngredients() * countsPerItemIds[itemId]).ToString("C2"));
            }
            control.DefineTable(new [] {
                    new GridLength(2, GridUnitType.Star),
                    new GridLength(3, GridUnitType.Star)});
            control.PrintTableLine(Types.Strings.TotalOfGrossSales + grossSales.ToString("C2"));
        }

        public static void PrintSaleTotalsByCategory(DateTime startDate, DateTime endDate,
            EventHandler windowClosedEventHandler = null)
        {
            PosDialogWindow window = ReportViewerControl.CreateInDefaultWindow(
                Types.Strings.TotalSalesByCategory);
            var control = window.DockedControl as ReportViewerControl;
            if (windowClosedEventHandler != null)
                window.Closed += windowClosedEventHandler;

            PrintSaleTotalsByCategory(startDate, endDate, control);

            window.Topmost = true;
            window.IsClosable = true;
            window.Show();
        }

        private static void PrintSaleTotalsByCategory(DateTime startDate, DateTime endDate,
            ReportViewerControl control, Employee employee = null)
        {
            Person person = null;
            if (employee != null)
                person = Person.Get(Employee.GetPersonId(employee.Id));

            control.DefineTable(1);
            if (employee == null)
                control.PrintTableLine(Types.Strings.TotalSalesByCategory);
            else
                control.PrintTableLine(Types.Strings.TotalSalesByCategoryFor + person.LastName + ", " +
                    person.FirstName + "'");
            control.PrintTableLine(Types.Strings.From + startDate);
            control.PrintTableLine(Types.Strings.To + endDate);

            control.DefineTable(new [] {
                    new GridLength(3, GridUnitType.Star),
                    new GridLength(1, GridUnitType.Star),
                    new GridLength(1, GridUnitType.Star)
                });

            control.PrintTableLine(
                Types.Strings.CategoryQuantitySalesRevenue,
                0, true);

            double grossSales = 0;
            var countsPerCategoryIds = new Dictionary<int, int>();
            var salesPerCategoryIds = new Dictionary<int, double>();
            foreach (TicketItem ticketItem in
                (employee == null)
                ? TicketItem.GetAll(startDate, endDate) 
                : TicketItem.GetAll(startDate, endDate, employee.Id))
            {
                int categoryId = Item.GetCategoryId(ticketItem.ItemId);
                double totalCost = ticketItem.GetTotalCost();
                grossSales += totalCost;
                if (!countsPerCategoryIds.Keys.Contains(categoryId))
                {
                    countsPerCategoryIds.Add(categoryId, ticketItem.Quantity);
                    salesPerCategoryIds.Add(categoryId, ticketItem.GetTotalCost());
                }
                else
                {
                    countsPerCategoryIds[categoryId] += ticketItem.Quantity;
                    salesPerCategoryIds[categoryId] += ticketItem.GetTotalCost();
                }
            }
            foreach (int categoryId in countsPerCategoryIds.Keys)
            {
                Category category = Category.Get(categoryId);
                control.PrintTableLine(
                    category.NameValue + "   " +                    
                    countsPerCategoryIds[categoryId] + "   " +
                    salesPerCategoryIds[categoryId].ToString("C2"));
            }
            control.DefineTable(new [] {
                    new GridLength(2, GridUnitType.Star),
                    new GridLength(3, GridUnitType.Star)});
            control.PrintTableLine(Types.Strings.TotalOfGrossSales + grossSales.ToString("C2"));
        }
        #endregion

        #region Sales by Employee Reports
        public static void PrintSalesByEmployee(DateTime startDate, DateTime endDate,
            EventHandler windowClosedEventHandler = null)
        {
            PosDialogWindow window = ReportViewerControl.CreateInDefaultWindow(
                Types.Strings.TotalSalesByEmployee);
            var control = window.DockedControl as ReportViewerControl;
            if (control == null) return;
            if (windowClosedEventHandler != null)
                window.Closed += windowClosedEventHandler;

            control.DefineTable(1);
            control.PrintTableLine(Types.Strings.TotalSalesByEmployee);
            control.PrintTableLine(Types.Strings.From + startDate);
            control.PrintTableLine(Types.Strings.To + endDate);
            control.DefineTable(2);

            control.PrintTableLine(
                Types.Strings.EmployeeSalesRevenue,
                0, true);

            var employeeGrossSales = new Dictionary<int, double>();
            foreach (Ticket ticket in TicketManager.GetRange(startDate, endDate))
            {
                if (!employeeGrossSales.Keys.Contains(ticket.EmployeeId))
                    employeeGrossSales.Add(ticket.EmployeeId, ticket.GetSubTotal());
                else
                    employeeGrossSales[ticket.EmployeeId] += ticket.GetSubTotal();
            }
            foreach (int employeeId in employeeGrossSales.Keys)
            {
                Person person = PersonManager.GetPerson(Employee.GetPersonId(employeeId));
                string name = person.FirstName + " " + person.LastName;
                control.PrintTableLine(name + "   " +
                    employeeGrossSales[employeeId].ToString("C2"));
            }

            window.Topmost = true;
            window.IsClosable = true;
            window.Show();
        }

        public static void PrintEmployeeSalesByCategory(DateTime startDate, DateTime endDate,
            EventHandler windowClosedEventHandler = null)
        {
            PosDialogWindow window = ReportViewerControl.CreateInDefaultWindow(
                Types.Strings.EmployeeSalesByCategory);
            var control = window.DockedControl as ReportViewerControl;
            if (control == null) return;
            if (windowClosedEventHandler != null)
                window.Closed += windowClosedEventHandler;

            control.ShowEmployeeList = true;
            control.StartDate = startDate;
            control.EndDate = endDate;
            control.SelectedEmployeeChanged += PrintEmployeeSalesByCategory_SelectedEmployeeChanged;
            PrintSaleTotalsByCategory(startDate, endDate, control, control.SelectedEmployee);

            window.Topmost = true;
            window.IsClosable = true;
            window.Show();
        }

        static void PrintEmployeeSalesByCategory_SelectedEmployeeChanged(object sender, EventArgs e)
        {
            var control = sender as ReportViewerControl;
            if (control == null) return;
            control.ClearDocument();
            PrintSaleTotalsByCategory(control.StartDate, control.EndDate, control, control.SelectedEmployee);
        }

        public static void PrintEmployeeSalesByItem(DateTime startDate, DateTime endDate,
            EventHandler windowClosedEventHandler = null)
        {
            PosDialogWindow window = ReportViewerControl.CreateInDefaultWindow(
                Types.Strings.EmployeeSalesByItem);
            var control = window.DockedControl as ReportViewerControl;
            if (control == null) return;
            if (windowClosedEventHandler != null)
                window.Closed += windowClosedEventHandler;

            control.ShowEmployeeList = true;
            control.StartDate = startDate;
            control.EndDate = endDate;
            control.SelectedEmployeeChanged += PrintEmployeeSalesByItem_SelectedEmployeeChanged;
            PrintSaleTotalsByItem(startDate, endDate, control, control.SelectedEmployee);

            window.Topmost = true;
            window.IsClosable = true;
            window.Show();
        }

        static void PrintEmployeeSalesByItem_SelectedEmployeeChanged(object sender, EventArgs e)
        {
            var control = sender as ReportViewerControl;
            if (control == null) return;
            control.ClearDocument();
            PrintSaleTotalsByItem(control.StartDate, control.EndDate, control, control.SelectedEmployee);
        }
        #endregion

        #region Labor Reports
        public static void PrintLaborEmployeeHours(DateTime startDate, DateTime endDate,
            EventHandler windowClosedEventHandler = null)
        {
            PosDialogWindow window = ReportViewerControl.CreateInDefaultWindow(
                Types.Strings.EmployeeHours);
            var control = window.DockedControl as ReportViewerControl;
            if (control == null) return;
            if (windowClosedEventHandler != null)
                window.Closed += windowClosedEventHandler;

            control.DefineTable(1);
            control.PrintTableLine(Types.Strings.EmployeeHours);
            control.PrintTableLine(Types.Strings.From + startDate);
            control.PrintTableLine(Types.Strings.To + endDate);

            control.DefineTable(new [] {
                    new GridLength(3, GridUnitType.Star),
                    new GridLength(1, GridUnitType.Star),
                    new GridLength(2, GridUnitType.Star)
                });

            control.PrintTableLine(Types.Strings.EmployeeJobHours, 0, true);

            var employeeHours = new Dictionary<int, JobHourTotal>();
            var oneDay = new TimeSpan(24, 0, 0);
            foreach (EmployeeTimesheet employeeTimesheet in
                EmployeeTimesheet.GetAll(startDate - oneDay, endDate))
            {
                DateTime startTime = employeeTimesheet.StartTime;
                DateTime endTime = DateTime.Now;
                if (employeeTimesheet.EndTime != null)
                    endTime = employeeTimesheet.EndTime.Value;

                // Ignore records with end times on the startDate - oneDay
                if (endTime.Year != startDate.Year)
                    continue;
                if (endTime.DayOfYear < startDate.DayOfYear)
                    continue;

                // Ignore times from the startDate - oneDay
                if ((startTime.Year != startDate.Year) ||
                    (startTime.DayOfYear < startDate.DayOfYear))
                {
                    startTime = startDate;
                }

                TimeSpan timeSpan = endTime - startTime;
                double hours = (timeSpan.Hours + (timeSpan.Minutes / 60.0f) +
                                ((timeSpan.Seconds >= 30) ? 1/60 : 0));

                if (!employeeHours.Keys.Contains(employeeTimesheet.EmployeeId))
                {
                    var jobHourTotal = new JobHourTotal();
                    jobHourTotal.AddHours(employeeTimesheet.JobId, hours);
                    employeeHours.Add(employeeTimesheet.EmployeeId, jobHourTotal);
                }
                else
                {
                    employeeHours[employeeTimesheet.EmployeeId].
                        AddHours(employeeTimesheet.JobId, hours);
                }
            }
            foreach (int employeeId in employeeHours.Keys)
            {
                Person person = PersonManager.GetPerson(Employee.GetPersonId(employeeId));
                string name = person.FirstName + " " + person.LastName;
                bool first = true;
                bool wasMore = false;
                foreach (int jobId in employeeHours[employeeId].GetJobIds())
                {
                    EmployeeJob job = EmployeeJob.Get(jobId);
                    if (first)
                    {
                        control.PrintTableLine(name + "  " +
                            job.Description + "  " +
                            employeeHours[employeeId].GetJobHours(jobId).ToString("F2"));
                        first = false;
                    }
                    else
                    {
                        control.PrintTableLine(PrinterManager.FixedNumberOfSpaces(50) + "  " +
                            job.Description + "  " +
                            employeeHours[employeeId].GetJobHours(jobId).ToString("F2"), 1);
                        wasMore = true;
                    }
                }
                if (wasMore)
                {
                    control.PrintTableLine(PrinterManager.FixedNumberOfSpaces(50) + "  " +
                        Types.Strings.Total + "  " +
                        employeeHours[employeeId].GetTotalHours().ToString("F2"), 1);
                }
            }

            window.Topmost = true;
            window.IsClosable = true;
            window.Show();
        }

        private class JobHourTotal
        {
            private readonly Dictionary<int, double> _jobHourCounts = new Dictionary<int, double>();
            public IEnumerable<int> GetJobIds()
            {
                return _jobHourCounts.Keys;
            }
            public double GetTotalHours()
            {
                return _jobHourCounts.Keys.Sum(jobId => _jobHourCounts[jobId]);
            }
            public double GetJobHours(int jobId)
            {
                return _jobHourCounts[jobId];
            }
            public void AddHours(int jobId, double hours)
            {
                if (!_jobHourCounts.Keys.Contains(jobId))
                    _jobHourCounts.Add(jobId, hours);
                else
                    _jobHourCounts[jobId] += hours;
            }
        }

        public static void PrintLaborHourlyTotals(DateTime startDate, DateTime endDate,
            EventHandler windowClosedEventHandler = null)
        {
            PosDialogWindow window = ReportViewerControl.CreateInDefaultWindow(
                Types.Strings.HourlyLaborTotals);
            var control = window.DockedControl as ReportViewerControl;
            if (control == null) return;
            if (windowClosedEventHandler != null)
                window.Closed += windowClosedEventHandler;

            control.DefineTable(1);
            control.PrintTableLine(Types.Strings.HourlyLaborTotal);
            control.PrintTableLine(Types.Strings.From + startDate);
            control.PrintTableLine(Types.Strings.To + endDate);
            control.DefineTable(3);

            control.PrintTableLine(Types.Strings.TimeOfDayLaborHoursCostOfLabor, 0, true);

            var hourlyTotals = new Dictionary<int, double>();
            var hourlyCostTotals = new Dictionary<int, double>();
            for (int i = 0; i <= 23; i++)
            {
                hourlyTotals.Add(i, 0);
                hourlyCostTotals.Add(i, 0);
            }

            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day,
                startDate.Hour, startDate.Minute, 0);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day,
                endDate.Hour, endDate.Minute, 0);

            var oneDay = new TimeSpan(24, 0, 0);
            foreach (EmployeeTimesheet timeSheet in EmployeeTimesheet.GetAll(startDate - oneDay, endDate))
            {
                DateTime startTime = timeSheet.StartTime;
                DateTime endTime = DateTime.Now;
                if (timeSheet.EndTime != null)
                    endTime = timeSheet.EndTime.Value;

                // Ignore records with end times on the startDate - oneDay
                if (endTime.Year != startDate.Year)
                    continue;
                if (endTime.DayOfYear < startDate.DayOfYear)
                    continue;

                // Ignore times from the startDate - oneDay
                if ((startTime.Year != startDate.Year) ||
                    (startTime.DayOfYear < startDate.DayOfYear))
                {
                    startTime = startDate;
                }

                EmployeePayRate pay = EmployeePayRate.GetEmployeePayRateForJob(timeSheet.EmployeeId, timeSheet.JobId);
                DateTime currentTestStart = startTime;
                while (true)
                {
                    var endOfHour = new DateTime(currentTestStart.Year,
                        currentTestStart.Month, currentTestStart.Day,
                        currentTestStart.Hour, 59, 59);
                    if (endOfHour > endTime)
                        endOfHour = endTime;
                    TimeSpan timeSpan = endOfHour - currentTestStart;
                    double totalHours = timeSpan.TotalHours;
                    if (totalHours > 0.97)
                        totalHours = 1;
                    hourlyTotals[currentTestStart.Hour] += totalHours;
                    hourlyCostTotals[currentTestStart.Hour] += (pay.Wage * totalHours);
                    currentTestStart = endOfHour.AddSeconds(1);
                    if (currentTestStart > endTime)
                        break;
                }
            }
                        
            for (int i = 0; i <= 23; i++)
            {
                control.PrintTableLine(i + ":00 to " + i + ":59" +
                    PrinterManager.FixedNumberOfSpaces(30) +
                    hourlyTotals[i].ToString("F1") + "   " +
                    hourlyCostTotals[i].ToString("C2"));
            }

            window.Topmost = true;
            window.IsClosable = true;
            window.Show();
        }

        public static void PrintLaborTimesheetChanges(DateTime startDate, DateTime endDate,
            EventHandler windowClosedEventHandler = null)
        {
            PosDialogWindow window = ReportViewerControl.CreateInDefaultWindow(
                Types.Strings.TimesheetChangeLog);
            var control = window.DockedControl as ReportViewerControl;
            if (control == null) return;
            if (windowClosedEventHandler != null)
                window.Closed += windowClosedEventHandler;

            control.DefineTable(1);
            control.PrintTableLine(Types.Strings.EmployeeTimesheetChangeLog);
            control.PrintTableLine(Types.Strings.From + startDate);
            control.PrintTableLine(Types.Strings.To + endDate);
            control.DefineTable(2);
            foreach (EmployeeTimesheetChangeLog change in
                EmployeeTimesheetChangeLog.GetAll(startDate, endDate))
            {
                Person managerEmployee = PersonManager.GetPerson(Employee.GetPersonId(change.EmployeeId));
                EmployeeTimesheet timeSheet = EmployeeTimesheet.Get(change.EmployeeTimesheetId);
                Person timesheetEmployee = PersonManager.GetPerson(Employee.GetPersonId(timeSheet.EmployeeId));
                if ((change.OldStartTime == null) &&
                    (change.OldEndTime == null) &&
                    (change.OldJob == null)  &&
                    (change.OldTips == null) &&
                    (change.OldDriverCompensation == null))
                {
                    EmployeeJob job = EmployeeJob.Get(timeSheet.JobId);
                    control.PrintTableLine("\'" + managerEmployee.FirstName + " " + managerEmployee.LastName + Types.Strings.DeletedTimesheetEntryFor +
                        timesheetEmployee.FirstName + " " + timesheetEmployee.LastName + "\'");

                    control.PrintTableLine(Types.Strings.ToriginalStartTimeWas + timeSheet.StartTime + "\'");
                    if (timeSheet.EndTime != null)
                        control.PrintTableLine(Types.Strings.ToriginalEndTimeWas + timeSheet.EndTime.Value + "\'");
                    control.PrintTableLine(Types.Strings.ToriginalJobWas + job.Description + "\'");
                    if (timeSheet.DeclaredTipAmount != null)
                        control.PrintTableLine(Types.Strings.ToriginalTipsWas + timeSheet.DeclaredTipAmount.Value.ToString("C2") + "\'");
                    if (timeSheet.DriverCompensationAmount != null)
                        control.PrintTableLine(Types.Strings.ToriginalDriverCompensationWas + timeSheet.DriverCompensationAmount.Value.ToString("C2") +"\'");
                }
                else
                {
                    control.PrintTableLine("\'" + managerEmployee.FirstName + " " + managerEmployee.LastName + Types.Strings.ChangedTimesheetEntryFor +
                        timesheetEmployee.FirstName + " " + timesheetEmployee.LastName + "\'");
                    control.PrintTableLine(Types.Strings.TtimesheetEntryIdIs + change.EmployeeTimesheetId + "\'");
                    if (change.OldStartTime != null)
                        control.PrintTableLine(Types.Strings.ToriginalStartTimeWas + change.OldStartTime.Value + "\'");
                    if (change.OldEndTime != null)
                        control.PrintTableLine(Types.Strings.ToriginalEndTimeWas + change.OldEndTime.Value + "\'");
                    if ((change.OldJob != null) && change.OldEndTime.HasValue)
                    {
                        EmployeeJob job = EmployeeJob.Get(change.OldJob.Value);
                        control.PrintTableLine(Types.Strings.ToriginalJobWas + job.Description + "\'");
                    }
                    if (change.OldTips != null)
                        control.PrintTableLine(Types.Strings.ToriginalTipsWas + change.OldTips.Value.ToString("C2") + "\'");
                    if (change.OldDriverCompensation != null)
                        control.PrintTableLine(Types.Strings.ToriginalDriverCompensationWas + change.OldDriverCompensation.Value.ToString("C2") + "\'");
                }
                control.PrintTableLine("");
            }
            window.Topmost = true;
            window.IsClosable = true;
            window.Show();
        }
        #endregion

        #region Ticket Transaction and Register Reports
        public static void PrintRegisterReport(RegisterDrawer drawer,
            EventHandler windowClosedEventHandler = null)
        {
            if (drawer == null)
                return;

            PosDialogWindow window = ReportViewerControl.CreateInDefaultWindow(
                Types.Strings.RegisterReport);
            var control = window.DockedControl as ReportViewerControl;
            if (control == null) return;
            if (windowClosedEventHandler != null)
                window.Closed += windowClosedEventHandler;

            Person person = PersonManager.GetPerson(Employee.GetPersonId(drawer.EmployeeId));
            var payments = new List<TicketPayment>(TicketPayment.GetAllForRegisterDrawer(drawer.Id));
            var deposits = new List<RegisterDeposit>(RegisterDeposit.GetAll(drawer.Id));
            var payouts = new List<RegisterPayout>(RegisterPayout.GetAll(drawer.Id));
            var noSales = new List<RegisterNoSale>(RegisterNoSale.GetAll(drawer.Id));
            var safeDrops = new List<RegisterDrop>(RegisterDrop.GetAll(drawer.Id));
            var refunds = new List<TicketRefund>(TicketRefund.GetAll(drawer.Id));
            var returns = new List<TicketItemReturn>(TicketItemReturn.GetAll(drawer.Id));
            var tickets = new List<Ticket>(TicketManager.GetTickets(GetTicketIds(payments)));
            var ticketCoupons = new List<TicketCoupon>(TicketCoupon.GetAllForTickets(tickets));
            var ticketDiscounts = new List<TicketDiscount>(TicketDiscount.GetAllForTickets(tickets));

            double[] sums = TicketManager.GetSumOfTickets(tickets);
            //double totalSales = sums[0];
            double taxExemptSalesTotal = sums[1];
            double couponTotal = TicketManager.GetSumOfCoupons(tickets);
            double discountTotal = TicketManager.GetSumOfDiscounts(tickets);
            double taxes = TicketManager.GetSumOfTaxes(tickets);
            double payoutTotal = PosHelper.GetSumOfPayouts(payouts);
            double depositTotal = PosHelper.GetSumOfDeposits(deposits);
            double droppedAmount = PosHelper.GetSumOfDrops(safeDrops);
            double refundAmount = PosHelper.GetSumOfRefunds(refunds);
            double returnAmount = PosHelper.GetSumOfReturns(returns);
            double amountOfSales = PosHelper.GetSumOfPayments(payments);
            // ToDo: Use these
            //double amountOfSalesInCash = PosHelper.GetSumOfPayments(payments, PaymentSource.Cash);
            //double amountOfSalesInCredit = PosHelper.GetSumOfPayments(payments, PaymentSource.Credit);
            //double amountOfSalesInCheck = PosHelper.GetSumOfPayments(payments, PaymentSource.Check);
            //double amountOfSalesInGiftCard = PosHelper.GetSumOfPayments(payments, PaymentSource.GiftCard);

            control.DefineTable(1);
            control.PrintTableLine(Types.Strings.RegisterReportForRegisterDrawer +
                drawer.Id + Environment.NewLine);
            control.PrintTableLine(Types.Strings.Employee + ": " + person.LastName + ", " +
                person.FirstName);
            control.PrintTableLine(Types.Strings.Time + DateTime.Now +
                Environment.NewLine);

            control.DefineTable(2);
            control.PrintTableLine(Types.Strings.StartingAmount +
                PrinterManager.FixedNumberOfSpaces(20) + drawer.StartAmount.ToString("C2"));
            if (drawer.EndAmount != null)
                control.PrintTableLine(Types.Strings.EndingAmount +
                    PrinterManager.FixedNumberOfSpaces(20) + drawer.EndAmount.Value.ToString("C2"));
            else
                control.PrintTableLine(Types.Strings.DrawerAmount +
                    PrinterManager.FixedNumberOfSpaces(20) + drawer.CurrentAmount.ToString("C2"));
            control.PrintTableLine(Types.Strings.DepositAmount +
                PrinterManager.FixedNumberOfSpaces(20) + depositTotal.ToString("C2"));
            control.PrintTableLine(Types.Strings.SafeDropAmount +
                PrinterManager.FixedNumberOfSpaces(20) + droppedAmount.ToString("C2"));
            control.PrintTableLine(Types.Strings.TotalGrossSales +
                PrinterManager.FixedNumberOfSpaces(20) + amountOfSales.ToString("C2"));
            control.PrintTableLine(Types.Strings.TaxCollected +
                PrinterManager.FixedNumberOfSpaces(20) + taxes.ToString("C2"));
            control.PrintTableLine(Types.Strings.TaxExemptSalesTotal +
                PrinterManager.FixedNumberOfSpaces(20) + taxExemptSalesTotal.ToString("C2") +
                Environment.NewLine);
            control.PrintTableLine(Types.Strings.NumberOfTickets +
                PrinterManager.FixedNumberOfSpaces(20) + tickets.Count());
            control.PrintTableLine(Types.Strings.NumberOfCoupons +
                PrinterManager.FixedNumberOfSpaces(20) + ticketCoupons.Count());
            control.PrintTableLine(Types.Strings.TotalOfCoupons +
                PrinterManager.FixedNumberOfSpaces(20) + couponTotal.ToString("C2"));
            control.PrintTableLine(Types.Strings.NumberOfDiscounts +
                PrinterManager.FixedNumberOfSpaces(20) + ticketDiscounts.Count());
            control.PrintTableLine(Types.Strings.TotalOfDiscounts +
                PrinterManager.FixedNumberOfSpaces(20) + discountTotal.ToString("C2"));
            control.PrintTableLine(Types.Strings.NumberOfRefunds +
                PrinterManager.FixedNumberOfSpaces(20) + refunds.Count());
            control.PrintTableLine(Types.Strings.TotalOfRefunds +
                PrinterManager.FixedNumberOfSpaces(20) + refundAmount.ToString("C2"));
            control.PrintTableLine(Types.Strings.NumberOfReturns +
                PrinterManager.FixedNumberOfSpaces(20) + returns.Count());
            control.PrintTableLine(Types.Strings.TotalOfReturns +
                PrinterManager.FixedNumberOfSpaces(20) + returnAmount.ToString("C2"));
            control.PrintTableLine(Types.Strings.NumberOfPayouts +
                PrinterManager.FixedNumberOfSpaces(20) + payouts.Count());
            control.PrintTableLine(Types.Strings.TotalOfPayouts +
                PrinterManager.FixedNumberOfSpaces(20) + payoutTotal.ToString("C2"));
            control.PrintTableLine(Types.Strings.NumberOfNoSales +
                PrinterManager.FixedNumberOfSpaces(20) + noSales.Count());

            window.Topmost = true;
            window.IsClosable = true;
            window.Show();
        }

        public static void PrintAdministrativeVoids(DateTime startDate, DateTime endDate,
            EventHandler windowClosedEventHandler = null)
        {
            PosDialogWindow window = ReportViewerControl.CreateInDefaultWindow(
                Types.Strings.VoidTransactionReport);
            var control = window.DockedControl as ReportViewerControl;
            if (control == null) return;
            if (windowClosedEventHandler != null)
                window.Closed += windowClosedEventHandler;

            control.DefineTable(1);
            control.PrintTableLine(Types.Strings.VoidTransactionReport);
            control.PrintTableLine(Types.Strings.From + startDate);
            control.PrintTableLine(Types.Strings.To + endDate);

            control.DefineTable(4);
            control.PrintTableLine(
                Types.Strings.EmployeeTimeTicketNumberAmount, 0, true);

            // Get all TicketVoid's for the specified date range
            IEnumerable<TicketVoid> ticketVoids = TicketVoid.GetAll(startDate, endDate);
            foreach (TicketVoid ticketVoid in ticketVoids)
            {
                string line = "";
                Person person = Person.Get(Employee.GetPersonId(ticketVoid.EmployeeId));
                if (person == null)
                    continue;
                line += person.LastName + ", " + person.FirstName + "   ";
                line += ticketVoid.When.ToShortDateString() + " " +
                    ticketVoid.When.ToShortTimeString() + "   ";
                line += ticketVoid.TicketId + "   ";
                line += ticketVoid.Amount.ToString("C2");
                control.PrintTableLine(line);
            }

            window.Topmost = true;
            window.IsClosable = true;
            window.Show();
        }

        public static void PrintReturns(DateTime startDate, DateTime endDate,
            EventHandler windowClosedEventHandler = null)
        {
            PosDialogWindow window = ReportViewerControl.CreateInDefaultWindow(
                Types.Strings.ReturnTransactionReport);
            var control = window.DockedControl as ReportViewerControl;
            if (control == null) return;
            if (windowClosedEventHandler != null)
                window.Closed += windowClosedEventHandler;

            control.DefineTable(1);
            control.PrintTableLine(Types.Strings.ReturnTransactionReport);
            control.PrintTableLine(Types.Strings.From + startDate);
            control.PrintTableLine(Types.Strings.To + endDate);

            control.DefineTable(new [] {
                    new GridLength(3, GridUnitType.Star),
                    new GridLength(3, GridUnitType.Star),
                    new GridLength(2.5, GridUnitType.Star),
                    new GridLength(0.5, GridUnitType.Star),
                    new GridLength(3, GridUnitType.Star),
                    new GridLength(1.5, GridUnitType.Star),
                    new GridLength(0, GridUnitType.Star)  // wtf???
                });

            control.PrintTableLine(
                Types.Strings.EmployeeTimeTicketNumberNumberItemAmount, 0, true);

            foreach (TicketItemReturn ticketItemReturn in
                TicketItemReturn.GetAll(startDate, endDate))
            {
                string line = "";
                Person person = Person.Get(Employee.GetPersonId(ticketItemReturn.EmployeeId));
                Item item = Item.Get(ticketItemReturn.ItemId);
                line += person.LastName + ", " + person.FirstName + "   ";
                line += ticketItemReturn.When.ToShortDateString() + " " +
                    ticketItemReturn.When.ToShortTimeString() + "   ";
                line += ticketItemReturn.TicketId + "   ";
                line += ticketItemReturn.ItemQuantity + "   ";
                line += item.FullName + "   ";
                line += ticketItemReturn.Amount.ToString("C2");
                control.PrintTableLine(line);
            }

            window.Topmost = true;
            window.IsClosable = true;
            window.Show();
        }

        public static void PrintRefunds(DateTime startDate, DateTime endDate,
            EventHandler windowClosedEventHandler = null)
        {
            PosDialogWindow window = ReportViewerControl.CreateInDefaultWindow(
                Types.Strings.RefundTransactionReport);
            var control = window.DockedControl as ReportViewerControl;
            if (control == null) return;
            if (windowClosedEventHandler != null)
                window.Closed += windowClosedEventHandler;

            control.DefineTable(1);
            control.PrintTableLine(Types.Strings.RefundTransactionReport);
            control.PrintTableLine(Types.Strings.From + startDate);
            control.PrintTableLine(Types.Strings.To + endDate);

            control.DefineTable(new [] {
                    new GridLength(3, GridUnitType.Star),
                    new GridLength(3, GridUnitType.Star),
                    new GridLength(2.5, GridUnitType.Star),
                    new GridLength(1.5, GridUnitType.Star),
                    new GridLength(2, GridUnitType.Star),
                    new GridLength(1.5, GridUnitType.Star),
                    new GridLength(0, GridUnitType.Star)  // wtf???
                });

            control.PrintTableLine(
                Types.Strings.EmployeeTimeTicketNumberRegisterNumberTypeAmount, 0, true);

            foreach (TicketRefund ticketRefund in TicketRefund.GetAll(startDate, endDate))
            {
                string line = "";
                Person person = Person.Get(Employee.GetPersonId(ticketRefund.EmployeeId));
                line += person.LastName + ", " + person.FirstName + "   ";
                line += ticketRefund.When.ToShortDateString() + " " +
                    ticketRefund.When.ToShortTimeString() + "   ";
                line += ticketRefund.TicketId + "   ";
                line += ticketRefund.RegisterDrawerId + "   ";
                switch (ticketRefund.Type)                
                {
                    case (TicketRefundType.Reopened):
                        line += Types.Strings.Reopened;
                        break;
                    case (TicketRefundType.Void):
                        line += Types.Strings.Voided;
                        break;
                    case (TicketRefundType.CancelUnmade):
                        line += Types.Strings.CancelUnmade + "   ";
                        break;
                    case (TicketRefundType.CancelMade):
                        line += Types.Strings.CancelMade + "   ";
                        break;
                }
                line += ticketRefund.Amount.ToString("C2");
                control.PrintTableLine(line);
            }

            window.Topmost = true;
            window.IsClosable = true;
            window.Show();
        }

        public static void PrintNoSales(DateTime startDate, DateTime endDate,
            EventHandler windowClosedEventHandler = null)
        {
            PosDialogWindow window = ReportViewerControl.CreateInDefaultWindow(
                Types.Strings.NoSaleTransactionReport);
            var control = window.DockedControl as ReportViewerControl;
            if (control == null) return;
            if (windowClosedEventHandler != null)
                window.Closed += windowClosedEventHandler;

            control.DefineTable(1);
            control.PrintTableLine(Types.Strings.NoSaleTransactionReport);
            control.PrintTableLine(Types.Strings.From + startDate);
            control.PrintTableLine(Types.Strings.To + endDate);

            control.DefineTable(new [] {
                    new GridLength(3, GridUnitType.Star),
                    new GridLength(3, GridUnitType.Star),
                    new GridLength(3, GridUnitType.Star),
                    new GridLength(0, GridUnitType.Star)  // wtf???
                });
            control.PrintTableLine(Types.Strings.EmployeeTimeRegisterNumber, 0, true);

            foreach (RegisterNoSale noSale in RegisterNoSale.GetAll(startDate, endDate))
            {
                string line = "";
                Person person = Person.Get(Employee.GetPersonId(noSale.EmployeeId));
                if (person == null)
                    continue;
                line += person.LastName + ", " + person.FirstName + "   ";
                line += noSale.When.ToShortDateString() + " " +
                    noSale.When.ToShortTimeString() + "   ";
                line += noSale.RegisterDrawerId + "   ";
                control.PrintTableLine(line);
            }

            window.Topmost = true;
            window.IsClosable = true;
            window.Show();
        }

        public static void PrintSafeDrops(DateTime startDate, DateTime endDate,
            EventHandler windowClosedEventHandler = null)
        {
            PosDialogWindow window = ReportViewerControl.CreateInDefaultWindow(
                Types.Strings.SafeDropTransactionReport);
            var control = window.DockedControl as ReportViewerControl;
            if (control == null) return;
            if (windowClosedEventHandler != null)
                window.Closed += windowClosedEventHandler;

            control.DefineTable(1);
            control.PrintTableLine(Types.Strings.SafeDropTransactionReport);
            control.PrintTableLine(Types.Strings.From + startDate);
            control.PrintTableLine(Types.Strings.To + endDate);

            control.DefineTable(new [] {
                    new GridLength(3, GridUnitType.Star),
                    new GridLength(3, GridUnitType.Star),
                    new GridLength(3, GridUnitType.Star),
                    new GridLength(2, GridUnitType.Star),
                    new GridLength(0, GridUnitType.Star)  // wtf???
                });
            control.PrintTableLine(Types.Strings.EmployeeTimeRegisterNumberAmount, 0, true);

            foreach (RegisterDrop drop in RegisterDrop.GetAll(startDate, endDate))
            {
                string line = "";
                Person person = Person.Get(Employee.GetPersonId(drop.EmployeeId));
                line += person.LastName + ", " + person.FirstName + "   ";
                line += drop.When.ToShortDateString() + " " +
                    drop.When.ToShortTimeString() + "   ";
                line += drop.RegisterDrawerId + "   ";
                line += drop.Amount.ToString("C2");
                control.PrintTableLine(line);
            }

            window.Topmost = true;
            window.IsClosable = true;
            window.Show();
        }

        public static void PrintPayouts(DateTime startDate, DateTime endDate,
            EventHandler windowClosedEventHandler = null)
        {
            PosDialogWindow window = ReportViewerControl.CreateInDefaultWindow(
                Types.Strings.PayoutTransactionReport);
            var control = window.DockedControl as ReportViewerControl;
            if (control == null) return;
            if (windowClosedEventHandler != null)
                window.Closed += windowClosedEventHandler;

            control.DefineTable(1);
            control.PrintTableLine(Types.Strings.PayoutTransactionReport);
            control.PrintTableLine(Types.Strings.From + startDate);
            control.PrintTableLine(Types.Strings.To + endDate);

            control.DefineTable(new [] {
                    new GridLength(3, GridUnitType.Star),
                    new GridLength(3, GridUnitType.Star),
                    new GridLength(2.5, GridUnitType.Star),
                    new GridLength(3, GridUnitType.Star), 
                    new GridLength(1.5, GridUnitType.Star),
                    new GridLength(0, GridUnitType.Star)  // wtf???
                });
            control.PrintTableLine(Types.Strings.EmployeeTimeRegisterNumberReasonAmount, 0, true);

            foreach (RegisterPayout payout in RegisterPayout.GetAll(startDate, endDate))
            {
                string line = "";
                Person person = Person.Get(Employee.GetPersonId(payout.EmployeeId));
                line += person.LastName + ", " + person.FirstName + "   ";
                line += payout.When.ToShortDateString() + " " +
                    payout.When.ToShortTimeString() + "   ";
                line += payout.RegisterDrawerId + "   ";
                line += payout.Reason + "   ";
                line += payout.Amount.ToString("C2");
                control.PrintTableLine(line);
            }

            window.Topmost = true;
            window.IsClosable = true;
            window.Show();
        }

        public static void PrintDeposits(DateTime startDate, DateTime endDate,
            EventHandler windowClosedEventHandler = null)
        {
            PosDialogWindow window = ReportViewerControl.CreateInDefaultWindow(
                Types.Strings.RegisterDepositTransactionReport);
            var control = window.DockedControl as ReportViewerControl;
            if (control == null) return;
            if (windowClosedEventHandler != null)
                window.Closed += windowClosedEventHandler;

            control.DefineTable(1);
            control.PrintTableLine(Types.Strings.RegisterDepositTransactionReport);
            control.PrintTableLine(Types.Strings.From + startDate);
            control.PrintTableLine(Types.Strings.To + endDate);

            control.DefineTable(new [] {
                    new GridLength(3, GridUnitType.Star),
                    new GridLength(3, GridUnitType.Star),
                    new GridLength(3, GridUnitType.Star),
                    new GridLength(2, GridUnitType.Star),
                    new GridLength(0, GridUnitType.Star)  // wtf???
                });
            control.PrintTableLine(Types.Strings.EmployeeTimeRegisterNumberAmount, 0, true);

            foreach (RegisterDeposit deposit in RegisterDeposit.GetAll(startDate, endDate))
            {
                string line = "";
                Person person = Person.Get(Employee.GetPersonId(deposit.EmployeeId));
                line += person.LastName + ", " + person.FirstName + "   ";
                line += deposit.When.ToShortDateString() + " " +
                    deposit.When.ToShortTimeString() + "   ";
                line += deposit.RegisterDrawerId + "   ";
                line += deposit.Amount.ToString("C2");
                control.PrintTableLine(line);
            }

            window.Topmost = true;
            window.IsClosable = true;
            window.Show();
        }

        public static void PrintCancels(DateTime startDate, DateTime endDate,
            EventHandler windowClosedEventHandler = null)
        {
            PosDialogWindow window = ReportViewerControl.CreateInDefaultWindow(
                Types.Strings.TicketItemCancelTransactionReport);
            var control = window.DockedControl as ReportViewerControl;
            if (control == null) return;
            if (windowClosedEventHandler != null)
                window.Closed += windowClosedEventHandler;

            control.DefineTable(1);
            control.PrintTableLine(Types.Strings.TicketItemCancelTransactionReport);
            control.PrintTableLine(Types.Strings.From + startDate);
            control.PrintTableLine(Types.Strings.To + endDate);

            control.DefineTable(new [] {
                    new GridLength(2, GridUnitType.Star),
                    new GridLength(2.25, GridUnitType.Star),
                    new GridLength(1, GridUnitType.Star),
                    new GridLength(2.25, GridUnitType.Star),
                    new GridLength(1, GridUnitType.Star),
                    new GridLength(1.25, GridUnitType.Star),
                    new GridLength(0, GridUnitType.Star)  // wtf???
                });
            control.PrintTableLine(Types.Strings.EmployeeTimeTicketNumberItemQuantityAmount, 0, true);

            foreach (TicketItem ticketItem in TicketItem.GetAll(startDate, endDate, true))
            {
                string line = "";
                if ((ticketItem.CancelingEmployeeId == null) || (ticketItem.WhenCanceled == null)) continue;
                Person person = Person.Get(Employee.GetPersonId(ticketItem.CancelingEmployeeId.Value));
                line += person.LastName + ", " + person.FirstName + "   ";
                line += ticketItem.WhenCanceled.Value.ToShortDateString() + " " +
                    ticketItem.WhenCanceled.Value.ToShortTimeString() + "   ";
                line += ticketItem.TicketId + "   ";
                Item item = Item.Get(ticketItem.ItemId);
                line += item.FullName + "   ";
                line += ticketItem.Quantity + "   ";
                if (!ticketItem.IsWasted)
                    line += Types.Strings.Unmade;
                else
                    line += ticketItem.GetTotalCost().ToString("C2");
                control.PrintTableLine(line);
            }

            window.Topmost = true;
            window.IsClosable = true;
            window.Show();
        }

        public static void PrintFloatingDocking(DateTime startDate, DateTime endDate,
            EventHandler windowClosedEventHandler = null)
        {
            PosDialogWindow window = ReportViewerControl.CreateInDefaultWindow(
                Types.Strings.FloatingDockingTransactionReport);
            var control = window.DockedControl as ReportViewerControl;
            if (control == null) return;
            if (windowClosedEventHandler != null)
                window.Closed += windowClosedEventHandler;

            control.DefineTable(1);
            control.PrintTableLine(Types.Strings.FloatingDockingTransactionReport);
            control.PrintTableLine(Types.Strings.From + startDate);
            control.PrintTableLine(Types.Strings.To + endDate);

            control.DefineTable(new [] {
                    new GridLength(3, GridUnitType.Star),
                    new GridLength(3, GridUnitType.Star),
                    new GridLength(3, GridUnitType.Star),
                    new GridLength(2, GridUnitType.Star),
                    new GridLength(2, GridUnitType.Star),
                    new GridLength(0, GridUnitType.Star)  // wtf???
                });
            control.PrintTableLine(Types.Strings.EmployeeUndockTimeDockTimeSourceDestination, 0, true);

            foreach (RegisterMove move in RegisterMove.GetAll(startDate, endDate))
            {
                RegisterDrawer drawer = RegisterDrawer.Get(move.RegisterDrawerId);
                Person person = Person.Get(Employee.GetPersonId(drawer.EmployeeId));
                string line = "";
                line += person.LastName + ", " + person.FirstName + "   ";
                line += move.StartTime.ToShortDateString() + " " +
                    move.StartTime.ToShortTimeString()  + "   ";
                line += ((move.EndTime != null) ? move.EndTime.Value.ToShortDateString() +
                    " " + move.EndTime.Value.ToShortTimeString() : Types.Strings.Undocked) + "   ";
                line += "" + move.SourceRegisterId + "-" + move.SourceRegisterSubId + "   ";
                if ((move.TargetRegisterId != null) && (move.TargetRegisterSubId != null))
                    line += "" + move.TargetRegisterId + "-" + move.TargetRegisterSubId + "   ";
                else if (move.TargetRegisterId != null)
                    line += "" + move.TargetRegisterId + "   ";
                else
                    line += Types.Strings.Undocked;
                control.PrintTableLine(line);
            }

            window.Topmost = true;
            window.IsClosable = true;
            window.Show();
        }
        #endregion

        #region Logging and Inventory Reports
        public static void PrintItemAdjustments(DateTime startDate, DateTime endDate,
            EventHandler windowClosedEventHandler = null)
        {
            PosDialogWindow window = ReportViewerControl.CreateInDefaultWindow(
                Types.Strings.ItemAdjustmentReport);
            var control = window.DockedControl as ReportViewerControl;
            if (control == null) return;
            if (windowClosedEventHandler != null)
                window.Closed += windowClosedEventHandler;

            //DateTime now = DateTime.Now;
            control.DefineTable(1);
            control.PrintTableLine(Types.Strings.ItemAdjustmentReport);
            control.PrintTableLine(Types.Strings.From + startDate);
            control.PrintTableLine(Types.Strings.To + endDate);

            control.DefineTable(new [] {
                    new GridLength(2.5, GridUnitType.Star),
                    new GridLength(2.5, GridUnitType.Star),
                    new GridLength(2.5, GridUnitType.Star),
                    new GridLength(2.5, GridUnitType.Star),
                    new GridLength(2.5, GridUnitType.Star),
                    new GridLength(0, GridUnitType.Star)  // wtf???
                });

            control.PrintTableLine(Types.Strings.WhenEmployeeTypeItemItemOptionSet, 0, true);

            foreach (ItemAdjustment itemAdjustment in
                ItemAdjustment.GetAll(startDate, endDate))
            {
                Person person = Person.Get(Employee.GetPersonId(itemAdjustment.EmployeeId));
                Item item = Item.Get(itemAdjustment.ItemId);
                string line = "";

                line += itemAdjustment.When.ToShortDateString() + " " +
                    itemAdjustment.When.ToShortTimeString() + "   ";
                line += person.LastName + ", " + person.FirstName + "   ";
                switch (itemAdjustment.Type)
                {
                    // ItemAdjustmentType.Addition, not being used
                    case ItemAdjustmentType.Addition:
                        line += Types.Strings.AddedItem;
                        break;
                    case ItemAdjustmentType.Discontinuation:
                        line += Types.Strings.DeletedItem;
                        break;
                    case ItemAdjustmentType.OptionSetAddition:
                        line += Types.Strings.OptionSetAdd;
                        break;
                    case ItemAdjustmentType.OptionSetDeletion:
                        line += Types.Strings.OptionSetDelete;
                        break;
                }
                line += item.FullName + "   ";
                if (itemAdjustment.ItemOptionSetId != null)
                {
                    ItemOptionSet itemOptionSet =
                        ItemOptionSet.Get(itemAdjustment.ItemOptionSetId.Value);
                    line += itemOptionSet.Name + "   ";
                }
                control.PrintTableLine(line);
            }

            // Show Window
            window.Topmost = true;
            window.IsClosable = true;
            window.Show();
        }

        public static void PrintPricingChanges(DateTime startDate, DateTime endDate,
            EventHandler windowClosedEventHandler = null)
        {
            PosDialogWindow window = ReportViewerControl.CreateInDefaultWindow(
                Types.Strings.ItemPriceChangeReport);
            var control = window.DockedControl as ReportViewerControl;
            if (control == null) return;
            if (windowClosedEventHandler != null)
                window.Closed += windowClosedEventHandler;

            //DateTime now = DateTime.Now;
            control.DefineTable(1);
            control.PrintTableLine(Types.Strings.ItemPriceChangeReport);
            control.PrintTableLine(Types.Strings.From + startDate);
            control.PrintTableLine(Types.Strings.To + endDate);

            control.DefineTable(new [] {
                    new GridLength(3.25, GridUnitType.Star),
                    new GridLength(2.5, GridUnitType.Star),
                    new GridLength(2.5, GridUnitType.Star),
                    new GridLength(2.5, GridUnitType.Star),
                    new GridLength(2, GridUnitType.Star),
                    new GridLength(2, GridUnitType.Star),
                    new GridLength(0, GridUnitType.Star)  // wtf???
                });

            control.PrintTableLine(Types.Strings.WhenEmployeeItemChangeOriginalValueNewValue, 0, true);

            foreach (ItemPricingChangeLog itemPricingChangeLog in
                ItemPricingChangeLog.GetAll(startDate, endDate))
            {
                bool isFirst = true;
                Person person = Person.Get(Employee.GetPersonId(itemPricingChangeLog.EmployeeId));
                Item item = Item.Get(itemPricingChangeLog.ItemId);
                string line = "";
                line += itemPricingChangeLog.ChangedTime.ToShortDateString() + " " +
                    itemPricingChangeLog.ChangedTime.ToShortTimeString() + "   ";
                line += person.LastName + ", " + person.FirstName + "   ";
                line += item.FullName + "   ";

                if ((itemPricingChangeLog.OldPrice.HasValue) ||
                    (itemPricingChangeLog.NewPrice.HasValue))
                {
                    if (!itemPricingChangeLog.ItemPricingId.HasValue)
                        line += Types.Strings.RegularPrice;
                    else
                        line += Types.Strings.SpecialPrice;
                    if (itemPricingChangeLog.OldPrice.HasValue)
                        line += itemPricingChangeLog.OldPrice.Value.ToString("C2") + "   ";
                    else
                        line += Types.Strings.Added + "   ";
                    if (itemPricingChangeLog.NewPrice.HasValue)
                        line += itemPricingChangeLog.NewPrice.Value.ToString("C2") + "   ";
                    else
                        line += Types.Strings.Removed + "   ";
                    control.PrintTableLine(line, (isFirst ? 0 : 3));
                    isFirst = false;
                }
                if ((itemPricingChangeLog.OldDayOfWeek.HasValue) ||
                    (itemPricingChangeLog.NewDayOfWeek.HasValue))
                {
                    line = (isFirst ? line : "") + Types.Strings.DayOfWeek + "   ";
                    if (itemPricingChangeLog.OldDayOfWeek.HasValue)
                        line += itemPricingChangeLog.OldDayOfWeek.Value + "   ";
                    else
                        line += Types.Strings.Added + "   ";
                    if (itemPricingChangeLog.NewDayOfWeek.HasValue)
                        line += itemPricingChangeLog.NewDayOfWeek.Value + "   ";
                    else
                        line += Types.Strings.Removed + "   ";
                    control.PrintTableLine(line, (isFirst ? 0 : 3));
                    isFirst = false;                 
                }
                if ((itemPricingChangeLog.OldStartTime.HasValue) ||
                    (itemPricingChangeLog.NewStartTime.HasValue))
                {
                    line = (isFirst ? line : "") + Types.Strings.StartTime + "   ";
                    if (itemPricingChangeLog.OldStartTime.HasValue)
                        line += itemPricingChangeLog.OldStartTime.Value + "   ";
                    else
                        line += Types.Strings.Added + "   ";
                    if (itemPricingChangeLog.NewStartTime.HasValue)
                        line += itemPricingChangeLog.NewStartTime.Value + "   ";
                    else
                        line += Types.Strings.Removed + "   ";
                    control.PrintTableLine(line, (isFirst ? 0 : 3));
                    isFirst = false;
                }
                if ((itemPricingChangeLog.OldEndTime.HasValue) ||
                    (itemPricingChangeLog.NewEndTime.HasValue))
                {
                    line = (isFirst ? line : "") + Types.Strings.EndTime;
                    if (itemPricingChangeLog.OldEndTime.HasValue)
                        line += itemPricingChangeLog.OldEndTime.Value + "   ";
                    else
                        line += Types.Strings.Added + "   ";
                    if (itemPricingChangeLog.NewEndTime.HasValue)
                        line += itemPricingChangeLog.NewEndTime.Value + "   ";
                    else
                        line += Types.Strings.Removed + "   ";
                    control.PrintTableLine(line, (isFirst ? 0 : 3));
                    //isFirst = false;
                }
            }

            window.Topmost = true;
            window.IsClosable = true;
            window.Show();
        }

        public static void PrintIngredientSetChanges(DateTime startDate, DateTime endDate,
            EventHandler windowClosedEventHandler = null)
        {
            PosDialogWindow window = ReportViewerControl.CreateInDefaultWindow(
                Types.Strings.IngredientRecipeAdjustmentReport);
            var control = window.DockedControl as ReportViewerControl;
            if (control == null) return;
            if (windowClosedEventHandler != null)
                window.Closed += windowClosedEventHandler;

            //DateTime now = DateTime.Now;
            control.DefineTable(1);
            control.PrintTableLine(Types.Strings.IngredientRecipeAdjustmentReport);
            control.PrintTableLine(Types.Strings.From + startDate);
            control.PrintTableLine(Types.Strings.To + endDate);

            control.DefineTable(new [] {
                    new GridLength(3.25, GridUnitType.Star),
                    new GridLength(2.5, GridUnitType.Star),
                    new GridLength(2.5, GridUnitType.Star),
                    new GridLength(2, GridUnitType.Star),
                    new GridLength(2, GridUnitType.Star),
                    new GridLength(2.5, GridUnitType.Star),
                    new GridLength(0, GridUnitType.Star)  // wtf???
                });

            control.PrintTableLine(Types.Strings.WhenEmployeeIngredientOriginalQuantityNewQuantityDescription, 0, true);

            foreach (IngredientAdjustment adjustment in IngredientAdjustment.GetAll(startDate, endDate))
            {
                if (!adjustment.RecipeIngredientId.HasValue)
                    continue;
                Ingredient ingredient = Ingredient.Get(adjustment.IngredientId);
                Person person = Person.Get(Employee.GetPersonId(adjustment.EmployeeId));
                string line = "";
                line += adjustment.When.ToShortDateString() + " " +
                    adjustment.When.ToShortTimeString() + "   ";
                line += person.LastName + ", " + person.FirstName + "   ";
                if (adjustment.NewValue.HasValue && adjustment.OriginalValue.HasValue &&
                    (Math.Abs(adjustment.NewValue.Value - -1) < double.Epsilon) &&
                    (Math.Abs(adjustment.OriginalValue.Value - -1) < double.Epsilon))
                {
                    MeasurementUnit originalValue = adjustment.MeasurementUnit;
                    var newValue = (MeasurementUnit)adjustment.RecipeIngredientId.Value;
                    line += ingredient.FullName + "   ";
                    line += originalValue + Types.Strings.S;
                    line += newValue + Types.Strings.S;
                    line += Types.Strings.MeasurementChanged;
                    control.PrintTableLine(line);
                    continue;
                }
                if (adjustment.RecipeIngredientId < 0)
                {
                    line += ingredient.FullName + "   ";
                    if (adjustment.RecipeIngredientId == -1)
                    {
                        double originalValue = (adjustment.OriginalValue.HasValue ?
                            adjustment.OriginalValue.Value : 0);
                        double newValue = (adjustment.NewValue.HasValue ?
                            adjustment.NewValue.Value : 0);
                        line += originalValue + "   ";
                        line += newValue + "   ";
                        line += Types.Strings.YieldChanged;
                    }
                    control.PrintTableLine(line);
                    continue;
                }
                Ingredient recipeIngredient = Ingredient.Get(adjustment.RecipeIngredientId.Value);
                line += recipeIngredient.FullName + "   ";
                if (!adjustment.OriginalValue.HasValue && adjustment.NewValue.HasValue)
                {
                    line += Types.Strings.Added + "   ";
                    line += adjustment.NewValue.Value + " " + adjustment.MeasurementUnit +
                        ((Math.Abs(adjustment.NewValue.Value - 1) > double.Epsilon) ? Types.Strings.S : "   ");
                }
                else if (!adjustment.NewValue.HasValue && adjustment.OriginalValue.HasValue)
                {
                    line += adjustment.OriginalValue.Value + " " + adjustment.MeasurementUnit +
                        ((Math.Abs(adjustment.OriginalValue.Value - 1) > double.Epsilon) ? Types.Strings.S : "   ");
                    line += Types.Strings.Removed + "   ";
                }
                else if ((adjustment.OriginalValue.HasValue) && (adjustment.NewValue.HasValue))
                {
                    line += adjustment.OriginalValue.Value + " " + adjustment.MeasurementUnit +
                        ((Math.Abs(adjustment.OriginalValue.Value - 1) > double.Epsilon) ? Types.Strings.S : "   ");
                    line += adjustment.NewValue.Value + " " + adjustment.MeasurementUnit +
                        ((Math.Abs(adjustment.NewValue.Value - 1) > double.Epsilon) ? Types.Strings.S : "   ");
                }
                line += ingredient.FullName + "   ";
                control.PrintTableLine(line);
            }

            window.Topmost = true;
            window.IsClosable = true;
            window.Show();
        }

        public static void PrintInventoryAdjustments(DateTime startDate, DateTime endDate,
            EventHandler windowClosedEventHandler = null)
        {
            PosDialogWindow window = ReportViewerControl.CreateInDefaultWindow(
                Types.Strings.InventoryAdjustmentsReport);
            var control = window.DockedControl as ReportViewerControl;
            if (control == null) return;
            if (windowClosedEventHandler != null)
                window.Closed += windowClosedEventHandler;

            //DateTime now = DateTime.Now;
            control.DefineTable(1);
            control.PrintTableLine(Types.Strings.InventoryAdjustmentsReport);
            control.PrintTableLine(Types.Strings.From + startDate);
            control.PrintTableLine(Types.Strings.To + endDate);

            control.DefineTable(new [] {
                    new GridLength(3, GridUnitType.Star),
                    new GridLength(2.5, GridUnitType.Star),
                    new GridLength(2, GridUnitType.Star),
                    new GridLength(2, GridUnitType.Star),
                    new GridLength(2, GridUnitType.Star),
                    new GridLength(2, GridUnitType.Star),
                    new GridLength(0, GridUnitType.Star)  // wtf???
                });

            control.PrintTableLine(Types.Strings.WhenEmployeeOriginalQuantityNewQuantityIngredient, 0, true);

            foreach (IngredientAdjustment adjustment in IngredientAdjustment.GetAll(startDate, endDate))
            {
                if (adjustment.RecipeIngredientId.HasValue ||
                    !adjustment.OriginalValue.HasValue ||
                    !adjustment.NewValue.HasValue)
                    continue;
                Ingredient ingredient = Ingredient.Get(adjustment.IngredientId);
                Person person = Person.Get(Employee.GetPersonId(adjustment.EmployeeId));
                string line = "";
                line += adjustment.When.ToShortDateString() + " " +
                    adjustment.When.ToShortTimeString() + "   ";
                line += person.LastName + ", " + person.FirstName + "   ";
                line += adjustment.OriginalValue.Value + " " + ingredient.MeasurementUnit +
                    ((Math.Abs(adjustment.OriginalValue.Value - 1) > double.Epsilon) ? Types.Strings.S : "   ");
                line += adjustment.NewValue.Value + " " + ingredient.MeasurementUnit +
                    ((Math.Abs(adjustment.NewValue.Value - 1) > double.Epsilon) ? Types.Strings.S : "   ");
                line += ingredient.FullName + "   ";
                control.PrintTableLine(line);
            }

            window.Topmost = true;
            window.IsClosable = true;
            window.Show();
        }

        public static void PrintItemIngredientChanges(DateTime startDate, DateTime endDate,
            EventHandler windowClosedEventHandler = null)
        {
            PosDialogWindow window = ReportViewerControl.CreateInDefaultWindow(
                Types.Strings.ItemRecipeAdjustmentReport);
            var control = window.DockedControl as ReportViewerControl;
            if (control == null) return;
            if (windowClosedEventHandler != null)
                window.Closed += windowClosedEventHandler;

            //DateTime now = DateTime.Now;
            control.DefineTable(1);
            control.PrintTableLine(Types.Strings.ItemRecipeAdjustmentReport);
            control.PrintTableLine(Types.Strings.From + startDate);
            control.PrintTableLine(Types.Strings.To + endDate);

            control.DefineTable(new [] {
                    new GridLength(3.25, GridUnitType.Star),
                    new GridLength(2.5, GridUnitType.Star),
                    new GridLength(2.5, GridUnitType.Star),
                    new GridLength(2, GridUnitType.Star),
                    new GridLength(2, GridUnitType.Star),
                    new GridLength(2.5, GridUnitType.Star),
                    new GridLength(0, GridUnitType.Star)  // wtf???
                });

            control.PrintTableLine(Types.Strings.WhenEmployeeItemOriginalQuantityNewQuantityIngredient, 0, true);

            foreach (ItemIngredientAdjustment adjustment in ItemIngredientAdjustment.GetAll(startDate, endDate))
            {
                Ingredient ingredient = Ingredient.Get(adjustment.IngredientId);
                Item item = Item.Get(adjustment.ItemId);
                Person person = Person.Get(Employee.GetPersonId(adjustment.EmployeeId));
                string line = "";
                line += adjustment.When.ToShortDateString() + " " +
                    adjustment.When.ToShortTimeString() + "   ";
                line += person.LastName + ", " + person.FirstName + "   ";
                line += item.FullName + "   ";
                if (!adjustment.OldAmount.HasValue && adjustment.NewAmount.HasValue)
                {
                    line += Types.Strings.Added + "   ";
                    line += adjustment.NewAmount.Value + " " + ingredient.MeasurementUnit.ToString() +
                        ((Math.Abs(adjustment.NewAmount.Value - 1) > double.Epsilon) ? Types.Strings.S : "   ");
                }
                else if (!adjustment.NewAmount.HasValue && adjustment.OldAmount.HasValue)
                {
                    line += adjustment.OldAmount.Value + " " + ingredient.MeasurementUnit.ToString() +
                        ((Math.Abs(adjustment.OldAmount.Value - 1) > double.Epsilon) ? Types.Strings.S : "   ");
                    line += Types.Strings.Removed + "   ";
                }
                else if ((adjustment.OldAmount.HasValue) && (adjustment.NewAmount.HasValue))
                {
                    line += adjustment.OldAmount.Value + " " + ingredient.MeasurementUnit.ToString() +
                        ((Math.Abs(adjustment.OldAmount.Value - 1) > double.Epsilon) ? Types.Strings.S : "   ");
                    line += adjustment.NewAmount.Value + " " + ingredient.MeasurementUnit.ToString() +
                        ((Math.Abs(adjustment.NewAmount.Value - 1) > double.Epsilon) ? Types.Strings.S : "   ");
                }
                line += ingredient.FullName + "   ";
                control.PrintTableLine(line);
            }

            window.Topmost = true;
            window.IsClosable = true;
            window.Show();
        }

        public static void PrintCurrentInventory(DateTime startDate, DateTime endDate,
            EventHandler windowClosedEventHandler = null)
        {
            PosDialogWindow window = ReportViewerControl.CreateInDefaultWindow(
                Types.Strings.CurrentInventoryReport);
            var control = window.DockedControl as ReportViewerControl;
            if (control == null) return;
            if (windowClosedEventHandler != null)
                window.Closed += windowClosedEventHandler;

            DateTime now = DateTime.Now;
            control.DefineTable(1);
            control.PrintTableLine(Types.Strings.CurrentInventoryReport);
            control.PrintTableLine(Types.Strings.Time + now);

            control.DefineTable(new [] {
                    new GridLength(3, GridUnitType.Star),
                    new GridLength(2, GridUnitType.Star),
                    new GridLength(0, GridUnitType.Star)  // wtf???
                });

            control.PrintTableLine(Types.Strings.IngredientQuantity, 0, true);

            foreach (Ingredient ingredient in Ingredient.GetAll())
            {
                string line = "";
                line += ingredient.FullName + "   ";
                line += ingredient.InventoryAmount.ToString("F2") + " " + ingredient.MeasurementUnit.ToString();
                if (Math.Abs(ingredient.InventoryAmount - 1) > double.Epsilon)
                    line += Types.Strings.S;
                control.PrintTableLine(line);
            }

            window.Topmost = true;
            window.IsClosable = true;
            window.Show();
        }

        public static void PrintWasteByIngredient(DateTime startDate, DateTime endDate,
            EventHandler windowClosedEventHandler = null)
        {
            PosDialogWindow window = ReportViewerControl.CreateInDefaultWindow(
                Types.Strings.WasteByIngredientReport);
            var control = window.DockedControl as ReportViewerControl;
            if (control == null) return;
            if (windowClosedEventHandler != null)
                window.Closed += windowClosedEventHandler;

            //DateTime now = DateTime.Now;
            control.DefineTable(1);
            control.PrintTableLine(Types.Strings.WasteByIngredientReport);
            control.PrintTableLine(Types.Strings.From + startDate);
            control.PrintTableLine(Types.Strings.To + endDate);

            control.DefineTable(new [] {
                    new GridLength(2, GridUnitType.Star),
                    new GridLength(1, GridUnitType.Star),
                    new GridLength(1, GridUnitType.Star),
                    new GridLength(0, GridUnitType.Star)  // wtf???
                });

            control.PrintTableLine(Types.Strings.IngredientQuantityCost, 0, true);

            var itemCount = new Dictionary<int, int>();
            var amountsByIngredient = new Dictionary<Ingredient, double>();

            foreach (TicketItem ticketItem in TicketItem.GetAllWaste(startDate, endDate))
            {
                ChangeValue(itemCount, ticketItem.ItemId, ticketItem.Quantity);
                foreach (TicketItemOption ticketItemOption in
                    TicketItemOption.GetAll(ticketItem.PrimaryKey))
                {
                    ItemOption itemOption = ItemOption.Get(ticketItemOption.ItemOptionId);
                    if (!itemOption.ProductAmount.HasValue) continue;
                    double amount = itemOption.ProductAmount.Value * ticketItem.Quantity;
                    if (itemOption.UsesIngredient)
                    {
                        if (!itemOption.ProductId.HasValue ||
                            !itemOption.ProductMeasurementUnit.HasValue) continue;
                        Ingredient ingredient = Ingredient.Get(itemOption.ProductId.Value);
                        if (itemOption.ProductMeasurementUnit != ingredient.MeasurementUnit)
                            amount = UnitConversion.Convert(amount,
                                itemOption.ProductMeasurementUnit.Value,
                                ingredient.MeasurementUnit);
                        ChangeValue(amountsByIngredient, ingredient, amount);
                    }
                    else if (itemOption.UsesItem)
                    {
                        ChangeValue(itemCount, ticketItem.ItemId, ticketItem.Quantity);
                    }
                }
            }

            foreach (int itemId in itemCount.Keys)
            {
                foreach (ItemIngredient itemIngredient in ItemIngredient.GetAll(itemId))
                {
                    double amount = itemIngredient.Amount * itemCount[itemId];
                    Ingredient ingredient = Ingredient.Get(itemIngredient.IngredientId);
                    if (itemIngredient.MeasurementUnit != ingredient.MeasurementUnit)
                        amount = UnitConversion.Convert(amount,
                            itemIngredient.MeasurementUnit, ingredient.MeasurementUnit);
                    ChangeValue(amountsByIngredient, ingredient, amount);
                }
            }

            foreach (Ingredient ingredient in amountsByIngredient.Keys)
            {
                string line = "";
                line += ingredient.FullName + "   ";
                line += amountsByIngredient[ingredient] + " " +
                    ingredient.MeasurementUnit.ToString();
                if (Math.Abs(ingredient.InventoryAmount - 1) > double.Epsilon)
                    line += Types.Strings.S;
                line += "   ";
                line += (amountsByIngredient[ingredient] *
                    ingredient.GetActualCostPerUnit()).ToString("C2");
                control.PrintTableLine(line);
            }

            window.Topmost = true;
            window.IsClosable = true;
            window.Show();
        }

        public static void PrintUsageByIngredient(DateTime startDate, DateTime endDate,
            EventHandler windowClosedEventHandler = null)
        {
            PosDialogWindow window = ReportViewerControl.CreateInDefaultWindow(
                Types.Strings.UsageByIngredientReport);
            var control = window.DockedControl as ReportViewerControl;
            if (control == null) return;
            if (windowClosedEventHandler != null)
                window.Closed += windowClosedEventHandler;

            //DateTime now = DateTime.Now;
            control.DefineTable(1);
            control.PrintTableLine(Types.Strings.UsageByIngredientReport);
            control.PrintTableLine(Types.Strings.From + startDate);
            control.PrintTableLine(Types.Strings.To + endDate);

            control.DefineTable(new [] {
                    new GridLength(3, GridUnitType.Star),
                    new GridLength(2, GridUnitType.Star),
                    new GridLength(2, GridUnitType.Star),
                    new GridLength(0, GridUnitType.Star)  // wtf???
                });

            control.PrintTableLine(Types.Strings.IngredientQuantityCost, 0, true);

            var itemCount = new Dictionary<int, int>();
            var amountsByIngredient =
                new Dictionary<Ingredient, double>();

            foreach (TicketItem ticketItem in TicketItem.GetAllUsage(startDate, endDate))
            {
                ChangeValue(itemCount, ticketItem.ItemId, ticketItem.Quantity);
                foreach (TicketItemOption ticketItemOption in
                    TicketItemOption.GetAll(ticketItem.PrimaryKey))
                {
                    ItemOption itemOption = ItemOption.Get(ticketItemOption.ItemOptionId);
                    if (!itemOption.ProductAmount.HasValue) continue;
                    double amount = itemOption.ProductAmount.Value * ticketItem.Quantity;
                    if (itemOption.UsesIngredient)
                    {
                        if (!itemOption.ProductId.HasValue ||
                            !itemOption.ProductMeasurementUnit.HasValue) continue;
                        Ingredient ingredient = Ingredient.Get(itemOption.ProductId.Value);
                        if (itemOption.ProductMeasurementUnit != ingredient.MeasurementUnit)
                            amount = UnitConversion.Convert(amount,
                                itemOption.ProductMeasurementUnit.Value,
                                ingredient.MeasurementUnit);
                        ChangeValue(amountsByIngredient, ingredient, amount);
                    }
                    else if (itemOption.UsesItem)
                    {
                        ChangeValue(itemCount, ticketItem.ItemId, ticketItem.Quantity);
                    }
                }
            }

            foreach (int itemId in itemCount.Keys)
            {
                foreach (ItemIngredient itemIngredient in ItemIngredient.GetAll(itemId))
                {
                    double amount = itemIngredient.Amount * itemCount[itemId];
                    Ingredient ingredient = Ingredient.Get(itemIngredient.IngredientId);
                    if (itemIngredient.MeasurementUnit != ingredient.MeasurementUnit)
                        amount = UnitConversion.Convert(amount,
                            itemIngredient.MeasurementUnit, ingredient.MeasurementUnit);
                    ChangeValue(amountsByIngredient, ingredient, amount);
                }
            }

            foreach (Ingredient ingredient in amountsByIngredient.Keys)
            {
                if (Math.Abs(amountsByIngredient[ingredient] - 0) < double.Epsilon)
                    continue;
                string line = "";
                line += ingredient.FullName + "   ";
                line += amountsByIngredient[ingredient] + " " +
                    ingredient.MeasurementUnit.ToString();
                if (Math.Abs(ingredient.InventoryAmount - 1) > double.Epsilon)
                    line += Types.Strings.S;
                line +=  (ingredient.GetActualCostPerUnit() *
                    amountsByIngredient[ingredient]).ToString("C2");
                control.PrintTableLine(line);
            }

            window.Topmost = true;
            window.IsClosable = true;
            window.Show();
        }

        private static void ChangeValue(Dictionary<Ingredient, double> amountsByIngredient, Ingredient ingredient, double amount)
        {
            if (amountsByIngredient.Keys.Contains(ingredient))
                amountsByIngredient[ingredient] += amount;
            else
                amountsByIngredient.Add(ingredient, amount);
        }

        private static void ChangeValue(Dictionary<int, int> itemCount, int itemId, int quantity)
        {
            if (itemCount.Keys.Contains(itemId))
                itemCount[itemId] += quantity;
            else
                itemCount.Add(itemId, quantity);
        }

        public static void PrintWasteByItem(DateTime startDate, DateTime endDate,
            EventHandler windowClosedEventHandler = null)
        {
            PosDialogWindow window = ReportViewerControl.CreateInDefaultWindow(
                Types.Strings.WasteByItemReport);
            var control = window.DockedControl as ReportViewerControl;
            if (control == null) return;
            if (windowClosedEventHandler != null)
                window.Closed += windowClosedEventHandler;

            //DateTime now = DateTime.Now;
            control.DefineTable(1);
            control.PrintTableLine(Types.Strings.WasteByItemReport);
            control.PrintTableLine(Types.Strings.From + startDate);
            control.PrintTableLine(Types.Strings.To + endDate);

            control.DefineTable(new [] {
                    new GridLength(2.5, GridUnitType.Star),
                    new GridLength(1, GridUnitType.Star),
                    new GridLength(1, GridUnitType.Star),
                    new GridLength(1, GridUnitType.Star),
                    new GridLength(0, GridUnitType.Star)  // wtf???
                });
            control.PrintTableLine(
                Types.Strings.ItemQuantityItemCostIngredientCost,
                0, true);

            var wastedItems = new Dictionary<int, int>();
            foreach (TicketItem ticketItem in TicketItem.GetAllWasted(startDate, endDate))
            {
                if (wastedItems.Keys.Contains(ticketItem.ItemId))
                    wastedItems[ticketItem.ItemId] += ticketItem.Quantity;
                else
                    wastedItems.Add(ticketItem.ItemId, ticketItem.Quantity);
            }

            foreach (int itemId in wastedItems.Keys)
            {
                Item item = Item.Get(itemId);
                string line = "";
                line += item.FullName + "   ";
                line += wastedItems[itemId] + "   ";
                line += (item.Price * wastedItems[itemId]).ToString("C2") + "   ";
                line += (item.GetCostOfIngredients() * wastedItems[itemId]).ToString("C2");
                control.PrintTableLine(line);
            }

            window.Topmost = true;
            window.IsClosable = true;
            window.Show();
        }

        public static void PrintWasteByCategory(DateTime startDate, DateTime endDate,
            EventHandler windowClosedEventHandler = null)
        {
            PosDialogWindow window = ReportViewerControl.CreateInDefaultWindow(
                Types.Strings.WasteByCategoryReport);
            var control = window.DockedControl as ReportViewerControl;
            if (control == null) return;
            if (windowClosedEventHandler != null)
                window.Closed += windowClosedEventHandler;

            //DateTime now = DateTime.Now;
            control.DefineTable(1);
            control.PrintTableLine(Types.Strings.WasteByCategoryReport);
            control.PrintTableLine(Types.Strings.From + startDate);
            control.PrintTableLine(Types.Strings.To + endDate);

            control.DefineTable(new [] {
                    new GridLength(3, GridUnitType.Star),
                    new GridLength(2, GridUnitType.Star),
                    new GridLength(2, GridUnitType.Star),
                    new GridLength(2, GridUnitType.Star),
                    new GridLength(0, GridUnitType.Star)  // wtf???
                });

            control.PrintTableLine(Types.Strings.CategoryQuantityItemCostIngredientCost, 0, true);

            // Get all wasted ticket items within the date range
            var wastedItems = new Dictionary<int, int>();
            foreach (TicketItem ticketItem in TicketItem.GetAllWasted(startDate, endDate))
            {
                if (wastedItems.Keys.Contains(ticketItem.ItemId))
                    wastedItems[ticketItem.ItemId] += ticketItem.Quantity;
                else
                    wastedItems.Add(ticketItem.ItemId, ticketItem.Quantity);
            }

            var wastedCategoriesCount = new Dictionary<int, int>();
            var wastedCategoriesTotal = new Dictionary<int, double>();
            var wastedCategoriesProductCost = new Dictionary<int, double>();
            foreach (int itemId in wastedItems.Keys)
            {
                Item item = Item.Get(itemId);
                if (wastedCategoriesCount.Keys.Contains(item.CategoryId))
                {
                    wastedCategoriesCount[item.CategoryId] += wastedItems[itemId];
                    wastedCategoriesTotal[item.CategoryId] +=
                        (item.Price * wastedItems[itemId]);
                    wastedCategoriesProductCost[item.CategoryId] +=
                        (item.GetCostOfIngredients() * wastedItems[itemId]);
                }
                else
                {
                    wastedCategoriesCount.Add(item.CategoryId, wastedItems[itemId]);
                    wastedCategoriesTotal.Add(item.CategoryId,
                        item.Price * wastedItems[itemId]);
                    wastedCategoriesProductCost.Add(item.CategoryId,
                        item.GetCostOfIngredients() * wastedItems[itemId]);
                }
            }

            foreach (int categoryId in wastedCategoriesCount.Keys)
            {
                Category category = Category.Get(categoryId);
                string line = "";
                line += category.NameValue + "   ";
                line += wastedCategoriesCount[categoryId] + "   ";
                line += wastedCategoriesTotal[categoryId].ToString("C2") + "   ";
                line += wastedCategoriesProductCost[categoryId].ToString("C2");
                control.PrintTableLine(line);
            }

            window.Topmost = true;
            window.IsClosable = true;
            window.Show();
        }
        #endregion

        #region Helper Functions
        private static IEnumerable<YearId> GetTicketIds(IEnumerable<TicketPayment> payments)
        {
            var ticketIds = new List<YearId>();
            foreach (var ticketPrimaryKey in
                payments.Select(payment => new YearId(payment.PrimaryKey.Year, payment.TicketId))
                .Where(ticketPrimaryKey => !ticketIds.Contains(ticketPrimaryKey)))
            {
                ticketIds.Add(ticketPrimaryKey);
                yield return ticketPrimaryKey;
            }
        }

        #endregion
    }
}
