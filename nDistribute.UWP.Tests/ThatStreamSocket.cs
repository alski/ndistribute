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
    using Windows.Networking.Connectivity;

    public class Locator
    {
        public static async Task<HostName> GetRemoteHostName()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(new Uri("https://api.ipify.org"));
                var address = await response.Content.ReadAsStringAsync();
                return new HostName(address);
            }
        }

        public static HostName GetNetworkHostName()
        {
            var connection = NetworkInformation.GetInternetConnectionProfile();

            if (connection?.NetworkAdapter != null)
            {
                return  NetworkInformation.GetHostNames()
                .FirstOrDefault(hostname =>
                    hostname.Type == HostNameType.Ipv4
                    && hostname.IPInformation?.NetworkAdapter?.NetworkAdapterId == connection.NetworkAdapter.NetworkAdapterId);
            }
            return null;
        }
    }


    [TestClass]
    public class ThatLocator
    {
        [TestMethod]
        public async Task ShouldFindIPAddress()
        {
            var ipAddress = await Locator.GetRemoteHostName();
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




            var remote = new Remote(new StreamSocketTransport.Client(Locator.GetNetworkHostName(), server.Port));
            await remote.SendAsync();

            EventuallyHelper.Eventually(() => messageReceived.ShouldNotBeNullOrEmpty());
        }



    }
}
