using KubeCalc.Grains;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace KubeCalc.Silo
{
    class Program
    {
        private bool siloStopping = false; 
        private readonly object syncLock = new object();
        private readonly ManualResetEvent _siloStopped = new ManualResetEvent(false);
        private ISiloHost _silo;
        static void Main(string[] args)
        {
            var main = new Program();
            main.Start(args).Wait();
        }

        public async Task Start(string[] args)
        {
            var configBuilder = new ConfigurationBuilder();
            var secretPath = "/etc/secrets/";

            if (args.Any(s => s.ToLowerInvariant() == "--localdebug"))
            {
                configBuilder.AddUserSecrets<Program>();
            }
            else
            {
                var secretPathArg = args.FirstOrDefault(s => s.ToLowerInvariant().StartsWith("--secretpath="));
                if (secretPathArg != null)
                {
                    secretPath = secretPathArg.Split('=').Last();
                }

                var secretFile = $"{secretPath}secrets.json";
                Console.WriteLine($"Loading secret from {secretFile}");
                configBuilder.AddJsonFile(secretFile);
            }

            var appConfiguration = configBuilder.Build();

            var clusterId = Environment.GetEnvironmentVariable("ClusterId") ?? "defaultClusterId";

            Console.WriteLine($"ClusterId = {clusterId}");

            var builder = new SiloHostBuilder()
                        .ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Information).AddConsole())
                        .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(KubeCalculatorGrain).Assembly).WithReferences())
                        .Configure<ClusterOptions>(o =>
                        {
                            o.ClusterId = clusterId;
                            o.ServiceId = new Guid("aeb9598c-37f6-4590-aa22-a9b945b23e14");
                        })
                        .Configure<ProcessExitHandlingOptions>(o =>
                        {
                            o.FastKillOnProcessExit = true;
                        })
                        .ConfigureEndpoints(22222, 40000, AddressFamily.InterNetwork, true)
                        .AddAzureTableGrainStorageAsDefault(o => o.ConnectionString = appConfiguration["AzureStorageConnectionString"])
                        .UseAzureStorageClustering(o => o.ConnectionString = appConfiguration["AzureStorageConnectionString"]);                        

            _silo = builder.Build();

            SetupApplicationShutdown();

            try
            {
                await _silo.StartAsync();

                System.Console.WriteLine("Silo is running...");

                _siloStopped.WaitOne();
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                throw;
            }                      

            await Task.Delay(1000);
            System.Console.WriteLine("*****************Program has exit********************");
        }

        private void SetupApplicationShutdown()
        {
            /// Capture the user pressing Ctrl+C 
            Console.CancelKeyPress += (s, a) => {
            
                /// Prevent the application from crashing ungracefully. 
                a.Cancel = true;
                    /// Don't allow the following code to repeat if the user presses Ctrl+C repeatedly. 

                lock (syncLock)
                    {
                        if (!siloStopping)
                        {
                            siloStopping = true;
                            Task.Run(StopSilo);
                        }
                    }
                /// Event handler execution exits immediately, leaving the silo shutdown running on a background thread, 
                /// but the app doesn't crash because a.Cancel has been set = true 
            };
        }

        private async Task StopSilo()
        {
            await _silo.StopAsync();
            System.Console.WriteLine("*****************Silo has stopped********************");
            _siloStopped.Set();
        }
    }
}
