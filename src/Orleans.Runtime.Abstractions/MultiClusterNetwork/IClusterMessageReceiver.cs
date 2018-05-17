using Orleans.MultiCluster;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.Runtime.MultiClusterNetwork
{
    public interface IClusterMessageReceiver
    {        
        Task StartReceivingAsync(string clusterId);
        Task StopReceivingAsync();

        void Subscribe(IClusterMessageReceiverGateway gateway);        
    }
}
