using System;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.IO;

namespace TaskManagerAccessService
{
    public class PipeServer
    {
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool WaitNamedPipe(string name, int timeout);

        private readonly EventWaitHandle _terminateHandle =
            new EventWaitHandle(false, EventResetMode.AutoReset);

        public event TextEventHandler MessageReceived;
        public event TextEventHandler Debug;

        public bool IsRunning
        {
            get;
            private set;
        }

        public string PipeName
        {
            get;
            private set;
        }

        public PipeServer(string name)
        {
            IsRunning = false;
            PipeName = name;
        }

        public void Start()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                var thread = new Thread(MainLoop);
                thread.Start();
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                // ToDo: Fix Hack - Close out the existing WaitForConnection()
                using (var closeExisting = new NamedPipeClientStream(PipeName))
                {
                    closeExisting.Connect();
                }
                _terminateHandle.WaitOne();
            }
        }

        public void WaitForServer()
        {
            WaitForServer(PipeName);
        }

        private void MainLoop()
        {
            while (IsRunning)
            {
                OnDebug("Starting...");
                try
                {
                    WindowsIdentity identity = WindowsIdentity.GetCurrent();
                    var pipeSecurity = new PipeSecurity();
                    //pipeSecurity.AddAccessRule(new PipeAccessRule("SYSTEM", PipeAccessRights.FullControl, AccessControlType.Allow));
                    //pipeSecurity.AddAccessRule(new PipeAccessRule("LOCAL SERVICE", PipeAccessRights.FullControl, AccessControlType.Allow));
                    pipeSecurity.AddAccessRule(new PipeAccessRule("Users", PipeAccessRights.ReadWrite, AccessControlType.Allow));
                    if ((identity != null) && (identity.Owner != null))
                        pipeSecurity.AddAccessRule(new PipeAccessRule(identity.Owner, PipeAccessRights.FullControl, AccessControlType.Allow));

                    var pipeServer = new NamedPipeServerStream(PipeName,
                        PipeDirection.InOut, -1, PipeTransmissionMode.Message,
                        PipeOptions.None, 256, 256, pipeSecurity,
                        HandleInheritability.Inheritable, PipeAccessRights.ChangePermissions &
                        PipeAccessRights.ReadWrite & PipeAccessRights.ReadData &
                        PipeAccessRights.CreateNewInstance);

                    // Wait for a client to connect
                    pipeServer.WaitForConnection();

                    // start the client thread
                    var thread = new Thread(ClientThreadStart);
                    thread.Start(pipeServer);
                }
                catch (Exception)
                {
                    IsRunning = false;
                }
            }

            _terminateHandle.Set();
        }

        private void ClientThreadStart(object arg)
        {
            var pipeServer = arg as NamedPipeServerStream;
            if (pipeServer == null) return;
            using (var rd = new StreamReader(pipeServer))
            {
                string contents = rd.ReadToEnd();
                if (!string.IsNullOrEmpty(contents) && (MessageReceived != null))
                    MessageReceived.Invoke(this, new TextEventArgs(contents));
            }
            pipeServer.Dispose();
        }

        private void OnDebug(string text)
        {
            if (Debug != null)
                Debug.Invoke(this, new TextEventArgs(text));
        }

        #region Static
        public static void WaitForServer(string name, string server = ".")
        {
            if (!IsServerAlive(name, server))
            {
                Thread.Sleep(200);
                while (!IsServerAlive(name, server))
                    Thread.Sleep(200);
            }
        }

        public static bool IsServerAlive(string pipeName, string server = ".")
        {
            try
            {
                string normalizedPath = Path.GetFullPath(
                    string.Format(@"\\{1}\pipe\{0}", pipeName, server));
                bool exists = WaitNamedPipe(normalizedPath, 0);
                if (!exists)
                {
                    int error = Marshal.GetLastWin32Error();
                    if (error == 0) // pipe does not exist
                        return true;
                    if (error == 2) // win32 error code for file not found
                        return true;
                    // all other errors indicate other issues
                }
                return false;
            }
            catch (Exception)
            {
                return true;
            }
        }

        #endregion
    }
}
