namespace nDistribute.WCF
{
    using nDistribute;
    using nDistribute.WCF.NetworkServiceClient;
    using System;

    /// <summary>A wrapper to convert between interfaces even though they are the same.</summary>
    internal class WCFClientNode : Node
    {
        private readonly NodeContractClient _client;

        /// <summary>Initialises a new instance of the <see cref="WCFClientNode"/> class.</summary>
        /// <param name="client">The client.</param>
        /// <param name="address">The address.</param>
        /// <param name="network">The network.</param>
        internal WCFClientNode(NodeContractClient client, NodeAddress address, INetwork network)
            : base(address, network)
        {
            this._client = client;
        }

        /// <summary>The connect.</summary>
        /// <param name="newNode">The new node.</param>
        /// <returns>The <see cref="NodeAddress"/>.</returns>
        public override NodeAddress Connect(NodeAddress newNode)
        {
            return this._client.Connect(newNode);
        }

        /// <summary>The advise connect.</summary>
        /// <param name="newParent">The new parent.</param>
        public override void AdviseConnect(NodeAddress newParent)
        {
            this._client.AdviseConnectAsync(newParent);            
        }

        /// <summary>The child disconnect.</summary>
        /// <param name="address">The address.</param>        
        public override void ChildDisconnect(NodeAddress address)
        {
            this._client.ChildDisconnectAsync(address);
        }

        /// <summary>The send.</summary>
        /// <param name="data">The data.</param>
        /// <param name="type"><see cref="Type"/> to deserialize data back to.</param>
        public override void Send(string type, byte[] data, NodeAddress from)
        {
            this._client.SendAsync(type, data, from);
        }
    }
}
