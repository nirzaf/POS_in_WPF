using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Collections;
using System.Threading;
using TemposLibrary;

namespace TemposClientAdministration.Helpers
{
    public class Srp6ServerSocket
    {
        // The modulus is a safe prime
        public const String Modulus = "20E176988FD33DE7AE0D296BF805A49F3F45B92FB59036DCC9F0624B89B2DB67";
        public const String UpdateFileLocation = @"D:\Viipe.com\PointOfSale\update.zip";
        public const String TemposVersionString = "1.0." + TemposBuildInfo.Revision;

        private SRP6 srpServer = null;
        private ArrayList clients = new ArrayList();
        private AsynchronousSocket listenerSocket;
        private MemoryStream receiveStream = null;
        private int crashReportSize = 0;
        private int crashReportEncryptedSize = 0;
        private int bytesReceived = 0;
        private bool isReceivingCrashReport = false;
        private bool isRequestingCrashReportUpload = false;

        public event EventHandler ServerStarted;
        public event EventHandler ServerStopped;
        public event AuthenticationRequestEventHandler AuthenticationRequest;
        public event ExceptionEventHandler ReceivedCrashException;
        public event TextEventHandler Debug;

        public byte[] IdentityHash
        {
            get { return srpServer.IdentityHash; }
        }

        public bool IsListening
        {
            get;
            private set;
        }

        public int ListeningPort
        {
            get;
            set;
        }

        public Srp6ServerSocket()
        {
            IsListening = false;
            ListeningPort = 43333;
        }

        public void Start()
        {
            if (!File.Exists(UpdateFileLocation))
            {
                OnDebug("Update file \"" + UpdateFileLocation + "\" doesn't exist!");
            }
            else if (!IsListening)
            {
                IsListening = true;
                if (listenerSocket == null)
                {
                    listenerSocket = new AsynchronousSocket();
                    listenerSocket.Debug += new TextEventHandler(listenerSocket_Debug);
                    listenerSocket.ClientConnected += new EventHandler(listenerSocket_ClientConnected);
                    listenerSocket.IsListeningChanged += new EventHandler(listenerSocket_IsListeningChanged);
                }
                listenerSocket.Bind(IPAddress.Any, ListeningPort);
                listenerSocket.StartListening(); // Blocks until StopListening
            }
            IsListening = false;
            if (ServerStopped != null) // Thread is about to exit
                ServerStopped.Invoke(this, new EventArgs());
        }

        void listenerSocket_IsListeningChanged(object sender, EventArgs e)
        {
            AsynchronousSocket socket = sender as AsynchronousSocket;
            if ((socket.IsListening) && (ServerStarted != null))
                ServerStarted.Invoke(this, new EventArgs());
        }

        void listenerSocket_Debug(object sender, TextEventArgs args)
        {
            OnDebug(args.Text);
        }

        public void Stop()
        {
            // Give 30 seconds for clients to finish downloading
            int retriesRemaining = 60;
            while ((clients.Count > 0) && (retriesRemaining > 0))
            {
                Thread.Sleep(500);
                retriesRemaining--;
            }
            foreach (object sock in clients)
            {
                AsynchronousSocket socket = sock as AsynchronousSocket;
                socket.Disconnect();
            }
            clients.Clear();
            listenerSocket.StopListening();
        }

        void listenerSocket_ClientConnected(object sender, EventArgs e)
        {
            AsynchronousSocket socket = sender as AsynchronousSocket;
            IPEndPoint endPoint = socket.TcpSocket.RemoteEndPoint as IPEndPoint;
            OnDebug("Client Connected: (" + endPoint.Address + ")");
            if (!IsClientPermitted(socket))
            {
                socket.Disconnect();
                socket.Close();
                return;
            }
            StartAuthenicationTimeOut(socket);
            clients.Add(socket);
            socket.Disconnected += new EventHandler(clientSocket_Disconnected);
            socket.ReceiveCompleted += new EventHandler(clientSocket_ReceiveCompleted);
        }

        private void StartAuthenicationTimeOut(AsynchronousSocket socket)
        {
            ParameterizedThreadStart threadStart =
                new ParameterizedThreadStart(DisconnectTimeoutStart);
            Thread thread = new Thread(threadStart);
            thread.Start(socket);
        }

