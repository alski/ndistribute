namespace nDistribute.Tests.ThatNode
{
    using System.Diagnostics.CodeAnalysis;
    using Moq;
    using NUnit.Framework;
    using Should;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "TestFixture.")]
    [TestFixture]
    public class SendToNetworkMethod
    {
        [Test]
        public void ShouldReceiveOnChildWhenSentToNetwork()
        {
            var childAddress = new NodeAddress("child");
            var localAddress = new NodeAddress("local");
            var data = "Hello world";

            var child = new Mock<INode>();
            child.Setup(x => x.Address).Returns(childAddress);
            var network = new Mock<INetwork>();
            network.Setup(x => x.FindOrDefault(null)).Returns((INode)null);
            network.Setup(x => x.FindOrCreate(childAddress)).Returns(child.Object);
            network.Setup(x => x.FindOrDefault(childAddress)).Returns(child.Object);

            var local = new Node(localAddress, network.Object);
            local.Connect(childAddress);
            local.SendToNetwork(network.Object, typeof(string).ToString(), NetworkChannel<string>.Serialize(data), null);

            child.Verify(x => x.Send(typeof(string).ToString(), It.IsAny<byte[]>(), It.IsAny<NodeAddress>()));
        }

        [Test]
        public void ShouldReceiveOnParentWhenSentToNetwork()
        {
            var parentAddress = new NodeAddress("parent");
            var localAddress = new NodeAddress("local") { Parent = parentAddress };
            var data = "Hello world";

            var parent = new Mock<INode>();
            parent.Setup(x => x.Address).Returns(parentAddress);
            var network = new Mock<INetwork>();
            network.Setup(x => x.FindOrDefault(null)).Returns((INode)null);
            network.Setup(x => x.FindOrCreate(parentAddress)).Returns(parent.Object);
            network.Setup(x => x.FindOrDefault(parentAddress)).Returns(parent.Object);

            var local = new Node(localAddress, network.Object);
            local.SendToNetwork(network.Object, typeof(string).ToString(), NetworkChannel<string>.Serialize(data), null);

            parent.Verify(x => x.Send(typeof(string).ToString(), It.IsAny<byte[]>(), It.IsAny<NodeAddress>()));
        }

        [Test]
        public void ShouldNotReceiveOnChildWhenSentToNetworkFromChild()
        {
            var childAddress = new NodeAddress("child");
            var localAddress = new NodeAddress("local");
            var data = "Hello world";

            var child = new Mock<INode>();
            child.Setup(x => x.Address).Returns(childAddress);
            var network = new Mock<INetwork>();
            network.Setup(x => x.FindOrDefault(null)).Returns((INode)null);
            network.Setup(x => x.FindOrCreate(childAddress)).Returns(child.Object);
            network.Setup(x => x.FindOrDefault(childAddress)).Returns(child.Object);

            var local = new Node(localAddress, network.Object);
            local.Connect(childAddress);
            local.SendToNetwork(network.Object, typeof(string).ToString(), NetworkChannel<string>.Serialize(data), childAddress);

            child.Verify(x => x.Send(typeof(string).ToString(), It.IsAny<byte[]>(), It.IsAny<NodeAddress>()), Times.Never());
        }

        [Test]
        public void ShouldNotReceiveOnParentWhenSentToNetworkFromParent()
        {
            var parentAddress = new NodeAddress("parent");
            var localAddress = new NodeAddress("local") { Parent = parentAddress };
            var data = "Hello world";

            var parent = new Mock<INode>();
            parent.Setup(x => x.Address).Returns(parentAddress);
            var network = new Mock<INetwork>();
            network.Setup(x => x.FindOrDefault(null)).Returns((INode)null);
            network.Setup(x => x.FindOrCreate(parentAddress)).Returns(parent.Object);
            network.Setup(x => x.FindOrDefault(parentAddress)).Returns(parent.Object);

            var local = new Node(localAddress, network.Object);
            local.SendToNetwork(network.Object, typeof(string).ToString(), NetworkChannel<string>.Serialize(data), parentAddress);

            parent.Verify(x => x.Send(typeof(string).ToString(), It.IsAny<byte[]>(), It.IsAny<NodeAddress>()), Times.Never());
        }

        [Test]
        public void ShouldNotReceiveOnNetworkWhenSentIfISentIt()
        {
            var network = InProcessNetwork.Create();
            var channel = network.GetChannel<string>();
            var parent = network.Local;

            string received = string.Empty;
            channel.Received += (sender, s) => received = s;
            parent.SendToNetwork(network, typeof(string).ToString(), NetworkChannel<string>.Serialize("Hello world"), parent.Address);
            received.ShouldBeEmpty();
        }
    }
}
