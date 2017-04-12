namespace nDistribute.WCF
{
    using System.ServiceModel;
    using nDistribute;

    /// <summary>The remote connection service.</summary>
    [ServiceBehavior(
        ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.Single,
        IncludeExceptionDetailInFaults = true)]
    public class RemoteConnectionService : INode
    {
        /// <summary>
        /// Gets or sets the node that we are going to use 
        /// </summary>
        public Node Node { get; set; }

        /// <inheritdoc/>
        public NodeAddress Address
        {
            get { return Node.Address; }
        }

        /// <inheritdoc/>
        public bool HasParent
        {
            get { return Node.HasParent; }
        }

        /// <inheritdoc/>
        public bool IsConnected
        {
            get { return Node.IsConnected; }
        }

        /// <inheritdoc/>
        public bool HasChild(NodeAddress address)
        {
            return Node.HasChild(address);
        }

        /// <inheritdoc/>
        public void Disconnect()
        {
            Node.Disconnect();
        }

        /// <inheritdoc/>
        public void SendToNetwork(INetwork network, string type, byte[] data, NodeAddress from)
        {
            Node.SendToNetwork(network, type, data, from);
        }

        /// <inheritdoc/>
        public NodeAddress Connect(NodeAddress newNode)
        {
            return Node.Connect(newNode);
        }

        /// <inheritdoc/>
        public void AdviseConnect(NodeAddress newParent)
        {
            Node.AdviseConnect(newParent);
        }

        /// <inheritdoc/>
        public void ChildDisconnect(NodeAddress address)
        {
            Node.ChildDisconnect(address);
        }

        /// <inheritdoc/>
        public void Send(string type, byte[] data, NodeAddress from)
        {
            Node.Send(type, data, from);
        }

        /// <inheritdoc/>
        public void ConnectChild(INode node)
        {
            Node.ConnectChild(node);
        }
    }
}
