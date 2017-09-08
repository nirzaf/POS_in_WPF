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
    public class Printer : DataModelBase
    {
        #region Licensed Access Only
        static Printer()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(Printer).Assembly.GetName().GetPublicKeyToken(),
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

        [ModeledData("Name")]
        public string PrinterName
        {
            get;
            private set;
        }

        [ModeledData()]
        public string NetworkPath
        {
            get;
            private set;
        }

        private Printer(int id, string name, string networkPath)
        {
            Id = id;
            PrinterName = name;
            NetworkPath = networkPath;
        }

        public void SetName(string name)
        {
            PrinterName = name;
        }

        public void SetNetworkPath(string path)
        {
            NetworkPath = path;
        }

        public bool Update()
        {
            return Printer.Update(this);
        }

        #region static
        /// <summary>
        /// Add a new entry to the Printer table
        /// </summary>
        public static Printer Add(string printerName, string networkPath)
        {
            Printer result = null;

            SqlConnection cn = GetConnection();
            string cmd = "AddPrinter";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@PrinterName", SqlDbType.Text, printerName);
                BuildSqlParameter(sqlCmd, "@PrinterNetworkPath", SqlDbType.Text, networkPath);
                BuildSqlParameter(sqlCmd, "@PrinterId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new Printer(Convert.ToInt32(sqlCmd.Parameters["@PrinterId"].Value),
                        printerName, networkPath);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Delete an entry from the Printer table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            Printer printer = Get(cn, id);
            if (printer != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM Printer WHERE PrinterId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the Printer table
        /// </summary>
        public static Printer Get(int id)
        {
            Printer result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);

            return result;
        }

        private static Printer Get(SqlConnection cn, int id)
        {
            Printer result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Printer WHERE PrinterId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildPrinter(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the Printer table
        /// </summary>
        public static IEnumerable<Printer> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Printer", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildPrinter(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the Printer table
        /// </summary>
        public static bool Update(Printer printer)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, printer);
            FinishedWithConnection(cn);

            return result;
        }

        private static bool Update(SqlConnection cn, Printer printer)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE Printer SET PrinterName=@PrinterName,PrinterNetworkPath=@PrinterNetworkPath WHERE PrinterId=@PrinterId";

                BuildSqlParameter(sqlCmd, "@PrinterId", SqlDbType.Int, printer.Id);
                BuildSqlParameter(sqlCmd, "@PrinterName", SqlDbType.Text, printer.PrinterName);
                BuildSqlParameter(sqlCmd, "@PrinterNetworkPath", SqlDbType.Text, printer.NetworkPath);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Build a Printer object from a SqlDataReader object
        /// </summary>
        private static Printer BuildPrinter(SqlDataReader rdr)
        {
            return new Printer(
                GetId(rdr),
                GetName(rdr),
                GetNetworkPath(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static string GetName(SqlDataReader rdr)
        {
            return rdr.GetString(1);
        }

        private static string GetNetworkPath(SqlDataReader rdr)
        {
            return rdr.GetString(2);
        }
        #endregion

    }
}