        private void DisconnectTimeoutStart(object obj)
        {
            AsynchronousSocket socket = obj as AsynchronousSocket;
            DateTime startTime = DateTime.Now;
            DateTime endTime = startTime + new TimeSpan(0, 0, 15);
            while (DateTime.Now < endTime)
            {
                Thread.Sleep(1000);
            }
            if (socket.Step < 2) // Not authenticated
            {
                socket.Disconnect();
                socket.Close();
            }
        }

        private bool IsClientPermitted(AsynchronousSocket newSocket)
        {
            bool result = true;
            int maxConnections = 2;
            IPEndPoint newEndPoint = newSocket.TcpSocket.RemoteEndPoint as IPEndPoint;
            foreach (object sock in clients)
            {
                AsynchronousSocket socket = sock as AsynchronousSocket;
                IPEndPoint endPoint = socket.TcpSocket.RemoteEndPoint as IPEndPoint;
                if (newEndPoint.Address.ToString() == endPoint.Address.ToString())
                    maxConnections--;
                if (maxConnections < 1)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        void clientSocket_Disconnected(object sender, EventArgs e)
        {
            AsynchronousSocket socket = sender as AsynchronousSocket;
            if (clients.Contains(socket))
                clients.Remove(socket);
            OnDebug("Client Disconnected (" + socket.TcpSocket.RemoteEndPoint.ToString() + ")");
        }

        void clientSocket_ReceiveCompleted(object sender, EventArgs e)
        {
            AsynchronousSocket socket = sender as AsynchronousSocket;
            if (socket.Step == 0)
                InitializeSrpServer(socket);
            else if (socket.Step == 1)
                ProcessClientsPublicKey(socket);
            else if (socket.Step == 2)
                ProcessClientMessage(socket);
        }

        private void ProcessClientMessage(AsynchronousSocket socket)
        {
            // Get the encrypted byte array from the client
            byte[] encryptedMessage = SRP6.SubArray(
                socket.ReceiveBuffer, 0,
                socket.ReceiveBufferLength);
            OnDebug("Received encrypted client message, length = " +
                socket.ReceiveBufferLength);

            // Start file receive
            if (isReceivingCrashReport && !socket.IsReceivingFile)
            {
                OnDebug("Starting receive of crash report");
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
            byte[] message = srpServer.Decrypt(encryptedMessage);
            ProcessDecryptedClientMessages(socket, message);
        }

        private void ProcessFileReceive(AsynchronousSocket socket, byte[] encryptedMessage)
        {
            socket.WriteFileReceive(encryptedMessage);
            bytesReceived += socket.ReceiveBufferSize;
            //if (ReceivedFileBlock != null)
            //    ReceivedFileBlock.Invoke(this, new EventArgs());
            if (socket.FileBytesReceived >= crashReportEncryptedSize)
            {
                isReceivingCrashReport = false;
                byte[] encryptedData = receiveStream.ToArray();
                byte[] decryptedData = srpServer.Decrypt(encryptedData);
                
                Exception ex = (Exception)decryptedData.DeserializeObject();
                if (ReceivedCrashException != null)
                    ReceivedCrashException.Invoke(this, new ExceptionEventArgs(ex));

                socket.EndFileReceive();
                socket.Disconnect();
            }
        }

        private void ProcessDecryptedClientMessages(AsynchronousSocket socket, byte[] message)
        {
            string command = ASCIIEncoding.ASCII.GetString(message, 0, (message.Length > 128 ? 128 : message.Length));
            OnDebug("Client Message \"" + SRP6.ArrayToString(message, 0, message.Length) + "\"");
            
            if (command.StartsWith("SUBMIT_CRASH")) // Client wants to send us a crash report
            {
                OnDebug("Beginning submitting Crash Report");
                string strMessage = SRP6.ArrayToString(message, 13, message.Length - 13);
                string[] strTokens = strMessage.Split(' ');
                try
                {
                    crashReportSize = Convert.ToInt32(strTokens[0]);
                }
                catch { }
                try
                {
                    crashReportEncryptedSize = Convert.ToInt32(strTokens[1]);
                }
                catch { }

                if ((crashReportSize > 0) && (crashReportEncryptedSize >= crashReportSize))
                {
                    isReceivingCrashReport = true;
                    // We know what to expect now, send "PROCEED" message.
                    srpServer.EncryptAndSend(socket, "PROCEED");
                }
                else
                {
                    OnDebug(
                        "Failed to get valid transfer information from the client");
                }
            }
            else if (command.StartsWith("VERSION"))
            {
                srpServer.EncryptAndSend(socket, TemposVersionString);
            }
            else if (command.StartsWith("SEND"))
            {
                socket.SendStream = new FileStream(UpdateFileLocation, FileMode.Open, FileAccess.Read);
                socket.EncryptedSendStream = srpServer.Encrypt(socket.SendStream);
                srpServer.EncryptAndSend(socket, socket.SendStream.Length.ToString() + " " +
                    socket.EncryptedSendStream.Length.ToString());
            }
            else if (command.StartsWith("CONTINUE"))
            {
                if (socket.EncryptedSendStream != null)
                {
                    socket.SendFile(socket.EncryptedSendStream);
                }
            }
            else
            {
                socket.Disconnect();
                OnDebug("Unhandled: " + command);
            }
        }

        private void ProcessCrashReport(AsynchronousSocket socket, byte[] message)
        {
            Exception ex = (Exception)message.DeserializeObject();
            OnDebug(ex.GetType().ToString() + Environment.NewLine +
                ex.Message + Environment.NewLine + ex.StackTrace);
        }

        protected bool OnAuthenticationRequest(AsynchronousSocket socket, byte[] identityHash)
        {
            bool allow = false;
            AuthenticationRequestEventHandler handler = AuthenticationRequest;
            
            if (handler != null)
            {
                AuthenticationRequestEventArgs args =
                    new AuthenticationRequestEventArgs(identityHash);
                foreach (AuthenticationRequestEventHandler tmp in
                    handler.GetInvocationList())
                {
                    tmp(this, args);
                    if (args.Allow)
                    {
                        allow = true;
                        break;
                    }
                }
            }
            
            if (!allow)
            {
                socket.Disconnect();
                return false;
            }

            return true;
        }

        private void InitializeSrpServer(AsynchronousSocket socket)
        {
            byte[] identityHash = SRP6.SubArray(socket.ReceiveBuffer, 0, socket.ReceiveBufferLength);
            OnDebug("IdentityHash = " + SRP6.BytesToHex(identityHash));

            // Verify that the server knows this identity before proceeding
            if (!OnAuthenticationRequest(socket, identityHash))
                return;

            // Server generates (and sends to client) public-key, scrambler, and salt
            srpServer = new SRP6(identityHash, Modulus, 0x07, 256, 128);
            socket.Step++;

            byte[] salt = srpServer.Salt.ToUnsignedByteArray();
            byte[] pubKey = srpServer.PublicKey.ToUnsignedByteArray();
            byte[] scrambler = srpServer.Scrambler.ToUnsignedByteArray();
            byte[] replyPacket = SRP6.ConcatArray(salt, pubKey, scrambler);

            OnDebug("Server->Salt, PublicKey, and Scrambler");
            OnDebug("Salt = " + srpServer.GetSalt());
            OnDebug("PublicKey = " + srpServer.GetPublicKey());
            OnDebug("Scrambler = " + srpServer.GetScrambler());
            socket.Send(replyPacket, 0, replyPacket.Length);

        }

        private void ProcessClientsPublicKey(AsynchronousSocket socket)
        {
            byte[] pubKey = SRP6.SubArray(socket.ReceiveBuffer, 0, socket.ReceiveBufferLength);

            string pubKeyString = SRP6.BytesToHex(pubKey);
            srpServer.SetSessionKey(pubKeyString);

            socket.SessionKey = srpServer.SessionKey.ToUnsignedByteArray();
            OnDebug("Client's PublicKey = " + pubKeyString);
            OnDebug("SessionKey = " + srpServer.GetSessionKey());
            socket.Step++;

            // From this point on, both client and server use encrypted communications
            OnDebug("Sending READY to client");
            socket.Send("READY");
        }

        private void OnDebug(string message)
        {
            if (Debug != null)
                Debug.Invoke(this, new TextEventArgs(message));
        }

    }
}
