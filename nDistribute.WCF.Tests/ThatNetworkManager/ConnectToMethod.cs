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
            WCFNetwork.Register();
            var connection = manager.ConnectTo("net-tcp:xxxx");
            connection.ShouldNotBeNull();
        }

    }
}
