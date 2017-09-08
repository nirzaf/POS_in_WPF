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
    public class EmployeeTimesheetChangeLog : DataModelBase
    {
        #region Licensed Access Only
        static EmployeeTimesheetChangeLog()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(EmployeeTimesheetChangeLog).Assembly.GetName().GetPublicKeyToken(),
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
        public int EmployeeTimesheetId
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
        public DateTime ChangeDate
        {
            get;
            private set;
        }

        [ModeledData()]
        public DateTime? OldStartTime
        {
            get;
            private set;
        }

        [ModeledData()]
        public DateTime? OldEndTime
        {
            get;
            private set;
        }

        [ModeledData()]
        public int? OldJob
        {
            get;
            private set;
        }

        [ModeledData()]
        public double? OldTips
        {
            get;
            private set;
        }

        [ModeledData("OldDriverComp")]
        public double? OldDriverCompensation
        {
            get;
            private set;
        }

        private EmployeeTimesheetChangeLog(int id, int employeeTimesheetId, int employeeId,
            DateTime changeDate, DateTime? oldStartTime, DateTime? oldEndTime,
            int? oldJob, double? oldTips, double? oldDriverCompensation)
        {
            Id = id;
            EmployeeTimesheetId = employeeTimesheetId;
            EmployeeId = employeeId;
            OldStartTime = oldStartTime;
            OldEndTime = oldEndTime;
            OldJob = oldJob;
            OldTips = oldTips;
            OldDriverCompensation = oldDriverCompensation;
        }

        #region Static
        /// <summary>
        /// Add a new entry to the EmployeeTimesheetChangeLog table
        /// </summary>
        public static EmployeeTimesheetChangeLog Add(int employeeTimesheetId, int employeeId,
            DateTime? oldStartTime, DateTime? oldEndTime, int? oldJob, double? oldTips,
            double? oldDriverCompensation)
        {
            EmployeeTimesheetChangeLog result = null;
            DateTime changeDate = DateTime.Now;

            SqlConnection cn = GetConnection();
            string cmd = "AddEmployeeTimesheetChangeLog";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@EmployeeTimesheetChangeLogEmployeeTimesheetId", SqlDbType.Int, employeeTimesheetId);
                BuildSqlParameter(sqlCmd, "@EmployeeTimesheetChangeLogEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(sqlCmd, "@EmployeeTimesheetChangeLogChangeDate", SqlDbType.DateTime, changeDate);
                BuildSqlParameter(sqlCmd, "@EmployeeTimesheetChangeLogOldStartTime", SqlDbType.DateTime, oldStartTime);
                BuildSqlParameter(sqlCmd, "@EmployeeTimesheetChangeLogOldEndTime", SqlDbType.DateTime, oldEndTime);
                BuildSqlParameter(sqlCmd, "@EmployeeTimesheetChangeLogOldJob", SqlDbType.SmallInt, oldJob);
                BuildSqlParameter(sqlCmd, "@EmployeeTimesheetChangeLogOldTips", SqlDbType.Float, oldTips);
                BuildSqlParameter(sqlCmd, "@EmployeeTimesheetChangeLogOldDriverComp", SqlDbType.Float, oldDriverCompensation);
                BuildSqlParameter(sqlCmd, "@EmployeeTimesheetChangeLogId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new EmployeeTimesheetChangeLog(Convert.ToInt32(sqlCmd.Parameters["@EmployeeTimesheetChangeLogId"].Value),
                        employeeTimesheetId, employeeId, changeDate, oldStartTime,
                        oldEndTime, oldJob, oldTips, oldDriverCompensation);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        private static void ResetAutoIdentity()
        {
            SqlConnection cn = GetConnection();
            string cmd = "DBCC CHECKIDENT (EmployeeTimesheetChangeLog,RESEED,0)";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
        }

        public static bool TableIsEmpty()
        {
            bool foundEntry = false;
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 EmployeeTimesheetChangeLogId FROM EmployeeTimesheetChangeLog", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        if (!rdr.IsDBNull(0))
                            foundEntry = true;
                    }
                }
            }
            FinishedWithConnection(cn);
            return !foundEntry;
        }

        /// <summary>
        /// Delete an entry from the EmployeeTimesheetChangeLog table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            EmployeeTimesheetChangeLog employeeTimesheetChangeLog = Get(cn, id);
            if (employeeTimesheetChangeLog != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM EmployeeTimesheetChangeLog WHERE EmployeeTimesheetChangeLogId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the EmployeeTimesheetChangeLog table
        /// </summary>
        public static EmployeeTimesheetChangeLog Get(int id)
        {
            EmployeeTimesheetChangeLog result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);
            return result;
        }

        private static EmployeeTimesheetChangeLog Get(SqlConnection cn, int id)
        {
            EmployeeTimesheetChangeLog result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM EmployeeTimesheetChangeLog WHERE EmployeeTimesheetChangeLogId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildEmployeeTimesheetChangeLog(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the EmployeeTimesheetChangeLog table
        /// </summary>
        public static IEnumerable<EmployeeTimesheetChangeLog> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM EmployeeTimesheetChangeLog", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildEmployeeTimesheetChangeLog(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get all the tickets for the specified DateTime range
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static IEnumerable<EmployeeTimesheetChangeLog> GetAll(DateTime startDate, DateTime endDate)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM EmployeeTimesheetChangeLog WHERE (EmployeeTimesheetChangeLogChangeDate >= @EmployeeTimesheetChangeLogSearchStartTime AND EmployeeTimesheetChangeLogChangeDate <= @EmployeeTimesheetChangeLogSearchEndTime)", cn))
            {
                BuildSqlParameter(cmd, "@EmployeeTimesheetChangeLogSearchStartTime", SqlDbType.DateTime, startDate);
                BuildSqlParameter(cmd, "@EmployeeTimesheetChangeLogSearchEndTime", SqlDbType.DateTime, endDate);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildEmployeeTimesheetChangeLog(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the EmployeeTimesheetChangeLog table
        /// </summary>
        public static bool Update(EmployeeTimesheetChangeLog employeeTimesheetChangeLog)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, employeeTimesheetChangeLog);
            FinishedWithConnection(cn);
            return result;
        }

        private static bool Update(SqlConnection cn, EmployeeTimesheetChangeLog employeeTimesheetChangeLog)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE EmployeeTimesheetChangeLog SET EmployeeTimesheetChangeLogEmployeeTimesheetId=@EmployeeTimesheetChangeLogEmployeeTimesheetId,EmployeeTimesheetChangeLogEmployeeId=@EmployeeTimesheetChangeLogEmployeeId,EmployeeTimesheetChangeLogEmployeeId=@EmployeeTimesheetChangeLogEmployeeId,EmployeeTimesheetChangeLogOldStartTime=@EmployeeTimesheetChangeLogOldStartTime,EmployeeTimesheetChangeLogOldEndTime=@EmployeeTimesheetChangeLogOldEndTime,EmployeeTimesheetChangeLogOldJob=@EmployeeTimesheetChangeLogOldJob,EmployeeTimesheetChangeLogOldTips=@EmployeeTimesheetChangeLogOldTips,EmployeeTimesheetChangeLogOldDriverComp=@EmployeeTimesheetChangeLogOldDriverComp WHERE EmployeeTimesheetChangeLogId=@EmployeeTimesheetChangeLogId";

                BuildSqlParameter(sqlCmd, "@EmployeeTimesheetChangeLogId", SqlDbType.Int, employeeTimesheetChangeLog.Id);
                BuildSqlParameter(sqlCmd, "@EmployeeTimesheetChangeLogEmployeeTimesheetId", SqlDbType.Int, employeeTimesheetChangeLog.EmployeeTimesheetId);
                BuildSqlParameter(sqlCmd, "@EmployeeTimesheetChangeLogEmployeeId", SqlDbType.Int, employeeTimesheetChangeLog.EmployeeId);
                BuildSqlParameter(sqlCmd, "@EmployeeTimesheetChangeLogChangeDate", SqlDbType.DateTime, employeeTimesheetChangeLog.ChangeDate);
                BuildSqlParameter(sqlCmd, "@EmployeeTimesheetChangeLogOldStartTime", SqlDbType.DateTime, employeeTimesheetChangeLog.OldStartTime);
                BuildSqlParameter(sqlCmd, "@EmployeeTimesheetChangeLogOldEndTime", SqlDbType.DateTime, employeeTimesheetChangeLog.OldEndTime);
                BuildSqlParameter(sqlCmd, "@EmployeeTimesheetChangeLogOldJob", SqlDbType.SmallInt, employeeTimesheetChangeLog.OldJob);
                BuildSqlParameter(sqlCmd, "@EmployeeTimesheetChangeLogOldTips", SqlDbType.Float, employeeTimesheetChangeLog.OldTips);
                BuildSqlParameter(sqlCmd, "@EmployeeTimesheetChangeLogOldDriverComp", SqlDbType.Float, employeeTimesheetChangeLog.OldDriverCompensation);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        public static void Reset(Type type)
        {
            if (type != typeof(EmployeeTimesheetChangeLog))
                throw new Exception("Bad use of reset");
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM EmployeeTimesheetChangeLog WHERE EmployeeTimesheetChangeLogId>0";
                sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            ResetAutoIdentity();
        }

        /// <summary>
        /// Build a EmployeeTimesheetChangeLog object from a SqlDataReader object
        /// </summary>
        private static EmployeeTimesheetChangeLog BuildEmployeeTimesheetChangeLog(SqlDataReader rdr)
        {
            return new EmployeeTimesheetChangeLog(
                GetId(rdr),
                GetEmployeeTimesheetId(rdr),
                GetEmployeeId(rdr),
                GetChangeDate(rdr),
                GetOldStart(rdr),
                GetOldEnd(rdr),
                GetOldJob(rdr),
                GetOldTips(rdr),
                GetOldDriverCompensation(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int GetEmployeeTimesheetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(1);
        }

        private static int GetEmployeeId(SqlDataReader rdr)
        {
            return rdr.GetInt32(2);
        }

        private static DateTime GetChangeDate(SqlDataReader rdr)
        {
            return rdr.GetDateTime(3);
        }

        private static DateTime? GetOldStart(SqlDataReader rdr)
        {
            DateTime? oldStart = null;
            if (!rdr.IsDBNull(4))
                oldStart = rdr.GetDateTime(4);
            return oldStart;
        }

        private static DateTime? GetOldEnd(SqlDataReader rdr)
        {
            DateTime? oldEnd = null;
            if (!rdr.IsDBNull(5))
                oldEnd = rdr.GetDateTime(5);
            return oldEnd;
        }

        private static int? GetOldJob(SqlDataReader rdr)
        {
            int? oldJob = null;
            if (!rdr.IsDBNull(6))
                oldJob = rdr.GetInt32(7);
            return oldJob;
        }

        private static double? GetOldTips(SqlDataReader rdr)
        {
            double? oldTips = null;
            if (!rdr.IsDBNull(7))
                oldTips = rdr.GetDouble(7);
            return oldTips;
        }

        private static double? GetOldDriverCompensation(SqlDataReader rdr)
        {
            double? oldDriverComp = null;
            if (!rdr.IsDBNull(8))
                oldDriverComp = rdr.GetDouble(8);
            return oldDriverComp;
        }
        #endregion

    }
}
