using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using PosModels.Helpers;
using System.Data;
using PosModels.Managers;
using PosModels.Types;
using System.Reflection;

namespace PosModels
{
    [ModeledDataClass()]
    public class EmployeeJob : DataModelBase
    {
        #region Licensed Access Only
        static EmployeeJob()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(EmployeeJob).Assembly.GetName().GetPublicKeyToken(),
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
        public string Description
        {
            get;
            private set;
        }

        [ModeledData()]
        public bool HasTips
        {
            get;
            private set;
        }

        [ModeledData("AllowDispatch")]
        public bool AllowedDispatch
        {
            get;
            private set;
        }

        [ModeledData()]
        public bool IsEnabled
        {
            get;
            private set;
        }

        private EmployeeJob(int id, string description, bool hasTips, bool allowedDispatch,
            bool isEnabled)
        {
            Id = id;
            Description = description;
            HasTips = hasTips;
            AllowedDispatch = allowedDispatch;
            IsEnabled = isEnabled;
        }

        public void SetIsEnabled(bool isEnabled)
        {
            IsEnabled = isEnabled;
        }

        public void SetDescription(string description)
        {
            Description = description;
        }

        public void SetHasTips(bool hasTips)
        {
            HasTips = hasTips;
        }

        public void SetAllowedDispatch(bool allowed)
        {
            AllowedDispatch = allowed;
        }

        public bool Update()
        {
            return EmployeeJob.Update(this);
        }

        #region static
        /// <summary>
        /// Add a new entry to the EmployeeJob table
        /// </summary>
        public static EmployeeJob Add(string description, bool hasTips, bool allowedDispatch)
        {
            EmployeeJob result = null;

            SqlConnection cn = GetConnection();
            string cmd = "AddEmployeeJob";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@EmployeeJobDescription", SqlDbType.Text, description);
                BuildSqlParameter(sqlCmd, "@EmployeeJobHasTips", SqlDbType.Bit, hasTips);
                BuildSqlParameter(sqlCmd, "@EmployeeJobAllowDispatch", SqlDbType.Bit, allowedDispatch);
                BuildSqlParameter(sqlCmd, "@EmployeeJobId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new EmployeeJob(Convert.ToInt32(sqlCmd.Parameters["@EmployeeJobId"].Value),
                        description, hasTips, allowedDispatch, true);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Delete an entry from the EmployeeJob table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            EmployeeJob employeeJob = Get(cn, id);
            if (employeeJob != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM EmployeeJob WHERE EmployeeJobId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the EmployeeJob table
        /// </summary>
        public static EmployeeJob Get(int id)
        {
            EmployeeJob result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);
            return result;
        }

        private static EmployeeJob Get(SqlConnection cn, int id)
        {
            EmployeeJob result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM EmployeeJob WHERE EmployeeJobId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildEmployeeJob(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the EmployeeJob table
        /// </summary>
        public static IEnumerable<EmployeeJob> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM EmployeeJob", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildEmployeeJob(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static IEnumerable<int> GetDispatchableJobIds()
        {
            foreach (EmployeeJob job in EmployeeJob.GetAll())
            {
                if (job.AllowedDispatch)
                    yield return job.Id;
            }
        }

        /// <summary>
        /// Update an entry in the EmployeeJob table
        /// </summary>
        public static bool Update(EmployeeJob employeeJob)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, employeeJob);
            FinishedWithConnection(cn);
            return result;
        }

        private static bool Update(SqlConnection cn, EmployeeJob employeeJob)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE EmployeeJob SET EmployeeJobDescription=@EmployeeJobDescription,EmployeeJobHasTips=@EmployeeJobHasTips,EmployeeJobAllowDispatch=@EmployeeJobAllowDispatch,EmployeeJobIsEnabled=@EmployeeJobIsEnabled WHERE EmployeeJobId=@EmployeeJobId";

                BuildSqlParameter(sqlCmd, "@EmployeeJobId", SqlDbType.Int, employeeJob.Id);
                BuildSqlParameter(sqlCmd, "@EmployeeJobDescription", SqlDbType.Text, employeeJob.Description);
                BuildSqlParameter(sqlCmd, "@EmployeeJobHasTips", SqlDbType.Bit, employeeJob.HasTips);
                BuildSqlParameter(sqlCmd, "@EmployeeJobAllowDispatch", SqlDbType.Bit, employeeJob.AllowedDispatch);
                BuildSqlParameter(sqlCmd, "@EmployeeJobIsEnabled", SqlDbType.Bit, employeeJob.IsEnabled);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Build a EmployeeJob object from a SqlDataReader object
        /// </summary>
        private static EmployeeJob BuildEmployeeJob(SqlDataReader rdr)
        {
            return new EmployeeJob(
                GetId(rdr),
                GetDescription(rdr),
                GetHasTips(rdr),
                GetAllowedDispatch(rdr),
                GetIsEnabled(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static string GetDescription(SqlDataReader rdr)
        {
            return rdr.GetString(1);
        }

        private static bool GetHasTips(SqlDataReader rdr)
        {
            return rdr.GetBoolean(2);
        }

        private static bool GetAllowedDispatch(SqlDataReader rdr)
        {
            return rdr.GetBoolean(3);
        }

        private static bool GetIsEnabled(SqlDataReader rdr)
        {
            return rdr.GetBoolean(4);
        }
        #endregion

    }
}
