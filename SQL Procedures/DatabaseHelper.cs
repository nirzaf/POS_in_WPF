using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;

public static class DatabaseHelper
{
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
}
