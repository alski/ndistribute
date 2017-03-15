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
        private static List<NetworkRegistration> availableNetworks = new List<NetworkRegistration>();
        private List<NetworkBase> connectedNetworks = new List<NetworkBase>();

        internal static string GetConfiguration(NetworkBase network)
        {
            return network.Local.ToString() + "=" + string.Join("|", network.GetConnectionNames());
        }

        public static int FindFreeTcpPort()
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

        internal NetworkBase ConnectTo(string remoteAddress)
        {
            foreach (var reg in availableNetworks)
            {
                NetworkBase result;
                if (reg.CanCreate(remoteAddress))
                {
                    result = reg.CreateNetwork(remoteAddress);
                    if (result != null)
                    {
                        connectedNetworks.Add(result);
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

            availableNetworks.Add(registration);
        }
    }
}
