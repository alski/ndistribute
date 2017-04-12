namespace nDistribute.Tests.ThatNetworkManager
{
    using NUnit.Framework;
    using Should;

    /// <summary>
    /// Tests for <see cref="INetwork.Connections"/>
    /// </summary>
    [TestFixture]
    public class ConnectionsProperty
    {
        /// <summary>
        /// The connections property should include a connected node name
        /// </summary>
        [Test]
        public void ShouldIncludeAConnectedNodeName()
        {
            var manager = new NetworkManager();
            InProcessNetwork.Register(manager);

            var network1 = InProcessNetwork.Create("1");
            var node1 = network1.Local;

            var network2 = InProcessNetwork.Create("2");
            network2.Connect(network1.Local.Address);

            network1.Connections.ShouldContain(network2.Local.Address.AsString);

            network2.Connections.ShouldContain(network1.Local.Address.AsString);
        }
    }
}
