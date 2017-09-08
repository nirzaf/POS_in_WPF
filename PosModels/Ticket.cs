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
    [ModeledDataClass]
    public class Ticket : DataModelBase
    {
        #region Licensed Access Only
        static Ticket()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(Ticket).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosModels.dll");
            }
#endif
        }
        #endregion

        [ModeledData("Year", "Id")]
        [ModeledDataType("SMALLINT", "INT")]
        public YearId PrimaryKey
        {
            get;
            private set;
        }

        [ModeledData]
        [ModeledDataType("SMALLINT")]
        public TicketType Type
        {
            get;
            private set;
        }

        [ModeledData]
        public int? OrderId
        {
            get;
            private set;
        }

        [ModeledData]
        public int PartyId
        {
            get;
            private set;
        }

        [ModeledData]
        public int SeatingId
        {
            get;
            private set;
        }

        [ModeledData]
        public int EmployeeId
        {
            get;
            private set;
        }

        [ModeledData]
        public int CustomerId
        {
            get;
            private set;
        }

        /// <summary>
        /// The time the ticket is created
        /// </summary>
        [ModeledData]
        public DateTime CreateTime
        {
            get;
            private set;
        }

        /// <summary>
        /// The time a ticket's order should be start being prepared 
        /// </summary>
        [ModeledData]
        public DateTime? StartTime
        {
            get;
            private set;
        }

        /// <summary>
        /// The time a ticket's order started being prepared.
        /// </summary>
        [ModeledData]
        public DateTime? PrepareTime
        {
            get;
            private set;
        }

        /// <summary>
        /// The time a ticket's order was finished being prepared.
        /// </summary>
        [ModeledData]
        public DateTime? ReadyTime
        {
            get;
            private set;
        }
        /// <summary>
        /// The DateTime the ticket is either cashed out or canceled
        /// </summary>
        [ModeledData]
        public DateTime? CloseTime
        {
            get;
            private set;
        }

        [ModeledData]
        [ModeledDataNullable]
        public string ManagerNote
        {
            get;
            private set;
        }

        [ModeledData]
        public bool IsCanceled
        {
            get;
            private set;
        }

        [ModeledData]
        [ModeledDataType("NVARCHAR")]
        [ModeledDataNullable]
        public string TaxExemptId
        {
            get;
            private set;
        }

        private Ticket(YearId primaryKey, int? orderId, TicketType type, int partyId, int seatingId, int serverId,
            int customerId, DateTime createTime, string taxExemptId)
        {
            PrimaryKey = primaryKey;
            OrderId = orderId;
            Type = type;
            PartyId = partyId;
            SeatingId = seatingId;
            EmployeeId = serverId;
            CustomerId = customerId;
            CreateTime = createTime;
            IsCanceled = false;
            ManagerNote = null;
            StartTime = null;
            PrepareTime = null;
            ReadyTime = null;
            CloseTime = null;
            TaxExemptId = taxExemptId;
            IsBeingModified = false;
        }

        private Ticket(YearId primaryKey, int? orderId, TicketType type, int partyId, int seatingId, int serverId,
            int customerId, DateTime createTime,
            DateTime? startTime, DateTime? prepareTime, DateTime? readyTime, DateTime? closeTime,
            string managerNote, bool isCanceled, string taxExemptId)
            : this(primaryKey, orderId, type, partyId, seatingId, serverId, customerId, createTime, taxExemptId)
        {
            ManagerNote = managerNote;
            IsCanceled = isCanceled;
            StartTime = startTime;
            PrepareTime = prepareTime;
            ReadyTime = readyTime;
            CloseTime = closeTime;
            IsBeingModified = false;
        }

        public bool IsClosed
        {
            get { return (CloseTime != null); }
        }

        public bool IsFutureTimed
        {
            get { return (StartTime != null); }
        }

        public bool IsPrepared
        {
            get { return (PrepareTime != null); }
        }

        private bool HasTicketItemChanges
        {
            get
            {
                return TicketItem.GetAll(PrimaryKey).Any(ticketItem => ticketItem.IsChanged);
            }
        }

        public bool IsToday
        {
            get
            {
                if (!IsClosed)
                    return true;
                if (DayOfOperation.Today == null)
                    return false;
                return (CloseTime > DayOfOperation.Today.StartOfDay);
            }
        }

        public bool IsBeingModified
        {
            get;
            private set;
        }

        /// <summary>
        /// Get the total of all coupons on this ticket.
        /// </summary>
        public double GetCouponTotal()
        {
            double result = 0;

            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = new SqlCommand("GetTicketCouponTotal", cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@TicketYear", SqlDbType.SmallInt, PrimaryKey.Year);
                BuildSqlParameter(sqlCmd, "@TicketId", SqlDbType.Int, PrimaryKey.Id);
                BuildSqlParameter(sqlCmd, "@TicketCouponTotal", SqlDbType.Float, ParameterDirection.Output);
                sqlCmd.ExecuteNonQuery();
                if (sqlCmd.Parameters["@TicketCouponTotal"].Value != DBNull.Value)
                    result = Convert.ToDouble(sqlCmd.Parameters["@TicketCouponTotal"].Value);
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Get the total of all coupons on this ticket.
        /// </summary>
        public double GetDiscountTotal()
        {
            double result = 0;

            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = new SqlCommand("GetTicketDiscountTotal", cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@TicketYear", SqlDbType.SmallInt, PrimaryKey.Year);
                BuildSqlParameter(sqlCmd, "@TicketId", SqlDbType.Int, PrimaryKey.Id);
                BuildSqlParameter(sqlCmd, "@TicketDiscountTotal", SqlDbType.Float, ParameterDirection.Output);
                sqlCmd.ExecuteNonQuery();
                if (sqlCmd.Parameters["@TicketDiscountTotal"].Value != DBNull.Value)
                    result = Convert.ToDouble(sqlCmd.Parameters["@TicketDiscountTotal"].Value);
            }
            FinishedWithConnection(cn);
            return result;
        }

        public int GetNumberOfTicketItems()
        {
            int result = 0;

            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM TicketItem " +
                    "WHERE (TicketItemYear=" + PrimaryKey.Year + " AND " +
                    "TicketItemTicketId=" + PrimaryKey.Id + ")", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = Convert.ToInt32(rdr[0].ToString());
                    }
                }
            }
            FinishedWithConnection(cn);

            return result;
        }

        /// <summary>
        /// Get the total tax for this ticket.
        /// </summary>
        public double GetTax()
        {
            double result = 0;

            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = new SqlCommand("GetTicketTax", cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@TicketYear", SqlDbType.SmallInt, PrimaryKey.Year);
                BuildSqlParameter(sqlCmd, "@TicketId", SqlDbType.Int, PrimaryKey.Id);
                BuildSqlParameter(sqlCmd, "@TicketTax", SqlDbType.Float, ParameterDirection.Output);
                sqlCmd.ExecuteNonQuery();
                if (sqlCmd.Parameters["@TicketTax"].Value != DBNull.Value)
                    result = Convert.ToDouble(sqlCmd.Parameters["@TicketTax"].Value);
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Get the total of all payments made to this ticket.
        /// </summary>
        public double GetPaymentTotal()
        {
            double result = 0;

            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = new SqlCommand("GetTicketPaymentTotal", cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@TicketYear", SqlDbType.SmallInt, PrimaryKey.Year);
                BuildSqlParameter(sqlCmd, "@TicketId", SqlDbType.Int, PrimaryKey.Id);
                BuildSqlParameter(sqlCmd, "@TicketPaymentTotal", SqlDbType.Float, ParameterDirection.Output);
                sqlCmd.ExecuteNonQuery();
                if (sqlCmd.Parameters["@TicketPaymentTotal"].Value != DBNull.Value)
                    result = Convert.ToDouble(sqlCmd.Parameters["@TicketPaymentTotal"].Value);
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Get the ticket subtotal, that's the cost of all ticket items.
        /// </summary>
        public double GetSubTotal()
        {
            double result = 0;

            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = new SqlCommand("GetTicketSubTotal", cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@TicketYear", SqlDbType.SmallInt, PrimaryKey.Year);
                BuildSqlParameter(sqlCmd, "@TicketId", SqlDbType.Int, PrimaryKey.Id);
                BuildSqlParameter(sqlCmd, "@TicketSubTotal", SqlDbType.Float, ParameterDirection.Output);
                sqlCmd.ExecuteNonQuery();
                if (sqlCmd.Parameters["@TicketSubTotal"].Value != DBNull.Value)
                    result = Convert.ToDouble(sqlCmd.Parameters["@TicketSubTotal"].Value);
            }
            FinishedWithConnection(cn);
            return result;
        }


        /// <summary>
        /// Get the ticket balance, that's the cost of all ticket items. Includes taxes.
        /// </summary>
        public double GetTotal()
        {
            return GetSubTotal() + GetTax() - GetDiscountTotal() - GetCouponTotal();
        }

        public bool Exists()
        {
            return Exists(this);
        }

        public bool HasReturnPendingTicketItem()
        {
            return TicketItem.GetAll(PrimaryKey).Any(ticketItem => ticketItem.IsPendingReturn);
        }

        public bool HasChangedTicketItem()
        {
            foreach (TicketItem ticketItem in TicketItem.GetAll(PrimaryKey))
            {
                if (ticketItem.IsChanged || ticketItem.OrderTime == null)
                    return true;
            }
            return false;
        }

        private void SetOrderId()
        {
            if (OrderId != null)
                return;
            int highestId = 0;
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand(
                "SELECT TOP 1 * FROM Ticket " +
                "WHERE (TicketPrepareTime > @TicketSearchStartTime) " +
                "ORDER BY TicketOrderId DESC", cn))
            {
                BuildSqlParameter(cmd, "@TicketSearchStartTime",
                    SqlDbType.DateTime, DayOfOperation.Today.StartOfDay);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        Ticket ticket = BuildTicket(rdr);
                        if (ticket.OrderId != null)
                            highestId = ticket.OrderId.Value;
                    }
                }
            }
            FinishedWithConnection(cn);
            OrderId = highestId + 1;
        }

        public void SetType(TicketType type)
        {
            Type = type;
        }

        public void SetPartyId(int id)
        {
            PartyId = id;
        }

        public void SetSeatingId(int seatingId)
        {
            SeatingId = seatingId;
        }

        public void SetEmployeeId(int id)
        {
            EmployeeId = id;
        }

        public void SetCustomerId(int id)
        {
            CustomerId = id;
        }

        public void SetTaxExemptId(string taxExemptId)
        {
            // max length of 50
            if ((taxExemptId != null) && (taxExemptId.Length > 50))
                taxExemptId = taxExemptId.Substring(0, 50);
            TaxExemptId = taxExemptId;
        }

        public void SetStartTime(DateTime? startTime)
        {
            StartTime = startTime;
        }

        public void SetPrepareTime(DateTime? prepareTime)
        {
            PrepareTime = prepareTime;
        }

        public void SetReadyTime(DateTime? readyTime)
        {
            ReadyTime = readyTime;
        }

        public void SetCloseTime(DateTime? closetime)
        {
            CloseTime = closetime;
        }

        public void SetManagerNote(string managerNote)
        {
            ManagerNote = managerNote;
        }

        public void SetIsCanceled(bool isCanceled)
        {
            IsCanceled = isCanceled;
        }

        public void SetIsBeingModified()
        {
            IsBeingModified = true;
        }

        public bool Update()
        {
            if (!IsClosed && !IsCanceled && (PrepareTime != null) && (OrderId == null))
                SetOrderId();
            return Update(this);
        }

        public void Cancel(int employeeId, bool wasteTicket)
        {
            // For each TicketItem, Cancel()
            foreach (TicketItem item in TicketItem.GetAll(PrimaryKey))
            {
                if (!item.CancelType.HasValue)
                    item.Cancel(CancelType.TicketCancel, employeeId, wasteTicket);
            }

            // Note: The actual ticket does not get deleted on a cancel
            SetCloseTime(DateTime.Now);
            SetIsCanceled(true);
            Update();
        }

        public void UnCancel()
        {
            // For each TicketItem, uncanel
            foreach (TicketItem item in TicketItem.GetAll(PrimaryKey))
            {
                if (item.CancelType.HasValue && (item.CancelType.Value == CancelType.TicketCancel))
                {
                    item.UnCancel();
                }
            }

            SetCloseTime(null);
            SetIsCanceled(false);
            Update();
        }

        #region static
        /// <summary>
        /// Adds a new entry to the Ticket table
        /// </summary>
        public static Ticket Add(TicketType type, int partyId, int seatingId,
            int employeeId, int customerId)
        {
            Ticket result = null;
            DateTime now = DateTime.Now;
            short year = DayOfOperation.CurrentYear;

            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = new SqlCommand("AddTicket", cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@TicketYear", SqlDbType.SmallInt, year);
                BuildSqlParameter(sqlCmd, "@TicketType", SqlDbType.SmallInt, type);
                BuildSqlParameter(sqlCmd, "@TicketPartyId", SqlDbType.Int, partyId);
                BuildSqlParameter(sqlCmd, "@TicketSeatingId", SqlDbType.Int, seatingId);
                BuildSqlParameter(sqlCmd, "@TicketEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(sqlCmd, "@TicketCustomerId", SqlDbType.Int, customerId);
                BuildSqlParameter(sqlCmd, "@TicketCreateTime", SqlDbType.DateTime, now);
                BuildSqlParameter(sqlCmd, "@TicketId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new Ticket(YearId.Create(year, Convert.ToInt32(sqlCmd.Parameters["@TicketId"].Value)),
                        null, type, partyId, seatingId, employeeId, customerId, now, null) { IsBeingModified = true };
                }
            }
            FinishedWithConnection(cn);

            return result;
        }

        public static void ResetAutoIdentity()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = new SqlCommand("DBCC CHECKIDENT (Ticket,RESEED,0)", cn))
            {
                sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
        }

        public static bool YearHasTickets(short year)
        {
            bool foundTicket = false;
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 TicketYear FROM Ticket WHERE TicketYear=" + year, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        if (!rdr.IsDBNull(0))
                            foundTicket = true;
                    }
                }
            }
            FinishedWithConnection(cn);
            return foundTicket;
        }

        /// <summary>
        /// Delete an entry from the Ticket table
        /// </summary>
        public static bool Delete(YearId primaryKey)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            Ticket ticket = Get(cn, primaryKey);
            if (ticket != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM Ticket WHERE (TicketYear=" + primaryKey.Year +
                        " AND TicketId=" + primaryKey.Id + ")";
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Returns true if the spcified ticket instance exists in the database
        /// </summary>
        public static bool Exists(Ticket ticket)
        {
            bool exists = false;
            if (ticket.PrimaryKey.Id <= 0)
                return false;

            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Ticket WHERE (TicketYear=" +
                    ticket.PrimaryKey.Year + " AND TicketId=" +
                    ticket.PrimaryKey.Id + ")", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        exists = true;
                    }
                }
            }
            FinishedWithConnection(cn);

            return exists;
        }

        public static IEnumerable<Ticket> GetAll(short? year = null)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Ticket" +
                    ((year == null) ? "" : " WHERE TicketYear=" + year), cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTicket(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get all the tickets that have the specified partyId
        /// </summary>
        /// <param name="partyId"></param>
        /// <returns></returns>
        public static IEnumerable<Ticket> GetAll(int partyId)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Ticket WHERE TicketPartyId=" + partyId, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTicket(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get all the tickets for the specified DateTime range
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static IEnumerable<Ticket> GetAll(DateTime startDate, DateTime endDate)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Ticket WHERE (((TicketStartTime >= @TicketSearchStartTime AND TicketStartTime <= @TicketSearchEndTime) OR " +
                    "(TicketPrepareTime >= @TicketSearchStartTime AND TicketPrepareTime <= @TicketSearchEndTime)) AND TicketIsCanceled = 0)", cn))
            {
                BuildSqlParameter(cmd, "@TicketSearchStartTime", SqlDbType.DateTime, startDate);
                BuildSqlParameter(cmd, "@TicketSearchEndTime", SqlDbType.DateTime, endDate);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTicket(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static int GetTotalNumberOfTickets(short year)
        {
            int result = 0;

            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Ticket" +
                    " WHERE TicketYear=" + year, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = Convert.ToInt32(rdr[0].ToString());
                    }
                }
            }
            FinishedWithConnection(cn);

            return result;
        }

        public static IEnumerable<Ticket> GetAllStartingAt(int startingTicketId, short? year = null)
        {
            SqlConnection cn = GetConnection();
            using (var cmd = new SqlCommand("SELECT * FROM Ticket WHERE (TicketYear=" +
                    ((year != null) ? year.Value : DayOfOperation.CurrentYear) + " AND TicketId>=" +
                    startingTicketId + ")", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTicket(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static IEnumerable<Ticket> GetAllActive()
        {
            SqlConnection cn = GetConnection();
            using (var cmd = new SqlCommand("SELECT * FROM Ticket " +
                "WHERE ((TicketCloseTime IS NULL) AND TicketIsCanceled=@TicketIsCanceled)", cn))
            {
                BuildSqlParameter(cmd, "@TicketIsCanceled", SqlDbType.Bit, false);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    DateTime now = DateTime.Now;
                    while (rdr.Read())
                    {
                        Ticket ticket = BuildTicket(rdr);
                        if ((ticket.PrepareTime == null) && (ticket.StartTime != null) &&
                            (ticket.StartTime > now))
                            continue; // Skip future orders
                        yield return ticket;
                    }
                }                
            }
            FinishedWithConnection(cn);
        }

        public static IEnumerable<Ticket> GetAllActive(int employeeId)
        {
            SqlConnection cn = GetConnection();
            using (var cmd = new SqlCommand("SELECT * FROM Ticket " +
                "WHERE ((TicketCloseTime IS NULL) AND TicketIsCanceled=@TicketIsCanceled AND TicketEmployeeId=@TicketEmployeeId)", cn))
            {
                BuildSqlParameter(cmd, "@TicketIsCanceled", SqlDbType.Bit, false);
                BuildSqlParameter(cmd, "@TicketEmployeeId", SqlDbType.Int, employeeId);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTicket(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static Ticket GetSuspendedTicket(int employeeId)
        {
            Ticket result = null;
            SqlConnection cn = GetConnection();
            using (var cmd = new SqlCommand("SELECT * FROM Ticket WHERE ((TicketPrepareTime IS NULL) AND (TicketStartTime IS NULL) AND TicketEmployeeId=@TicketEmployeeId)", cn))
            {
                BuildSqlParameter(cmd, "@TicketEmployeeId", SqlDbType.Int, employeeId);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildTicket(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        public static Ticket Get(YearId primaryKey)
        {
            SqlConnection cn = GetConnection();
            Ticket result = Get(cn, primaryKey);
            FinishedWithConnection(cn);

            return result;
        }

        private static Ticket Get(SqlConnection cn, YearId primaryKey)
        {
            Ticket result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Ticket WHERE (TicketYear=" +
                primaryKey.Year + " AND TicketId=" + primaryKey.Id + ")", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildTicket(rdr);
                    }
                }
            }
            return result;
        }

        public static TicketType? GetTicketType(YearId ticketPrimaryKey)
        {
            TicketType? ticketType = null;
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT TicketType FROM Ticket " +
                "WHERE (TicketYear=" + ticketPrimaryKey.Year + " AND TicketId=" + ticketPrimaryKey.Id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        try
                        {
                            ticketType = (TicketType)Convert.ToInt16(rdr[0].ToString());
                        }
                        catch
                        {
                            ticketType = null;
                        }
                    }
                }
            }
            FinishedWithConnection(cn);
            return ticketType;
        }

        public static void Refresh(Ticket ticket)
        {
            Refresh(ticket, Get(ticket.PrimaryKey));
        }

        public static void Refresh(Ticket ticket, Ticket tempTicket)
        {
            if ((ticket == null) || ticket.HasTicketItemChanges || ticket.IsBeingModified)
                return;
            ticket.Type = tempTicket.Type;
            ticket.OrderId = tempTicket.OrderId;
            ticket.PartyId = tempTicket.PartyId;
            ticket.SeatingId = tempTicket.SeatingId;
            ticket.EmployeeId = tempTicket.EmployeeId;
            ticket.CustomerId = tempTicket.CustomerId;
            ticket.StartTime = tempTicket.StartTime;
            ticket.PrepareTime = tempTicket.PrepareTime;
            ticket.ReadyTime = tempTicket.ReadyTime;
            ticket.CloseTime = tempTicket.CloseTime;
            ticket.IsCanceled = tempTicket.IsCanceled;
            // Don't because this is not in the model, and indicates
            // if a ticket is being modified
            //ticket.IsBeingModified = tempTicket.IsBeingModified;
            ticket.ManagerNote = tempTicket.ManagerNote;
            ticket.TaxExemptId = tempTicket.TaxExemptId;
        }

        public static bool Update(Ticket ticket)
        {
            ticket.IsBeingModified = false;
            SqlConnection cn = GetConnection();
            bool result = Update(cn, ticket);
            FinishedWithConnection(cn);
            return result;
        }

        private static bool Update(SqlConnection cn, Ticket ticket)
        {
            Int32 rowsAffected;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE Ticket SET TicketOrderId=@TicketOrderId,TicketType=@TicketType,TicketPartyId=@TicketPartyId,TicketSeatingId=@TicketSeatingId,TicketEmployeeId=@TicketEmployeeId,TicketStartTime=@TicketStartTime,TicketPrepareTime=@TicketPrepareTime,TicketReadyTime=@TicketReadyTime,TicketCloseTime=@TicketCloseTime,TicketManagerNote=@TicketManagerNote,TicketCustomerId=@TicketCustomerId,TicketIsCanceled=@TicketIsCanceled,TicketTaxExemptId=@TicketTaxExemptId WHERE (TicketYear=@TicketYear AND TicketId=@TicketId)";

                BuildSqlParameter(sqlCmd, "@TicketYear", SqlDbType.SmallInt, ticket.PrimaryKey.Year);
                BuildSqlParameter(sqlCmd, "@TicketId", SqlDbType.Int, ticket.PrimaryKey.Id);
                BuildSqlParameter(sqlCmd, "@TicketOrderId", SqlDbType.Int, ticket.OrderId);
                BuildSqlParameter(sqlCmd, "@TicketType", SqlDbType.SmallInt, ticket.Type);
                BuildSqlParameter(sqlCmd, "@TicketPartyId", SqlDbType.Int, ticket.PartyId);
                BuildSqlParameter(sqlCmd, "@TicketSeatingId", SqlDbType.Int, ticket.SeatingId);
                BuildSqlParameter(sqlCmd, "@TicketEmployeeId", SqlDbType.Int, ticket.EmployeeId);
                BuildSqlParameter(sqlCmd, "@TicketCustomerId", SqlDbType.Int, ticket.CustomerId);
                BuildSqlParameter(sqlCmd, "@TicketStartTime", SqlDbType.DateTime, ticket.StartTime);
                BuildSqlParameter(sqlCmd, "@TicketPrepareTime", SqlDbType.DateTime, ticket.PrepareTime);
                BuildSqlParameter(sqlCmd, "@TicketReadyTime", SqlDbType.DateTime, ticket.ReadyTime);
                BuildSqlParameter(sqlCmd, "@TicketCloseTime", SqlDbType.DateTime, ticket.CloseTime);
                BuildSqlParameter(sqlCmd, "@TicketManagerNote", SqlDbType.Text, ticket.ManagerNote);
                BuildSqlParameter(sqlCmd, "@TicketIsCanceled", SqlDbType.Bit, ticket.IsCanceled);
                BuildSqlParameter(sqlCmd, "@TicketTaxExemptId", SqlDbType.NVarChar, ticket.TaxExemptId);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        public static int GetHighestTicketId(short year)
        {
            Ticket highestTicket = null;
            int highestId = 0;
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd =
                    new SqlCommand("SELECT TOP 1 * FROM Ticket WHERE (TicketYear=" +
                        year + ") ORDER BY TicketId DESC", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        highestTicket = BuildTicket(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
            if (highestTicket != null)
                highestId = highestTicket.PrimaryKey.Id;
            return highestId;
        }

        public static void Reset(Type type)
        {
            if (type != typeof(Ticket))
                throw new Exception("Bad use of reset");
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM Ticket WHERE TicketId>0";
                sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            ResetAutoIdentity();
        }

        private static Ticket BuildTicket(SqlDataReader rdr)
        {
            short year = GetYear(rdr);
            int id = GetId(rdr);
            return new Ticket(YearId.Create(year, id),
                GetOrderId(rdr),
                GetTicketType(rdr),
                GetPartyId(rdr),
                GetSeatingId(rdr),
                GetEmployeeId(rdr),
                GetCustomerId(rdr),
                GetCreatedTime(rdr),
                GetStartTime(rdr),
                GetPrepareTime(rdr),
                GetReadyTime(rdr),
                GetClosedTime(rdr),
                GetManagerNote(rdr),
                GetIsCanceled(rdr),
                GetTaxExemptId(rdr));
        }

        private static short GetYear(SqlDataReader rdr)
        {
            return rdr.GetInt16(0);
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(1);
        }

        private static int? GetOrderId(SqlDataReader rdr)
        {
            if (rdr.IsDBNull(2))
                return null;
            return rdr.GetInt32(2);
        }

        private static TicketType GetTicketType(SqlDataReader rdr)
        {
            return rdr.GetInt16(3).GetTicketType();
        }

        private static int GetPartyId(SqlDataReader rdr)
        {
            return rdr.GetInt32(4);
        }

        private static int GetSeatingId(SqlDataReader rdr)
        {
            return rdr.GetInt32(5);
        }

        public static int GetEmployeeId(SqlDataReader rdr)
        {
            return rdr.GetInt32(6);
        }

        private static int GetCustomerId(SqlDataReader rdr)
        {
            return rdr.GetInt32(7);
        }

        private static DateTime GetCreatedTime(SqlDataReader rdr)
        {
            return rdr.GetDateTime(8);
        }

        private static DateTime? GetStartTime(SqlDataReader rdr)
        {
            DateTime? startTime = null;
            if (!rdr.IsDBNull(9))
                startTime = rdr.GetDateTime(9);
            return startTime;
        }

        private static DateTime? GetPrepareTime(SqlDataReader rdr)
        {
            DateTime? prepareTime = null;
            if (!rdr.IsDBNull(10))
                prepareTime = rdr.GetDateTime(10);
            return prepareTime;
        }

        private static DateTime? GetReadyTime(SqlDataReader rdr)
        {
            DateTime? readyTime = null;
            if (!rdr.IsDBNull(11))
                readyTime = rdr.GetDateTime(11);
            return readyTime;
        }

        private static DateTime? GetClosedTime(SqlDataReader rdr)
        {
            DateTime? closedTime = null;
            if (!rdr.IsDBNull(12))
                closedTime = rdr.GetDateTime(12);
            return closedTime;
        }

        private static string GetManagerNote(SqlDataReader rdr)
        {
            string managerNote = null;
            if (!rdr.IsDBNull(13))
            {
                string value = rdr.GetString(13);
                if (!value.Equals(""))
                    managerNote = value;
            }
            return managerNote;
        }

        private static bool GetIsCanceled(SqlDataReader rdr)
        {
            return rdr.GetBoolean(14);
        }

        private static string GetTaxExemptId(SqlDataReader rdr)
        {
            string taxExemptId = null;
            if (!rdr.IsDBNull(15))
            {
                string value = rdr.GetString(15);
                if (!value.Equals(""))
                    taxExemptId = value;
            }
            return taxExemptId;
        }
        #endregion

    }
}
