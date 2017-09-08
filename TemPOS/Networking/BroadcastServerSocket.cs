#if !DEMO
using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using PosModels;
using PosModels.Managers;
using TemposLibrary;

namespace PointOfSale.Networking
{
    public class BroadcastServerSocket
    {
        #region Licensed Access Only
        static BroadcastServerSocket()
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

        private static ArrayList clients = new ArrayList();
        private static AsynchronousSocket listenerSocket;

        [Obfuscation(Exclude = true)]
        public static event EventHandler StartedListening;

        public static bool IsRunning
        {
            get
            {
                if (listenerSocket == null)
                    return false;
                return listenerSocket.IsListening;
            }
        }

        public static bool IsEnabled
        {
            get { return (LocalSetting.Values.String["StartBroadcastServer"] != null); }
            set { LocalSetting.Values.String["StartBroadcastServer"] = (value ? "Yes" : null); }
        }

        public static int? Port
        {
            get
            {
                StoreSetting setting = SettingManager.GetStoreSetting("BroadcastServerPort");
                if (setting == null)
                    return null;
                return setting.IntValue;
            }
            set
            {
                SettingManager.SetStoreSetting("BroadcastServerPort", value);                
            }
        }

        public static void Start()
        {
            int? port = Port;
            if (port == null)
                return;
            ParameterizedThreadStart threadStart =
                new ParameterizedThreadStart(StartThread);
            Thread thread = new Thread(threadStart);
            thread.Start(port.Value);
        }

        private static void StartThread(object objectPort)
        {
            int port = (int)objectPort;
            //Console.WriteLine("Listening on port 41111");
            listenerSocket = new AsynchronousSocket();
            listenerSocket.IsListeningChanged +=
                new EventHandler(listenerSocket_IsListeningChanged);
            listenerSocket.ClientConnected +=
                new EventHandler(listenerSocket_ClientConnected);
            listenerSocket.Bind(IPAddress.Any, port);
            listenerSocket.StartListening(); // Blocks until StopListening
        }

        private static void listenerSocket_IsListeningChanged(object sender, EventArgs e)
        {
            AsynchronousSocket socket = (sender as AsynchronousSocket);
            if (socket.IsListening && (StartedListening != null))
            {
                StartedListening.Invoke(socket, new EventArgs());
            }
        }

        public static void Stop()
        {
            listenerSocket.StopListening();

            AsynchronousSocket[] sockets = (AsynchronousSocket[])
                clients.ToArray(typeof(AsynchronousSocket));
            foreach (AsynchronousSocket socket in sockets)
            {
                socket.Disconnect();
            }
            clients.Clear();
        }

        public static void BroadcastMessage(string message)
        {
            foreach (AsynchronousSocket client in clients)
            {
                if (client.IsConnected)
                    client.Send(message);
            }
        }

        private static void listenerSocket_ClientConnected(object sender, EventArgs e)
        {
            AsynchronousSocket socket = sender as AsynchronousSocket;
            clients.Add(socket);
            socket.Disconnected += new EventHandler(clientSocket_Disconnected);
            socket.ReceiveCompleted += new EventHandler(clientSocket_ReceiveCompleted);
        }

        private static void clientSocket_Disconnected(object sender, EventArgs e)
        {
            AsynchronousSocket socket = sender as AsynchronousSocket;
            clients.Remove(socket);
            Console.WriteLine("Server: Client Disconnected (" + socket.TcpSocket.RemoteEndPoint.ToString() + ")");
        }

        private static void clientSocket_ReceiveCompleted(object sender, EventArgs e)
        {
            AsynchronousSocket socket = sender as AsynchronousSocket;
            byte[] message = SRP6.SubArray(
                socket.ReceiveBuffer, 0,
                socket.ReceiveBufferLength);
            foreach (AsynchronousSocket client in clients)
            {
                if (client == socket)
                    continue;
                if (client.IsConnected)
                    client.Send(message, 0, message.Length);
            }
        }
    }
}
#endif