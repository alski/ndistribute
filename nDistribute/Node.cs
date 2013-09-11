namespace nDistribute
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>The node.</summary>
    [DebuggerDisplay("{Address}")]
    public class Node : NodeWithoutContract
    {
        /// <summary>Initialises a new instance of the <see cref="Node"/> class.</summary>
        /// <remarks>You can't call this, instead use <see cref="INetwork.FindOrCreate"/> which will create and track nodes for you  </remarks>
        /// <param name="newAddress">The new address.</param>
        /// <param name="locator">The locator used to connect to other nodes.</param>
        internal Node(NodeAddress newAddress, INetwork locator)
        {
            this.Address = newAddress;
            this.Network = locator;
            this.ElectionStrategy = new FirstComeFirstElectedStrategy();
        }

        /// <summary>Connects with a new node, by determining which is the parent.</summary>
        /// <remarks>Delegates to the <see cref="NodeWithoutContract.ElectionStrategy"/> to confirm if we should be the parent of the node.</remarks>
        /// <param name="newNode">The new node.</param>
        /// <returns>The <see cref="NodeAddress"/>.</returns>
        public override NodeAddress Connect(NodeAddress newNode)
        {
            if (this.Address.Address == newNode.Address)
            {
                throw new InvalidOperationException(this.Address + " - Can't connect to " + newNode);
            }

            var parent = this as INode;
            var child = this.Network.FindOrCreate(newNode);
            this.ElectionStrategy.DetermineParent(ref parent, ref child);
            if (parent == this)
            {
                ConnectChild(child);
                return this.Address;
            }
            else
            {
                child.AdviseConnect(this.Address);
                return null;
            }
        }

        public override void ConnectChild(INode child)
        {
            Contract.Assert(child.Address.Matches(Address.Parent) == false);

            if (child.Address.Parent != this.Address)
            {
                if (child.HasParent)
                {
                    var existingParent = this.Network.FindOrCreate(child.Address.Parent);
                    existingParent.ChildDisconnect(child.Address);
                    child.Address.Parent = null;
                }

                this.AddChild(child.Address);
                child.Address.Parent = this.Address;

                // Also register our address with the other network, which will call back into here but the 'if (parent==this)' in Connect  will stop the recursive call
                child.AdviseConnect(this.Address);
                OnIsConnectedChanged();
            }
        }

        /// <summary>Detach this node from its parent and advises each child to connect to the 
        /// parent individually via <see cref="AdviseConnect"/>.</summary>
        public override void Disconnect()
        {
            NodeAddress parent = this.Address.Parent;
            this.DetachFromParent();
            // Take a copy of the children so they disconnect at will
            foreach (var child in this.Children.ToArray())
            {
                var childNode = this.Network.FindOrCreate(child);
                childNode.AdviseConnect(parent);
            }
        }

        /// <summary>The advise connect.</summary>
        /// <param name="newParent">The parent.</param>
        public override void AdviseConnect(NodeAddress newParent)
        {
            if (HasParent && !this.Address.Parent.Matches(newParent))
            {
                this.DetachFromParent();
            }

            var parentNode = this.Network.FindOrCreate(newParent);
            var actualParentAddress = parentNode.Connect(this.Address);
            if (actualParentAddress.Matches( newParent))
            {
                this.Address.Parent = newParent;
                OnIsConnectedChanged();
            }            
        }

        /// <summary>Disconnect the given child.</summary>
        /// <param name="address">The address.</param>
        public override void ChildDisconnect(NodeAddress address)
        {
            Contract.Assert(Children.Any(x => x.Matches(address)));
            this.Children.RemoveAll(x => x.Matches(address));
        }

        /// <summary>Sends data across the network.</summary>
        /// <param name="data">The data.</param>
        /// <param name="type"><see cref="Type"/> to deserialize data back to.</param>
        public override void Send(string type, byte[] data, NodeAddress from)
        {
            this.Network.OnReceived(type, data);
            this.SendToNetwork(this.Network, type, data, from);
        }        

        public event EventHandler<EventArgs> IsConnectedChanged;

        protected void OnIsConnectedChanged()
        {
            var temp = IsConnectedChanged;
            if (temp != null)
            {
                temp(this, EventArgs.Empty);
            }
        }
    }
}