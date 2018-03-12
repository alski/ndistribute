namespace nDistribute.UWP.Tests.ThatWebSocket
{
    using System.Threading.Tasks;

    public interface IAsyncTransportClient
    {
        Task SendAsync(string request);
    }
}
