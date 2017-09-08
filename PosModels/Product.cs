using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

// NEEDS UPDATING

#if Disabled
namespace MicrosoftSQLApplication.Models.Example
{
    public class Product
    {
        public int Id
        {
            get;
            private set;
        }

        public string Description
        {
            get;
            private set;
        }

        public DateTime PurchaseTime
        {
            get;
            private set;
        }

        public double Cost
        {
            get;
            private set;
        }

        public string Comment
        {
            get;
            private set;
        }

        private Product(int id, string description, DateTime purchaseTime, double cost)
        {
            Id = id;
            Description = description;
            PurchaseTime = purchaseTime;
            Cost = cost;
        }

        private Product(int id, string description, DateTime purchaseTime, double cost, string comment)
            : this(id, description, purchaseTime, cost)
        {
            Comment = comment;
        }

        /// <summary>
        /// Modify the Cost attribute for this object
        /// </summary>
        /// <param name="cost"></param>
        public void SetCost(double cost)
        {
            Cost = cost;
        }

        /// <summary>
        /// Modify the Comment attribute for this object
        /// </summary>
        /// <param name="comment"></param>
        public void SetComment(string comment)
        {
            Comment = comment;
        }

        /// <summary>
        /// Refreash the instance with current database values
        /// </summary>
        public void Refresh()
        {
            Product product = Product.Get(Id);
            // Update this instances fields from the new instance.
        }

        /// <summary>
        /// Update the database entry represented by this object
        /// </summary>
        public bool Update()
        {
            return Product.Update(this);
        }

        #region Static
        /// <summary>
        /// Add a new entry to the Product table
        /// </summary>
        public static Product Add(string description, double cost, string comment)
        {
            Product result = null;
            DateTime purchaseTime = DateTime.Now;

            using (SqlConnection cn = new SqlConnection(LocalSetting.ConnectionString))
            {
                cn.Open();
                string cmd = "AddProduct";
                using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    DatabaseHelper.BuildSqlParameter(sqlCmd, "@ProductDescription", SqlDbType.Text, description);
                    DatabaseHelper.BuildSqlParameter(sqlCmd, "@ProductPurchaseTime", SqlDbType.DateTime, purchaseTime);
                    DatabaseHelper.BuildSqlParameter(sqlCmd, "@ProductCost", SqlDbType.Float, cost);
                    DatabaseHelper.BuildSqlParameter(sqlCmd, "@ProductComment", SqlDbType.Text, comment);
                    DatabaseHelper.BuildSqlParameter(sqlCmd, "@ProductId", SqlDbType.Int, ParameterDirection.ReturnValue);
                    if (sqlCmd.ExecuteNonQuery() > 0)
                    {
                        result = new Product(Convert.ToInt32(sqlCmd.Parameters["@ProductId"].Value),
                            description, purchaseTime, cost, comment);
                    }
                }
                cn.Close();
            }
            return result;
        }


        /// <summary>
        /// Reseeds identity for the Product table
        /// </summary>
        private static void ResetAutoIdentity()
        {
            using (SqlConnection cn = new SqlConnection(LocalSetting.ConnectionString))
            {
                cn.Open();
                string cmd = "DBCC CHECKIDENT (Lock,RESEED,0)";
                using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
                {
                    sqlCmd.ExecuteNonQuery();
                }
                cn.Close();
            }
        }

        /// <summary>
        /// Returns true if the table is empty
        /// </summary>
        public static bool TableIsEmpty()
        {
            bool foundEntry = false;
            using (SqlConnection cn = new SqlConnection(LocalSetting.ConnectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT TOP 1 ProductId FROM Product", cn);
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    if (!rdr.IsDBNull(0))
                        foundEntry = true;
                }
                rdr.Close();
                cn.Close();
            }
            return !foundEntry;
        }

