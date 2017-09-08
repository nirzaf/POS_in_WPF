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
    public class DayOfOperation : DataModelBase
    {
        #region Licensed Access Only / Static Initializer
        static DayOfOperation()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(DayOfOperation).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosModels.dll");
            }
#endif
            SetToday();
        }
        #endregion

        [ModeledData()]
        public int Id
        {
            get;
            private set;
        }

        [ModeledData("StartOfDayWhen")]
        public DateTime StartOfDay
        {
            get;
            private set;
        }

        [ModeledData("EndOfDayWhen")]
        public DateTime? EndOfDay
        {
            get;
            private set;
        }

        [ModeledData()]
        public int? EndOfDayEmployeeId
        {
            get;
            private set;
        }

        [ModeledData()]
        public int? LastTicketId
        {
            get;
            private set;
        }

        private DayOfOperation(int id, DateTime startOfDay, DateTime? endOfDay,
            int? endOfDayEmployeeId, int? lastTicketId)
        {
            Id = id;
            StartOfDay = startOfDay;
            EndOfDay = endOfDay;
            EndOfDayEmployeeId = endOfDayEmployeeId;
            LastTicketId = lastTicketId;
        }

        private static void SetToday()
        {
            DayOfOperation highest = Get(GetHighestDayOfOperationId());
            if (highest != null)
                Today = ((highest.EndOfDay == null) ? highest : null);
            else
                Today = null;
        }

        public static void SetTodayNull()
        {
            Today = null;
        }

        private static DayOfOperation todayValue;

        public static DayOfOperation Today
        {
            get
            {
                return todayValue;
            }
            private set
            {
                todayValue = value;
            }
        }

        public static short CurrentYear
        {
            get
            {
                if (Today == null)
                    return YearOfLastStartOfDay;
                return (short)Today.StartOfDay.Year;
            }
        }

        public static short YearOfLastStartOfDay
        {
            get
            {
                int id = GetHighestDayOfOperationId();
                DayOfOperation lastStartOfDay = Get(id);
                if (lastStartOfDay == null)
                    return YearOfNextStartOfDay;
                return (short)lastStartOfDay.StartOfDay.Year;
            }
        }

        public static short YearOfNextStartOfDay
        {
            get
            {
                return (short)DateTime.Now.Year;
            }
        }

        public static bool WaitingForEndOfYear
        {
            get
            {
                bool yearChanged = (DayOfOperation.YearOfNextStartOfDay > DayOfOperation.YearOfLastStartOfDay);
                if (!yearChanged)
                    return false;
                int? ticketOffsetId = SettingManager.GetStoreSetting("DailyIdOffset").IntValue;
                if (ticketOffsetId == null)
                    return false;
                return (ticketOffsetId.Value != 0);
            }
        }

        private static void Add(DateTime startOfDay)
        {
            string query = "INSERT INTO DayOfOperation (DayOfOperationId, DayOfOperationStartOfDayWhen) " +
                "VALUES (@DayOfOperationId, @DayOfOperationStartOfDayWhen);";
            SqlConnection conn = GetConnection();
            int count = 0;
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM DayOfOperation", conn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        count = Convert.ToInt32(rdr[0].ToString());
                    }
                }
            }
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                BuildSqlParameter(cmd, "@DayOfOperationId", SqlDbType.Int, count + 1);
                BuildSqlParameter(cmd, "@DayOfOperationStartOfDayWhen", SqlDbType.DateTime, startOfDay);
                cmd.ExecuteScalar();
            }
            FinishedWithConnection(conn);
        }

        public static DayOfOperation GetEarliestInYear(short year)
        {
            DayOfOperation result = null;
            SqlConnection conn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 * FROM DayOfOperation WHERE DayOfOperationStartOfDayWhen>=@FirstDayOfYear ORDER BY DayOfOperationStartOfDayWhen", conn))
            {
                BuildSqlParameter(cmd, "@FirstDayOfYear", SqlDbType.DateTime, new DateTime(year, 1, 1, 0, 0, 0));
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildDayOfOperation(rdr);
                    }
                }
            }
            FinishedWithConnection(conn);
            return result;
        }

        public static DayOfOperation GetLatestInYear(short year)
        {
            DayOfOperation result = null;
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 * FROM DayOfOperation WHERE DayOfOperationStartOfDayWhen<=@LastDayOfYear ORDER BY DayOfOperationStartOfDayWhen DESC", cn))
            {
                BuildSqlParameter(cmd, "@LastDayOfYear", SqlDbType.DateTime, new DateTime(year, 12, 31, 23, 59, 59));
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    result = BuildDayOfOperation(rdr);
                }
                rdr.Close();
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Counts the number of entries in the Product table
        /// </summary>
        public static int Count()
        {
            int count = 0;
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM DayOfOperation", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        count = Convert.ToInt32(rdr[0].ToString());
                    }
                }
            }
            FinishedWithConnection(cn);
            return count;
        }

        public static DayOfOperation Get(int id)
        {
            DayOfOperation result = null;
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM DayOfOperation WHERE DayOfOperationId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildDayOfOperation(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        private static int GetHighestDayOfOperationId()
        {
            DayOfOperation highestDayOfOperation = null;
            int highestId = 0;
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd =
                    new SqlCommand("SELECT TOP 1 * FROM DayOfOperation ORDER BY DayOfOperationId DESC", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        highestDayOfOperation = BuildDayOfOperation(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
            if (highestDayOfOperation != null)
                highestId = highestDayOfOperation.Id;
            return highestId;
        }

        public static void ProcessStartOfDay()
        {
            Add(DateTime.Now);
            SetToday();
        }

        public static bool ProcessEndOfDay(DayOfOperation today, int endOfDayEmployeeId, int lastTicketId)
        {            
            Int32 rowsAffected = 0;
            if (today == null)
                return false;
            // Update model
            today.EndOfDay = DateTime.Now;
            today.EndOfDayEmployeeId = endOfDayEmployeeId;
            today.LastTicketId = lastTicketId;
            // Update database
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE DayOfOperation SET DayOfOperationEndOfDayWhen=@DayOfOperationEndOfDayWhen,DayOfOperationEndOfDayEmployeeId=@DayOfOperationEndOfDayEmployeeId,DayOfOperationLastTicketId=@DayOfOperationLastTicketId WHERE DayOfOperationId=@DayOfOperationId";

                BuildSqlParameter(sqlCmd, "@DayOfOperationId", SqlDbType.Int, today.Id);
                BuildSqlParameter(sqlCmd, "@DayOfOperationEndOfDayWhen", SqlDbType.DateTime, today.EndOfDay);
                BuildSqlParameter(sqlCmd, "@DayOfOperationEndOfDayEmployeeId", SqlDbType.Int, today.EndOfDayEmployeeId);
                BuildSqlParameter(sqlCmd, "@DayOfOperationLastTicketId", SqlDbType.Int, today.LastTicketId);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        public static void Reset(Type type)
        {
            if (type != typeof(DayOfOperation))
                throw new Exception("Bad use of reset");
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM DayOfOperation WHERE DayOfOperationId>0";
                sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
        }

        private static DayOfOperation BuildDayOfOperation(SqlDataReader rdr)
        {
            return new DayOfOperation(
                GetId(rdr),
                GetStartOfDay(rdr),
                GetEndOfDay(rdr),
                GetEndOfDayEmployeeId(rdr),
                GetLastTicketId(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static DateTime GetStartOfDay(SqlDataReader rdr)
        {
            return rdr.GetDateTime(1);
        }

        private static DateTime? GetEndOfDay(SqlDataReader rdr)
        {
            DateTime? endOfDay = null;
            if (!rdr.IsDBNull(2))
                endOfDay = rdr.GetDateTime(2);
            return endOfDay;
        }

        private static int? GetEndOfDayEmployeeId(SqlDataReader rdr)
        {
            int? endOfDayEmployeeId = null;
            if (!rdr.IsDBNull(3))
                endOfDayEmployeeId = rdr.GetInt32(3);
            return endOfDayEmployeeId;
        }

        private static int? GetLastTicketId(SqlDataReader rdr)
        {
            int? lastTicketId = null;
            if (!rdr.IsDBNull(4))
                lastTicketId = rdr.GetInt32(4);
            return lastTicketId;
        }
    }
}
