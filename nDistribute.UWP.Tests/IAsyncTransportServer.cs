namespace nDistribute.UWP.Tests.ThatWebSocket
{
    using System;
    using System.Threading.Tasks;

    public interface IAsyncTransportServer
    {
        Task StartLocalAsync();

        EventHandler<string> MessageReceived { get; set; }
    }
}
