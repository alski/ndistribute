namespace nDistribute
{
    using System;
    using System.IO;

    /// <summary>
    /// A communication channel to a Node.
    /// </summary>
    /// <typeparam name="T">The type of data that will be passed or recevied through the channel.</typeparam>
    public class NetworkChannel<T> : IChannel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkChannel{T}"/> class.
        /// </summary>
        /// <param name="network">The netwrok that this channel is to be created for.</param>
        public NetworkChannel(NetworkBase network)
        {
            Network = network;
        }

        /// <summary>
        /// Event that occurs whenever the channel receives data.
        /// </summary>
        public event EventHandler<T> Received;

        /// <summary>
        /// Gets the parent network that this channel is a member of.
        /// </summary>
        public NetworkBase Network { get; private set; }

        /// <inheritdoc/>
        public void OnReceived(byte[] bytes)
        {
            Received?.Invoke(this, Deserialize(bytes));
        }

        /// <summary>
        /// Sends data to the network.
        /// </summary>
        /// <param name="data">The information to send.</param>
        public void Send(T data)
        {
            var bytes = Serialize(data);
            Network.Local.SendToNetwork(Network, typeof(T).AssemblyQualifiedName, bytes, Network.Local.Address);
        }

        /// <summary>
        /// Conversion function to prepare data for sending over the wire.
        /// </summary>
        /// <param name="data">Data to be serialised.</param>
        /// <returns>An array of bytes.</returns>
        internal static byte[] Serialize(T data)
        {
            using (var ms = new MemoryStream())
            {
                NetworkBase.Formatter.Serialize(ms, data);
                return ms.GetBuffer();
            }
        }

        private static T Deserialize(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                return (T)NetworkBase.Formatter.Deserialize(ms);
            }
        }
    }
}