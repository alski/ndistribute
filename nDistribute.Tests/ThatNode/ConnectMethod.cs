namespace nDistribute.Tests.ThatNode
{
    using System.Diagnostics.CodeAnalysis;

    using NUnit.Framework;

    using Should;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "TestFixture.")]
    [TestFixture]
    public class ConnectMethod
    {
        [Test]
        public void ShouldBecomeChildWhenAskingToJoin()
        {
            var parentNetwork = InProcessNetwork.Create();
            var parentNode = parentNetwork.Local;
            var childNetwork = InProcessNetwork.Create("child");
            var child = childNetwork.Local.Address;

            parentNode.Connect(child);

            parentNode.HasChild(child).ShouldBeTrue();
            child.Parent.Matches(parentNode.Address).ShouldBeTrue();

            var childNode = parentNetwork.FindOrDefault(child);
            childNode.HasParent.ShouldBeTrue();
            parentNode.IsConnected.ShouldBeTrue();
            childNode.IsConnected.ShouldBeTrue();
        }

        [Test]
        public void ShouldBeAbleToHaveMultipleChildren()
        {
            var parentNetwork = InProcessNetwork.Create();
            var parentNode = parentNetwork.Local;
            var child1Network = InProcessNetwork.Create("child1");
            var child1 = child1Network.Local.Address;
            var child2Network = InProcessNetwork.Create("child2");
            var child2 = child2Network.Local.Address;

            parentNode.Connect(child1);
            parentNode.Connect(child2);

            parentNode.HasChild(child1).ShouldBeTrue();
            parentNode.HasChild(child2).ShouldBeTrue();
        }

        [Test]
        public void ShouldBeAbleToChain()
        {
            var grandParentNetwork = InProcessNetwork.Create();
            var grandParent = grandParentNetwork.Local;
            var parentNetwork = InProcessNetwork.Create("Parent");
            var parent = parentNetwork.Local.Address;
            var childNetwork = InProcessNetwork.Create("Child");
            var child = childNetwork.Local.Address;

            grandParent.Connect(parent);
            
            var parentNode = grandParentNetwork.FindOrDefault(parent);
            parentNode.Connect(child);

            grandParent.HasChild(parent).ShouldBeTrue();
            parentNetwork.Local.HasChild(child).ShouldBeTrue();
            //but local representation doesn't know about grandparent relationship
            parentNode.HasChild(child).ShouldBeFalse();
        }

        [Test]
        public void ShouldBeAbleToDisconnect()
        {
            var parentNetwork = InProcessNetwork.Create();
            var parent = parentNetwork.Local;
            var childNetwork = InProcessNetwork.Create("child");
            var child = childNetwork.Local.Address;

            parent.Connect(child);
            parent.HasChild(child).ShouldBeTrue();

            var child1Node = parentNetwork.FindOrDefault(child);
            child1Node.Disconnect();

            parent.HasChild(child).ShouldBeFalse();
        }

        [Test]
        public void ShouldBeAbleToDropNodeFromChainWithoutLosingGrandChildren()
        {
            var grandParentNetwork = InProcessNetwork.Create();
            var grandparentNode = grandParentNetwork.Local;

            var parentNetwork = InProcessNetwork.Create("parent");
            var parent = parentNetwork.Local.Address;
            var childNetwork = InProcessNetwork.Create("child");
            var child = childNetwork.Local.Address;

            grandparentNode.Connect(parent);
            var parentNode = //grandParentNetwork.FindOrCreate(parent);
                parentNetwork.Local;
            parentNode.Connect(child);

            parentNode.Disconnect();
            grandparentNode.HasChild(parent).ShouldBeFalse();
            grandparentNode.HasChild(child).ShouldBeTrue();
        }

        [Test]
        public void ShouldNotHaveSameNodeAppearingTwice()
        {
            var grandparentNetwork = InProcessNetwork.Create();
            var grandparentNode = grandparentNetwork.Local;

            var parentNetwork = InProcessNetwork.Create("parent");
            var parent = parentNetwork.Local.Address;
            var childNetwork = InProcessNetwork.Create("child");
            var child = childNetwork.Local.Address;
           
            grandparentNode.Connect(parent);
            var parentNode = parentNetwork.Local;
            parentNode.Connect(child);

            grandparentNode.Connect(child);
            grandparentNode.HasChild(child).ShouldBeTrue();
            parentNode.HasChild(child).ShouldBeFalse();
        }

        [Test]
        public void ShouldNotHaveDuplicateChildren()
        {
            var parentNetwork = InProcessNetwork.Create("parent");
            var parent = parentNetwork.Local.Address;
            var childNetwork = InProcessNetwork.Create("child");
            var child = childNetwork.Local.Address;

            var parentNode = parentNetwork.Local;
            parentNode.Connect(child);

            parentNode.IsConnected.ShouldBeTrue();
            parentNode.HasChild(child).ShouldBeTrue();
            ((NodeWithoutContract)parentNode).Children.Count.ShouldEqual(1);

            var childNode = childNetwork.Local;
            childNode.IsConnected.ShouldBeTrue();            
            ((NodeWithoutContract)childNode).Children.Count.ShouldEqual(0);
        }
     

    }
}
