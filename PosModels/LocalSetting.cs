using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using PosModels.Helpers;
using TemposLibrary;

namespace PosModels
{
    [Serializable]
    public sealed class LocalSetting
    {
        #region Licensed Access Only
        static LocalSetting()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(LocalSetting).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosModels.dll");
            }
#endif
#if !DEMO
            // Remove the demo config file now that customer has the release version
            string filePath = GetFilePath(true);
            if (File.Exists(filePath))
                File.Delete(filePath);
#endif
        }
        #endregion

        #region SettingCollection class
        [Serializable]
        public class SettingCollection<T>
        {
            private readonly Dictionary<string, T> _settings = new Dictionary<string, T>();

            public T this[string settingName]
            {
                get
                {
                    return _settings.ContainsKey(settingName) ?
                        _settings[settingName] : default(T);
                }
                set
                {
                    if (_settings.ContainsKey(settingName))
                    {
                        if (Equals(value, default(T)))
                            _settings.Remove(settingName);
                        else
                            _settings[settingName] = value;
                    }
                    else
                        _settings.Add(settingName, value);
                }
            }
        }
        #endregion

        #region Instance Model
        #region Fields
        private string _databaseServerName;
        private string _databaseServerDatabaseName;
        private string _databaseServerLoginName;
        private string _databaseServerPassword;
        private string _companyName;
        private string _applicationSerialNumber;
        private readonly SettingCollection<String> _stringSettings =
            new SettingCollection<string>();
        private readonly SettingCollection<Int32> _int32Settings =
            new SettingCollection<Int32>();
        private readonly SettingCollection<Double> _doubleSettings =
            new SettingCollection<Double>();
        private readonly SettingCollection<Int16> _int16Settings =
            new SettingCollection<Int16>();
        private readonly SettingCollection<Boolean> _boolSettings =
            new SettingCollection<Boolean>();
#if !DEMO
        private byte[] _fingerprint;
#endif
        #endregion

        #region Value Properties
        public SettingCollection<String> String
        {
            get { return _stringSettings; }
        }

        public SettingCollection<Int32> Int32
        {
            get { return _int32Settings; }
        }

        public SettingCollection<Int16> Int16
        {
            get { return _int16Settings; }
        }

        public SettingCollection<Double> Double
        {
            get { return _doubleSettings; }
        }

        public SettingCollection<Boolean> Boolean
        {
            get { return _boolSettings; }
        }
        #endregion

        private LocalSetting()
        {
            _path = GetFilePath();

            _databaseServerName = "(local)";
            _databaseServerDatabaseName = "TemPOS";
            _databaseServerLoginName = "";
            _databaseServerPassword = "";
            _companyName = "";
            _applicationSerialNumber = "";
#if !DEMO
            _fingerprint = null;
#endif
#if DEBUG
            _stringSettings["IsAuthorized"] = "Yes";
            _companyName = "Tempos Software";
            _applicationSerialNumber = "(None)";
#endif
        }

#if DEMO
        private static string GetFilePath(bool demoFile = true)
#else
        private static string GetFilePath(bool demoFile = false)
