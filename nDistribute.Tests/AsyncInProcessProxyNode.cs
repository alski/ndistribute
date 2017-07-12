namespace nDistribute.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>A wrapper to convert between interfaces even though they are the same.</summary>
    internal class AsyncInProcessProxyNode : Node
    {
        private readonly AsyncInProcessNetwork network;

        /// <summary>Initializes a new instance of the <see cref="InProcessProxyNode"/> class.</summary>
        /// <param name="address">The address.</param>
        /// <param name="network">The network.</param>
        internal AsyncInProcessProxyNode(NodeAddress address, AsyncInProcessNetwork network)
            : base(address, network)
        {
            this.network = network;

            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Gets a unique Id for this node.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>The connect.</summary>
        /// <param name="newNode">The new node.</param>
        /// <returns>The <see cref="NodeAddress"/>.</returns>
        public override NodeAddress Connect(NodeAddress newNode)
        {
            var originalNode = FindOriginalNode();
            if (originalNode is InProcessProxyNode)
            {
                throw new NotImplementedException();
            }

            return originalNode.Connect(newNode);
        }

        /// <summary>The advise connect.</summary>
        /// <param name="newParent">The new parent.</param>
        public override void AdviseConnect(NodeAddress newParent)
        {
            var originalNode = FindOriginalNode();
            if (originalNode is InProcessProxyNode)
            {
                throw new NotImplementedException();
            }

            originalNode.AdviseConnect(newParent);
        }

        /// <inheritdoc/>
        public override void ChildDisconnect(NodeAddress address)
        {
            var originalNode = FindOriginalNode();
            if (originalNode is InProcessProxyNode)
            {
                throw new NotImplementedException();
            }

            originalNode.ChildDisconnect(address);
        }

        /// <inheritdoc/>
        public override void Send(string type, byte[] data, NodeAddress from)
        {
            var originalNode = FindOriginalNode();
            if (originalNode is InProcessProxyNode)
            {
                throw new NotImplementedException();
            }

            network.StartAsync(() => originalNode.Send(type, data, from));
        }


        private INode FindOriginalNode()
        {
            var network = AsyncInProcessNetwork.Networks.Single(n => n.Local.Address == Address);
            return network.Local;
        }
    }

}
