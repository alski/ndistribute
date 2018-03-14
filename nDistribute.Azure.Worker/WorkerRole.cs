using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;

namespace nDistribute.Azure.Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("nDistribute.Azure.Worker is running");

            try
            {
                Task.WaitAll(
                    //RunTCPAsync(cancellationTokenSource.Token),
                    RunUDPAsync(cancellationTokenSource.Token));
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("nDistribute.Azure.Worker has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("nDistribute.Azure.Worker is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("nDistribute.Azure.Worker has stopped");
        }

        private async Task RunUDPAsync(CancellationToken cancellationToken)
        {
            UdpClient listener = null;
            try
            {
                listener = new UdpClient(
                    RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["UDPEndpoint"].IPEndpoint);
                //listener.ExclusiveAddressUse = false;                
            }
            catch (SocketException)
            {
                Trace.Write("UDPEndpoint could not start.", "Error");
                return;
            }

            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("UDP Working");
                var client= await listener.ReceiveAsync();
                var stream = new StreamReader(new MemoryStream( client.Buffer));
                Trace.TraceInformation($"udp: {client.RemoteEndPoint}");

                var result = $"{client.RemoteEndPoint}";
                var bytes = Encoding.Unicode.GetBytes(result);
                listener.Send(bytes, bytes.Length, client.RemoteEndPoint);
            }
        }

        private async Task RunTCPAsync(CancellationToken cancellationToken)
        {
            TcpListener listener = null;
            try
            {
                listener = new TcpListener(
                    RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["TCPEndpoint"].IPEndpoint);
                listener.ExclusiveAddressUse = false;
                listener.Start();
            }
            catch (SocketException)
            {
                Trace.Write("TCPEndpoint could not start.", "Error");
                return;
            }

            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("TCP Working");
                var client = await listener.AcceptTcpClientAsync();
                var stream = client.GetStream();
                Trace.TraceInformation($"TCP client: {client.Client.LocalEndPoint}-{client.Client.RemoteEndPoint}");

                var writer = new StreamWriter(stream) { AutoFlush = true };
                writer.Write($"{client.Client.LocalEndPoint}-{client.Client.RemoteEndPoint}");
                client.Close();
            }
        }

    }
}
