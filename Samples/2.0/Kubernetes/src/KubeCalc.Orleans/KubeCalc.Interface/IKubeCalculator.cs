using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KubeCalc.Interface
{
    public interface IKubeCalculator : IGrainWithIntegerKey
    {
        Task<long> CalculateAsync();
        Task<string> MachineName();
    }
}
