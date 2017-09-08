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
    public class Discount : DataModelBase
    {
        #region Licensed Access Only
        static Discount()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(Discount).Assembly.GetName().GetPublicKeyToken(),
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

        [ModeledData()]
        public double? Amount
        {
            get;
            private set;
        }

        [ModeledData()]
        public bool AmountIsPercentage
        {
            get;
            private set;
        }

        [ModeledData("RequirePermission")]
        public bool RequiresPermission
        {
            get;
            private set;
        }

        [ModeledData()]
        public bool ApplyToTicketItem
        {
            get;
            private set;
        }

        [ModeledData("LimitCountPerDay")]
        public int CountLimit
        {
            get;
            private set;
        }

        [ModeledData("LimitAmountPerDay")]
        public double AmountLimit
        {
            get;
            private set;
        }

        [ModeledData()]
        public bool IsActive
        {
            get;
            private set;
        }

        [ModeledData()]
        public bool IsDiscontinued
        {
            get;
            private set;
        }

        private Discount(int id, string description, double? amount, bool amountIsPercentage,
            bool requiresPermission, bool applyToTicketItem, int countLimit, double amountLimit,
            bool isActive, bool isDiscontinued)
        {
            Id = id;
            Description = description;
            Amount = amount;
            AmountIsPercentage = amountIsPercentage;
            RequiresPermission = requiresPermission;
            ApplyToTicketItem = applyToTicketItem;
            CountLimit = countLimit;
            AmountLimit = amountLimit;
            IsActive = isActive;
            IsDiscontinued = isDiscontinued;
        }

        public void Discontinue()
        {
            IsActive = false;
            IsDiscontinued = true;
            Update(this);
        }

        public void SetIsActive(bool isActive)
        {
            if (!IsDiscontinued)
                IsActive = isActive;
        }

        public void SetApplyToTicketItem(bool applyToTicketItem)
        {
            ApplyToTicketItem = applyToTicketItem;
        }

        public void SetDescription(string description)
        {
            Description = description;
        }

        public void SetAmount(double? amount)
        {
            Amount = amount;
        }

        public void SetAmountIsPercentage(bool amountIsPercentage)
        {
            AmountIsPercentage = amountIsPercentage;
        }

        public void SetRequiresPermission(bool requiresPermission)
        {
            RequiresPermission = requiresPermission;
        }

        public bool Update()
        {
            return Discount.Update(this);
        }

        #region static
        /// <summary>
        /// Add a new entry to the Discount table
        /// </summary>
        public static Discount Add(string description, double? amount,
            bool amountIsPercentage, bool requiresManager, bool applyToTicketItem,
            int countLimit, double amountLimit)
        {
            Discount result = null;
            bool isActive = true;
            bool isDiscontinued = true;

            SqlConnection cn = GetConnection();
            string cmd = "AddDiscount";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@DiscountDescription", SqlDbType.Text, description);
                BuildSqlParameter(sqlCmd, "@DiscountAmount", SqlDbType.Float, amount);
                BuildSqlParameter(sqlCmd, "@DiscountAmountIsPercentage", SqlDbType.Bit, amountIsPercentage);
                BuildSqlParameter(sqlCmd, "@DiscountRequirePermission", SqlDbType.Bit, requiresManager);
                BuildSqlParameter(sqlCmd, "@DiscountApplyToTicketItem", SqlDbType.Bit, applyToTicketItem);
                BuildSqlParameter(sqlCmd, "@DiscountLimitCountPerDay", SqlDbType.Int, countLimit);
                BuildSqlParameter(sqlCmd, "@DiscountLimitAmountPerDay", SqlDbType.Float, amountLimit);
                BuildSqlParameter(sqlCmd, "@DiscountIsActive", SqlDbType.Bit, isActive);
                BuildSqlParameter(sqlCmd, "@DiscountIsDiscontinued", SqlDbType.Bit, isDiscontinued);
                BuildSqlParameter(sqlCmd, "@DiscountId", SqlDbType.Int, ParameterDirection.ReturnValue);

                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new Discount(Convert.ToInt32(sqlCmd.Parameters["@DiscountId"].Value),
                        description, amount, amountIsPercentage, applyToTicketItem,
                        requiresManager, countLimit, amountLimit, isActive, isDiscontinued);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Get an entry from the Discount table
        /// </summary>
        public static Discount Get(int id)
        {
            Discount result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);
            return result;
        }

        private static Discount Get(SqlConnection cn, int id)
        {
            Discount result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Discount WHERE DiscountId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildDiscount(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the Discount table
        /// </summary>
        public static IEnumerable<Discount> GetAll(bool activeOnly = false)
        {
            SqlConnection cn = GetConnection();
            string cmdText = (activeOnly ?
                "SELECT * FROM Discount WHERE (DiscountIsDiscontinued=0 AND DiscountIsActive=1)" :
                "SELECT * FROM Discount WHERE DiscountIsDiscontinued=0");
            using (SqlCommand cmd = new SqlCommand(cmdText, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildDiscount(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the Discount table
        /// </summary>
        public static bool Update(Discount discount)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, discount);
            FinishedWithConnection(cn);
            return result;
        }

        private static bool Update(SqlConnection cn, Discount discount)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE Discount SET DiscountDescription=@DiscountDescription,DiscountAmount=@DiscountAmount,DiscountAmountIsPercentage=@DiscountAmountIsPercentage,DiscountRequirePermission=@DiscountRequirePermission,DiscountLimitCountPerDay=@DiscountLimitCountPerDay,DiscountLimitAmountPerDay=@DiscountLimitAmountPerDay,DiscountIsActive=@DiscountIsActive,DiscountIsDiscontinued=@DiscountIsDiscontinued WHERE DiscountId=@DiscountId";

                BuildSqlParameter(sqlCmd, "@DiscountId", SqlDbType.Int, discount.Id);
                BuildSqlParameter(sqlCmd, "@DiscountDescription", SqlDbType.Text, discount.Description);
                BuildSqlParameter(sqlCmd, "@DiscountAmount", SqlDbType.Float, discount.Amount);
                BuildSqlParameter(sqlCmd, "@DiscountAmountIsPercentage", SqlDbType.Bit, discount.AmountIsPercentage);
                BuildSqlParameter(sqlCmd, "@DiscountRequirePermission", SqlDbType.Bit, discount.RequiresPermission);
                BuildSqlParameter(sqlCmd, "@DiscountApplyToTicketItem", SqlDbType.Bit, discount.ApplyToTicketItem);
                BuildSqlParameter(sqlCmd, "@DiscountLimitCountPerDay", SqlDbType.Int, discount.CountLimit);
                BuildSqlParameter(sqlCmd, "@DiscountLimitAmountPerDay", SqlDbType.Float, discount.AmountLimit);
                BuildSqlParameter(sqlCmd, "@DiscountIsActive", SqlDbType.Bit, discount.IsActive);
                BuildSqlParameter(sqlCmd, "@DiscountIsDiscontinued", SqlDbType.Bit, discount.IsDiscontinued);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Build a Discount object from a SqlDataReader object
        /// </summary>
        private static Discount BuildDiscount(SqlDataReader rdr)
        {
            return new Discount(
                GetId(rdr),
                GetDescription(rdr),
                GetAmount(rdr),
                GetAmountIsPercentage(rdr),
                GetRequiresPermission(rdr),
                GetApplyToTicketItem(rdr),
                GetCountLimit(rdr),
                GetAmountLimit(rdr),
                GetIsActive(rdr),
                GetIsDiscontinued(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static string GetDescription(SqlDataReader rdr)
        {
            return rdr.GetString(1);
        }

        private static double? GetAmount(SqlDataReader rdr)
        {
            double? amount = null;
            if (!rdr.IsDBNull(2))
                amount = rdr.GetDouble(2);
            return amount;
        }

        private static bool GetAmountIsPercentage(SqlDataReader rdr)
        {
            return rdr.GetBoolean(3);
        }

        private static bool GetRequiresPermission(SqlDataReader rdr)
        {
            return rdr.GetBoolean(4);
        }

        private static bool GetApplyToTicketItem(SqlDataReader rdr)
        {
            return rdr.GetBoolean(5);
        }

        private static int GetCountLimit(SqlDataReader rdr)
        {
            return rdr.GetInt32(6);
        }

        private static double GetAmountLimit(SqlDataReader rdr)
        {
            return rdr.GetDouble(7);
        }

        private static bool GetIsActive(SqlDataReader rdr)
        {
            return rdr.GetBoolean(8);
        }

        private static bool GetIsDiscontinued(SqlDataReader rdr)
        {
            return rdr.GetBoolean(9);
        }
        #endregion

    }
}
