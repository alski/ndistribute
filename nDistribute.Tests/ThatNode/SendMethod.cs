namespace nDistribute.Tests.ThatNode
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using NUnit.Framework;

    using Should;
    using Moq;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "TestFixture.")]
    [TestFixture]
    public class SendMethod
    {
        [Test]
        public void ShouldReceiveOnNetworkWhenSent()
        {
            var network  = InProcessNetwork.Create();
            var parent = network.Local;
            var channel = network.GetChannel<string>();
            
            string received = string.Empty;
            channel.Received += (sender, s) => received = s;
            parent.Send(typeof(string).AssemblyQualifiedName, NetworkChannel<string>.Serialize("Hello world"), null);
            received.ShouldEqual("Hello world");
        }

        [Test]
        public void ShouldReceiveOnChildWhenSent()
        {
            var childAddress = new NodeAddress("child");
            var localAddress = new NodeAddress("local");            
            var data = NetworkChannel<string>.Serialize("Hello world");

            var child = new Mock<INode>();
            child.Setup(x => x.Address).Returns(childAddress);
            var network = new Mock<INetwork>();
            network.Setup(x => x.FindOrDefault(null)).Returns((INode)null);
            network.Setup(x => x.FindOrCreate(childAddress)).Returns(child.Object);
            network.Setup(x => x.FindOrDefault(childAddress)).Returns(child.Object);

            var local = new Node(localAddress, network.Object);
            local.Connect(childAddress);
            local.Send(typeof(string).ToString(), data, null);

            child.Verify(x => x.Send(typeof(string).ToString(), data, It.IsAny<NodeAddress>()));
        }

        [Test]
        public void ShouldReceiveOnParentWhenSent()
        {
            var parentAddress = new NodeAddress("parent");
            var localAddress = new NodeAddress("local") { Parent = parentAddress };
            var data = NetworkChannel<string>.Serialize("Hello world");

            var parent = new Mock<INode>();
            parent.Setup(x => x.Address).Returns(parentAddress);
            var network = new Mock<INetwork>();
            network.Setup(x => x.FindOrDefault(null)).Returns((INode)null);
            network.Setup(x => x.FindOrCreate(parentAddress)).Returns(parent.Object);
            network.Setup(x => x.FindOrDefault(parentAddress)).Returns(parent.Object);

            var local = new Node(localAddress, network.Object);
            local.Send(typeof(string).ToString(), data, null);

            parent.Verify(x => x.Send(typeof(string).ToString(), data, It.IsAny<NodeAddress>()), Times.Once());
        }

        [Test]
        public void ShouldSendComplexType()
        {
            var data = new ComplexTypeForTest { IntProperty = 5, StringField = "Hello World" };
            var network = InProcessNetwork.Create();
            var channel = network.GetChannel<ComplexTypeForTest>();
            var parent = network.Local;

            ComplexTypeForTest received = null;
            channel.Received += (sender, s) => received = s;
            parent.Send(typeof(ComplexTypeForTest).AssemblyQualifiedName, NetworkChannel<ComplexTypeForTest>.Serialize(data), null);
            received.IntProperty.ShouldEqual(data.IntProperty);
            received.StringField.ShouldEqual(data.StringField);
        }

        [Serializable]
        public class ComplexTypeForTest
        {
            public int IntProperty { get; set; }
            public string StringField;
        }
    }
}
