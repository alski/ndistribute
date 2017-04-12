namespace nDistribute.WCF
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.ServiceModel;
    using nDistribute;

    /// <summary>A network that uses WCF as an underlying transport.</summary>
    public class WCFNetwork : NetworkBase
    {
        private ServiceHost host;
        private NodeAddress localAddress;

        static WCFNetwork()
        {
            SchemaName = Uri.UriSchemeNetTcp;
        }

        /// <summary>Initializes a new instance of the <see cref="WCFNetwork"/> class.</summary>
        /// <param name="getConfig">Means of getting configuration for reconnecting with.</param>
        /// <param name="startupPolicy"><see cref="StartupPolicy"/> defining how we startup.</param>
        public WCFNetwork(Func<string> getConfig = null, StartupPolicy startupPolicy = StartupPolicy.Normal)
            : base(startupPolicy)
        {
            var config = getConfig == null ? null : getConfig();
            var connections = ParseConfiguration(config);
            localAddress = connections?.Item1 == null
                ? new NodeAddress(SchemaName, Environment.MachineName, NetworkManager.FindFreeTcpPort())
                : new NodeAddress(connections.Item1); 
        }

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

            try
            {
                StartService();
            }
            catch (Exception)
            {
                if ((StartupPolicy & StartupPolicy.FindNewAddressIfAlreadyInUse) == StartupPolicy.FindNewAddressIfAlreadyInUse)
                {
                    localAddress = GetDefaultNode();
                    ResetLocal();

                    StartService();
                }
                else
                {
                    throw;
                }
            }
        }

        /// <inheritdoc/>
        public override void Connect(NodeAddress child)
        {
            if (IsStarted() == false)
            {
                Start();
            }

            base.Connect(child);
        }

        /// <summary>
        /// Used in unit testing only.
        /// </summary>
        /// <param name="child">The node we expect to have as a child.</param>
        /// <returns>Whether we have the given address as a child.</returns>
        internal bool HasChild(NodeAddress child)
        {
            return Local.HasChild(child);
        }

        /// <summary>Stops the WCF host.</summary>
        internal void Stop()
        {
            host.Close();
            host = null;
        }

        /// <inheritdoc/>
        protected override NodeAddress GetDefaultNode()
        {
            return new NodeAddress(SchemaName, Environment.MachineName, NetworkManager.FindFreeTcpPort());
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
                new EndpointAddress(address.AsString));
            client.Open();
            return new WCFClientNode(client, address, this);
        }

        /// <summary>Creates the local Node.</summary>
        /// <returns>The <see cref="INode"/>.</returns>
        protected override INode CreateLocal()
        {
            var result = new Node(GetDefaultNode(), this);
            result.IsConnectedChanged += Node_IsConnectedChanged;
            return result;
        }

        private void Node_IsConnectedChanged(object sender, EventArgs e)
        {
            OnIsConnectedChanged();
        }

        private void StartService()
        {
            Service = new RemoteConnectionService { Node = Local as Node };
            host = new ServiceHost(Service, new Uri(Local.Address.AsString));
            host.AddServiceEndpoint(typeof(INodeContract), new NetTcpBinding(), Local.Address.AsString);
            host.Open();
            Service.Node = Local as Node;
        }

        private bool IsStarted()
        {
            return host != null;
        } 
    }
}
