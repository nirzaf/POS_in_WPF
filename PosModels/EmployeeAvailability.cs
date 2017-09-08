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
    public class EmployeeAvailability : DataModelBase
    {
        #region Licensed Access Only
        static EmployeeAvailability()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(EmployeeAvailability).Assembly.GetName().GetPublicKeyToken(),
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

        [ModeledData("DayOfTheWeek")]
        [ModeledDataType("TINYINT")]
        public Days Day
        {
            get;
            private set;
        }

        [ModeledData()]
        public TimeSpan StartTime
        {
            get;
            private set;
        }

        [ModeledData()]
        public TimeSpan EndTime
        {
            get;
            private set;
        }

        private EmployeeAvailability(int id, int employeeId, Days day,
            TimeSpan startTime, TimeSpan endTime)
        {
            Id = id;
            EmployeeId = employeeId;
            Day = day;
            StartTime = startTime;
            EndTime = endTime;
        }

        public void SetDay(Days day)
        {
            Day = day;
        }

        public void SetStartTime(TimeSpan startTime)
        {
            StartTime = startTime;
        }

        public void SetEndTime(TimeSpan endTime)
        {
            EndTime = endTime;
        }

        /// <summary>
        /// Add a new entry to the EmployeeAvailability table
        /// </summary>
        public static EmployeeAvailability Add(int employeeId, Days day,
            TimeSpan startTime, TimeSpan endTime)
        {
            EmployeeAvailability result = null;
            DateTime purchaseTime = DateTime.Now;

            SqlConnection cn = GetConnection();
            string cmd = "AddEmployeeAvailability";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@EmployeeAvailabilityEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(sqlCmd, "@EmployeeAvailabilityDayOfTheWeek", SqlDbType.TinyInt, day);
                BuildSqlParameter(sqlCmd, "@EmployeeAvailabilityStartTime", SqlDbType.Time, startTime);
                BuildSqlParameter(sqlCmd, "@EmployeeAvailabilityEndTime", SqlDbType.Text, endTime);
                BuildSqlParameter(sqlCmd, "@EmployeeAvailabilityId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new EmployeeAvailability(Convert.ToInt32(sqlCmd.Parameters["@EmployeeAvailabilityId"].Value),
                        employeeId, day, startTime, endTime);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Get an entry from the EmployeeAvailability table
        /// </summary>
        public static EmployeeAvailability Get(int id)
        {
            EmployeeAvailability result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);
            return result;
        }

        private static EmployeeAvailability Get(SqlConnection cn, int id)
        {
            EmployeeAvailability result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM EmployeeAvailability WHERE EmployeeAvailabilityId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildEmployeeAvailability(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the EmployeeAvailability table
        /// </summary>
        public static IEnumerable<EmployeeAvailability> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM EmployeeAvailability", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildEmployeeAvailability(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the EmployeeAvailability table
        /// </summary>
        public static bool Update(EmployeeAvailability employeeAvailability)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, employeeAvailability);
            FinishedWithConnection(cn);
            return result;
        }

        private static bool Update(SqlConnection cn, EmployeeAvailability employeeAvailability)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE EmployeeAvailability SET EmployeeAvailabilityEmployeeId=@EmployeeAvailabilityEmployeeId,EmployeeAvailabilityDayOfTheWeek=@EmployeeAvailabilityDayOfTheWeek,EmployeeAvailabilityStartTime=@EmployeeAvailabilityStartTime,EmployeeAvailabilityEndTime=@EmployeeAvailabilityEndTime WHERE EmployeeAvailabilityId=@EmployeeAvailabilityId";

                BuildSqlParameter(sqlCmd, "@EmployeeAvailabilityId", SqlDbType.Int, employeeAvailability.Id);
                BuildSqlParameter(sqlCmd, "@EmployeeAvailabilityEmployeeId", SqlDbType.Int, employeeAvailability.EmployeeId);
                BuildSqlParameter(sqlCmd, "@EmployeeAvailabilityDayOfTheWeek", SqlDbType.TinyInt, employeeAvailability.Day);
                BuildSqlParameter(sqlCmd, "@EmployeeAvailabilityStartTime", SqlDbType.Time, employeeAvailability.StartTime);
                BuildSqlParameter(sqlCmd, "@EmployeeAvailabilityEndTime", SqlDbType.Text, employeeAvailability.EndTime);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Build a EmployeeAvailability object from a SqlDataReader object
        /// </summary>
        private static EmployeeAvailability BuildEmployeeAvailability(SqlDataReader rdr)
        {
            return new EmployeeAvailability(
                GetId(rdr),
                GetEmployeeId(rdr),
                GetDay(rdr),
                GetStartTime(rdr),
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

        private static Days GetDay(SqlDataReader rdr)
        {
            return (Days)rdr.GetByte(2);
        }

        private static TimeSpan GetStartTime(SqlDataReader rdr)
        {
            return rdr.GetTimeSpan(3);
        }

        private static TimeSpan GetEndTime(SqlDataReader rdr)
        {
            return rdr.GetTimeSpan(4);
        }
    }
}
