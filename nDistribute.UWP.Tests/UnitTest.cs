namespace nDistribute.UWP.Tests.ThatWebSocket
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Windows.Networking;
    using Windows.Networking.Connectivity;
    using Windows.Networking.Sockets;
    using Windows.Storage.Streams;

    using Shouldly;
    using Eventually;

    [TestClass]
    public class WebSocket
    {

        public class Local
        {
            public Local()
            {
                var connection = NetworkInformation.GetInternetConnectionProfile();

                if (connection?.NetworkAdapter != null)
                {
                    RemoteName = NetworkInformation.GetHostNames()
                    .FirstOrDefault(hostname =>
                        hostname.Type == HostNameType.Ipv4
                        && hostname.IPInformation?.NetworkAdapter?.NetworkAdapterId == connection.NetworkAdapter.NetworkAdapterId);
                }
                else
                {
                    RemoteName = new HostName("localHost");
                }
            }

            private StreamSocketListener server = new StreamSocketListener();

            public string MessageReceived { get; private set; } = string.Empty;

            public string Port => server.Information.LocalPort;

            public HostName RemoteName { get; }

            public async Task StartAsync()
            {
                //Server
                server.ConnectionReceived += async (sender, args) =>
                {
                    using (var streamReader = new StreamReader(args.Socket.InputStream.AsStreamForRead()))
                    {
                        MessageReceived = await streamReader.ReadLineAsync();
                    }
                    Debug.WriteLine("Message received: " + MessageReceived);

                    //await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.serverListBox.Items.Add(string.Format("server received the request: \"{0}\"", request)));

                    //sender should be disposed....
                    sender.Dispose();
                };

                //string.Empty is find me a port :-)
                await server.BindServiceNameAsync(string.Empty);
            }
        }

        public class Remote
        {
            public Remote(HostName hostName, string port)
            {
                HostName = hostName;
                Port = port;
            }

            public HostName HostName { get; }
            public string Port { get; }

            public async void Send()
            {
                //Client
                using (var client = new StreamSocket())
                {
                    await client.ConnectAsync(HostName, Port);

                    // Send a request to the echo server.
                    string request = $"Hello, local port {Port}!";
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

        [TestMethod]
        public async Task ShouldSendMessages()
        {
            var local = new Local();
            await local.StartAsync();

            var remote = new Remote(local.RemoteName, local.Port);
            remote.Send();

            local.Eventually(x => x.MessageReceived.ShouldNotBeNullOrEmpty());
        }

    }
}
