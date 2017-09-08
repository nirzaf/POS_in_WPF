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
    public class Lock : DataModelBase
    {
        #region Licensed Access Only
        static Lock()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(Lock).Assembly.GetName().GetPublicKeyToken(),
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
        [ModeledDataType("SMALLINT")]
        public TableName Table
        {
            get;
            private set;
        }

        [ModeledData()]
        public int TableId
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
        [ModeledDataNullable()]
        public string TerminalName
        {
            get;
            private set;
        }

        private Lock(int id, TableName table, int tableId,
            int employeeId, string terminalName)
        {
            Id = id;
            Table = table;
            TableId = tableId;
            EmployeeId = employeeId;
            TerminalName = terminalName;
        }

        #region static
        /// <summary>
        /// Add a new entry to the Lock table
        /// </summary>
        public static Lock Add(TableName table, int tableId, int employeeId,
            string terminalName)
        {
            Lock result = null;

            string cmd = "INSERT INTO Lock (LockTable, LockTableId, LockEmployeeId, LockTerminalName) " +
                "VALUES (@LockTable, @LockTableId, @LockEmployeeId, @LockTerminalName);" +
                "SELECT CAST(scope_identity() AS int)";
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                BuildSqlParameter(sqlCmd, "@LockTable", SqlDbType.SmallInt, table);
                BuildSqlParameter(sqlCmd, "@LockTableId", SqlDbType.Int, tableId);
                BuildSqlParameter(sqlCmd, "@LockEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(sqlCmd, "@LockTerminalName", SqlDbType.Text, terminalName);
                BuildSqlParameter(sqlCmd, "@LockId", SqlDbType.Int, ParameterDirection.ReturnValue);
                int id = (Int32)sqlCmd.ExecuteScalar();
                result = new Lock(id, table, tableId, employeeId, terminalName);
            }
            FinishedWithConnection(cn);
            return result;
        }

        private static void ResetAutoIdentity()
        {
            SqlConnection cn = GetConnection();
            string cmd = "DBCC CHECKIDENT (Lock,RESEED,0)";
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
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 LockId FROM Lock", cn))
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
        /// Delete an entry from the Lock table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            Lock iLock = Get(cn, id);
            if (iLock != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM Lock WHERE LockId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Delete an entry from the Lock table
        /// </summary>
        public static bool Delete(TableName table, int tableId)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM Lock WHERE LockTable=" + (int)table + " AND LockTableId=" + tableId;
                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Delete all locks with the specified employee id
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public static bool DeleteAllEmployeeLocks(int employeeId)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM Lock WHERE LockEmployeeId=" + employeeId;
                result = (sqlCmd.ExecuteNonQuery() > 0);
            }
            FinishedWithConnection(cn);

            return result;
        }

        /// <summary>
        /// Get an entry from the Lock table
        /// </summary>
        public static Lock Get(int id)
        {
            Lock result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);
            return result;
        }

        private static Lock Get(SqlConnection cn, int id)
        {
            Lock result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Lock WHERE LockId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildLock(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get the Lock table entry, for a particular table type and tableId
        /// </summary>
        public static Lock Get(TableName table, int tableId)
        {
            Lock result = null;

            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Lock WHERE LockTable=" + (int)table + " AND LockTableId=" + tableId, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildLock(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);

            return result;
        }

        /// <summary>
        /// Get all the entries in the Lock table, for a particular table type
        /// </summary>
        public static IEnumerable<Lock> GetAll(TableName table)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Lock WHERE LockTable=" + (int)table, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildLock(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static void Reset(Type type)
        {
            if (type != typeof(Lock))
                throw new Exception("Bad use of reset");
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM Lock WHERE LockId>0";
                sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            ResetAutoIdentity();
        }

        /// <summary>
        /// Build a Lock object from a SqlDataReader object
        /// </summary>
        private static Lock BuildLock(SqlDataReader rdr)
        {
            return new Lock(
                GetId(rdr),
                GetTableName(rdr),
                GetTableId(rdr),
                GetEmployeeId(rdr),
                GetTerminalName(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return Convert.ToInt32(rdr[0].ToString());
        }

        private static TableName GetTableName(SqlDataReader rdr)
        {
            return rdr.GetInt16(1).GetTableName();
        }

        private static int GetTableId(SqlDataReader rdr)
        {
            return rdr.GetInt32(2);
        }

        private static int GetEmployeeId(SqlDataReader rdr)
        {
            return rdr.GetInt32(3);
        }

        private static string GetTerminalName(SqlDataReader rdr)
        {
            string terminalName = null;
            if (!rdr.IsDBNull(4))
            {
                string value = rdr.GetString(4);
                if (!value.Equals(""))
                    terminalName = value;
            }
            return terminalName;
        }
        #endregion

    }
}
