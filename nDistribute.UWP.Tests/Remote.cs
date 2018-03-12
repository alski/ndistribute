namespace nDistribute.UWP.Tests.ThatWebSocket
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Windows.Networking;
    using Windows.Networking.Sockets;

    public class Remote
    {
        public Remote(IAsyncTransportClient client)
        {
            this.Client = client;
        }

        public IAsyncTransportClient Client { get; }

        public async Task SendAsync()
        {
            string request = $"Hello, local!";
            await Client.SendAsync(request);
        }
    }
}
