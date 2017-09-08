using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using PosModels.Helpers;
using PosModels.Managers;
using PosModels.Types;
using TemposLibrary;

namespace PosModels
{
    [ModeledDataClass()]
    public class SerialObject : DataModelBase
    {
        #region Licensed Access Only
        static SerialObject()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(DirectDepositTransaction).Assembly.GetName().GetPublicKeyToken(),
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
        public string Name
        {
            get;
            private set;
        }

        [ModeledData("Data")]
        [ModeledDataType("VARBINARY")]
        public byte[] SerialData
        {
            get;
            private set;
        }

        private SerialObject(int id, string name, byte[] serialData)
        {
            Id = id;
            Name = name;
            SerialData = serialData;
        }

        public object GetDeserializedObject()
        {
            return SerialData.DeserializeObject();
        }

        public void SetSerializedObject(object serializableObject)
        {
            SerialData = serializableObject.SerializeObject();            
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public bool Update()
        {
            return Update(this);
        }

        public static SerialObject Add(string name, object serializableObject)
        {
            byte[] serialData = serializableObject.SerializeObject();
            SerialObject result = null;

            string query = "INSERT INTO SerialObject (SerialObjectName, SerialObjectData) " +
                "VALUES (@SerialObjectName, @SerialObjectData);" +
                "SELECT CAST(scope_identity() AS int)";
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                BuildSqlParameter(cmd, "@SerialObjectName", SqlDbType.Text, name);
                BuildSqlParameter(cmd, "@SerialObjectData", SqlDbType.VarBinary, serialData);
                int id = (Int32)cmd.ExecuteScalar();
                if (id > 0)
                {
                    result = new SerialObject(id, name, serialData);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        public static bool Exists(string name)
        {
            int count = 0;

            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM SerialObject WHERE SerialObjectName LIKE @SerialObjectName", cn))
            {
                BuildSqlParameter(cmd, "@SerialObjectName", SqlDbType.Text, name);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        try
                        {
                            count = Convert.ToInt32(rdr[0].ToString());
                        }
                        catch
                        {
                        }
                    }
                }
            }
            FinishedWithConnection(cn);

            return (count == 0);
        }
        
        public static SerialObject Get(string settingName)
        {
            SerialObject result = null;
            SqlConnection cn = GetConnection();
            result = Get(cn, settingName);
            FinishedWithConnection(cn);
            return result;
        }

        private static SerialObject Get(SqlConnection cn, string settingName)
        {
            SerialObject result = null;
            if (settingName == null)
                return null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM SerialObject WHERE SerialObjectName LIKE @SerialObjectName", cn))
            {
                BuildSqlParameter(cmd, "@SerialObjectName", SqlDbType.Text, settingName);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildSerialObject(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Update an entry in the Product table
        /// </summary>
        public static bool Update(SerialObject product)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, product);
            FinishedWithConnection(cn);

            return result;
        }

        private static bool Update(SqlConnection cn, SerialObject serialObject)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE SerialObject SET SerialObjectName=@SerialObjectName,SerialObjectData=@SerialObjectData WHERE SerialObjectId=@SerialObjectId";

                BuildSqlParameter(sqlCmd, "@SerialObjectId", SqlDbType.Int, serialObject.Id);
                BuildSqlParameter(sqlCmd, "@SerialObjectName", SqlDbType.Text, serialObject.Name);
                BuildSqlParameter(sqlCmd, "@SerialObjectData", SqlDbType.VarBinary, serialObject.SerialData);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Delete an entry from the Product table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM SerialObject WHERE SerialObjectId=" + id;
                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Delete an entry from the Product table
        /// </summary>
        public static bool Delete(string settingName)
        {
            Int32 rowsAffected = 0;
            SerialObject serialObject = Get(settingName);
            if (serialObject != null)
            {
                SqlConnection cn = GetConnection();
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM SerialObject WHERE SerialObjectId=" + serialObject.Id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
                FinishedWithConnection(cn);
            }
            return (rowsAffected != 0);
        }

        private static SerialObject BuildSerialObject(SqlDataReader rdr)
        {
            int id = rdr.GetInt32(0);
            string name = rdr.GetString(1);
            SqlBytes serialData = rdr.GetSqlBytes(2);
            byte[] serialBytes = (byte[])serialData.Buffer.Clone();
            return new SerialObject(id, name, serialBytes);
        }

    }
}
