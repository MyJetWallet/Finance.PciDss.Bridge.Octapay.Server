using Finance.PciDss.Bridge.Octapay.Server.Services.Integrations.Contracts.Requests;
using Finance.PciDss.Bridge.Octapay.Server.Services.Integrations.Contracts.Responses;
using System.Threading.Tasks;

namespace Finance.PciDss.Bridge.Octapay.Server.Services.Integrations
{
    public interface IOctapayHttpClient
    {
        Task<Response<CreateOctapayInvoiceResponse, string>> RegisterInvoiceAsync(
            CreateOctapayInvoiceRequest request);
    }
}