namespace nDistribute.Tests
{
    using System;
    using System.Linq;

    /// <summary>A wrapper to convert between interfaces even though they are the same.</summary>
    internal class InProcessProxyNode : Node
    {
        private readonly InProcessNetwork _network;
        
        /// <summary>Initialises a new instance of the <see cref="InProcessProxyNode"/> class.</summary>
        /// <param name="address">The address.</param>
        /// <param name="network">The network.</param>
        internal InProcessProxyNode(NodeAddress address, InProcessNetwork network)
            : base(address, network)
        {
            _network = network;

            ID = Guid.NewGuid();
        }

        public Guid ID { get; private set; }

        /// <summary>The connect.</summary>
        /// <param name="newNode">The new node.</param>
        /// <returns>The <see cref="NodeAddress"/>.</returns>
        public override NodeAddress Connect(NodeAddress newNode)
        {
            var originalNode = this.FindOriginalNode();
            if (originalNode is InProcessProxyNode)
            {
                throw new NotImplementedException();
            }

            return originalNode.Connect(newNode);
        }

        private INode FindOriginalNode()
        {
            var network = InProcessNetwork.Networks[this.Address.Address];
            var remoteNode = new NodeAddress(network.NetworkName);
            return network.FindOrCreate(remoteNode);
        }

        /// <summary>The advise connect.</summary>
        /// <param name="newParent">The new parent.</param>
        public override void AdviseConnect(NodeAddress newParent)
        {
            var originalNode = this.FindOriginalNode();
            if (originalNode is InProcessProxyNode)
            {
                throw new NotImplementedException();
            }

             originalNode.AdviseConnect(newParent);
        }

        public override void ChildDisconnect(NodeAddress address)
        {
            var originalNode = this.FindOriginalNode();
            if (originalNode is InProcessProxyNode)
            {
                throw new NotImplementedException();
            }

            originalNode.ChildDisconnect(address);
        }

        public override void Send(string type, byte[] data, NodeAddress from)
        {
            var originalNode = this.FindOriginalNode();
            if (originalNode is InProcessProxyNode)
            {
                throw new NotImplementedException();
            }
            originalNode.Send(type, data, from);
        }
        
    }
}
