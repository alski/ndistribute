namespace nDistribute.WCF.Tests.ThatWCFNetwork
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using nDistribute.WCF;
    using nDistribute.WCF.TestExe.Model;
    using NUnit.Framework;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Testing code.")]
    public class CommonSetup
    {
        protected WCFNetwork TestNetwork { get; set; }

        protected NetworkChannel<OutgoingMessage> TestOutgoing { get; set; }

        protected NetworkChannel<ReturnMessage> TestReturn { get; set; }

        protected Process Test2Process { get; set; }

        protected Process Test3Process { get; set; }

        protected string Address { get; set; }

        protected Process Test2_1Process { get; set; }

        protected NodeAddress Test2Address { get; set; }

        protected NodeAddress Test3Address { get; set; }

        protected NodeAddress Test2_1Address { get; set; }

        /// <summary>
        /// Trying to set up a network that look likes
        /// <code>
        /// _test1
        /// |-----------,
        /// _test2  _test2.1
        /// |
        /// _test3
        /// </code>
        /// </summary>
        //[TestFixtureSetUp]
        public void Setup()
        {        
            this.TestNetwork = new WCFNetwork();
            this.TestNetwork.Start();
            Address = TestNetwork.Local.Address.AsString;
            TestNetwork.GetChannel<RegisteredMessage>().Received += RegisteredAddressRecieved;
            this.TestOutgoing = TestNetwork.GetChannel<OutgoingMessage>();
            this.TestReturn = TestNetwork.GetChannel<ReturnMessage>();

            Debug.WriteLine(Environment.CurrentDirectory);
            Test2Process = Process.Start("nDistribute.WCF.TestExe.exe", Address);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Test2Process.Kill();
        }

        private void RegisteredAddressRecieved(object sender, RegisteredMessage e)
        {
            switch (e.Name)
            {
                case "test2":
                    Test2Address = e.ToAddress();
                    break;

                case "test3":
                    Test3Address = e.ToAddress();
                    break;

                case "test2_1":
                    Test2_1Address = e.ToAddress();
                    if (Test2_1Process == null)
                    {
                        Test2_1Process = Process.Start(".\nDistribute.WCF.TestExe.exe", "test2_1 " + Address);
                    }

                    break;
            }
        }
    }
}
