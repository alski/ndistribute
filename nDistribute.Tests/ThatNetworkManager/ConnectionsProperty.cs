using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Should;

namespace nDistribute.Tests.ThatNetworkManager
{
    [TestFixture]
    public class ConnectionsProperty
    {
        [Test]
        public void ShouldIncludeAConnectedNodeName()
        {
            var manager = new NetworkManager();
            InProcessNetwork.Register(manager);

            var network1 = InProcessNetwork.Create("1");
            var node1 = network1.Local;

            var network2 = InProcessNetwork.Create("2");
            network2.Connect(network1.Local.Address);

            network1.Connections.ShouldContain(network1.Local.Address.Address);
            network1.Connections.ShouldContain(network2.Local.Address.Address);

            network2.Connections.ShouldContain(network1.Local.Address.Address);
            network2.Connections.ShouldContain(network2.Local.Address.Address);
        }
    }
}
