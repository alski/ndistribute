using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Should;

namespace nDistribute.WCF.Tests.ThatNetworkManager
{
    [TestFixture]
    public class ConnectToMethod
    {
        [Test]
        public void ShouldConnect()
        {
            var xxxx = new WCFNetwork();
            xxxx.Start();

            var manager = new NetworkManager();
            WCFNetwork.Register(manager);
            var connection = manager.ConnectTo(xxxx.Local.Address);
            connection.ShouldNotBeNull();
        }


        [Test]
        public void ShouldNotConnectIfUnregistered()
        {
            var manager = new NetworkManager();
            var connection = manager.ConnectTo(new NodeAddress(WCFNetwork.SchemaName + ":xxxx"));
            connection.ShouldBeNull();
        }

    }
}


