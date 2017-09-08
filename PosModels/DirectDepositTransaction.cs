using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using PosModels.Helpers;
using PosModels.Managers;
using PosModels.Types;

namespace PosModels
{
    [ModeledDataClass()]
    public class DirectDepositTransaction : DataModelBase
    {
        #region Licensed Access Only
        static DirectDepositTransaction()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(DirectDepositTransaction).Assembly.GetName().GetPublicKeyToken(),
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
        public double Amount
        {
            get;
            private set;
        }

        [ModeledData("Date")]
        public DateTime TransactionDateTime
        {
            get;
            private set;
        }

        private DirectDepositTransaction(int id, int employeeId, double amount,
            DateTime transactionDateTime)
        {
            Id = id;
            EmployeeId = employeeId;
            Amount = amount;
            TransactionDateTime = transactionDateTime;
        }

        public void SetEmployeeId(int employeeId)
        {
            EmployeeId = employeeId;
        }

        public void SetAmount(double amount)
        {
            Amount = amount;
        }

        public void SetTransactionDateTime(DateTime dateTime)
        {
            TransactionDateTime = dateTime;
        }

        public bool Update()
        {
            return DirectDepositTransaction.Update(this);
        }

        #region static
        /// <summary>
        /// Add a new entry to the DirectDepositTransaction table
        /// </summary>
        public static DirectDepositTransaction Add(int employeeId, double amount)
        {
            DirectDepositTransaction result = null;
            DateTime transactionTime = DateTime.Now;

            SqlConnection cn = GetConnection();
            string cmd = "AddDirectDepositTransaction";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@DirectDepositTransactionEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(sqlCmd, "@DirectDepositTransactionAmount", SqlDbType.Float, amount);
                BuildSqlParameter(sqlCmd, "@DirectDepositTransactionDate", SqlDbType.DateTime, transactionTime);
                BuildSqlParameter(sqlCmd, "@DirectDepositTransactionId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new DirectDepositTransaction(Convert.ToInt32(sqlCmd.Parameters["@DirectDepositTransactionId"].Value),
                        employeeId, amount, transactionTime);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Delete an entry from the DirectDepositTransaction table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            DirectDepositTransaction directDepositTransaction = Get(cn, id);
            if (directDepositTransaction != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM DirectDepositTransaction WHERE DirectDepositTransactionId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the DirectDepositTransaction table
        /// </summary>
        public static DirectDepositTransaction Get(int id)
        {
            DirectDepositTransaction result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);
            return result;
        }

        private static DirectDepositTransaction Get(SqlConnection cn, int id)
        {
            DirectDepositTransaction result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM DirectDepositTransaction WHERE DirectDepositTransactionId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildDirectDepositTransaction(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the DirectDepositTransaction table
        /// </summary>
        public static IEnumerable<DirectDepositTransaction> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM DirectDepositTransaction", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildDirectDepositTransaction(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the DirectDepositTransaction table
        /// </summary>
        public static bool Update(DirectDepositTransaction directDepositTransaction)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, directDepositTransaction);
            FinishedWithConnection(cn);
            return result;
        }

        private static bool Update(SqlConnection cn, DirectDepositTransaction directDepositTransaction)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE DirectDepositTransaction SET DirectDepositTransactionEmployeeId=@DirectDepositTransactionEmployeeId,DirectDepositTransactionAmount=@DirectDepositTransactionAmount,DirectDepositTransactionDate=@DirectDepositTransactionDate WHERE DirectDepositTransactionId=@DirectDepositTransactionId";

                BuildSqlParameter(sqlCmd, "@DirectDepositTransactionId", SqlDbType.Int, directDepositTransaction.Id);
                BuildSqlParameter(sqlCmd, "@DirectDepositTransactionEmployeeId", SqlDbType.Int, directDepositTransaction.EmployeeId);
                BuildSqlParameter(sqlCmd, "@DirectDepositTransactionAmount", SqlDbType.Float, directDepositTransaction.Amount);
                BuildSqlParameter(sqlCmd, "@DirectDepositTransactionDate", SqlDbType.DateTime, directDepositTransaction.TransactionDateTime);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Build a DirectDepositTransaction object from a SqlDataReader object
        /// </summary>
        private static DirectDepositTransaction BuildDirectDepositTransaction(SqlDataReader rdr)
        {
            return new DirectDepositTransaction(
                GetId(rdr), 
                GetEmployeeId(rdr),
                GetAmount(rdr),
                GetTransactionDateTime(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int GetEmployeeId(SqlDataReader rdr)
        {
            return rdr.GetInt32(1);
        }

        private static double GetAmount(SqlDataReader rdr)
        {
            return rdr.GetDouble(2);
        }

        private static DateTime GetTransactionDateTime(SqlDataReader rdr)
        {
            return rdr.GetDateTime(3);
        }
        #endregion

    }
}
