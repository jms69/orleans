using KubeCalc.Interface;
using Orleans;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace KubeCalc.Grains
{
    public class KubeCalculatorGrain : Grain<long?>, IKubeCalculator
    {
        public override async Task OnActivateAsync()
        {                   
            if (!this.State.HasValue)
            {
                var input = this.GetPrimaryKeyLong();
                this.State = input * input * input;                
            }

            await base.OnActivateAsync();
        }
        public Task<long> CalculateAsync()
        {
                        
            return Task.FromResult(this.State.Value);
        }

        public Task<string> MachineName()
        {
            return Task.FromResult(System.Environment.MachineName);
        }
    }
}
