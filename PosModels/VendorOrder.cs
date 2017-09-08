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
    [ModeledDataClass]
    public class VendorOrder : DataModelBase
    {
        #region Licensed Access Only
        static VendorOrder()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(VendorOrder).Assembly.GetName().GetPublicKeyToken(),
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
        public int VendorId
        {
            get;
            private set;
        }

        [ModeledData()]
        public int EmployeeId
        {
            get;
            private set;
        }

        [ModeledData("OrderedDate")]
        public DateTime? OrderTime
        {
            get;
            private set;
        }

        private VendorOrder(int id, int vendorId, int employeeId, DateTime? orderTime)
        {
            Id = id;
            VendorId = vendorId;
            EmployeeId = employeeId;
            OrderTime = orderTime;
        }

        #region static
        /// <summary>
        /// Add a new entry to the VendorOrder table
        /// </summary>
        public static VendorOrder Add(int vendorId, int employeeId)
        {
            VendorOrder result = null;

            SqlConnection cn = GetConnection();
            string cmd = "AddVendorOrder";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@VendorOrderVendorId", SqlDbType.Int, vendorId);
                BuildSqlParameter(sqlCmd, "@VendorOrderEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(sqlCmd, "@VendorOrderOrderedDate", SqlDbType.DateTime, null);
                BuildSqlParameter(sqlCmd, "@VendorOrderId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new VendorOrder(Convert.ToInt32(sqlCmd.Parameters["@VendorOrderId"].Value),
                        vendorId, employeeId, null);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Delete an entry from the VendorOrder table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            VendorOrder vendorOrder = Get(cn, id);
            if (vendorOrder != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM VendorOrder WHERE VendorOrderId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the VendorOrder table
        /// </summary>
        public static VendorOrder Get(int id)
        {
            VendorOrder result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);

            return result;
        }

        private static VendorOrder Get(SqlConnection cn, int id)
        {
            VendorOrder result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM VendorOrder WHERE VendorOrderId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildVendorOrder(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the VendorOrder table
        /// </summary>
        public static IEnumerable<VendorOrder> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM VendorOrder", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildVendorOrder(rdr);
                    }
                }
            }
        }

        /// <summary>
        /// Update an entry in the VendorOrder table
        /// </summary>
        public static bool Update(VendorOrder vendorOrder)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, vendorOrder);
            FinishedWithConnection(cn);

            return result;
        }

        private static bool Update(SqlConnection cn, VendorOrder vendorOrder)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE VendorOrder SET VendorOrderVendorId=@VendorOrderVendorId,VendorOrderEmployeeId=@VendorOrderEmployeeId,VendorOrderEmployeeId=@VendorOrderEmployeeId WHERE VendorOrderId=@VendorOrderId";

                BuildSqlParameter(sqlCmd, "@VendorOrderId", SqlDbType.Int, vendorOrder.Id);
                BuildSqlParameter(sqlCmd, "@VendorOrderVendorId", SqlDbType.Int, vendorOrder.VendorId);
                BuildSqlParameter(sqlCmd, "@VendorOrderEmployeeId", SqlDbType.Int, vendorOrder.EmployeeId);
                BuildSqlParameter(sqlCmd, "@VendorOrderOrderedDate", SqlDbType.DateTime, vendorOrder.OrderTime);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Build a VendorOrder object from a SqlDataReader object
        /// </summary>
        private static VendorOrder BuildVendorOrder(SqlDataReader rdr)
        {
            return new VendorOrder(
                GetId(rdr),
                GetVendorId(rdr),
                GetEmployeeId(rdr),
                GetOrderTime(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int GetVendorId(SqlDataReader rdr)
        {
            return rdr.GetInt32(1);
        }

        private static int GetEmployeeId(SqlDataReader rdr)
        {
            return rdr.GetInt32(2);
        }

        private static DateTime? GetOrderTime(SqlDataReader rdr)
        {
            DateTime? orderTime = null;
            if (rdr[3] != null)
                orderTime = rdr.GetDateTime(3);
            return orderTime;
        }
        #endregion

    }
}
