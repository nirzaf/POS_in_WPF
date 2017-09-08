using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using PosControls;
using PosModels;
using TemPOS.Types;
using PosModels.Managers;
using PosModels.Types;
using Strings = TemPOS.Types.Strings;
using System.Reflection;

namespace TemPOS.Helpers
{
    public static class SqlServerSetup
    {
        public static void FinishInstall()
        {
            //MessageBox.Show(Strings.AboutToInstallDatabase);

            ExecuteEmbeddedSqlScript("TemPOS.Resources.EnableClrScript.sql", false);
            if (!DataModelBase.DatabaseExists("TemPOS"))
            {
                //System.Windows.Forms.Form window = CreateNotificationWindow();
                //window.Show();
                try
                {
                    //ExecuteEmbeddedSqlScript("TemPOS.Resources.EnableClrScript.sql");
                    ExecuteEmbeddedSqlScript("TemPOS.Resources.InstallDatabaseScript.sql", false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Types.Strings.DatabaseInstallationException);
                    return;
                }
                //window.Hide();
            }

            // Warning: Do not apply database patches at install time,
            //          instead apply patches the first time the app starts.
        }

        public static void ApplyDatabasePatches()
        {
            string patchName = Strings.None;
            StoreSetting storeSetting = SettingManager.GetStoreSetting("DatabasePatchVersion");
            int currentPatchVersion = 0;
            if ((storeSetting != null) && storeSetting.IntValue.HasValue)
                currentPatchVersion = storeSetting.IntValue.Value;
            try
            {
                if (currentPatchVersion < 1)
                {
                    // Adds IngredientParQuantity to Ingredient table
                    patchName = "Patch #1";
                    ExecuteEmbeddedSqlScript("TemPOS.Resources.AddIngredientParQuantity.sql", true);
                    currentPatchVersion = 1;
                }

                if (currentPatchVersion < 2)
                {
                    // Flags database as trustworthy
                    patchName = "Patch #2";
                    DataModelBase.ExecuteNonQuery("ALTER DATABASE " +
                                                   LocalSetting.DatabaseServerDatabaseName + " SET TRUSTWORTHY ON" +
                                                   Environment.NewLine + "GO");
                    currentPatchVersion = 2;
                }

                if (currentPatchVersion < 3)
                {
                    // Adds TicketCouponPseudoEmployeeId, and TicketDiscountPseudoEmployeeId
                    // Removes TicketDiscountEmployeeId
                    patchName = "Patch #3";
                    ExecuteEmbeddedSqlScript("TemPOS.Resources.AddPseudoEmployeeId.sql");
                    currentPatchVersion = 3;
                }

                if (currentPatchVersion < 4)
                {
                    // Adds the ItemGroup table, and ItemIsGrouping and TicketItemParentTicketItemId fields
                    patchName = "Patch #4";
                    ExecuteEmbeddedSqlScript("TemPOS.Resources.AddItemGrouping-Part1.sql");
                    try
                    {
                        ExecuteEmbeddedSqlScript("TemPOS.Resources.AddItemGrouping-Part2.sql");
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Message.Contains("identical to an assembly that is already registered"))
                            SqlConnection.ClearAllPools();
                    }
                    ExecuteEmbeddedSqlScript("TemPOS.Resources.AddItemGrouping-Part3.sql");
                    currentPatchVersion = 4;
                }

                if (currentPatchVersion < 5)
                {
                    // Adds TicketItemFireTime
                    patchName = "Patch #5";
                    ExecuteEmbeddedSqlScript("TemPOS.Resources.AddTicketItemFireTime.sql");
                    currentPatchVersion = 5;
                }

                if (currentPatchVersion < 6)
                {
                    // Adds TicketItemFireTime
                    patchName = "Patch #6";
                    ExecuteEmbeddedSqlScript("TemPOS.Resources.AddItemButtonImage.sql");
                    currentPatchVersion = 6;
                }
            }
            catch (Exception ex)
            {
                try
                {
                    SettingManager.SetStoreSetting("DatabasePatchVersion", currentPatchVersion);
                }
                catch (Exception)
                {
                }
                PosDialogWindow.ShowDialog(ex.Message, Types.Strings.DatabasePatchException + patchName);
                return;
            }
            SettingManager.SetStoreSetting("DatabasePatchVersion", currentPatchVersion);
        }

        private static void ExecuteEmbeddedSqlScript(string path, bool useDatabase = true)
        {
            // Get the PosControls assembly
            var assemblyName = path.Substring(0, path.IndexOf('.'));
            var assembly = GetAssemblyByName(assemblyName);
            using (var stream = assembly.GetManifestResourceStream(path))
            {
                if (stream != null)
                    DataModelBase.ExecuteSqlScript(new StreamReader(stream), useDatabase);
            }
        }

        private static Assembly GetAssemblyByName(String assemblyName)
        {
            foreach (Assembly currentAssembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                string name = currentAssembly.ToString();
                name = name.Substring(0, name.IndexOf(','));
                if (name == assemblyName)
                    return currentAssembly;
            }
            return null;
        }
    }
}
