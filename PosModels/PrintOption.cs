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
    public class PrintOption : DataModelBase
    {
        #region Licensed Access Only
        static PrintOption()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(PrintOption).Assembly.GetName().GetPublicKeyToken(),
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
        public int PrintOptionSetId
        {
            get;
            private set;
        }

        [ModeledData()]
        public int PrinterId
        {
            get;
            private set;
        }

        private PrintOption(int id, int printOptionSetId, int printerId)
        {
            Id = id;
            PrintOptionSetId = printOptionSetId;
            PrinterId = printerId;
        }

        public void SetPrinterOptionSetId(int id)
        {
            PrintOptionSetId = id;
        }

        public void SetPrinterId(int id)
        {
            PrinterId = id;
        }

        public bool Update()
        {
            return PrintOption.Update(this);
        }

        #region static
        /// <summary>
        /// Add a new entry to the Printer table
        /// </summary>
        public static PrintOption Add(int printerOptionSetId, int printerId)
        {
            PrintOption result = null;
            DateTime purchaseTime = DateTime.Now;

            SqlConnection cn = GetConnection();
            string cmd = "AddPrintOption";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@PrintOptionPrintOptionSetId", SqlDbType.Int, printerOptionSetId);
                BuildSqlParameter(sqlCmd, "@PrintOptionPrinterId", SqlDbType.Int, purchaseTime);
                BuildSqlParameter(sqlCmd, "@PrintOptionId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new PrintOption(Convert.ToInt32(sqlCmd.Parameters["@PrintOptionId"].Value),
                        printerOptionSetId, printerId);
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
            PrintOption printOption = Get(cn, id);
            if (printOption != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM PrintOption WHERE PrintOptionId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the Printer table
        /// </summary>
        public static PrintOption Get(int id)
        {
            PrintOption result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);

            return result;
        }

        private static PrintOption Get(SqlConnection cn, int id)
        {
            PrintOption result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM PrintOption WHERE PrintOptionId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildPrintOption(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the Printer table
        /// </summary>
        public static IEnumerable<PrintOption> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM PrintOption", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildPrintOption(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get all the entries in the Printer table
        /// </summary>
        public static IEnumerable<PrintOption> GetAll(int printOptionSetId)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM PrintOption WHERE PrintOptionPrintOptionSetId=" + printOptionSetId, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildPrintOption(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get all print options that belong to a particular PrintOptionSet
        /// </summary>
        public static IEnumerable<PrintOption> GetInSet(int printOptionSetId)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM PrintOption WHERE PrintOptionPrintOptionSetId=" + printOptionSetId, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildPrintOption(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the Printer table
        /// </summary>
        public static bool Update(PrintOption printOption)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, printOption);
            FinishedWithConnection(cn);

            return result;
        }

        private static bool Update(SqlConnection cn, PrintOption printOption)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE PrintOption SET PrintOptionPrintOptionSetId=@PrintOptionPrintOptionSetId,PrintOptionPrinterId=@PrintOptionPrinterId WHERE PrintOptionId=@PrintOptionId";

                BuildSqlParameter(sqlCmd, "@PrintOptionId", SqlDbType.Int, printOption.Id);
                BuildSqlParameter(sqlCmd, "@PrintOptionPrintOptionSetId", SqlDbType.Int, printOption.PrintOptionSetId);
                BuildSqlParameter(sqlCmd, "@PrintOptionPrinterId", SqlDbType.Int, printOption.PrinterId);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Build a Printer object from a SqlDataReader object
        /// </summary>
        private static PrintOption BuildPrintOption(SqlDataReader rdr)
        {
            return new PrintOption(
                GetId(rdr),
                GetPrintOptionSetId(rdr),
                GetPrinterId(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int GetPrintOptionSetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(1);
        }

        private static int GetPrinterId(SqlDataReader rdr)
        {
            return rdr.GetInt32(2);
        }
        #endregion

    }
}
