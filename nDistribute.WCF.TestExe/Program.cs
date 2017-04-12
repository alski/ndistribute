namespace nDistribute.WCF.TestExe
{
    using System;
    using System.Diagnostics;
    using nDistribute.WCF.TestExe.Model;

    /// <summary>
    /// Program for testing
    /// </summary>
    public class Program
    {
        private static WCFNetwork network;
        private static NetworkChannel<ReturnMessage> returnChannel;

        /// <summary>
        /// Main method for simple test exe
        /// </summary>
        /// <param name="args">Startup arguments.</param>
        public static void Main(string[] args)
        {
            Debugger.Launch();

            network = new WCFNetwork();
            network.Start();
            network.GetChannel<OutgoingMessage>().Received += Program_Received;
            network.Connect(new NodeAddress(args[0]));
            network.GetChannel<RegisteredMessage>().Send(new RegisteredMessage { Address = network.Local.Address.AsString });

            returnChannel = network.GetChannel<ReturnMessage>();
            
            //You should never see this, but just in case
            Console.WriteLine("Press any key to terminate");
            Console.ReadKey();
        }

        private static void Program_Received(object sender, OutgoingMessage e)
        {
            Console.WriteLine("Received outgoing: " + e.Message);
            returnChannel.Send(
                new ReturnMessage
                {
                    Message = e.Message,
                    NodeAddress = network.Local.Address.AsString
                });
        }
    }
}
