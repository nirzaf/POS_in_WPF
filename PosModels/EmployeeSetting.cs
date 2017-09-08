using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using PosModels.Helpers;
using PosModels.Managers;
using PosModels.Types;

namespace PosModels
{
    [ModeledDataClass()]
    public class EmployeeSetting : DataModelBase
    {
        #region Licensed Access Only
        static EmployeeSetting()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(EmployeeSetting).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosModels.dll");
            }
#endif
        }
        #endregion

        [ModeledData()]
        public int Id
        {
            get;
            private set;
        }

        [ModeledData()]
        public int EmployeeId
        {
            get;
            private set;
        }

        [ModeledData()]
        public string Name
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataNullable()]
        public string StringValue
        {
            get;
            private set;
        }

        [ModeledData()]
        public int? IntValue
        {
            get;
            private set;
        }

        [ModeledData()]
        public double? FloatValue
        {
            get;
            private set;
        }

        [ModeledData()]
        public DateTime? DateTimeValue
        {
            get;
            private set;
        }

        private EmployeeSetting(int id, int employeeId,
            string settingName, string stringValue, int? intValue,
            double? doubleValue, DateTime? dateTimeValue)
        {
            Id = id;
            EmployeeId = employeeId;
            Name = settingName;
            StringValue = stringValue;
            IntValue = intValue;
            FloatValue = doubleValue;
            DateTimeValue = dateTimeValue;
        }

        #region static
        public static EmployeeSetting Get(int employeeId, string settingName)
        {
            EmployeeSetting result = null;
            SqlConnection cn = GetConnection();
            result = Get(cn, employeeId, settingName);
            FinishedWithConnection(cn);
            return result;
        }

        private static EmployeeSetting Get(SqlConnection cn, int employeeId, string settingName)
        {
            EmployeeSetting result = null;
            if (settingName == null)
                return null;
            for (int i = 0; i < 2; i++)
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM EmployeeSetting WHERE (EmployeeSettingEmployeeId=@EmployeeSettingEmployeeId) AND (EmployeeSettingName LIKE @EmployeeSettingName)", cn))
                {
                    BuildSqlParameter(cmd, "@EmployeeSettingEmployeeId", SqlDbType.Int, employeeId);
                    BuildSqlParameter(cmd, "@EmployeeSettingName", SqlDbType.Text, settingName);
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            result = BuildEmployeeSetting(rdr);
                        }
                    }
                }
                if (result == null)
                    EmployeeSetting.Insert(cn, employeeId, settingName, (string)null);
                else
                    break;
            }
            return result;
        }

        public static void Set(int employeeId, string settingName, string value)
        {
            SqlConnection cn = GetConnection();
            EmployeeSetting currentValue = Get(cn, employeeId, settingName);
            if (currentValue != null)
            {
                if (value != null)
                {
                    // Update
                    Update(cn, currentValue.Id, value);
                }
                else
                {
                    // Delete
                    Delete(cn, currentValue.Id);
                }
            }
            else
            {
                if (value != null)
                {
                    // Insert
                    Insert(cn, employeeId, settingName, value);
                }
            }
            FinishedWithConnection(cn);
        }

        public static void Set(int employeeId, string settingName, int? intValue)
        {
            SqlConnection cn = GetConnection();
            EmployeeSetting currentValue = Get(cn, employeeId, settingName);
            if (currentValue != null)
            {
                if ((intValue != null) || !intValue.HasValue)
                {
                    // Update
                    Update(cn, currentValue.Id, intValue);
                }
                else
                {
                    // Delete
                    Delete(cn, currentValue.Id);
                }
            }
            else
            {
                if (intValue != null)
                {
                    // Insert
                    Insert(cn, employeeId, settingName, intValue);
                }
            }
            FinishedWithConnection(cn);
        }

        public static void Set(int employeeId, string settingName, double? doubleValue)
        {
            SqlConnection cn = GetConnection();
            EmployeeSetting currentValue = Get(cn, employeeId, settingName);
            if (currentValue != null)
            {
                if ((doubleValue != null) || !doubleValue.HasValue)
                {
                    // Update
                    Update(cn, currentValue.Id, doubleValue);
                }
                else
                {
                    // Delete
                    Delete(cn, currentValue.Id);
                }
            }
            else
            {
                if (doubleValue != null)
                {
                    // Insert
                    Insert(cn, employeeId, settingName, doubleValue);
                }
            }
            FinishedWithConnection(cn);
        }

        public static void Set(int employeeId, string settingName, DateTime? dateTimeValue)
        {
            SqlConnection cn = GetConnection();
            EmployeeSetting currentValue = Get(cn, employeeId, settingName);
            if (currentValue != null)
            {
                if ((dateTimeValue != null) || !dateTimeValue.HasValue)
                {
                    // Update
                    Update(cn, currentValue.Id, dateTimeValue);
                }
                else
                {
                    // Delete
                    Delete(cn, currentValue.Id);
                }
            }
            else
            {
                if (dateTimeValue != null)
                {
                    // Insert
                    Insert(cn, employeeId, settingName, dateTimeValue);
                }
            }
            FinishedWithConnection(cn);
        }

        public static bool RemoveAllSettings(int employeeId)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM EmployeeSetting WHERE EmployeeSettingEmployeeId=@EmployeeSettingEmployeeId";

                BuildSqlParameter(sqlCmd, "@EmployeeSettingEmployeeId", SqlDbType.Int, employeeId);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        private static void Delete(SqlConnection cn, int id)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM EmployeeSetting WHERE EmployeeSettingId=@EmployeeSettingId";

                BuildSqlParameter(sqlCmd, "@EmployeeSettingId", SqlDbType.Int, id);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
        }

        private static void Update(SqlConnection cn, int id, string stringValue)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                // UPDATE Setting SET SettingValue=stringValue WHERE SettingId=setting
                sqlCmd.CommandText = "UPDATE EmployeeSetting SET EmployeeSettingStringValue=@EmployeeSettingStringValue WHERE EmployeeSettingId=@EmployeeSettingId";

                BuildSqlParameter(sqlCmd, "@EmployeeSettingId", SqlDbType.Int, id);
                BuildSqlParameter(sqlCmd, "@EmployeeSettingStringValue", SqlDbType.Text, stringValue);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
        }

        private static void Update(SqlConnection cn, int id, int? intValue)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                // UPDATE Setting SET SettingValue=stringValue WHERE SettingId=setting
                sqlCmd.CommandText = "UPDATE EmployeeSetting SET EmployeeSettingIntValue=@EmployeeSettingIntValue WHERE EmployeeSettingId=@EmployeeSettingId";

                BuildSqlParameter(sqlCmd, "@EmployeeSettingId", SqlDbType.Int, id);
                BuildSqlParameter(sqlCmd, "@EmployeeSettingIntValue", SqlDbType.Int, intValue);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
        }

        private static void Update(SqlConnection cn, int id, double? doubleValue)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                // UPDATE Setting SET SettingValue=stringValue WHERE SettingId=setting
                sqlCmd.CommandText = "UPDATE EmployeeSetting SET EmployeeSettingFloatValue=@EmployeeSettingFloatValue WHERE EmployeeSettingId=@EmployeeSettingId";

                BuildSqlParameter(sqlCmd, "@EmployeeSettingId", SqlDbType.Int, id);
                BuildSqlParameter(sqlCmd, "@EmployeeSettingFloatValue", SqlDbType.Float, doubleValue);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
        }

        private static void Update(SqlConnection cn, int id, DateTime? dateTimeValue)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE EmployeeSetting SET EmployeeSettingDateTimeValue=@EmployeeSettingDateTimeValue WHERE EmployeeSettingId=@EmployeeSettingId";

                BuildSqlParameter(sqlCmd, "@EmployeeSettingId", SqlDbType.Int, id);
                BuildSqlParameter(sqlCmd, "@EmployeeSettingDateTimeValue", SqlDbType.DateTime, dateTimeValue);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
        }

        private static void Insert(SqlConnection cn, int employeeId, string settingName, string stringValue)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "INSERT INTO EmployeeSetting (EmployeeSettingEmployeeId, EmployeeSettingName, EmployeeSettingStringValue) VALUES (@EmployeeSettingEmployeeId, @EmployeeSettingName, @EmployeeSettingStringValue)";
                BuildSqlParameter(sqlCmd, "@EmployeeSettingEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(sqlCmd, "@EmployeeSettingName", SqlDbType.Text, settingName);
                BuildSqlParameter(sqlCmd, "@EmployeeSettingStringValue", SqlDbType.Text, stringValue);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            //return (rowsAffected != 0);
        }

        private static void Insert(SqlConnection cn, int employeeId, string settingName, int? intValue)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "INSERT INTO EmployeeSetting (EmployeeSettingEmployeeId, EmployeeSettingName, EmployeeSettingIntValue) VALUES (@EmployeeSettingEmployeeId, @EmployeeSettingName, @EmployeeSettingIntValue)";
                BuildSqlParameter(sqlCmd, "@EmployeeSettingEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(sqlCmd, "@EmployeeSettingName", SqlDbType.Text, settingName);
                BuildSqlParameter(sqlCmd, "@EmployeeSettingIntValue", SqlDbType.Int, intValue);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            //return (rowsAffected != 0);
        }

        private static void Insert(SqlConnection cn, int employeeId, string settingName, double? doubleValue)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "INSERT INTO EmployeeSetting (EmployeeSettingEmployeeId, EmployeeSettingName, EmployeeSettingFloatValue) VALUES (@EmployeeSettingEmployeeId, @EmployeeSettingName, @EmployeeSettingFloatValue)";
                BuildSqlParameter(sqlCmd, "@EmployeeSettingEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(sqlCmd, "@EmployeeSettingName", SqlDbType.Text, settingName);
                BuildSqlParameter(sqlCmd, "@EmployeeSettingFloatValue", SqlDbType.Float, doubleValue);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            //return (rowsAffected != 0);
        }

        private static void Insert(SqlConnection cn, int employeeId, string settingName, DateTime? dateTimeValue)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "INSERT INTO EmployeeSetting (EmployeeSettingEmployeeId, EmployeeSettingName, EmployeeSettingDateTimeValue) VALUES (@EmployeeSettingEmployeeId, @EmployeeSettingName, @EmployeeSettingDateTimeValue)";
                BuildSqlParameter(sqlCmd, "@EmployeeSettingEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(sqlCmd, "@EmployeeSettingName", SqlDbType.Text, settingName);
                BuildSqlParameter(sqlCmd, "@EmployeeSettingDateTimeValue", SqlDbType.DateTime, dateTimeValue);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            //return (rowsAffected != 0);
        }

        public static void Refresh(EmployeeSetting setting)
        {
            if (setting != null)
                Refresh(setting, EmployeeSetting.Get(setting.EmployeeId, setting.Name));
        }

        public static void Refresh(EmployeeSetting setting, EmployeeSetting tempSetting)
        {
            if ((setting == null) || (tempSetting == null))
                return;
            setting.Name = tempSetting.Name;
            setting.EmployeeId = tempSetting.EmployeeId;
            setting.StringValue = tempSetting.StringValue;
            setting.IntValue = tempSetting.IntValue;
            setting.FloatValue = tempSetting.FloatValue;
            setting.DateTimeValue = tempSetting.DateTimeValue;
        }

        private static EmployeeSetting BuildEmployeeSetting(SqlDataReader rdr)
        {
            return new EmployeeSetting(
                GetId(rdr),
                GetEmployeeId(rdr),
                GetSettingName(rdr),
                GetStringValue(rdr),
                GetIntValue(rdr),
                GetDoubleValue(rdr),
                GetDateTimeValue(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int GetEmployeeId(SqlDataReader rdr)
        {
            return rdr.GetInt32(1);
        }

        private static string GetSettingName(SqlDataReader rdr)
        {
            return rdr.GetString(2);
        }

        private static string GetStringValue(SqlDataReader rdr)
        {
            string stringValue = null;
            if (!rdr.IsDBNull(3))
                stringValue = rdr.GetString(3);
            return stringValue;
        }

        private static int? GetIntValue(SqlDataReader rdr)
        {
            int? intValue = null;
            if (!rdr.IsDBNull(4))
                intValue = rdr.GetInt32(4);
            return intValue;
        }

        private static double? GetDoubleValue(SqlDataReader rdr)
        {
            double? doubleValue = null;
            if (!rdr.IsDBNull(5))
                doubleValue = rdr.GetDouble(5);
            return doubleValue;
        }

        private static DateTime? GetDateTimeValue(SqlDataReader rdr)
        {
            DateTime? dateTimeValue = null;
            if (!rdr.IsDBNull(6))
                dateTimeValue = rdr.GetDateTime(6);
            return dateTimeValue;
        }
        #endregion

    }
}
