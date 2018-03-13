namespace nDistribute.Tests.ThatWebSocket
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.PeerToPeer;
    using System.Net.Sockets;
    using System.Linq;
    using System.Threading.Tasks;
    using Eventually;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Shouldly;

    [TestClass]
    public class PeerToPeer
    {
        [TestMethod]
        [Ignore("Doesn't work due to missing IpV6 on VirginMedia")]
        public async Task ShouldFindPeer()
        {
            var server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Any, 0));
            server.Listen(32);

            var name = new PeerName("ShouldFindPeer", PeerNameType.Unsecured);
            var reg = new PeerNameRegistration(name, ((IPEndPoint)server.LocalEndPoint).Port);
            reg.UseAutoEndPointSelection = true;
            reg.Start();

            var nameAsString = name.ToString();

            var fred = Cloud.GetAvailableClouds();

            var resolver = new PeerNameResolver();
            var records = resolver.Resolve(new PeerName(nameAsString));

            var result = records.First();
        }

    }
}
