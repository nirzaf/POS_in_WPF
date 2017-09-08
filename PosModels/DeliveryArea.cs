using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using PosModels.Helpers;
using PosModels.Managers;
using PosModels.Types;

namespace PosModels
{
    [ModeledDataClass()]
    public class DeliveryArea : DataModelBase
    {
        #region Licensed Access Only
        static DeliveryArea()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(DeliveryArea).Assembly.GetName().GetPublicKeyToken(),
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
        public string StreetName
        {
            get;
            private set;
        }

        [ModeledData("RangeEnd")]
        public int StartingAddress
        {
            get;
            private set;
        }

        [ModeledData("RangeStart")]
        public int EndingAddress
        {
            get;
            private set;
        }

        // For rural routes like "N120 Road Rd."
        // Stored in DB as nchar(10), max length is thus 10 characters
        [ModeledData("StreetAddressPrefix")]
        [ModeledDataType("NCHAR")]
        [ModeledDataNullable()]
        public string AddressPrefix
        {
            get;
            private set;
        }

        [ModeledData("Fee")]
        public double? DeliveryFee
        {
            get;
            private set;
        }

        private DeliveryArea(int id, string streetName, int startingAddress,
            int endingAddress, string addressPrefix, double? deliveryFee)
        {
            Id = id;
            StreetName = streetName;
            StartingAddress = startingAddress;
            EndingAddress = endingAddress;
            AddressPrefix = addressPrefix;
            DeliveryFee = deliveryFee;
        }

        public void SetAddressPrefix(string text)
        {
            if (text.Length > 10)
                text = text.Substring(0, 10);
            AddressPrefix = text;
        }

        public bool Update()
        {
            return DeliveryArea.Update(this);
        }

        #region static
        /// <summary>
        /// Add a new entry to the DeliveryArea table
        /// </summary>
        public static DeliveryArea Add(string streetName, int startingAddress,
            int endingAddress, string addressPrefix, double? deliveryFee)
        {
            DeliveryArea result = null;

            SqlConnection cn = GetConnection();
            string cmd = "AddDeliveryArea";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@DeliveryAreaStreetName", SqlDbType.Text, streetName);
                BuildSqlParameter(sqlCmd, "@DeliveryAreaRangeStart", SqlDbType.Int, startingAddress);
                BuildSqlParameter(sqlCmd, "@DeliveryAreaRangeEnd", SqlDbType.Int, endingAddress);
                BuildSqlParameter(sqlCmd, "@DeliveryAreaStreetAddressPrefix", SqlDbType.NChar, addressPrefix);
                BuildSqlParameter(sqlCmd, "@DeliveryAreaFee", SqlDbType.Float, deliveryFee);
                BuildSqlParameter(sqlCmd, "@DeliveryAreaId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new DeliveryArea(Convert.ToInt32(sqlCmd.Parameters["@DeliveryAreaId"].Value),
                        streetName, startingAddress, endingAddress, addressPrefix, deliveryFee);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Delete an entry from the DeliveryArea table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            DeliveryArea deliveryArea = Get(cn, id);
            if (deliveryArea != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM DeliveryArea WHERE DeliveryAreaId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the DeliveryArea table
        /// </summary>
        public static DeliveryArea Get(int id)
        {
            DeliveryArea result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);
            return result;
        }

        private static DeliveryArea Get(SqlConnection cn, int id)
        {
            DeliveryArea result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM DeliveryArea WHERE DeliveryAreaId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildDeliveryArea(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the DeliveryArea table
        /// </summary>
        public static IEnumerable<DeliveryArea> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM DeliveryArea", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildDeliveryArea(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the DeliveryArea table
        /// </summary>
        public static bool Update(DeliveryArea deliveryArea)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, deliveryArea);
            FinishedWithConnection(cn);
            return result;
        }

        private static bool Update(SqlConnection cn, DeliveryArea deliveryArea)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE DeliveryArea SET DeliveryAreaStreetName=@DeliveryAreaStreetName,DeliveryAreaRangeStart=@DeliveryAreaRangeStart,DeliveryAreaRangeEnd=@DeliveryAreaRangeEnd,DeliveryAreaStreetAddressPrefix=@DeliveryAreaStreetAddressPrefix WHERE DeliveryAreaId=@DeliveryAreaId";

                BuildSqlParameter(sqlCmd, "@DeliveryAreaId", SqlDbType.Int, deliveryArea.Id);
                BuildSqlParameter(sqlCmd, "@DeliveryAreaStreetName", SqlDbType.Text, deliveryArea.StreetName);
                BuildSqlParameter(sqlCmd, "@DeliveryAreaRangeStart", SqlDbType.Int, deliveryArea.StartingAddress);
                BuildSqlParameter(sqlCmd, "@DeliveryAreaRangeEnd", SqlDbType.Int, deliveryArea.EndingAddress);
                BuildSqlParameter(sqlCmd, "@DeliveryAreaStreetAddressPrefix", SqlDbType.NChar, deliveryArea.AddressPrefix);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Build a DeliveryArea object from a SqlDataReader object
        /// </summary>
        private static DeliveryArea BuildDeliveryArea(SqlDataReader rdr)
        {
            return new DeliveryArea(
                GetId(rdr),
                GetStreetName(rdr),
                GetStartingAddress(rdr),
                GetEndingAddress(rdr),
                GetAddressPrefix(rdr),
                GetDeliveryFee(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static string GetStreetName(SqlDataReader rdr)
        {
            return rdr.GetString(1);
        }

        private static int GetStartingAddress(SqlDataReader rdr)
        {
            return rdr.GetInt32(2);
        }

        private static int GetEndingAddress(SqlDataReader rdr)
        {
            return rdr.GetInt32(3);
        }

        private static string GetAddressPrefix(SqlDataReader rdr)
        {
            string addressPrefix = null;
            if (!rdr.IsDBNull(4))
                addressPrefix = rdr.GetString(4);
            return addressPrefix;
        }

        private static double? GetDeliveryFee(SqlDataReader rdr)
        {
            double? deliveryFee = null;
            if (!rdr.IsDBNull(5))
                deliveryFee = rdr.GetDouble(5);
            return deliveryFee;
        }
        #endregion

    }
}
