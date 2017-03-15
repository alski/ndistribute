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
            var manager = new NetworkManager();
            WCFNetwork.Register(manager);
            var connection = manager.ConnectTo(WCFNetwork.SchemaName+":xxxx");
            connection.ShouldNotBeNull();
        }


        [Test]
        public void ShouldNotConnectIfUnregistered()
        {
            var manager = new NetworkManager();
            var connection = manager.ConnectTo(WCFNetwork.SchemaName + ":xxxx");
            connection.ShouldBeNull();
        }

    }
}


