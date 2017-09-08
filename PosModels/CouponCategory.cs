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
    public class CouponCategory : DataModelBase
    {
        #region Licensed Access Only
        static CouponCategory()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(CouponCategory).Assembly.GetName().GetPublicKeyToken(),
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
        public int CategoryId
        {
            get;
            private set;
        }

        private CouponCategory(int id, int couponId, int categoryId)
        {
            Id = id;
            CouponId = couponId;
            CategoryId = categoryId;
        }

        #region static
        /// <summary>
        /// Add a new entry to the CouponCategory table
        /// </summary>
        public static CouponCategory Add(int couponId, int categoryId)
        {
            CouponCategory result = null;

            SqlConnection cn = GetConnection(); 
            string cmd = "AddCouponCategory";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@CouponCategoryCouponId", SqlDbType.Int, couponId);
                BuildSqlParameter(sqlCmd, "@CouponCategoryCategoryId", SqlDbType.Int, categoryId);
                BuildSqlParameter(sqlCmd, "@CouponCategoryId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new CouponCategory(Convert.ToInt32(sqlCmd.Parameters["@CouponCategoryId"].Value),
                        couponId, categoryId);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Counts the phoneNumber of CouponCategory's for the specified CouponId
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
        /// Delete an entry from the CouponCategory table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            CouponCategory couponCategory = Get(cn, id);
            if (couponCategory != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM CouponCategory WHERE CouponCategoryId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Delete an entry from the CouponCategory table
        /// </summary>
        public static bool Delete(int couponId, int categoryId)
        {
            Int32 rowsAffected = 0;
            
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM CouponCategory WHERE CouponCategoryCouponId=" + couponId + " AND CouponCategoryCategoryId=" + categoryId;
                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the CouponCategory table
        /// </summary>
        public static CouponCategory Get(int id)
        {
            CouponCategory result = null;

            SqlConnection cn = GetConnection(); 
            result = Get(cn, id);
            FinishedWithConnection(cn);
            return result;
        }

        private static CouponCategory Get(SqlConnection cn, int id)
        {
            CouponCategory result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM CouponCategory WHERE CouponCategoryId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildCouponCategory(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the CouponCategory table
        /// </summary>
        public static IEnumerable<CouponCategory> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM CouponCategory", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildCouponCategory(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get all the entries in the CouponCategory table, for a particular CouponId
        /// </summary>
        public static IEnumerable<CouponCategory> GetAll(int couponId)
        {
            SqlConnection cn = GetConnection(); 
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM CouponCategory WHERE CouponCategoryCouponId=" + couponId, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildCouponCategory(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the CouponCategory table
        /// </summary>
        public static bool Update(CouponCategory couponCategory)
        {
            bool result = false;

            SqlConnection cn = GetConnection(); 
            result = Update(cn, couponCategory);
            FinishedWithConnection(cn);
            return result;
        }

        private static bool Update(SqlConnection cn, CouponCategory couponCategory)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE CouponCategory SET CouponCategoryCouponId=@CouponCategoryCouponId,CouponCategoryCategoryId=@CouponCategoryCategoryId WHERE CouponCategoryId=@CouponCategoryId";

                BuildSqlParameter(sqlCmd, "@CouponCategoryId", SqlDbType.Int, couponCategory.Id);
                BuildSqlParameter(sqlCmd, "@CouponCategoryCouponId", SqlDbType.Int, couponCategory.CouponId);
                BuildSqlParameter(sqlCmd, "@CouponCategoryCategoryId", SqlDbType.Int, couponCategory.CategoryId);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        public static CouponCategory FindByCategoryId(IEnumerable<CouponCategory> couponCategories, int categoryId)
        {
            foreach (CouponCategory couponCategory in couponCategories)
            {
                if (couponCategory.CategoryId == categoryId)
                    return couponCategory;
            }
            return null;
        }

        public static IEnumerable<CouponCategory> FindAllByCouponId(IEnumerable<CouponCategory> couponCategories, int couponId)
        {
            foreach (CouponCategory couponCategory in couponCategories)
            {
                if (couponCategory.CouponId == couponId)
                    yield return couponCategory;
            }
        }

        public static bool ContainsCategoryId(IEnumerable<CouponCategory> collection, int categoryId)
        {
            bool found = false;
            foreach (CouponCategory category in collection)
            {
                if (category.Id == categoryId)
                {
                    found = true;
                    break;
                }
            }
            return found;
        }

        /// <summary>
        /// Build a CouponCategory object from a SqlDataReader object
        /// </summary>
        private static CouponCategory BuildCouponCategory(SqlDataReader rdr)
        {
            return new CouponCategory(
                GetId(rdr),
                GetCouponId(rdr),
                GetCategoryId(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int GetCouponId(SqlDataReader rdr)
        {
            return rdr.GetInt32(1);
        }

        private static int GetCategoryId(SqlDataReader rdr)
        {
            return rdr.GetInt32(2);
        }

        #endregion
    }
}
