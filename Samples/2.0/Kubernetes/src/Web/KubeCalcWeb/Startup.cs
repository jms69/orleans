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
            
            services.AddSingleton<IClusterClient>(s => {
                var client = new ClientBuilder()                   
                                                .Configure<ClusterOptions>(o =>
                                                {
                                                    o.ClusterId = clusterId;
                                                    o.ServiceId = new Guid("aeb9598c-37f6-4590-aa22-a9b945b23e14");
                                                })                             
                                                .UseStaticClustering(new [] { new IPEndPoint(IPAddress.Loopback, 40000)})
                                                .ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Information).AddConsole())
                                                .Build();
                client.Connect().Wait();

                return client;
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
