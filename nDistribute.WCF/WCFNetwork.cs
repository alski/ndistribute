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

        private ServiceHost _host;
        private NodeAddress localAddress;

        /// <summary>Initialises a new instance of the <see cref="WCFNetwork"/> class.</summary>
        public WCFNetwork(Func<string> getConfig = null, StartupPolicy startupPolicy = StartupPolicy.Normal )
            : base(startupPolicy)
        {
            var config = getConfig == null ? null : getConfig();
            var connections = ParseConfiguration(config);
            localAddress = connections?.Item1 == null
                ? new NodeAddress(SchemaName, Environment.MachineName, NetworkManager.FindFreeTcpPort())
                : new NodeAddress(connections.Item1);;
        }

        private NodeAddress GetDefaultNode()
        {
            return new NodeAddress(SchemaName, Environment.MachineName, NetworkManager.FindFreeTcpPort());
        }

        private Node BuildNode(NodeAddress node)
        {
             var result= new Node(node, this);
            result.IsConnectedChanged += Node_IsConnectedChanged;
            return result;
        }

        void Node_IsConnectedChanged(object sender, EventArgs e)
        {
            OnIsConnectedChanged();
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
            catch(Exception ex)
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

        private void StartService()
        {
            Service = new RemoteConnectionService { Node = Local as Node};
            _host = new ServiceHost(Service, new Uri(Local.Address));
            _host.AddServiceEndpoint(typeof(INodeContract), new NetTcpBinding(), Local.Address);
            _host.Open();
            Service.Node = Local as Node;
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
            return BuildNode(localAddress);
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

        
        public override void Connect(NodeAddress child)
        {
            if (IsStarted() == false)
                Start();

            base.Connect(child);
        }
    }
}
