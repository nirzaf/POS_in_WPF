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
    public class ZipCodeState : DataModelBase
    {
        #region Licensed Access Only
        static ZipCodeState()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(ZipCodeState).Assembly.GetName().GetPublicKeyToken(),
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
        public string Abbreviation
        {
            get;
            private set;
        }

        [ModeledData("Name")]
        public string StateName
        {
            get;
            private set;
        }

        private ZipCodeState(int id, string abbreviations, string stateName)
        {
            Id = id;
            Abbreviation = abbreviations;
            StateName = stateName;
        }

        #region static
        /// <summary>
        /// Get an entry from the ZipCodeState table
        /// </summary>
        public static ZipCodeState GetByName(string name)
        {
            ZipCodeState result = null;
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ZipCodeState WHERE ZipCodeStateAbbreviation LIKE @ZipCodeStateAbbreviation OR ZipCodeStateName LIKE @ZipCodeStateName", cn))
            {
                BuildSqlParameter(cmd, "@ZipCodeStateAbbreviation", SqlDbType.Text, name);
                BuildSqlParameter(cmd, "@ZipCodeStateName", SqlDbType.Text, name);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildZipCodeState(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Get an entry from the ZipCodeState table
        /// </summary>
        public static ZipCodeState Get(int id)
        {
            ZipCodeState result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);

            return result;
        }

        private static ZipCodeState Get(SqlConnection cn, int id)
        {
            ZipCodeState result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ZipCodeState WHERE ZipCodeStateId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildZipCodeState(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the ZipCodeState table
        /// </summary>
        public static IEnumerable<ZipCodeState> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ZipCodeState", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildZipCodeState(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Build a ZipCodeState object from a SqlDataReader object
        /// </summary>
        private static ZipCodeState BuildZipCodeState(SqlDataReader rdr)
        {
            return new ZipCodeState(
                GetId(rdr),
                GetAbbreviation(rdr),
                GetStateName(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static string GetAbbreviation(SqlDataReader rdr)
        {
            return rdr.GetString(1);
        }

        private static string GetStateName(SqlDataReader rdr)
        {
            return rdr.GetString(2);
        }
        #endregion

    }
}