#endif
        {
            string rootDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                //Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                //Path.DirectorySeparatorChar + "TemPOS";
            if (rootDirectory != null && !Directory.Exists(rootDirectory))
                Directory.CreateDirectory(rootDirectory);
            if (!demoFile)
                return rootDirectory + @"\TemPOS.data";
            return rootDirectory + @"\TemPOS-Demo.data";
        }
        #endregion

        #region Static
        private static LocalSetting _singleton = new LocalSetting();
        private static string _path;

        public static string ConnectionString
        {
            get
            {
                /*
                if (string.IsNullOrEmpty(DatabaseServerName) ||
                    string.IsNullOrEmpty(DatabaseServerDatabaseName))
                    return null;

                if (!string.IsNullOrEmpty(DatabaseServerLoginName) &&
                    !string.IsNullOrEmpty(DatabaseServerPassword))
                    return "Data Source=" + DatabaseServerName +
                        ";Initial Catalog=" + DatabaseServerDatabaseName +
                        ";User ID=" + DatabaseServerLoginName +
                        ";Password=" + DatabaseServerPassword + ";";
                */
                return @"Server=" + DatabaseServerName +
                       ";Database=" + DatabaseServerDatabaseName +
                       ";Trusted_Connection=Yes;";
            }
        }

        public static string ConnectionStringNoDatabase
        {
            get
            {
                if (string.IsNullOrEmpty(DatabaseServerName) ||
                    string.IsNullOrEmpty(DatabaseServerDatabaseName))
                    return null;

                if (!string.IsNullOrEmpty(DatabaseServerLoginName) &&
                    !string.IsNullOrEmpty(DatabaseServerPassword))
                    return "Data Source=" + DatabaseServerName +
                        ";User ID=" + DatabaseServerLoginName +
                        ";Password=" + DatabaseServerPassword + ";";

                return @"Server=" + DatabaseServerName +
                       ";Trusted_Connection=Yes;";
            }
        }

        public static bool ConnectionStringIsUsingSqlAuthentication
        {
            get
            {
                return (!string.IsNullOrWhiteSpace(DatabaseServerLoginName) ||
                    !string.IsNullOrWhiteSpace(DatabaseServerPassword));
            }
        }

        public static string DatabaseServerName
        {
            get { return _singleton._databaseServerName; }
            set { _singleton._databaseServerName = value; }
        }

        public static string DatabaseServerDatabaseName
        {
            get { return _singleton._databaseServerDatabaseName; }
            set { _singleton._databaseServerDatabaseName = value; }
        }

        public static string DatabaseServerLoginName
        {
            get { return _singleton._databaseServerLoginName; }
            set { _singleton._databaseServerLoginName = value; }
        }

        public static string DatabaseServerPassword
        {
            get { return _singleton._databaseServerPassword; }
            set { _singleton._databaseServerPassword = value; }
        }

        public static string CompanyName
        {
            get { return _singleton._companyName; }
            set { _singleton._companyName = value; }
        }

        public static string ApplicationSerialNumber
        {
            get { return _singleton._applicationSerialNumber; }
            set { _singleton._applicationSerialNumber = value; }
        }

        public static LocalSetting Values
        {
            get { return _singleton; }
        }
#if !DEMO
        public static byte[] ComputerFingerprint
        {
            get { return _singleton._fingerprint; }
            set { _singleton._fingerprint = value; }
        }
#endif
        public static bool FileExists
        {
            get { return File.Exists(_path); }
        }

        private static void Deserialize(byte[] bytes)
        {
            var localSetting = bytes.DeserializeObject() as LocalSetting;
            if (localSetting != null)
                _singleton = localSetting;
        }

        private static byte[] Serialize()
        {
            return _singleton.SerializeObject();
        }

        public static bool Initialize()
        {
            // If a settings file exists, deserialize it.
            if (File.Exists(_path))
            {
                bool readFail = false;
                var stream = new FileStream(_path, FileMode.Open, FileAccess.Read);
                var reader = new BinaryReader(stream);
                try
                {
                    byte[] encryptedBytes = reader.ReadBytes((int)stream.Length);
#if !DEMO
                    byte[] decryptedBytes = AESHelper.Decrypt(encryptedBytes, "!" + Fingerprint.Value + "!");
                    Deserialize(decryptedBytes);
#else
                    Deserialize(encryptedBytes);
#endif
                }
                catch { readFail = true; }
                reader.Close();
                stream.Close();

                // If the settings file is bad or license isn't valid
#if !DEMO
                if (readFail || !LocalSettingFileIsOriginal())
#else
                if (readFail)
#endif
                {
                    Reset();
                    return false;
                }
            }
            return true;
        }

        public static void Reset()
        {
            // Delete the bad settings file
            File.Delete(_path);

            // Re-initialize the singleton
            _singleton = new LocalSetting();

            // Recreate
#if !DEMO
            ComputerFingerprint = Fingerprint.Value;
#endif
            Update();
        }

#if !DEMO
        private static bool LocalSettingFileIsOriginal()
        {
            if (ComputerFingerprint == null)
            {
                ComputerFingerprint = Fingerprint.Value;
                Update();
                return true;
            }
            IStructuralEquatable eqa1 = ComputerFingerprint;
            return eqa1.Equals(Fingerprint.Value, StructuralComparisons.StructuralEqualityComparer);
        }
#endif
        public static void Update()
        {
            byte[] decryptedBytes = Serialize();
#if !DEMO
            byte[] encryptedBytes = AESHelper.Encrypt(decryptedBytes, "!" + Fingerprint.Value + "!");
#else
            byte[] encryptedBytes = decryptedBytes;
#endif
            var stream = new FileStream(GetFilePath(), FileMode.OpenOrCreate, FileAccess.Write);
            var writer = new BinaryWriter(stream);
            writer.Write(encryptedBytes);
            writer.Flush();
            writer.Close();
            stream.Close();
        }
        #endregion
    }
}
