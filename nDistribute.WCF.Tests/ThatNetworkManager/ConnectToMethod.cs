namespace nDistribute.WCF.Tests.ThatNetworkManager
{
    using NUnit.Framework;
    using Should;

    /// <summary>
    /// Tests for the <see cref="NetworkManager.ConnectTo(NodeAddress)"/> method
    /// </summary>
    [TestFixture]
    public class ConnectToMethod
    {
        /// <summary>
        /// Tests that <see cref="NetworkManager.ConnectTo(NodeAddress)"/>  should connect
        /// </summary>
        [Test]
        public void ShouldConnect()
        {
            var registeredNetwork = new WCFNetwork();
            registeredNetwork.Start();

            var manager = new NetworkManager();
            manager.Register(registeredNetwork);

            var externalNetwork = new WCFNetwork();
            externalNetwork.Start();

            var connection = manager.ConnectTo(externalNetwork.Local.Address);
            connection.ShouldNotBeNull();
        }

        /// <summary>
        /// Tests that <see cref="NetworkManager.ConnectTo(NodeAddress)"/> should not connect if the network type is unregistered.
        /// </summary>
        [Test]
        public void ShouldNotConnectIfUnregistered()
        {
            var manager = new NetworkManager();
            var connection = manager.ConnectTo(new NodeAddress(WCFNetwork.SchemaName + ":xxxx"));
            connection.ShouldBeNull();
        }
    }
}
