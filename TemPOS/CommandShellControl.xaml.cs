using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using PosControls;
using PosModels;
using PosModels.Managers;
using PosModels.Types;
using TemPOS.Helpers;
using TemPOS.Managers;
using Strings = TemPOS.Types.Strings;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for CommandShellControl.xaml
    /// </summary>
    public partial class CommandShellControl : UserControl
    {
        public delegate void CommandDelegate(string command);

        public CommandShellControl()
        {
            InitializeComponent();
            flowDocumentScroll.IsEnabled = false;
            flowDocumentScroll.Document = new FlowDocument();
            Loaded += CommandShellControl_Loaded;
        }

        [Obfuscation(Exclude = true)]
        void CommandShellControl_Loaded(object sender, RoutedEventArgs e)
        {
            flowDocumentScroll.Document.PageWidth = flowDocumentScroll.ActualWidth - 5;
        }

        [Obfuscation(Exclude = true)]
        private void keyboardControl_EnterPressed(object sender, EventArgs e)
        {
            string command = keyboardControl.Text;
            keyboardControl.Text = "";
            DoCommand(command);
        }

        [Obfuscation(Exclude = true)]
        private void keyboardControl_ConsoleClear(object sender, EventArgs e)
        {
            DoClearCommand(null);
        }

        private void DoCommand(string command)
        {
            if (String.IsNullOrEmpty(command) || String.IsNullOrWhiteSpace(command))
                return;
            string[] tokens = command.Split(' ');
            // SQL - SQL command
            // SET - Some value in configuration manager
            // STORESETTING [SETTING] As Int = [Value] - The StoreSetting SQL table
            bool found = ProcessCommand("sqlquery", command, tokens, DoSqlReaderCommand);
            if (!found)
                found = ProcessCommand("sqlnonquery", command, tokens, DoSqlNonQueryCommand);
            if (!found)
                found = ProcessCommand("set", command, tokens, DoSetCommand);
            if (!found)
                found = ProcessCommand("list", command, tokens, DoListCommand);
            if (!found)
                found = ProcessCommand("storesetting", command, tokens, DoStoreSettingCommand);
            if (!found)
                found = ProcessCommand("clear", command, tokens, DoClearCommand);
            if (!found)
                found = ProcessCommand("log", command, tokens, DoLogCommand);
            if (!found)
                found = ProcessCommand("test", command, tokens, DoTestCommand);
            if (!found)
                found = ProcessCommand("help", command, tokens, DoHelpCommand);
            if (!found)
                found = ProcessCommand("throw", command, tokens, DoThrowCommand);
            if (!found)
                found = ProcessCommand("reset", command, tokens, DoResetCommand);
            if (!found)
                PrintLine("Error: Unknown Command - \"" + tokens[0] + "\"");
        }

        private bool ProcessCommand(string commandName, string commandLine,
            string[] tokens, CommandDelegate commandDelegate)
        {
            try
            {
                if (tokens.Length > 1 && tokens[0].ToLower().Equals(commandName))
                    commandDelegate(commandLine.Substring(tokens[0].Length + 1));
                else if (tokens[0].ToLower().Equals(commandName))
                    commandDelegate(null);
                else
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                PrintLine("CommandException: Command=\"" + tokens[0] + "\", Message=\"" + ex.Message + "\"");
                return true;
            }
        }

        private void DoTestCommand(string testCommandParameters)
        {
        }

        private void DoResetCommand(string resetCommandParameters)
        {
            if (!resetCommandParameters.ToLower().Equals("system"))
                return;
            {
                if (PosDialogWindow.ShowDialog(
                    Types.Strings.ShellResetSystem,
                    Types.Strings.ShellResetSystemTitle, DialogButtons.YesNo) ==
                    DialogButton.Yes)
                {
                    ResetTransactionalTables();
                    App.SwitchToDefaultDesktopOnClose = true;
                    App.ShutdownApplication();
                }
            }
        }

        private static void ResetTransactionalTables()
        {
            SettingManager.SetStoreSetting("DailyIdOffset", 0);
            DayOfOperation.Reset(typeof(DayOfOperation));
            EmployeeTimesheet.Reset(typeof(EmployeeTimesheet));
            EmployeeTimesheetChangeLog.Reset(typeof(EmployeeTimesheetChangeLog));
            Lock.Reset(typeof(Lock));
            Party.Reset(typeof(Party));
            PartyInvite.Reset(typeof(PartyInvite));
            RegisterDeposit.Reset(typeof(RegisterDeposit));
            RegisterDrawer.Reset(typeof(RegisterDrawer));
            RegisterDrop.Reset(typeof(RegisterDrop));
            RegisterNoSale.Reset(typeof(RegisterNoSale));
            RegisterPayout.Reset(typeof(RegisterPayout));
            Ticket.Reset(typeof(Ticket));
            TicketCoupon.Reset(typeof(TicketCoupon));
            TicketDiscount.Reset(typeof(TicketDiscount));
            TicketItem.Reset(typeof(TicketItem));
            TicketItemOption.Reset(typeof(TicketItemOption));
            TicketItemReturn.Reset(typeof(TicketItemReturn));
            TicketPayment.Reset(typeof(TicketPayment));
            TicketRefund.Reset(typeof(TicketRefund));
            TicketVoid.Reset(typeof(TicketVoid));
        }

        private void DoThrowCommand(string testCommandParameters)
        {
            throw new Exception(testCommandParameters);
        }

        private void DoLogCommand(string logCommandParameters)
        {
            if (logCommandParameters.ToLower().Equals("delete"))
                DoLogDelete();
            else if (logCommandParameters.ToLower().Equals("show"))
                DoLogShow();
            else
                PrintHelpForLogCommand();
        }

        private void DoLogDelete()
        {
            if (!LogFileExists())
                return;
            if (PosDialogWindow.ShowDialog(
                Types.Strings.ShellDeleteLog,
                Types.Strings.DeleteConfirmation, DialogButtons.YesNo) !=
                DialogButton.Yes) return;
            Logger.CloseLog(); 
            File.Delete(Logger.LogFileName);
            Logger.OpenLog();
        }

        private bool LogFileExists()
        {
            if (!Logger.LogFileExists)
            {
                PrintLine(Types.Strings.ShellNoLogFileExists);
                return false;
            }
            return true;
        }

        private void DoLogShow()
        {
            if (!LogFileExists())
                return;

            Logger.CloseLog();
            FileStream file = File.Open(Logger.LogFileName, FileMode.Open);
            StreamReader reader = new StreamReader(file);
            string result = "";

            while (!reader.EndOfStream)
            {
                result += reader.ReadLine() + Environment.NewLine;
            }
            reader.Close();
            file.Close();
            PrintLine(result);
            Logger.OpenLog();
        }

        private void DoHelpCommand(string testCommandParameters)
        {
            PrintLine(Types.Strings.Help);
            PrintLine("");
            PrintHelpForSqlQueryCommand();
            PrintHelpForSqlNonQueryCommand();
            PrintHelpForStoreSettingCommand();
            PrintHelpForLogCommand();
            PrintLine("\t" + Types.Strings.ShellHelpClearLine1);
            PrintLine("\t\t" + Types.Strings.ShellHelpClearLine2);
        }

        private void PrintHelpForLogCommand()
        {
            PrintLine("\t" + Types.Strings.ShellHelpLogLine1);
            PrintLine("\t\t" + Types.Strings.ShellHelpLogLine2);
        }

        private void PrintHelpForSqlQueryCommand()
        {
            PrintLine("\t" + Types.Strings.ShellHelpSqlQueryLine1);
            PrintLine("\t\t" + Types.Strings.ShellHelpSqlQueryLine2);
        }

        private void PrintHelpForSqlNonQueryCommand()
        {
            PrintLine("\t" + Types.Strings.ShellHelpSqlNonQueryLine1);
            PrintLine("\t\t" + Types.Strings.ShellHelpSqlNonQueryLine2);
        }

        private void PrintHelpForStoreSettingCommand()
        {
            PrintLine("\t" + Types.Strings.ShellHelpStoreSettingLine1);
            PrintLine("\t\t" + Types.Strings.ShellHelpStoreSettingLine2);
            PrintLine("\t\t" + Types.Strings.ShellHelpStoreSettingLine3);
        }

        private void DoClearCommand(string parameters)
        {
            flowDocumentScroll.Document = new FlowDocument
            {
                PageWidth = flowDocumentScroll.ActualWidth - 5
            };
        }

        private void DoStoreSettingCommand(string storeSettingCommand)
        {
            string[] tokens = storeSettingCommand.Split(' ');
            if (tokens.Length == 0)
            {
                PrintLine(Types.Strings.ShellHelpUsage);
                PrintHelpForStoreSettingCommand();
                return;
            }

            if (tokens.Length == 1)
            {
                PrintCurrentStoreSettingValue(tokens[0]);
                return;
            }

            bool badUsage = false;
            string name = tokens[0];
            string typeName = tokens[1];
            if (typeName.ToLower() == "int")
            {
                try
                {

                    int? value = null;
                    if (tokens[2] != "null")
                        value = Convert.ToInt32(tokens[2]);
                    StoreSetting.Set(name, value);
                }
                catch
                {
                    badUsage = true;
                }
            }
            else if (typeName.ToLower() == "string")
            {
                try
                {
                    if (tokens[2] != "null")
                        StoreSetting.Set(name, storeSettingCommand.Substring(8 + name.Length));
                    else
                        StoreSetting.Set(name, (string)null);
                }
                catch
                {
                    badUsage = true;
                }
            }
            else if (typeName.ToLower() == "double")
            {
                try
                {
                    double? value = null;
                    if (tokens[2] != "null")
                        value = Convert.ToDouble(tokens[2]);
                    StoreSetting.Set(name, value);
                }
                catch
                {
                    badUsage = true;
                }
            }
            else if (typeName.ToLower() == "datetime")
            {
                try
                {
                    DateTime? value = null;
                    if (tokens[2] != "null")
                        value = Convert.ToDateTime(tokens[2]);
                    StoreSetting.Set(name, value);
                }
                catch
                {
                    badUsage = true;
                }
            }

            if (badUsage)
            {
                PrintLine(Types.Strings.ShellHelpUsage);
                PrintHelpForStoreSettingCommand();
            }
        }

        private void PrintCurrentStoreSettingValue(string settingName)
        {
            StoreSetting setting = StoreSetting.Get(settingName);
            if (setting == null)
            {
                PrintLine(Types.Strings.ShellStoreSettingNotSet + settingName);
                return;
            }

            bool found = false;
            if (setting.IntValue != null)
            {
                PrintLine(settingName + " (int) = " + setting.IntValue.Value);
                found = true;
            }
            if (setting.StringValue != null)
            {
                PrintLine(settingName + " (string) = " + setting.StringValue);
                found = true;
            }
            if (setting.FloatValue != null)
            {
                PrintLine(settingName + " (double) = " + setting.FloatValue.Value);
                found = true;
            }
            if (setting.DateTimeValue != null)
            {
                PrintLine(settingName + " (datetime) = " + setting.DateTimeValue.Value);
                found = true;
            }

            if (!found)
                PrintLine(Types.Strings.ShellStoreSettingNotSet + settingName);
        }

        /// <summary>
        /// Changes values in ConfigurationManager
        /// </summary>
        /// <param name="setCommand"></param>
        private void DoSetCommand(string setCommand)
        {
            string[] tokens = setCommand.Split(' ');

            if (tokens.Length == 0)
            {
                // ToDo: Usage:
                return;
            }

            if (tokens[0].ToLower().Equals("keyboardlock"))
                DoSetTemposLibrary(tokens);
            else if (tokens[0].ToLower().Equals("occasions"))
                DoSetSeating(tokens);
            else if (tokens[0].ToLower().Equals("printer"))
                DoSetPrinter(tokens);
        }

        private void DoSetSeating(string[] tokens)
        {
            if (tokens.Length == 1)
            {
                PrintLine(ConfigurationManager.UseKeyboardHook ?
                    Types.Strings.ShellSeatingStatusOn : Types.Strings.ShellSeatingStatusOff);
                return;
            }
            if ((tokens[1].ToLower().Equals("on")) ||
                (tokens[1].ToLower().Equals("true")))
            {
                if (!ConfigurationManager.UseSeating)
                    ConfigurationManager.SetUseSeating(true);
                PrintLine(Types.Strings.ShellSeatingStatusOn);
            }
            else if ((tokens[1].ToLower().Equals("off")) ||
                (tokens[1].ToLower().Equals("false")))
            {
                if (ConfigurationManager.UseSeating)
                    ConfigurationManager.SetUseSeating(false);
                PrintLine(Types.Strings.ShellSeatingStatusOff);
            }
        }

        private void DoSetPrinter(string[] tokens)
        {
            if (tokens.Length < 3)
            {
                // ToDo: Usage:
                return;
            }
            
            //activePosPrinterLocal = InitializePosPrinter("PrinterName");
            //activePosPrinterJournal = InitializePosPrinter("PrinterNameJournal");
            //activePosPrinterKitchen1 = InitializePosPrinter("PrinterNameKitchen1");
            //activePosPrinterKitchen2 = InitializePosPrinter("PrinterNameKitchen2");
            //activePosPrinterKitchen3 = InitializePosPrinter("PrinterNameKitchen3");

            //if (tokens[1].ToLower().Equals("journal"))
        }

        private void DoListCommand(string listCommandParameters)
        {
            string[] tokens = listCommandParameters.Split(' ');

            if (tokens.Length == 0)
            {
                // ToDo: Usage:
                return;
            }

            if ((tokens[0].ToLower().Equals("printer")) ||
                (tokens[0].ToLower().Equals("printers")))
                DoListPrinters(tokens);
            else if ((tokens[0].ToLower().Equals("table")) ||
                (tokens[0].ToLower().Equals("tables")))
                DoListTables(tokens);
        }

        private void DoListTables(string[] tokens)
        {
            // use DatabaseName (not needed)
            // select distinct name from sysobjects where xtype='U'
            SqlConnection cn = DataModelBase.GetConnection();
            SqlCommand cmd = new SqlCommand("select distinct name from sysobjects where xtype='U'", cn);
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                PrintLine(rdr[0].ToString());
            }
            rdr.Close();
            DataModelBase.FinishedWithConnection(cn);
        }

        private void DoListPrinters(string[] tokens)
        {
#if !DEMO
            Microsoft.PointOfService.DeviceCollection collection = DeviceManager.PosPrinterDeviceCollection;
            if (collection.Count == 0)
            {
                PrintLine(Strings.ShellNoPrintersFound);
                return;
            }
            for (int i=0; i < collection.Count; i++)
            {
                Microsoft.PointOfService.DeviceInfo info = collection[i];
                PrintLine("[" + i + "] " + info.Description);
            }
#endif
        }

        private void DoSetTemposLibrary(string[] tokens)
        {
            if (tokens.Length == 1)
            {
                PrintLine(ConfigurationManager.UseKeyboardHook ? 
                    Types.Strings.ShellKeyboardLockStatusOn : Types.Strings.ShellKeyboardLockStatusOff);
                return;
            }
            if ((tokens[1].ToLower().Equals("on")) ||
                (tokens[1].ToLower().Equals("true")))
            {
                if (!ConfigurationManager.UseKeyboardHook)
                {
                    ConfigurationManager.SetUseKeyboardHook(true);
                    UserControlManager.Enable();
                }
                PrintLine(Types.Strings.ShellKeyboardLockStatusOn);
            }
            else if ((tokens[1].ToLower().Equals("off")) ||
                (tokens[1].ToLower().Equals("false")))
            {
                if (ConfigurationManager.UseKeyboardHook)
                {
                    ConfigurationManager.SetUseKeyboardHook(false);
                    UserControlManager.Disable();
                }
                PrintLine(Types.Strings.ShellKeyboardLockStatusOff);
            }
        }

        private void DoSqlReaderCommand(string sqlCommand)
        {
            if (string.IsNullOrEmpty(sqlCommand))
                return;

            var list = new List<SqlModel>();

            SqlConnection cn = DataModelBase.GetConnection();
            SqlCommand cmd = new SqlCommand(sqlCommand, cn);
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new SqlModel(rdr));
            }
            rdr.Close();
            DataModelBase.FinishedWithConnection(cn);
            OutputSqlQuery(list.ToArray());
        }

        private void DoSqlNonQueryCommand(string sqlCommand)
        {
            if (sqlCommand == null)
            {
                return;
            }
            Int32 rowsAffected;
            SqlConnection cn = DataModelBase.GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = sqlCommand;
                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            DataModelBase.FinishedWithConnection(cn);
            OutputSqlNonQuery(rowsAffected != 0);
        }

        private void OutputSqlNonQuery(bool result)
        {
            PrintLine(result ?
                Types.Strings.ShellSqlNonQueryResultSuccess :
                Types.Strings.ShellSqlNonQueryResultFailed);
        }

        private void OutputSqlQuery(SqlModel[] sqlModels)
        {
            string results = "";
            foreach (SqlModel sqlModel in sqlModels)
            {
                string line = "";
                for (int j = 0; j < sqlModel.FieldCount; j++)
                {
                    if (sqlModel.Values[j] == null)
                        line += "{NULL}\t";
                    else
                        line += sqlModel.Values[j] + "\t";
                }
                results += line + Environment.NewLine;
            }
            PrintLine(results);
        }

        private void PrintLine(string text)
        {
            if (flowDocumentScroll.Document.Blocks.Count > 0)
            {
                Paragraph p = flowDocumentScroll.Document.Blocks.FirstBlock as Paragraph;
                if (p != null)
                {
                    Run r = p.Inlines.FirstInline as Run;
                    if (r != null)
                        text = r.Text + Environment.NewLine + text;
                }
                flowDocumentScroll.Document.Blocks.Clear();
            }
                
            flowDocumentScroll.Document.Blocks.Add(new Paragraph(new Run(text)));
            flowDocumentScroll.UpdateLayout();
            dragScrollViewer.ScrollToEnd();
        }

        public static PosDialogWindow CreateInDefaultWindow()
        {
            CommandShellControl control = new CommandShellControl();
            return new PosDialogWindow(control, Types.Strings.ShellWindowTitle, 830, 685);
        }

        private class SqlModel
        {
            public int FieldCount
            {
                get;
                private set;
            }

            public string[] Values
            {
                get;
                private set;
            }

            public SqlModel(SqlDataReader rdr)
            {
                FieldCount = rdr.FieldCount;
                Values = new string[FieldCount];
                for (int i = 0; i < FieldCount; i++)
                {
                    Values[i] = null;
                    if (!rdr.IsDBNull(i))
                        Values[i] = rdr[i].ToString();
                }
            }
        }
    }
}
