using System;
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
    public class BarRecipe : DataModelBase
    {
        #region Licensed Access Only
        static BarRecipe()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(BarRecipe).Assembly.GetName().GetPublicKeyToken(),
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

        [ModeledData()]
        public string Glass
        {
            get;
            private set;
        }

        [ModeledData()]
        public string Mix1Name
        {
            get;
            private set;
        }

        [ModeledData()]
        public string Mix1Amount
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataNullable()]
        public string Mix2Name
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataNullable()]
        public string Mix2Amount
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataNullable()]
        public string Mix3Name
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataNullable()]
        public string Mix3Amount
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataNullable()]
        public string Mix4Name
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataNullable()]
        public string Mix4Amount
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataNullable()]
        public string Ice
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataNullable()]
        public string IceAmount
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataNullable()]
        public string Garnish
        {
            get;
            private set;
        }

        private BarRecipe(int id, string name, string glass, string mix1, string mix1Amount, string mix2,
            string mix2Amount, string mix3, string mix3Amount, string mix4, string mix4Amount,
            string ice, string iceAmount, string garnish)
        {
            Id = id;
            Name = name;
            Glass = glass;
            Mix1Name = mix1;
            Mix1Amount = mix1Amount;
            Mix2Name = mix2;
            Mix2Amount = mix2Amount;
            Mix3Name = mix3;
            Mix3Amount = mix3Amount;
            Mix4Name = mix4;
            Mix4Amount = mix4Amount;
            Ice = ice;
            IceAmount = iceAmount;
            Garnish = garnish;
        }

        /// <summary>
        /// Add a new entry to the Product table
        /// </summary>
        public static void Add(string name, string glass, string mix1, string mix1Amount, string mix2,
            string mix2Amount, string mix3, string mix3Amount, string mix4, string mix4Amount,
            string ice, string iceAmount, string garnish)
        {
            DateTime purchaseTime = DateTime.Now;

            SqlConnection cn = GetConnection();
            string cmd = "INSERT INTO BarRecipe (BarRecipeName, BarRecipeGlass, BarRecipeMix1Name, BarRecipeMix1Amount, BarRecipeMix2Name, BarRecipeMix2Amount, BarRecipeMix3Name, BarRecipeMix3Amount, BarRecipeMix4Name, BarRecipeMix4Amount, BarRecipeIce, BarRecipeIceAmount, BarRecipeGarnish) " +
                "VALUES (@BarRecipeName, @BarRecipeGlass, @BarRecipeMix1Name, @BarRecipeMix1Amount, @BarRecipeMix2Name, @BarRecipeMix2Amount, @BarRecipeMix3Name, @BarRecipeMix3Amount, @BarRecipeMix4Name, @BarRecipeMix4Amount, @BarRecipeIce, @BarRecipeIceAmount, @BarRecipeGarnish)";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                BuildSqlParameter(sqlCmd, "@BarRecipeName", SqlDbType.Text, name);
                BuildSqlParameter(sqlCmd, "@BarRecipeGlass", SqlDbType.Text, glass);
                BuildSqlParameter(sqlCmd, "@BarRecipeMix1Name", SqlDbType.Text, mix1);
                BuildSqlParameter(sqlCmd, "@BarRecipeMix1Amount", SqlDbType.Text, mix1Amount);
                BuildSqlParameter(sqlCmd, "@BarRecipeMix2Name", SqlDbType.Text, mix2);
                BuildSqlParameter(sqlCmd, "@BarRecipeMix2Amount", SqlDbType.Text, mix2Amount);
                BuildSqlParameter(sqlCmd, "@BarRecipeMix3Name", SqlDbType.Text, mix3);
                BuildSqlParameter(sqlCmd, "@BarRecipeMix3Amount", SqlDbType.Text, mix3Amount);
                BuildSqlParameter(sqlCmd, "@BarRecipeMix4Name", SqlDbType.Text, mix4);
                BuildSqlParameter(sqlCmd, "@BarRecipeMix4Amount", SqlDbType.Text, mix4Amount);
                BuildSqlParameter(sqlCmd, "@BarRecipeIce", SqlDbType.Text, ice);
                BuildSqlParameter(sqlCmd, "@BarRecipeIceAmount", SqlDbType.Text, iceAmount);
                BuildSqlParameter(sqlCmd, "@BarRecipeGarnish", SqlDbType.Text, garnish);
                sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
        }

    }
}
