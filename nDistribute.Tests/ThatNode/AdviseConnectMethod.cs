namespace nDistribute.Tests.ThatNode
{
    using System.Diagnostics.CodeAnalysis;

    using NUnit.Framework;

    using Should;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "TestFixture.")]
    [TestFixture]
    public class AdviseConnectMethod
    {
        [Test]
        public void ShouldUpdateSecondNetworkWithConnection()
        {
            var network1 = InProcessNetwork.Create();
            var node1 = network1.Local;

            var network2 = InProcessNetwork.Create("network2");
            var node2 = network2.Local;

            node2.AdviseConnect(node1.Address);

            node1.HasChild(node2.Address).ShouldBeTrue();
            node2.HasParent.ShouldBeTrue();
            node2.Address.Parent.Matches(node1.Address).ShouldBeTrue();

            network1.FindOrDefault(node2.Address).HasParent.ShouldBeTrue();
            network2.FindOrDefault(node1.Address).ShouldNotBeNull();
        }
    }
}
