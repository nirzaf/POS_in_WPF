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
    public class EmployeeStatus : DataModelBase
    {
        #region Licensed Access Only
        static EmployeeStatus()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(EmployeeStatus).Assembly.GetName().GetPublicKeyToken(),
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
        public DateTime HireDate
        {
            get;
            private set;
        }

        [ModeledData()]
        public DateTime? TerminationDate
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataNullable()]
        public string TerminationReason
        {
            get;
            private set;
        }

        private EmployeeStatus(int id, int employeeId, DateTime hireDate,
            DateTime? terminationDate, string terminationReason)
        {
            Id = id;
            EmployeeId = employeeId;
            HireDate = hireDate;
            TerminationDate = terminationDate;
            TerminationReason = terminationReason;
        }

        public void Terminate(DateTime terminationDate, string terminationReason)
        {
            if (TerminationDate != null)
            {
                TerminationDate = terminationDate;
                TerminationReason = terminationReason;
                Update(this);
            }
        }

        /// <summary>
        /// Add a new entry to the EmployeeStatus table
        /// </summary>
        public static EmployeeStatus Add(int employeeId, DateTime hireDate)
        {
            EmployeeStatus result = null;
            DateTime purchaseTime = DateTime.Now;

            string query = "INSERT INTO EmployeeStatus (EmployeeStatusEmployeeId, EmployeeStatusHireDate) " +
                "VALUES (@EmployeeStatusEmployeeId, @EmployeeStatusHireDate);" +
                "SELECT CAST(scope_identity() AS int)";
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                BuildSqlParameter(cmd, "@EmployeeStatusEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(cmd, "@EmployeeStatusHireDate", SqlDbType.DateTime, hireDate);
                int id = (Int32)cmd.ExecuteScalar();
                if (id > 0)
                {
                    result = new EmployeeStatus(id, employeeId, hireDate, null, null);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Get an entry from the EmployeeStatus table
        /// </summary>
        public static EmployeeStatus Get(int id)
        {
            EmployeeStatus result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);
            return result;
        }

        private static EmployeeStatus Get(SqlConnection cn, int id)
        {
            EmployeeStatus result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM EmployeeStatus WHERE EmployeeStatusId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildEmployeeTermination(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the EmployeeStatus table
        /// </summary>
        public static IEnumerable<EmployeeStatus> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM EmployeeStatus", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildEmployeeTermination(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Returns the specified employee's unterminated EmployeeStatus entry
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static EmployeeStatus GetEmployeesActiveStatus(int employeeId)
        {
            EmployeeStatus result = null;
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM EmployeeStatus WHERE ((EmployeeStatusTerminationDate IS NOT NULL) AND EmployeeStatusEmployeeId=" + employeeId + ")", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildEmployeeTermination(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Update an entry in the EmployeeStatus table
        /// </summary>
        private static bool Update(EmployeeStatus employeeStatus)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, employeeStatus);
            FinishedWithConnection(cn);
            return result;
        }

        private static bool Update(SqlConnection cn, EmployeeStatus employeeStatus)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE EmployeeStatus SET EmployeeStatusEmployeeId=@EmployeeStatusEmployeeId,EmployeeStatusHireDate=@EmployeeStatusHireDate,EmployeeStatusTerminationDate=@EmployeeStatusTerminationDate,EmployeeStatusTerminationReason=@EmployeeStatusTerminationReason WHERE EmployeeStatusId=@EmployeeStatusId";

                BuildSqlParameter(sqlCmd, "@EmployeeStatusId", SqlDbType.Int, employeeStatus.Id);
                BuildSqlParameter(sqlCmd, "@EmployeeStatusEmployeeId", SqlDbType.Int, employeeStatus.EmployeeId);
                BuildSqlParameter(sqlCmd, "@EmployeeStatusHireDate", SqlDbType.DateTime, employeeStatus.HireDate);
                BuildSqlParameter(sqlCmd, "@EmployeeStatusTerminationDate", SqlDbType.DateTime, employeeStatus.TerminationDate);
                BuildSqlParameter(sqlCmd, "@EmployeeStatusTerminationReason", SqlDbType.Text, employeeStatus.TerminationReason);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }


        private static EmployeeStatus BuildEmployeeTermination(SqlDataReader rdr)
        {
            return new EmployeeStatus(
                GetId(rdr),
                GetEmployeeId(rdr),
                GetHireDate(rdr),
                GetTerminationDate(rdr),
                GetTerminationReason(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int GetEmployeeId(SqlDataReader rdr)
        {
            return rdr.GetInt32(1);
        }

        private static DateTime GetHireDate(SqlDataReader rdr)
        {
            return rdr.GetDateTime(2);
        }

        private static DateTime? GetTerminationDate(SqlDataReader rdr)
        {
            DateTime? result = null;
            if (!rdr.IsDBNull(3))
                result = rdr.GetDateTime(3);
            return result;
        }

        private static string GetTerminationReason(SqlDataReader rdr)
        {
            string result = null;
            if (!rdr.IsDBNull(4))
                result = rdr.GetString(4);
            return result;
        }

    }
}
