using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Ionic.Zip;
using PosModels;
using PosModels.Types;
using TemPOS.Helpers;
using TemposLibrary;
using PosModels.Helpers;
using PosModels.Managers;
using System.Windows;

namespace TemPOS.Networking
{
    public static class Updater
    {
        #region Licensed Access Only / Static Initializer
        static Updater()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(Updater).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to link to this application");
            }
#endif
#if !DEMO
            // Initialize
            string rootDirectory =
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                Path.DirectorySeparatorChar + "TemPOS Update";
            UpdateDirectoryPath = rootDirectory + Path.DirectorySeparatorChar + "Update";
            ZipPath = rootDirectory + Path.DirectorySeparatorChar + "update.zip";
            Username = LocalSetting.CompanyName;
            Password = LocalSetting.ApplicationSerialNumber;
            if (!Directory.Exists(rootDirectory))
                Directory.CreateDirectory(rootDirectory);
#endif
        }
        #endregion

#if !DEMO
        private static Encoding ZipEncoding = Encoding.Unicode;

        [Obfuscation(Exclude = true)]
        public static event EventHandler Connected;
        [Obfuscation(Exclude = true)]
        public static event EventHandler ConnectFailed;
        [Obfuscation(Exclude = true)]
        public static event EventHandler Disconnected;
        [Obfuscation(Exclude = true)]
        public static event EventHandler Authenticated;
        [Obfuscation(Exclude = true)]
        public static event EventHandler UpdateReceived;
        [Obfuscation(Exclude = true)]
        public static event EventHandler ReceivedVersion;
        [Obfuscation(Exclude = true)]
        public static event EventHandler ReceivedFileBlock;
        
        [Obfuscation(Exclude = true)]
        public static event TextEventHandler ZipExtractionFailed;
        [Obfuscation(Exclude = true)]
        public static event TextEventHandler Debug;

        public static string UpdateDirectoryPath
        {
            get;
            private set;
        }

        public static string ZipPath
        {
            get;
            private set;
        }

        public static string Username
        {
            get;
            set;
        }

        public static string Password
        {
            get;
            set;
        }

        public static void StartVersionCheck()
        {
            Srp6ClientSocket client = new Srp6ClientSocket(Username, Password, ZipPath);
            InitializeClientSocket(client);
            ThreadPool.QueueUserWorkItem(ClientStartVersionCheckThread, client);
        }

        public static void StartCrashReport(Exception exception)
        {
            Srp6ClientSocket client = new Srp6ClientSocket(Username, Password, exception);
            InitializeClientSocket(client);
            ThreadPool.QueueUserWorkItem(ClientCrashStartThread, client);
        }

        public static void StartUpdate()
        {
            Srp6ClientSocket client = new Srp6ClientSocket(Username, Password, ZipPath);
            InitializeClientSocket(client);
            ThreadPool.QueueUserWorkItem(ClientStartThread, client);
        }

        private static void InitializeClientSocket(Srp6ClientSocket client)
        {
            client.Connected += new EventHandler(Client_Connected);
            client.ConnectFailed += new EventHandler(Client_ConnectFailed);
            client.Disconnected += new EventHandler(Client_Disconnected);
            client.Authenticated += new EventHandler(Client_Authenticated);
            client.UpdateReceived += new EventHandler(Client_UpdateReceived);
            client.ReceivedFileBlock += new EventHandler(Client_ReceivedFileBlock);
            client.ReceivedVersion += new EventHandler(Client_ReceivedVersion);
            client.Debug += new TextEventHandler(client_Debug);
        }

        private static void ClientStartThread(object clientObject)
        {
            Srp6ClientSocket client = clientObject as Srp6ClientSocket;
            client.Start();
        }

        private static void ClientCrashStartThread(object clientObject)
        {
            Srp6ClientSocket client = clientObject as Srp6ClientSocket;
            client.SendCrashReport();
        }

        private static void ClientStartVersionCheckThread(object clientObject)
        {
            Srp6ClientSocket client = clientObject as Srp6ClientSocket;
            client.BeginVersionCheck();
        }

        private static void client_Debug(object sender, TextEventArgs args)
        {
            if (Debug != null)
                Debug.Invoke(sender, args);
        }

        private static void Client_Connected(object sender, EventArgs e)
        {
            if (Connected != null)
                Connected.Invoke(sender, e);
        }

        private static void Client_ConnectFailed(object sender, EventArgs e)
        {
            if (ConnectFailed != null)
                ConnectFailed.Invoke(sender, e);
        }

        private static void Client_Disconnected(object sender, EventArgs e)
        {
            if (Disconnected != null)
                Disconnected.Invoke(sender, e);
        }

        private static void Client_Authenticated(object sender, EventArgs e)
        {
            if (Authenticated != null)
                Authenticated.Invoke(sender, e);
        }

        private static void Client_ReceivedVersion(object sender, EventArgs e)
        {
            if (ReceivedVersion != null)
                ReceivedVersion.Invoke(sender, e);
        }

        private static void Client_ReceivedFileBlock(object sender, EventArgs e)
        {
            if (ReceivedFileBlock != null)
                ReceivedFileBlock.Invoke(sender, e);
        }

        private static void Client_UpdateReceived(object sender, EventArgs e)
        {
            if (UpdateReceived != null)
                UpdateReceived.Invoke(sender, e);
            BeginUpdate();
        }

        private static void BeginUpdate()
        {
            if (!File.Exists(ZipPath))
                return;
            ZipFile zip = new ZipFile(ZipPath, ZipEncoding);
            zip.UseZip64WhenSaving = Zip64Option.Always;
            zip.UseUnicodeAsNecessary = true;
            zip.CaseSensitiveRetrieval = true;
            zip.FullScan = true;
            zip.ZipErrorAction = ZipErrorAction.Throw;
            RemoveUpdateDirectory();
            Directory.CreateDirectory(UpdateDirectoryPath);
            try
            {
                zip.ExtractAll(UpdateDirectoryPath);
            }
            catch (Exception ex)
            {
                if (ZipExtractionFailed != null)
                    ZipExtractionFailed.Invoke(zip, new TextEventArgs(ex.Message +
                        Environment.NewLine + ex.StackTrace));
                return;
            }

            // Start the TemposUpdateInstaller.exe program
            int currentProcessId = Process.GetCurrentProcess().Id;
            string installerExePath = UpdateDirectoryPath + @"\TemposUpdateInstaller.exe";
            if (File.Exists(installerExePath))
            {
                // Start the installer
                if (VistaSecurity.StartProgram(installerExePath, "\"" +
                    currentProcessId.ToString() + "\" \"" +
                    VistaSecurity.GetExecutablePath() + "\"", false, false))
                {
                    // Remove the Zip File
                    RemoveZipFile(zip);

                    // Shutdown this application
                    App.SwitchToDefaultDesktopOnClose = false; 
                    App.ShutdownApplication(false);
                }
            }
            else
            {
                // Remove the Zip File
                RemoveZipFile(zip);
                if (ZipExtractionFailed != null)
                    ZipExtractionFailed.Invoke(zip,
                        new TextEventArgs("Could not find installer"));
            }
        }

        public static void RemoveUpdateDirectory()
        {
            if (Directory.Exists(UpdateDirectoryPath))
                Directory.Delete(UpdateDirectoryPath, true);
        }

        private static void RemoveZipFile(ZipFile zip)
        {
            zip.Dispose();
            File.Delete(ZipPath);
        }
#endif
        public static bool InstallSQLAssembly()
        {
            string databaseName = LocalSetting.DatabaseServerDatabaseName;
            string assemblyPath =
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                + @"\TemposProcs.dll";
            
            if (!ServiceHelper.IsSqlServiceRunningLocally || !File.Exists(assemblyPath))
                return false;

            try
            {
                return DataModelBase.InstallAssembly(assemblyPath, databaseName, "PointOfSale", true);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex.Message + Environment.NewLine + ex.StackTrace);
                Logger.CloseLog(); 
                return false;
            }
        }
    }
}
