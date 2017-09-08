using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using PosModels.Helpers;
using PosModels.Managers;
using PosModels.Types;

namespace PosModels
{
    [ModeledDataClass()]
    public class DeliveryDriver : DataModelBase
    {
        #region Licensed Access Only
        static DeliveryDriver()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(DeliveryDriver).Assembly.GetName().GetPublicKeyToken(),
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

        [ModeledData("EmployeeId")]
        public int DriversEmployeeId
        {
            get;
            private set;
        }

        [ModeledData("CashierEmployeeId")]
        public int CashiersEmployeeId
        {
            get;
            private set;
        }

        [ModeledData("Start")]
        public DateTime StartTime
        {
            get;
            private set;
        }

        [ModeledData()]
        public DateTime? LastDelivery
        {
            get;
            private set;
        }

        [ModeledData("End")]
        public DateTime? EndTime
        {
            get;
            private set;
        }

        [ModeledData()]
        public double AmountBankrolled
        {
            get;
            private set;
        }

        private DeliveryDriver(int id, int driversEmployeeId, int cashiersEmployeeId,
            double amountBankrolled, DateTime startTime, DateTime? lastDelivery,
            DateTime? endTime)
        {
            Id = id;
            DriversEmployeeId = driversEmployeeId;
            CashiersEmployeeId = cashiersEmployeeId;
            StartTime = startTime;
            LastDelivery = lastDelivery;
            EndTime = endTime;
            AmountBankrolled = amountBankrolled;
        }

        public void SetEndTime(DateTime? endTime)
        {
            EndTime = endTime;
        }

        public void SetLastDelivery(DateTime? lastDelivery)
        {
            LastDelivery = lastDelivery;
        }

        public bool Update()
        {
            return Update(this);
        }

        #region static
        /// <summary>
        /// Add a new entry to the DeliveryDriver table
        /// </summary>
        public static DeliveryDriver Add(int driversEmployeeId, int cashiersEmployeeId,
            double amountBankrolled)
        {
            DeliveryDriver result = null;
            DateTime startTime = DateTime.Now;

            SqlConnection cn = GetConnection();
            string cmd = "AddDeliveryDriver";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@DeliveryDriverEmployeeId", SqlDbType.Int, driversEmployeeId);
                BuildSqlParameter(sqlCmd, "@DeliveryDriverCashierEmployeeId", SqlDbType.Int, cashiersEmployeeId);
                BuildSqlParameter(sqlCmd, "@DeliveryDriverStart", SqlDbType.DateTime, startTime);
                BuildSqlParameter(sqlCmd, "@DeliveryDriverEnd", SqlDbType.DateTime, null);
                BuildSqlParameter(sqlCmd, "@DeliveryDriverAmountBankrolled", SqlDbType.Float, amountBankrolled);
                BuildSqlParameter(sqlCmd, "@DeliveryDriverId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new DeliveryDriver(Convert.ToInt32(sqlCmd.Parameters["@DeliveryDriverId"].Value),
                        driversEmployeeId, cashiersEmployeeId, amountBankrolled,
                        startTime, null, null);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Delete an entry from the DeliveryDriver table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            DeliveryDriver deliveryDriver = Get(cn, id);
            if (deliveryDriver != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM DeliveryDriver WHERE DeliveryDriverId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the DeliveryDriver table
        /// </summary>
        public static DeliveryDriver Get(int id)
        {
            DeliveryDriver result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);
            return result;
        }

        private static DeliveryDriver Get(SqlConnection cn, int id)
        {
            DeliveryDriver result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM DeliveryDriver WHERE DeliveryDriverId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildDeliveryDriver(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the DeliveryDriver table
        /// </summary>
        public static IEnumerable<DeliveryDriver> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM DeliveryDriver", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildDeliveryDriver(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static IEnumerable<DeliveryDriver> GetAllActive()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM DeliveryDriver WHERE DeliveryDriverEnd IS NULL ORDER BY DeliveryDriverLastDelivery,DeliveryDriverStart DESC", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildDeliveryDriver(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static IEnumerable<int> GetAllActiveEmployeeIds()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT DeliveryDriverEmployeeId FROM DeliveryDriver WHERE DeliveryDriverEnd IS NULL", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return Convert.ToInt32(rdr[0].ToString());
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the DeliveryDriver table
        /// </summary>
        public static bool Update(DeliveryDriver deliveryDriver)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, deliveryDriver);
            FinishedWithConnection(cn);
            return result;
        }

        private static bool Update(SqlConnection cn, DeliveryDriver deliveryDriver)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE DeliveryDriver SET DeliveryDriverEmployeeId=@DeliveryDriverEmployeeId,DeliveryDriverCashierEmployeeId=@DeliveryDriverCashierEmployeeId,DeliveryDriverAmountBankrolled=@DeliveryDriverAmountBankrolled,DeliveryDriverStart=@DeliveryDriverStart,DeliveryDriverLastDelivery=@DeliveryDriverLastDelivery,DeliveryDriverEnd=@DeliveryDriverEnd WHERE DeliveryDriverId=@DeliveryDriverId";

                BuildSqlParameter(sqlCmd, "@DeliveryDriverId", SqlDbType.Int, deliveryDriver.Id);
                BuildSqlParameter(sqlCmd, "@DeliveryDriverEmployeeId", SqlDbType.Int, deliveryDriver.DriversEmployeeId);
                BuildSqlParameter(sqlCmd, "@DeliveryDriverCashierEmployeeId", SqlDbType.Int, deliveryDriver.CashiersEmployeeId);
                BuildSqlParameter(sqlCmd, "@DeliveryDriverAmountBankrolled", SqlDbType.Float, deliveryDriver.AmountBankrolled);
                BuildSqlParameter(sqlCmd, "@DeliveryDriverStart", SqlDbType.DateTime, deliveryDriver.StartTime);
                BuildSqlParameter(sqlCmd, "@DeliveryDriverLastDelivery", SqlDbType.DateTime, deliveryDriver.LastDelivery);
                BuildSqlParameter(sqlCmd, "@DeliveryDriverEnd", SqlDbType.DateTime, deliveryDriver.EndTime);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Build a DeliveryDriver object from a SqlDataReader object
        /// </summary>
        private static DeliveryDriver BuildDeliveryDriver(SqlDataReader rdr)
        {
            return new DeliveryDriver(
                GetId(rdr),
                GetEmployeeId(rdr),
                GetCashierId(rdr),
                GetAmountBankrolled(rdr),
                GetStartTime(rdr),
                GetLastDelivery(rdr),
                GetEndTime(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int GetEmployeeId(SqlDataReader rdr)
        {
            return rdr.GetInt32(1);
        }

        private static int GetCashierId(SqlDataReader rdr)
        {
            return rdr.GetInt32(2);
        }

        private static double GetAmountBankrolled(SqlDataReader rdr)
        {
            return rdr.GetDouble(3);
        }

        private static DateTime GetStartTime(SqlDataReader rdr)
        {
            return rdr.GetDateTime(4);
        }

        private static DateTime? GetLastDelivery(SqlDataReader rdr)
        {
            DateTime? lastDelivery = null;
            if (!rdr.IsDBNull(5))
                lastDelivery = rdr.GetDateTime(5);
            return lastDelivery;
        }

        private static DateTime? GetEndTime(SqlDataReader rdr)
        {
            DateTime? endTime = null;
            if (!rdr.IsDBNull(6))
                endTime = rdr.GetDateTime(6);
            return endTime;
        }
        #endregion

    }
}
