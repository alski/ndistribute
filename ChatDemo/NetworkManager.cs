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
            //SettingsProperty portProperty = Properties.Settings.Default.Properties["Port"];
            //if (portProperty == null)
            //{
            //    var attrs = new SettingsAttributeDictionary();
            //    attrs.Add(typeof(UserScopedSettingAttribute), new UserScopedSettingAttribute());
            //    portProperty = new SettingsProperty("Port", typeof(int),
            //        new LocalFileSettingsProvider(),
            //        false,
            //        0,
            //        SettingsSerializeAs.String,
            //        attrs,
            //        false,
            //        false);
            //    Properties.Settings.Default.Properties.Add(portProperty);
            //    Properties.Settings.Default.Save();
            //}
            var port = (int)Properties.Settings.Default["Port"];

            if (port == 0)
            {
                port = NetworkFactory.FreeTcpPort();
                Properties.Settings.Default["Port"] = port;
                Properties.Settings.Default.Save();
            }

            WCFNetwork network;
            try
            {
               network = new WCFNetwork(port);
                network.IsConnectedChanged += RememberNetwork;
                network.Start();
            }
            catch (AddressAlreadyInUseException ex)
            {
                network = new WCFNetwork(NetworkFactory.FreeTcpPort());
                network.IsConnectedChanged += RememberNetwork;
                network.Start();
            }
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
                        network.Connect(connection);
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
