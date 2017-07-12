using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Collections.ObjectModel;
using Should;
using nDistribute.WCF;
using nDistribute.Tests;

namespace nDistribute.Data.Tests
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void ShouldBuildCollectionFromStreamOfEvents()
        {
            var result = new List<int>();
            
            var receiverNetwork = AsyncInProcessNetwork.Create();
            var receiverChannel = receiverNetwork.GetChannel<int>();

            var senderNetwork = AsyncInProcessNetwork.Create("receiver");
            var senderChannel = senderNetwork.GetChannel<int>();
            senderNetwork.Connect(receiverNetwork.Local.Address);
            
            receiverChannel.Received += (sender, e) => result.Add(e);

            senderChannel.Send(1);
            senderChannel.Send(2);
            senderChannel.Send(3);

            senderNetwork.WaitForTasks();

            result.First().ShouldEqual(1);
            result.Skip(1).First().ShouldEqual(2);
            result.Skip(2).First().ShouldEqual(3);
            result.Count.ShouldEqual(3);
        }
    }
}
