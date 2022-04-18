using System.Threading.Tasks;
using Finance.PciDss.Abstractions;
using Finance.PciDss.Bridge.Octapay.Server.Services.Integrations.Contracts.Requests;
using Finance.PciDss.Bridge.Octapay.Server.Services.Integrations.Contracts.Responses;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;
using Serilog;

namespace Finance.PciDss.Bridge.Octapay.Server.Services.Integrations
{
    public class OctapayHttpClient : IOctapayHttpClient
    {
        private readonly ISettingsModelProvider _settingsModelProvider;

        public OctapayHttpClient(ISettingsModelProvider settingsModelProvider)
        {
            _settingsModelProvider = settingsModelProvider;
        }

        private SettingsModel SettingsModel => _settingsModelProvider.Get();

        public async Task<Response<CreateOctapayInvoiceResponse, string>> RegisterInvoiceAsync(
            CreateOctapayInvoiceRequest request)
        {
            Log.Logger.Information("Octapay send request : {@requests}, link {link}", request, SettingsModel.OctapayApiUrl);
            var result = await SettingsModel
                .OctapayApiUrl
                .AppendPathSegments("api", "transaction")
                .WithHeader("Content-Type", "application/json")
                .AllowHttpStatus("400,422")
                .PostStringAsync(JsonConvert.SerializeObject(request));

            return await result.DeserializeTo<CreateOctapayInvoiceResponse, string>();
        }
    }
}