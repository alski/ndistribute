namespace nDistribute.UWP.Tests.ThatWebSocket
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Windows.Networking;
    using Windows.Networking.Connectivity;
    using Windows.Networking.Sockets;

    public static class StreamSocketTransport
    {
        public class Server : IAsyncTransportServer
        {
            private StreamSocketListener server = new StreamSocketListener();

            public Server()
            {
            }

            public string Port => server.Information.LocalPort;

            public async Task StartLocalAsync()
            {
                //Server
                await server.BindServiceNameAsync("53700");
                server.ConnectionReceived += async (sender, args) =>
                {
                    using (var streamReader = new StreamReader(args.Socket.InputStream.AsStreamForRead()))
                    {
                        MessageReceived.Invoke(this, await streamReader.ReadLineAsync());
                    }
                    Debug.WriteLine("Message received: " + MessageReceived);

                    //sender should be disposed....
                    sender.Dispose();
                };

                //string.Empty is find me a port :-)
                await server.BindServiceNameAsync(string.Empty);
            }

            public EventHandler<string> MessageReceived { get; set; }
        }

        public class Client : IAsyncTransportClient
        {
            public Client(HostName hostName, string port)
            {
                HostName = hostName;
                Port = port;
            }
            public HostName HostName { get; }
            public string Port { get; }

            public async Task SendAsync(string request)
            {
                using (var client = new StreamSocket())
                {
                    await client.ConnectAsync(HostName, Port);

                    // Send a request to the echo server.
                    using (Stream outputStream = client.OutputStream.AsStreamForWrite())
                    {
                        using (var streamWriter = new StreamWriter(outputStream))
                        {
                            await streamWriter.WriteLineAsync(request);
                            await streamWriter.FlushAsync();
                        }
                    }
                }
            }
        }
    }
}
