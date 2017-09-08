using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using PosModels.Helpers;
using PosModels.Types;

namespace PosModels
{
    [ModeledDataClass()]
    public class RegisterDrawer : DataModelBase
    {
        #region Licensed Access Only
        static RegisterDrawer()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(RegisterDrawer).Assembly.GetName().GetPublicKeyToken(),
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
        [ModeledDataType("TINYINT")]
        [ModeledDataNullable()]
        public int? RegisterId
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataType("TINYINT")]
        [ModeledDataNullable()]
        public int? RegisterSubId
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
        public DateTime StartTime
        {
            get;
            private set;
        }

        [ModeledData()]
        public double StartAmount
        {
            get;
            private set;
        }

        [ModeledData()]
        public double CurrentAmount
        {
            get;
            private set;
        }

        [ModeledData("CloseTime")]
        public DateTime? EndTime
        {
            get;
            private set;
        }

        [ModeledData("CloseAmount")]
        public double? EndAmount
        {
            get;
            private set;
        }

        private RegisterDrawer(int id, int? registerId, int? registerSubId, int employeeId,
            DateTime startTime, double startAmount, double currentAmount, double? endAmount,
            DateTime? endTime)
        {
            Id = id;
            RegisterSubId = registerSubId;
            RegisterId = registerId;
            EmployeeId = employeeId;
            StartTime = startTime;
            StartAmount = startAmount;
            CurrentAmount = currentAmount;
            EndTime = endTime;
            EndAmount = endAmount;
        }

        public void Close()
        {
            EndTime = DateTime.Now;
            EndAmount = CurrentAmount;
            CurrentAmount = 0;
            Update(this);
        }

        public void UnsetRegisterId()
        {
            RegisterId = null;
            RegisterSubId = null;
            Update(this);
        }

        public void SetRegisterId(int registerId, int registerSubId = 0)
        {
            RegisterId = MathHelper.Clamp(registerId, 0, 255);
            RegisterSubId = MathHelper.Clamp(registerSubId, 0, 255);
            Update(this);
        }

        public void AddToCurrentAmount(double amount)
        {
            CurrentAmount += amount;
            Update(this);
        }

        public void RemoveFromCurrentAmount(double amount)
        {
            CurrentAmount -= amount;
            Update(this);
        }

        #region static
        /// <summary>
        /// Add a new entry to the RegisterDrawer table
        /// </summary>
        public static RegisterDrawer Add(int registerId, int registerSubId, int employeeId,
            double startAmount)
        {
            RegisterDrawer result = null;
            double? closeAmount = null;
            DateTime startTime = DateTime.Now;

            registerId = MathHelper.Clamp(registerId, 0, 255);
            registerSubId = MathHelper.Clamp(registerSubId, 0, 255);

            SqlConnection cn = GetConnection();
            string cmd = "AddRegisterDrawer";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@RegisterDrawerRegisterId", SqlDbType.TinyInt, registerId);
                BuildSqlParameter(sqlCmd, "@RegisterDrawerRegisterSubId", SqlDbType.TinyInt, registerSubId);
                BuildSqlParameter(sqlCmd, "@RegisterDrawerEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(sqlCmd, "@RegisterDrawerStartTime", SqlDbType.DateTime, startTime);
                BuildSqlParameter(sqlCmd, "@RegisterDrawerStartAmount", SqlDbType.Float, startAmount);
                BuildSqlParameter(sqlCmd, "@RegisterDrawerCurrentAmount", SqlDbType.Float, startAmount);
                BuildSqlParameter(sqlCmd, "@RegisterDrawerCloseAmount", SqlDbType.Float, closeAmount);
                BuildSqlParameter(sqlCmd, "@RegisterDrawerId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new RegisterDrawer(Convert.ToInt32(sqlCmd.Parameters["@RegisterDrawerId"].Value),
                        registerId, registerSubId, employeeId, startTime, startAmount, startAmount, closeAmount, null);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        private static void ResetAutoIdentity()
        {
            SqlConnection cn = GetConnection();
            string cmd = "DBCC CHECKIDENT (RegisterDrawer,RESEED,0)";
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
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 RegisterDrawerId FROM RegisterDrawer", cn))
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
        /// Delete an entry from the RegisterDrawer table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            RegisterDrawer registerDrawer = Get(cn, id);
            if (registerDrawer != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM RegisterDrawer WHERE RegisterDrawerId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the RegisterDrawer table
        /// </summary>
        public static RegisterDrawer Get(int id)
        {
            RegisterDrawer result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);

            return result;
        }

        private static RegisterDrawer Get(SqlConnection cn, int id)
        {
            RegisterDrawer result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM RegisterDrawer WHERE RegisterDrawerId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildRegisterDrawer(rdr);
                    }
                }
            }
            return result;
        }

