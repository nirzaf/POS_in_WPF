using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace TemposLibrary
{
    public class AsynchronousSocket
    {
        #region Licensed Access Only
        static AsynchronousSocket()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(AsynchronousSocket).Assembly.GetName().GetPublicKeyToken(),
                 System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use TemposLibrary.dll");
            }
#endif
        }
        #endregion

        #region Fields
        private Socket socket = null;
        private bool isListening = false;

        // ManualResetEvent instances signal completion.
        private ManualResetEvent connectDone = new ManualResetEvent(false);
        private ManualResetEvent connectExit = new ManualResetEvent(false);
        private ManualResetEvent sendDone = new ManualResetEvent(false);
        private ManualResetEvent receiveDone = new ManualResetEvent(false);
        private ManualResetEvent disconnectDone = new ManualResetEvent(false);
        private ManualResetEvent listenClientDone = new ManualResetEvent(false);
        #endregion

        #region Events
        // Connect Events
        public event EventHandler Connected;
        public event EventHandler ConnectFailed;

        // Disconnect Event
        public event EventHandler Disconnected;

        // Send Events
        public event EventHandler SendCompleted;
        public event EventHandler SendFailed;

        // Receive Event
        public event EventHandler ReceiveCompleted;
        public event EventHandler ReceiveFileCompleted;

        // Listening Events
        public event EventHandler ClientConnected;
        public event EventHandler IsListeningChanged;

        // Debug
        public event TextEventHandler Debug;
        #endregion

        #region Properties
        public bool IsConnected
        {
            get;
            private set;
        }

        public bool IsListening
        {
            get { return isListening; }
            private set
            {
                if (value != isListening)
                {
                    isListening = value;
                    if (IsListeningChanged != null)                    
                        IsListeningChanged.Invoke(this, new EventArgs());
                }

            }
        }

        public int BytesSent
        {
            get;
            private set;
        }

        public bool IsReceivingFile
        {
            get;
            private set;
        }

        public int FileBytesReceived
        {
            get;
            private set;
        }

        public Stream ReceiveStream
        {
            get;
            private set;
        }

        public byte[] SessionKey
        {
            get;
            set;
        }

        public Stream SendStream
        {
            get;
            set;
        }

        public Stream EncryptedSendStream
        {
            get;
            set;
        }

        /// <summary>
        /// Used to step for the server's clients
        /// </summary>
        public int Step
        {
            get;
            set;
        }

        /// <summary>
        /// Set for client connection sockets
        /// </summary>
        public AsynchronousSocket ListenerSocket
        {
            get;
            private set;
        }

        public byte[] ReceiveBuffer
        {
            get;
            private set;
        }

        public int ReceiveBufferSize
        {
            get
            {
                return ReceiveBuffer.Length;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException();
                ReceiveBuffer = new byte[value];
            }
        }

        public Socket TcpSocket
        {
            get { return socket; }
        }


        public int ReceiveBufferLength
        {
            get;
            private set;
        }
        #endregion

        #region Constructors
        public AsynchronousSocket()
        {
            InitializeProperties();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //socket.LingerState = new LingerOption(false, 0);
            socket.SetSocketOption(SocketOptionLevel.Socket,
                SocketOptionName.DontLinger, true);
        }

        public AsynchronousSocket(AddressFamily addressFamily)
        {
            InitializeProperties();
            socket = new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);
            //socket.LingerState = new LingerOption(false, 0);
            socket.SetSocketOption(SocketOptionLevel.Socket,
                SocketOptionName.DontLinger, true);
        }

        public AsynchronousSocket(SocketInformation socketInformation)
        {
            InitializeProperties();
            socket = new Socket(socketInformation);
            //socket.LingerState = new LingerOption(false, 0);
            socket.SetSocketOption(SocketOptionLevel.Socket,
                SocketOptionName.DontLinger, true);
        }

        public AsynchronousSocket(AddressFamily addressFamily, SocketType socketType,
            ProtocolType protocolType)
        {
            InitializeProperties();
            socket = new Socket(addressFamily, socketType, protocolType);
            //socket.LingerState = new LingerOption(false, 0);
            socket.SetSocketOption(SocketOptionLevel.Socket,
                SocketOptionName.DontLinger, true);
        }

        public AsynchronousSocket(AsynchronousSocket listenerSocket, Socket clientSocket)
        {
            if (clientSocket == null)
                throw new ArgumentNullException();
            InitializeProperties();
            ListenerSocket = listenerSocket;
            IsConnected = clientSocket.Connected; // Bad: trusting that this is correct
            this.socket = clientSocket;
            //clientSocket.LingerState = new LingerOption(false, 0);
            clientSocket.SetSocketOption(SocketOptionLevel.Socket,
                SocketOptionName.DontLinger, true);
        }

        private void InitializeProperties()
        {
            // Initialize Properties
            Step = 0;
            ReceiveStream = null;
            SendStream = null;
            EncryptedSendStream = null;
            SessionKey = null;
            FileBytesReceived = 0;
            IsReceivingFile = false;
            ReceiveBufferSize = 4096;
            IsConnected = false;
            BytesSent = 0;
            ListenerSocket = null;
        }
        #endregion

        #region Connect
        public void Connect(string hostname, int port, int msTimeout = 0)
        {
            connectExit.Reset();
            connectDone.Reset();
            socket.BeginConnect(hostname, port, ConnectCallback, socket);
            HandleConnectEvents(msTimeout);
        }

        public void Connect(IPAddress ipAddress, int port, int msTimeout = 0)
        {
            connectExit.Reset();
            connectDone.Reset();
            socket.BeginConnect(ipAddress, port, ConnectCallback, socket);
            HandleConnectEvents(msTimeout);
        }

        public void Connect(IPEndPoint endPoint, int msTimeout = 0)
        {
            connectExit.Reset();
            connectDone.Reset();
            socket.BeginConnect(endPoint, ConnectCallback, socket);
            HandleConnectEvents(msTimeout);
        }

        private void HandleConnectEvents(int msTimeout)
        {
            if (msTimeout == 0)
                connectDone.WaitOne();
            else
                connectDone.WaitOne(msTimeout, false);
            if (IsConnected)
                OnConnected();
            else
                OnConnectFailed();
            connectExit.WaitOne();
        }

        private void ConnectCallback(IAsyncResult result)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)result.AsyncState;

                // Complete the connection.
                client.EndConnect(result);

                // Signal that the connection has been made.
                IsConnected = true;
                connectDone.Set();
            }
            catch (Exception e)
            {
                IsConnected = false;
                connectDone.Set();
            }
        }

        private void OnConnected()
        {
            if (Connected != null)
                Connected.Invoke(this, new EventArgs());
            BeginReceiving();
        }

        private void OnConnectFailed()
        {
            if (ConnectFailed != null)
                ConnectFailed.Invoke(this, new EventArgs());
            connectExit.Set();
        }
        #endregion

        #region Send
        public void Send(string data)
        {
            if (data == null)
                throw new ArgumentNullException();

            sendDone.Reset();

            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            socket.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), socket);

            HandleSendEvents();
        }

        public void Send(byte[] data, int offset, int length)
        {
            if (data == null)
                throw new ArgumentNullException();

            sendDone.Reset();

            // Begin sending the data to the remote device.
            socket.BeginSend(data, offset, length, 0,
                new AsyncCallback(SendCallback), socket);

            HandleSendEvents();
        }

        public void SendFile(Stream stream)
        {
            byte[] chunk = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            int bytesRead = stream.Read(chunk, 0, chunk.Length);
            Send(chunk, 0, chunk.Length);
        }

        public void SendFile(string path)
        {
            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            SendFile(stream);
            stream.Close();
        }

        private void HandleSendEvents()
        {
            sendDone.WaitOne();
            if (BytesSent >= 0)
                OnSendCompleted();
            else
                OnSendFailed();
        }

        private void OnSendFailed()
        {
            if (SendFailed != null)
                SendFailed.Invoke(this, new EventArgs());
        }

        private void OnSendCompleted()
        {
            if (SendCompleted != null)
                SendCompleted.Invoke(this, new EventArgs());
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                BytesSent = client.EndSend(ar);

                // Signal that all bytes have been sent.
                sendDone.Set();
            }
            catch (Exception e)
            {
                BytesSent = -1;
                sendDone.Set();
            }
        }        
        #endregion

        #region Disconnect
        public void Disconnect()
        {
            disconnectDone.Reset();
            try
            {
                OnDebug("Debug: Begin Disconnect");
                ShutdownSocket();
                socket.BeginDisconnect(false, DisconnectCallback, socket);
            }
            catch (SocketException ex)
            {
                OnDebug("Disconnect Exception: " + ex.Message);
                IsConnected = false;
                return;
            }
            disconnectDone.WaitOne();
            OnDisconnected();
        }

        private void ShutdownSocket()
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception)
            {
            }
        }

        private void DisconnectCallback(IAsyncResult ar)
        {
            try
            {
                OnDebug("Debug: Begin Disconnect Callback");
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Block until disconnected
                client.EndDisconnect(ar);

                // Signal that the disconnect is done
                disconnectDone.Set();
            }
            catch (Exception ex)
            {
                OnDebug("Disconnect Exception: " + ex.Message);
                disconnectDone.Set();
            }
        }

        private void OnDisconnected()
        {
            if (IsConnected)
            {
                IsReceivingFile = false;
                IsConnected = false;
                BytesSent = 0;
                if (Disconnected != null)
                    Disconnected.Invoke(this, new EventArgs());
                socket = new Socket(socket.AddressFamily, socket.SocketType, socket.ProtocolType);
                socket.SetSocketOption(SocketOptionLevel.Socket,
                    SocketOptionName.DontLinger, true);
            }
            connectExit.Set();
        }
        #endregion

        #region Binding
        public void Bind(IPAddress ipAddress, int port)
        {
            Bind(new IPEndPoint(ipAddress, port));
        }

        public void Bind(IPEndPoint endPoint)
        {
            if (!socket.IsBound)
                socket.Bind(endPoint);
        }
        #endregion

        #region Listening
        public void StartListening()
        {
            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                socket.Listen(1000);

                IsListening = true;
                IAsyncResult result = null;
                while (IsListening)
                {
                    // Set the event to nonsignaled state.
                    listenClientDone.Reset();
                    
                    // Start an asynchronous socket to listen for connections.
                    result = socket.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        socket);

                    // Wait until a connection is made before continuing.
                    listenClientDone.WaitOne();
                }
                //result.AsyncWaitHandle.SafeWaitHandle.Close();
                //socket.Listen(0);
                //socket.Close(-1);
            }
            catch (Exception e)
            {
                OnDebug(e.Message);
            }
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            listenClientDone.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            try
            {
                Socket handler = listener.EndAccept(ar);
                AsynchronousSocket clientSocket = new AsynchronousSocket(this, handler);
                OnClientConnected(clientSocket);
                clientSocket.BeginReceiving();
            }
            catch (ObjectDisposedException)
            {
                // Object disposed, np, ignore it
            }
            catch (Exception ex)
            {                
                OnDebug(ex.ToString());
            }
        }

        private void OnClientConnected(AsynchronousSocket clientSocket)
        {
            if (ClientConnected != null)
                ClientConnected.Invoke(clientSocket, new EventArgs());
        }

        public void StopListening()
        {
            IsListening = false;

            socket.Close();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //socket.LingerState = new LingerOption(false, 0);
            socket.SetSocketOption(SocketOptionLevel.Socket,
                SocketOptionName.DontLinger, true);

            listenClientDone.Set();
        }

        void closerSocket_Connected(object sender, EventArgs e)
        {
            AsynchronousSocket closerSocket = (AsynchronousSocket)sender;
            closerSocket.Disconnect();
        }

        #endregion

        #region Receiving
        public void BeginFileReceive(string outputFilePath)
        {
            IsReceivingFile = true;
            FileBytesReceived = 0;
            if (!File.Exists(outputFilePath))
                ReceiveStream = new FileStream(outputFilePath, FileMode.CreateNew, FileAccess.Write);
            else
                ReceiveStream = new FileStream(outputFilePath, FileMode.Truncate, FileAccess.Write);
        }

        public void BeginFileReceive(Stream stream)
        {
            IsReceivingFile = true;
            FileBytesReceived = 0;
            ReceiveStream = stream;
        }

        public void WriteFileReceive(byte[] data)
        {
            // write all bytes to file
            FileBytesReceived += data.Length;
            ReceiveStream.Write(data, 0, data.Length);
            ReceiveStream.Flush();
        }

        public void WriteFileReceive(byte[] data, int offset, int length)
        {
            byte[] usedData = data;
            if (offset > 0 || length < data.Length)
                usedData = SubArray(data, offset, length);
            WriteFileReceive(usedData);
        }

        public void EndFileReceive()
        {
            IsReceivingFile = false;
            ReceiveStream.Close();
            ReceiveStream = null;
        }

        private void BeginReceiving()
        {
            try
            {
                socket.BeginReceive(ReceiveBuffer, 0, ReceiveBufferSize, 0,
                    new AsyncCallback(ReadCallback), socket);
            }
            catch (SocketException ex)
            {
                // Remote is disconnected
                OnDebug(ex.Message);
                OnDisconnected();
            }
        }

        private void ReadCallback(IAsyncResult ar)
        {
            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            Socket handler = (Socket)ar.AsyncState;

            // Read data from the client socket. 
            try
            {
                ReceiveBufferLength = handler.EndReceive(ar);

                if (ReceiveBufferLength > 0)
                {
                    OnReceiveCompleted();
                    BeginReceiving();
                }
                else
                {
                    OnDisconnected();
                }
            }
            catch (ObjectDisposedException)
            {
                // Object disposed, np, ignore it
            }
            catch (SocketException ex)
            {
                OnDebug("SocketException: " + ex.Message);
                OnDisconnected();
            }
        }

        private void OnReceiveCompleted()
        {
            if (ReceiveCompleted != null)
                ReceiveCompleted.Invoke(this, new EventArgs());
        }
        #endregion

        #region Close
        public void Close()
        {
            try
            {
                socket.Close();
            }
            catch (Exception ex)
            {
                OnDebug(ex.Message);
            }
        }
        #endregion

        #region Buffer Handling
        public string ReceiveBufferToString()
        {
            return ASCIIEncoding.ASCII.GetString(ReceiveBuffer, 0, ReceiveBufferLength);
        }

        public static byte[] CheckSumHash(byte[] input1)
        {
            try
            {
                SHA1 sha = new SHA1CryptoServiceProvider();
                sha.ComputeHash(input1);
                return sha.Hash;
            }
            catch (System.Exception e)
            {
                return null;
            }
        }

        public static byte[] SubArray(byte[] totalArray, int offset, int length)
        {
            if (offset + length > totalArray.Length)
                throw new ArgumentException();
            byte[] result = new byte[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = totalArray[i + offset];
            }
            return result;
        }

        public static string[] TokenizeWithQuotes(string text)
        {
            List<string> results = new List<string>();
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex(@"((""((?<token>.*?)(?<!\\)"")|(?<token>[\w]+))(\s)*)", options);
            string input = @"   Here is ""my string"" it has   "" six  matches""   ";
            var result = (from Match m in regex.Matches(input)
                          where m.Groups["token"].Success
                          select m.Groups["token"].Value).ToList();

            for (int i = 0; i < result.Count(); i++)
            {
                result.Add(result[i]);
            }
            return results.ToArray();
        }

        #endregion

        #region Debug
        private void OnDebug(string message)
        {
            if (Debug != null)
                Debug.Invoke(this, new TextEventArgs(message));
        }
        #endregion
    }
}