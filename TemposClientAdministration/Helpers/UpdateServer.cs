using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TemposLibrary;
using System.Threading;
using TemposUpdateServiceModels;

namespace TemposClientAdministration.Helpers
{
    public static class UpdateServer
    {
        private static Srp6ServerSocket server = new Srp6ServerSocket();
        private static Thread startThread = null;

        public static event TextEventHandler Debug;
        public static event EventHandler Started;
        public static event EventHandler Stopped;
        public static event EventHandler NewCrashIncident;

        public static Thread ServerThread
        {
            get { return startThread; }
        }

        public static bool IsRunning
        {
            get { return server.IsListening; }
        }

        static UpdateServer()
        {
            server.AuthenticationRequest +=
                new AuthenticationRequestEventHandler(server_AuthenticationRequest);
            server.ReceivedCrashException +=
                new ExceptionEventHandler(server_ReceivedCrashException);
            server.ServerStarted += new EventHandler(server_ServerStarted);
            server.ServerStopped += new EventHandler(server_ServerStopped);
            server.Debug += new TextEventHandler(server_Debug);
        }

        public static void Start()
        {
            // OnStart
            //ThreadPool.QueueUserWorkItem(ServerStartThread, server);
            try
            {
                if ((startThread != null) && startThread.IsAlive)
                    OnDebug("Old thread is still running");

                ParameterizedThreadStart threadStart =
                    new ParameterizedThreadStart(ServerStartThread);
                startThread = new Thread(threadStart);
                startThread.Start(server);
            }
            catch (Exception ex)
            {
                OnDebug(ex.Message);
            }
        }

        public static void Stop()
        {
            server.Stop();
        }

        private static void server_Debug(object sender, TextEventArgs args)
        {
            OnDebug(args.Text);
        }

        static void server_ServerStarted(object sender, EventArgs e)
        {
            OnDebug("Started");
            if (Started != null)
                Started.Invoke(sender, new EventArgs());
        }
        private static void server_ServerStopped(object sender, EventArgs e)
        {
            OnDebug("Stopped");
            if (Stopped != null)
                Stopped.Invoke(null, new EventArgs());
        }

        static void server_ReceivedCrashException(object sender, ExceptionEventArgs args)
        {
            Srp6ServerSocket socket = sender as Srp6ServerSocket;
            byte[] identityHash = socket.IdentityHash;
            License license = License.Find(identityHash);
            Customer customer = Customer.Get(license.CustomerId);
            CrashIncident crashIncident = CrashIncident.Add(customer.Id);
            CrashReport rootCrashReport = CrashReport.Add(crashIncident.Id, args.Exception);
            if (rootCrashReport != null)
            {
                crashIncident.SetTopLevelCrashReportId(rootCrashReport.Id);
                crashIncident.Update();
            }
            if (NewCrashIncident != null)
                NewCrashIncident.Invoke(sender, new EventArgs());
        }

        static void server_AuthenticationRequest(object sender,
            AuthenticationRequestEventArgs args)
        {
            License license = License.Find(args.IdentityHash);
            if ((license != null) && license.IsValid)
                args.Allow = true;
        }

        static private void ServerStartThread(object unused)
        {
            //OnDebug("Started Server Thread");
            server.Start();
        }

        static private void OnDebug(string message)
        {
            if (Debug != null)
                Debug.Invoke(null, new TextEventArgs(message));
        }


        /*
        /// <summary>
        /// Blocks until all worker threads have returned to the thread pool.
        /// </summary>
        static void WaitForThreads()
        {
            int maxThreads = 0;
            int placeHolder = 0;
            int availThreads = 0;

            //Now wait until all threads from the Threadpool have returned
            while (true)
            {
                //figure out what the max worker thread count it
                ThreadPool.GetMaxThreads(out maxThreads, out placeHolder);
                ThreadPool.GetAvailableThreads(out availThreads, out placeHolder);

                if (availThreads == maxThreads) break;

                // Sleep
                Thread.Sleep(TimeSpan.FromMilliseconds(500));
            }
        }
        */
    }
}
