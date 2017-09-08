using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TemposUpdateServiceModels
{
    public class CrashReport
    {
        public int Id
        {
            get;
            private set;
        }

        public int CrashIncidentId
        {
            get;
            private set;
        }

        public string ExceptionName
        {
            get;
            private set;
        }

        public string ExceptionMessage
        {
            get;
            private set;
        }

        public string StackTrace
        {
            get;
            private set;
        }

        /// <summary>
        /// Each crash report is an exception, since exceptions can have inner-exceptions,
        /// One crash could create multiple crash reports, the InnerExceptionCrashReportId
        /// value points to the inner exception.
        /// </summary>
        public int? InnerExceptionCrashReportId
        {
            get;
            private set;
        }

        private CrashReport(int id, int crashIncidentId, string exceptionName,
            string exceptionMessage, string stackTrace, int? innerExceptionCrashReportId)
        {
            Id = id;
            CrashIncidentId = crashIncidentId;
            ExceptionName = exceptionName;
            ExceptionMessage = exceptionMessage;
            StackTrace = stackTrace;
            InnerExceptionCrashReportId = innerExceptionCrashReportId;
        }

        public void SetInnerExceptionCrashReportId(int id)
        {
            InnerExceptionCrashReportId = id;
        }

        public bool Update()
        {
            return Update(this);
        }

        public static CrashReport Add(int crashIncidentId, Exception ex)
        {
            if (ex.InnerException == null)
                return Add(crashIncidentId, ex.GetType().ToString(), ex.Message,
                    ex.StackTrace, null);

            // Handle InnerException
            CrashReport innerCrashReport = Add(crashIncidentId, ex.InnerException);
            CrashReport currentCrashReport = Add(crashIncidentId, ex.GetType().ToString(),
                ex.Message, ex.StackTrace, innerCrashReport.Id);
            return currentCrashReport;
        }

        public static CrashReport Add(int crashIncidentId, string exceptionName,
            string exceptionMessage, string stackTrace, int? innerExceptionCrashReportId)
        {
            CrashReport result = null;

            string query = "INSERT INTO CrashReport (CrashReportCrashIncidentId, CrashReportExceptionName, CrashReportExceptionMessage, CrashReportExceptionStackTrace, CrashReportInnerExceptionCrashReportId) " +
                "VALUES (@CrashReportCrashIncidentId, @CrashReportExceptionName, @CrashReportExceptionMessage, @CrashReportExceptionStackTrace, @CrashReportInnerExceptionCrashReportId);" +
                "SELECT CAST(scope_identity() AS int)";
            using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    DatabaseHelper.BuildSqlParameter(cmd, "@CrashReportCrashIncidentId", SqlDbType.Int, crashIncidentId);
                    DatabaseHelper.BuildSqlParameter(cmd, "@CrashReportExceptionName", SqlDbType.Text, exceptionName);
                    DatabaseHelper.BuildSqlParameter(cmd, "@CrashReportExceptionMessage", SqlDbType.Text, exceptionMessage);
                    DatabaseHelper.BuildSqlParameter(cmd, "@CrashReportExceptionStackTrace", SqlDbType.Text, stackTrace);
                    DatabaseHelper.BuildSqlParameter(cmd, "@CrashReportInnerExceptionCrashReportId", SqlDbType.Int, innerExceptionCrashReportId);
                    conn.Open();
                    int id = (Int32)cmd.ExecuteScalar();
                    if (id > 0)
                    {
                        result = new CrashReport(id, crashIncidentId, exceptionName,
                            exceptionMessage, stackTrace, innerExceptionCrashReportId);
                    }
                }
                conn.Close();
            }

            return result;
        }

        /// <summary>
        /// Delete entries from the CrashReport table with a matching CrashIncidentId
        /// </summary>
        public static bool DeleteAll(int crashIncidentId)
        {
            Int32 rowsAffected = 0;
            using (SqlConnection cn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                cn.Open();
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM CrashReport WHERE CrashReportCrashIncidentId=" + crashIncidentId;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
                cn.Close();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get all entries in the CrashReport table
        /// </summary>
        /// <returns></returns>
        public static CrashReport[] GetAll()
        {
            List<CrashReport> list = new List<CrashReport>();

            using (SqlConnection cn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM CrashReport", cn);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(BuildCrashReport(rdr));
                }
                rdr.Close();
                cn.Close();
            }

            return list.ToArray();
        }

        /// <summary>
        /// Get an entry from the CrashReport table
        /// </summary>
        public static CrashReport Get(int id)
        {
            CrashReport result = null;

            using (SqlConnection cn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                cn.Open();
                result = Get(cn, id);
                cn.Close();
            }

            return result;
        }

        private static CrashReport Get(SqlConnection cn, int id)
        {
            CrashReport result = null;
            SqlCommand cmd = new SqlCommand("SELECT * FROM CrashReport WHERE CrashReportId=" + id, cn);
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                result = BuildCrashReport(rdr);
            }
            rdr.Close();
            return result;
        }

        /// <summary>
        /// Update an entry in the CrashReport table
        /// </summary>
        public static bool Update(CrashReport entry)
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

        private static bool Update(SqlConnection cn, CrashReport entry)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand cmd = cn.CreateCommand())
            {
                cmd.CommandText = "UPDATE CrashReport SET CrashReportCrashIncidentId=@CrashReportCrashIncidentId,CrashReportExceptionName=@CrashReportExceptionName,CrashReportExceptionMessage=@CrashReportExceptionMessage,CrashReportExceptionStackTrace=@CrashReportExceptionStackTrace,CrashReportInnerExceptionCrashReportId=@CrashReportInnerExceptionCrashReportId WHERE CrashReportId=@CrashReportId";

                DatabaseHelper.BuildSqlParameter(cmd, "@CrashReportId", SqlDbType.Int, entry.Id);
                DatabaseHelper.BuildSqlParameter(cmd, "@CrashReportCrashIncidentId", SqlDbType.Int, entry.CrashIncidentId);
                DatabaseHelper.BuildSqlParameter(cmd, "@CrashReportExceptionName", SqlDbType.Text, entry.ExceptionName);
                DatabaseHelper.BuildSqlParameter(cmd, "@CrashReportExceptionMessage", SqlDbType.Text, entry.ExceptionMessage);
                DatabaseHelper.BuildSqlParameter(cmd, "@CrashReportExceptionStackTrace", SqlDbType.Text, entry.StackTrace);
                DatabaseHelper.BuildSqlParameter(cmd, "@CrashReportInnerExceptionCrashReportId", SqlDbType.Int, entry.InnerExceptionCrashReportId);

                rowsAffected = cmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        private static CrashReport BuildCrashReport(SqlDataReader rdr)
        {
            int id = Convert.ToInt32(rdr[0].ToString());
            int incidentId = Convert.ToInt32(rdr[1].ToString());
            string exceptionName = rdr[2].ToString();
            string exceptionMessage = rdr[3].ToString();
            string stackTrack = rdr[4].ToString();
            int? innerExceptionId = null;
            if (!rdr.IsDBNull(5))
                innerExceptionId = Convert.ToInt32(rdr[5].ToString());
            return new CrashReport(id, incidentId, exceptionName, exceptionMessage,
                stackTrack, innerExceptionId);
        }


    }
}
