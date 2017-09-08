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
using TemposLibrary;

namespace PosModels
{
    [ModeledDataClass()]
    public class Employee : DataModelBase
    {
        #region Licensed Access Only
        static Employee()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(Employee).Assembly.GetName().GetPublicKeyToken(),
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
        public int PersonId
        {
            get;
            private set;
        }

        [ModeledData("ScanCode")]
        [ModeledDataNullable()]
        public byte[] ScanCodeData
        {
            get;
            private set;
        }

        [ModeledData("PermissionsCode")]
        [ModeledDataNullable()]
        public byte[] RawPermissionField
        {
            get;
            private set;
        }

        /// <summary>
        /// In the US, this is usually the social security number
        /// </summary>
        [ModeledData("FederalId")]
        [ModeledDataNullable()]
        public string FederalTaxId
        {
            get;
            private set;
        }

        [ModeledData()]
        public bool IsTerminated
        {
            get;
            private set;
        }

        [ModeledData()]
        public bool IsRemoved
        {
            get;
            private set;
        }

        private Employee(int id, int personId, byte[] scanCode, byte[] rawPermissionField,
            string federalTaxId, bool isTerminated, bool isRemoved)
        {
            Id = id;
            PersonId = personId;
            ScanCodeData = scanCode;
            IsTerminated = isTerminated;
            RawPermissionField = rawPermissionField;
            FederalTaxId = federalTaxId;
            IsRemoved = isRemoved;
        }

        public IEnumerable<EmployeeJob> GetJobs()
        {
            foreach (EmployeeJob employeeJob in Employee.GetJobs(Id))
            {
                if (employeeJob.IsEnabled)
                    yield return employeeJob;
            }
        }

        public IEnumerable<Permissions> GetPermissions()
        {
            string[] names = Enum.GetNames(typeof(Permissions));
            foreach (string name in names)
            {
                Permissions permission = (Permissions)Enum.Parse(typeof(Permissions), name);
                if (HasPermission(permission))
                    yield return permission;
            }
        }

        public bool CheckPassword(string scanCode)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(scanCode);
            byte[] hashedBytes = SRP6.Sha1Hash(bytes);
            for (int i = 0; i < hashedBytes.Length; i++)
            {
                if (hashedBytes[i] != ScanCodeData[i])
                    return false;
            }
            return true;
        }

        public bool HasPermission(Permissions permission)
        {
            if (permission == Permissions.None)
                return false;
            byte byteIndex = (byte)((int)permission / 8);
            byte bitIndex = (byte)((int)permission % 8);
            return Bit.TestBit(RawPermissionField[byteIndex], bitIndex);
        }

        /// <summary>
        /// Does Employee has one of the specified permissions
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public bool HasPermission(Permissions[] permissions)
        {
            foreach (Permissions permission in permissions)
            {
                if (HasPermission(permission))
                    return true;
            }
            return false;
        }

        public void SetFederalTaxId(string federalTaxId)
        {
            if ((federalTaxId != null) && federalTaxId.Equals(""))
                federalTaxId = null;
            FederalTaxId = federalTaxId;
        }

        public void SetPermission(Permissions permission, bool hasPermission)
        {
            if (permission == Permissions.None)
                return;
            byte byteIndex = (byte)((int)permission / 8);
            byte bitIndex = (byte)((int)permission % 8);
            byte mask = (byte)Bit.LeftShift(1, bitIndex);
            if (hasPermission)
                RawPermissionField[byteIndex] = Bit.SetBit(mask, RawPermissionField[byteIndex]);
            else
                RawPermissionField[byteIndex] = Bit.ClearBit(mask, RawPermissionField[byteIndex]);
        }

        public void GrantAllPermissions()
        {
            for (int i = 0; i < RawPermissionField.Length; i++)
            {
                RawPermissionField[i] = (byte)0xff;
            }
        }

        public bool IsClockedIn()
        {
            EmployeeTimesheet clockIn =
                EmployeeTimesheet.GetOpenEntryForEmployee(Id);
            return (clockIn != null);
        }

        public void SetScanCodeData(string data)
        {
            byte[] bytes = null;
            if (data != null)
                bytes = UTF8Encoding.UTF8.GetBytes(data);
            SetScanCodeData(bytes);
        }

        public void SetScanCodeData(byte[] scanCodeData)
        {
            byte[] hashedScanCode = null;
            if (scanCodeData != null)
                hashedScanCode = SRP6.Sha1Hash(scanCodeData);
            if ((hashedScanCode != null) && (hashedScanCode.Length != 520))
            {
                byte[] originalScanCode = hashedScanCode;
                hashedScanCode = new byte[520];
                for (int i = 0; i < 520; i++)
                {
                    if (i < originalScanCode.Length)
                        hashedScanCode[i] = originalScanCode[i];
                    else
                        hashedScanCode[i] = 0;
                }
            }
            ScanCodeData = hashedScanCode;
        }

