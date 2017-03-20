namespace nDistribute.WCF.Tests.ThatWCFNetwork
{
    using System.Diagnostics.CodeAnalysis;

    using NUnit.Framework;

    using Should;
    using nDistribute.WCF;
    using System.Diagnostics;
    using System;
    using nDistribute.WCF.TestExe.Model;
    using System.Threading;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Testing code.")]
    public class CommonSetup
    {
        protected WCFNetwork _testNetwork;
        protected NetworkChannel<OutgoingMessage> _testOutgoing;
        protected NetworkChannel<ReturnMessage> _testReturn;
        protected Process _test2Process;
        protected Process _test3Process;
        protected string _address;
        protected Process _test2_1Process;
        protected NodeAddress _test2Address;
        protected NodeAddress _test3Address;
        protected NodeAddress _test2_1Address;

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
        
            this._testNetwork = new WCFNetwork();
            this._testNetwork.Start();
            _address = _testNetwork.Local.Address.Address;
            _testNetwork.GetChannel<RegisteredMessage>().Received += RegisteredAddressRecieved;
            this._testOutgoing = _testNetwork.GetChannel<OutgoingMessage>();
            this._testReturn = _testNetwork.GetChannel<ReturnMessage>();

            Debug.WriteLine(Environment.CurrentDirectory);
            _test2Process = Process.Start("nDistribute.WCF.TestExe.exe", _address);
            //_test3Process = Process.Start("nDistribute.WCF.TestExe.exe", _address);

        }

        void RegisteredAddressRecieved(object sender, RegisteredMessage e)
        {
            switch (e.Name)
            {
                case "test2":
                    _test2Address = e.ToAddress();
                    break;

                case "test3":
                    _test3Address = e.ToAddress();
                    break;

                case "test2_1":
                    _test2_1Address = e.ToAddress();
                    if (_test2_1Process == null)
                        _test2_1Process = Process.Start(".\nDistribute.WCF.TestExe.exe", "test2_1 " + _address);
                    break;
            }
        }

        [TestFixtureTearDown]
        public void Teardown()
        {
            _test2Process.Kill();
            //_test3Process.Kill();
            //if (_test2_1Process != null)
            //    _test2_1Process.Kill();
        }

    }
}
