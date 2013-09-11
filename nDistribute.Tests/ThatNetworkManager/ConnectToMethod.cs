using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Should;

namespace nDistribute.Tests.ThatNetworkManager
{
    [TestFixture]
    public class ConnectToMethod
    {
        [Test]
        public void ShouldConnect()
        {
            var manager = new NetworkManager();
            InProcessNetwork.Register();
            var connection = manager.ConnectTo("inprocess:xxxx");
            connection.ShouldNotBeNull();
        }

    }
}
