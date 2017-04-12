namespace nDistribute
{
    /// <summary>
    /// An interface that describes the common features of all networks.
    /// </summary>
    public interface INetwork
    {
        /// <summary>
        /// Finds or creates a <see cref="INode"/> for a given <see cref="NodeAddress"/> on the network.
        /// </summary>
        /// <param name="nodeAddress">The address that you want to be able to contact in the network.</param>
        /// <returns>A <see cref="INode"/> for the correct address.</returns>
        INode FindOrCreate(NodeAddress nodeAddress);

        /// <summary>
        /// Finds an existing <see cref="INode"/> for a given <see cref="NodeAddress"/> on the network.
        /// </summary>
        /// <param name="nodeAddress">The address that you want to be able to contact in the network.</param>
        /// <returns>A <see cref="INode"/> for the correct address or null if it did not exist.</returns>
        INode FindOrDefault(NodeAddress nodeAddress);

        /// <summary>
        /// Low level thing that poossibly should not be exposed.
        /// </summary>
        /// <param name="type">Name of the type that has been sent</param>
        /// <param name="data">Data that should be cast to the type.</param>
        void OnReceived(string type, byte[] data);
    }
}
