namespace nDistribute
{
    /// <summary>
    /// Representation of a communication channel
    /// </summary>
    public interface IChannel
    {
        /// <summary>
        /// Should be called by network to indicate data has arrived.
        /// </summary>
        /// <param name="bytes">Unstructured data that arrived.</param>
        void OnReceived(byte[] bytes);
    }
}