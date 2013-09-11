using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nDistribute.WCF.TestExe.Model
{
    [Serializable]
    public class RegisteredMessage
    {
        public string Name { get; set; }
        public string Address { get; set; }

        public NodeAddress ToAddress()
        {
            return new NodeAddress(Address);
        }
    }
}
