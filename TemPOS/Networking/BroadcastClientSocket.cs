#if !DEMO
using System;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using TemposLibrary;

namespace PointOfSale.Networking
{
    public static class BroadcastClientSocket
    {
        #region Licensed Access Only / Static Initializer
        static BroadcastClientSocket()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(BroadcastClientSocket).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to link to this application");
            }
#endif
            IsConnected = false;
            Socket = null;
        }
        #endregion

        [Obfuscation(Exclude = true)]
        public static event EventHandler ReceivedMessage;
        
        [Obfuscation(Exclude = true)]
        public static event EventHandler Connected;

        public static bool IsConnected
        {
            get;
            private set;
        }

        public static AsynchronousSocket Socket
        {
            get;
            private set;
        }

        public static void Start()
        {
            int? port = BroadcastServerSocket.Port;
            if (port == null)
                return;
            Thread thread = new Thread(StartThread);
            thread.Start(port.Value);
        }

        private static void StartThread(object objectPort)
        {
            int port = (int)objectPort;
            Socket = new AsynchronousSocket();
            Socket.Connected += Socket_Connected;
            Socket.ConnectFailed += Socket_ConnectFailed;
            Socket.Disconnected += Socket_Disconnected;
            Socket.SendCompleted += Socket_SendCompleted;
            Socket.SendFailed += Socket_SendFailed;
            Socket.ReceiveCompleted += Socket_ReceiveCompleted;
            Socket.Connect(IPAddress.Loopback, port, 10000); // Blocks until disconnected
        }

        public static void SendMessage(string message)
        {
            try
            {
                Socket.Send(message);
            }
            catch (Exception)
            {
            }
        }

        public static void SendRemoteLogout(int employeeId)
        {
            SendMessage("RemoteLogout " + employeeId);
        }

        public static void Stop()
        {
            try
            {
                Socket.Disconnect();
            }
            catch (Exception)
            {

            }
            finally
            {
                if (Socket != null)
                    Socket.Close();
                Socket = null;
            }
        }

        static void Socket_ReceiveCompleted(object sender, EventArgs e)
        {
            AsynchronousSocket socket = sender as AsynchronousSocket;
            if (socket == null) return;
            byte[] message = SRP6.SubArray(
                socket.ReceiveBuffer, 0,
                socket.ReceiveBufferLength);
            string strMessage = 
                Encoding.ASCII.GetString(message, 0, message.Length);
            if (ReceivedMessage != null)
                ReceivedMessage.Invoke(strMessage, new EventArgs());
        }

        static void Socket_SendFailed(object sender, EventArgs e)
        {
            //AsynchronousSocket socket = sender as AsynchronousSocket;

        }

        static void Socket_SendCompleted(object sender, EventArgs e)
        {
            //AsynchronousSocket socket = sender as AsynchronousSocket;
            
        }

        static void Socket_Disconnected(object sender, EventArgs e)
        {
            //AsynchronousSocket socket = sender as AsynchronousSocket;
            IsConnected = false;            
        }

        static void Socket_ConnectFailed(object sender, EventArgs e)
        {
            //AsynchronousSocket socket = sender as AsynchronousSocket;
            IsConnected = false;
        }

        static void Socket_Connected(object sender, EventArgs e)
        {
            AsynchronousSocket socket = sender as AsynchronousSocket;
            IsConnected = true;
            if (Connected != null)
                Connected.Invoke(socket, new EventArgs());            
        }
    }
}
#endif