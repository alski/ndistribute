namespace nDistribute.WCF.Tests.ThatWCFNetwork
{
    using System.Diagnostics.CodeAnalysis;

    using NUnit.Framework;

    using Should;
    using nDistribute.WCF;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Testing code.")]
    [TestFixture]
    public class SendMethod : CommonSetup
    {

        //[Test]
        //public void ShouldSendToParent()
        //{
        //    string received = null;
        //    this._test1.Received += (sender, s) => received = s;
        //    this._test2.Send("ShouldSendToParent");
        //    received.ShouldEqual("ShouldSendToParent");
        //}

        //[Test]
        //public void ShouldSendToParentsParent()
        //{
        //    string received = null;
        //    this._test1.Received += (sender, s) => received = s;
        //    this._test3.Send("ShouldSendToParentsParent");
        //    received.ShouldEqual("ShouldSendToParentsParent");
        //}

        //[Test]
        //public void ShouldSendToChild()
        //{
        //    string received = null;
        //    this._test2.Received += (sender, s) => received = s;
        //    this._test1.Send("ShouldSendToChild");
        //    received.ShouldEqual("ShouldSendToChild");
        //}

        //[Test]
        //public void ShouldSendToChildsChild()
        //{
        //    string received = null;
        //    this._test3.Received += (sender, s) => received = s;
        //    this._test1.Send("ShouldSendToChildsChild");
        //    received.ShouldEqual("ShouldSendToChildsChild");
        //}

        //[Test]
        //public void ShouldSendToParentsChild()
        //{
        //    string received = null;
        //    this._test2_1.Received += (sender, s) => received = s;
        //    this._test2.Send("ShouldSendToParentsChild");
        //    received.ShouldEqual("ShouldSendToParentsChild");
        //}

    }
}
