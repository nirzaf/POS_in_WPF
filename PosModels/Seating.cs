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
    public class Seating : DataModelBase, IComparable, IEquatable<Seating>
    {
        #region Licensed Access Only
        static Seating()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(Seating).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosModels.dll");
            }
#endif
        }
        #endregion

        public const int NoSeatingId = 0;

        [ModeledData()]
        public int Id
        {
            get;
            private set;
        }

        [ModeledData()]
        public int RoomId
        {
            get;
            private set;
        }

        [ModeledData()]
        public string Description
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataType("SMALLINT")]
        public int Capacity
        {
            get;
            private set;
        }

        [ModeledData()]
        public bool IsUnused
        {
            get;
            private set;
        }

        private Seating(int id, int roomId, string description, int capacity,
            bool isUnused)
        {
            Id = id;
            RoomId = roomId;
            Description = description;
            Capacity = capacity;
            IsUnused = isUnused;
        }

        public void SetRoomId(int roomId)
        {
            RoomId = roomId;
        }

        public void SetDescription(string description)
        {
            Description = description;
        }

        public void SetCapacity(int capacity)
        {
            Capacity = MathHelper.Clamp(capacity, short.MinValue, short.MaxValue);
        }

        public void SetIsUnused(bool isUnused)
        {
            IsUnused = isUnused;
        }

        public bool Update()
        {
            return Seating.Update(this);
        }

        public int CompareTo(object obj)
        {
            if (obj is Seating)
            {
                Seating seatingObj = obj as Seating;
                return Description.CompareTo(seatingObj.Description);
            }
            throw new InvalidOperationException("Failed to compare two elements in the array");
        }

        public bool Equals(Seating other)
        {
            return (Id == other.Id);
        }

        #region static
        /// <summary>
        /// Add a new entry to the Seating table
        /// </summary>
        public static Seating Add(int roomId, string description, int capacity, bool isUnused)
        {
            Seating result = null;

            SqlConnection cn = GetConnection();
            string cmd = "AddSeating";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@SeatingRoomId", SqlDbType.Int, roomId);
                BuildSqlParameter(sqlCmd, "@SeatingDescription", SqlDbType.Text, description);
                BuildSqlParameter(sqlCmd, "@SeatingCapacity", SqlDbType.SmallInt, capacity);
                BuildSqlParameter(sqlCmd, "@SeatingIsUnused", SqlDbType.Bit, isUnused);
                BuildSqlParameter(sqlCmd, "@SeatingId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new Seating(Convert.ToInt32(sqlCmd.Parameters["@SeatingId"].Value),
                        roomId, description, capacity, isUnused);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Get all the entries in the Seating table
        /// </summary>
        public static IEnumerable<Seating> GetAll(bool? isUnused = null)
        {
            SqlConnection cn = GetConnection();
            SqlCommand cmd = null;
            try
            {
                if (isUnused == null)
                    cmd = new SqlCommand("SELECT * FROM Seating", cn);
                else
                    cmd = new SqlCommand("SELECT * FROM Seating WHERE SeatingIsUnused=" +
                        (isUnused.Value ? "1" : "0"), cn);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildSeating(rdr);
                    }
                }
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
                FinishedWithConnection(cn);
            }
        }

        /// <summary>
        /// Update an entry in the Seating table
        /// </summary>
        public static bool Update(Seating seating)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, seating);
            FinishedWithConnection(cn);

            return result;
        }

        private static bool Update(SqlConnection cn, Seating seating)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE Seating SET SeatingRoomId=@SeatingRoomId,SeatingDescription=@SeatingDescription,SeatingCapacity=@SeatingCapacity,SeatingIsUnused=@SeatingIsUnused WHERE SeatingId=@SeatingId";

                BuildSqlParameter(sqlCmd, "@SeatingId", SqlDbType.Int, seating.Id);
                BuildSqlParameter(sqlCmd, "@SeatingRoomId", SqlDbType.Int, seating.RoomId);
                BuildSqlParameter(sqlCmd, "@SeatingDescription", SqlDbType.Text, seating.Description);
                BuildSqlParameter(sqlCmd, "@SeatingCapacity", SqlDbType.SmallInt, seating.Capacity);
                BuildSqlParameter(sqlCmd, "@SeatingIsUnused", SqlDbType.Bit, seating.IsUnused);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Build a Seating object from a SqlDataReader object
        /// </summary>
        private static Seating BuildSeating(SqlDataReader rdr)
        {
            return new Seating(
                GetId(rdr),
                GetRoomId(rdr),
                GetDescription(rdr),
                GetCapacity(rdr),
                GetIsUnused(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int GetRoomId(SqlDataReader rdr)
        {
            return rdr.GetInt32(1);
        }

        private static string GetDescription(SqlDataReader rdr)
        {
            return rdr.GetString(2);
        }

        private static int GetCapacity(SqlDataReader rdr)
        {
            return rdr.GetInt16(3);
        }

        private static bool GetIsUnused(SqlDataReader rdr)
        {
            return rdr.GetBoolean(4);
        }
        #endregion

    }
}
