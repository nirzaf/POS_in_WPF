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
    public class Room : DataModelBase
    {
        #region Licensed Access Only
        static Room()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(Room).Assembly.GetName().GetPublicKeyToken(),
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
        public string Description
        {
            get;
            private set;
        }

        [ModeledData("TicketType")]
        [ModeledDataType("SMALLINT")]
        public TicketType TicketingType
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

        private Room(int id, string description, TicketType ticketingType,
            bool isUnused)
        {
            Id = id;
            Description = description;
            TicketingType = ticketingType;
            IsUnused = isUnused;
        }

        public void SetDescription(string description)
        {
            Description = description;
        }

        public void SetTicketingType(TicketType ticketingType)
        {
            TicketingType = ticketingType;
        }

        public void SetIsUnused(bool isUnused)
        {
            IsUnused = isUnused;
        }

        public bool Update()
        {
            return Room.Update(this);
        }

        #region static
        public static Room Add(string description, TicketType ticketType, bool isUnused)
        {
            Room result = null;

            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = new SqlCommand("AddRoom", cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@RoomDescription", SqlDbType.Text, description);
                BuildSqlParameter(sqlCmd, "@RoomTicketType", SqlDbType.SmallInt, ticketType);
                BuildSqlParameter(sqlCmd, "@RoomIsUnused", SqlDbType.Bit, isUnused);
                BuildSqlParameter(sqlCmd, "@RoomId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new Room(Convert.ToInt32(sqlCmd.Parameters["@RoomId"].Value),
                        description, ticketType, isUnused);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Room> GetAll(bool? isUnused = null)
        {
            SqlConnection cn = GetConnection();
            SqlCommand cmd = null;
            try
            {
                if (isUnused == null)
                    cmd = new SqlCommand("SELECT * FROM Room", cn);
                else
                    cmd = new SqlCommand("SELECT * FROM Room WHERE RoomIsUnused=" +
                        (isUnused.Value ? "1" : "0"), cn);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildRoom(rdr);
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

        public static bool Update(Room room)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, room);
            FinishedWithConnection(cn);

            return result;
        }

        private static bool Update(SqlConnection cn, Room room)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE Room SET RoomDescription=@RoomDescription,RoomTicketType=@RoomTicketType,RoomIsUnused=@RoomIsUnused WHERE RoomId=@RoomId";

                BuildSqlParameter(sqlCmd, "@RoomId", SqlDbType.Int, room.Id);
                BuildSqlParameter(sqlCmd, "@RoomDescription", SqlDbType.Text, room.Description);
                BuildSqlParameter(sqlCmd, "@RoomTicketType", SqlDbType.SmallInt, room.TicketingType);
                BuildSqlParameter(sqlCmd, "@RoomIsUnused", SqlDbType.Bit, room.IsUnused);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        private static Room BuildRoom(SqlDataReader rdr)
        {
            return new Room(
                GetId(rdr),
                GetRoomName(rdr),
                GetOrderType(rdr),
                GetIsUnused(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static string GetRoomName(SqlDataReader rdr)
        {
            return rdr.GetString(1);
        }

        private static TicketType GetOrderType(SqlDataReader rdr)
        {
            return rdr.GetInt16(2).GetTicketType();
        }

        private static bool GetIsUnused(SqlDataReader rdr)
        {
            return rdr.GetBoolean(3);
        }
        #endregion

    }
}