        public bool Terminate(DateTime terminationDate, string reason = null)
        {
            if (IsTerminated)
                return false;
            EmployeeStatus status = EmployeeStatus.GetEmployeesActiveStatus(Id);
            if (status != null)
                status.Terminate(terminationDate, reason);
            IsTerminated = true;
            ScanCodeData = null;
            return Update(this);
        }

        public bool Rehire(DateTime dateTime)
        {
            if (!IsTerminated)
                return false;
            EmployeeStatus.Add(Id, DateTime.Now);
            IsTerminated = false;
            return Update(this);
        }

        public bool Remove()
        {
            if (!IsTerminated)
                return false;
            IsRemoved = true;
            return Update(this);
        }

        public bool Update()
        {
            return Update(this);
        }

        #region static
        public static Employee Add(int personId, Permissions[] permissions,
            string password, string federalTaxId)
        {
            byte[] bytes = null;
            if (password != null)
                bytes = Encoding.UTF8.GetBytes(password);
            return Add(personId, permissions, bytes, federalTaxId);
        }

        public static Employee Add(int personId, Permissions[] permissions,
            byte[] scanCode, string federalTaxId)
        {
            byte[] permissionBytes = GetPermissionBytes(permissions);
            byte[] hashedScanCode = SRP6.Sha1Hash(scanCode);
            Employee result = null;

            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = new SqlCommand("AddEmployee", cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@EmployeePersonId", SqlDbType.Int, personId);
                BuildSqlParameter(sqlCmd, "@EmployeeScanCode", SqlDbType.Binary, hashedScanCode);
                BuildSqlParameter(sqlCmd, "@EmployeePermissionsCode", SqlDbType.Binary, permissionBytes);
                BuildSqlParameter(sqlCmd, "@EmployeeFederalId", SqlDbType.Text, federalTaxId);
                BuildSqlParameter(sqlCmd, "@EmployeeId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new Employee(Convert.ToInt32(sqlCmd.Parameters["@EmployeeId"].Value),
                        personId, hashedScanCode, permissionBytes, federalTaxId,
                        false, false);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        private static byte[] GetPermissionBytes(Permissions[] permissions)
        {
            byte[] result = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            if (permissions != null)
            {
                foreach (Permissions permission in permissions)
                {
                    if (permission == Permissions.None)
                        continue;
                    byte byteIndex = (byte)((int)permission / 8);
                    byte bitIndex = (byte)((int)permission % 8);
                    byte mask = (byte)Bit.LeftShift(1, bitIndex);
                    result[byteIndex] = Bit.SetBit(mask, result[byteIndex]);
                }
            }
            return result;
        }

        /// <summary>
        /// Get an entry from the Employee table
        /// </summary>
        public static Employee Get(int id)
        {
            Employee result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);
            return result;
        }

        private static Employee Get(SqlConnection cn, int id)
        {
            Employee result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Employee WHERE EmployeeId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildEmployee(rdr);
                    }
                }
            }
            return result;
        }

        public static int GetPersonId(int employeeId)
        {
            int personId = 0;

            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT EmployeePersonId FROM Employee WHERE EmployeeId=" + employeeId, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        try
                        {
                            personId = Convert.ToInt32(rdr[0].ToString());
                        }
                        catch
                        {
                        }
                    }
                }
            }
            FinishedWithConnection(cn);
            return personId;
        }

        public static IEnumerable<EmployeeJob> GetJobs(int employeeId)
        {
            List<int> jobIds = new List<int>();
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT EmployeePayRateEmployeeJobId FROM EmployeePayRate INNER JOIN EmployeeJob ON EmployeePayRateEmployeeJobId = EmployeeJobId WHERE EmployeePayRateEmployeeId=@EmployeePayRateEmployeeId", cn))
            {
                BuildSqlParameter(cmd, "@EmployeePayRateEmployeeId", SqlDbType.Int, employeeId);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        try
                        {
                            jobIds.Add(Convert.ToInt32(rdr[0]));
                        }
                        catch
                        {
                        }
                    }
                }
                foreach (int jobId in jobIds)
                {
                    yield return EmployeeJob.Get(jobId);
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get all the entries in the Employee table
        /// </summary>
        public static IEnumerable<Employee> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Employee WHERE EmployeeIsRemoved=0", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildEmployee(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get an entry from the Employee table, for the matching scanCode
        /// </summary>
        public static Employee GetFromScanCode(string scanCode)
        {
#if DEBUG
            if (String.IsNullOrEmpty(scanCode))
            {
                IEnumerable<Employee> employees = Employee.GetAll();
                foreach (Employee employee in employees)
                {
                    if (employee.ScanCodeData == null)
                        return employee;
                }
                return null;
            }
#endif
            if (!String.IsNullOrEmpty(scanCode))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(scanCode);
                byte[] hashedBytes = SRP6.Sha1Hash(bytes);
                IEnumerable<Employee> employees = Employee.GetAll();
                foreach (Employee employee in employees)
                {
                    if (ScanCodesMatch(employee.ScanCodeData, hashedBytes))
                        return employee;
                }
            }
            return null;
        }

        private static bool ScanCodesMatch(byte[] scanCodeData, byte[] bytes)
        {
            bool match = false;
            if ((scanCodeData == null) || (bytes == null))
                return false;
            for (int i = 0; i < scanCodeData.Length; i++)
            {
                if ((i < bytes.Length) && (scanCodeData[i] != bytes[i]))
                    break;
                else if ((i >= bytes.Length) && (scanCodeData[i] != 0))
                    break;
                if (i == scanCodeData.Length - 1)
                    match = true;
            }
            return match;
        }

        /// <summary>
        /// Update an entry in the Employee table
        /// </summary>
        public static bool Update(Employee employee)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, employee);
            FinishedWithConnection(cn);

            return result;
        }

        private static bool Update(SqlConnection cn, Employee employee)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE Employee SET EmployeePersonId=@EmployeePersonId,EmployeeScanCode=@EmployeeScanCode,EmployeePermissionsCode=@EmployeePermissionsCode,EmployeeFederalId=@EmployeeFederalId,EmployeeIsTerminated=@EmployeeIsTerminated,EmployeeIsRemoved=@EmployeeIsRemoved WHERE EmployeeId=@EmployeeId";

                BuildSqlParameter(sqlCmd, "@EmployeeId", SqlDbType.Int, employee.Id);
                BuildSqlParameter(sqlCmd, "@EmployeePersonId", SqlDbType.Int, employee.PersonId);
                BuildSqlParameter(sqlCmd, "@EmployeeScanCode", SqlDbType.Binary, employee.ScanCodeData);
                BuildSqlParameter(sqlCmd, "@EmployeePermissionsCode", SqlDbType.Binary, employee.RawPermissionField);
                BuildSqlParameter(sqlCmd, "@EmployeeFederalId", SqlDbType.Text, employee.FederalTaxId);
                BuildSqlParameter(sqlCmd, "@EmployeeIsTerminated", SqlDbType.Bit, employee.IsTerminated);
                BuildSqlParameter(sqlCmd, "@EmployeeIsRemoved", SqlDbType.Bit, employee.IsRemoved);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        private static byte[] GenerateScanCodeData()
        {
            byte[] scanCode = new byte[520];
            Random random = new Random();
            random.NextBytes(scanCode);
            return scanCode;
        }

        public static void Refresh(Employee employee)
        {
            Refresh(employee, Employee.Get(employee.Id));
        }

        public static void Refresh(Employee employee, Employee tempEmployee)
        {
            if ((employee == null) || (tempEmployee == null))
                return;
            employee.IsTerminated = tempEmployee.IsTerminated;
            employee.RawPermissionField = tempEmployee.RawPermissionField;
            employee.ScanCodeData = tempEmployee.ScanCodeData;
            employee.PersonId = tempEmployee.PersonId;
            employee.IsRemoved = tempEmployee.IsRemoved;
            employee.FederalTaxId = tempEmployee.FederalTaxId;
        }

        public static bool NoEmployeeExists
        {
            get
            {
                int count = 0;

                SqlConnection cn = GetConnection();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Employee", cn);
                SqlDataReader rdr = cmd.ExecuteReader();
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
                rdr.Close();
                FinishedWithConnection(cn);
                return (count == 0);
            }
        }

        /// <summary>
        /// Build a Employee object from a SqlDataReader object
        /// </summary>
        private static Employee BuildEmployee(SqlDataReader rdr)
        {
            return new Employee(
                GetId(rdr),
                GetPersonId(rdr),
                GetScanCode(rdr),
                GetRawPermissions(rdr),
                GetFederalTaxId(rdr),
                GetIsTerminated(rdr),
                GetIsRemoved(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int GetPersonId(SqlDataReader rdr)
        {
            return rdr.GetInt32(1);
        }

        private static byte[] GetScanCode(SqlDataReader rdr)
        {
            byte[] scanCode = null;
            if (!rdr.IsDBNull(2))
                scanCode = rdr.GetSqlBinary(2).Value;
            return scanCode;
        }

        private static byte[] GetRawPermissions(SqlDataReader rdr)
        {
            byte[] rawPermissions = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            if (!rdr.IsDBNull(3))
                rawPermissions = rdr.GetSqlBinary(3).Value;
            return rawPermissions;
        }

        private static string GetFederalTaxId(SqlDataReader rdr)
        {
            string result = null;
            if (!rdr.IsDBNull(4))
                result = rdr.GetString(4);
            return result;
        }

        private static bool GetIsTerminated(SqlDataReader rdr)
        {
            return rdr.GetBoolean(5);
        }

        private static bool GetIsRemoved(SqlDataReader rdr)
        {
            return rdr.GetBoolean(6);
        }
        #endregion

    }
}
