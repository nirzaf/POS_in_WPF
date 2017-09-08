using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using PosModels.Helpers;
using System.Data;
using PosModels.Managers;
using PosModels.Types;
using System.Reflection;

namespace PosModels
{
    [ModeledDataClass()]
    public class PhoneNumber : DataModelBase
    {
        #region Licensed Access Only
        static PhoneNumber()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(PhoneNumber).Assembly.GetName().GetPublicKeyToken(),
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

        [ModeledData("Value")]
        public string Number
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataNullable()]
        public string Description
        {
            get;
            private set;
        }

        private PhoneNumber(int id, string number, string description)
        {
            Id = id;
            Number = number;
            Description = description;
        }

        public void SetNumber(string number)
        {
            Number = number;
        }

        public void SetDescription(string text)
        {
            Description = text;
        }

        public string GetFormattedPhoneNumber()
        {
            if (Number.Length != 10)
                return null;
            return "(" + Number.Substring(0, 3) + ") " +
                Number.Substring(3, 3) + "-" + Number.Substring(6);
        }

        public bool Update()
        {
            return PhoneNumber.Update(this);
        }

        #region static
        public static PhoneNumber Add(string phoneNumber, string description)
        {
            PhoneNumber result = null;

            SqlConnection cn = GetConnection();
            string cmd = "AddPhoneNumber";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@PhoneNumberValue", SqlDbType.Text, phoneNumber);
                BuildSqlParameter(sqlCmd, "@PhoneNumberDescription", SqlDbType.Text, description);
                BuildSqlParameter(sqlCmd, "@PhoneNumberId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new PhoneNumber(Convert.ToInt32(sqlCmd.Parameters["@PhoneNumberId"].Value),
                        phoneNumber, description);
                }
            }
            FinishedWithConnection(cn);

            return result;
        }


        /// <summary>
        /// Delete an entry from the PhoneNumber table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            PhoneNumber phoneNumber = Get(cn, id);
            if (phoneNumber != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM PhoneNumber WHERE PhoneNumberId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Finds, in an existing array, a PhoneNumber entry for the specified phone phoneNumber stringValue
        /// </summary>
        public static PhoneNumber Find(PhoneNumber[] numbers, string phoneNumber)
        {
            if (numbers == null)
                return null;
            foreach (PhoneNumber number in numbers)
            {
                if (number.Number.Equals(phoneNumber))
                    return number;
            }
            return null;
        }

        /// <summary>
        /// Get an entry from the PhoneNumber table
        /// </summary>
        public static PhoneNumber Get(int id)
        {
            PhoneNumber result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);

            return result;
        }

        private static PhoneNumber Get(SqlConnection cn, int id)
        {
            PhoneNumber result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM PhoneNumber WHERE PhoneNumberId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildPhoneNumber(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get an entry from the PhoneNumber table
        /// </summary>
        public static PhoneNumber Get(string phoneNumber)
        {
            PhoneNumber result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, phoneNumber);
            FinishedWithConnection(cn);

            return result;
        }

        private static PhoneNumber Get(SqlConnection cn, string phoneNumber)
        {
            PhoneNumber result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM PhoneNumber WHERE PhoneNumberValue LIKE @PhoneNumberValue", cn))
            {
                BuildSqlParameter(cmd, "@PhoneNumberValue", SqlDbType.Text, phoneNumber);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildPhoneNumber(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the PhoneNumber table
        /// </summary>
        public static IEnumerable<PhoneNumber> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM PhoneNumber", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildPhoneNumber(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the PhoneNumber table
        /// </summary>
        public static bool Update(PhoneNumber phoneNumber)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, phoneNumber);
            FinishedWithConnection(cn);

            return result;
        }

        private static bool Update(SqlConnection cn, PhoneNumber phoneNumber)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE PhoneNumber SET PhoneNumberValue=@PhoneNumberValue,PhoneNumberDescription=@PhoneNumberDescription WHERE PhoneNumberId=@PhoneNumberId";

                BuildSqlParameter(sqlCmd, "@PhoneNumberId", SqlDbType.Int, phoneNumber.Id);
                BuildSqlParameter(sqlCmd, "@PhoneNumberValue", SqlDbType.Text, phoneNumber.Number);
                BuildSqlParameter(sqlCmd, "@PhoneNumberDescription", SqlDbType.Text, phoneNumber.Description);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        public static void Refresh(PhoneNumber phoneNumber)
        {
            Refresh(phoneNumber, PhoneNumber.Get(phoneNumber.Id));
        }

        public static void Refresh(PhoneNumber phoneNumber, PhoneNumber tempPhoneNumber)
        {
            if (phoneNumber == null)
                return;
            phoneNumber.Description = tempPhoneNumber.Description;
            phoneNumber.Number = tempPhoneNumber.Number;
        }

        /// <summary>
        /// Build a PhoneNumber object from a SqlDataReader object
        /// </summary>
        private static PhoneNumber BuildPhoneNumber(SqlDataReader rdr)
        {
            return new PhoneNumber(
                GetId(rdr),
                GetNumber(rdr),
                GetDescription(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static string GetNumber(SqlDataReader rdr)
        {
            return rdr.GetString(1);
        }

        private static string GetDescription(SqlDataReader rdr)
        {
            if (rdr.IsDBNull(2))
                return null;
            return rdr.GetString(2);
        }
        #endregion

    }
}
