using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TemposUpdateServiceModels
{
    public class CrashIncident
    {
        public int Id
        {
            get;
            private set;
        }

        public int CustomerId
        {
            get;
            private set;
        }

        public DateTime When
        {
            get;
            private set;
        }

        public int TopLevelCrashReportId
        {
            get;
            private set;
        }

        private CrashIncident(int id, int customerId, DateTime when, int crashReportId)
        {
            Id = id;
            CustomerId = customerId;
            When = when;
            TopLevelCrashReportId = crashReportId;
        }

        public void SetTopLevelCrashReportId(int id)
        {
            TopLevelCrashReportId = id;
        }

        public bool Delete()
        {
            CrashReport.DeleteAll(Id);
            return Delete(Id);
        }

        public bool Update()
        {
            return Update(this);
        }

        public static CrashIncident Add(int customerId)
        {
            CrashIncident result = null;
            DateTime when = DateTime.Now;

            string query = "INSERT INTO CrashIncident (CrashIncidentCustomerId, CrashIncidentWhen, CrashIncidentInitialCrashReportId) " +
                "VALUES (@CrashIncidentCustomerId, @CrashIncidentWhen, 0);" +
                "SELECT CAST(scope_identity() AS int)";
            using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    DatabaseHelper.BuildSqlParameter(cmd, "@CrashIncidentCustomerId", SqlDbType.Int, customerId);
                    DatabaseHelper.BuildSqlParameter(cmd, "@CrashIncidentWhen", SqlDbType.DateTime, when);
                    conn.Open();
                    int id = (Int32)cmd.ExecuteScalar();
                    if (id > 0)
                    {
                        result = new CrashIncident(id, customerId, when, 0);
                    }
                }
                conn.Close();
            }

            return result;
        }

        /// <summary>
        /// Delete an entry from the CrashIncident table
        /// </summary>
        private static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            using (SqlConnection cn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                cn.Open();
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM CrashIncident WHERE CrashIncidentId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
                cn.Close();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Gets all entires in the CrashIncident table
        /// </summary>
        /// <returns></returns>
        public static CrashIncident[] GetAll()
        {
            List<CrashIncident> list = new List<CrashIncident>();

            using (SqlConnection cn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM CrashIncident", cn);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(BuildCrashIncident(rdr));
                }
                rdr.Close();
                cn.Close();
            }

            return list.ToArray();
        }

        /// <summary>
        /// Get an entry from the CrashIncident table
        /// </summary>
        public static CrashIncident Get(int id)
        {
            CrashIncident result = null;

            using (SqlConnection cn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                cn.Open();
                result = Get(cn, id);
                cn.Close();
            }

            return result;
        }

        private static CrashIncident Get(SqlConnection cn, int id)
        {
            CrashIncident result = null;
            SqlCommand cmd = new SqlCommand("SELECT * FROM CrashIncident WHERE CrashIncidentId=" + id, cn);
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                result = BuildCrashIncident(rdr);
            }
            rdr.Close();
            return result;
        }

        public static int GetHighestId()
        {
            int result = 0;
            using (SqlConnection cn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT CAST((SELECT TOP 1 CrashIncidentId FROM CrashIncident ORDER BY CrashIncidentId DESC) AS Int)", cn);
                result = (Int32)cmd.ExecuteScalar();
                cn.Close();
            }

            return result;
        }

        /// <summary>
        /// Update an entry in the CrashIncident table
        /// </summary>
        public static bool Update(CrashIncident entry)
        {
            bool result = false;

            using (SqlConnection cn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                cn.Open();
                result = Update(cn, entry);
                cn.Close();
            }

            return result;
        }

        private static bool Update(SqlConnection cn, CrashIncident entry)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand cmd = cn.CreateCommand())
            {
                cmd.CommandText = "UPDATE CrashIncident SET CrashIncidentCustomerId=@CrashIncidentCustomerId,CrashIncidentWhen=@CrashIncidentWhen,CrashIncidentInitialCrashReportId=@CrashIncidentInitialCrashReportId WHERE CrashIncidentId=@CrashIncidentId";

                DatabaseHelper.BuildSqlParameter(cmd, "@CrashIncidentId", SqlDbType.Int, entry.Id);
                DatabaseHelper.BuildSqlParameter(cmd, "@CrashIncidentCustomerId", SqlDbType.Int, entry.CustomerId);
                DatabaseHelper.BuildSqlParameter(cmd, "@CrashIncidentWhen", SqlDbType.DateTime, entry.When);
                DatabaseHelper.BuildSqlParameter(cmd, "@CrashIncidentInitialCrashReportId", SqlDbType.Int, entry.TopLevelCrashReportId);

                rowsAffected = cmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        private static CrashIncident BuildCrashIncident(SqlDataReader rdr)
        {
            int id = Convert.ToInt32(rdr[0].ToString());
            int customerId = Convert.ToInt32(rdr[1].ToString());
            DateTime when = Convert.ToDateTime(rdr[2].ToString());
            int crashReportId = Convert.ToInt32(rdr[3].ToString());
            return new CrashIncident(id, customerId, when, crashReportId);
        }

    }
}
