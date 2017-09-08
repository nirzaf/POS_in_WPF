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
    public class Party : DataModelBase
    {
        #region Licensed Access Only
        static Party()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(Party).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosModels.dll");
            }
#endif
        }
        #endregion

        public const int NoPartyId = 0;

        [ModeledData()]
        public int Id
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataNullable()]
        public string CustomerName
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataType("SMALLINT")]
        public int Size
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataNullable()]
        public string Note
        {
            get;
            private set;
        }

        private Party(int id, string customerName, int partySize, string note)
        {
            Id = id;
            CustomerName = customerName;
            Size = partySize;
            Note = note;
        }

        public void SetSize(int size)
        {
            Size = MathHelper.Clamp(size, short.MinValue, short.MaxValue);
        }

        public void SetNote(string note)
        {
            Note = note;
        }

        public void SetCustomerName(string customerName)
        {
            CustomerName = customerName;
        }

        public bool Update()
        {
            return Party.Update(this);
        }

        #region Static
        /// <summary>
        /// Add a new entry to the Party table
        /// </summary>
        public static Party Add(string customerName, int partySize, string note)
        {
            Party result = null;

            SqlConnection cn = GetConnection();
            string cmd = "AddParty";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@PartyCustomerName", SqlDbType.Text, customerName);
                BuildSqlParameter(sqlCmd, "@PartySize", SqlDbType.SmallInt, partySize);
                BuildSqlParameter(sqlCmd, "@PartyNote", SqlDbType.Text, note);
                BuildSqlParameter(sqlCmd, "@PartyId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new Party(Convert.ToInt32(sqlCmd.Parameters["@PartyId"].Value),
                        customerName, partySize, note);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        private static void ResetAutoIdentity()
        {
            SqlConnection cn = GetConnection();
            string cmd = "DBCC CHECKIDENT (Party,RESEED,0)";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
        }

        public static bool TableIsEmpty()
        {
            bool foundEntry = false;
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 PartyId FROM Party", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        if (!rdr.IsDBNull(0))
                            foundEntry = true;
                    }
                }
            }
            FinishedWithConnection(cn);
            return !foundEntry;
        }

        /// <summary>
        /// Delete an entry from the Party table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            Party party = Get(cn, id);
            if (party != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM Party WHERE PartyId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the Party table
        /// </summary>
        public static Party Get(int id)
        {
            Party result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);

            return result;
        }

        private static Party Get(SqlConnection cn, int id)
        {
            Party result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Party WHERE PartyId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildParty(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the Party table
        /// </summary>
        public static IEnumerable<Party> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Party", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildParty(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the Party table
        /// </summary>
        public static bool Update(Party party)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, party);
            FinishedWithConnection(cn);

            return result;
        }

        private static bool Update(SqlConnection cn, Party party)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE Party SET PartyCustomerName=@PartyCustomerName,PartySize=@PartySize,PartyNote=@PartyNote WHERE PartyId=@PartyId";

                BuildSqlParameter(sqlCmd, "@PartyId", SqlDbType.Int, party.Id);
                BuildSqlParameter(sqlCmd, "@PartyCustomerName", SqlDbType.Text, party.CustomerName);
                BuildSqlParameter(sqlCmd, "@PartySize", SqlDbType.SmallInt, party.Size);
                BuildSqlParameter(sqlCmd, "@PartyNote", SqlDbType.Text, party.Note);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        public static void Reset(Type type)
        {
            if (type != typeof(Party))
                throw new Exception("Bad use of reset");
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM Party WHERE PartyId>0";
                sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            ResetAutoIdentity();
        }

        /// <summary>
        /// Build a Party object from a SqlDataReader object
        /// </summary>
        private static Party BuildParty(SqlDataReader rdr)
        {
            return new Party(
                GetId(rdr),
                GetCustomerName(rdr),
                GetSize(rdr),
                GetComment(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static string GetCustomerName(SqlDataReader rdr)
        {
            if (!rdr.IsDBNull(1))
                return rdr.GetString(1);
            return null;
        }

        private static int GetSize(SqlDataReader rdr)
        {
            return rdr.GetInt16(2);
        }

        private static string GetComment(SqlDataReader rdr)
        {
            string comment = null;
            if (!rdr.IsDBNull(3))
            {
                string value = rdr.GetString(3);
                if (!value.Equals(""))
                    comment = value;
            }
            return comment;
        }
        #endregion

    }
}
