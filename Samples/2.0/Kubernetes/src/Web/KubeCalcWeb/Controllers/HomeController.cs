using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KubeCalcWeb.Models;
using Orleans;
using KubeCalc.Interface;

namespace KubeCalcWeb.Controllers
{
    public class HomeController : Controller
    {
        protected const int BLOCKSIZE = 25;
        protected const int COLUMNS = 4;

        private IClusterClient _clusterClient;
        public HomeController(IClusterClient clusterClient) => _clusterClient = clusterClient;

        public async Task<IActionResult> Index([FromQuery]int? input)
        {
            var startValue = input ?? 100;

            var calcOutput = await CalculateBlock(startValue);
            return View(calcOutput);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        protected Task<CalcResultModel> CalculateBlock(int input)
        {
            var results = Enumerable.Range(input, BLOCKSIZE * COLUMNS)
                      .Select(async value =>
                      {
                          var calcGrain = _clusterClient.GetGrain<IKubeCalculator>(value);
                          var cubeResult = await calcGrain.CalculateAsync();
                          var instance = await calcGrain.MachineName();
                          return new { Value = value, CubeResult = cubeResult, Instance = instance };
                      })
                      .ToDictionary(k => k.Result.Value, v => $"{v.Result.Value}^3 = {v.Result.CubeResult} ({v.Result.Instance})");

            var calcResult =  new CalcResultModel {
                                BlockSize = BLOCKSIZE,
                                Columns = COLUMNS,
                                Input = input,
                                CalcResult = results
                              };

            return Task.FromResult(calcResult);
        }
    }
}
