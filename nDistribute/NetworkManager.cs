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
        private static List<NetworkRegistration> _availableNetworks = new List<NetworkRegistration>();
        private List<NetworkBase> _connectedNetworks = new List<NetworkBase>();

        public static int FreeTcpPort()
        {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        internal bool Reconnect()
        {
            return false;
        }

        public IEnumerable<INetwork> Connections { get { return _connectedNetworks; } }

        internal NetworkBase ConnectTo(string remoteAddress)
        {
            foreach (var reg in _availableNetworks)
            {
                NetworkBase result;
                if (reg.CanCreate(remoteAddress))
                {
                    result = reg.CreateNetwork(remoteAddress);
                    if (result != null)
                    {
                        _connectedNetworks.Add(result);
                        return result;
                    }
                }
            }
            return null;
        }

        internal static void Register(NetworkRegistration registration) 
        {
            if (registration == null)
                throw new ArgumentException("Cannot register null", "registration");

            _availableNetworks.Add(registration);
        }
    }
}
