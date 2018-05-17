using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Core;
using Orleans.MultiCluster;
using Orleans.Runtime;
using Orleans.SystemTargetInterfaces;
using Orleans.Concurrency;
using Orleans.Runtime.MultiClusterNetwork;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace Orleans.Runtime.LogConsistency
{
    [Reentrant]
    internal class ProtocolGateway : SystemTarget, ILogConsistencyProtocolGateway, IClusterMessageReceiverGateway
    {
        private readonly IClusterMessageReceiver clusterMessageReceiver;
        private readonly ILogger logger;
        private TaskScheduler protocolTaskScheduler;

        public ProtocolGateway(SiloAddress silo, ILoggerFactory loggerFactory, IClusterMessageReceiver clusterMessageReceiver = null)
            : base(Constants.ProtocolGatewayId, silo, loggerFactory)
        {
            this.clusterMessageReceiver = clusterMessageReceiver;
            this.logger = loggerFactory.CreateLogger<ProtocolGateway>();
            this.protocolTaskScheduler = TaskScheduler.Current;
        }

        public ILogConsistencyProtocolMessage ReceiveFromRemoteCluster(string grainIdStr, ILogConsistencyProtocolMessage payload)
        {
            var runtimeClient = this.RuntimeClient.ServiceProvider.GetRequiredService<InsideRuntimeClient>();
            var id = GrainId.FromParsableString(grainIdStr);
            var g = runtimeClient.InternalGrainFactory.GetGrain<ILogConsistencyProtocolParticipant>(id);
            return g.OnProtocolMessageReceived(payload).GetAwaiter().GetResult();

            //ILogConsistencyProtocolMessage result = null;
            //Task.Factory.StartNew(() =>
            //{
            //    var g = this.RuntimeClient.InternalGrainFactory.GetGrain<ILogConsistencyProtocolParticipant>(id);
            //    result = g.OnProtocolMessageReceived(payload).GetAwaiter().GetResult();
            //}, CancellationToken.None, TaskCreationOptions.None, this.protocolTaskScheduler).GetAwaiter().GetResult();        
            //return result;
        }

        public async Task<ILogConsistencyProtocolMessage> RelayMessage(GrainId id, ILogConsistencyProtocolMessage payload)
        {
            var g = RuntimeClient.InternalGrainFactory.GetGrain<ILogConsistencyProtocolParticipant>(id);
            return await g.OnProtocolMessageReceived(payload);
        }

        public async Task StartReceivers(string myClusterId)
        {
            if (clusterMessageReceiver != null)
            {
                clusterMessageReceiver.Subscribe(this);
                await clusterMessageReceiver.StartReceivingAsync(myClusterId);
            }
        }

        public async Task StopReceivers()
        {
            if (clusterMessageReceiver != null)
            {
                await clusterMessageReceiver.StopReceivingAsync();                
            }
        }
    }
}
