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
    public class DeliveryRestriction : DataModelBase
    {
        #region Licensed Access Only
        static DeliveryRestriction()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(DeliveryRestriction).Assembly.GetName().GetPublicKeyToken(),
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
        public int DeliveryAreaId
        {
            get;
            private set;
        }

        [ModeledData()]
        public int CustomerId
        {
            get;
            private set;
        }

        [ModeledData()]
        public TimeSpan? StartTime
        {
            get;
            private set;
        }

        [ModeledData()]
        public TimeSpan? EndTime
        {
            get;
            private set;
        }

        [ModeledData()]
        public bool OnlyLimitAmount
        {
            get;
            private set;
        }

        [ModeledData()]
        public double AmountLimit
        {
            get;
            private set;
        }

        private DeliveryRestriction(int id, int deliveryAreaId, int customerId, TimeSpan? startTime, TimeSpan? endTime,
            bool onlyLimitAmount, double amountLimit)
        {
            Id = id;
            DeliveryAreaId = deliveryAreaId;
            CustomerId = customerId;
            StartTime = startTime;
            EndTime = endTime;
            OnlyLimitAmount = onlyLimitAmount;
            AmountLimit = amountLimit;
        }

        #region static
        /// <summary>
        /// Add a new entry to the DeliveryRestriction table
        /// </summary>
        public static DeliveryRestriction Add(int deliveryAreaId, int customerId, TimeSpan? startTime, TimeSpan? endTime,
            bool onlyLimitAmount, double amountLimit)
        {
            DeliveryRestriction result = null;
            DateTime purchaseTime = DateTime.Now;

            SqlConnection cn = GetConnection();
            string cmd = "AddDeliveryRestriction";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@DeliveryRestrictionDeliveryAreaId", SqlDbType.Int, deliveryAreaId);
                BuildSqlParameter(sqlCmd, "@DeliveryRestrictionCustomerId", SqlDbType.Int, customerId);
                BuildSqlParameter(sqlCmd, "@DeliveryRestrictionStartTime", SqlDbType.Time, startTime);
                BuildSqlParameter(sqlCmd, "@DeliveryRestrictionEndTime", SqlDbType.Time, endTime);
                BuildSqlParameter(sqlCmd, "@DeliveryRestrictionOnlyLimitAmount", SqlDbType.Bit, onlyLimitAmount);
                BuildSqlParameter(sqlCmd, "@DeliveryRestrictionAmountLimit", SqlDbType.Float, amountLimit);
                BuildSqlParameter(sqlCmd, "@DeliveryRestrictionId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new DeliveryRestriction(Convert.ToInt32(sqlCmd.Parameters["@DeliveryRestrictionId"].Value),
                        deliveryAreaId, customerId, startTime, endTime, onlyLimitAmount, amountLimit);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Delete an entry from the DeliveryRestriction table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            DeliveryRestriction deliveryRestriction = Get(cn, id);
            if (deliveryRestriction != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM DeliveryRestriction WHERE DeliveryRestrictionId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the DeliveryRestriction table
        /// </summary>
        public static DeliveryRestriction Get(int id)
        {
            DeliveryRestriction result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);
            return result;
        }

        private static DeliveryRestriction Get(SqlConnection cn, int id)
        {
            DeliveryRestriction result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM DeliveryRestriction WHERE DeliveryRestrictionId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildDeliveryRestriction(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the DeliveryRestriction table
        /// </summary>
        public static IEnumerable<DeliveryRestriction> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM DeliveryRestriction", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildDeliveryRestriction(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the DeliveryRestriction table
        /// </summary>
        public static bool Update(DeliveryRestriction deliveryRestriction)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, deliveryRestriction);
            FinishedWithConnection(cn);
            return result;
        }

        private static bool Update(SqlConnection cn, DeliveryRestriction deliveryRestriction)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE DeliveryRestriction SET DeliveryRestrictionDeliveryAreaId=@DeliveryRestrictionDeliveryAreaId,DeliveryRestrictionCustomerId=@DeliveryRestrictionCustomerId,DeliveryRestrictionStartTime=@DeliveryRestrictionStartTime,DeliveryRestrictionEndTime=@DeliveryRestrictionEndTime,DeliveryRestrictionOnlyLimitAmount=@DeliveryRestrictionOnlyLimitAmount,DeliveryRestrictionAmountLimit=@DeliveryRestrictionAmountLimit WHERE DeliveryRestrictionId=@DeliveryRestrictionId";

                BuildSqlParameter(sqlCmd, "@DeliveryRestrictionId", SqlDbType.Int, deliveryRestriction.Id);
                BuildSqlParameter(sqlCmd, "@DeliveryRestrictionDeliveryAreaId", SqlDbType.Int, deliveryRestriction.DeliveryAreaId);
                BuildSqlParameter(sqlCmd, "@DeliveryRestrictionCustomerId", SqlDbType.Int, deliveryRestriction.CustomerId);
                BuildSqlParameter(sqlCmd, "@DeliveryRestrictionStartTime", SqlDbType.Time, deliveryRestriction.StartTime);
                BuildSqlParameter(sqlCmd, "@DeliveryRestrictionEndTime", SqlDbType.Time, deliveryRestriction.EndTime);
                BuildSqlParameter(sqlCmd, "@DeliveryRestrictionOnlyLimitAmount", SqlDbType.Bit, deliveryRestriction.OnlyLimitAmount);
                BuildSqlParameter(sqlCmd, "@DeliveryRestrictionAmountLimit", SqlDbType.Float, deliveryRestriction.AmountLimit);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Build a DeliveryRestriction object from a SqlDataReader object
        /// </summary>
        private static DeliveryRestriction BuildDeliveryRestriction(SqlDataReader rdr)
        {
            return new DeliveryRestriction(
                GetId(rdr),
                GetDeliverAreaId(rdr),
                GetCustomerId(rdr),
                GetStartTime(rdr),
                GetEndTime(rdr),
                GetOnlyLimitAmount(rdr),
                GetAmountLimit(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int GetDeliverAreaId(SqlDataReader rdr)
        {
            return rdr.GetInt32(1);
        }

        private static int GetCustomerId(SqlDataReader rdr)
        {
            return rdr.GetInt32(2);
        }

        private static TimeSpan? GetStartTime(SqlDataReader rdr)
        {
            TimeSpan? startTime = null;
            if (!rdr.IsDBNull(3))
                startTime = rdr.GetTimeSpan(3);
            return startTime;
        }

        private static TimeSpan? GetEndTime(SqlDataReader rdr)
        {
            TimeSpan? endTime = null;
            if (!rdr.IsDBNull(4))
                endTime = rdr.GetTimeSpan(4);
            return endTime;
        }

        private static bool GetOnlyLimitAmount(SqlDataReader rdr)
        {
            return rdr.GetBoolean(5);
        }

        private static double GetAmountLimit(SqlDataReader rdr)
        {
            return rdr.GetDouble(6);
        }
        #endregion
    }
}
