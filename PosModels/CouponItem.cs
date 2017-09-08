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
    public class CouponItem : DataModelBase
    {
        #region Licensed Access Only
        static CouponItem()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(CouponItem).Assembly.GetName().GetPublicKeyToken(),
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
        public int CouponId
        {
            get;
            private set;
        }

        [ModeledData()]
        public int ItemId
        {
            get;
            private set;
        }

        private CouponItem(int id, int couponId, int itemId)
        {
            Id = id;
            CouponId = couponId;
            ItemId = itemId;
        }

        #region static
        /// <summary>
        /// Add a new entry to the CouponItem table
        /// </summary>
        public static CouponItem Add(int couponId, int itemId)
        {
            CouponItem result = null;

            SqlConnection cn = GetConnection();
            string cmd = "AddCouponItem";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@CouponItemCouponId", SqlDbType.Int, couponId);
                BuildSqlParameter(sqlCmd, "@CouponItemItemId", SqlDbType.Int, itemId);
                BuildSqlParameter(sqlCmd, "@CouponItemId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new CouponItem(Convert.ToInt32(sqlCmd.Parameters["@CouponItemId"].Value),
                        couponId, itemId);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Counts the phoneNumber of CouponItem's for the specified CouponId
        /// </summary>
        /// <param name="couponId"></param>
        public static int Count(int couponId)
        {
            int result = 0;
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM CouponCategory WHERE CouponCategoryCouponId=" + couponId, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = Convert.ToInt32(rdr[0].ToString());
                    }
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Delete an entry from the CouponItem table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            CouponItem couponItem = Get(cn, id);
            if (couponItem != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM CouponItem WHERE CouponItemId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Delete an entry from the CouponCategory table
        /// </summary>
        public static bool Delete(int couponId, int itemId)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM CouponItem WHERE CouponItemCouponId=" + couponId + " AND CouponItemItemId=" + itemId;
                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the CouponItem table
        /// </summary>
        public static CouponItem Get(int id)
        {
            CouponItem result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);
            return result;
        }

        private static CouponItem Get(SqlConnection cn, int id)
        {
            CouponItem result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM CouponItem WHERE CouponItemId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildCouponItem(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Get all the entries in the CouponItem table
        /// </summary>
        public static IEnumerable<CouponItem> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM CouponItem", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildCouponItem(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get all the entries in the CouponItem table, for a particular CouponId
        /// </summary>
        public static IEnumerable<CouponItem> GetAll(int couponId)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM CouponItem WHERE CouponItemCouponId=" + couponId, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildCouponItem(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the CouponItem table
        /// </summary>
        public static bool Update(CouponItem couponItem)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, couponItem);
            FinishedWithConnection(cn);
            return result;
        }

        private static bool Update(SqlConnection cn, CouponItem couponItem)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE CouponItem SET CouponItemCouponId,@CouponItemCouponId,CouponItemItemId=@CouponItemItemId WHERE CouponItemId=@CouponItemId";

                BuildSqlParameter(sqlCmd, "@CouponItemId", SqlDbType.Int, couponItem.Id);
                BuildSqlParameter(sqlCmd, "@CouponItemCouponId", SqlDbType.Int, couponItem.CouponId);
                BuildSqlParameter(sqlCmd, "@CouponItemItemId", SqlDbType.Text, couponItem.ItemId);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        public static CouponItem FindByItemId(IEnumerable<CouponItem> couponItems, int itemId)
        {
            foreach (CouponItem couponItem in couponItems)
            {
                if (couponItem.ItemId == itemId)
                    return couponItem;
            }
            return null;
        }

        public static IEnumerable<CouponItem> FindAllByCouponId(IEnumerable<CouponItem> couponItems, int couponId)
        {
            foreach (CouponItem couponItem in couponItems)
            {
                if (couponItem.CouponId == couponId)
                    yield return couponItem;
            }
        }

        public static bool ArrayContainsItemId(IEnumerable<CouponItem> collection, int itemId)
        {
            bool found = false;
            foreach (CouponItem item in collection)
            {
                if (item.Id == itemId)
                {
                    found = true;
                    break;
                }
            }
            return found;
        }

        /// <summary>
        /// Build a CouponItem object from a SqlDataReader object
        /// </summary>
        private static CouponItem BuildCouponItem(SqlDataReader rdr)
        {
            return new CouponItem(
                GetId(rdr),
                GetCouponId(rdr),
                GetItemId(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int GetCouponId(SqlDataReader rdr)
        {
            return rdr.GetInt32(1);
        }


        private static int GetItemId(SqlDataReader rdr)
        {
            return rdr.GetInt32(2);
        }
        #endregion

    }
}
