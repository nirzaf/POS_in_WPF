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
    public class Tax : DataModelBase
    {
        #region Licensed Access Only
        static Tax()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(Tax).Assembly.GetName().GetPublicKeyToken(),
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

        [ModeledData("Description")]
        public string TaxName
        {
            get;
            private set;
        }

        [ModeledData()]
        public double Percentage
        {
            get;
            private set;
        }

        private Tax(int id, string name, double percentage)
        {
            Id = id;
            TaxName = name;
            Percentage = percentage;
        }

        public void SetTaxName(string taxName)
        {
            TaxName = taxName;
        }

        public void SetPercentage(double percentage)
        {
            Percentage = percentage;
        }

        public bool Update()
        {
            return Tax.Update(this);
        }

        #region static
        /// <summary>
        /// Add a new entry to the tax table
        /// </summary>
        public static Tax Add(string name, double percentage)
        {
            Tax result = null;

            SqlConnection cn = GetConnection();
            string cmd = "AddTax";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@TaxDescription", SqlDbType.Text, name);
                BuildSqlParameter(sqlCmd, "@TaxPercentage", SqlDbType.Float, percentage);
                BuildSqlParameter(sqlCmd, "@TaxId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new Tax(Convert.ToInt32(sqlCmd.Parameters["@TaxId"].Value),
                        name, percentage);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Delete an entry from the Tax table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            Tax tax = Get(cn, id);
            if (tax != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM Tax WHERE TaxId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the Tax table
        /// </summary>
        public static Tax Get(int id)
        {
            Tax result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);

            return result;
        }

        private static Tax Get(SqlConnection cn, int id)
        {
            Tax result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Tax WHERE TaxId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildTax(rdr);
                    }
                }
            }
            return result;
        }

        public static Tax Get(string taxName)
        {
            foreach (Tax tax in GetAll())
            {
                if (tax.TaxName.Equals(taxName))
                    return tax;
            }
            return null;
        }

        /// <summary>
        /// Get all the entries in the Tax table
        /// </summary>
        public static IEnumerable<Tax> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Tax", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTax(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the Tax table
        /// </summary>
        public static bool Update(Tax tax)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, tax);
            FinishedWithConnection(cn);

            return result;
        }

        private static bool Update(SqlConnection cn, Tax tax)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE Tax SET TaxDescription=@TaxDescription,TaxPercentage=@TaxPercentage WHERE TaxId=@TaxId";

                BuildSqlParameter(sqlCmd, "@TaxId", SqlDbType.Int, tax.Id);
                BuildSqlParameter(sqlCmd, "@TaxDescription", SqlDbType.Text, tax.TaxName);
                BuildSqlParameter(sqlCmd, "@TaxPercentage", SqlDbType.Float, tax.Percentage);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        public static Tax FindTax(IEnumerable<Tax> taxes, int id)
        {
            foreach (Tax tax in taxes)
            {
                if (tax.Id == id)
                    return tax;
            }
            return null;
        }

        /// <summary>
        /// Build a Tax object from a SqlDataReader object
        /// </summary>
        private static Tax BuildTax(SqlDataReader rdr)
        {
            return new Tax(
                GetId(rdr),
                GetName(rdr),
                GetPercentage(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static string GetName(SqlDataReader rdr)
        {
            return rdr.GetString(1);
        }

        private static double GetPercentage(SqlDataReader rdr)
        {
            return rdr.GetDouble(2);
        }
        #endregion

    }
}
