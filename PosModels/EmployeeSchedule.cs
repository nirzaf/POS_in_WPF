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
    public class EmployeeSchedule : DataModelBase
    {
        #region Licensed Access Only
        static EmployeeSchedule()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(EmployeeSchedule).Assembly.GetName().GetPublicKeyToken(),
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

        [ModeledData("EmployeeJobId")]
        public int JobId
        {
            get;
            private set;
        }

        [ModeledData()]
        public DateTime StartTime
        {
            get;
            private set;
        }

        [ModeledData("StopTime")]
        public DateTime EndTime
        {
            get;
            private set;
        }

        private EmployeeSchedule(int id, int employeeId, int jobId, DateTime startTime, DateTime endTime)
        {
            Id = id;
            EmployeeId = employeeId;
            JobId = jobId;
            StartTime = startTime;
            EndTime = endTime;
        }

        #region static
        /// <summary>
        /// Add a new entry to the EmployeeSchedule table
        /// </summary>
        public static EmployeeSchedule Add(int employeeId, int jobId, DateTime startTime, DateTime endTime)
        {
            EmployeeSchedule result = null;

            SqlConnection cn = GetConnection();
            string cmd = "AddEmployeeSchedule";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@EmployeeScheduleEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(sqlCmd, "@EmployeeScheduleEmployeeJobId", SqlDbType.SmallInt, jobId);
                BuildSqlParameter(sqlCmd, "@EmployeeScheduleStartTime", SqlDbType.DateTime, startTime);
                BuildSqlParameter(sqlCmd, "@EmployeeScheduleStopTime", SqlDbType.DateTime, endTime);
                BuildSqlParameter(sqlCmd, "@EmployeeScheduleId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new EmployeeSchedule(Convert.ToInt32(sqlCmd.Parameters["@EmployeeScheduleId"].Value),
                        employeeId, jobId, startTime, endTime);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Delete an entry from the EmployeeSchedule table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            EmployeeSchedule employeeSchedule = Get(cn, id);
            if (employeeSchedule != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM EmployeeSchedule WHERE EmployeeScheduleId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the EmployeeSchedule table
        /// </summary>
        public static EmployeeSchedule Get(int id)
        {
            EmployeeSchedule result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);
            return result;
        }

        private static EmployeeSchedule Get(SqlConnection cn, int id)
        {
            EmployeeSchedule result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM EmployeeSchedule WHERE EmployeeScheduleId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildEmployeeSchedule(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the EmployeeSchedule table
        /// </summary>
        public static IEnumerable<EmployeeSchedule> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM EmployeeSchedule", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildEmployeeSchedule(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the EmployeeSchedule table
        /// </summary>
        public static bool Update(EmployeeSchedule employeeSchedule)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, employeeSchedule);
            FinishedWithConnection(cn);
            return result;
        }

        private static bool Update(SqlConnection cn, EmployeeSchedule employeeSchedule)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE EmployeeSchedule SET EmployeeScheduleDescription=@EmployeeScheduleDescription,EmployeeSchedulePurchaseTime=@EmployeeSchedulePurchaseTime,EmployeeScheduleCost=@EmployeeScheduleCost,EmployeeScheduleComment=@EmployeeScheduleComment WHERE EmployeeScheduleId=@EmployeeScheduleId";

                BuildSqlParameter(sqlCmd, "@EmployeeScheduleId", SqlDbType.Int, employeeSchedule.Id);
                BuildSqlParameter(sqlCmd, "@EmployeeScheduleEmployeeId", SqlDbType.Int, employeeSchedule.EmployeeId);
                BuildSqlParameter(sqlCmd, "@EmployeeScheduleEmployeeJobId", SqlDbType.SmallInt, employeeSchedule.JobId);
                BuildSqlParameter(sqlCmd, "@EmployeeScheduleStartTime", SqlDbType.DateTime, employeeSchedule.StartTime);
                BuildSqlParameter(sqlCmd, "@EmployeeScheduleStopTime", SqlDbType.DateTime, employeeSchedule.EndTime);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Build a EmployeeSchedule object from a SqlDataReader object
        /// </summary>
        private static EmployeeSchedule BuildEmployeeSchedule(SqlDataReader rdr)
        {
            return new EmployeeSchedule(
                GetId(rdr),
                GetEmployeeId(rdr),
                GetJobId(rdr),
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

        private static int GetJobId(SqlDataReader rdr)
        {
            return rdr.GetInt32(2);
        }

        private static DateTime GetStartTime(SqlDataReader rdr)
        {
            return rdr.GetDateTime(3);
        }

        private static DateTime GetEndTime(SqlDataReader rdr)
        {
            return rdr.GetDateTime(4);
        }
        #endregion

    }
}
