namespace nDistribute.WCF
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.ServiceModel;

    using nDistribute;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>A network that uses WCF as an underlying transport.</summary>
    public class WCFNetwork : NetworkBase
    {
        static WCFNetwork()
        {
            SchemaName = Uri.UriSchemeNetTcp;
        }

        public static void Register()
        {
            NetworkManager.Register(
                new NetworkRegistration
                {
                    CanCreate = x => x.StartsWith(SchemaName),
                    CreateNetwork = y => new WCFNetwork(y)
                });
        }

        private ServiceHost _host;
        private Node _node;

        /// <summary>Initialises a new instance of the <see cref="WCFNetwork"/> class.</summary>
        public WCFNetwork(int port)
            : this(new NodeAddress(SchemaName, Environment.MachineName, port))
        { }

        private WCFNetwork(string serialisedEndpoint)
            : this(new NodeAddress(serialisedEndpoint))
        { }

        private WCFNetwork(NodeAddress address)
        {
            //if (RemoteConnectionService.Node != null)
            //    throw new InvalidOperationException("WCFNetwork does not support more than one network per process in this version.");
            _node = new Node(address, this);
            _node.IsConnectedChanged += Node_IsConnectedChanged;
        }

        void Node_IsConnectedChanged(object sender, EventArgs e)
        {
            OnIsConnectedChanged();
        }

        protected void OnIsConnectedChanged()
        {
            IsConnectedChanged?.Invoke(this, new ConnectedEventArgs { Connected = GetConnections().ToArray() });
        }

        private IEnumerable<string> GetConnections()
        {
            if (_node.Address.Parent != null)
                yield return _node.Address.Parent.Address;

            foreach (var x in _node.Children)
                yield return x.Address;
        }

        public NodeAddress Address { get { return _node.Address; } }

        /// <summary>Gets or sets the service.</summary>
        private RemoteConnectionService Service { get; set; }

        /// <summary>Starts the WCF host.</summary>
        /// <exception cref="InvalidOperationException">Occurs if you try to start the host when it is already running.</exception>
        public void Start()
        {
            if (IsStarted())
            {
                throw new InvalidOperationException("Service is already running. Use Stop first.");
            }

            Contract.EndContractBlock();

            Service = new RemoteConnectionService { Node = _node };
            _host = new ServiceHost(Service, new Uri(Address.Address));
            _host.AddServiceEndpoint(typeof(INodeContract), new NetTcpBinding(), Address.Address);
            _host.Open();
            Service.Node = _node;
        }

        private bool IsStarted()
        {
            return _host != null;
        }

        /// <summary>Stops the WCF host.</summary>
        internal void Stop()
        {
            _host.Close();
            _host = null;
        }

        /// <summary>Creates nodes representing other nodes.</summary>
        /// <param name="address">The address.</param>
        /// <returns>The <see cref="INode"/>.</returns>
        protected override INode Create(NodeAddress address)
        {
            var binding = new NetTcpBinding();
            if (Debugger.IsAttached)
            {
                binding.CloseTimeout = TimeSpan.FromMinutes(1);
            }

            var client = new NetworkServiceClient.NodeContractClient(
                binding,
                new EndpointAddress(address.Address));
            client.Open();
            return new WCFClientNode(client, address, this);
        }

        /// <summary>Creates the local Node.</summary>
        /// <returns>The <see cref="INode"/>.</returns>
        protected override INode CreateLocal()
        {
            //Should never be called, but just in case....
            return _node;
        }

        /// <summary>
        /// Used in unit testing only.
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        internal bool HasChild(NodeAddress child)
        {
            return Local.HasChild(child);
        }

        public bool IsConnected { get { return Local.IsConnected; } }

        public event EventHandler<ConnectedEventArgs> IsConnectedChanged;

        public override void Connect(NodeAddress child)
        {
            if (IsStarted() == false)
                Start();

            base.Connect(child);
        }
    }

    public class ConnectedEventArgs : EventArgs
    {
        public IEnumerable<string> Connected { get; set; }
    }
}
