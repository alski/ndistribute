namespace nDistribute.UWP.Tests.ThatWebSocket
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Windows.Storage.Streams;

    using Shouldly;
    using Eventually;
    using Windows.Web.Http;
    using System.Net;
    using Windows.Networking;

    public class Locator
    {
        public static async Task<HostName> GetCurrentIPAddress()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(new Uri("https://api.ipify.org"));
                var address = await response.Content.ReadAsStringAsync();
                return new HostName(address);
            }
        }
    }


    [TestClass]
    public class ThatLocator
    {
        [TestMethod]
        public async Task ShouldFindIPAddress()
        {
            var ipAddress = await Locator.GetCurrentIPAddress();
            ipAddress.ShouldNotBeNull();
            var ip = ipAddress.IPInformation.ToString();
            //Not loopback
            ip.ShouldNotStartWith("127");
            //Not local
            ip.ShouldNotStartWith("172");
            ip.ShouldNotStartWith("192");
            ip.ShouldNotStartWith("10");
        }
    }

    [TestClass]
    public class ThatSteamSocket
    {
        [TestMethod]
        public async Task ShouldSendMessages()
        {
            var server = new StreamSocketTransport.Server();
            string messageReceived = string.Empty;

            var local = new Local(server);
            local.MessageReceived += (s, e) => messageReceived = e;
            await local.StartAsync();

            var remote = new Remote(new StreamSocketTransport.Client(server.RemoteName, server.Port));
            await remote.SendAsync();

            EventuallyHelper.Eventually(() => messageReceived.ShouldNotBeNullOrEmpty());
        }



    }
}
