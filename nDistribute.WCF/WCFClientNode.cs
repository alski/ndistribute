namespace nDistribute.WCF
{
    using nDistribute;
    using nDistribute.WCF.NetworkServiceClient;

    /// <summary>A wrapper to convert between interfaces even though they are the same.</summary>
    internal class WCFClientNode : Node
    {
        private readonly NodeContractClient client;

        /// <summary>Initializes a new instance of the <see cref="WCFClientNode"/> class.</summary>
        /// <param name="client">The client.</param>
        /// <param name="address">The address.</param>
        /// <param name="network">The network.</param>
        internal WCFClientNode(NodeContractClient client, NodeAddress address, INetwork network)
            : base(address, network)
        {
            this.client = client;
        }

        /// <summary>The connect.</summary>
        /// <param name="newNode">The new node.</param>
        /// <returns>The <see cref="NodeAddress"/>.</returns>
        public override NodeAddress Connect(NodeAddress newNode)
        {
            return client.Connect(newNode);
        }

        /// <summary>The advise connect.</summary>
        /// <param name="newParent">The new parent.</param>
        public override void AdviseConnect(NodeAddress newParent)
        {
            client.AdviseConnectAsync(newParent);            
        }

        /// <summary>The child disconnect.</summary>
        /// <param name="address">The address.</param>        
        public override void ChildDisconnect(NodeAddress address)
        {
            client.ChildDisconnectAsync(address);
        }

        /// <summary>Sends data out across the network.</summary>        
        /// <param name="type"><see cref="Type"/> to deserialize data back to.</param>
        /// <param name="data">The data.</param>
        /// <param name="from">Where the data came from.</param>
        public override void Send(string type, byte[] data, NodeAddress from)
        {
            client.SendAsync(type, data, from);
        }
    }
}
