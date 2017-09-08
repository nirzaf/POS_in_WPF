using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using PosModels.Types;
using System.Data.SqlClient;
using PosModels.Managers;
using System.Data;
using PosModels.Helpers;

namespace PosModels
{
    [ModeledDataClass()]
    public class ItemGroup : DataModelBase
    {
        #region Licensed Access Only
        static ItemGroup()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(ItemGroup).Assembly.GetName().GetPublicKeyToken(),
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
        public int SourceItemId
        {
            get;
            private set;
        }

        [ModeledData()]
        public int TargetItemId
        {
            get;
            private set;
        }

        [ModeledData()]
        public int TargetItemQuantity
        {
            get;
            private set;
        }

        private ItemGroup(int id, int sourceItemId, int targetItemId, int targetItemQuantity)
        {
            Id = id;
            SourceItemId = sourceItemId;
            TargetItemId = targetItemId;
            TargetItemQuantity = targetItemQuantity;
        }

        public void SetSourceItemId(int itemId)
        {
            SourceItemId = itemId;
        }

        public void SetTargetItemId(int itemId)
        {
            TargetItemId = itemId;
        }

        public void SetTargetItemQuantity(int quantity)
        {
            TargetItemQuantity = quantity;
        }

        public bool Update()
        {
            return Update(this);
        }

        #region static
        public static ItemGroup Add(int sourceItemId, int targetItemId, int targetItemQuantity)
        {
            ItemGroup result = null;
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = new SqlCommand("AddItemGroup", cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@ItemGroupSourceItemId", SqlDbType.Int, sourceItemId);
                BuildSqlParameter(sqlCmd, "@ItemGroupTargetItemId", SqlDbType.Int, targetItemId);
                BuildSqlParameter(sqlCmd, "@ItemGroupTargetItemQuantity", SqlDbType.Int, targetItemQuantity);
                BuildSqlParameter(sqlCmd, "@ItemGroupId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new ItemGroup(Convert.ToInt32(sqlCmd.Parameters["@ItemGroupId"].Value), 
                        sourceItemId, targetItemId, targetItemQuantity);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        public static bool Delete(int itemGroupId)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM ItemGroup WHERE ItemGroupId=" + itemGroupId;
                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        public static bool IsGroupMember(int itemId)
        {
            bool result = false;
            string sqlCmdText =
                "SELECT TOP 1 ItemGroupId FROM ItemGroup " +
                "WHERE (ItemGroupSourceItemId=" + itemId + " OR ItemGroupTargetItemId=" + itemId + ")";
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand(sqlCmdText, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        result = true;
                    }
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        public static ItemGroup Get(int itemGroupId)
        {
            ItemGroup result = null;

            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ItemGroup WHERE ItemGroupId=" + itemGroupId, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildItemGroup(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        public static int? GetTargetItemQuantity(int sourceItemId, int targetItemId)
        {
            int? result = null;
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand(
                "SELECT ItemGroupTargetItemQuantity FROM ItemGroup " + 
                "WHERE (ItemGroupSourceItemId=@ItemGroupSourceItemId AND ItemGroupTargetItemId=@ItemGroupTargetItemId)", cn))
            {
                BuildSqlParameter(cmd, "@ItemGroupSourceItemId", SqlDbType.Int, sourceItemId);
                BuildSqlParameter(cmd, "@ItemGroupTargetItemId", SqlDbType.Int, targetItemId);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        result = rdr.GetInt32(0);
                    }
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        public static IEnumerable<ItemGroup> GetAll(int? itemId = null)
        {
            string sqlCmdText = ((itemId == null) ?
                "SELECT * FROM ItemGroup" :
                "SELECT * FROM ItemGroup WHERE ItemGroupSourceItemId=" + itemId.Value);
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand(sqlCmdText, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildItemGroup(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static bool Update(ItemGroup itemGroup)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE ItemGroup SET ItemGroupSourceItemId=@ItemGroupSourceItemId,ItemGroupTargetItemId=@ItemGroupTargetItemId,ItemGroupTargetItemQuantity=@ItemGroupTargetItemQuantity WHERE ItemGroupId=@ItemGroupId";

                BuildSqlParameter(sqlCmd, "@ItemGroupId", SqlDbType.Int, itemGroup.Id);
                BuildSqlParameter(sqlCmd, "@ItemGroupSourceItemId", SqlDbType.Int, itemGroup.SourceItemId);
                BuildSqlParameter(sqlCmd, "@ItemGroupTargetItemId", SqlDbType.Int, itemGroup.TargetItemId);
                BuildSqlParameter(sqlCmd, "@ItemGroupTargetItemQuantity", SqlDbType.Int, itemGroup.TargetItemQuantity);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        private static ItemGroup BuildItemGroup(SqlDataReader rdr)
        {
            return new ItemGroup(
                GetId(rdr),
                GetSourceItemId(rdr),
                GetTargetItemId(rdr),
                GetTargerItemQuantity(rdr));

        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int GetSourceItemId(SqlDataReader rdr)
        {
            return rdr.GetInt32(1);
        }

        private static int GetTargetItemId(SqlDataReader rdr)
        {
            return rdr.GetInt32(2);
        }

        private static int GetTargerItemQuantity(SqlDataReader rdr)
        {
            return rdr.GetInt32(3);
        }
        #endregion
    }
}
