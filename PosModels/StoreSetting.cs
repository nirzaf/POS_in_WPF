using System;
using System.Data;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using PosModels.Types;

namespace PosModels
{
    [Table]
    public class StoreSetting : DataModelBase
    {
        #region Licensed Access Only
// ReSharper disable EmptyConstructor
        static StoreSetting()
// ReSharper restore EmptyConstructor
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(DirectDepositTransaction).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosModels.dll");
            }
#endif
        }
        #endregion

        [Column]
        public int Id
        {
            get;
            private set;
        }

        [Column(CanBeNull = false)]
        public string Name
        {
            get;
            private set;
        }

        [Column(CanBeNull = true)]
        public string StringValue
        {
            get;
            private set;
        }

        [Column]
        public int? IntValue
        {
            get;
            private set;
        }

        [Column]
        public double? FloatValue
        {
            get;
            private set;
        }

        [Column]
        public DateTime? DateTimeValue
        {
            get;
            private set;
        }

        private StoreSetting(int id, string settingName, string stringValue,
            int? intValue, double? doubleValue, DateTime? dateTimeValue)
        {
            Id = id;
            Name = settingName;
            StringValue = stringValue;
            IntValue = intValue;
            FloatValue = doubleValue;
            DateTimeValue = dateTimeValue;
        }

        #region static
        public static StoreSetting Get(string settingName)
        {
            SqlConnection cn = GetConnection();
            StoreSetting result = Get(cn, settingName);
            FinishedWithConnection(cn);
            return result;
        }

        private static StoreSetting Get(SqlConnection cn, string settingName, bool recursionProtected = false)
        {
            StoreSetting result = null;
            if (settingName == null)
                return null;
            using (var cmd = new SqlCommand("SELECT * FROM StoreSetting WHERE StoreSettingName LIKE @StoreSettingName", cn))
            {
                BuildSqlParameter(cmd, "@StoreSettingName", SqlDbType.Text, settingName);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildStoreSetting(rdr);
                    }
                }
            }
            if ((result == null) && !recursionProtected)
            {
                Insert(cn, settingName, null);
                result = Get(cn, settingName, true);
            }
            return result;
        }

        public static int Increment(int settingId)
        {
            int result = -1;
            SqlConnection cn = GetConnection();
            using (var sqlCmd = new SqlCommand("IncrementSetting", cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@StoreSettingId", SqlDbType.Int, settingId);
                BuildSqlParameter(sqlCmd, "@ResultValue", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = Convert.ToInt32(sqlCmd.Parameters["@ResultValue"].Value);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        public static void Set(string settingName, string value)
        {
            SqlConnection cn = GetConnection();
            StoreSetting currentValue = Get(cn, settingName);
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
                    Insert(cn, settingName, value);
                }
            }
            FinishedWithConnection(cn);
        }

        public static void Set(string settingName, int? intValue)
        {
            SqlConnection cn = GetConnection();
            StoreSetting currentValue = Get(cn, settingName);
            if (currentValue != null)
            {
                if (intValue != null)
                {
                    // Update
                    Update(cn, currentValue.Id, intValue.Value);
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
                    Insert(cn, settingName, intValue.Value);
                }
            }
            FinishedWithConnection(cn);
        }

        public static void Set(string settingName, double? doubleValue)
        {
            SqlConnection cn = GetConnection();
            StoreSetting currentValue = Get(cn, settingName);
            if (currentValue != null)
            {
                if (doubleValue != null)
                {
                    // Update
                    Update(cn, currentValue.Id, doubleValue.Value);
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
                    Insert(cn, settingName, doubleValue.Value);
                }
            }
            FinishedWithConnection(cn);
        }

        public static void Set(string settingName, DateTime? dateTimeValue)
        {
            SqlConnection cn = GetConnection();
            StoreSetting currentValue = Get(cn, settingName);
            if (currentValue != null)
            {
                if (dateTimeValue != null)
                {
                    // Update
                    Update(cn, currentValue.Id, dateTimeValue.Value);
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
                    Insert(cn, settingName, dateTimeValue.Value);
                }
            }
            FinishedWithConnection(cn);
        }

        private static void Delete(SqlConnection cn, int id)
        {
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM StoreSetting WHERE StoreSettingId=@StoreSettingId";
                BuildSqlParameter(sqlCmd, "@StoreSettingId", SqlDbType.Int, id);
                sqlCmd.ExecuteNonQuery();
            }
        }

        private static void Update(SqlConnection cn, int id, string stringValue)
        {
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE StoreSetting SET StoreSettingStringValue=@StoreSettingStringValue WHERE StoreSettingId=@StoreSettingId";
                BuildSqlParameter(sqlCmd, "@StoreSettingId", SqlDbType.Int, id);
                BuildSqlParameter(sqlCmd, "@StoreSettingStringValue", SqlDbType.Text, stringValue);
                sqlCmd.ExecuteNonQuery();
            }
        }

        private static void Update(SqlConnection cn, int id, int intValue)
        {
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE StoreSetting SET StoreSettingIntValue=@StoreSettingIntValue WHERE StoreSettingId=@StoreSettingId";
                BuildSqlParameter(sqlCmd, "@StoreSettingId", SqlDbType.Int, id);
                BuildSqlParameter(sqlCmd, "@StoreSettingIntValue", SqlDbType.Int, intValue);
                sqlCmd.ExecuteNonQuery();
            }
        }

        private static void Update(SqlConnection cn, int id, double doubleValue)
        {
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE StoreSetting SET StoreSettingFloatValue=@StoreSettingFloatValue WHERE StoreSettingId=@StoreSettingId";
                BuildSqlParameter(sqlCmd, "@StoreSettingId", SqlDbType.Int, id);
                BuildSqlParameter(sqlCmd, "@StoreSettingFloatValue", SqlDbType.Float, doubleValue);
                sqlCmd.ExecuteNonQuery();
            }
        }

        private static void Update(SqlConnection cn, int id, DateTime dateTimeValue)
        {
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE StoreSetting SET StoreSettingDateTimeValue=@StoreSettingDateTimeValue WHERE StoreSettingId=@StoreSettingId";
                BuildSqlParameter(sqlCmd, "@StoreSettingId", SqlDbType.Int, id);
                BuildSqlParameter(sqlCmd, "@StoreSettingDateTimeValue", SqlDbType.DateTime, dateTimeValue);
                sqlCmd.ExecuteNonQuery();
            }
        }

        private static void Insert(SqlConnection cn, string settingName, string stringValue)
        {
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "INSERT INTO StoreSetting (StoreSettingName, StoreSettingStringValue) VALUES (@StoreSettingName, @StoreSettingStringValue)";
                BuildSqlParameter(sqlCmd, "@StoreSettingName", SqlDbType.Text, settingName);
                BuildSqlParameter(sqlCmd, "@StoreSettingStringValue", SqlDbType.Text, stringValue);
                sqlCmd.ExecuteNonQuery();
            }
        }

        private static void Insert(SqlConnection cn, string settingName, int intValue)
        {
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "INSERT INTO StoreSetting (StoreSettingName, StoreSettingIntValue) VALUES (@StoreSettingName, @StoreSettingIntValue)";
                BuildSqlParameter(sqlCmd, "@StoreSettingName", SqlDbType.Text, settingName);
                BuildSqlParameter(sqlCmd, "@StoreSettingIntValue", SqlDbType.Int, intValue);
                sqlCmd.ExecuteNonQuery();
            }
        }

        private static void Insert(SqlConnection cn, string settingName, double doubleValue)
        {
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "INSERT INTO StoreSetting (StoreSettingName, StoreSettingFloatValue) VALUES (@StoreSettingName, @StoreSettingFloatValue)";
                BuildSqlParameter(sqlCmd, "@StoreSettingName", SqlDbType.Text, settingName);
                BuildSqlParameter(sqlCmd, "@StoreSettingFloatValue", SqlDbType.Float, doubleValue);
                sqlCmd.ExecuteNonQuery();
            }
        }

        private static void Insert(SqlConnection cn, string settingName, DateTime dateTimeValue)
        {
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "INSERT INTO StoreSetting (StoreSettingName, StoreSettingDateTimeValue) VALUES (@StoreSettingName, @StoreSettingDateTimeValue)";
                BuildSqlParameter(sqlCmd, "@StoreSettingName", SqlDbType.Text, settingName);
                BuildSqlParameter(sqlCmd, "@StoreSettingDateTimeValue", SqlDbType.DateTime, dateTimeValue);
                sqlCmd.ExecuteNonQuery();
            }
        }

        public static void Refresh(StoreSetting setting)
        {
            if (setting != null)
                Refresh(setting, Get(setting.Name));
        }

        public static void Refresh(StoreSetting setting, StoreSetting tempSetting)
        {
            if ((setting == null) || (tempSetting == null))
                return;
            setting.Name = tempSetting.Name;
            setting.StringValue = tempSetting.StringValue;
            setting.IntValue = tempSetting.IntValue;
            setting.FloatValue = tempSetting.FloatValue;
            setting.DateTimeValue = tempSetting.DateTimeValue;
        }

        private static StoreSetting BuildStoreSetting(SqlDataReader rdr)
        {
            return new StoreSetting(
                GetId(rdr),
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

        private static string GetSettingName(SqlDataReader rdr)
        {
            return rdr.GetString(1);
        }

        private static string GetStringValue(SqlDataReader rdr)
        {
            string stringValue = null;
            if (!rdr.IsDBNull(2))
            {
                string value = rdr.GetString(2);
                if (!value.Equals(""))
                    stringValue = value;
            }
            return stringValue;
        }

        private static int? GetIntValue(SqlDataReader rdr)
        {
            int? intValue = null;
            if (!rdr.IsDBNull(3))
                intValue = rdr.GetInt32(3);
            return intValue;
        }

        private static double? GetDoubleValue(SqlDataReader rdr)
        {
            double? doubleValue = null;
            if (!rdr.IsDBNull(4))
                doubleValue = rdr.GetDouble(4);
            return doubleValue;
        }

        private static DateTime? GetDateTimeValue(SqlDataReader rdr)
        {
            DateTime? dateTimeValue = null;
            if (!rdr.IsDBNull(5))
                dateTimeValue = rdr.GetDateTime(5);
            return dateTimeValue;
        }
        #endregion

    }
}
