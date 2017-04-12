namespace nDistribute.Tests
{
    using NUnit.Framework;
    using Should;

    /// <summary>
    /// Tests for inprocess communication 
    /// </summary>
    [TestFixture]
    public class InProcessTests
    {
        /// <summary>
        /// Shoudl create a local node
        /// </summary>
        [Test]
        public void ShouldCreateLocalNode()
        {
            var network1 = InProcessNetwork.Create();
            var node1 = network1.Local;
            node1.ShouldNotBeNull();
            node1.Address.AsString.ShouldEqual(InProcessNetwork.SchemaName + ":ShouldCreateLocalNode");
        }

        /// <summary>
        /// The inprocess network should not share nodes
        /// </summary>
        [Test]
        public void ShouldNotShareNodes()
        {
            var network1 = InProcessNetwork.Create();
            var node1 = network1.Local;
            node1.ShouldNotBeType<InProcessProxyNode>();
            node1.ShouldBeType<Node>();

            var network2 = InProcessNetwork.Create("ShouldNotShareNodes2");
            var node2 = network2.Local;
            node2.ShouldNotBeType<InProcessProxyNode>();
            node2.ShouldBeType<Node>();
            
            node1.Connect(node2.Address);
            var network1Node2 = network1.FindOrDefault(node2.Address);
            network1Node2.ShouldBeType<InProcessProxyNode>();

            node2.ShouldNotBeType<InProcessProxyNode>();
            node2.ShouldBeType<Node>();

            node2.HasParent.ShouldBeTrue();
            node2.Address.Parent.Matches(node1.Address).ShouldBeTrue();

            network1Node2.HasParent.ShouldBeTrue();
            network1Node2.Address.Parent.Matches(node1.Address).ShouldBeTrue();

            node1.HasChild(node2.Address).ShouldBeTrue();

            var network2Node1 = network2.FindOrDefault(node1.Address);
            network2Node1.ShouldNotBeNull();
        }
    }
}
