using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;

namespace TemposUpdateServiceModels
{
    public class License
    {
        public int Id
        {
            get;
            private set;
        }

        public int CustomerId
        {
            get;
            private set;
        }

        public string SerialNumber
        {
            get;
            private set;
        }

        public byte[] IdentityHash
        {
            get;
            private set;
        }

        public bool IsValid
        {
            get;
            private set;
        }

        private License(int id, int customerId, string serialNumber, byte[] identityHash,
            bool isValid)
        {
            Id = id;
            CustomerId = customerId;
            SerialNumber = serialNumber;
            IdentityHash = identityHash;
            IsValid = isValid;
        }

        public void SetCustomerId(int customerId)
        {
            CustomerId = customerId;
        }

        public void SetSerialNumber(string serialNumber)
        {
            SerialNumber = serialNumber;
        }

        public void SetIdentityHash(byte[] identityHash)
        {
            IdentityHash = identityHash;
        }

        public void SetIsValid(bool isValid)
        {
            IsValid = isValid;
        }

        public bool Update()
        {
            return Update(this);
        }

        public static License Add(int customerId, string serialNumber, byte[] identityHash)
        {
            License result = null;

            string query = "INSERT INTO License (LicenseCustomerId, LicenseSerialNumber, LicenseIdentityHash, LicenseIsValid) " +
                "VALUES (@LicenseCustomerId, @LicenseSerialNumber, @LicenseIdentityHash, @LicenseIsValid);" +
                "SELECT CAST(scope_identity() AS int)";
            using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    DatabaseHelper.BuildSqlParameter(cmd, "@LicenseCustomerId", SqlDbType.Int, customerId);
                    DatabaseHelper.BuildSqlParameter(cmd, "@LicenseSerialNumber", SqlDbType.Text, serialNumber);
                    DatabaseHelper.BuildSqlParameter(cmd, "@LicenseIdentityHash", SqlDbType.Binary, identityHash);
                    DatabaseHelper.BuildSqlParameter(cmd, "@LicenseIsValid", SqlDbType.Bit, true);
                    conn.Open();
                    int id = (Int32)cmd.ExecuteScalar();
                    if (id > 0)
                    {
                        result = new License(id, customerId, serialNumber, identityHash, true);
                    }
                }
                conn.Close();
            }

            return result;
        }

        public static License Find(byte[] identityHash)
        {
            License result = null;

            if ((identityHash != null) && (identityHash.Length == 20))
                using (SqlConnection cn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM License WHERE LicenseIdentityHash = @LicenseIdentityHash", cn);
                    DatabaseHelper.BuildSqlParameter(cmd, "@LicenseIdentityHash", SqlDbType.Binary, identityHash);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        result = BuildLicense(rdr);
                    }
                    rdr.Close();
                    cn.Close();
                }

            return result;
        }

        /// <summary>
        /// Get an entry from the License table
        /// </summary>
        public static License Get(int id)
        {
            License result = null;

            using (SqlConnection cn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                cn.Open();
                result = Get(cn, id);
                cn.Close();
            }

            return result;
        }

        private static License Get(SqlConnection cn, int id)
        {
            License result = null;
            SqlCommand cmd = new SqlCommand("SELECT * FROM License WHERE LicenseId=" + id, cn);
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                result = BuildLicense(rdr);
            }
            rdr.Close();
            return result;
        }

        public static License[] GetAll()
        {
            List<License> list = new List<License>();

            using (SqlConnection cn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM License", cn);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(BuildLicense(rdr));
                }
                rdr.Close();
                cn.Close();
            }

            return list.ToArray();
        }
        
        /// <summary>
        /// Update an entry in the License table
        /// </summary>
        public static bool Update(License entry)
        {
            bool result = false;

            using (SqlConnection cn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                cn.Open();
                result = Update(cn, entry);
                cn.Close();
            }

            return result;
        }

        private static bool Update(SqlConnection cn, License entry)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand cmd = cn.CreateCommand())
            {
                cmd.CommandText = "UPDATE License SET LicenseCustomerId=@LicenseCustomerId,LicenseSerialNumber=@LicenseSerialNumber,LicenseIdentityHash=@LicenseIdentityHash,LicenseIsValid=@LicenseIsValid WHERE LicenseId=@LicenseId";

                DatabaseHelper.BuildSqlParameter(cmd, "@LicenseId", SqlDbType.Int, entry.Id);
                DatabaseHelper.BuildSqlParameter(cmd, "@LicenseCustomerId", SqlDbType.Int, entry.CustomerId);
                DatabaseHelper.BuildSqlParameter(cmd, "@LicenseSerialNumber", SqlDbType.Text, entry.SerialNumber);
                DatabaseHelper.BuildSqlParameter(cmd, "@LicenseIdentityHash", SqlDbType.Binary, entry.IdentityHash);
                DatabaseHelper.BuildSqlParameter(cmd, "@LicenseIsValid", SqlDbType.Bit, entry.IsValid);

                rowsAffected = cmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        private static License BuildLicense(SqlDataReader rdr)
        {
            int id = Convert.ToInt32(rdr[0].ToString());
            int customerId = Convert.ToInt32(rdr[1].ToString());
            string serialNumber = rdr[2].ToString();
            byte[] identityHash = rdr.GetSqlBinary(3).Value;
            bool isValid = Convert.ToBoolean(rdr[4].ToString());
            return new License(id, customerId, serialNumber, identityHash, isValid);
        }

    }
}
