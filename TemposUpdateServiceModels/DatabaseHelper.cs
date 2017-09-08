using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TemposUpdateServiceModels
{
    public static class DatabaseHelper
    {
        public const string ConnectionString =
            @"Data Source=Dalaran\SQLEXPRESS;Initial Catalog=TemposOffice;User ID=TemposOffice;Password=tempos1234;";

        public static void BuildSqlParameter(SqlCommand sqlCmd, string name, SqlDbType sqlType, object value)
        {
           if (value == null)
                value = DBNull.Value;
            SqlParameter sqlParam = new SqlParameter(name, sqlType);
            sqlCmd.Parameters.Add(sqlParam);
            sqlCmd.Parameters[name].Value = value;
        }

        public static void BuildSqlParameter(SqlCommand sqlCmd, string name, SqlDbType sqlType, ParameterDirection direction)
        {
            SqlParameter sqlParam = new SqlParameter(name, sqlType);
            sqlCmd.Parameters.Add(sqlParam);
            sqlCmd.Parameters[name].Direction = direction;
        }

        public static bool SqlExecute(string sqlCommandText)
        {
            Int32 rowsAffected = 0;
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = sqlCommandText;
                    sqlCmd.CommandType = CommandType.Text;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
                cn.Close();
            }
            return (rowsAffected != 0);
        }
    }
}
