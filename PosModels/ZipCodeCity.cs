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
    public class ZipCodeCity : DataModelBase
    {
        #region Licensed Access Only
        static ZipCodeCity()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(ZipCodeCity).Assembly.GetName().GetPublicKeyToken(),
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
        public string City
        {
            get;
            private set;
        }

        [ModeledData("ZipCodeStateId")]
        public int StateId
        {
            get;
            private set;
        }

        [ModeledData()]
        public double? Latitude
        {
            get;
            private set;
        }

        [ModeledData()]
        public double? Longitude
        {
            get;
            private set;
        }

        private ZipCodeCity(int id, string city, int stateId,
            double? latitude, double? longitude)
        {
            Id = id;
            Latitude = latitude;
            Longitude = longitude;
            City = city;
            StateId = stateId;
        }

        #region Static
        /// <summary>
        /// Add a new entry to the ZipCodeCity table
        /// </summary>
        public static ZipCodeCity Add(string city, int stateId, double? latitude, double? longitude)
        {
            ZipCodeCity result = null;
            DateTime purchaseTime = DateTime.Now;

            SqlConnection cn = GetConnection();
            string cmd = "AddZipCodeCity";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@ZipCodeCityName", SqlDbType.Text, city);
                BuildSqlParameter(sqlCmd, "@ZipCodeCityLatitude", SqlDbType.Float, latitude);
                BuildSqlParameter(sqlCmd, "@ZipCodeCityLongitude", SqlDbType.Float, longitude);
                BuildSqlParameter(sqlCmd, "@ZipCodeCityZipCodeStateId", SqlDbType.Int, stateId);
                BuildSqlParameter(sqlCmd, "@ZipCodeCityId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new ZipCodeCity(Convert.ToInt32(sqlCmd.Parameters["@ZipCodeCityId"].Value),
                        city, stateId, latitude, longitude);
                }
            }
            FinishedWithConnection(cn);

            return result;
        }

        /// <summary>
        /// Delete an entry from the ZipCodeCity table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            ZipCodeCity zipCodeCity = Get(cn, id);
            if (zipCodeCity != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM ZipCodeCity WHERE ZipCodeCityId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the ZipCodeCity table
        /// </summary>
        public static ZipCodeCity Get(int id)
        {
            ZipCodeCity result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);

            return result;
        }

        private static ZipCodeCity Get(SqlConnection cn, int id)
        {
            ZipCodeCity result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ZipCodeCity WHERE ZipCodeCityId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildZipCodeCity(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the ZipCodeCity table
        /// </summary>
        public static IEnumerable<ZipCodeCity> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ZipCodeCity", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildZipCodeCity(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get all entries from the ZipCodeCity table for a city
        /// </summary>
        public static ZipCodeCity GetByName(int zipCodeStateId, string name)
        {
            ZipCodeCity result = null;
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ZipCodeCity WHERE ZipCodeCityZipCodeStateId=@ZipCodeCityZipCodeStateId AND ZipCodeCityName LIKE @ZipCodeCityName", cn))
            {
                BuildSqlParameter(cmd, "@ZipCodeCityZipCodeStateId", SqlDbType.Int, zipCodeStateId);
                BuildSqlParameter(cmd, "@ZipCodeCityName", SqlDbType.Text, name);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildZipCodeCity(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        ///
        /// Build a ZipCodeCity object from a SqlDataReader object
        /// </summary>
        private static ZipCodeCity BuildZipCodeCity(SqlDataReader rdr)
        {
            return new ZipCodeCity(
                GetId(rdr),
                GetName(rdr),
                GetStateId(rdr),
                GetLatitude(rdr),
                GetLongitude(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static string GetName(SqlDataReader rdr)
        {
            return rdr.GetString(1);
        }

        private static double? GetLatitude(SqlDataReader rdr)
        {
            double? lat = null;
            if (!rdr.IsDBNull(2))
                lat = rdr.GetDouble(2);
            return lat;
        }

        private static double? GetLongitude(SqlDataReader rdr)
        {
            double? lon = null;
            if (!rdr.IsDBNull(3))
                lon = rdr.GetDouble(3);
            return lon;
        }

        private static int GetStateId(SqlDataReader rdr)
        {
            return rdr.GetInt32(4);
        }
        #endregion

    }
}
