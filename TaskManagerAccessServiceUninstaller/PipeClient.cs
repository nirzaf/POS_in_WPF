using System.IO.Pipes;
using System.IO;

namespace TaskManagerAccessServiceUninstaller
{
    public class PipeClient
    {
        public string PipeName
        {
            get;
            private set;
        }

        public string PipeServerName
        {
            get;
            private set;
        }

        public PipeClient(string name, string server)
        {
            PipeName = name;
            PipeServerName = server;
        }

        public void SendMessage(string message)
        {
            using (var clientStream = new NamedPipeClientStream(PipeServerName, PipeName,
                PipeDirection.Out, PipeOptions.None))
            {
                clientStream.Connect(5000);

                using (var writer = new StreamWriter(clientStream))
                {
                    writer.AutoFlush = true;
                    writer.WriteLine(message);
                }
            }
        }

    }
}
