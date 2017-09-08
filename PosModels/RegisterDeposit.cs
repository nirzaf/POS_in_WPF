using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using PosModels.Types;

namespace PosModels
{
    [ModeledDataClass()]
    public class RegisterDeposit : DataModelBase
    {
        #region Licensed Access Only
        static RegisterDeposit()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(RegisterDeposit).Assembly.GetName().GetPublicKeyToken(),
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
        public int RegisterDrawerId
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

        [ModeledData()]
        public DateTime When
        {
            get;
            private set;
        }

        private RegisterDeposit(int id, int registerDrawerId, int employeeId,
            double amount, DateTime when)
        {
            Id = id;
            RegisterDrawerId = registerDrawerId;
            EmployeeId = employeeId;
            Amount = amount;
            When = when;
        }

        #region static
        /// <summary>
        /// Add a new entry to the RegisterDeposit table
        /// </summary>
        public static RegisterDeposit Add(int registerDrawerId, int employeeId, double amount)
        {
            RegisterDeposit result = null;
            DateTime when = DateTime.Now;

            SqlConnection cn = GetConnection();
            string cmd = "AddRegisterDeposit";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@RegisterDepositRegisterDrawerId", SqlDbType.Int, registerDrawerId);
                BuildSqlParameter(sqlCmd, "@RegisterDepositEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(sqlCmd, "@RegisterDepositAmount", SqlDbType.Float, amount);
                BuildSqlParameter(sqlCmd, "@RegisterDepositWhen", SqlDbType.DateTime, when);
                BuildSqlParameter(sqlCmd, "@RegisterDepositId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new RegisterDeposit(Convert.ToInt32(sqlCmd.Parameters["@RegisterDepositId"].Value),
                        registerDrawerId, employeeId, amount, when);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        private static void ResetAutoIdentity()
        {
            SqlConnection cn = GetConnection();
            string cmd = "DBCC CHECKIDENT (RegisterDeposit,RESEED,0)";
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
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 RegisterDepositId FROM RegisterDeposit", cn))
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
        /// Get an entry from the RegisterDeposit table
        /// </summary>
        public static RegisterDeposit Get(int id)
        {
            RegisterDeposit result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);

            return result;
        }

        private static RegisterDeposit Get(SqlConnection cn, int id)
        {
            RegisterDeposit result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM RegisterDeposit WHERE RegisterDepositId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildRegisterDeposit(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the RegisterDeposit table
        /// </summary>
        public static IEnumerable<RegisterDeposit> GetAll(int? registerDrawerId = null)
        {
            SqlConnection cn = GetConnection();
            SqlCommand cmd = null;
            try
            {
                if (registerDrawerId == null)
                    cmd = new SqlCommand("SELECT * FROM RegisterDeposit", cn);
                else
                    cmd = new SqlCommand("SELECT * FROM RegisterDeposit WHERE RegisterDepositRegisterDrawerId=" + registerDrawerId.Value, cn);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildRegisterDeposit(rdr);
                    }
                }
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
            }
        }

        /// <summary>
        /// Get all the entries in the RegisterDeposit table, in the specified
        /// DateTime range
        /// </summary>
        public static IEnumerable<RegisterDeposit> GetAll(DateTime startDate, DateTime endDate)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM RegisterDeposit WHERE (RegisterDepositWhen >= @RegisterDepositSearchStartTime AND RegisterDepositWhen <= @RegisterDepositSearchEndTime)", cn))
            {
                BuildSqlParameter(cmd, "@RegisterDepositSearchStartTime", SqlDbType.DateTime, startDate);
                BuildSqlParameter(cmd, "@RegisterDepositSearchEndTime", SqlDbType.DateTime, endDate);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildRegisterDeposit(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static void Reset(Type type)
        {
            if (type != typeof(RegisterDeposit))
                throw new Exception("Bad use of reset");
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM RegisterDeposit WHERE RegisterDepositId>0";
                sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            ResetAutoIdentity();
        }

        /// <summary>
        /// Build a RegisterDeposit object from a SqlDataReader object
        /// </summary>
        private static RegisterDeposit BuildRegisterDeposit(SqlDataReader rdr)
        {
            return new RegisterDeposit(
                GetId(rdr),
                GetRegisterDrawerId(rdr),
                GetEmployeeId(rdr),
                GetAmount(rdr),
                GetWhen(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int GetRegisterDrawerId(SqlDataReader rdr)
        {
            return rdr.GetInt32(1);
        }

        private static int GetEmployeeId(SqlDataReader rdr)
        {
            return rdr.GetInt32(2);
        }

        private static double GetAmount(SqlDataReader rdr)
        {
            return rdr.GetDouble(3);
        }

        private static DateTime GetWhen(SqlDataReader rdr)
        {
            return rdr.GetDateTime(4);
        }
        #endregion

    }
}
