using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;

namespace KubeCalcWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var clusterId = "LocalCluster";

            var siloAddresses = Configuration["silohost"] == null ? new[] { IPAddress.Loopback } : Dns.GetHostEntry(Configuration["silohost"]).AddressList;

            services.AddSingleton<IClusterClient>(s => {

                while (true)
                {
                    try
                    {
                        var client = new ClientBuilder()
                                .Configure<ClusterOptions>(o =>
                                {
                                    o.ClusterId = clusterId;
                                    o.ServiceId = new Guid("aeb9598c-37f6-4590-aa22-a9b945b23e14");
                                })
                                .UseStaticClustering(siloAddresses.Select(a => new IPEndPoint(a, 40000)).ToArray())
                                .ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Information).AddConsole())
                                .Build();

                        client.Connect().Wait();
                        return client;
                    }
                    catch (AggregateException aex)
                    {
                        var baseEx = aex.InnerException;
                        Console.WriteLine(baseEx.Message);
                    }
                    System.Threading.Thread.Sleep(2000);
                }                
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
