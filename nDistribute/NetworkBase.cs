namespace nDistribute
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;

    [Flags]
    public enum StartupPolicy
    {
        FindNewAddressIfAlreadyInUse = 1, //0b01,
        ReconnectPreviousAddresses = 2, //0b10,
        Normal = FindNewAddressIfAlreadyInUse | ReconnectPreviousAddresses
    }

    /// <summary>The node locator.</summary>
    public abstract class NetworkBase : INetwork
    {
        static NetworkBase()
        {
            Formatter = new BinaryFormatter();
        }

        public static string SchemaName { get; protected set; }

        private readonly List<INode> connectedNodes = new List<INode>();

        private Lazy<INode> localNode;

        internal NetworkRegistration AsRegistration()
        {
            return new NetworkRegistration
            {
                CanCreate = this.CanCreate,
                CreateNetwork = () => this
            };
        }

        private IList<IChannel> channels = new List<IChannel>();

        /// <summary>Initialises a new instance of the <see cref="NetworkBase"/> class.</summary>
        protected NetworkBase()
        {
            ResetLocal();
        }

        public NetworkBase(StartupPolicy startupPolicy)
            : this()
        {
            StartupPolicy = startupPolicy;
        }

        public IEnumerable<IChannel> Channels => channels;

        public event EventHandler<IChannel> ChannelCreated;

        internal virtual bool CanCreate(NodeAddress remoteAddress)
        {
            return remoteAddress.AsString.StartsWith(SchemaName);
        }

        public static BinaryFormatter Formatter { get; set; }

        /// <summary>Gets the local Node.</summary>
        public INode Local
        {
            get { return localNode.Value; }
        }

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

        /// <summary>Creates nodes representing other nodes.</summary>
        /// <param name="address">The address.</param>
        /// <returns>The <see cref="INode"/>.</returns>
        protected abstract INode Create(NodeAddress address);

        /// <summary>Creates the local Node.</summary>
        /// <returns>The <see cref="INode"/>.</returns>
        protected abstract INode CreateLocal();

        protected void ResetLocal()
        {
            localNode = new Lazy<INode>(CreateLocal);
        }

        protected abstract NodeAddress GetDefaultNode();
       
        /// <summary>Add a node to the network.</summary>
        /// <param name="child">The child.</param>
        public virtual void Connect(NodeAddress child)
        {
            Local.Connect(child);
        }

        /// <summary>
        /// Reads a previously stored configuration, 
        /// </summary>
        /// <param name="configuration"></param>
        protected Tuple<string, IEnumerable<string>> ParseConfiguration(string configuration)
        {
            var parts = (configuration ?? "").Split('=');
            var local = parts[0];
            var remote = parts.Length > 1
                ? parts[1].Split('|')
                : Enumerable.Empty<string>();
            return new Tuple<string, IEnumerable<string>>(local, remote);
        }

        //todo: Make this protected 
        public void OnReceived(string typeName, byte[] bytes)
        {
            var type = Type.GetType(typeName, throwOnError: true);
            var channel = GetChannel(type);
            channel.OnReceived(bytes);
        }

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


        private IEnumerable<string> GetConnections()
        {
            if (localNode.Value.Address.Parent != null)
                yield return localNode.Value.Address.Parent.AsString;

            foreach (var x in (localNode.Value as Node)?.Children)
                yield return x.AsString;
        }

        public bool IsConnected { get { return Local.IsConnected; } }

        public StartupPolicy StartupPolicy { get; }

        public event EventHandler<ConnectedEventArgs> IsConnectedChanged;

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

        private void OnChannelCreated(IChannel found)
        {
            ChannelCreated?.Invoke(this, found);
        }

        internal string GetConfiguration()
        {
            return Local.Address.ToString() + "=" + string.Join("|", Connections);
        }

        internal IEnumerable<string> Connections => connectedNodes.Select(x => x.Address.AsString);
    }

    public class ConnectedEventArgs : EventArgs
    {
        public IEnumerable<string> Connected { get; set; }
    }
}