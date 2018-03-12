namespace nDistribute
{
    using System.Threading.Tasks;

    public interface IAsyncTransportClient
    {
        Task SendAsync(string request);
    }
}
