namespace nDistribute.Tests.ThatNetworkManager
{
    using NUnit.Framework;
    using Should;

    /// <summary>
    /// Tests for <see cref="NetworkManager.ConnectTo(NodeAddress)"/>
    /// </summary>
    [TestFixture]
    public class ConnectToMethod
    {
        /// <summary>
        /// Tests that the ConnectTo method returns a connection.
        /// </summary>
        [Test]
        public void ShouldConnect()
        {
            var xxxx = InProcessNetwork.Create("xxxx");

            var manager = new NetworkManager();
            InProcessNetwork.Register(manager);
            var connection = manager.ConnectTo(xxxx.Local.Address);
            connection.ShouldNotBeNull();
        }

        /// <summary>
        /// Should not connect if unregistered.
        /// </summary>
        [Test]
        public void ShouldNotConnectIfUnregistered()
        {
            var manager = new NetworkManager();
            var connection = manager.ConnectTo(new NodeAddress(InProcessNetwork.SchemaName + ":xxxx"));
            connection.ShouldBeNull();
        }
    }
}
