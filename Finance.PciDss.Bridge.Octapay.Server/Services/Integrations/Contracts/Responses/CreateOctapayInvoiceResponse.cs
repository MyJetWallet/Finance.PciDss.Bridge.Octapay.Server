using Destructurama.Attributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.PciDss.Bridge.Octapay.Server.Services.Integrations.Contracts.Responses
{
    public class CreateOctapayInvoiceResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("customer_order_id")]
        public string CustomerOrderId { get; set; }

        [JsonProperty("order_id")]
        public string PsTransactionId { get; set; }

        [LogMasked(ShowFirst = 5, ShowLast = 3, PreserveLength = true)]
        [JsonProperty("api_key")]
        public string ApiKey { get; set; }

        [JsonProperty("errors")]
        public object Errors { get; set; }

        [JsonProperty("redirect_3ds_url")]
        public string RedirectUrl { get; set; }

        public bool IsFailed()
        {
            return string.IsNullOrEmpty(Status) || Status.Equals("fail", StringComparison.OrdinalIgnoreCase);
        }

        public bool IsSuccessWithoutRedirectTo3ds()
        {
            return !string.IsNullOrEmpty(Status) 
                && Status.Equals("success", StringComparison.OrdinalIgnoreCase)
                && string.IsNullOrEmpty(RedirectUrl);
        }

        public bool ShouldBeRedirectTo3ds()
        {
            return !string.IsNullOrEmpty(Status)
                && Status.Equals("3d_redirect", StringComparison.OrdinalIgnoreCase);
        }

        public string GetError()
        {
            return $"Message {Message}, Errors {Errors}";
        }
    }
}
