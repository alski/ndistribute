namespace nDistribute
{
    using System;
    using System.Collections.Generic;

    /// <summary>The interface a Node should fully implement including both its remote and local operations.</summary>
    public interface INode : INodeContract
    {
        /// <summary>Gets the address.</summary>
        NodeAddress Address { get; }

        /// <summary>Gets a value indicating whether has parent.</summary>
        bool HasParent { get; }

        /// <summary>The has child.</summary>
        /// <param name="address">The address.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        bool HasChild(NodeAddress address);

        /// <summary>The disconnect.</summary>
        void Disconnect();

        /// <summary>Collaborates with a network to send data around it.</summary>
        /// <param name="network">The network.</param>
        /// <param name="data">The data.</param>
        /// <param name="type"><see cref="Type"/> to deserialize data back to.</param>
        /// <param name="from">Where not to send the data back to.</param>
        void SendToNetwork(INetwork network, string type, byte[] data, NodeAddress from);

        /// <summary>
        /// Confirms if this <see cref="INode"/> has a parent or children.
        /// </summary>
        bool IsConnected { get; }
       
        void ConnectChild(INode node);
    }
}