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
            var registeredNetwork = new WCFNetwork();
            registeredNetwork.Start();

            var manager = new NetworkManager();
            manager.Register(registeredNetwork);

            var externalNetwork = new WCFNetwork();
            externalNetwork.Start();

            var connection = manager.ConnectTo(externalNetwork.Local.Address);
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


