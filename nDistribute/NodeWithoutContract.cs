namespace nDistribute
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>The node without contract.</summary>
    public abstract class NodeWithoutContract : INode
    {        
        /// <summary>Gets or sets the address.</summary>
        public NodeAddress Address { get; protected set; }

        /// <summary>Gets a value indicating whether has parent.</summary>
        public bool HasParent
        {
            get { return Address.Parent != null; }
        }

        /// <summary>
        /// Gets a value indicating whether this node is connected to others.
        /// </summary>
        public bool IsConnected
        {
            get { return Address.Parent != null || Children.Any(); }
        }
    
        /// <summary>Gets the children.</summary>
        protected internal List<NodeAddress> Children { get; } = new List<NodeAddress>();

        /// <summary>Gets or sets the <see cref="INetwork"/>.</summary>
        protected INetwork Network { get; set; }

        /// <summary>Gets or sets the election strategy.</summary>
        protected IElectionStrategy ElectionStrategy { get; set; }

        /// <summary>Determines if this <see cref="Node"/> has the given child.</summary>
        /// <param name="address">The address.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool HasChild(NodeAddress address)
        {
            return Children.Any(x => x.Matches(address));
        }

        /// <summary>The disconnect.</summary>
        public abstract void Disconnect();

        /// <summary>Ask the node given in the <paramref name="newNode"/> parameter who we should connect to.</summary>
        /// <remarks>The <see cref="Node.ElectionStrategy"/> is used to decide who we should actually connect to as a child. 
        /// This could mean that this node actually becomes the child of a completely different node, for example when the 
        /// node we asked is already full and delegates us to its child. Alternatively it could decide that we shouldn't be anyone's child,
        /// so it will attempt to connect to us by calling our <see cref="INodeContract.Connect"/> instead.</remarks>
        /// <param name="newNode">The node we think we want to connect to.</param>
        /// <returns>The node we actually did connect to.</returns>        
        public abstract NodeAddress Connect(NodeAddress newNode);

        /// <summary>Send advise to another node of who it should connect to.</summary>
        /// <remarks>When a parent has children and the parent is closing down, we will advise each child to connect to the 
        /// grandparent instead. (See <see cref="INodeContract.Connect"/> for a quick description of how the grandparent will delegate 
        /// connection to other nodes instead.</remarks>
        /// <param name="newParent">The parent.</param>
        public abstract void AdviseConnect(NodeAddress newParent);

        /// <summary>Advises a child node to disconnect from its parent.</summary>
        /// <param name="address">The address.</param>        
        public abstract void ChildDisconnect(NodeAddress address);

        /// <summary>Sends data across the network.</summary>
        /// <param name="type"><see cref="Type"/> that the data should be deserialized to.</param>
        /// <param name="data">The data.</param>
        /// <param name="from">Where not to send data back to.</param>
        public abstract void Send(string type, byte[] data, NodeAddress from);

        /// <summary>
        /// Sends some data to the entire network.
        /// </summary>
        /// <param name="network">Where to send the data.</param>
        /// <param name="type">Type of data to send.</param>
        /// <param name="data">Untyped data to send.</param>
        /// <param name="from">Where this data originates from.</param>
        public void SendToNetwork(INetwork network, string type, byte[] data, NodeAddress from)
        {
            var recipient = network.FindOrDefault(Address.Parent);
            if (recipient != null
                && recipient.Address.Matches(from) == false)
            {
                recipient.Send(type, data, Address);
            }

            foreach (var x in Children)
            {
                if (x.Matches(from) == false)
                {
                    recipient = network.FindOrDefault(x);
                    recipient.Send(type, data, Address);
                }
            }
        }     

        /// <summary>
        /// Connects a child node to this one.
        /// </summary>
        /// <param name="node">Child to connect.</param>
        public abstract void ConnectChild(INode node);

        /// <summary>Detaches this node from its parent.</summary>
        protected void DetachFromParent()
        {
            var parent = Address.Parent;
            var parentNode = Network.FindOrCreate(parent);
            parentNode.ChildDisconnect(Address);
            Address.Parent = null;
        }

        /// <summary>Registers a child node.</summary>
        /// <param name="address">The address.</param>
        protected void AddChild(NodeAddress address)
        {
            Children.Add(address);
        }
    }
}
