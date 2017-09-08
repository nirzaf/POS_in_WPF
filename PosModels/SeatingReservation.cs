using System;
using System.Collections.Generic;
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
    public class SeatingReservation : DataModelBase
    {
        #region Licensed Access Only
        static SeatingReservation()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(SeatingReservation).Assembly.GetName().GetPublicKeyToken(),
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
        public int CustomerId
        {
            get;
            private set;
        }

        [ModeledData("Time")]
        public DateTime ReservationTime
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataType("SMALLINT")]
        public int PartySize
        {
            get;
            private set;
        }

        [ModeledData()]
        public int SeatingId
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

        private SeatingReservation(int id, int customerId, DateTime reservationTime,
            int partySize, int seatingId, int employeeId)
        {
            Id = id;
            CustomerId = customerId;
            ReservationTime = reservationTime;
            PartySize = partySize;
            SeatingId = seatingId;
            EmployeeId = employeeId;
        }

        #region static
        /// <summary>
        /// Add a new entry to the SeatingReservation table
        /// </summary>
        public static SeatingReservation Add(int customerId, DateTime reservationTime,
            int partySize, int seatingId, int employeeId)
        {
            SeatingReservation result = null;
            DateTime purchaseTime = DateTime.Now;

            partySize = MathHelper.Clamp(partySize, short.MinValue, short.MaxValue);

            SqlConnection cn = GetConnection();
            string cmd = "AddSeatingReservation";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@SeatingReservationCustomerId", SqlDbType.Int, customerId);
                BuildSqlParameter(sqlCmd, "@SeatingReservationTime", SqlDbType.DateTime, reservationTime);
                BuildSqlParameter(sqlCmd, "@SeatingReservationPartySize", SqlDbType.SmallInt, partySize);
                BuildSqlParameter(sqlCmd, "@SeatingReservationSeatingId", SqlDbType.Int, seatingId);
                BuildSqlParameter(sqlCmd, "@SeatingReservationEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(sqlCmd, "@SeatingReservationId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new SeatingReservation(Convert.ToInt32(sqlCmd.Parameters["@SeatingReservationId"].Value),
                        customerId, reservationTime, partySize, seatingId, employeeId);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Delete an entry from the SeatingReservation table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            SeatingReservation seatingReservation = Get(cn, id);
            if (seatingReservation != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM SeatingReservation WHERE SeatingReservationId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the SeatingReservation table
        /// </summary>
        public static SeatingReservation Get(int id)
        {
            SeatingReservation result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);

            return result;
        }

        private static SeatingReservation Get(SqlConnection cn, int id)
        {
            SeatingReservation result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM SeatingReservation WHERE SeatingReservationId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildSeatingReservation(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the SeatingReservation table
        /// </summary>
        public static IEnumerable<SeatingReservation> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM SeatingReservation", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildSeatingReservation(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the SeatingReservation table
        /// </summary>
        public static bool Update(SeatingReservation seatingReservation)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, seatingReservation);
            FinishedWithConnection(cn);

            return result;
        }

        private static bool Update(SqlConnection cn, SeatingReservation seatingReservation)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE SeatingReservation SET SeatingReservationCustomerId=@SeatingReservationCustomerId,SeatingReservationTime=@SeatingReservationTime,SeatingReservationPartySize=@SeatingReservationPartySize,SeatingReservationSeatingId=@SeatingReservationSeatingId,SeatingReservationEmployeeId=@SeatingReservationEmployeeId WHERE SeatingReservationId=@SeatingReservationId";

                BuildSqlParameter(sqlCmd, "@SeatingReservationId", SqlDbType.Int, seatingReservation.Id);
                BuildSqlParameter(sqlCmd, "@SeatingReservationCustomerId", SqlDbType.Int, seatingReservation.CustomerId);
                BuildSqlParameter(sqlCmd, "@SeatingReservationTime", SqlDbType.DateTime, seatingReservation.ReservationTime);
                BuildSqlParameter(sqlCmd, "@SeatingReservationPartySize", SqlDbType.SmallInt, seatingReservation.PartySize);
                BuildSqlParameter(sqlCmd, "@SeatingReservationSeatingId", SqlDbType.Int, seatingReservation.SeatingId);
                BuildSqlParameter(sqlCmd, "@SeatingReservationEmployeeId", SqlDbType.Int, seatingReservation.EmployeeId);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Build a SeatingReservation object from a SqlDataReader object
        /// </summary>
        private static SeatingReservation BuildSeatingReservation(SqlDataReader rdr)
        {
            return new SeatingReservation(
                GetId(rdr),
                GetCustomerId(rdr),
                GetReservationTime(rdr),
                GetPartySize(rdr),
                GetSeatingId(rdr),
                GetEmployeeId(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int GetCustomerId(SqlDataReader rdr)
        {
            return rdr.GetInt32(1);
        }

        private static DateTime GetReservationTime(SqlDataReader rdr)
        {
            return rdr.GetDateTime(2);
        }

        private static int GetPartySize(SqlDataReader rdr)
        {
            return rdr.GetInt16(3);
        }

        private static int GetSeatingId(SqlDataReader rdr)
        {
            return rdr.GetInt32(4);
        }

        private static int GetEmployeeId(SqlDataReader rdr)
        {
            return rdr.GetInt32(5);
        }
        #endregion

    }
}
