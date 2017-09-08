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
    public class ZipCode : DataModelBase
    {
        #region Licensed Access Only
        static ZipCode()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(ZipCode).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosModels.dll");
            }
#endif
        }
        #endregion

        [ModeledData()]
        public int PostalCode
        {
            get;
            private set;
        }

        [ModeledData("ZipCodeCityId")]
        public int CityId
        {
            get;
            private set;
        }

        private ZipCode(int postalCode, int cityId)
        {
            PostalCode = postalCode;
            CityId = cityId;
        }

        public void SetCityId(int cityId)
        {
            CityId = cityId;
        }

        public bool Update()
        {
            return ZipCode.Update(this);
        }

        #region static
        /// <summary>
        /// Add a new entry to the PostalCode table
        /// </summary>
        public static ZipCode Add(int postalCode, int cityId)
        {
            ZipCode result = null;

            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "INSERT INTO ZipCode (ZipCodePostalCode, ZipCodeZipCodeCityId) VALUES (@ZipCodePostalCode, @ZipCodeZipCodeCityId)";
                BuildSqlParameter(sqlCmd, "@ZipCodePostalCode", SqlDbType.Int, postalCode);
                BuildSqlParameter(sqlCmd, "@ZipCodeZipCodeCityId", SqlDbType.Int, cityId);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new ZipCode(postalCode, cityId);
                }
            }
            FinishedWithConnection(cn);

            return result;
        }

        /// <summary>
        /// Delete an entry from the PostalCode table
        /// </summary>
        public static bool Delete(int postalCode)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            ZipCode zipCode = Get(cn, postalCode);
            if (zipCode != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM ZipCode WHERE ZipCodePostalCode=" + postalCode;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the PostalCode table
        /// </summary>
        public static ZipCode Get(int postalCode)
        {
            ZipCode result = null;
            if (postalCode > 0)
            {
                SqlConnection cn = GetConnection();
                result = Get(cn, postalCode);
                FinishedWithConnection(cn);
            }
            return result;
        }

        private static ZipCode Get(SqlConnection cn, int postalCode)
        {
            ZipCode result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ZipCode WHERE ZipCodePostalCode=" + postalCode, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildZipCode(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the ZipCode table
        /// </summary>
        public static IEnumerable<ZipCode> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ZipCode", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildZipCode(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get all the entries in the ZipCode table
        /// </summary>
        public static IEnumerable<ZipCode> GetAll(int cityId)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ZipCode WHERE ZipCodeZipCodeCityId=" + cityId, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildZipCode(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the PostalCode table
        /// </summary>
        public static bool Update(ZipCode zipCode)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, zipCode);
            FinishedWithConnection(cn);

            return result;
        }

        private static bool Update(SqlConnection cn, ZipCode zipCode)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE ZipCode SET ZipCodeZipCodeCityId=@ZipCodeZipCodeCityId WHERE ZipCodePostalCode=@ZipCodePostalCode";

                BuildSqlParameter(sqlCmd, "@ZipCodePostalCode", SqlDbType.Int, zipCode.PostalCode);
                BuildSqlParameter(sqlCmd, "@ZipCodeZipCodeCityId", SqlDbType.Int, zipCode.CityId);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        private static ZipCode BuildZipCode(SqlDataReader rdr)
        {
            return new ZipCode(
                GetPostalId(rdr),
                GetCityId(rdr));
        }

        private static int GetPostalId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int GetCityId(SqlDataReader rdr)
        {
            return rdr.GetInt32(1);
        }
        #endregion

    }
}
