using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nDistribute
{
    public class NetworkRegistration
    {
            public Func<NodeAddress, bool> CanCreate { get; set; }
            public Func<NetworkBase> CreateNetwork { get; set; }

    }
}
