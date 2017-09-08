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
    public class Vendor : DataModelBase
    {
        #region Licensed Access Only
        static Vendor()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(Vendor).Assembly.GetName().GetPublicKeyToken(),
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

        [ModeledData("CompanyName")]
        public string VendorName
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataNullable()]
        public string ContactName
        {
            get;
            private set;
        }

        [ModeledData("Address1")]
        public string AddressLine1
        {
            get;
            private set;
        }

        [ModeledData("Address2")]
        [ModeledDataNullable()]
        public string AddressLine2
        {
            get;
            private set;
        }

        [ModeledData()]
        public int ZipCodeId
        {
            get;
            private set;
        }

        [ModeledData()]
        public int PhoneNumberId
        {
            get;
            private set;
        }

        [ModeledData()]
        public int PhoneNumberSetId
        {
            get;
            private set;
        }

        private Vendor(int id, string vendorName, string contactName, string addressLine1,
            string addressLine2, int zipCodeId, int phoneNumberId, int phoneNumberSetId)
        {
            Id = id;
            VendorName = vendorName;
            ContactName = contactName;
            AddressLine1 = addressLine1;
            AddressLine2 = addressLine2;
            ZipCodeId = zipCodeId;
            PhoneNumberId = phoneNumberId;
            PhoneNumberSetId = phoneNumberSetId;
        }

        public void SetVendorName(string vendorName)
        {
            VendorName = vendorName;
        }

        public void SetContactName(string contactName)
        {
            ContactName = contactName;
        }

        public void SetAddressLine1(string addressLine1)
        {
            AddressLine1 = addressLine1;
        }

        public void SetAddressLine2(string addressLine2)
        {
            AddressLine2 = addressLine2;
        }

        public void SetZipCodeId(int zipCodeId)
        {
            ZipCodeId = zipCodeId;
        }

        public void SetPhoneNumberId(int phoneNumberId)
        {
            PhoneNumberId = phoneNumberId;
        }

        public void SetPhoneNumberSetId(int phoneNumberSetId)
        {
            PhoneNumberSetId = phoneNumberSetId;
        }

        #region static
        /// <summary>
        /// Add a new entry to the Vendor table
        /// </summary>
        public static Vendor Add(string vendorName, string contactName, string addressLine1,
            string addressLine2, int zipCodeId, int phoneNumberId, int phoneNumberSetId)
        {
            Vendor result = null;
            DateTime purchaseTime = DateTime.Now;

            SqlConnection cn = GetConnection();
            string cmd = "AddVendor";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@VendorName", SqlDbType.Text, vendorName);
                BuildSqlParameter(sqlCmd, "@VendorContactName", SqlDbType.Text, contactName);
                BuildSqlParameter(sqlCmd, "@VendorAddress1", SqlDbType.Text, addressLine1);
                BuildSqlParameter(sqlCmd, "@VendorAddress2", SqlDbType.Text, addressLine2);
                BuildSqlParameter(sqlCmd, "@VendorZipCodeId", SqlDbType.Int, zipCodeId);
                BuildSqlParameter(sqlCmd, "@VendorPhoneNumberId", SqlDbType.Int, phoneNumberId);
                BuildSqlParameter(sqlCmd, "@VendorPhoneNumberSetId", SqlDbType.Int, phoneNumberSetId);
                BuildSqlParameter(sqlCmd, "@VendorId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new Vendor(Convert.ToInt32(sqlCmd.Parameters["@VendorId"].Value),
                        vendorName, contactName, addressLine1, addressLine2, zipCodeId,
                        phoneNumberId, phoneNumberSetId);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Delete an entry from the Vendor table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            Vendor vendor = Get(cn, id);
            if (vendor != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM Vendor WHERE VendorId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the Vendor table
        /// </summary>
        public static Vendor Get(int id)
        {
            Vendor result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);

            return result;
        }

        private static Vendor Get(SqlConnection cn, int id)
        {
            Vendor result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Vendor WHERE VendorId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildVendor(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the Vendor table
        /// </summary>
        public static IEnumerable<Vendor> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Vendor", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildVendor(rdr);
                    }
                }
            }
        }

        /// <summary>
        /// Update an entry in the Vendor table
        /// </summary>
        public static bool Update(Vendor vendor)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, vendor);
            FinishedWithConnection(cn);

            return result;
        }

        private static bool Update(SqlConnection cn, Vendor vendor)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE Vendor SET VendorName=@VendorName,VendorContactName=@VendorContactName,VendorAddress1=@VendorAddress1,VendorAddress2=@VendorAddress2,VendorZipCodeId=@VendorZipCodeId,VendorPhoneNumberId=@VendorPhoneNumberId,VendorPhoneNumberSetId=@VendorPhoneNumberSetId WHERE VendorId=@VendorId";

                BuildSqlParameter(sqlCmd, "@VendorId", SqlDbType.Int, vendor.Id);
                BuildSqlParameter(sqlCmd, "@VendorName", SqlDbType.Text, vendor.VendorName);
                BuildSqlParameter(sqlCmd, "@VendorContactName", SqlDbType.Text, vendor.ContactName);
                BuildSqlParameter(sqlCmd, "@VendorAddress1", SqlDbType.Text, vendor.AddressLine1);
                BuildSqlParameter(sqlCmd, "@VendorAddress2", SqlDbType.Text, vendor.AddressLine2);
                BuildSqlParameter(sqlCmd, "@VendorZipCodeId", SqlDbType.Int, vendor.ZipCodeId);
                BuildSqlParameter(sqlCmd, "@VendorPhoneNumberId", SqlDbType.Int, vendor.PhoneNumberId);
                BuildSqlParameter(sqlCmd, "@VendorPhoneNumberSetId", SqlDbType.Int, vendor.PhoneNumberSetId);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Build a Vendor object from a SqlDataReader object
        /// </summary>
        private static Vendor BuildVendor(SqlDataReader rdr)
        {
            return new Vendor(
                GetId(rdr),
                GetName(rdr),
                GetContactName(rdr),
                GetAddress1(rdr),
                GetAddress2(rdr),
                GetZipCodeId(rdr),
                GetPhoneNumberId(rdr),
                GetPhoneNumberSetId(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static string GetName(SqlDataReader rdr)
        {
            return rdr.GetString(1);
        }

        private static string GetContactName(SqlDataReader rdr)
        {
            string contactName = null;
            if (!rdr.IsDBNull(2))
            {
                string value = rdr.GetString(2);
                if (!value.Equals(""))
                    contactName = value;
            }
            return contactName;
        }

        private static string GetAddress1(SqlDataReader rdr)
        {
            return rdr.GetString(3);
        }

        private static string GetAddress2(SqlDataReader rdr)
        {
            string address2 = null;
            if (!rdr.IsDBNull(4))
            {
                string value = rdr.GetString(4);
                if (!value.Equals(""))
                    address2 = value;
            }
            return address2;
        }

        private static int GetZipCodeId(SqlDataReader rdr)
        {
            return rdr.GetInt32(5);
        }

        private static int GetPhoneNumberId(SqlDataReader rdr)
        {
            return rdr.GetInt32(6);
        }

        private static int GetPhoneNumberSetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(7);
        }
        #endregion

    }
}
