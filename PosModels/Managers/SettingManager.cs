using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PosModels;

namespace PosModels.Managers
{
    public static class SettingManager
    {
        private static Dictionary<int, StoreSetting> StoreSettings =
            new Dictionary<int, StoreSetting>();

        private static Dictionary<int, EmployeeSetting> EmployeeSettings =
            new Dictionary<int, EmployeeSetting>();

        static SettingManager()
        {
            // Don't intiialize
        }

        public static int? GetInt32(string storeSettingName)
        {
            StoreSetting setting = GetStoreSetting(storeSettingName);
            return setting.IntValue;
        }

        public static DateTime? GetDateTime(string storeSettingName)
        {
            StoreSetting setting = GetStoreSetting(storeSettingName);
            return setting.DateTimeValue;
        }

        public static double? GetDouble(string storeSettingName)
        {
            StoreSetting setting = GetStoreSetting(storeSettingName);
            return setting.FloatValue;
        }

        public static string GetString(string storeSettingName)
        {
            StoreSetting setting = GetStoreSetting(storeSettingName);
            return setting.StringValue;
        }

        /// <summary>
        /// Get a single StoreSetting from the StoreSetting table
        /// </summary>
        public static StoreSetting GetStoreSetting(string storeSettingName)
        {
            // Invalid Id
            if (storeSettingName == null)
                return null;

            // Scan existing
            StoreSetting storeSetting = StoreSetting.Get(storeSettingName);
            if ((StoreSettings.Count > 0) && (StoreSettings.Keys.Contains(storeSetting.Id)))
            {
                StoreSetting.Refresh(StoreSettings[storeSetting.Id], storeSetting);
                return StoreSettings[storeSetting.Id];
            }

            // Not found, let's check the database
            if (storeSetting != null)
            {
                StoreSettings.Add(storeSetting.Id, storeSetting);
                return storeSetting;
            }
            return null;
        }

        /// <summary>
        /// Get a single EmployeeSetting from the EmployeeSetting table
        /// </summary>
        public static EmployeeSetting GetEmployeeSetting(int employeeId, string employeeSettingName)
        {
            // Invalid Id
            if (employeeSettingName == null)
                return null;

            // Scan existing
            EmployeeSetting employeeSetting = EmployeeSetting.Get(employeeId, employeeSettingName);
            if ((EmployeeSettings.Count > 0) && (EmployeeSettings.Keys.Contains(employeeSetting.Id)))
            {
                EmployeeSetting.Refresh(EmployeeSettings[employeeSetting.Id], employeeSetting);
                return EmployeeSettings[employeeSetting.Id];
            }

            // Not found, let's check the database
            if (employeeSetting != null)
            {
                EmployeeSettings.Add(employeeSetting.Id, employeeSetting);
                return employeeSetting;
            }
            return null;
        }

        public static void SetStoreSetting(string settingName, string stringValue)
        {
            StoreSetting.Set(settingName, stringValue);
            StoreSetting managed = GetUpdatedManagedStoreSetting(StoreSetting.Get(settingName));
        }

        public static void SetStoreSetting(string settingName, int? intValue)
        {
            StoreSetting.Set(settingName, intValue);
            StoreSetting managed = GetUpdatedManagedStoreSetting(StoreSetting.Get(settingName));
        }

        public static void SetStoreSetting(string settingName, double? doubleValue)
        {
            StoreSetting.Set(settingName, doubleValue);
            StoreSetting managed = GetUpdatedManagedStoreSetting(StoreSetting.Get(settingName));
        }

        public static void SetStoreSetting(string settingName, DateTime? dateTimeValue)
        {
            StoreSetting.Set(settingName, dateTimeValue);
            StoreSetting managed = GetUpdatedManagedStoreSetting(StoreSetting.Get(settingName));
        }

        public static void SetEmployeeSetting(int employeeId, string settingName, string stringValue)
        {
            EmployeeSetting.Set(employeeId, settingName, stringValue);
            EmployeeSetting managed = GetUpdatedManagedEmployeeSetting(EmployeeSetting.Get(employeeId, settingName));
        }

        public static void SetEmployeeSetting(int employeeId, string settingName, int? intValue)
        {
            EmployeeSetting.Set(employeeId, settingName, intValue);
            EmployeeSetting managed = GetUpdatedManagedEmployeeSetting(EmployeeSetting.Get(employeeId, settingName));
        }

        public static void SetEmployeeSetting(int employeeId, string settingName, double? doubleValue)
        {
            EmployeeSetting.Set(employeeId, settingName, doubleValue);
            EmployeeSetting managed = GetUpdatedManagedEmployeeSetting(EmployeeSetting.Get(employeeId, settingName));
        }

        public static void SetEmployeeSetting(int employeeId, string settingName, DateTime? dateTimeValue)
        {
            EmployeeSetting.Set(employeeId, settingName, dateTimeValue);
            EmployeeSetting managed = GetUpdatedManagedEmployeeSetting(EmployeeSetting.Get(employeeId, settingName));
        }

        public static void RemoveAllSettingsForEmployee(int employeeId)
        {
            EmployeeSetting.RemoveAllSettings(employeeId);
        }

        /// <summary>
        /// Gets the managed StoreSetting, or creates a managed StoreSetting if
        /// one doesn't exist.
        /// </summary>
        private static StoreSetting GetUpdatedManagedStoreSetting(StoreSetting storeSetting)
        {
            if ((StoreSettings.Count == 0) || !StoreSettings.Keys.Contains(storeSetting.Id))
            {
                // Setting is not a managed instance yet
                StoreSettings.Add(storeSetting.Id, storeSetting);
            }
            else
            {
                // Refresh the managed StoreSetting with the current StoreSetting instance
                StoreSetting.Refresh(StoreSettings[storeSetting.Id], storeSetting);
            }
            return StoreSettings[storeSetting.Id];     
        }

        /// <summary>
        /// Gets the managed EmployeeSetting, or creates a managed EmployeeSetting if
        /// one doesn't exist.
        /// </summary>
        private static EmployeeSetting GetUpdatedManagedEmployeeSetting(EmployeeSetting employeeSetting)
        {
            if ((EmployeeSettings.Count == 0) || !EmployeeSettings.Keys.Contains(employeeSetting.Id))
            {
                // Setting is not a managed instance yet
                EmployeeSettings.Add(employeeSetting.Id, employeeSetting);
            }
            else
            {
                // Refresh the managed EmployeeSetting with the current employeeSetting instance
                EmployeeSetting.Refresh(EmployeeSettings[employeeSetting.Id], employeeSetting);
            }
            return EmployeeSettings[employeeSetting.Id];
        }

    }
}
