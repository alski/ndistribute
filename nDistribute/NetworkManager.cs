namespace nDistribute
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;

    /// <summary>
    /// Functions to help manage the network 
    /// </summary>
    public class NetworkManager
    {
        private List<NetworkRegistration> availableNetworks = new List<NetworkRegistration>();
        private List<NetworkBase> connectedNetworks = new List<NetworkBase>();
    
        /// <summary>
        /// Finds an unused TCP port.
        /// </summary>
        /// <returns>Id of a port that can be opened.</returns>
        public static int FindFreeTcpPort()
        {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        /// <summary>
        /// Registers a type of network to communicate with.
        /// </summary>
        /// <param name="network">A pre build network that we will use in communications.</param>
        internal void Register(NetworkBase network)
        {
            Register(network.AsRegistration());
        }
        
        /// <summary>
        /// Methiod to be replaced!!!
        /// </summary>
        /// <param name="remoteAddress">The address of a remote node to connect to.</param>
        /// <returns>A network connected to that node.</returns>
        internal NetworkBase ConnectTo(NodeAddress remoteAddress)
        {
            var existing = connectedNetworks.FirstOrDefault(n => n.CanCreate(remoteAddress));
            if (existing != null)
            {
                existing.Connect(remoteAddress);
                return existing;
            }

            foreach (var reg in availableNetworks)
            {
                NetworkBase result;
                if (reg.CanCreate(remoteAddress))
                {
                    result = reg.CreateNetwork();
                    result.Connect(remoteAddress);
                    if (result != null)
                    {
                        connectedNetworks.Add(result);
                        return result;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Registers a network type with so we can use it in the future.
        /// </summary>
        /// <param name="registration">The <see cref="NetworkRegistration"/> to use.</param>
        internal void Register(NetworkRegistration registration) 
        {
            if (registration == null)
            {
                throw new ArgumentException("Cannot register null", "registration");
            }

            availableNetworks.Add(registration);
        }
    }
}
