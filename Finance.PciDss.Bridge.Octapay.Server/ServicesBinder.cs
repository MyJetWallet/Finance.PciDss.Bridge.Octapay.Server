using Finance.PciDss.Bridge.Octapay.Server.Services.Integrations;
using Microsoft.Extensions.DependencyInjection;
using MyCrm.AuditLog.Grpc;
using Serilog;
using SimpleTrading.ConvertService.Grpc;
using SimpleTrading.GrpcTemplate;
using SimpleTrading.MyLogger;
using SimpleTrading.SettingsReader;

namespace Finance.PciDss.Bridge.Octapay.Server
{
    public static class ServicesBinder
    {
        public static string AppName { get; private set; } = "Finance.PciDss.BridgeOctapay.Server";

        public static void BindOctapayHttpCLient(this IServiceCollection services)
        {
            services.AddSingleton<IOctapayHttpClient, OctapayHttpClient>();
        }

        public static void BindLogger(this IServiceCollection services, SettingsModel settings)
        {
            var logger = new MyLogger(AppName, settings.SeqServiceUrl);
            services.AddSingleton<ILogger>(logger);
            Log.Logger = logger;
        }

        public static void BindSettings(this IServiceCollection services)
        {
            services.AddSingleton<ISettingsModelProvider, SettingsModelProvider>();
        }

        public static void BindGrpcServices(this IServiceCollection services)
        {
            var clientAuditLogGrpcService = new GrpcServiceClient<IMyCrmAuditLogGrpcService>(
                () => SettingsReader
                    .ReadSettings<SettingsModel>()
                    .AuditLogGrpcServiceUrl);

            services.AddSingleton(clientAuditLogGrpcService);
        }
    }
}
