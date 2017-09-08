#if !DEMO
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using PosModels;
using TemposLibrary;

namespace PointOfSale.Networking
{
    public class Srp6ClientSocket
    {
        #region Licensed Access Only
        static Srp6ClientSocket()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(BroadcastClientSocket).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to link to this application");
            }
#endif
        }
        #endregion

        // The modulus is a safe prime
        public const String Modulus = "20E176988FD33DE7AE0D296BF805A49F3F45B92FB59036DCC9F0624B89B2DB67";

        private int step = 0;
        private SRP6 srpClient = null;
        private RijndaelManaged aes = new RijndaelManaged();
        private bool isSendingCrashReport = false;
        private bool isCheckingVersion = false;
        private bool isRequestingSend = false;
        private bool isReceivingUpdate = false;
        private bool wasUpdateCompleted = false;
        private int encryptedFileSize = 0;
        private MemoryStream receiveStream = null;
        private byte[] IdentityHash = null;
        private bool checkVersionOnly = false;
        private bool crashReportOnly = false;
        private bool noop = false;

        [Obfuscation(Exclude = true)]
        public event EventHandler Connected;
        [Obfuscation(Exclude = true)]
        public event EventHandler ConnectFailed;
        [Obfuscation(Exclude = true)]
        public event EventHandler Disconnected;
        [Obfuscation(Exclude = true)]
        public event EventHandler Authenticated;
        [Obfuscation(Exclude = true)]
        public event EventHandler UpdateReceived;
        [Obfuscation(Exclude = true)]
        public event EventHandler CrashReportCompleted;
        [Obfuscation(Exclude = true)]
        public event EventHandler CrashReportFailed;
        [Obfuscation(Exclude = true)]
        public event EventHandler ReceivedVersion;
        [Obfuscation(Exclude = true)]
        public event EventHandler ReceivedFileBlock;

        [Obfuscation(Exclude = true)]
        public event TextEventHandler Debug;

        public bool IsConnecting
        {
            get;
            private set;
        }

        public AsynchronousSocket Socket
        {
            get;
            private set;
        }

        public int UpdateFileSize
        {
            get;
            private set;
        }

        public int BytesReceived
        {
            get;
            private set;
        }

        public String UpdateFileLocation
        {
            get;
            private set;
        }

        public static string NewestUpdateVersion
        {
            get;
            private set;
        }

        public Exception CrashException
        {
            get;
            set;
        }

        /// <summary>
        /// Used to check for a valid license
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public Srp6ClientSocket(string username, string password)
        {
            InitializeDefaultProperties();
            IdentityHash = SRP6.GenerateIdentityHash(username, password);
            noop = true;
        }

        /// <summary>
        /// Socket constructor for crash reports
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="ex"></param>
        public Srp6ClientSocket(string username, string password, Exception ex)
        {
            InitializeDefaultProperties();
            IdentityHash = SRP6.GenerateIdentityHash(username, password);
            CrashException = ex;
            UpdateFileLocation = null;
        }

        /// <summary>
        /// Socket constructor for download updates
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="updateFileLocation"></param>
        public Srp6ClientSocket(string username, string password, string updateFileLocation)
        {
            InitializeDefaultProperties();
            IdentityHash = SRP6.GenerateIdentityHash(username, password);
            UpdateFileLocation = updateFileLocation;
        }

        public void Start()
        {
            InitializeDefaultProperties();
            isSendingCrashReport = false;
            isCheckingVersion = false;
            Socket = new AsynchronousSocket();
            Socket.Connected += new EventHandler(socket_Connected);
            Socket.ConnectFailed += new EventHandler(socket_ConnectFailed);
            Socket.Disconnected += new EventHandler(socket_Disconnected);
            Socket.SendCompleted += new EventHandler(socket_SendCompleted);
            Socket.SendFailed += new EventHandler(socket_SendFailed);
            Socket.ReceiveCompleted += new EventHandler(socket_ReceiveCompleted);
            int port = -1;
            try
            {
                port = Convert.ToInt32(LocalSetting.Values.String["UpdateServerPort"]);
            }
            catch
            {
                return;
            }
            Socket.Connect(
                LocalSetting.Values.String["UpdateServer"], port, 10000); // Blocks until disconnected
        }

        private void InitializeDefaultProperties()
        {
            UpdateFileSize = 0;
            BytesReceived = 0;
            if ((Socket != null) && (Socket.IsConnected))
            {
                Socket.Disconnect();
                Socket.Close();
            }
            Socket = null;
            IsConnecting = false;
            isRequestingSend = false;
            isReceivingUpdate = false;
            wasUpdateCompleted = false;
            encryptedFileSize = 0;
            if (receiveStream != null)
                receiveStream.Dispose();
            receiveStream = null;
        }

        public void BeginVersionCheck()
        {
            checkVersionOnly = true;
            isSendingCrashReport = false;
            Start();
        }

        public void SendCrashReport()
        {
            if (CrashException == null)
                return;
            if (!(CrashException is ISerializable))
            {
                OnDebug("Could not serialize exception type \"" + CrashException.GetType().ToString());
                return;
            }
            crashReportOnly = true;
            isSendingCrashReport = false;
            isReceivingUpdate = false;
            checkVersionOnly = false;
            Start();
        }

        void socket_ConnectFailed(object sender, EventArgs e)
        {
            AsynchronousSocket socket = sender as AsynchronousSocket;
            if (ConnectFailed != null)
                ConnectFailed.Invoke(this, new EventArgs());
        }

        void socket_Connected(object sender, EventArgs e)
        {
            AsynchronousSocket socket = sender as AsynchronousSocket;
            if (Connected != null)
                Connected.Invoke(this, new EventArgs());

            // Send the server my identity hash
            socket.Send(IdentityHash, 0, IdentityHash.Length);
            OnDebug("Client->IdentityHash");
            OnDebug("IndetityHash = " + SRP6.BytesToHex(IdentityHash));
        }

        void socket_ReceiveCompleted(object sender, EventArgs e)
        {
            AsynchronousSocket socket = sender as AsynchronousSocket;
            if (step == 0)
                ServerGreetingReceived(socket);
            else if (step == 1)
                ServerReadyReceived(socket);
            else if (step == 2)
                ProcessServerMessages(socket);
        }

        private void ProcessServerMessages(AsynchronousSocket socket)
        {
            // Get the encrypted byte array from the client
            byte[] encryptedMessage = SRP6.SubArray(
                socket.ReceiveBuffer, 0,
                socket.ReceiveBufferLength);

            // Start file receive
            if (isReceivingUpdate && !socket.IsReceivingFile)
            {
                receiveStream = new MemoryStream();
                socket.BeginFileReceive(receiveStream);
            }

            // FileReceive In-Progress
            if (socket.IsReceivingFile)
            {
                ProcessFileReceive(socket, encryptedMessage);
                return;
            }

            // Get the decrypted message
            byte[] message = srpClient.Decrypt(encryptedMessage);
            ProcessDecryptedServerMessages(socket, message);
        }

        private void ProcessFileReceive(AsynchronousSocket socket, byte[] encryptedMessage)
        {
            socket.WriteFileReceive(encryptedMessage);
            BytesReceived += Socket.ReceiveBufferSize;
            if (ReceivedFileBlock != null)
                ReceivedFileBlock.Invoke(this, new EventArgs());
            if (socket.FileBytesReceived >= encryptedFileSize)
            {
                isReceivingUpdate = false;
                byte[] encryptedData = receiveStream.ToArray();
                byte[] decryptedData = srpClient.Decrypt(encryptedData);
                FileStream fileStream;
                if (!File.Exists(UpdateFileLocation))
                    fileStream = new FileStream(UpdateFileLocation, FileMode.CreateNew, FileAccess.Write);
                else
                    fileStream = new FileStream(UpdateFileLocation, FileMode.Truncate, FileAccess.Write);
                fileStream.Write(decryptedData, 0, decryptedData.Length);
                fileStream.Flush();
                fileStream.Close();
                socket.EndFileReceive();
                FileInfo fileInfo = new FileInfo(UpdateFileLocation);
                if (fileInfo.Length == UpdateFileSize)
                    wasUpdateCompleted = true;
                socket.Disconnect();
            }
        }

        private void ServerReadyReceived(AsynchronousSocket socket)
        {
            byte[] message = SRP6.SubArray(
                socket.ReceiveBuffer, 0,
                socket.ReceiveBufferLength);
            string strMessage = SRP6.ArrayToString(message, 0, message.Length);

            // Server and client are now both ready for encrypted communications
            if (strMessage.Equals("READY"))
            {
                OnDebug("Received \"READY\" from server");
                step++;
                if (Authenticated != null)
                    Authenticated.Invoke(this, new EventArgs());

                if (crashReportOnly)
                {
                    // Request to send a crash report to the server
                    OnDebug("Telling server we want to upload a crash report");
                    try
                    {
                        byte[] serializedObject = CrashException.SerializeObject();
                        socket.SendStream = new MemoryStream(serializedObject);
                        socket.EncryptedSendStream = srpClient.Encrypt(socket.SendStream);
                        srpClient.EncryptAndSend(socket, "SUBMIT_CRASH " +
                            socket.SendStream.Length + " " +
                            socket.EncryptedSendStream.Length);
                    }
                    catch (Exception ex)
                    {
                        OnDebug("Exception: " + ex.Message);
                    }
                }
                else
                {
                    // Request the current version
                    OnDebug("Sending version-check request to server");
                    isCheckingVersion = true;
                    srpClient.EncryptAndSend(socket, "VERSION");
                }
            }
            else
            {
                OnDebug("Authentication Failed");
                socket.Disconnect();
            }
        }

        private void ProcessDecryptedServerMessages(AsynchronousSocket socket, byte[] message)
        {
            // Server is going to tell us the size of the update
            if (isRequestingSend)
            {
                isRequestingSend = false;
                string strMessage = SRP6.ArrayToString(message, 0, message.Length);
                string[] strTokens = strMessage.Split(' ');
                try
                {
                    UpdateFileSize = Convert.ToInt32(strTokens[0]);
                }
                catch { }
                try
                {
                    encryptedFileSize = Convert.ToInt32(strTokens[1]);
                }
                catch { }

                if ((UpdateFileSize > 0) && (encryptedFileSize >= UpdateFileSize))
                {
                    isReceivingUpdate = true;
                    srpClient.EncryptAndSend(socket, "CONTINUE");
                }
                else
                {
                    OnDebug("Bad transfer information sent from the server");
                }
                return;
            }

            {
                string strMessage = SRP6.ArrayToString(message, 0, message.Length);
                OnDebug("Server Message \"" + strMessage + "\"");

                if (strMessage.Equals("PROCEED")) // CrashException != null
                {
                    // Send the CrashException to the server (serialized)
                    OnDebug("Sending crash report to server");
                    srpClient.EncryptAndSend(socket, CrashException.SerializeObject());
                }
            }
            if (isCheckingVersion)
            {
                isCheckingVersion = false;
                string text = SRP6.ArrayToString(message, 0, message.Length);
                OnDebug("Server reports that version is \"" + text + "\"");
                Srp6ClientSocket.NewestUpdateVersion = text;
                if (ReceivedVersion != null)
                    ReceivedVersion.Invoke(this, new EventArgs());
                if (checkVersionOnly)
                {
                    isCheckingVersion = false;
                    socket.Disconnect();
                }
                else
                {
                    isRequestingSend = true;
                    srpClient.EncryptAndSend(socket, "SEND");
                }
            }

        }

        private void ServerGreetingReceived(AsynchronousSocket socket)
        {
            if (socket.ReceiveBufferLength != 80)
            {
                OnDebug("Server did not send a proper greeting");
                socket.Disconnect();
                return;
            }
            string salt = SRP6.BytesToHex(SRP6.SubArray(socket.ReceiveBuffer, 0, 32));
            string pubKey = SRP6.BytesToHex(SRP6.SubArray(socket.ReceiveBuffer, 32, 32));
            string scrambler = SRP6.BytesToHex(SRP6.SubArray(socket.ReceiveBuffer, 64, 16));

            // Setup the SessionKey
            srpClient = new SRP6(IdentityHash, Modulus, 0x07, salt);
            srpClient.SetSessionKey(pubKey, scrambler);
            OnDebug("Salt = " + salt);
            OnDebug("Server's PublicKey = " + pubKey);
            OnDebug("Scrambler = " + scrambler);

            step++;

            // Send my public key to the server
            OnDebug("Client->PublicKey");
            OnDebug("PublicKey = " + srpClient.GetPublicKey());
            byte[] reply = srpClient.PublicKey.ToUnsignedByteArray();
            socket.Send(reply, 0, reply.Length);
            OnDebug("SessionKey = " + srpClient.GetSessionKey());
        }

        void socket_Disconnected(object sender, EventArgs e)
        {
            AsynchronousSocket socket = sender as AsynchronousSocket;
            if (socket.IsReceivingFile)
                socket.EndFileReceive();
            if (Disconnected != null)
                Disconnected.Invoke(this, new EventArgs());
            if (wasUpdateCompleted && (UpdateReceived != null))
                UpdateReceived.Invoke(this, new EventArgs());
        }

        void socket_SendCompleted(object sender, EventArgs e)
        {
            AsynchronousSocket socket = sender as AsynchronousSocket;
            OnDebug("Send Completed");
        }

        void socket_SendFailed(object sender, EventArgs e)
        {
            AsynchronousSocket socket = sender as AsynchronousSocket;
            OnDebug("Send Failed");
        }

        public void OnDebug(string message)
        {
            if (Debug != null)
                Debug.Invoke(this, new TextEventArgs(message));
        }
    }
}
#endif