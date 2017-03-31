using nDistribute;
using nDistribute.WCF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ChatDemo
{
    public static class NetworkManager
    {
        public static WCFNetwork Build()
        {
            WCFNetwork network = new WCFNetwork(()=> Properties.Settings.Default.WasConnectedTo);
            network.IsConnectedChanged += RememberNetwork;
            network.Start();

            return network;
        }

        public static void TryConnect(WCFNetwork network)
        {
            var wasConnectedTo = Properties.Settings.Default.WasConnectedTo;
            if (string.IsNullOrEmpty(wasConnectedTo) == false)
            {
                foreach (var connection in wasConnectedTo.Split('|'))
                {
                    try
                    {
                        network.Connect(new NodeAddress(connection));
                    }
                    catch
                    { }

                    if (network.IsConnected)
                        break;
                }
            }
        }

        private static void RememberNetwork(object sender, ConnectedEventArgs e)
        {
            if (e.Connected.Any())
            {
                var connected = string.Join("|", e.Connected);
                if (connected != Properties.Settings.Default.WasConnectedTo)
                {
                    Properties.Settings.Default.WasConnectedTo = connected;
                    Properties.Settings.Default.Save();
                }
            }
        }
    }
}
