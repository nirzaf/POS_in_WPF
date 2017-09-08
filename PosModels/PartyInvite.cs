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
    public class PartyInvite : DataModelBase
    {
        #region Licensed Access Only
        static PartyInvite()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(PartyInvite).Assembly.GetName().GetPublicKeyToken(),
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
        public int PartyId
        {
            get;
            private set;
        }

        [ModeledData("Name")]
        public string GuestName
        {
            get;
            private set;
        }

        private PartyInvite(int id, int partyId, string guestName)
        {
            Id = id;
            PartyId = partyId;
            GuestName = guestName;
        }

        public void SetGuestName(string guestName)
        {
            GuestName = guestName;
        }

        public bool Update()
        {
            return PartyInvite.Update(this);
        }

        #region static
        /// <summary>
        /// Add a new entry to the PartyInvite table
        /// </summary>
        public static PartyInvite Add(int partyId, string guestName)
        {
            PartyInvite result = null;

            SqlConnection cn = GetConnection();
            string cmd = "AddPartyInvite";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@PartyInvitePartyId", SqlDbType.Int, partyId);
                BuildSqlParameter(sqlCmd, "@PartyInviteName", SqlDbType.Text, guestName);
                BuildSqlParameter(sqlCmd, "@PartyInviteId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new PartyInvite(Convert.ToInt32(sqlCmd.Parameters["@PartyInviteId"].Value),
                        partyId, guestName);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        private static void ResetAutoIdentity()
        {
            SqlConnection cn = GetConnection();
            string cmd = "DBCC CHECKIDENT (PartyInvite,RESEED,0)";
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
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 PartyInviteId FROM PartyInvite", cn))
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
        /// Delete an entry from the PartyInvite table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            PartyInvite partyInvite = Get(cn, id);
            if (partyInvite != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM PartyInvite WHERE PartyInviteId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the PartyInvite table
        /// </summary>
        public static PartyInvite Get(int id)
        {
            PartyInvite result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);

            return result;
        }

        private static PartyInvite Get(SqlConnection cn, int id)
        {
            PartyInvite result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM PartyInvite WHERE PartyInviteId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildPartyInvite(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the PartyInvite table
        /// </summary>
        public static IEnumerable<PartyInvite> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM PartyInvite", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildPartyInvite(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get all the entries, for the specified party, from the PartyInvite table
        /// </summary>
        public static IEnumerable<PartyInvite> GetAll(int partyId)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM PartyInvite WHERE PartyInvitePartyId=" + partyId, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildPartyInvite(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the PartyInvite table
        /// </summary>
        public static bool Update(PartyInvite partyInvite)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, partyInvite);
            FinishedWithConnection(cn);

            return result;
        }

        private static bool Update(SqlConnection cn, PartyInvite partyInvite)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE PartyInvite SET PartyInvitePartyId=@PartyInvitePartyId,PartyInviteName=@PartyInviteName WHERE PartyInviteId=@PartyInviteId";

                BuildSqlParameter(sqlCmd, "@PartyInviteId", SqlDbType.Int, partyInvite.Id);
                BuildSqlParameter(sqlCmd, "@PartyInvitePartyId", SqlDbType.Int, partyInvite.PartyId);
                BuildSqlParameter(sqlCmd, "@PartyInviteName", SqlDbType.Text, partyInvite.GuestName);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        public static void Reset(Type type)
        {
            if (type != typeof(PartyInvite))
                throw new Exception("Bad use of reset");
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM PartyInvite WHERE PartyInviteId>0";
                sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            ResetAutoIdentity();
        }

        /// <summary>
        /// Build a PartyInvite object from a SqlDataReader object
        /// </summary>
        private static PartyInvite BuildPartyInvite(SqlDataReader rdr)
        {
            return new PartyInvite(
                GetId(rdr),
                GetPartyId(rdr),
                GetName(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int GetPartyId(SqlDataReader rdr)
        {
            return rdr.GetInt32(1);
        }

        private static string GetName(SqlDataReader rdr)
        {
            return rdr.GetString(2);
        }
        #endregion

    }
}
