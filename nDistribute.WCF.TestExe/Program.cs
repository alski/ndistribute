using nDistribute.WCF.TestExe.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nDistribute.WCF.TestExe
{
    class Program
    {
        static WCFNetwork _network;
        static NetworkChannel<ReturnMessage> _returnChannel;

        static void Main(string[] args)
        {
            Debugger.Launch();

            _network = new WCFNetwork();
            _network.Start();
            _network.GetChannel<OutgoingMessage>().Received += Program_Received;
            _network.Connect(new NodeAddress(args[0]));
            _network.GetChannel<RegisteredMessage>().Send(new RegisteredMessage { Address = _network.Local.Address.Address });

            _returnChannel = _network.GetChannel<ReturnMessage>();
            //You should never see this, but just in case
            Console.WriteLine("Press any key to terminate");
            Console.ReadKey();
        }

        static void Program_Received(object sender, OutgoingMessage e)
        {
            Console.WriteLine("Received outgoing: " + e.Message);
            _returnChannel.Send(
                new ReturnMessage
                {
                    Message = e.Message,
                    NodeAddress = _network.Address.Address
                });
        }
    }
}
