namespace nDistribute.UWP.Tests.ThatWebSocket
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Windows.Storage.Streams;

    using Shouldly;
    using Eventually;

    [TestClass]
    public class WebSocket
    {
        [TestMethod]
        public async Task ShouldSendMessages()
        {
            var server = new StreamSocketTransport.Server();
            string messageReceived = string.Empty;

            var local = new Local(server);
            local.MessageReceived += (s,e) => messageReceived = e;
            await local.StartAsync();

            var remote = new Remote(new StreamSocketTransport.Client(server.RemoteName, server.Port));
            await remote.SendAsync();

            EventuallyHelper.Eventually(()=>messageReceived.ShouldNotBeNullOrEmpty());
        }

    }
}
