using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.Linq.Mapping;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace PosModels.Types
{
    public abstract class DataModelBase
    {
        private static bool? _isInDesignMode;
        /// <summary>
        /// Gets a value indicating whether the control is in design mode (running in Blend
        /// or Visual Studio).
        /// </summary>
        public static bool IsInDesignMode
        {
            get
            {
#if DEBUG
                if (!_isInDesignMode.HasValue)
                {
#if SILVERLIGHT
                    _isInDesignMode = DesignerProperties.IsInDesignTool;
#else
                    //var prop = DesignerProperties.IsInDesignModeProperty;
                    //_isInDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement))
                    //    .Metadata.DefaultValue;
                    _isInDesignMode = false;
#endif
                }
                return _isInDesignMode.Value;
#else
                return false;
#endif
            }
        }

        public static SqlConnection GetConnection()
        {
            return GetConnection(true, true);
        }

        public static SqlConnection GetConnection(bool forceLocal, bool includeDatabase = false)
        {
            string databaseName = LocalSetting.DatabaseServerDatabaseName;
#if DEBUG
            if (IsInDesignMode)
            {
                forceLocal = true;
                databaseName = "TemPOS";
                includeDatabase = true;
            }
#endif
            var result =
                new SqlConnection((forceLocal ?
                @"Server=Dalaran\MSSQL2012" + (includeDatabase ? ";Database=" + databaseName : "") + ";Integrated Security=True" :
                (includeDatabase ? LocalSetting.ConnectionString : LocalSetting.ConnectionStringNoDatabase)));
            result.Open();
            return result;
        }

        public static void FinishedWithConnection(SqlConnection connection)
        {
            connection.Close();
        }

        public static void CloseAll()
        {
            SqlConnection.ClearAllPools();
        }

        public static void BuildOleDbParameter(OleDbCommand sqlCmd, string name, SqlDbType sqlType, object value)
        {
            if (value == null)
                value = DBNull.Value;
            var sqlParam = new OleDbParameter(name, sqlType);
            sqlCmd.Parameters.Add(sqlParam);
            sqlCmd.Parameters[name].Value = value;
        }

        public static void BuildOleDbParameter(OleDbCommand sqlCmd, string name, SqlDbType sqlType, ParameterDirection direction)
        {
            var sqlParam = new OleDbParameter(name, sqlType);
            sqlCmd.Parameters.Add(sqlParam);
            sqlCmd.Parameters[name].Direction = direction;
        }

        public static void BuildSqlParameter(SqlCommand sqlCmd, string name, SqlDbType sqlType, object value)
        {
            if (value == null)
                value = DBNull.Value;
            var sqlParam = new SqlParameter(name, sqlType);
            sqlCmd.Parameters.Add(sqlParam);
            sqlCmd.Parameters[name].Value = value;
        }

        public static void BuildSqlParameter(SqlCommand sqlCmd, string name, SqlDbType sqlType, ParameterDirection direction)
        {
            var sqlParam = new SqlParameter(name, sqlType);
            sqlCmd.Parameters.Add(sqlParam);
            sqlCmd.Parameters[name].Direction = direction;
        }

        public static bool TestConnection(out string statusMessage)
        {
            try
            {
                FinishedWithConnection(GetConnection(true, false));
                statusMessage = null;
                return true;
            }
            catch (Exception ex)
            {
                statusMessage = ex.Message;
                return false;
            }
        }

        public static bool DatabaseExists(string databaseName)
        {
            SqlConnection cn = GetConnection(true, false);
            DataTable schema = cn.GetSchema(SqlClientMetaDataCollectionNames.Databases);
            bool results = schema.Rows.Cast<DataRow>().Any(row => row[0].Equals(databaseName));
            FinishedWithConnection(cn);
            return results;
        }

        /// <summary>
        /// Install an assembly without the need for the sysadmin server permission, by passing a hex
        /// string rather than a file path.
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <param name="databaseName"></param>
        /// <param name="assemblyName"></param>
        /// <param name="alter"></param>
        /// <returns></returns>
        public static bool InstallAssembly(string assemblyPath, string databaseName, string assemblyName, bool alter = false)
        {
            if (!File.Exists(assemblyPath))
                return false;

            string assemblyHexString = GetAssemblyHexString(assemblyPath);

            string cmd =
                "USE [" + databaseName + @"]" + Environment.NewLine +
                "GO" + Environment.NewLine +
                (alter ? "ALTER" : "CREATE") + " ASSEMBLY [" + assemblyName + "] FROM " + assemblyHexString + Environment.NewLine +
                "WITH PERMISSION_SET = SAFE" + Environment.NewLine +
                //"EXECUTE AS USER='dbo'" + Environment.NewLine +
                "GO";

            bool updated = true;
            SqlException exception = null;
            SqlConnection conn = GetConnection();
            try
            {
                ExecuteNonQuery(cmd, conn);
            }
            catch (SqlException ex)
            {
                if (!ex.Message.Contains("identical to an assembly that is already registered"))
                    exception = ex;
                SqlConnection.ClearAllPools();
                updated = false;
            }
            finally
            {
                FinishedWithConnection(conn);
            }
            if (exception != null)
                throw exception;
            return updated;
        }

        public static string GetAssemblyHexString(string assemblyPath)
        {
            if (!Path.IsPathRooted(assemblyPath))
                assemblyPath = Path.Combine(Environment.CurrentDirectory, assemblyPath);

            var builder = new StringBuilder();
            builder.Append("0x");

            using (var stream = new FileStream(assemblyPath,
                                                      FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                int currentByte = stream.ReadByte();
                while (currentByte > -1)
                {
                    builder.Append(currentByte.ToString("X2", CultureInfo.InvariantCulture));
                    currentByte = stream.ReadByte();
                }
            }

            return builder.ToString();
        }

        public static void ExecuteSqlScript(string pathToSqlFile)
        {
            var file = new FileInfo(pathToSqlFile);
            ExecuteSqlScript(file.OpenText());
        }

        public static void ExecuteSqlScript(StreamReader streamReader, bool useDatabase = true)
        {
            string script = "";
            using (streamReader)
            {
                script += streamReader.ReadToEnd();
            }

            ExecuteNonQuery(script, useDatabase);
        }

        public static void ExecuteNonQuery(string scriptText, bool useDatabase = true)
        {
            SqlConnection conn = GetConnection(true, false);
            ExecuteNonQuery(scriptText, conn, useDatabase);
            FinishedWithConnection(conn);
        }

        public static void ExecuteNonQuery(string scriptText, SqlConnection conn, bool useDatabase = true)
        {
            if (useDatabase)
                scriptText = "USE [" + LocalSetting.DatabaseServerDatabaseName + "]" +
                             Environment.NewLine + "GO" + Environment.NewLine + scriptText;
            foreach (string commandString in Regex.Split(scriptText, "^\\s*GO\\s*$",
                RegexOptions.Multiline | RegexOptions.IgnoreCase)
                .Where(commandString => !String.IsNullOrWhiteSpace(commandString)))
            {
                using (var sqlCommand = new SqlCommand(commandString, conn))
                {
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        #region Model / Database Comparisons

        public static List<string> GetDatabaseTableNames()
        {
            SqlConnection cn = GetConnection();
            DataTable schema = cn.GetSchema(SqlClientMetaDataCollectionNames.Tables);
            var results = (
                from DataRow row in schema.Rows
                from DataColumn col in schema.Columns
                where col.ColumnName.Equals("TABLE_NAME")
                select String.Format("{0}", row[col])).ToList();
            FinishedWithConnection(cn);

            results.Sort();
            return results;
        }

        public static IEnumerable<string> GetTableColumnTypes()
        {
            SqlConnection cn = GetConnection();
            DataTable schema = cn.GetSchema(SqlClientMetaDataCollectionNames.Columns);

            string tableName = null;
            string columnName = null;
            string columnType = null;
            string columnIsNullable = null;
            foreach (DataRow row in schema.Rows)
            {
                foreach (DataColumn col in schema.Columns)
                {
                    if (col.ColumnName.Equals("TABLE_NAME"))
                        tableName = String.Format("{0}", row[col]);
                    else if (col.ColumnName.Equals("COLUMN_NAME"))
                        columnName = String.Format("{0}", row[col]);
                    else if (col.ColumnName.Equals("DATA_TYPE"))
                        columnType = String.Format("{0}", row[col]);
                    else if (col.ColumnName.Equals("IS_NULLABLE"))
                        columnIsNullable = String.Format("{0}", row[col]);
                    else
                        continue;
                    if ((tableName != null) && (columnName != null) &&
                        (columnType != null) && (columnIsNullable != null))
                    {
                        string result = tableName + "." + columnName + " " + columnType.ToUpper() +
                                        " " + (columnIsNullable.Equals("YES") ? "NULLABLE" : "");
                        tableName = null;
                        columnName = null;
                        columnType = null;
                        columnIsNullable = null;
                        string[] tokens = result.Split('.');
                        string value = result.Replace("." + tokens[0], ".").Trim();
                        yield return value;
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static List<Type> GetModelClasses()
        {
            var results = new List<Type>();

            // Setup event handler to resolve assemblies
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve +=
                CurrentDomain_ReflectionOnlyAssemblyResolve;

            string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            Assembly a = Assembly.ReflectionOnlyLoadFrom(path + @"\PosModels.dll");

            // process types here
            foreach (Type t in a.GetTypes())
            {
                MemberInfo info = t;
                IList<CustomAttributeData> list = info.GetCustomAttributesData();

                bool found = list.Any(customAttribute =>
                    (customAttribute.Constructor.DeclaringType != null) &&
                    (customAttribute.Constructor.DeclaringType.Name.Equals("ModeledDataClassAttribute") ||
                    customAttribute.Constructor.DeclaringType.Name.Equals("TableAttribute")));
                if (!found)
                    continue;
                string className = t.ToString();
                if (className.Contains("+"))
                    className = className.Substring(0, className
                        .IndexOf("+", StringComparison.Ordinal));
                Type t1 = Type.GetType(className);
                if (!results.Contains(t1))
                    results.Add(t1);
            }

            results.Sort(new TypeCompare());
            return results;
        }

        // method later in the class:

        private static Assembly CurrentDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Assembly.ReflectionOnlyLoad(args.Name);
        }

        public class TypeCompare : IComparer<Type>
        {
            public int Compare(Type x, Type y)
            {
                return String.Compare(x.ToString(), y.ToString(), StringComparison.Ordinal);
            }
        }

        public static IEnumerable<string> GetModeledDataProperties(Type t)
        {
            foreach (PropertyInfo prop in t.GetProperties())
            {
                string tableName = t.ToString();
                if (tableName.Contains('.'))
                {
                    int lastIndex = tableName.LastIndexOf('.');
                    tableName = tableName.Substring(lastIndex + 1, tableName.Length - lastIndex - 1);
                }

                MemberInfo info = prop;
                ModeledDataAttribute attrib = null;
                ModeledDataTypeAttribute typeAttrib = null;
                ModeledDataNullableAttribute nullableAttrib = null;
                ColumnAttribute columnAttribute = null;

                foreach (object attribObject in info.GetCustomAttributes(true))
                {
                    if (attribObject is ModeledDataAttribute)
                        attrib = attribObject as ModeledDataAttribute;
                    else if (attribObject is ModeledDataTypeAttribute)
                        typeAttrib = attribObject as ModeledDataTypeAttribute;
                    else if (attribObject is ModeledDataNullableAttribute)
                        nullableAttrib = attribObject as ModeledDataNullableAttribute;
                    else if (attribObject is ColumnAttribute)
                        columnAttribute = attribObject as ColumnAttribute;
                }

                // Check for nullable types
                bool isNullable = false;
                Type propType = prop.PropertyType;
                if (propType.Name.StartsWith("Nullable"))
                {
                    propType = Nullable.GetUnderlyingType(propType);
                    isNullable = true;
                }

                // Has a ColumnAttribute
                if (columnAttribute != null)
                {
                    if (propType == typeof(String))
                        isNullable = true;
                    if (isNullable && !columnAttribute.CanBeNull)
                        isNullable = false;
                    yield return tableName + "." +
                        (!string.IsNullOrEmpty(columnAttribute.Name) ?
                        columnAttribute.Name : prop.Name) + " " +
                        GetSqlTypeString(!string.IsNullOrEmpty(columnAttribute.DbType) ?
                        columnAttribute.DbType : propType.Name, isNullable);
                }

                // Has a ModeledDataAttribute
                else if (attrib != null)
                {
                    if (attrib.Alias1Name == null)
                        yield return tableName + "." + prop.Name + " " +
                                     GetSqlTypeString(
                                         (((typeAttrib != null) && (typeAttrib.Alias1Type != null)) ?
                                         typeAttrib.Alias1Type : propType.Name), ((nullableAttrib != null) || isNullable));
                    else
                        yield return tableName + "." + attrib.Alias1Name + " " +
                                     GetSqlTypeString(
                                         (((typeAttrib != null) && (typeAttrib.Alias1Type != null)) ?
                                         typeAttrib.Alias1Type : propType.Name), ((nullableAttrib != null) || isNullable));
                    if (typeAttrib != null)
                    {
                        if (attrib.Alias2Name != null)
                            yield return tableName + "." + attrib.Alias2Name + " " +
                                         GetSqlTypeString(typeAttrib.Alias2Type, (nullableAttrib != null));
                        if (attrib.Alias3Name != null)
                            yield return tableName + "." + attrib.Alias3Name + " " +
                                         GetSqlTypeString(typeAttrib.Alias3Type, (nullableAttrib != null));
                        if (attrib.Alias4Name != null)
                            yield return tableName + "." + attrib.Alias4Name + " " +
                                         GetSqlTypeString(typeAttrib.Alias4Type, (nullableAttrib != null));
                    }
                }
            }
        }

        private static string GetSqlTypeString(string name, bool isNullable = false)
        {
            if (name.Equals("Byte"))
                return "TINYINT" + (isNullable ? " NULLABLE" : "");
            if (name.Equals("Int16"))
                return "SMALLINT" + (isNullable ? " NULLABLE" : "");
            if (name.Equals("Int32"))
                return "INT" + (isNullable ? " NULLABLE" : "");
            if (name.Equals("Boolean"))
                return "BIT" + (isNullable ? " NULLABLE" : "");
            if (name.Equals("Double"))
                return "FLOAT" + (isNullable ? " NULLABLE" : "");
            if (name.Equals("String"))
                return "TEXT" + (isNullable ? " NULLABLE" : "");
            if (name.Equals("DateTime"))
                return "DATETIME" + (isNullable ? " NULLABLE" : "");
            if (name.Equals("Byte[]"))
                return "BINARY" + (isNullable ? " NULLABLE" : "");
            if (name.Equals("TimeSpan"))
                return "TIME" + (isNullable ? " NULLABLE" : "");
            return name + (isNullable ? " NULLABLE" : "");
        }

        /// <summary>
        /// Checks that the database tables being used match the modeled classes
        /// </summary>
        /// <returns></returns>
        public static bool ValidateDatabase()
        {
            var modelClasses = GetModelClasses();

            var modelEntries = modelClasses.SelectMany(GetModeledDataProperties).ToList();
            modelEntries.Sort();

            var databaseEntries = GetTableColumnTypes().ToList();
            databaseEntries.Sort();

            // Look for mismatches
            int mismatchCount = modelEntries.Count(modelEntry => !databaseEntries.Contains(modelEntry)) +
                databaseEntries.Count(databaseEntry => !modelEntries.Contains(databaseEntry));
            return (mismatchCount == 0);
        }

#if DEBUG
        /// <summary>
        /// Checks that the database tables being used match the modeled classes
        /// </summary>
        /// <returns></returns>
        public static string InvalidDatabaseReport()
        {
            var modelClasses = GetModelClasses();
            var modelEntries = modelClasses.SelectMany(GetModeledDataProperties).ToList();
            var databaseEntries = GetTableColumnTypes().ToList();

            // Look for mismatches
            var missingInDatabase = modelEntries.Where(modelEntry => !databaseEntries.Contains((modelEntry))).ToList();
            var missingInModel = databaseEntries.Where(databaseEntry => !modelEntries.Contains((databaseEntry))).ToList();

            string results = "";
            if (missingInModel.Count > 0)
            {
                results += "Results for missing in model" + Environment.NewLine;
                results = missingInModel.Aggregate(results, (current, missing) => current + ("    " + missing));
                results += Environment.NewLine;
            }
            if (missingInDatabase.Count > 0)
            {
                results += "Results for missing in database" + Environment.NewLine;
                results = missingInDatabase.Aggregate(results, (current, missing) => current + ("    " + missing + Environment.NewLine));
            }

            return results;
        }
#endif
        #endregion
    }
}
