namespace nDistribute.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Runtime.CompilerServices;
    using nDistribute;
    using System.Threading.Tasks;
    using System.Linq;

    /// <summary>An in-process network.</summary>
    public class AsyncInProcessNetwork : NetworkBase
    {
        private string testMemberName;
        private readonly List<Task> tasks = new List<Task>();


        static AsyncInProcessNetwork()
        {
            SchemaName = "asyncinprocess";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncInProcessNetwork"/> class.
        /// </summary>
        private AsyncInProcessNetwork()
        {
            AddNetwork(this);
        }

        /// <summary>
        /// Gets a list of networks we know about just to help with debugging
        /// </summary>
        internal static List<AsyncInProcessNetwork> Networks { get; } = new List<AsyncInProcessNetwork>();

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
                    CreateNetwork = () => new AsyncInProcessNetwork()
                });
        }

        /// <summary>
        /// Factory method to create a new <see cref="InProcessNetwork"/>.
        /// </summary>
        /// <param name="testMethod">Name of the method that created this or other uniquely identifier to help with debugging</param>
        /// <returns>An <see cref="InProcessNetwork"/></returns>
        public static AsyncInProcessNetwork Create([CallerMemberName] string testMethod = "")
        {
            var result = new AsyncInProcessNetwork()
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

        internal void StartAsync(Action action)
        {
            var newTask = Task.Factory.StartNew(action);
            lock (tasks)
            {
                tasks.Add(newTask);
                tasks.RemoveAll(t => t.IsCompleted);
            }
        }

        public void WaitForTasks()
        {
            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>The create.</summary>
        /// <param name="address">The address.</param>
        /// <returns>The <see cref="INode"/>.</returns>
        protected override INode Create(NodeAddress address)
        {
            if (!address.Equals(Local))
            {
                return new AsyncInProcessProxyNode(address, this);
            }

            return new Node(address, this);
        }

        /// <inheritdoc/>
        protected override NodeAddress GetDefaultNode()
        {
            return new NodeAddress(AsyncInProcessNetwork.SchemaName + ":" + testMemberName);
        }

        private void AddNetwork(AsyncInProcessNetwork network)
        {
            Contract.Requires(!Networks.Contains(network));

            Networks.Add(this);
        }
    }

}
