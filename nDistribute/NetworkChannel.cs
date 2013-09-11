using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nDistribute
{
    //Need this to allow collection in Network
    public interface IChannel
    {
        void OnReceived(byte[] bytes);
    }

    public class NetworkChannel<T> : IChannel
    {
        public NetworkChannel(NetworkBase network)
        {
            Network = network;
        }

        public event EventHandler<T> Received;

        public NetworkBase Network { get; private set; }

        public void Send(T data)
        {
            var bytes = Serialize(data);
            Network.Local.SendToNetwork(Network, typeof(T).AssemblyQualifiedName, bytes, Network.Local.Address);
        }

        internal static byte[] Serialize(T data)
        {
            using (var ms = new MemoryStream())
            {
                NetworkBase.Formatter.Serialize(ms, data);
                return ms.GetBuffer();
            }
        }

        private static T Deserialize(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                return (T)NetworkBase.Formatter.Deserialize(ms);
            }
        }

        public void OnReceived(byte[] bytes)
        {
            var temp = Received;
            if (temp != null)
            {
                temp(this, Deserialize(bytes));
            }
        }
    }
}