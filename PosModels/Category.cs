using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using PosModels.Types;

namespace PosModels
{
    [Table]
    public class Category : DataModelBase, IComparable, IEquatable<Category>
    {
        #region Licensed Access Only
        static Category()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(Category).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosModels.dll");
            }
#endif
        }
        #endregion

        [Column]
        public int Id
        {
            get;
            private set;
        }

        [Column]
        public short DisplayIndex
        {
            get;
            private set;
        }

        [Column(Name = "Name", CanBeNull = false)]
        public string NameValue
        {
            get;
            private set;
        }

        [Column]
        public bool IsDiscontinued
        {
            get;
            private set;
        }

        private Category(int id, short displayIndex, string value, bool isDiscontinued)
        {
            Id = id;
            DisplayIndex = displayIndex;
            NameValue = value;
            IsDiscontinued = isDiscontinued;
        }

        public void Discontinue()
        {
            DisplayIndex = -1;
            IsDiscontinued = true;
            Update(this);
        }

        public void SetDisplayIndex(short displayIndex)
        {
            DisplayIndex = displayIndex;
        }

        public void SetName(string name)
        {
            NameValue = name;
        }

        public bool Update()
        {
            return Update(this);
        }

        #region static
        /// <summary>
        /// Add an entry to the Category table
        /// </summary>
        public static Category Add(string categoryName, short displayIndex)
        {
            Category result = null;
            SqlConnection cn = GetConnection();
            using (var sqlCmd = new SqlCommand("AddCategory", cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@CategoryName", SqlDbType.Text, categoryName);
                BuildSqlParameter(sqlCmd, "@CategoryDisplayIndex", SqlDbType.SmallInt, displayIndex);
                BuildSqlParameter(sqlCmd, "@CategoryIsDiscontinued", SqlDbType.Bit, false);
                BuildSqlParameter(sqlCmd, "@CategoryId", SqlDbType.SmallInt, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new Category(Convert.ToInt32(sqlCmd.Parameters["@CategoryId"].Value),
                        displayIndex, categoryName, false);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        public static Category Get(string categoryName)
        {
            SqlConnection cn = GetConnection();
            Category result = Get(cn, categoryName);
            FinishedWithConnection(cn);
            return result;
        }

        private static Category Get(SqlConnection cn, string categoryName)
        {
            Category result = null;            
            using (var cmd = new SqlCommand("SELECT * FROM Category WHERE CategoryName LIKE @CategoryName AND CategoryIsDiscontinued=0", cn))
            {
                BuildSqlParameter(cmd, "@CategoryName", SqlDbType.Text, categoryName);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildCategory(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get an entry from the Product table
        /// </summary>
        public static Category Get(int id)
        {
            SqlConnection cn = GetConnection();
            Category result = Get(cn, id);
            FinishedWithConnection(cn);
            return result;
        }

        private static Category Get(SqlConnection cn, int id)
        {
            Category result = null;
            using (var cmd = new SqlCommand("SELECT * FROM Category WHERE CategoryId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildCategory(rdr);
                    }
                }
            }
            return result;
        }

        public static IEnumerable<Category> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (var cmd = new SqlCommand("SELECT * FROM Category WHERE CategoryIsDiscontinued=0", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildCategory(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static bool Update(Category category)
        {
            SqlConnection cn = GetConnection();
            bool result = Update(cn, category);
            FinishedWithConnection(cn);
            return result;
        }

        private static bool Update(SqlConnection cn, Category category)
        {
            Int32 rowsAffected;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE Category SET CategoryDisplayIndex=@CategoryDisplayIndex,CategoryName=@CategoryName,CategoryIsDiscontinued=@CategoryIsDiscontinued WHERE CategoryId=@CategoryId";

                BuildSqlParameter(sqlCmd, "@CategoryId", SqlDbType.Int, category.Id);
                BuildSqlParameter(sqlCmd, "@CategoryDisplayIndex", SqlDbType.SmallInt, category.DisplayIndex);
                BuildSqlParameter(sqlCmd, "@CategoryName", SqlDbType.Text, category.NameValue);
                BuildSqlParameter(sqlCmd, "@CategoryIsDiscontinued", SqlDbType.Bit, category.IsDiscontinued);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        private static Category BuildCategory(SqlDataReader rdr)
        {
            return new Category(
                GetId(rdr),
                GetDisplayIndex(rdr),
                GetName(rdr),
                GetIsDiscontinued(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static short GetDisplayIndex(SqlDataReader rdr)
        {
            return rdr.GetInt16(1);
        }

        private static string GetName(SqlDataReader rdr)
        {
            return rdr.GetString(2);
        }

        private static bool GetIsDiscontinued(SqlDataReader rdr)
        {
            return rdr.GetBoolean(3);
        }

        public static Category FindById(IEnumerable<Category> collection, int id)
        {
            return collection.FirstOrDefault(category => category.Id == id);
        }

        public static Category FindByName(IEnumerable<Category> collection, string name)
        {
            return collection.FirstOrDefault(category => category.NameValue.Equals(name));
        }
        #endregion   

        public int CompareTo(object obj)
        {
            if (obj is Category)
            {
                Category category = (Category)obj;
                if (category.DisplayIndex > this.DisplayIndex)
                    return -1;
                if (category.DisplayIndex < this.DisplayIndex)
                    return 1;
                return 0;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public bool Equals(Category category)
        {
            return (category.DisplayIndex == this.DisplayIndex);
        }
    }
}
