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
    public class Coupon : DataModelBase
    {
        #region Licensed Access Only
        static Coupon()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(Coupon).Assembly.GetName().GetPublicKeyToken(),
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
        public double Amount
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

        [ModeledData("Active")]
        public bool IsActive
        {
            get;
            private set;
        }

        [ModeledData()]
        public bool IsExclusive
        {
            get;
            private set;
        }

        [ModeledData()]
        public bool ApplyToAll
        {
            get;
            private set;
        }

        [ModeledData()]
        public bool ThirdPartyCompensation
        {
            get;
            private set;
        }

        [ModeledData()]
        public double? AmountLimit
        {
            get;
            private set;
        }

        [ModeledData()]
        public int? LimitPerTicket
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

        private Coupon(int id, string description, double amount, bool amountIsPercentage,
            bool isActive, bool isExclusive, bool applyToAll, bool thirdPartyCompensation,
            double? amountLimit, int? limitPerTicket, bool isDiscontinued)
        {
            Id = id;
            Description = description;
            Amount = amount;
            AmountIsPercentage = amountIsPercentage;
            IsActive = isActive;
            IsExclusive = isExclusive;
            ApplyToAll = applyToAll;
            ThirdPartyCompensation = thirdPartyCompensation;
            AmountLimit = amountLimit;
            LimitPerTicket = limitPerTicket;
            IsDiscontinued = isDiscontinued;
        }

        public bool ApplyToTicket()
        {
            int catCount = CouponCategory.Count(Id);
            int itemCount = CouponItem.Count(Id);
            return (catCount + itemCount == 0);
        }

        public void Discontinue()
        {
            IsActive = false;
            IsDiscontinued = true;
            Update(this);
        }

        public void SetDescription(string description)
        {
            Description = description;
        }

        public void SetAmount(double amount)
        {
            Amount = amount;
        }

        public void SetIsPercentage(bool isPercentage)
        {
            AmountIsPercentage = isPercentage;
        }

        public void SetIsActive(bool isActive)
        {
            IsActive = isActive;
        }

        public void SetExclusive(bool isExclusive)
        {
            IsExclusive = isExclusive;
        }

        public void SetApplyToAll(bool applyToAll)
        {
            ApplyToAll = applyToAll;
        }

        public void SetThirdPartyCompensation(bool thirdPartyCompensation)
        {
            ThirdPartyCompensation = thirdPartyCompensation;
        }

        public void SetAmountLimit(double? amountLimit)
        {
            AmountLimit = amountLimit;
        }

        public void SetLimitPerTicket(int? limitPerTicket)
        {
            LimitPerTicket = limitPerTicket;
        }

        public void Update()
        {
            Coupon.Update(this);
        }

        #region static
        /// <summary>
        /// Add an entry to the Coupon table
        /// </summary>
        public static Coupon Add(string description, double amount, bool amountIsPercentage,
            bool isActive, bool isExclusive, bool applyToAll, bool thirdPartyCompensation,
            double? amountLimit, int? limitPerTicket)
        {
            Coupon result = null;
            bool isDiscontinued = false;

            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = new SqlCommand("AddCoupon", cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@CouponDescription", SqlDbType.Text, description);
                BuildSqlParameter(sqlCmd, "@CouponAmount", SqlDbType.Float, amount);
                BuildSqlParameter(sqlCmd, "@CouponAmountIsPercentage", SqlDbType.Bit, amountIsPercentage);
                BuildSqlParameter(sqlCmd, "@CouponActive", SqlDbType.Bit, isActive);
                BuildSqlParameter(sqlCmd, "@CouponIsExclusive", SqlDbType.Bit, isExclusive);
                BuildSqlParameter(sqlCmd, "@CouponApplyToAll", SqlDbType.Bit, applyToAll);
                BuildSqlParameter(sqlCmd, "@CouponThirdPartyCompensation", SqlDbType.Bit, thirdPartyCompensation);
                BuildSqlParameter(sqlCmd, "@CouponAmountLimit", SqlDbType.Float, amountLimit);
                BuildSqlParameter(sqlCmd, "@CouponLimitPerTicket", SqlDbType.Int, limitPerTicket);
                BuildSqlParameter(sqlCmd, "@CouponIsDiscontinued", SqlDbType.Bit, isDiscontinued);
                BuildSqlParameter(sqlCmd, "@CouponId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new Coupon(Convert.ToInt32(sqlCmd.Parameters["@CouponId"].Value),
                        description, amount, amountIsPercentage, isActive, isExclusive,
                        applyToAll, thirdPartyCompensation, amountLimit, limitPerTicket,
                        isDiscontinued);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Get all entries in the Coupon table
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Coupon> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Coupon WHERE CouponIsDiscontinued=0", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildCoupon(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get an entry from the Coupon table
        /// </summary>
        /// <param name="couponId"></param>
        /// <returns></returns>
        public static Coupon Get(int couponId)
        {
            Coupon result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, couponId);
            FinishedWithConnection(cn);
            return result;
        }

        private static Coupon Get(SqlConnection cn, int couponId)
        {
            Coupon result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Coupon WHERE CouponId=" + couponId, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildCoupon(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Update an entry in the Coupon table
        /// </summary>
        public static bool Update(Coupon coupon)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, coupon);
            FinishedWithConnection(cn);
            return result;
        }

        private static bool Update(SqlConnection cn, Coupon coupon)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE Coupon SET CouponDescription=@CouponDescription,CouponAmount=@CouponAmount,CouponAmountIsPercentage=@CouponAmountIsPercentage,CouponActive=@CouponActive,CouponIsExclusive=@CouponIsExclusive,CouponApplyToAll=@CouponApplyToAll,CouponThirdPartyCompensation=@CouponThirdPartyCompensation,CouponAmountLimit=@CouponAmountLimit,CouponLimitPerTicket=@CouponLimitPerTicket,CouponIsDiscontinued=@CouponIsDiscontinued WHERE CouponId=@CouponId";

                BuildSqlParameter(sqlCmd, "@CouponId", SqlDbType.Int, coupon.Id);
                BuildSqlParameter(sqlCmd, "@CouponDescription", SqlDbType.Text, coupon.Description);
                BuildSqlParameter(sqlCmd, "@CouponAmount", SqlDbType.Float, coupon.Amount);
                BuildSqlParameter(sqlCmd, "@CouponAmountIsPercentage", SqlDbType.Bit, coupon.AmountIsPercentage);
                BuildSqlParameter(sqlCmd, "@CouponActive", SqlDbType.Bit, coupon.IsActive);
                BuildSqlParameter(sqlCmd, "@CouponIsExclusive", SqlDbType.Bit, coupon.IsExclusive);
                BuildSqlParameter(sqlCmd, "@CouponApplyToAll", SqlDbType.Bit, coupon.ApplyToAll);
                BuildSqlParameter(sqlCmd, "@CouponThirdPartyCompensation", SqlDbType.Bit, coupon.ThirdPartyCompensation);
                BuildSqlParameter(sqlCmd, "@CouponAmountLimit", SqlDbType.Float, coupon.AmountLimit);
                BuildSqlParameter(sqlCmd, "@CouponLimitPerTicket", SqlDbType.Int, coupon.LimitPerTicket);
                BuildSqlParameter(sqlCmd, "@CouponIsDiscontinued", SqlDbType.Bit, coupon.IsDiscontinued);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        public static Coupon BuildCoupon(SqlDataReader rdr)
        {
            return new Coupon(
                GetId(rdr),
                GetDescription(rdr),
                GetAmount(rdr),
                GetAmountIsPercentage(rdr),
                GetIsActive(rdr),
                GetIsExclusive(rdr),
                GetApplyToAll(rdr),
                GetThirdPartyCompensation(rdr),
                GetAmountLimit(rdr),
                GetLimitPerTicket(rdr),
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

        private static double GetAmount(SqlDataReader rdr)
        {
            return rdr.GetDouble(2);
        }

        private static bool GetAmountIsPercentage(SqlDataReader rdr)
        {
            return rdr.GetBoolean(3);
        }

        private static bool GetIsActive(SqlDataReader rdr)
        {
            return rdr.GetBoolean(4);
        }

        private static bool GetIsExclusive(SqlDataReader rdr)
        {
            return rdr.GetBoolean(5);
        }

        private static bool GetApplyToAll(SqlDataReader rdr)
        {
            return rdr.GetBoolean(6);
        }

        private static bool GetThirdPartyCompensation(SqlDataReader rdr)
        {
            return rdr.GetBoolean(7);
        }

        private static double? GetAmountLimit(SqlDataReader rdr)
        {
            double? amountLimit = null;
            if (!rdr.IsDBNull(8))
                amountLimit = rdr.GetDouble(8);
            return amountLimit;
        }

        private static int? GetLimitPerTicket(SqlDataReader rdr)
        {
            int? limitPerTicket = null;
            if (!rdr.IsDBNull(9))
                limitPerTicket = rdr.GetInt32(9);
            return limitPerTicket;
        }

        private static bool GetIsDiscontinued(SqlDataReader rdr)
        {
            return rdr.GetBoolean(10);
        }
        #endregion

    }
}
