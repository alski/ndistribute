using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nDistribute.WCF.TestExe.Model
{
    [Serializable]
    public class ReturnMessage
    {
        public string Message { get; set; }
        public string NodeName { get; set; }
        public string NodeAddress { get; set; }
    }
}
