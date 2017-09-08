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
    public class DirectDeposit : DataModelBase
    {
        #region Licensed Access Only
        static DirectDeposit()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(DirectDeposit).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosModels.dll");
            }
#endif
        }
        #endregion

        [ModeledData()]
        public int EmployeeId
        {
            get;
            private set;
        }

        [ModeledData()]
        public string AccountNumber
        {
            get;
            private set;
        }

        [ModeledData()]
        public string RoutingNumber
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataType("TINYINT")]
        public BankAccountType AccountType
        {
            get;
            private set;
        }

        private DirectDeposit(int employeeId, string accountNumber, string routingNumber,
            BankAccountType accountType)
        {
            EmployeeId = employeeId;
            AccountNumber = accountNumber;
            RoutingNumber = routingNumber;
            AccountType = accountType;
        }

        public void SetAccountNumber(string accountNumber)
        {
            AccountNumber = accountNumber;
        }

        public void SetRoutingNumber(string routingNumber)
        {
            RoutingNumber = routingNumber;
        }

        public void SetAccountType(BankAccountType accountType)
        {
            AccountType = accountType;
        }

        public bool Update()
        {
            return DirectDeposit.Update(this);
        }

        #region static
        /// <summary>
        /// Add a new entry to the DirectDeposit table
        /// </summary>
        public static DirectDeposit Add(int employeeId, string accountNumber,
            string routingNumber, BankAccountType accountType)
        {
            DirectDeposit result = null;

            SqlConnection cn = GetConnection();
            string cmd = "AddDirectDeposit";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@DirectDepositEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(sqlCmd, "@DirectDepositAccountNumber", SqlDbType.Text, accountNumber);
                BuildSqlParameter(sqlCmd, "@DirectDepositRoutingNumber", SqlDbType.Text, routingNumber);
                BuildSqlParameter(sqlCmd, "@DirectDepositAccountType", SqlDbType.TinyInt, accountType);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new DirectDeposit(employeeId, accountNumber,
                        routingNumber, accountType);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Delete an entry from the DirectDeposit table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            DirectDeposit directDeposit = Get(cn, id);
            if (directDeposit != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM DirectDeposit WHERE DirectDepositEmployeeId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the DirectDeposit table
        /// </summary>
        public static DirectDeposit Get(int id)
        {
            DirectDeposit result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);
            return result;
        }

        private static DirectDeposit Get(SqlConnection cn, int id)
        {
            DirectDeposit result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM DirectDeposit WHERE DirectDepositEmployeeId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildDirectDeposit(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the DirectDeposit table
        /// </summary>
        public static IEnumerable<DirectDeposit> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM DirectDeposit", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildDirectDeposit(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the DirectDeposit table
        /// </summary>
        public static bool Update(DirectDeposit directDeposit)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, directDeposit);
            FinishedWithConnection(cn);
            return result;
        }

        private static bool Update(SqlConnection cn, DirectDeposit directDeposit)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE DirectDeposit SET DirectDepositAccountNumber=@DirectDepositAccountNumber,DirectDepositRoutingNumber=@DirectDepositRoutingNumber,DirectDepositAccountType=@DirectDepositAccountType WHERE DirectDepositEmployeeId=@DirectDepositEmployeeId";

                BuildSqlParameter(sqlCmd, "@DirectDepositEmployeeId", SqlDbType.Int, directDeposit.EmployeeId);
                BuildSqlParameter(sqlCmd, "@DirectDepositAccountNumber", SqlDbType.Text, directDeposit.AccountNumber);
                BuildSqlParameter(sqlCmd, "@DirectDepositRoutingNumber", SqlDbType.Text, directDeposit.RoutingNumber);
                BuildSqlParameter(sqlCmd, "@DirectDepositAccountType", SqlDbType.TinyInt, directDeposit.AccountType);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Build a DirectDeposit object from a SqlDataReader object
        /// </summary>
        private static DirectDeposit BuildDirectDeposit(SqlDataReader rdr)
        {
            return new DirectDeposit(
                GetEmployeeId(rdr),
                GetAccountNumber(rdr),
                GetRoutingNumber(rdr),
                GetAccountType(rdr));
        }

        private static int GetEmployeeId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static string GetAccountNumber(SqlDataReader rdr)
        {
            return rdr.GetString(1);
        }

        private static string GetRoutingNumber(SqlDataReader rdr)
        {
            return rdr.GetString(2);
        }

        private static BankAccountType GetAccountType(SqlDataReader rdr)
        {
            return (BankAccountType)rdr.GetByte(3);
        }
        #endregion

    }
}
