using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nDistribute
{
    public class NetworkRegistration
    {
            public Func<string, bool> CanCreate { get; set; }
            public Func<string, NetworkBase> CreateNetwork { get; set; }

    }
}
