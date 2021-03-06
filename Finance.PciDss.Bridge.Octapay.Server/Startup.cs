using System;
using Finance.PciDss.Bridge.Octapay.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;
using ProtoBuf.Grpc.Server;
using SimpleTrading.BaseMetrics;
using SimpleTrading.ServiceStatusReporterConnector;

namespace Finance.PciDss.Bridge.Octapay.Server
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            SettingsModel settingsModel = SimpleTrading.SettingsReader.SettingsReader.ReadSettings<SettingsModel>();
            services.BindSettings();
            services.BindLogger(settingsModel);
            services.BindOctapayHttpCLient();
            services.BindGrpcServices();
            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddControllers();
            services.AddGrpc();
            services.AddCodeFirstGrpc(option =>
            {
                option.Interceptors.Add<ErrorLoggerInterceptor>();
                option.BindMetricsInterceptors();
            });            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.BindServicesTree(typeof(Startup).Assembly);
            app.BindIsAlive();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGrpcService<OctapayGrpcService>();
                endpoints.MapMetrics();
            });
        }
    }
}
