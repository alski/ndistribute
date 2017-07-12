namespace nDistribute.Tests
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Runtime.CompilerServices;
    using nDistribute;

    /// <summary>An in-process network.</summary>
    public class InProcessNetwork : NetworkBase
    {
        private string testMemberName;

        static InProcessNetwork()
        {
            SchemaName = "inprocess";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InProcessNetwork"/> class.
        /// </summary>
        private InProcessNetwork()
        {
            AddNetwork(this);
        }

        /// <summary>
        /// Gets a list of networks we know about just to help with debugging
        /// </summary>
        internal static List<InProcessNetwork> Networks { get; } = new List<InProcessNetwork>();

        /// <summary>
        /// Gets the unique identifier passed to this network on creation
        /// </summary>
        internal string NetworkName { get; private set; }

        /// <summary>
        /// Registers this type with the <see cref="NetworkManager"/>
        /// </summary>
        /// <param name="manager">The manager to registre with.</param>
        public static void Register(NetworkManager manager)
        {
            manager.Register(
                new NetworkRegistration
                {
                    CanCreate = x => x.AsString.StartsWith(SchemaName + ":"),
                    CreateNetwork = () => new InProcessNetwork()
                });
        }

        /// <summary>
        /// Factory method to create a new <see cref="InProcessNetwork"/>.
        /// </summary>
        /// <param name="testMethod">Name of the method that created this or other uniquely identifier to help with debugging</param>
        /// <returns>An <see cref="InProcessNetwork"/></returns>
        public static InProcessNetwork Create([CallerMemberName] string testMethod = "")
        {
            var result = new InProcessNetwork()
            {
                testMemberName = testMethod
            };
            return result;
        }

        /// <summary>Creates the local Node.</summary>
        /// <returns>The <see cref="INode"/>.</returns>
        protected override INode CreateLocal()
        {
            return new Node(GetDefaultNode(), this);
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

        /// <inheritdoc/>
        protected override NodeAddress GetDefaultNode()
        {
            return new NodeAddress(InProcessNetwork.SchemaName + ":" + testMemberName);
        }

        private void AddNetwork(InProcessNetwork network)
        {
            Contract.Requires(!Networks.Contains(network));

            Networks.Add(this);
        }
    }
}
