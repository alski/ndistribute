namespace nDistribute.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using nDistribute;
    using System.Runtime.CompilerServices;

    /// <summary>An in-process network.</summary>
    internal class InProcessNetwork : NetworkBase
    {
        public static void Register()
        {
            NetworkManager.Register(
                new NetworkRegistration
                {
                    CanCreate = x => x.StartsWith("inprocess:"),
                    CreateNetwork = x => new InProcessNetwork(x)
                });
        }

        internal static Dictionary<string, InProcessNetwork> Networks = new Dictionary<string, InProcessNetwork>();
        private string _testMemberName;

        internal static InProcessNetwork Create(string networkName = null, [CallerMemberName] string testMethod = "")
        {
            var name = string.IsNullOrEmpty(networkName) ? testMethod : testMethod + ":" + networkName;
            var result = new InProcessNetwork(name);
            result._testMemberName = testMethod;
            return result;
        }

        /// <summary>Initialises a new instance of the <see cref="InProcessNetwork"/> class.</summary>
        /// <param name="networkName">The node name.</param>
        /// <param name="localNodeName">The local Node Name.</param>
        /// <param name="alternateNetwork">Network to work with </param>
        private InProcessNetwork(string networkName)
        {
            this.NetworkName = networkName;
            
            this.AddNetwork(this);
        }

        internal string NetworkName { get; private set; }

        private void AddNetwork(InProcessNetwork network)
        {            
            Networks.Add(network.NetworkName, this);
        }

        /// <summary>Creates the local Node.</summary>
        /// <returns>The <see cref="INode"/>.</returns>
        protected override INode CreateLocal()
        {
            return Create(new NodeAddress(this.NetworkName));
        }

        /// <summary>The create.</summary>
        /// <param name="address">The address.</param>
        /// <returns>The <see cref="INode"/>.</returns>
        protected override INode Create(NodeAddress address)
        {
            if (address.Address != NetworkName)
            {
                return new InProcessProxyNode(address, this);
            }
            return new Node(address, this);
        }

        internal InProcessNetwork CreateNetwork(string newNetwork, [CallerMemberName] string networkPath = "")
        {
            if (_testMemberName != networkPath)
                throw new ArgumentException(string.Format("Networks should only be added inside the same test [{0} != {1}]", _testMemberName, networkPath));

            var result = new InProcessNetwork(networkPath + "." + newNetwork);
            return result;
        }
    }
}
