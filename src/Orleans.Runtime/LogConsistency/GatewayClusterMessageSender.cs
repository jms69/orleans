using Microsoft.Extensions.Logging;
using Orleans.LogConsistency;
using Orleans.MultiCluster;
using Orleans.Runtime.MultiClusterNetwork;
using Orleans.SystemTargetInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.Runtime.LogConsistency
{
    internal class GatewayClusterMessageSender : IClusterMessageSender
    {
        private readonly IMultiClusterOracle multiClusterOracle;    
        private readonly ILogger log;
        private readonly IInternalGrainFactory grainFactory; 

        public GatewayClusterMessageSender(
            IInternalGrainFactory grainFactory,
            ILoggerFactory loggerFactory,
            IMultiClusterOracle multiClusterOracle)
        {
            this.grainFactory = grainFactory;
            this.log = loggerFactory.CreateLogger<GatewayClusterMessageSender>();
            this.multiClusterOracle = multiClusterOracle;
        }

        public async Task<ILogConsistencyProtocolMessage> SendToRemoteCluster(string clusterId, string grainId, ILogConsistencyProtocolMessage payload)
        {
            var clusterGateway = this.multiClusterOracle.GetRandomClusterGateway(clusterId);

            if (clusterGateway == null)
                throw new ProtocolTransportException("no active gateways found for cluster");

            var repAgent = this.grainFactory.GetSystemTarget<ILogConsistencyProtocolGateway>(Constants.ProtocolGatewayId, clusterGateway);

            var grainRefId = GrainId.FromParsableString(grainId);

            return await repAgent.RelayMessage(grainRefId, payload);
        }
    }
}
