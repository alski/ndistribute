namespace nDistribute
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;

    /// <summary>
    /// The various options when the network is started up.
    /// </summary>
    [Flags]
    public enum StartupPolicy
    {
        /// <summary>
        /// Obsolete 
        /// </summary>
        FindNewAddressIfAlreadyInUse = 1, //0b01,

        /// <summary>
        /// Automatically attempt reconnect to all nodes we were connected to last time we ran
        /// </summary>
        ReconnectPreviousAddresses = 2, //0b10,

        /// <summary>
        ///  A combination of <see cref="FindNewAddressIfAlreadyInUse"/> and <see cref="ReconnectPreviousAddresses"/>.
        /// </summary>
        Normal = FindNewAddressIfAlreadyInUse | ReconnectPreviousAddresses
    }

    /// <summary>The node locator.</summary>
    public abstract class NetworkBase : INetwork
    {
        private readonly List<INode> connectedNodes = new List<INode>();
        private Lazy<INode> localNode;
        private IList<IChannel> channels = new List<IChannel>();

        static NetworkBase()
        {
            Formatter = new BinaryFormatter();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkBase"/> class.
        /// </summary>
        /// <param name="startupPolicy">The <see cref="StartupPolicy"/> that this network should have b</param>
        protected NetworkBase(StartupPolicy startupPolicy)
            : this()
        {
            StartupPolicy = startupPolicy;
        }

        /// <summary>Initializes a new instance of the <see cref="NetworkBase"/> class.</summary>
        protected NetworkBase()
        {
            ResetLocal();
        }

        /// <summary>
        /// Event to let you knwo when a channel is created.
        /// </summary>
        public event EventHandler<IChannel> ChannelCreated;

        /// <summary>
        /// Event for when the connection status changes.
        /// </summary>
        public event EventHandler<ConnectedEventArgs> IsConnectedChanged;

        /// <summary>
        /// Gets or sets formatter to use to convert from the data sent over the wire to the typed information that will be received in channels.
        /// </summary>
        public static BinaryFormatter Formatter { get; set; }

        /// <summary>
        /// Gets or sets the schema that this transport type should identify itself as.
        /// </summary>
        public static string SchemaName { get; protected set; }

        /// <summary>
        /// Gets the channels that this network will distribute information on.
        /// </summary>
        public IEnumerable<IChannel> Channels => channels;

        /// <summary>
        /// Gets a value indicating whether this network is connected.
        /// </summary>
        public bool IsConnected => Local.IsConnected; 

        /// <summary>
        /// Gets the <see cref="StartupPolicy"/> this network was created with.
        /// </summary>
        public StartupPolicy StartupPolicy { get; }

        /// <summary>Gets the local Node.</summary>
        public INode Local
        {
            get { return localNode.Value; }
        }

        /// <summary>
        /// Gets the current set of connections for display.
        /// </summary>
        internal IEnumerable<string> Connections => connectedNodes.Select(x => x.Address.AsString);

        /// <summary>Finds the <see cref="Node"/> for the given <see cref="NodeAddress"/>.</summary>
        /// <param name="address">The <see cref="NodeAddress"/> to find.</param>
        /// <returns>The found <see cref="Node"/>.</returns>
        public INode FindOrCreate(NodeAddress address)
        {
            var found = FindOrDefault(address);
            if (found == null)
            {
                found = Create(address);
                connectedNodes.Add(found);
            }

            return found;
        }

        /// <summary>Finds the <see cref="Node"/> for the given <see cref="NodeAddress"/>.</summary>
        /// <param name="address">The <see cref="NodeAddress"/> to find.</param>
        /// <returns>The found <see cref="Node"/> or null.</returns>
        public INode FindOrDefault(NodeAddress address)
        {
            var found = (from x in connectedNodes where x.Address.Matches(address) select x).FirstOrDefault();
            return found;
        }

        /// <summary>Add a node to the network.</summary>
        /// <param name="child">The child.</param>
        public virtual void Connect(NodeAddress child)
        {
            Local.Connect(child);
        }

        //todo: Make this protected 
        public void OnReceived(string typeName, byte[] bytes)
        {
            var type = Type.GetType(typeName, throwOnError: true);
            var channel = GetChannel(type);
            channel.OnReceived(bytes);
        }

        /// <summary>
        /// Gets the channel for sending and receiving messages of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of data to be transferred.</typeparam>
        /// <returns>A <see cref="NetworkChannel{T}"/> that can transfer data.</returns>
        public NetworkChannel<T> GetChannel<T>()
        {
            var found = (from x in Channels
                         let typedChannel = x as NetworkChannel<T>
                         where typedChannel != null
                         select typedChannel).FirstOrDefault();
            if (found == null)
            {
                found = new NetworkChannel<T>(this);
                channels.Add(found);
                OnChannelCreated(found);
            }

            return found;
        }

        /// <summary>
        /// Determines if this network can connect to a given <see cref="NodeAddress"/>
        /// </summary>
        /// <param name="remoteAddress">The remote address that we should detect if we can connect to.</param>
        /// <returns>True if we know how to connect to that type of address.</returns>
        internal virtual bool CanCreate(NodeAddress remoteAddress)
        {
            return remoteAddress.AsString.StartsWith(SchemaName);
        }

        /// <summary>
        /// Returns the network registration for this communication type.
        /// </summary>
        /// <returns>A <see cref="NetworkRegistration"/> that will be used when we register with <see cref="NetworkManager.Register(NetworkRegistration)"/></returns>
        internal NetworkRegistration AsRegistration()
        {
            return new NetworkRegistration
            {
                CanCreate = this.CanCreate,
                CreateNetwork = () => this
            };
        }

        /// <summary>
        /// Gets the configuration that can be used to restore this connection.
        /// </summary>
        /// <returns>A serialised configuration</returns>
        internal string GetConfiguration()
        {
            return Local.Address.ToString() + "=" + string.Join("|", Connections);
        }

        /// <summary>Creates nodes representing other nodes.</summary>
        /// <param name="address">The address.</param>
        /// <returns>The <see cref="INode"/>.</returns>
        protected abstract INode Create(NodeAddress address);

        /// <summary>Creates the local Node.</summary>
        /// <returns>The <see cref="INode"/>.</returns>
        protected abstract INode CreateLocal();
        
        /// <summary>
        ///  Sets up the lazy creation of the local node.
        /// </summary>
        /// <remarks>
        /// Can be used by inheriting classes should they need to reconnect, or when trying to connect and the preferred <see cref="NodeAddress"/> was unavailable.
        /// </remarks>
        protected void ResetLocal()
        {
            localNode = new Lazy<INode>(CreateLocal);
        }

        /// <summary>
        /// Builds the preferred local address
        /// </summary>
        /// <remarks>
        /// This can change between calls for example when a TCP port becomes used and a call to <see cref="NetworkManager.FindFreeTcpPort"/> returns a different one.
        /// </remarks>
        /// <returns>The address of the preferred local node.</returns>
        protected abstract NodeAddress GetDefaultNode();

        /// <summary>
        /// Reads a previously stored configuration, 
        /// </summary>
        /// <param name="configuration">The configuration to parse.</param>
        /// <returns>A tuple of preferred local address and a list of remote addresses that we were previous conected to.</returns>
        protected Tuple<string, IEnumerable<string>> ParseConfiguration(string configuration)
        {
            var parts = (configuration ?? string.Empty).Split('=');
            var local = parts[0];
            var remote = parts.Length > 1
                ? parts[1].Split('|')
                : Enumerable.Empty<string>();
            return new Tuple<string, IEnumerable<string>>(local, remote);
        }
        
        /// <summary>
        /// Handler for when the connection status changes.
        /// </summary>
        protected void OnIsConnectedChanged()
        {
            IsConnectedChanged?.Invoke(this, new ConnectedEventArgs { Connected = GetConnections().ToArray() });

            if (!IsConnected
                && (StartupPolicy & StartupPolicy.ReconnectPreviousAddresses) == StartupPolicy.ReconnectPreviousAddresses)
            {
                foreach (var node in connectedNodes)
                {
                    Connect(node.Address);
                }
            }
        }

        /// <summary>
        /// Gets the remote connections.
        /// </summary>
        /// <remarks>This is just a list of strings as this is only for display purposes.</remarks>
        /// <returns>A list of remote node names.</returns>
        private IEnumerable<string> GetConnections()
        {
            if (localNode.Value.Address.Parent != null)
            {
                yield return localNode.Value.Address.Parent.AsString;
            }

            foreach (var x in (localNode.Value as Node)?.Children)
            {
                yield return x.AsString;
            }
        }

        private IChannel GetChannel(Type type)
        {
            var found = (from x in Channels
                         where x.GetType().GenericTypeArguments.First() == type
                         select x).FirstOrDefault();
            if (found == null)
            {
                found = (IChannel)Activator.CreateInstance(type, new object[] { this });
                channels.Add(found);
                OnChannelCreated(found);
            }

            return found;
        }

        private void OnChannelCreated(IChannel found)
        {
            ChannelCreated?.Invoke(this, found);
        }
    }
}