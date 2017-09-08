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
    public class EmployeeTimesheet : DataModelBase
    {
        #region Licensed Access Only
        static EmployeeTimesheet()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(EmployeeTimesheet).Assembly.GetName().GetPublicKeyToken(),
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

        [ModeledData()]
        public DateTime? EndTime
        {
            get;
            private set;
        }

        [ModeledData("TipDeclare")]
        public double? DeclaredTipAmount
        {
            get;
            private set;
        }

        [ModeledData("DriverCompensation")]
        public double? DriverCompensationAmount
        {
            get;
            private set;
        }

        [ModeledData()]
        public bool IsDeleted
        {
            get;
            private set;
        }

        [ModeledData()]
        public bool IsLocked
        {
            get;
            private set;
        }

        private EmployeeTimesheet(int id, int employeeId, int jobId, DateTime startTime,
            DateTime? endTime, double? tipsDeclared, double? driverCompensationAmount,
            bool isDeleted, bool isLocked)
        {
            Id = id;
            EmployeeId = employeeId;
            JobId = jobId;
            StartTime = startTime;
            EndTime = endTime;
            DeclaredTipAmount = tipsDeclared;
            DriverCompensationAmount = driverCompensationAmount;
            IsDeleted = isDeleted;
            IsLocked = isLocked;
        }

        public void SetEmployeeId(int employeeId)
        {
            EmployeeId = employeeId;
        }

        public void SetJobId(int jobId)
        {
            JobId = jobId;
        }

        public void SetStartTime(DateTime startTime)
        {
            StartTime = startTime;
        }

        public void SetEndTime(DateTime? endTime)
        {
            EndTime = endTime;
        }

        public void SetDeclaredTipAmount(double? amount)
        {
            DeclaredTipAmount = amount;
        }

        public void SetDriverCompensationAmount(double? amount)
        {
            DriverCompensationAmount = amount;
        }

        public void SetIsDeleted(bool isDeleted)
        {
            IsDeleted = isDeleted;
        }

        public void SetIsLocked(bool isLocked)
        {
            IsLocked = isLocked;
        }

        public bool Update()
        {
            return EmployeeTimesheet.Update(this);
        }

        #region static
        /// <summary>
        /// Add a new entry to the EmployeeTimesheet table
        /// </summary>
        public static EmployeeTimesheet Add(int employeeId, int jobId)
        {
            EmployeeTimesheet result = null;
            DateTime startTime = DateTime.Now;

            SqlConnection cn = GetConnection();
            string cmd = "AddEmployeeTimesheet";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@EmployeeTimesheetEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(sqlCmd, "@EmployeeTimesheetEmployeeJobId", SqlDbType.SmallInt, jobId);
                BuildSqlParameter(sqlCmd, "@EmployeeTimesheetStartTime", SqlDbType.DateTime, startTime);
                BuildSqlParameter(sqlCmd, "@EmployeeTimesheetId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new EmployeeTimesheet(Convert.ToInt32(sqlCmd.Parameters["@EmployeeTimesheetId"].Value),
                        employeeId, jobId, startTime, null, null, null, false, false);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        private static void ResetAutoIdentity()
        {
            SqlConnection cn = GetConnection();
            string cmd = "DBCC CHECKIDENT (EmployeeTimesheet,RESEED,0)";
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
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 EmployeeTimesheetId FROM EmployeeTimesheet", cn))
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
        /// Delete an entry from the EmployeeTimesheet table
        /// </summary>
        private static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();            
            EmployeeTimesheet employeeTimesheet = Get(cn, id);
            if (employeeTimesheet != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM EmployeeTimesheet WHERE EmployeeTimesheetId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the EmployeeTimesheet table
        /// </summary>
        public static EmployeeTimesheet Get(int id)
        {
            EmployeeTimesheet result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);
            return result;
        }

        private static EmployeeTimesheet Get(SqlConnection cn, int id)
        {
            EmployeeTimesheet result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM EmployeeTimesheet WHERE EmployeeTimesheetId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildEmployeeTimesheet(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Finds the first clock-in with no clock-out for the specified employee
        /// </summary>
        public static EmployeeTimesheet GetOpenEntryForEmployee(int employeeId)
        {
            EmployeeTimesheet result = null;

            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM EmployeeTimesheet WHERE EmployeeTimesheetEmployeeId=" + employeeId +
                    " AND EmployeeTimesheetEndTime IS NULL", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildEmployeeTimesheet(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Get's all clocked-in entries
        /// </summary>
        public static IEnumerable<EmployeeTimesheet> GetAllOpen()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM EmployeeTimesheet WHERE EmployeeTimesheetEndTime IS NULL", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildEmployeeTimesheet(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get all the entries in the EmployeeTimesheet table
        /// </summary>
        public static IEnumerable<EmployeeTimesheet> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM EmployeeTimesheet", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildEmployeeTimesheet(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get all EmployeeTimesheet entries for the specified DateTime range
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static IEnumerable<EmployeeTimesheet> GetAll(DateTime startDate, DateTime endDate)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM EmployeeTimesheet WHERE (EmployeeTimesheetStartTime >= @EmployeeTimesheetSearchStartTime AND EmployeeTimesheetStartTime <= @EmployeeTimesheetSearchEndTime AND EmployeeTimesheetIsDeleted = 0)", cn))
            {
                BuildSqlParameter(cmd, "@EmployeeTimesheetSearchStartTime", SqlDbType.DateTime, startDate);
                BuildSqlParameter(cmd, "@EmployeeTimesheetSearchEndTime", SqlDbType.DateTime, endDate);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildEmployeeTimesheet(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Determines if the specified times will overlap an existing shift for the specified employee
        /// </summary>
        /// <param name="existingId">The EmployeeTimesheet.Id being checked</param>
        /// <param name="employeeId">The EmployeeTimesheet.EmployeeId being checked</param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static bool IsOverlapping(int existingId, int employeeId, DateTime startDate, DateTime endDate)
        {
            bool result = false;
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 * FROM EmployeeTimesheet WHERE " +
                "(EmployeeTimesheetEmployeeId = @EmployeeTimesheetEmployeeId) AND (EmployeeTimesheetIsDeleted = 0) AND " +
                "(EmployeeTimesheetId != @EmployeeTimesheetId) AND " +
                "((EmployeeTimesheetStartTime >= @EmployeeTimesheetSearchStartTime AND EmployeeTimesheetStartTime <= @EmployeeTimesheetSearchEndTime) OR " +
                "(EmployeeTimesheetEndTime >= @EmployeeTimesheetSearchStartTime AND EmployeeTimesheetEndTime <= @EmployeeTimesheetSearchEndTime))", cn))
            {
                BuildSqlParameter(cmd, "@EmployeeTimesheetId", SqlDbType.Int, existingId);
                BuildSqlParameter(cmd, "@EmployeeTimesheetEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(cmd, "@EmployeeTimesheetSearchStartTime", SqlDbType.DateTime, startDate);
                BuildSqlParameter(cmd, "@EmployeeTimesheetSearchEndTime", SqlDbType.DateTime, endDate);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                        result = true;
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Update an entry in the EmployeeTimesheet table
        /// </summary>
        public static bool Update(EmployeeTimesheet employeeTimesheet)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, employeeTimesheet);
            FinishedWithConnection(cn);
            return result;
        }

        private static bool Update(SqlConnection cn, EmployeeTimesheet employeeTimesheet)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE EmployeeTimesheet SET EmployeeTimesheetEmployeeId=@EmployeeTimesheetEmployeeId,EmployeeTimesheetEmployeeJobId=@EmployeeTimesheetEmployeeJobId,EmployeeTimesheetStartTime=@EmployeeTimesheetStartTime,EmployeeTimesheetEndTime=@EmployeeTimesheetEndTime,EmployeeTimesheetTipDeclare=@EmployeeTimesheetTipDeclare,EmployeeTimesheetDriverCompensation=@EmployeeTimesheetDriverCompensation,EmployeeTimesheetIsDeleted=@EmployeeTimesheetIsDeleted,EmployeeTimesheetIsLocked=@EmployeeTimesheetIsLocked WHERE EmployeeTimesheetId=@EmployeeTimesheetId";

                BuildSqlParameter(sqlCmd, "@EmployeeTimesheetId", SqlDbType.Int, employeeTimesheet.Id);
                BuildSqlParameter(sqlCmd, "@EmployeeTimesheetEmployeeId", SqlDbType.Int, employeeTimesheet.EmployeeId);
                BuildSqlParameter(sqlCmd, "@EmployeeTimesheetEmployeeJobId", SqlDbType.SmallInt, employeeTimesheet.JobId);
                BuildSqlParameter(sqlCmd, "@EmployeeTimesheetStartTime", SqlDbType.DateTime, employeeTimesheet.StartTime);
                BuildSqlParameter(sqlCmd, "@EmployeeTimesheetEndTime", SqlDbType.DateTime, employeeTimesheet.EndTime);
                BuildSqlParameter(sqlCmd, "@EmployeeTimesheetTipDeclare", SqlDbType.Float, employeeTimesheet.DeclaredTipAmount);
                BuildSqlParameter(sqlCmd, "@EmployeeTimesheetDriverCompensation", SqlDbType.Float, employeeTimesheet.DriverCompensationAmount);
                BuildSqlParameter(sqlCmd, "@EmployeeTimesheetIsDeleted", SqlDbType.Bit, employeeTimesheet.IsDeleted);
                BuildSqlParameter(sqlCmd, "@EmployeeTimesheetIsLocked", SqlDbType.Bit, employeeTimesheet.IsDeleted);
                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        public static void Reset(Type type)
        {
            if (type != typeof(EmployeeTimesheet))
                throw new Exception("Bad use of reset");
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM EmployeeTimesheet WHERE EmployeeTimesheetId>0";
                sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            ResetAutoIdentity();
        }

        /// <summary>
        /// Build a EmployeeTimesheet object from a SqlDataReader object
        /// </summary>
        private static EmployeeTimesheet BuildEmployeeTimesheet(SqlDataReader rdr)
        {
            return new EmployeeTimesheet(
                GetId(rdr),
                GetEmployeeId(rdr),
                GetJobId(rdr),
                GetStartTime(rdr),
                GetEndTime(rdr),
                GetTipsDeclared(rdr),
                GetDriverCompensation(rdr),
                GetIsDeleted(rdr),
                GetIsLocked(rdr));
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

        private static DateTime? GetEndTime(SqlDataReader rdr)
        {
            DateTime? endTime = null;
            if (!rdr.IsDBNull(4))
                endTime = rdr.GetDateTime(4);
            return endTime;
        }

        private static double? GetTipsDeclared(SqlDataReader rdr)
        {
            double? tipsDeclared = null;
            if (!rdr.IsDBNull(5))
                tipsDeclared = rdr.GetDouble(5);
            return tipsDeclared;
        }

        private static double? GetDriverCompensation(SqlDataReader rdr)
        {
            double? driverCompensation = null;
            if (!rdr.IsDBNull(6))
                driverCompensation = rdr.GetDouble(6);
            return driverCompensation;
        }

        private static bool GetIsDeleted(SqlDataReader rdr)
        {
            return rdr.GetBoolean(7);
        }

        private static bool GetIsLocked(SqlDataReader rdr)
        {
            return rdr.GetBoolean(8);
        }
        #endregion

    }
}
