namespace nDistribute.Tests.ThatNode
{
    using System.Diagnostics.CodeAnalysis;

    using NUnit.Framework;

    using Should;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "TestFixture.")]
    [TestFixture]
    public class Constructor
    {
        [Test]
        public void ShouldHaveNoParent()
        {
            var network = InProcessNetwork.Create();
            network.Local.HasParent.ShouldBeFalse();
        }
    }
}
