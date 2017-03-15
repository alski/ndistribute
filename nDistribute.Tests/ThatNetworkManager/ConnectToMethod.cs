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
            InProcessNetwork.Register(manager);
            var connection = manager.ConnectTo(InProcessNetwork.SchemaName +":xxxx");
            connection.ShouldNotBeNull();
        }


        [Test]
        public void ShouldNotConnectIfUnregistered()
        {
            var manager = new NetworkManager();
            var connection = manager.ConnectTo(InProcessNetwork.SchemaName + ":xxxx");
            connection.ShouldBeNull();
        }

    }
}
