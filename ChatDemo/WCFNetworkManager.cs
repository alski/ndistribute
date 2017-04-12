namespace ChatDemo
{
    using System.Linq;
    using nDistribute;
    using nDistribute.WCF;

    /// <summary>
    /// A network manager built into the ChatDemo
    /// </summary>
    /// <remarks>I probably should get rid of this.</remarks>
    public static class WCFNetworkManager
    {
        /// <summary>
        /// Factory method to create a <see cref="WCFNetwork"/>
        /// </summary>
        /// <returns>A new <see cref="WCFNetwork"/></returns>
        public static WCFNetwork Build()
        {
            WCFNetwork network = new WCFNetwork(() => Properties.Settings.Default.WasConnectedTo);
            network.IsConnectedChanged += RememberNetwork;
            network.Start();

            return network;
        }

        /// <summary>
        /// Try and connect to the network according to the settings I have saved
        /// </summary>
        /// <param name="network">Network to connect to</param>
        /// <remarks>Kill me quickly</remarks>
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
                    {
                    }

                    if (network.IsConnected)
                    {
                        break;
                    }
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
