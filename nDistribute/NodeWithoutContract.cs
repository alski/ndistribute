namespace nDistribute
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>The node without contract.</summary>
    public abstract class NodeWithoutContract : INode
    {
        /// <summary>The _children.</summary>
        protected internal readonly List<NodeAddress> Children = new List<NodeAddress>();

        /// <summary>Gets or sets the address.</summary>
        public NodeAddress Address { get; protected set; }

        /// <summary>Gets a value indicating whether has parent.</summary>
        public bool HasParent
        {
            get { return this.Address.Parent != null; }
        }

        /// <summary>Gets or sets the <see cref="INetwork"/>.</summary>
        protected INetwork Network { get; set; }

        /// <summary>Gets or sets the election strategy.</summary>
        protected IElectionStrategy ElectionStrategy { get; set; }

        /// <summary>Determines if this <see cref="Node"/> has the given child.</summary>
        /// <param name="address">The address.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool HasChild(NodeAddress address)
        {
            return this.Children.Any(x => x.Matches(address));
        }

        /// <summary>The disconnect.</summary>
        public abstract void Disconnect();

        /// <summary>Ask the node given in the <paramref name="newNode"/> parameter who we should connect to.</summary>
        /// <remarks>The <see cref="Node.ElectionStrategy"/> is used to decide who we should actually connect to as a child. 
        /// This could mean that this node actually becomes the child of a completely different node, for example when the 
        /// node we asked is already full and delegates us to its child. Alternatively it could decide that we shouldn't be anyone's child,
        /// so it will attempt to connect to us by calling our <see cref="INodeContract.Connect"/> instead.</remarks>
        /// <param name="newNode">The new node.</param>        
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
        /// <param name="from">Where not to send data back to.</param>
        /// <param name="data">The data.</param>
        /// <param name="type"><see cref="Type"/> that the data should be deserialized to.</param>
        public abstract void Send(string type, byte[] data, NodeAddress from);

        /// <summary>Detaches this node from its parent.</summary>
        /// <returns>The <see cref="bool"/>.</returns>
        protected void DetachFromParent()
        {
            var parent = this.Address.Parent;
            var parentNode = this.Network.FindOrCreate(parent);
            parentNode.ChildDisconnect(this.Address);
            this.Address.Parent = null;
        }

        /// <summary>Registers a child node.</summary>
        /// <param name="address">The address.</param>
        protected void AddChild(NodeAddress address)
        {
            this.Children.Add(address);
        }

        public void SendToNetwork(INetwork network, string type, byte[] data, NodeAddress from)
        {
            var recipient = network.FindOrDefault(this.Address.Parent);
            if (recipient != null
                && recipient.Address.Matches(from) == false)
            {
                recipient.Send(type, data, this.Address);
            }

            foreach (var x in this.Children)
            {
                if (x.Matches(from) == false)
                {
                    recipient = network.FindOrDefault(x);
                    recipient.Send(type, data, this.Address);
                }
            }
        }

        public bool IsConnected { get { return Address.Parent != null || Children.Any(); } }

        public abstract void ConnectChild(INode node);
    }
}
