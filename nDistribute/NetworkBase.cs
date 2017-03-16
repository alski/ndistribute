namespace nDistribute
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;

    /// <summary>The node locator.</summary>
    public abstract class NetworkBase : INetwork
    {
        static NetworkBase()
        {
            Formatter = new BinaryFormatter(); 
        }

        public static string SchemaName { get; protected set; }

        private readonly List<INode> cachedNodes = new List<INode>();

        private readonly Lazy<INode> localNode;

        private IList<IChannel> channels = new List<IChannel>();

        /// <summary>Initialises a new instance of the <see cref="NetworkBase"/> class.</summary>
        protected NetworkBase()
        {
            localNode = new Lazy<INode>(BuildLocal);
        }

        public IEnumerable<IChannel> Channels => channels;

        public event EventHandler<IChannel> ChannelCreated;
        
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
                cachedNodes.Add(found);
            }
            return found;
        }

        /// <summary>Finds the <see cref="Node"/> for the given <see cref="NodeAddress"/>.</summary>
        /// <param name="address">The <see cref="NodeAddress"/> to find.</param>
        /// <returns>The found <see cref="Node"/> or null.</returns>
        public INode FindOrDefault(NodeAddress address)
        {
            var found = (from x in cachedNodes where x.Address.Matches(address) select x).FirstOrDefault();
            return found;
        }

        /// <summary>Creates nodes representing other nodes.</summary>
        /// <param name="address">The address.</param>
        /// <returns>The <see cref="INode"/>.</returns>
        protected abstract INode Create(NodeAddress address);

        /// <summary>Creates the local Node.</summary>
        /// <returns>The <see cref="INode"/>.</returns>
        protected abstract INode CreateLocal();

        private INode BuildLocal()
        {
            var newNode = CreateLocal();
            cachedNodes.Add(newNode);
            return newNode;
        }

        /// <summary>Add a node to the network.</summary>
        /// <param name="child">The child.</param>
        public virtual void Connect(NodeAddress child)
        {
            Local.Connect(child);
        }

        public void Connect(string address)
        {
            Connect(new NodeAddress(address));
        }


        //todo: Make this protected 
        public void OnReceived(string typeName, byte[] bytes)
        {
            var type = Type.GetType(typeName, throwOnError: true);
            var channel = GetChannel(type);
            channel.OnReceived(bytes);
        }

        private IChannel GetChannel(Type type)
        {
            var found = (from x in Channels
                         where x.GetType().GenericTypeArguments.First() == type
                         select x).FirstOrDefault();
            if (found == null)
            {
                found = (IChannel) Activator.CreateInstance(type, new object[] { this });
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
            return Local.Address.Address + "=" + string.Join("|", Connections);
        }

        internal IEnumerable<string> Connections => cachedNodes.Select(x => x.Address.Address);
    }
}