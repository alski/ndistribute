namespace nDistribute.WCF.Tests.ThatWCFNetwork
{
    using System.Diagnostics.CodeAnalysis;

    using NUnit.Framework;

    using Should;
    using nDistribute.WCF;
    using System.Threading;
    using System.Threading.Tasks;
    using System;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Testing code.")]
    [TestFixture]
    public class ConnectMethod : CommonSetup
    {

        //[Test]
        //public void ShouldConnect()
        //{
        //    Setup();
         
        //    var canceller = new CancellationTokenSource();
        //    var connect = Task.Run(() => WaitForConnect(_testNetwork, canceller.Token));
        //    canceller.CancelAfter(TimeSpan.FromSeconds(10));
        //    connect.Wait();

        //    _testNetwork.IsConnected.ShouldBeTrue();

        //    _test2Address.ShouldNotBeNull();
        //    _test2Address.Parent.Matches(this._testNetwork.Address).ShouldBeTrue();
        //    this._testNetwork.HasChild(_test2Address).ShouldBeTrue();

            //_test2_1Network.Address.ShouldNotBeNull();
            //_test2_1Network.Address.Parent.Matches(this._test1Network.Address).ShouldBeTrue();
            //this._testNetwork.HasChild(_test2_1Address).ShouldBeTrue();

            //_test3Network.Address.ShouldNotBeNull();
            //_test3Network.Address.Parent.Matches(this._test2Network.Address).ShouldBeTrue();
            //this._testNetwork.HasChild(_test3Address).ShouldBeTrue();
        //}

        private void WaitForConnect(WCFNetwork _testNetwork, CancellationToken token)
        {
            while (_testNetwork.IsConnected == false 
                && token.IsCancellationRequested == false)
            {
                Thread.Sleep(100);
            }
        }

    }
}