        public static RegisterDrawer GetFloating(int employeeId)
        {
            RegisterDrawer result = null;
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM RegisterDrawer WHERE (RegisterDrawerRegisterId IS NULL) AND RegisterDrawerEmployeeId=" + employeeId, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildRegisterDrawer(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Get the opened RegisterDrawer for the specified registerId
        /// </summary>
        public static IEnumerable<RegisterDrawer> GetOpen(int registerId)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM RegisterDrawer " +
                    "WHERE RegisterDrawerRegisterId=" + registerId +
                    " AND (RegisterDrawerCloseTime IS NULL)", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildRegisterDrawer(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get all the entries in the RegisterDrawer table
        /// </summary>
        public static IEnumerable<RegisterDrawer> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM RegisterDrawer", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildRegisterDrawer(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the RegisterDrawer table
        /// </summary>
        public static bool Update(RegisterDrawer registerDrawer)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, registerDrawer);
            FinishedWithConnection(cn);

            return result;
        }

        private static bool Update(SqlConnection cn, RegisterDrawer registerDrawer)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE RegisterDrawer SET RegisterDrawerRegisterId=@RegisterDrawerRegisterId,RegisterDrawerRegisterSubId=@RegisterDrawerRegisterSubId,RegisterDrawerEmployeeId=@RegisterDrawerEmployeeId,RegisterDrawerStartTime=@RegisterDrawerStartTime,RegisterDrawerStartAmount=@RegisterDrawerStartAmount,RegisterDrawerCurrentAmount=@RegisterDrawerCurrentAmount,RegisterDrawerCloseTime=@RegisterDrawerCloseTime,RegisterDrawerCloseAmount=@RegisterDrawerCloseAmount WHERE RegisterDrawerId=@RegisterDrawerId";

                BuildSqlParameter(sqlCmd, "@RegisterDrawerId", SqlDbType.Int, registerDrawer.Id);
                BuildSqlParameter(sqlCmd, "@RegisterDrawerRegisterId", SqlDbType.TinyInt, registerDrawer.RegisterId);
                BuildSqlParameter(sqlCmd, "@RegisterDrawerRegisterSubId", SqlDbType.TinyInt, registerDrawer.RegisterSubId);
                BuildSqlParameter(sqlCmd, "@RegisterDrawerEmployeeId", SqlDbType.Int, registerDrawer.EmployeeId);
                BuildSqlParameter(sqlCmd, "@RegisterDrawerStartTime", SqlDbType.DateTime, registerDrawer.StartTime);
                BuildSqlParameter(sqlCmd, "@RegisterDrawerStartAmount", SqlDbType.Float, registerDrawer.StartAmount);
                BuildSqlParameter(sqlCmd, "@RegisterDrawerCurrentAmount", SqlDbType.Float, registerDrawer.CurrentAmount);
                BuildSqlParameter(sqlCmd, "@RegisterDrawerCloseTime", SqlDbType.DateTime, registerDrawer.EndTime);
                BuildSqlParameter(sqlCmd, "@RegisterDrawerCloseAmount", SqlDbType.Float, registerDrawer.EndAmount);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        public static void Reset(Type type)
        {
            if (type != typeof(RegisterDrawer))
                throw new Exception("Bad use of reset");
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM RegisterDrawer WHERE RegisterDrawerId>0";
                sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            ResetAutoIdentity();
        }

        /// <summary>
        /// Build a RegisterDrawer object from a SqlDataReader object
        /// </summary>
        private static RegisterDrawer BuildRegisterDrawer(SqlDataReader rdr)
        {
            return new RegisterDrawer(
                GetId(rdr),
                GetRegisterId(rdr),
                GetRegisterSubId(rdr),
                GetEmployeeId(rdr),
                GetStartTime(rdr),
                GetStartAmount(rdr),
                GetCurrentAmount(rdr),
                GetCloseAmount(rdr),
                GetEndTime(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int? GetRegisterId(SqlDataReader rdr)
        {
            int? registerId = null;
            if (!rdr.IsDBNull(1))
                registerId = rdr.GetByte(1);
            return registerId;
        }

        private static int? GetRegisterSubId(SqlDataReader rdr)
        {
            int? registerSubId = null;
            if (!rdr.IsDBNull(2))
                registerSubId = rdr.GetByte(2);
            return registerSubId;
        }

        private static int GetEmployeeId(SqlDataReader rdr)
        {
            return rdr.GetInt32(3);
        }

        private static DateTime GetStartTime(SqlDataReader rdr)
        {
            return rdr.GetDateTime(4);
        }

        private static double GetStartAmount(SqlDataReader rdr)
        {
            return rdr.GetDouble(5);
        }

        private static double GetCurrentAmount(SqlDataReader rdr)
        {
            return rdr.GetDouble(6);
        }

        private static double? GetCloseAmount(SqlDataReader rdr)
        {
            double? closeAmount = null;
            if (!rdr.IsDBNull(7))
                closeAmount = rdr.GetDouble(7);
            return closeAmount;
        }

        private static DateTime? GetEndTime(SqlDataReader rdr)
        {
            if (!rdr.IsDBNull(8))
                return rdr.GetDateTime(8);
            return null;
        }

        #endregion
    }
}
