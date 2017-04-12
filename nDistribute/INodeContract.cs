namespace nDistribute
{
    using System.ServiceModel;

    /// <summary>The Node remote contract. This should be implemented to facilitate communication between nodes.</summary>
    [ServiceContract]
    public interface INodeContract
    {
        /// <summary>Ask the node given in the <paramref name="newNode"/> parameter who we should connect to.</summary>
        /// <remarks>The <see cref="Node.ElectionStrategy"/> is used to decide who we should actually connect to as a child. 
        /// This could mean that this node actually becomes the child of a completely different node, for example when the 
        /// node we asked is already full and delegates us to its child. Alternatively it could decide that we shouldn't be anyone's child,
        /// so it will attempt to connect to us by calling our <see cref="Connect"/> instead.</remarks>
        /// <param name="newNode">The new node.</param>
        /// <returns>The <see cref="NodeAddress"/> of the node we should connect to.</returns>        
        [OperationContract]
        NodeAddress Connect(NodeAddress newNode);

        /// <summary>Advise the node who should connect to.</summary>
        /// <remarks>Whenever a node is connecting or if its connection should change then this call is made. It is used in several scenarios
        /// <list >
        /// <value>Under normal <see cref="Connect"/> then the parent node will advise the child node using <see cref="AdviseConnect"/> that it is connected. 
        /// This results in the child calling <see cref="Connect"/> on the parent again, which is ignored as that is the desired state already.</value>
        /// <value>
        /// When a parent has children and the parent is closing down, we will advise each child to connect to the 
        /// grandparent instead. </value>
        /// </list>
        /// (See <see cref="Connect"/> for a quick description of how the grandparent will delegate 
        /// connection to other nodes instead.) </remarks>
        /// <param name="newParent">The parent.</param>
        [OperationContract(IsOneWay=true)]
        void AdviseConnect(NodeAddress newParent);

        /// <summary>Advises a child node to disconnect from its parent.</summary>
        /// <param name="address">The address.</param>
        [OperationContract(IsOneWay=true)]
        void ChildDisconnect(NodeAddress address);

        /// <summary>Sends data across the network.</summary>
        /// <param name="type"><see cref="Type"/> that the data should be deserialized to.</param>
        /// <param name="data">The data.</param>
        /// <param name="from"> Where the data is coming from so you can avoid sending it back </param>
        [OperationContract(IsOneWay=true)]
        void Send(string type, byte[] data, NodeAddress from);
    }
}