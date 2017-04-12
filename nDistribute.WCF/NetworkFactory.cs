namespace nDistribute.WCF
{
    using System.Net;
    using System.Net.Sockets;

    /// <summary>
    /// A set of helper methods for networks
    /// </summary>
    public static class NetworkFactory
    {
        /// <summary>
        /// Finds a free tcp port.
        /// </summary>
        /// <returns>The id of the free port.</returns>
        public static int FreeTcpPort()
        {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }
    }
}
