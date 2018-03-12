namespace nDistribute
{
    using System;
    using System.Threading.Tasks;

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
