using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace nDistribute
{
    public class NetworkManager
    {
        private List<NetworkRegistration> availableNetworks = new List<NetworkRegistration>();
        private List<NetworkBase> connectedNetworks = new List<NetworkBase>();
    
        public static int FindFreeTcpPort()
        {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        internal void Register(NetworkBase network)
        {
            Register(network.AsRegistration());
        }

        internal bool Reconnect()
        {
            return false;
        }      

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

        internal void Register(NetworkRegistration registration) 
        {
            if (registration == null)
                throw new ArgumentException("Cannot register null", "registration");

            availableNetworks.Add(registration);
        }
    }
}
