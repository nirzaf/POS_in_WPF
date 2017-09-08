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
    public class RegisterPayout : DataModelBase
    {
        #region Licensed Access Only
        static RegisterPayout()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(RegisterPayout).Assembly.GetName().GetPublicKeyToken(),
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
        public string Reason
        {
            get;
            private set;
        }

        [ModeledData("Time")]
        public DateTime When
        {
            get;
            private set;
        }

        private RegisterPayout(int id, int registerDrawerId, int employeeId,
            double amount, string reason, DateTime payoutTime)
        {
            Id = id;
            RegisterDrawerId = registerDrawerId;
            EmployeeId = employeeId;
            Amount = amount;
            When = payoutTime;
            Reason = reason;
        }

        #region Static
        /// <summary>
        /// Add a new entry to the RegisterPayout table
        /// </summary>
        public static RegisterPayout Add(int registerDrawerId, int employeeId,
            double amount, string reason, DateTime payoutTime)
        {
            RegisterPayout result = null;
            DateTime purchaseTime = DateTime.Now;

            SqlConnection cn = GetConnection();
            string cmd = "AddRegisterPayout";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@RegisterPayoutRegisterDrawerId", SqlDbType.Int, registerDrawerId);
                BuildSqlParameter(sqlCmd, "@RegisterPayoutEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(sqlCmd, "@RegisterPayoutAmount", SqlDbType.Float, amount);
                BuildSqlParameter(sqlCmd, "@RegisterPayoutReason", SqlDbType.Text, reason);
                BuildSqlParameter(sqlCmd, "@RegisterPayoutTime", SqlDbType.DateTime, payoutTime);
                BuildSqlParameter(sqlCmd, "@RegisterPayoutId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new RegisterPayout(Convert.ToInt32(sqlCmd.Parameters["@RegisterPayoutId"].Value),
                        registerDrawerId, employeeId, amount, reason, payoutTime);
                }
            }
            FinishedWithConnection(cn);

            return result;
        }

        private static void ResetAutoIdentity()
        {
            SqlConnection cn = GetConnection();
            string cmd = "DBCC CHECKIDENT (RegisterPayout,RESEED,0)";
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
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 RegisterPayoutId FROM RegisterPayout", cn))
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
        /// Delete an entry from the RegisterPayout table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            RegisterPayout registerPayout = Get(cn, id);
            if (registerPayout != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM RegisterPayout WHERE RegisterPayoutId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the RegisterPayout table
        /// </summary>
        public static RegisterPayout Get(int id)
        {
            RegisterPayout result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);

            return result;
        }

        private static RegisterPayout Get(SqlConnection cn, int id)
        {
            RegisterPayout result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM RegisterPayout WHERE RegisterPayoutId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildRegisterPayout(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the RegisterPayout table
        /// </summary>
        public static IEnumerable<RegisterPayout> GetAll(int registerDrawerId)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM RegisterPayout WHERE RegisterPayoutRegisterDrawerId=" + registerDrawerId, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildRegisterPayout(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static IEnumerable<RegisterPayout> GetAll(DateTime startDate, DateTime endDate)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM RegisterPayout WHERE (RegisterPayoutTime >= @RegisterPayoutSearchStartTime AND RegisterPayoutTime <= @RegisterPayoutSearchEndTime)", cn))
            {
                BuildSqlParameter(cmd, "@RegisterPayoutSearchStartTime", SqlDbType.DateTime, startDate);
                BuildSqlParameter(cmd, "@RegisterPayoutSearchEndTime", SqlDbType.DateTime, endDate);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildRegisterPayout(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the RegisterPayout table
        /// </summary>
        public static bool Update(RegisterPayout registerPayout)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, registerPayout);
            FinishedWithConnection(cn);

            return result;
        }

        private static bool Update(SqlConnection cn, RegisterPayout registerPayout)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE RegisterPayout SET RegisterPayoutRegisterDrawerId=@RegisterPayoutRegisterDrawerId,RegisterPayoutEmployeeId=@RegisterPayoutEmployeeId,RegisterPayoutAmount=@RegisterPayoutAmount,RegisterPayoutReason=@RegisterPayoutReason,RegisterPayoutTime=@RegisterPayoutTime WHERE RegisterPayoutId=@RegisterPayoutId";

                BuildSqlParameter(sqlCmd, "@RegisterPayoutId", SqlDbType.Int, registerPayout.Id);
                BuildSqlParameter(sqlCmd, "@RegisterPayoutRegisterDrawerId", SqlDbType.Int, registerPayout.RegisterDrawerId);
                BuildSqlParameter(sqlCmd, "@RegisterPayoutEmployeeId", SqlDbType.Int, registerPayout.EmployeeId);
                BuildSqlParameter(sqlCmd, "@RegisterPayoutAmount", SqlDbType.Float, registerPayout.Amount);
                BuildSqlParameter(sqlCmd, "@RegisterPayoutReason", SqlDbType.Float, registerPayout.Reason);
                BuildSqlParameter(sqlCmd, "@RegisterPayoutTime", SqlDbType.DateTime, registerPayout.When);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        public static void Reset(Type type)
        {
            if (type != typeof(RegisterPayout))
                throw new Exception("Bad use of reset");
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM RegisterPayout WHERE RegisterPayoutId>0";
                sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            ResetAutoIdentity();
        }

        /// <summary>
        /// Build a RegisterPayout object from a SqlDataReader object
        /// </summary>
        private static RegisterPayout BuildRegisterPayout(SqlDataReader rdr)
        {
            return new RegisterPayout(
                GetId(rdr),
                GetRegisterId(rdr),
                GetEmployeeId(rdr),
                GetAmount(rdr),
                GetReason(rdr),
                GetWhen(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int GetRegisterId(SqlDataReader rdr)
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

        private static string GetReason(SqlDataReader rdr)
        {
            return rdr.GetString(4);
        }

        private static DateTime GetWhen(SqlDataReader rdr)
        {
            return rdr.GetDateTime(5);
        }
        #endregion

    }
}
