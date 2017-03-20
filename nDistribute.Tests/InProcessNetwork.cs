namespace nDistribute.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using nDistribute;
    using System.Runtime.CompilerServices;
    using System.Diagnostics.Contracts;

    /// <summary>An in-process network.</summary>
    internal class InProcessNetwork : NetworkBase
    {
        static InProcessNetwork()
        {
            SchemaName = "inprocess";
        }

        public static void Register(NetworkManager manager)
        {
            manager.Register(
                new NetworkRegistration
                {
                    CanCreate = x => x.Address.StartsWith(SchemaName + ":"),
                    CreateNetwork = () => new InProcessNetwork()
                });
        }

        internal static List<InProcessNetwork> Networks = new List<InProcessNetwork>();

        private string testMemberName;

        internal static InProcessNetwork Create([CallerMemberName] string testMethod = "")
        {
            var result = new InProcessNetwork()
            {
                testMemberName = testMethod
            };
            return result;
        }

        /// <summary>Initialises a new instance of the <see cref="InProcessNetwork"/> class.</summary>
        private InProcessNetwork()
        {
            AddNetwork(this);
        }

        internal string NetworkName { get; private set; }

        private void AddNetwork(InProcessNetwork network)
        {
            Contract.Requires(!Networks.Contains(network));

            Networks.Add(this);
        }

        /// <summary>Creates the local Node.</summary>
        /// <returns>The <see cref="INode"/>.</returns>
        protected override INode CreateLocal()
        {
            return new Node(new NodeAddress(InProcessNetwork.SchemaName+":"+ testMemberName), this);
        }

        /// <summary>The create.</summary>
        /// <param name="address">The address.</param>
        /// <returns>The <see cref="INode"/>.</returns>
        protected override INode Create(NodeAddress address)
        {
            if (!address.Equals(Local))
            {
                return new InProcessProxyNode(address, this);
            }
            return new Node(address, this);
        }
    }
}
