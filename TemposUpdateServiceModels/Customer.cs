using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace TemposUpdateServiceModels
{
    public class Customer
    {
        public int Id
        {
            get;
            private set;
        }

        public string BusinessName
        {
            get;
            private set;
        }

        public string ContactsName
        {
            get;
            private set;
        }

        public string Address
        {
            get;
            private set;
        }

        public string City
        {
            get;
            private set;
        }

        public string State
        {
            get;
            private set;
        }

        public string ZipCode
        {
            get;
            private set;
        }

        public string Phone1
        {
            get;
            private set;
        }

        public string Phone2
        {
            get;
            private set;
        }

        private Customer(int id, string businessName, string contactName, string address,
            string city, string state, string zipCode, string phone1, string phone2)
        {
            Id = id;
            BusinessName = businessName;
            ContactsName = contactName;
            Address = address;
            City = city;
            State = state;
            ZipCode = zipCode;
            Phone1 = phone1;
            Phone2 = phone2;
        }

        public void SetBusinessName(string businessName)
        {
            BusinessName = businessName;
        }

        public void SetContactsName(string contactsName)
        {
            ContactsName = contactsName;
        }

        public void SetAddress(string address)
        {
            Address = address;
        }

        public void SetCity(string city)
        {
            City = city;
        }

        public void SetState(string state)
        {
            State = state;
        }

        public void SetZipCode(string zipCode)
        {
            ZipCode = zipCode;
        }

        public void SetPhone1(string phone1)
        {
            Phone1 = phone1;
        }

        public void SetPhone2(string phone2)
        {
            Phone2 = phone2;
        }

        public bool Update()
        {
            return Update(this);
        }


        public static Customer Add(string businessName, string contactName, string address,
            string city, string state, string zipCode, string phone1, string phone2)
        {
            Customer result = null;

            string query = "INSERT INTO Customer (CustomerBusinessName, CustomerName, CustomerAddress, CustomerCity, CustomerState, CustomerZip, CustomerPhone1, CustomerPhone2) " +
                "VALUES (@CustomerBusinessName, @CustomerName, @CustomerAddress, @CustomerCity, @CustomerState, @CustomerZip, @CustomerPhone1, @CustomerPhone2);" +
                "SELECT CAST(scope_identity() AS int)";
            using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    DatabaseHelper.BuildSqlParameter(cmd, "@CustomerBusinessName", SqlDbType.Text, businessName);
                    DatabaseHelper.BuildSqlParameter(cmd, "@CustomerName", SqlDbType.Text, contactName);
                    DatabaseHelper.BuildSqlParameter(cmd, "@CustomerAddress", SqlDbType.Text, address);
                    DatabaseHelper.BuildSqlParameter(cmd, "@CustomerCity", SqlDbType.Text, city);
                    DatabaseHelper.BuildSqlParameter(cmd, "@CustomerState", SqlDbType.Text, state);
                    DatabaseHelper.BuildSqlParameter(cmd, "@CustomerZip", SqlDbType.Text, zipCode);
                    DatabaseHelper.BuildSqlParameter(cmd, "@CustomerPhone1", SqlDbType.Text, phone1);
                    DatabaseHelper.BuildSqlParameter(cmd, "@CustomerPhone2", SqlDbType.Text, phone2);
                    conn.Open();
                    int id = (Int32)cmd.ExecuteScalar();
                    if (id > 0)
                    {
                        result = new Customer(id, businessName, contactName, address,
                            city, state, zipCode, phone1, phone2);
                    }
                }
                conn.Close();
            }

            return result;
        }


        public static Customer Get(int customerId)
        {
            Customer result = null;

            using (SqlConnection cn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                cn.Open();
                result = Get(cn, customerId);
                cn.Close();
            }

            return result;
        }

        private static Customer Get(SqlConnection cn, int id)
        {
            Customer result = null;
            SqlCommand cmd = new SqlCommand("SELECT * FROM Customer WHERE CustomerId=" + id, cn);
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                result = BuildCustomer(rdr);
            }
            rdr.Close();
            return result;
        }

        public static Customer[] GetAll()
        {
            List<Customer> list = new List<Customer>();

            using (SqlConnection cn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Customer", cn);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(BuildCustomer(rdr));
                }
                rdr.Close();
                cn.Close();
            }

            return list.ToArray();
        }

        /// <summary>
        /// Update an entry in the Customer table
        /// </summary>
        public static bool Update(Customer entry)
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

        private static bool Update(SqlConnection cn, Customer entry)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand cmd = cn.CreateCommand())
            {
                cmd.CommandText = "UPDATE Customer SET CustomerBusinessName=@CustomerBusinessName,CustomerName=@CustomerName,CustomerAddress=@CustomerAddress,CustomerCity=@CustomerCity,CustomerState=@CustomerState,CustomerZip=@CustomerZip,CustomerPhone1=@CustomerPhone1,CustomerPhone2=@CustomerPhone2 WHERE CustomerId=@CustomerId";

                DatabaseHelper.BuildSqlParameter(cmd, "@CustomerId", SqlDbType.Int, entry.Id);
                DatabaseHelper.BuildSqlParameter(cmd, "@CustomerBusinessName", SqlDbType.Text, entry.BusinessName);
                DatabaseHelper.BuildSqlParameter(cmd, "@CustomerName", SqlDbType.Text, entry.ContactsName);
                DatabaseHelper.BuildSqlParameter(cmd, "@CustomerAddress", SqlDbType.Text, entry.Address);
                DatabaseHelper.BuildSqlParameter(cmd, "@CustomerCity", SqlDbType.Text, entry.City);
                DatabaseHelper.BuildSqlParameter(cmd, "@CustomerState", SqlDbType.Text, entry.State);
                DatabaseHelper.BuildSqlParameter(cmd, "@CustomerZip", SqlDbType.Text, entry.ZipCode);
                DatabaseHelper.BuildSqlParameter(cmd, "@CustomerPhone1", SqlDbType.Text, entry.Phone1);
                DatabaseHelper.BuildSqlParameter(cmd, "@CustomerPhone2", SqlDbType.Text, entry.Phone2);

                rowsAffected = cmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        private static Customer BuildCustomer(SqlDataReader rdr)
        {
            int id = Convert.ToInt32(rdr[0].ToString());
            string businessName = rdr[1].ToString();
            string contactName = rdr[2].ToString();
            string address = rdr[3].ToString();
            string city = rdr[4].ToString();
            string state = rdr[5].ToString();
            string zipCode = rdr[6].ToString();
            string phone1 = rdr[7].ToString();
            string phone2 = null;
            if (!rdr.IsDBNull(8))
                phone2 = rdr[8].ToString();
            return new Customer(id, businessName, contactName, address, city, state,
                zipCode, phone1, phone2);
        }
    }
}
