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
    using Windows.Networking.Sockets;
    using Windows.Storage.Streams;

    [TestClass]
    public class WebSocket
    {
        [TestMethod]
        public async Task ShouldSendMessages()
        {

            var fred = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile().GetNetworkNames().ToArray();

            var messageReceived = string.Empty;

            //Server
            var server = new StreamSocketListener();
            server.ConnectionReceived += async (sender, args) =>
            {
                using (var streamReader = new StreamReader(args.Socket.InputStream.AsStreamForRead()))
                {
                    messageReceived = await streamReader.ReadLineAsync();
                }
                Debug.WriteLine("Message received: " + messageReceived);

                //await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.serverListBox.Items.Add(string.Format("server received the request: \"{0}\"", request)));

                //sender should be disposed....
                sender.Dispose();
            };

            //string.Empty is find me a port :-)
            await server.BindServiceNameAsync(string.Empty);


            //Client
            using (var client = new StreamSocket())
            {
                var hostnames = Windows.Networking.Connectivity.NetworkInformation.GetHostNames();
                var hostName = hostnames.First();
                //var hostName = new Windows.Networking.HostName("localhost");

                await client.ConnectAsync(hostName, server.Information.LocalPort);

                // Send a request to the echo server.
                string request = $"Hello, local port {server.Information.LocalPort}!";
                using (Stream outputStream = client.OutputStream.AsStreamForWrite())
                {
                    using (var streamWriter = new StreamWriter(outputStream))
                    {
                        await streamWriter.WriteLineAsync(request);
                        await streamWriter.FlushAsync();
                    }
                }
            }

            Wait.Until(() => !string.IsNullOrEmpty(messageReceived));
        }

    }
}
