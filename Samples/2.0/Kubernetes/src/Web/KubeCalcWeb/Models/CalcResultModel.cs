using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KubeCalcWeb.Models
{
    public class CalcResultModel
    {
        public int BlockSize { get; set; }

        public int Columns { get; set; }

        public int Input { get; set; }

        public Dictionary<int,string> CalcResult { get; set; }
    }
}
