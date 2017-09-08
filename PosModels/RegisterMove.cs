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
    public class RegisterMove : DataModelBase
    {
        #region Licensed Access Only
        static RegisterMove()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(RegisterMove).Assembly.GetName().GetPublicKeyToken(),
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
        [ModeledDataType("TINYINT")]
        public int SourceRegisterId
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataType("TINYINT")]
        public int SourceRegisterSubId
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
        [ModeledDataType("TINYINT")]
        [ModeledDataNullable()]
        public int? TargetRegisterId
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataType("TINYINT")]
        [ModeledDataNullable()]
        public int? TargetRegisterSubId
        {
            get;
            private set;
        }

        [ModeledData()]
        public DateTime? EndTime
        {
            get;
            private set;
        }

        private RegisterMove(int id, int registerDrawerId, int sourceRegisterId,
            int sourceRegisterSubId, DateTime startTime, int? targetRegisterId,
            int? targetRegisterSubId, DateTime? endTime)
        {
            Id = id;
            RegisterDrawerId = registerDrawerId;
            SourceRegisterId = sourceRegisterId;
            SourceRegisterSubId = sourceRegisterSubId;
            StartTime = startTime;
            TargetRegisterId = targetRegisterId;
            TargetRegisterSubId = targetRegisterSubId;
            EndTime = endTime;
        }

        public void SetTarget(int registerId, int registerSubId)
        {
            TargetRegisterId = MathHelper.Clamp(registerId, 0, 255);
            TargetRegisterSubId = MathHelper.Clamp(registerSubId, 0, 255);
            EndTime = DateTime.Now;
            Update(this);
        }

        #region static
        /// <summary>
        /// Add a new entry to the RegisterMove table
        /// </summary>
        public static RegisterMove Add(int registerDrawerId, int sourceRegisterId,
            int sourceRegisterSubId)
        {
            RegisterMove result = null;
            DateTime startTime = DateTime.Now;

            sourceRegisterId = MathHelper.Clamp(sourceRegisterId, 0, 255);
            sourceRegisterSubId = MathHelper.Clamp(sourceRegisterSubId, 0, 255);

            SqlConnection cn = GetConnection();
            string cmd = "AddRegisterMove";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@RegisterMoveRegisterDrawerId", SqlDbType.Int, registerDrawerId);
                BuildSqlParameter(sqlCmd, "@RegisterMoveSourceRegisterId", SqlDbType.TinyInt, sourceRegisterId);
                BuildSqlParameter(sqlCmd, "@RegisterMoveSourceRegisterSubId", SqlDbType.TinyInt, sourceRegisterSubId);
                BuildSqlParameter(sqlCmd, "@RegisterMoveStartTime", SqlDbType.DateTime, startTime);
                BuildSqlParameter(sqlCmd, "@RegisterMoveId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new RegisterMove(Convert.ToInt32(sqlCmd.Parameters["@RegisterMoveId"].Value),
                        registerDrawerId, sourceRegisterId, sourceRegisterSubId,
                        startTime, null, null, null);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Get an entry from the RegisterMove table
        /// </summary>
        public static RegisterMove Get(int id)
        {
            RegisterMove result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);

            return result;
        }

        private static RegisterMove Get(SqlConnection cn, int id)
        {
            RegisterMove result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM RegisterMove WHERE RegisterMoveId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildRegisterMove(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the open (no target specified) RegisterMove for the spcified employee id
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public static RegisterMove GetOpen(int employeeId)
        {
            RegisterDrawer registerDrawer = RegisterDrawer.GetFloating(employeeId);
            if (registerDrawer == null)
                return null;
            RegisterMove result = null;
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM RegisterMove WHERE (RegisterMoveEndTime IS NULL) AND RegisterMoveRegisterDrawerId=" + registerDrawer.Id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildRegisterMove(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Get all the entries in the RegisterMove table
        /// </summary>
        public static IEnumerable<RegisterMove> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM RegisterMove", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildRegisterMove(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the RegisterMove table
        /// </summary>
        public static bool Update(RegisterMove registerMove)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, registerMove);
            FinishedWithConnection(cn);

            return result;
        }

        private static bool Update(SqlConnection cn, RegisterMove registerMove)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE RegisterMove SET RegisterMoveRegisterDrawerId=@RegisterMoveRegisterDrawerId,RegisterMoveSourceRegisterId=@RegisterMoveSourceRegisterId,RegisterMoveSourceRegisterSubId=@RegisterMoveSourceRegisterSubId,RegisterMoveStartTime=@RegisterMoveStartTime,RegisterMoveTargetRegisterId=@RegisterMoveTargetRegisterId,RegisterMoveTargetRegisterSubId=@RegisterMoveTargetRegisterSubId,RegisterMoveEndTime=@RegisterMoveEndTime WHERE RegisterMoveId=@RegisterMoveId";

                BuildSqlParameter(sqlCmd, "@RegisterMoveId", SqlDbType.Int, registerMove.Id);
                BuildSqlParameter(sqlCmd, "@RegisterMoveRegisterDrawerId", SqlDbType.Int, registerMove.RegisterDrawerId);
                BuildSqlParameter(sqlCmd, "@RegisterMoveSourceRegisterId", SqlDbType.TinyInt, registerMove.SourceRegisterId);
                BuildSqlParameter(sqlCmd, "@RegisterMoveSourceRegisterSubId", SqlDbType.TinyInt, registerMove.SourceRegisterSubId);
                BuildSqlParameter(sqlCmd, "@RegisterMoveStartTime", SqlDbType.DateTime, registerMove.StartTime);
                BuildSqlParameter(sqlCmd, "@RegisterMoveTargetRegisterId", SqlDbType.TinyInt, registerMove.TargetRegisterId);
                BuildSqlParameter(sqlCmd, "@RegisterMoveTargetRegisterSubId", SqlDbType.TinyInt, registerMove.TargetRegisterSubId);
                BuildSqlParameter(sqlCmd, "@RegisterMoveEndTime", SqlDbType.DateTime, registerMove.EndTime);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Build a RegisterMove object from a SqlDataReader object
        /// </summary>
        private static RegisterMove BuildRegisterMove(SqlDataReader rdr)
        {
            return new RegisterMove(
                GetId(rdr),
                GetRegisterDrawerId(rdr),
                GetSourceRegisterId(rdr),
                GetSourceRegisterSubId(rdr),
                GetStartTime(rdr),
                GetTargetRegisterId(rdr),
                GetTargetRegisterSubId(rdr),
                GetEndTime(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int GetRegisterDrawerId(SqlDataReader rdr)
        {
            return rdr.GetInt32(1);
        }

        private static int GetSourceRegisterId(SqlDataReader rdr)
        {
            return rdr.GetByte(2);
        }

        private static int GetSourceRegisterSubId(SqlDataReader rdr)
        {
            return rdr.GetByte(3);
        }

        private static DateTime GetStartTime(SqlDataReader rdr)
        {
            return rdr.GetDateTime(4);
        }

        private static int? GetTargetRegisterId(SqlDataReader rdr)
        {
            if (!rdr.IsDBNull(5))
                return rdr.GetByte(5);
            return null;
        }

        private static int? GetTargetRegisterSubId(SqlDataReader rdr)
        {
            if (!rdr.IsDBNull(6))
                return rdr.GetByte(6);
            return null;
        }

        private static DateTime? GetEndTime(SqlDataReader rdr)
        {
            if (!rdr.IsDBNull(7))
                return rdr.GetDateTime(7);
            return null;
        }
        #endregion

        /// <summary>
        /// Get all the entries in the RegisterMove table, in the specified
        /// DateTime range
        /// </summary>
        public static IEnumerable<RegisterMove> GetAll(DateTime startDate, DateTime endDate)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM RegisterMove WHERE (RegisterMoveStartTime >= @RegisterMoveSearchStartTime AND RegisterMoveStartTime <= @RegisterMoveSearchEndTime)", cn))
            {
                BuildSqlParameter(cmd, "@RegisterMoveSearchStartTime", SqlDbType.DateTime, startDate);
                BuildSqlParameter(cmd, "@RegisterMoveSearchEndTime", SqlDbType.DateTime, endDate);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildRegisterMove(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }
    }
}
