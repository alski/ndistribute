namespace nDistribute.WCF
{
    using System.ServiceModel;

    using nDistribute;
    using System;

    /// <summary>The remote connection service.</summary>
    [ServiceBehavior(
        ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.Single,
        IncludeExceptionDetailInFaults = true)]
    public class RemoteConnectionService : INode
    {
        public Node Node { get; set; }

        public NodeAddress Address
        {
            get { return Node.Address; }
        }

        public bool HasParent
        {
            get { return Node.HasParent; }
        }

        public bool HasChild(NodeAddress address)
        {
            return Node.HasChild(address);
        }

        public void Disconnect()
        {
            Node.Disconnect();
        }

        public void SendToNetwork(INetwork network, string type, byte[] data, NodeAddress from)
        {
            Node.SendToNetwork(network, type, data, from);
        }

        public NodeAddress Connect(NodeAddress newNode)
        {
            return Node.Connect(newNode);
        }

        public void AdviseConnect(NodeAddress newParent)
        {
            Node.AdviseConnect(newParent);
        }

        public void ChildDisconnect(NodeAddress address)
        {
            Node.ChildDisconnect(address);
        }

        public void Send(string type, byte[] data, NodeAddress from)
        {
            Node.Send(type, data, from);
        }

        public bool IsConnected
        {
            get { return Node.IsConnected; }
        }

        public void ConnectChild(INode node)
        {
            Node.ConnectChild(node);
        }

    }
}





