using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nDistribute
{
    public interface INetwork
    {
        INode FindOrCreate(NodeAddress nodeAddress);
        INode FindOrDefault(NodeAddress nodeAddress);

        void OnReceived(string type, byte[] data);
    }
}
