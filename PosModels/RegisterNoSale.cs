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
    public class RegisterNoSale : DataModelBase
    {
        #region Licensed Access Only
        static RegisterNoSale()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(RegisterNoSale).Assembly.GetName().GetPublicKeyToken(),
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

        [ModeledData("Time")]
        public DateTime When
        {
            get;
            private set;
        }

        private RegisterNoSale(int id, int registerDrawerId, int employeeId,
            DateTime when)
        {
            Id = id;
            RegisterDrawerId = registerDrawerId;
            EmployeeId = employeeId;
            When = when;
        }

        #region static
        /// <summary>
        /// Add a new entry to the RegisterNoSale table
        /// </summary>
        public static RegisterNoSale Add(int registerDrawerId, int employeeId)
        {
            RegisterNoSale result = null;
            DateTime noSaleTime = DateTime.Now;

            SqlConnection cn = GetConnection();
            string cmd = "AddRegisterNoSale";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@RegisterNoSaleRegisterDrawerId", SqlDbType.Int, registerDrawerId);
                BuildSqlParameter(sqlCmd, "@RegisterNoSaleEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(sqlCmd, "@RegisterNoSaleTime", SqlDbType.DateTime, noSaleTime);
                BuildSqlParameter(sqlCmd, "@RegisterNoSaleId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new RegisterNoSale(Convert.ToInt32(sqlCmd.Parameters["@RegisterNoSaleId"].Value),
                        registerDrawerId, employeeId, noSaleTime);
                }
            }
            FinishedWithConnection(cn);

            return result;
        }

        private static void ResetAutoIdentity()
        {
            SqlConnection cn = GetConnection();
            string cmd = "DBCC CHECKIDENT (RegisterNoSale,RESEED,0)";
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
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 RegisterNoSaleId FROM RegisterNoSale", cn))
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
        /// Delete an entry from the RegisterNoSale table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            RegisterNoSale registerNoSale = Get(cn, id);
            if (registerNoSale != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM RegisterNoSale WHERE RegisterNoSaleId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the RegisterNoSale table
        /// </summary>
        public static RegisterNoSale Get(int id)
        {
            RegisterNoSale result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);

            return result;
        }

        private static RegisterNoSale Get(SqlConnection cn, int id)
        {
            RegisterNoSale result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM RegisterNoSale WHERE RegisterNoSaleId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildRegisterNoSale(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the RegisterNoSale table
        /// </summary>
        public static IEnumerable<RegisterNoSale> GetAll(int registerDrawerId)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM RegisterNoSale WHERE RegisterNoSaleRegisterDrawerId=" + registerDrawerId, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildRegisterNoSale(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static IEnumerable<RegisterNoSale> GetAll(DateTime startDate, DateTime endDate)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM RegisterNoSale WHERE (RegisterNoSaleTime >= @RegisterNoSaleSearchStartTime AND RegisterNoSaleTime <= @RegisterNoSaleSearchEndTime)", cn))
            {
                BuildSqlParameter(cmd, "@RegisterNoSaleSearchStartTime", SqlDbType.DateTime, startDate);
                BuildSqlParameter(cmd, "@RegisterNoSaleSearchEndTime", SqlDbType.DateTime, endDate);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildRegisterNoSale(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the RegisterNoSale table
        /// </summary>
        public static bool Update(RegisterNoSale registerNoSale)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, registerNoSale);
            FinishedWithConnection(cn);

            return result;
        }

        private static bool Update(SqlConnection cn, RegisterNoSale registerNoSale)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE RegisterNoSale SET RegisterNoSaleRegisterDrawerId=@RegisterNoSaleRegisterDrawerId,RegisterNoSaleEmployeeId=@RegisterNoSaleEmployeeId,RegisterNoSaleTime=@RegisterNoSaleTime WHERE RegisterNoSaleId=@RegisterNoSaleId";

                BuildSqlParameter(sqlCmd, "@RegisterNoSaleId", SqlDbType.Int, registerNoSale.Id);
                BuildSqlParameter(sqlCmd, "@RegisterNoSaleRegisterDrawerId", SqlDbType.Int, registerNoSale.RegisterDrawerId);
                BuildSqlParameter(sqlCmd, "@RegisterNoSaleEmployeeId", SqlDbType.Int, registerNoSale.EmployeeId);
                BuildSqlParameter(sqlCmd, "@RegisterNoSaleTime", SqlDbType.DateTime, registerNoSale.When);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        public static void Reset(Type type)
        {
            if (type != typeof(RegisterNoSale))
                throw new Exception("Bad use of reset");
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM RegisterNoSale WHERE RegisterNoSaleId>0";
                sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            ResetAutoIdentity();
        }

        /// <summary>
        /// Build a RegisterNoSale object from a SqlDataReader object
        /// </summary>
        private static RegisterNoSale BuildRegisterNoSale(SqlDataReader rdr)
        {
            return new RegisterNoSale(
                GetId(rdr),
                GetRegisterDrawerId(rdr),
                GetEmployeeId(rdr),
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

        private static DateTime GetWhen(SqlDataReader rdr)
        {
            return rdr.GetDateTime(3);
        }
        #endregion

    }
}
