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
    public class VendorOrderItem : DataModelBase
    {
        #region Licensed Access Only
        static VendorOrderItem()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(VendorOrderItem).Assembly.GetName().GetPublicKeyToken(),
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
        public int VendorOrderId
        {
            get;
            private set;
        }

        [ModeledData()]
        public int VendorItemId
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataType("SMALLINT")]
        public int Quantity
        {
            get;
            private set;
        }

        private VendorOrderItem(int id, int vendorOrderId, int vendorItemId,
            int quantity)
        {
            Id = id;
            VendorOrderId = vendorOrderId;
            VendorItemId = vendorItemId;
            Quantity = quantity;
        }

        #region static
        /// <summary>
        /// Add a new entry to the VendorOrderItem table
        /// </summary>
        public static VendorOrderItem Add(int vendorOrderId, int vendorItemId, int quantity)
        {
            VendorOrderItem result = null;

            quantity = quantity.Clamp(1, short.MaxValue);

            SqlConnection cn = GetConnection();
            string cmd = "AddVendorOrderItem";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@VendorOrderItemVendorOrderId", SqlDbType.Int, vendorOrderId);
                BuildSqlParameter(sqlCmd, "@VendorOrderItemVendorItemId", SqlDbType.Int, vendorItemId);
                BuildSqlParameter(sqlCmd, "@VendorOrderItemQuantity", SqlDbType.Int, quantity);
                BuildSqlParameter(sqlCmd, "@VendorOrderItemId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new VendorOrderItem(Convert.ToInt32(sqlCmd.Parameters["@VendorOrderItemId"].Value),
                        vendorOrderId, vendorItemId, quantity);
                }
            }
            FinishedWithConnection(cn);

            return result;
        }

        /// <summary>
        /// Delete an entry from the VendorOrderItem table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            VendorOrderItem vendorOrderItem = Get(cn, id);
            if (vendorOrderItem != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM VendorOrderItem WHERE VendorOrderItemId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the VendorOrderItem table
        /// </summary>
        public static VendorOrderItem Get(int id)
        {
            VendorOrderItem result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);

            return result;
        }

        private static VendorOrderItem Get(SqlConnection cn, int id)
        {
            VendorOrderItem result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM VendorOrderItem WHERE VendorOrderItemId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildVendorOrderItem(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the VendorOrderItem table
        /// </summary>
        public static IEnumerable<VendorOrderItem> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM VendorOrderItem", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildVendorOrderItem(rdr);
                    }
                }
            }
        }

        /// <summary>
        /// Update an entry in the VendorOrderItem table
        /// </summary>
        public static bool Update(VendorOrderItem vendorOrderItem)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, vendorOrderItem);
            FinishedWithConnection(cn);

            return result;
        }

        private static bool Update(SqlConnection cn, VendorOrderItem vendorOrderItem)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE VendorOrderItem SET VendorOrderItemVendorOrderId=@VendorOrderItemVendorOrderId,VendorOrderItemVendorItemId=@VendorOrderItemVendorItemId,VendorOrderItemQuantity=@VendorOrderItemQuantity WHERE VendorOrderItemId=@VendorOrderItemId";

                BuildSqlParameter(sqlCmd, "@VendorOrderItemId", SqlDbType.Int, vendorOrderItem.Id);
                BuildSqlParameter(sqlCmd, "@VendorOrderItemVendorOrderId", SqlDbType.Int, vendorOrderItem.VendorOrderId);
                BuildSqlParameter(sqlCmd, "@VendorOrderItemVendorItemId", SqlDbType.Int, vendorOrderItem.VendorItemId);
                BuildSqlParameter(sqlCmd, "@VendorOrderItemQuantity", SqlDbType.Int, vendorOrderItem.Quantity);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Build a VendorOrderItem object from a SqlDataReader object
        /// </summary>
        private static VendorOrderItem BuildVendorOrderItem(SqlDataReader rdr)
        {
            return new VendorOrderItem(
                GetId(rdr),
                GetVendorOrderId(rdr),
                GetVendorItemId(rdr),
                GetQuantity(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int GetVendorOrderId(SqlDataReader rdr)
        {
            return rdr.GetInt32(1);
        }

        private static int GetVendorItemId(SqlDataReader rdr)
        {
            return rdr.GetInt32(2);
        }

        private static int GetQuantity(SqlDataReader rdr)
        {
            return rdr.GetInt16(3);
        }
        #endregion

    }
}