        /// <summary>
        /// Counts the number of entries in the Product table
        /// </summary>
        public static int Count()
        {
            int count = 0;
            using (SqlConnection cn = new SqlConnection(LocalSetting.ConnectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Product", cn);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    count = Convert.ToInt32(rdr[0].ToString());
                }
                rdr.Close();
                cn.Close();
            }
            return count;
        }

        /// <summary>
        /// Delete an entry from the Product table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            using (SqlConnection cn = new SqlConnection(LocalSetting.ConnectionString))
            {
                cn.Open();
                Product product = Get(cn, id);
                if (product != null)
                {
                    using (SqlCommand sqlCmd = cn.CreateCommand())
                    {
                        sqlCmd.CommandText = "DELETE FROM Product WHERE ProductId=" + id;
                        rowsAffected = sqlCmd.ExecuteNonQuery();
                    }
                }
                cn.Close();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the Product table
        /// </summary>
        public static Product Get(int id)
        {
            Product result = null;

            using (SqlConnection cn = new SqlConnection(LocalSetting.ConnectionString))
            {
                cn.Open();
                result = Get(cn, id);
                cn.Close();
            }

            return result;
        }

        private static Product Get(SqlConnection cn, int id)
        {
            Product result = null;
            SqlCommand cmd = new SqlCommand("SELECT * FROM Product WHERE ProductId=" + id, cn);
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                result = BuildProduct(rdr);
            }
            rdr.Close();
            return result;
        }

        /// <summary>
        /// Get all the entries in the Product table
        /// </summary>
        public static IEnumerable<Product> GetAll()
        {
            using (SqlConnection cn = new SqlConnection(LocalSetting.ConnectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Product", cn);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    yield return BuildProduct(rdr);
                }
                rdr.Close();
                cn.Close();
            }
        }

        /// <summary>
        /// Update an entry in the Product table
        /// </summary>
        public static bool Update(Product product)
        {
            bool result = false;

            using (SqlConnection cn = new SqlConnection(LocalSetting.ConnectionString))
            {
                cn.Open();
                result = Update(cn, product);
                cn.Close();
            }

            return result;
        }

        private static bool Update(SqlConnection cn, Product product)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE Product SET ProductDescription=@ProductDescription,ProductPurchaseTime=@ProductPurchaseTime,ProductCost=@ProductCost,ProductComment=@ProductComment WHERE ProductId=@ProductId";

                DatabaseHelper.BuildSqlParameter(sqlCmd, "@ProductId", SqlDbType.Int, product.Id);
                DatabaseHelper.BuildSqlParameter(sqlCmd, "@ProductDescription", SqlDbType.Text, product.Description);
                DatabaseHelper.BuildSqlParameter(sqlCmd, "@ProductPurchaseTime", SqlDbType.DateTime, product.PurchaseTime);
                DatabaseHelper.BuildSqlParameter(sqlCmd, "@ProductCost", SqlDbType.Float, product.Cost);
                DatabaseHelper.BuildSqlParameter(sqlCmd, "@ProductComment", SqlDbType.Text, product.Comment);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Deletes all entries in the Product table
        /// </summary>
        public static void Reset()
        {
            using (SqlConnection cn = new SqlConnection(LocalSetting.ConnectionString))
            {
                cn.Open();
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM Product WHERE ProductId>0";
                    sqlCmd.ExecuteNonQuery();
                }
                cn.Close();
            }
            ResetAutoIdentity();
        }

        /// <summary>
        /// Build a Product object from a SqlDataReader object
        /// </summary>
        private static Product BuildProduct(SqlDataReader rdr)
        {
            int id = Convert.ToInt32(rdr[0].ToString());
            string description = rdr[1].ToString();
            DateTime purchaseTime = Convert.ToDateTime(rdr[2].ToString());
            double cost = Convert.ToDouble(rdr[3].ToString());
            string comment = null;
            
            if ((rdr[4] != null) && (rdr[4].ToString() != ""))
                comment = rdr[4].ToString();

            return new Product(id, description, purchaseTime, cost, comment);
        }
        #endregion
    }
}
#endif