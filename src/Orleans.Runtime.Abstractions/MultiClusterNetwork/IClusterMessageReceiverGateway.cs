using Orleans.MultiCluster;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orleans.Runtime.MultiClusterNetwork
{
    public interface IClusterMessageReceiverGateway
    {
        ILogConsistencyProtocolMessage ReceiveFromRemoteCluster(string grainIdStr, ILogConsistencyProtocolMessage payload);
    }
}
