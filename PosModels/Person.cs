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
    public class Person : DataModelBase
    {
        #region Licensed Access Only
        static Person()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(Person).Assembly.GetName().GetPublicKeyToken(),
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
        public string FirstName
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataType("NCHAR")]
        public char? MiddleInitial
        {
            get;
            private set;
        }

        [ModeledData()]
        public string LastName
        {
            get;
            private set;
        }

        [ModeledData()]
        public string AddressLine1
        {
            get;
            private set;
        }

        [ModeledData()]
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
        public int PhoneNumberId1
        {
            get;
            private set;
        }

        [ModeledData()]
        public int PhoneNumberId2
        {
            get;
            private set;
        }

        [ModeledData()]
        public int PhoneNumberId3
        {
            get;
            private set;
        }

        [ModeledData()]
        public int PhoneNumberId4
        {
            get;
            private set;
        }

        [ModeledData()]
        public int PhoneNumberId5
        {
            get;
            private set;
        }

        [ModeledData()]
        public int PhoneNumberId6
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataNullable()]
        public string EMailAddress
        {
            get;
            private set;
        }

        private Person(int id, string firstName, char? middleInitial, string lastName,
            string address1, string address2, int zipCodeId, int phoneNumberId1,
            int phoneNumberId2, int phoneNumberId3, int phoneNumberId4, int phoneNumberId5,
            int phoneNumberId6, string eMailAddress)
        {
            Id = id;
            FirstName = firstName;
            MiddleInitial = middleInitial;
            LastName = lastName;
            AddressLine1 = address1;
            AddressLine2 = address2;
            ZipCodeId = zipCodeId;
            PhoneNumberId1 = phoneNumberId1;
            PhoneNumberId2 = phoneNumberId2;
            PhoneNumberId3 = phoneNumberId3;
            PhoneNumberId4 = phoneNumberId4;
            PhoneNumberId5 = phoneNumberId5;
            PhoneNumberId6 = phoneNumberId6;
            EMailAddress = eMailAddress;
        }

        public void SetFirstName(string firstName)
        {
            FirstName = firstName;
        }

        public void SetMiddleInitial(char? middleInitial)
        {
            MiddleInitial = middleInitial;
        }

        public void SetLastName(string lastName)
        {
            LastName = lastName;
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

        public void SetPhoneNumberId1(int phoneNumberId1)
        {
            PhoneNumberId1 = phoneNumberId1;
        }

        public void SetPhoneNumberId2(int phoneNumberId2)
        {
            PhoneNumberId2 = phoneNumberId2;
        }

        public void SetPhoneNumberId3(int phoneNumberId3)
        {
            PhoneNumberId3 = phoneNumberId3;
        }

        public void SetPhoneNumberId4(int phoneNumberId4)
        {
            PhoneNumberId4 = phoneNumberId4;
        }

        public void SetPhoneNumberId5(int phoneNumberId5)
        {
            PhoneNumberId5 = phoneNumberId5;
        }

        public void SetPhoneNumberId6(int phoneNumberId6)
        {
            PhoneNumberId6 = phoneNumberId6;
        }

        public void SetEMailAddress(string eMailAddress)
        {
            EMailAddress = eMailAddress;
        }

        public bool Update()
        {
            return Person.Update(this);
        }

        #region static
        public static Person Add(string firstName, char? middleInitial, string lastName,
            string address1, string address2, int zipCodeId, int phoneNumberId1,
            int phoneNumberId2, int phoneNumberId3, int phoneNumberId4, int phoneNumberId5,
            int phoneNumberId6, string eMailAddress)
        {
            // Generate ScanCode for a new Person
            Person result = null;

            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = new SqlCommand("AddPerson", cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@PersonFirstName", SqlDbType.Text, firstName);
                BuildSqlParameter(sqlCmd, "@PersonMiddleInitial", SqlDbType.NChar, middleInitial);
                BuildSqlParameter(sqlCmd, "@PersonLastName", SqlDbType.Text, lastName);
                BuildSqlParameter(sqlCmd, "@PersonAddressLine1", SqlDbType.Text, address1);
                BuildSqlParameter(sqlCmd, "@PersonAddressLine2", SqlDbType.Text, address2);
                BuildSqlParameter(sqlCmd, "@PersonZipCodeId", SqlDbType.Int, zipCodeId);
                BuildSqlParameter(sqlCmd, "@PersonPhoneNumberId1", SqlDbType.Int, phoneNumberId1);
                BuildSqlParameter(sqlCmd, "@PersonPhoneNumberId2", SqlDbType.Int, phoneNumberId2);
                BuildSqlParameter(sqlCmd, "@PersonPhoneNumberId3", SqlDbType.Int, phoneNumberId3);
                BuildSqlParameter(sqlCmd, "@PersonPhoneNumberId4", SqlDbType.Int, phoneNumberId4);
                BuildSqlParameter(sqlCmd, "@PersonPhoneNumberId5", SqlDbType.Int, phoneNumberId5);
                BuildSqlParameter(sqlCmd, "@PersonPhoneNumberId6", SqlDbType.Int, phoneNumberId6);
                BuildSqlParameter(sqlCmd, "@PersonEMailAddress", SqlDbType.Text, eMailAddress);
                BuildSqlParameter(sqlCmd, "@PersonId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new Person(Convert.ToInt32(sqlCmd.Parameters["@PersonId"].Value),
                        firstName, middleInitial, lastName, address1, address2, zipCodeId,
                        phoneNumberId1, phoneNumberId2, phoneNumberId3, phoneNumberId4,
                        phoneNumberId5, phoneNumberId6, eMailAddress);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Delete an entry from the Person table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            Person person = Get(cn, id);
            if (person != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM Person WHERE PersonId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the Person table
        /// </summary>
        public static Person Get(int id)
        {
            Person result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);

            return result;
        }

        private static Person Get(SqlConnection cn, int id)
        {
            Person result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Person WHERE PersonId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildPerson(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the Person table
        /// </summary>
        public static IEnumerable<Person> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Person", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildPerson(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the Person table
        /// </summary>
        public static bool Update(Person person)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, person);
            FinishedWithConnection(cn);

            return result;
        }

        private static bool Update(SqlConnection cn, Person person)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE Person SET PersonFirstName=@PersonFirstName,PersonMiddleInitial=@PersonMiddleInitial,PersonLastName=@PersonLastName,PersonAddressLine1=@PersonAddressLine1,PersonAddressLine2=@PersonAddressLine2,PersonZipCodeId=@PersonZipCodeId,PersonPhoneNumberId1=@PersonPhoneNumberId1,PersonPhoneNumberId2=@PersonPhoneNumberId2,PersonPhoneNumberId3=@PersonPhoneNumberId3,PersonPhoneNumberId4=@PersonPhoneNumberId4,PersonPhoneNumberId5=@PersonPhoneNumberId5,PersonPhoneNumberId6=@PersonPhoneNumberId6,PersonEMailAddress=@PersonEMailAddress WHERE PersonId=@PersonId";

                BuildSqlParameter(sqlCmd, "@PersonId", SqlDbType.Int, person.Id);
                BuildSqlParameter(sqlCmd, "@PersonFirstName", SqlDbType.Text, person.FirstName);
                BuildSqlParameter(sqlCmd, "@PersonMiddleInitial", SqlDbType.NChar, person.MiddleInitial);
                BuildSqlParameter(sqlCmd, "@PersonLastName", SqlDbType.Text, person.LastName);
                BuildSqlParameter(sqlCmd, "@PersonAddressLine1", SqlDbType.Text, person.AddressLine1);
                BuildSqlParameter(sqlCmd, "@PersonAddressLine2", SqlDbType.Text, person.AddressLine2);
                BuildSqlParameter(sqlCmd, "@PersonZipCodeId", SqlDbType.Int, person.ZipCodeId);
                BuildSqlParameter(sqlCmd, "@PersonPhoneNumberId1", SqlDbType.Int, person.PhoneNumberId1);
                BuildSqlParameter(sqlCmd, "@PersonPhoneNumberId2", SqlDbType.Int, person.PhoneNumberId2);
                BuildSqlParameter(sqlCmd, "@PersonPhoneNumberId3", SqlDbType.Int, person.PhoneNumberId3);
                BuildSqlParameter(sqlCmd, "@PersonPhoneNumberId4", SqlDbType.Int, person.PhoneNumberId4);
                BuildSqlParameter(sqlCmd, "@PersonPhoneNumberId5", SqlDbType.Int, person.PhoneNumberId5);
                BuildSqlParameter(sqlCmd, "@PersonPhoneNumberId6", SqlDbType.Int, person.PhoneNumberId6);
                BuildSqlParameter(sqlCmd, "@PersonEMailAddress", SqlDbType.Text, person.EMailAddress);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        public static void Refresh(Person person)
        {
            Refresh(person, Person.Get(person.Id));
        }

        public static void Refresh(Person person, Person tempPerson)
        {
            if ((person == null) || (tempPerson == null))
                return;
            person.AddressLine1 = tempPerson.AddressLine1;
            person.AddressLine2 = tempPerson.AddressLine2;
            person.FirstName = tempPerson.FirstName;
            person.LastName = tempPerson.LastName;
            person.MiddleInitial = tempPerson.MiddleInitial;
            person.PhoneNumberId1 = tempPerson.PhoneNumberId1;
            person.PhoneNumberId2 = tempPerson.PhoneNumberId2;
            person.PhoneNumberId3 = tempPerson.PhoneNumberId3;
            person.PhoneNumberId4 = tempPerson.PhoneNumberId4;
            person.PhoneNumberId5 = tempPerson.PhoneNumberId5;
            person.PhoneNumberId6 = tempPerson.PhoneNumberId6;
            person.EMailAddress = tempPerson.EMailAddress;
            person.ZipCodeId = tempPerson.ZipCodeId;
        }

        /// <summary>
        /// Build a Person object from a SqlDataReader object
        /// </summary>
        private static Person BuildPerson(SqlDataReader rdr)
        {
            return new Person(
                GetId(rdr),
                GetFirstName(rdr),
                GetMiddleInitial(rdr),
                GetLastName(rdr),
                GetAddress1(rdr),
                GetAddress2(rdr),
                GetZipCodeId(rdr),
                GetPhoneNumberId1(rdr),
                GetPhoneNumberId2(rdr),
                GetPhoneNumberId3(rdr),
                GetPhoneNumberId4(rdr),
                GetPhoneNumberId5(rdr),
                GetPhoneNumberId6(rdr),
                GetEMailAddress(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static string GetFirstName(SqlDataReader rdr)
        {
            return rdr.GetString(1);
        }

        private static char? GetMiddleInitial(SqlDataReader rdr)
        {
            char? middleInitial = null;
            if (!rdr.IsDBNull(2))
            {
                string result = rdr.GetString(2);
                if (!string.IsNullOrEmpty(result))
                    middleInitial = result[0];
            }
            return middleInitial;
        }
             
        private static string GetLastName(SqlDataReader rdr)
        {
            return rdr.GetString(3);
        }

        private static string GetAddress1(SqlDataReader rdr)
        {
            return rdr.GetString(4);
        }

        private static string GetAddress2(SqlDataReader rdr)
        {
            return rdr.GetString(5);
        }

        private static int GetZipCodeId(SqlDataReader rdr)
        {
            return rdr.GetInt32(6);
        }

        private static int GetPhoneNumberId1(SqlDataReader rdr)
        {
            return rdr.GetInt32(7);
        }

        private static int GetPhoneNumberId2(SqlDataReader rdr)
        {
            return rdr.GetInt32(8);
        }

        private static int GetPhoneNumberId3(SqlDataReader rdr)
        {
            return rdr.GetInt32(9);
        }

        private static int GetPhoneNumberId4(SqlDataReader rdr)
        {
            return rdr.GetInt32(10);
        }

        private static int GetPhoneNumberId5(SqlDataReader rdr)
        {
            return rdr.GetInt32(11);
        }

        private static int GetPhoneNumberId6(SqlDataReader rdr)
        {
            return rdr.GetInt32(12);
        }

        private static string GetEMailAddress(SqlDataReader rdr)
        {
            string eMail = null;
            if (!rdr.IsDBNull(13))
            {
                string value = rdr.GetString(13);
                if (!value.Equals(""))
                    eMail = value;
            }
            return eMail;
        }
        #endregion

    }
}
