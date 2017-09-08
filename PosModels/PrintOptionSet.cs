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
    public class PrintOptionSet : DataModelBase
    {
        #region Licensed Access Only
        static PrintOptionSet()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(PrintOptionSet).Assembly.GetName().GetPublicKeyToken(),
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
        public string OptionSetName
        {
            get;
            private set;
        }

        private PrintOptionSet(int id, string name)
        {
            Id = id;
            OptionSetName = name;
        }

        public void SetOptionName(string name)
        {
            OptionSetName = name;
        }

        public bool Update()
        {
            return PrintOptionSet.Update(this);
        }

        #region static
        /// <summary>
        /// Add a new entry to the PrintOptionSet table
        /// </summary>
        public static PrintOptionSet Add(string description, double cost)
        {
            PrintOptionSet result = null;
            DateTime purchaseTime = DateTime.Now;

            SqlConnection cn = GetConnection();
            string cmd = "AddPrintOptionSet";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@PrintOptionSetDescription", SqlDbType.Text, description);
                BuildSqlParameter(sqlCmd, "@PrintOptionSetId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new PrintOptionSet(Convert.ToInt32(sqlCmd.Parameters["@PrintOptionSetId"].Value),
                        description);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Delete an entry from the PrintOptionSet table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            PrintOptionSet printOptionSet = Get(cn, id);
            if (printOptionSet != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM PrintOptionSet WHERE PrintOptionSetId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the PrintOptionSet table
        /// </summary>
        public static PrintOptionSet Get(int id)
        {
            PrintOptionSet result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);

            return result;
        }

        private static PrintOptionSet Get(SqlConnection cn, int id)
        {
            PrintOptionSet result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM PrintOptionSet WHERE PrintOptionSetId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildPrintOptionSet(rdr);
                    }
                }
            }
            return result;
        }

        public static PrintOptionSet Get(string name)
        {
            IEnumerable<PrintOptionSet> optionSets = GetAll();
            foreach (PrintOptionSet set in optionSets)
            {
                if (set.OptionSetName.Equals(name))
                {
                    return set;
                }
            }
            return null;
        }

        /// <summary>
        /// Get all the entries in the PrintOptionSet table
        /// </summary>
        public static IEnumerable<PrintOptionSet> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM PrintOptionSet", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildPrintOptionSet(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the PrintOptionSet table
        /// </summary>
        public static bool Update(PrintOptionSet printOptionSet)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, printOptionSet);
            FinishedWithConnection(cn);

            return result;
        }

        private static bool Update(SqlConnection cn, PrintOptionSet printOptionSet)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE PrintOptionSet SET PrintOptionSetName=@PrintOptionSetName WHERE PrintOptionSetId=@PrintOptionSetId";

                BuildSqlParameter(sqlCmd, "@PrintOptionSetId", SqlDbType.Int, printOptionSet.Id);
                BuildSqlParameter(sqlCmd, "@PrintOptionSetDescription", SqlDbType.Text, printOptionSet.OptionSetName);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Build a PrintOptionSet object from a SqlDataReader object
        /// </summary>
        private static PrintOptionSet BuildPrintOptionSet(SqlDataReader rdr)
        {
            return new PrintOptionSet(
                GetId(rdr),
                GetName(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static string GetName(SqlDataReader rdr)
        {
            return rdr.GetString(1);
        }
        #endregion

    }
}
