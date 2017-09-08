using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using PosModels.Types;

namespace PosModels
{
    [ModeledDataClass()]
    public class RegisterDrop : DataModelBase
    {
        #region Licensed Access Only
        static RegisterDrop()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(RegisterDrop).Assembly.GetName().GetPublicKeyToken(),
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

        [ModeledData("Time")]
        public DateTime When
        {
            get;
            private set;
        }

        private RegisterDrop(int id, int registerDrawerId, int employeeId,
            double amount, DateTime dropTime)
        {
            Id = id;
            RegisterDrawerId = registerDrawerId;
            EmployeeId = employeeId;
            Amount = amount;
            When = dropTime;
        }

        #region static
        /// <summary>
        /// Add a new entry to the RegisterDrop table
        /// </summary>
        public static RegisterDrop Add(int registerDrawerId, int employeeId, double amount, DateTime dropTime)
        {
            RegisterDrop result = null;
            DateTime purchaseTime = DateTime.Now;

            SqlConnection cn = GetConnection();
            string cmd = "AddRegisterDrop";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@RegisterDropRegisterDrawerId", SqlDbType.Int, registerDrawerId);
                BuildSqlParameter(sqlCmd, "@RegisterDropEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(sqlCmd, "@RegisterDropAmount", SqlDbType.Float, amount);
                BuildSqlParameter(sqlCmd, "@RegisterDropTime", SqlDbType.DateTime, dropTime);
                BuildSqlParameter(sqlCmd, "@RegisterDropId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new RegisterDrop(Convert.ToInt32(sqlCmd.Parameters["@RegisterDropId"].Value),
                        registerDrawerId, employeeId, amount, dropTime);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        private static void ResetAutoIdentity()
        {
            SqlConnection cn = GetConnection();
            string cmd = "DBCC CHECKIDENT (RegisterDrop,RESEED,0)";
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
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 RegisterDropId FROM RegisterDrop", cn))
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
        /// Delete an entry from the RegisterDrop table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            RegisterDrop registerDrop = Get(cn, id);
            if (registerDrop != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM RegisterDrop WHERE RegisterDropId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the RegisterDrop table
        /// </summary>
        public static RegisterDrop Get(int id)
        {
            RegisterDrop result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);

            return result;
        }

        private static RegisterDrop Get(SqlConnection cn, int id)
        {
            RegisterDrop result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM RegisterDrop WHERE RegisterDropId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildRegisterDrop(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the RegisterDrop table
        /// </summary>
        public static IEnumerable<RegisterDrop> GetAll(int registerDrawerId)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM RegisterDrop WHERE RegisterDropRegisterDrawerId=" + registerDrawerId, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildRegisterDrop(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static IEnumerable<RegisterDrop> GetAll(DateTime startDate, DateTime endDate)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM RegisterDrop WHERE (RegisterDropTime >= @RegisterDropSearchStartTime AND RegisterDropTime <= @RegisterDropSearchEndTime)", cn))
            {
                BuildSqlParameter(cmd, "@RegisterDropSearchStartTime", SqlDbType.DateTime, startDate);
                BuildSqlParameter(cmd, "@RegisterDropSearchEndTime", SqlDbType.DateTime, endDate);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildRegisterDrop(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the RegisterDrop table
        /// </summary>
        public static bool Update(RegisterDrop registerDrop)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, registerDrop);
            FinishedWithConnection(cn);

            return result;
        }

        private static bool Update(SqlConnection cn, RegisterDrop registerDrop)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE RegisterDrop SET RegisterDropRegisterDrawerId=@RegisterDropRegisterDrawerId,RegisterDropEmployeeId=@RegisterDropEmployeeId,RegisterDropAmount=@RegisterDropAmount,RegisterDropTime=@RegisterDropTime WHERE RegisterDropId=@RegisterDropId";

                BuildSqlParameter(sqlCmd, "@RegisterDropId", SqlDbType.Int, registerDrop.Id);
                BuildSqlParameter(sqlCmd, "@RegisterDropRegisterDrawerId", SqlDbType.Int, registerDrop.RegisterDrawerId);
                BuildSqlParameter(sqlCmd, "@RegisterDropEmployeeId", SqlDbType.Int, registerDrop.EmployeeId);
                BuildSqlParameter(sqlCmd, "@RegisterDropAmount", SqlDbType.Float, registerDrop.Amount);
                BuildSqlParameter(sqlCmd, "@RegisterDropTime", SqlDbType.DateTime, registerDrop.When);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        public static void Reset(Type type)
        {
            if (type != typeof(RegisterDrop))
                throw new Exception("Bad use of reset");
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM RegisterDrop WHERE RegisterDropId>0";
                sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            ResetAutoIdentity();
        }

        /// <summary>
        /// Build a RegisterDrop object from a SqlDataReader object
        /// </summary>
        private static RegisterDrop BuildRegisterDrop(SqlDataReader rdr)
        {
            return new RegisterDrop(
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
