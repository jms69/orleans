using Orleans.MultiCluster;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.Runtime.MultiClusterNetwork
{
    public interface IClusterMessageSender
    {
        Task<ILogConsistencyProtocolMessage> SendToRemoteCluster(string clusterId, string grainId, ILogConsistencyProtocolMessage payload);
    }
}
