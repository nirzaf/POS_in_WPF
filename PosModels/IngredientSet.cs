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
    public class IngredientSet : DataModelBase
    {
        #region Licensed Access Only
        static IngredientSet()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(IngredientSet).Assembly.GetName().GetPublicKeyToken(),
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

        /// <summary>
        /// This ingredient id is the id of the ingredient with extended ingredients
        /// </summary>
        [ModeledData()]
        public int ExtendedIngredientId
        {
            get;
            private set;
        }

        /// <summary>
        /// This ingredient id is the id of the extended ingredient
        /// </summary>
        [ModeledData()]
        public int IngredientId
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
        [ModeledDataType("SMALLINT")]
        public MeasurementUnit MeasurementUnit
        {
            get;
            private set;
        }

        private IngredientSet(int id, int extendedIngredientId,
            int ingredientId, double amount, MeasurementUnit measuementUnit)
        {
            Id = id;
            ExtendedIngredientId = extendedIngredientId;
            IngredientId = ingredientId;
            Amount = amount;
            MeasurementUnit = measuementUnit;
        }

        public void SetExtendedIngredientId(int extendedIngredientId)
        {
            ExtendedIngredientId = extendedIngredientId;
        }

        public void SetIngredientId(int ingredientId)
        {
            IngredientId = ingredientId;
        }

        public void SetAmount(double amount)
        {
            Amount = amount;
        }

        public void SetMeasurementUnit(MeasurementUnit measurementUnit)
        {
            MeasurementUnit = measurementUnit;
        }

        public bool Update()
        {
            return IngredientSet.Update(this);
        }

        #region Static
        /// <summary>
        /// Add a new entry to the IngredientSet table
        /// </summary>
        public static IngredientSet Add(int extendedIngredientId, int ingredientId,
            double amount, MeasurementUnit measurementUnit)
        {
            IngredientSet result = null;

            SqlConnection cn = GetConnection();
            string cmd = "AddIngredientSet";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@IngredientSetExtendedIngredientId", SqlDbType.Int, extendedIngredientId);
                BuildSqlParameter(sqlCmd, "@IngredientSetIngredientId", SqlDbType.Int, ingredientId);
                BuildSqlParameter(sqlCmd, "@IngredientSetAmount", SqlDbType.Float, amount);
                BuildSqlParameter(sqlCmd, "@IngredientSetMeasurementUnit", SqlDbType.SmallInt, measurementUnit);
                BuildSqlParameter(sqlCmd, "@IngredientSetId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new IngredientSet(Convert.ToInt32(sqlCmd.Parameters["@IngredientSetId"].Value),
                        extendedIngredientId, ingredientId, amount, measurementUnit);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Delete an entry from the IngredientSet table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            IngredientSet ingredientSet = Get(cn, id);
            if (ingredientSet != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM IngredientSet WHERE IngredientSetId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the IngredientSet table
        /// </summary>
        public static IngredientSet Get(int id)
        {
            IngredientSet result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);
            return result;
        }

        private static IngredientSet Get(SqlConnection cn, int id)
        {
            IngredientSet result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM IngredientSet WHERE IngredientSetId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildIngredientSet(rdr);
                    }
                }
            }
            return result;
        }

        public static IngredientSet Get(int extendedIngredientId, int ingredientId)
        {
            IngredientSet result = null;

            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM IngredientSet WHERE IngredientSetExtendedIngredientId=" +
                    extendedIngredientId + " AND IngredientSetIngredientId=" + ingredientId, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildIngredientSet(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        public static bool HasEntries(int ingredientId)
        {
            bool result = false;
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand(
                "SELECT TOP 1 * FROM IngredientSet WHERE IngredientSetExtendedIngredientId=" +
                    ingredientId, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = true;
                    }
                }
            }
            FinishedWithConnection(cn);
            return result;
        }
        
        /// <summary>
        /// Get all the entries in the IngredientSet table
        /// </summary>
        public static IEnumerable<IngredientSet> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM IngredientSet", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildIngredientSet(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get all the entries in the IngredientSet table
        /// </summary>
        public static IEnumerable<IngredientSet> GetAll(int ingredientId)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM IngredientSet WHERE IngredientSetExtendedIngredientId=" + ingredientId, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildIngredientSet(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the IngredientSet table
        /// </summary>
        public static bool Update(IngredientSet ingredientSet)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, ingredientSet);
            FinishedWithConnection(cn);
            return result;
        }

        private static bool Update(SqlConnection cn, IngredientSet ingredientSet)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE IngredientSet SET IngredientSetExtendedIngredientId=@IngredientSetExtendedIngredientId,IngredientSetIngredientId=@IngredientSetIngredientId,IngredientSetAmount=@IngredientSetAmount,IngredientSetMeasurementUnit=@IngredientSetMeasurementUnit WHERE IngredientSetId=@IngredientSetId";

                BuildSqlParameter(sqlCmd, "@IngredientSetId", SqlDbType.Int, ingredientSet.Id);
                BuildSqlParameter(sqlCmd, "@IngredientSetExtendedIngredientId", SqlDbType.Int, ingredientSet.ExtendedIngredientId);
                BuildSqlParameter(sqlCmd, "@IngredientSetIngredientId", SqlDbType.Int, ingredientSet.IngredientId);
                BuildSqlParameter(sqlCmd, "@IngredientSetMeasurementUnit", SqlDbType.SmallInt, ingredientSet.MeasurementUnit);
                BuildSqlParameter(sqlCmd, "@IngredientSetAmount", SqlDbType.Float, ingredientSet.Amount);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Build a IngredientSet object from a SqlDataReader object
        /// </summary>
        private static IngredientSet BuildIngredientSet(SqlDataReader rdr)
        {
            return new IngredientSet(
                GetId(rdr),
                GetExtendedIngredientId(rdr),
                GetIngredientId(rdr),
                GetAmount(rdr),
                GetMeasurementUnit(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int GetExtendedIngredientId(SqlDataReader rdr)
        {
            return rdr.GetInt32(1);
        }

        private static int GetIngredientId(SqlDataReader rdr)
        {
            return rdr.GetInt32(2);
        }

        private static double GetAmount(SqlDataReader rdr)
        {
            return rdr.GetDouble(3);
        }

        private static MeasurementUnit GetMeasurementUnit(SqlDataReader rdr)
        {
            return rdr.GetInt16(4).GetMeasurementUnit();
        }
        #endregion
    }
}
