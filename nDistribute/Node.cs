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
            Address = newAddress;
            Network = locator;
            ElectionStrategy = new FirstComeFirstElectedStrategy();
        }

        /// <summary>Connects with a new node, by determining which is the parent.</summary>
        /// <remarks>Delegates to the <see cref="NodeWithoutContract.ElectionStrategy"/> to confirm if we should be the parent of the node.</remarks>
        /// <param name="newNode">The new node.</param>
        /// <returns>The <see cref="NodeAddress"/>.</returns>
        public override NodeAddress Connect(NodeAddress newNode)
        {
            if (Address.Address == newNode.Address)
            {
                throw new InvalidOperationException(Address + " - Can't connect to " + newNode);
            }

            var parent = this as INode;
            var child = Network.FindOrCreate(newNode);
            ElectionStrategy.DetermineParent(ref parent, ref child);
            if (parent == this)
            {
                ConnectChild(child);
                return Address;
            }
            else
            {
                child.AdviseConnect(Address);
                return null;
            }
        }

        public override void ConnectChild(INode child)
        {
            Contract.Assert(child.Address.Matches(Address.Parent) == false);

            if (child.Address.Parent != Address)
            {
                if (child.HasParent)
                {
                    var existingParent = Network.FindOrCreate(child.Address.Parent);
                    existingParent.ChildDisconnect(child.Address);
                    child.Address.Parent = null;
                }

                AddChild(child.Address);
                child.Address.Parent = Address;

                // Also register our address with the other network, which will call back into here but the 'if (parent==this)' in Connect  will stop the recursive call
                child.AdviseConnect(Address);
                OnIsConnectedChanged();
            }
        }

        /// <summary>Detach this node from its parent and advises each child to connect to the 
        /// parent individually via <see cref="AdviseConnect"/>.</summary>
        public override void Disconnect()
        {
            NodeAddress parent = Address.Parent;
            DetachFromParent();
            // Take a copy of the children so they disconnect at will
            foreach (var child in Children.ToArray())
            {
                var childNode = Network.FindOrCreate(child);
                childNode.AdviseConnect(parent);
            }
        }

        /// <summary>The advise connect.</summary>
        /// <param name="newParent">The parent.</param>
        public override void AdviseConnect(NodeAddress newParent)
        {
            if (HasParent && !Address.Parent.Matches(newParent))
            {
                DetachFromParent();
            }

            var parentNode = Network.FindOrCreate(newParent);
            var actualParentAddress = parentNode.Connect(Address);
            if (actualParentAddress.Matches( newParent))
            {
                Address.Parent = newParent;
                OnIsConnectedChanged();
            }            
        }

        /// <summary>Disconnect the given child.</summary>
        /// <param name="address">The address.</param>
        public override void ChildDisconnect(NodeAddress address)
        {
            Contract.Assert(Children.Any(x => x.Matches(address)));
            Children.RemoveAll(x => x.Matches(address));
        }

        /// <summary>Sends data across the network.</summary>
        /// <param name="data">The data.</param>
        /// <param name="type"><see cref="Type"/> to deserialize data back to.</param>
        public override void Send(string type, byte[] data, NodeAddress from)
        {
            Network.OnReceived(type, data);
            SendToNetwork(Network, type, data, from);
        }        

        public event EventHandler<EventArgs> IsConnectedChanged;

        protected void OnIsConnectedChanged()
        {
            IsConnectedChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}