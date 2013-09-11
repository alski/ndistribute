namespace nDistribute.Tests.ThatNetwork
{
    using System.Diagnostics.CodeAnalysis;

    using NUnit.Framework;

    using Should;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "TestFixture.")]
    [TestFixture]
    public class SendMethod
    {
        [Test]
        public void ShouldReceiveWhenSent()
        {
            var network = InProcessNetwork.Create();
            var parent = network.Local;
            var childNetwork = InProcessNetwork.Create("child");
            var child = childNetwork.Local.Address;
            network.Connect(child);
            var channel = network.GetChannel<string>();
            //Call this to prevent test failure with Can't find constructor on type string
            var childChannel = childNetwork.GetChannel<string>();
            var childNode = network.FindOrDefault(child);

            string received = string.Empty;
            childChannel.Received += (sender, s) => received = s;
            channel.Send("Hello world");
            received.ShouldEqual("Hello world");
        }

        [TestAttribute]
        public void ShouldReceiveOnParentNetworkWhenSent()
        {
            var parentNetwork = InProcessNetwork.Create("Parent");
            var childNetwork = parentNetwork.CreateNetwork("Child");
            var parentChannel = parentNetwork.GetChannel<string>(); 
            parentNetwork.Connect(childNetwork.Local.Address);
            var childChannel = childNetwork.GetChannel<string>();

            string received = string.Empty;
            parentChannel.Received += (sender, s) => received = s;
            childChannel.Send("Hello world");
            received.ShouldEqual("Hello world");
        }
        

        [TestAttribute]
        public void ShouldReceiveOnChildNetworkWhenSent()
        {
            var parentNetwork = InProcessNetwork.Create("Parent");
            var parentNode = parentNetwork.Local;
            var parentChannel = parentNetwork.GetChannel<string>();
            var childNetwork = parentNetwork.CreateNetwork("Child");
            var childNode = childNetwork.Local;
            var childChannel = childNetwork.GetChannel<string>();
            parentNetwork.Connect(childNode.Address);

            string received = string.Empty;
            childChannel.Received += (sender, s) => received = s;
            parentChannel.Send("Hello world");
            received.ShouldEqual("Hello world");
        }

        [TestAttribute]
        public void ShouldReceiveOnAllChildNetworksWhenSent()
        {
            var parentNetwork = InProcessNetwork.Create("Parent");
            var parentNode = parentNetwork.Local;
            var parentChannel = parentNetwork.GetChannel<string>();
            var childANetwork = parentNetwork.CreateNetwork("ChildA");
            var childANode = childANetwork.Local;
            var childAChannel = childANetwork.GetChannel<string>();
            var childBNetwork = parentNetwork.CreateNetwork("ChildB");
            var childBNode = childBNetwork.Local;
            var childBChannel = childBNetwork.GetChannel<string>();

            parentNetwork.Connect(childANode.Address);
            parentNetwork.Connect(childBNode.Address);

            string receivedA = string.Empty;
            string receivedB = string.Empty;
            childAChannel.Received += (sender, s) => receivedA = s;
            childBChannel.Received += (sender, s) => receivedB = s;
            parentChannel.Send("Hello world");
            receivedA.ShouldEqual("Hello world");
            receivedB.ShouldEqual("Hello world");
        }

        //[Test]
        //public void ShouldReceiveOnAParentsChildWhenSent()
        //{
        //    var networkSource = InProcessNetwork.Create("source");
        //    var networkParent = networkSource.CreateNetwork("Parent");
        //    var networkParentsChild = networkSource.CreateNetwork("ParentsChild");

        //    networkParent.Connect(networkSource.Local.Address);
        //    networkSource.Local.Address.Parent.Matches(networkParent.Local.Address).ShouldBeTrue();

        //    networkParent.Connect(networkParentsChild.Local.Address);
        //    networkParentsChild.Local.Address.Parent.Matches(networkParent.Local.Address).ShouldBeTrue();

        //    string receivedParent = string.Empty;
        //    string receivedParentsChild = string.Empty;
        //    networkParent.Received += (sender, s) => receivedParent = s;
        //    networkParentsChild.Received += (sender, s) => receivedParentsChild = s;
        //    networkSource.Send("Hello world");

        //    receivedParent.ShouldEqual("Hello world");
        //    receivedParentsChild.ShouldEqual("Hello world");
        //}
    }
}
