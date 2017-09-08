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
    public class EmployeeCheck : DataModelBase
    {
        #region Licensed Access Only
        static EmployeeCheck()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(EmployeeCheck).Assembly.GetName().GetPublicKeyToken(),
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

        [ModeledData("GrossPay")]
        public double GrossAmount
        {
            get;
            private set;
        }

        [ModeledData()]
        public double FederalTax
        {
            get;
            private set;
        }

        [ModeledData()]
        public double StateTax
        {
            get;
            private set;
        }

        [ModeledData()]
        public double LocalTax
        {
            get;
            private set;
        }

        [ModeledData()]
        public double FicaTax
        {
            get;
            private set;
        }

        [ModeledData()]
        public double SocialSecurityTax
        {
            get;
            private set;
        }

        [ModeledData("NetPay")]
        public double NetAmount
        {
            get;
            private set;
        }

        [ModeledData()]
        public DateTime Issued
        {
            get;
            private set;
        }

        private EmployeeCheck(int id, int employeeId, double grossAmount,
            double federalTax, double stateTax, double localTax, double ficaTax,
            double socialSecurityTax, double netAmount, DateTime issued)
        {
            Id = id;
            EmployeeId = employeeId;
            GrossAmount = grossAmount;
            FederalTax = federalTax;
            StateTax = stateTax;
            LocalTax = localTax;
            FicaTax = ficaTax;
            SocialSecurityTax = socialSecurityTax;
            NetAmount = netAmount;
            Issued = issued;
        }

        #region Static
        /// <summary>
        /// Add a new entry to the EmployeeCheck table
        /// </summary>
        public static EmployeeCheck Add(int employeeId, double grossAmount,
            double federalTax, double stateTax, double localTax, double ficaTax,
            double socialSecurityTax, double netAmount)
        {
            EmployeeCheck result = null;
            DateTime issued = DateTime.Now;

            SqlConnection cn = GetConnection();
            string cmd = "AddEmployeeCheck";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@EmployeeCheckEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(sqlCmd, "@EmployeeCheckGrossPay", SqlDbType.Float, grossAmount);
                BuildSqlParameter(sqlCmd, "@EmployeeCheckFederalTax", SqlDbType.Float, federalTax);
                BuildSqlParameter(sqlCmd, "@EmployeeCheckStateTax", SqlDbType.Float, stateTax);
                BuildSqlParameter(sqlCmd, "@EmployeeCheckLocalTax", SqlDbType.Float, localTax);
                BuildSqlParameter(sqlCmd, "@EmployeeCheckFicaTax", SqlDbType.Float, ficaTax);
                BuildSqlParameter(sqlCmd, "@EmployeeCheckSocialSecurityTax", SqlDbType.Float, socialSecurityTax);
                BuildSqlParameter(sqlCmd, "@EmployeeCheckNetPay", SqlDbType.Float, netAmount);
                BuildSqlParameter(sqlCmd, "@EmployeeCheckIssued", SqlDbType.DateTime, issued);
                BuildSqlParameter(sqlCmd, "@EmployeeCheckId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new EmployeeCheck(Convert.ToInt32(sqlCmd.Parameters["@EmployeeCheckId"].Value),
                        employeeId, grossAmount, federalTax, stateTax, localTax,
                        ficaTax, socialSecurityTax, netAmount, issued);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Delete an entry from the EmployeeCheck table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            EmployeeCheck employeeCheck = Get(cn, id);
            if (employeeCheck != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM EmployeeCheck WHERE EmployeeCheckId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the EmployeeCheck table
        /// </summary>
        public static EmployeeCheck Get(int id)
        {
            EmployeeCheck result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);
            return result;
        }

        private static EmployeeCheck Get(SqlConnection cn, int id)
        {
            EmployeeCheck result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM EmployeeCheck WHERE EmployeeCheckId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildEmployeeCheck(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the EmployeeCheck table
        /// </summary>
        public static IEnumerable<EmployeeCheck> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM EmployeeCheck", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildEmployeeCheck(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the EmployeeCheck table
        /// </summary>
        public static bool Update(EmployeeCheck employeeCheck)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, employeeCheck);
            FinishedWithConnection(cn);
            return result;
        }

        private static bool Update(SqlConnection cn, EmployeeCheck employeeCheck)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE EmployeeCheck SET EmployeeCheckEmployeeId=@EmployeeCheckEmployeeId,EmployeeCheckGrossPay=@EmployeeCheckGrossPay,EmployeeCheckFederalTax=@EmployeeCheckFederalTax,EmployeeCheckStateTax=@EmployeeCheckStateTax,EmployeeCheckLocalTax=@EmployeeCheckLocalTax,EmployeeCheckFicaTax=@EmployeeCheckFicaTax,EmployeeCheckSocialSecurityTax=@EmployeeCheckSocialSecurityTax,EmployeeCheckNetPay=@EmployeeCheckNetPay,EmployeeCheckIssued=@EmployeeCheckIssued WHERE EmployeeCheckId=@EmployeeCheckId";

                BuildSqlParameter(sqlCmd, "@EmployeeCheckId", SqlDbType.Int, employeeCheck.Id);
                BuildSqlParameter(sqlCmd, "@EmployeeCheckEmployeeId", SqlDbType.Int, employeeCheck.EmployeeId);
                BuildSqlParameter(sqlCmd, "@EmployeeCheckGrossPay", SqlDbType.Float, employeeCheck.GrossAmount);
                BuildSqlParameter(sqlCmd, "@EmployeeCheckFederalTax", SqlDbType.Float, employeeCheck.FederalTax);
                BuildSqlParameter(sqlCmd, "@EmployeeCheckStateTax", SqlDbType.Float, employeeCheck.StateTax);
                BuildSqlParameter(sqlCmd, "@EmployeeCheckLocalTax", SqlDbType.Float, employeeCheck.LocalTax);
                BuildSqlParameter(sqlCmd, "@EmployeeCheckFicaTax", SqlDbType.Float, employeeCheck.FicaTax);
                BuildSqlParameter(sqlCmd, "@EmployeeCheckSocialSecurityTax", SqlDbType.Float, employeeCheck.SocialSecurityTax);
                BuildSqlParameter(sqlCmd, "@EmployeeCheckNetPay", SqlDbType.Float, employeeCheck.NetAmount);
                BuildSqlParameter(sqlCmd, "@EmployeeCheckIssued", SqlDbType.DateTime, employeeCheck.Issued);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Build a EmployeeCheck object from a SqlDataReader object
        /// </summary>
        private static EmployeeCheck BuildEmployeeCheck(SqlDataReader rdr)
        {
            return new EmployeeCheck(
                GetId(rdr),
                GetEmployeeId(rdr),
                GetGrossAmount(rdr),
                GetFederalTax(rdr),
                GetStateTax(rdr),
                GetLocalTax(rdr),
                GetFicaTax(rdr),
                GetSocialSecurityTax(rdr),
                GetNetAmount(rdr),
                GetIssuedDate(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int GetEmployeeId(SqlDataReader rdr)
        {
            return rdr.GetInt32(1);
        }

        private static double GetGrossAmount(SqlDataReader rdr)
        {
            return rdr.GetDouble(2);
        }

        private static double GetFederalTax(SqlDataReader rdr)
        {
            return rdr.GetDouble(3);
        }

        private static double GetStateTax(SqlDataReader rdr)
        {
            return rdr.GetDouble(4);
        }

        private static double GetLocalTax(SqlDataReader rdr)
        {
            return rdr.GetDouble(5);
        }

        private static double GetFicaTax(SqlDataReader rdr)
        {
            return rdr.GetDouble(6);
        }

        private static double GetSocialSecurityTax(SqlDataReader rdr)
        {
            return rdr.GetDouble(7);
        }

        private static double GetNetAmount(SqlDataReader rdr)
        {
            return rdr.GetDouble(8);
        }

        private static DateTime GetIssuedDate(SqlDataReader rdr)
        {
            return rdr.GetDateTime(9);
        }
        #endregion

    }
}
