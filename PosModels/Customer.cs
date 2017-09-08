using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using PosModels.Helpers;
using PosModels.Managers;
using PosModels.Types;

namespace PosModels
{
    [ModeledDataClass()]
    public class Customer : DataModelBase
    {
        #region Licensed Access Only
        static Customer()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(Customer).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosModels.dll");
            }
#endif
        }
        #endregion

        public const int NoCustomerId = 0;

        [ModeledData()]
        public int Id
        {
            get;
            private set;
        }

        [ModeledData()]
        public int PersonId
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataNullable()]
        public string Comment
        {
            get;
            private set;
        }

        private Customer(int id, int personId, string comment)
        {
            Id = id;
            PersonId = personId;
            Comment = comment;
        }

        public void SetComment(string comment)
        {
            Comment = comment;
        }

        public bool Update()
        {
            return Customer.Update(this);
        }

        #region static
        /// <summary>
        /// Add a new entry to the Customer table
        /// </summary>
        public static Customer Add(int personId, string comment)
        {
            Customer result = null;

            SqlConnection cn = GetConnection();
            string cmd = "AddCustomer";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@CustomerPersonId", SqlDbType.Int, personId);
                BuildSqlParameter(sqlCmd, "@CustomerComment", SqlDbType.Text, comment);
                BuildSqlParameter(sqlCmd, "@CustomerId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new Customer(Convert.ToInt32(sqlCmd.Parameters["@CustomerId"].Value),
                        personId, comment);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Delete an entry from the Customer table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            Customer customer = Get(cn, id);
            if (customer != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM Customer WHERE CustomerId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the Customer table
        /// </summary>
        public static Customer Get(int id)
        {
            Customer result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);
            return result;
        }

        private static Customer Get(SqlConnection cn, int id)
        {
            Customer result = null;

            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Customer WHERE CustomerId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildCustomer(rdr);
                    }
                }
            }
            return result;
        }

        public static Customer GetByPhoneNumber(string phoneNumber)
        {
            Customer result = null;
            PhoneNumber phone = PhoneNumber.Get(phoneNumber);
            if (phone == null)
                return null;
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT Customer.*,Person.* FROM Customer INNER JOIN Person ON CustomerPersonId = PersonId WHERE ((PersonPhoneNumberId1=@PhoneNumberId) OR (PersonPhoneNumberId2=@PhoneNumberId) OR (PersonPhoneNumberId3=@PhoneNumberId) OR (PersonPhoneNumberId4=@PhoneNumberId) OR (PersonPhoneNumberId5=@PhoneNumberId) OR (PersonPhoneNumberId6=@PhoneNumberId))", cn))
            {
                BuildSqlParameter(cmd, "@PhoneNumberId", SqlDbType.Int, phone.Id);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildCustomer(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        public static int GetPersonId(int customerId)
        {
            int personId = 0;

            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT CustomerPersonId FROM Customer WHERE CustomerId=" + customerId, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        try
                        {
                            personId = Convert.ToInt32(rdr[0].ToString());
                        }
                        catch
                        {
                        }
                    }
                }
            }
            FinishedWithConnection(cn);
            return personId;
        }

        /// <summary>
        /// Get all the entries in the Customer table
        /// </summary>
        public static IEnumerable<Customer> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Customer", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildCustomer(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the Customer table
        /// </summary>
        public static bool Update(Customer customer)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, customer);
            FinishedWithConnection(cn);
            return result;
        }

        private static bool Update(SqlConnection cn, Customer customer)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE Customer SET CustomerPersonId=@CustomerPersonId,CustomerComment=@CustomerComment WHERE CustomerId=@CustomerId";

                BuildSqlParameter(sqlCmd, "@CustomerId", SqlDbType.Int, customer.Id);
                BuildSqlParameter(sqlCmd, "@CustomerPersonId", SqlDbType.Int, customer.PersonId);
                BuildSqlParameter(sqlCmd, "@CustomerComment", SqlDbType.Text, customer.Comment);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Build a Customer object from a SqlDataReader object
        /// </summary>
        private static Customer BuildCustomer(SqlDataReader rdr)
        {
            return new Customer(
                GetId(rdr),
                GetPersonId(rdr),
                GetComment(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int GetPersonId(SqlDataReader rdr)
        {
            return rdr.GetInt32(1);
        }

        private static string GetComment(SqlDataReader rdr)
        {
            string comment = null;
            if (!rdr.IsDBNull(2))
            {
                string value = rdr.GetString(2);
                if (!value.Equals(""))
                    comment = value;
            }
            return comment;
        }
        #endregion

    }
}
