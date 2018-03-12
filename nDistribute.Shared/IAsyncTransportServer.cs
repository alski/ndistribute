namespace nDistribute
{ 
    using System;
    using System.Threading.Tasks;

    public interface IAsyncTransportServer
    {
        Task StartLocalAsync();

        EventHandler<string> MessageReceived { get; set; }
    }
}
