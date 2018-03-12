namespace nDistribute.UWP.Tests.ThatWebSocket
{
    using System;
    using System.Threading.Tasks;


    public class Local
    {
        public Local(IAsyncTransportServer transport)
        {
            this.Transport = transport;

            Transport.MessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(object sender, string e)
        {
            MessageReceived?.Invoke(sender, e);
        }

        public EventHandler<string> MessageReceived { get; set; }


        public IAsyncTransportServer Transport { get; }

        public async Task StartAsync()
        {
            await Transport.StartLocalAsync();
        }
    }
}
