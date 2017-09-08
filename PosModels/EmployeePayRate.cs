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
    public class EmployeePayRate : DataModelBase
    {
        #region Licensed Access Only
        static EmployeePayRate()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(EmployeePayRate).Assembly.GetName().GetPublicKeyToken(),
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
        public double Wage
        {
            get;
            private set;
        }

        [ModeledData()]
        public double OvertimeWage
        {
            get;
            private set;
        }

        private EmployeePayRate(int id, int employeeId, int jobId,
            double wage, double overtimeWage)
        {
            Id = id;
            EmployeeId = employeeId;
            JobId = jobId;
            Wage = wage;
            OvertimeWage = overtimeWage;
        }

        public void SetWage(double wage)
        {
            Wage = wage;
        }

        public bool Update()
        {
            return Update(this);
        }

        #region static
        public static EmployeePayRate Add(int employeeId, int jobId,
            double wage, double overtimeWage)
        {
            EmployeePayRate result = null;

            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = new SqlCommand("AddEmployeePayRate", cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@EmployeePayRateEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(sqlCmd, "@EmployeePayRateEmployeeJobId", SqlDbType.Int, jobId);
                BuildSqlParameter(sqlCmd, "@EmployeePayRateWage", SqlDbType.Float, wage);
                BuildSqlParameter(sqlCmd, "@EmployeePayRateOvertimeWage", SqlDbType.Float, overtimeWage);
                BuildSqlParameter(sqlCmd, "@EmployeePayRateId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new EmployeePayRate(Convert.ToInt32(sqlCmd.Parameters["@EmployeePayRateId"].Value),
                        employeeId, jobId, wage, overtimeWage);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        public static bool Delete(int employeeId, int jobId)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM EmployeePayRate WHERE EmployeePayRateEmployeeId=" +
                    employeeId + " AND EmployeePayRateEmployeeJobId=" + jobId;
                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the EmployeePayRate table
        /// </summary>
        public static EmployeePayRate Get(int id)
        {
            EmployeePayRate result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);
            return result;
        }

        private static EmployeePayRate Get(SqlConnection cn, int id)
        {
            EmployeePayRate result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM EmployeePayRate WHERE EmployeePayRateId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildEmployeePayRate(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the EmployeePayRate table
        /// </summary>
        public static IEnumerable<EmployeePayRate> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM EmployeePayRate", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildEmployeePayRate(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static EmployeePayRate GetEmployeePayRateForJob(int employeeId, int jobId)
        {
            EmployeePayRate result = null;
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand(
                "SELECT * FROM EmployeePayRate WHERE (EmployeePayRateEmployeeId="
                + employeeId + " AND EmployeePayRateEmployeeJobId=" + jobId + ")", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildEmployeePayRate(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Update an entry in the EmployeePayRate table
        /// </summary>
        public static bool Update(EmployeePayRate employeePayRate)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, employeePayRate);
            FinishedWithConnection(cn);
            return result;
        }

        private static bool Update(SqlConnection cn, EmployeePayRate employeePayRate)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE EmployeePayRate SET EmployeePayRateEmployeeId=@EmployeePayRateEmployeeId,EmployeePayRateEmployeeJobId=@EmployeePayRateEmployeeJobId,EmployeePayRateWage=@EmployeePayRateWage,EmployeePayRateOvertimeWage=@EmployeePayRateOvertimeWage WHERE EmployeePayRateId=@EmployeePayRateId";

                BuildSqlParameter(sqlCmd, "@EmployeePayRateId", SqlDbType.Int, employeePayRate.Id);
                BuildSqlParameter(sqlCmd, "@EmployeePayRateEmployeeId", SqlDbType.Int, employeePayRate.EmployeeId);
                BuildSqlParameter(sqlCmd, "@EmployeePayRateEmployeeJobId", SqlDbType.Int, employeePayRate.JobId);
                BuildSqlParameter(sqlCmd, "@EmployeePayRateWage", SqlDbType.Float, employeePayRate.Wage);
                BuildSqlParameter(sqlCmd, "@EmployeePayRateOvertimeWage", SqlDbType.Float, employeePayRate.OvertimeWage);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Build a EmployeePayRate object from a SqlDataReader object
        /// </summary>
        private static EmployeePayRate BuildEmployeePayRate(SqlDataReader rdr)
        {
            return new EmployeePayRate(
                GetId(rdr),
                GetEmployeeId(rdr),
                GetJobId(rdr),
                GetWage(rdr),
                GetOvertimeWage(rdr));
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

        private static double GetWage(SqlDataReader rdr)
        {
            return rdr.GetDouble(3);
        }

        private static double GetOvertimeWage(SqlDataReader rdr)
        {
            return rdr.GetDouble(4);
        }
        #endregion
    }
}
