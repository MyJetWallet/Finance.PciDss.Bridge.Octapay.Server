using Destructurama.Attributed;
using Newtonsoft.Json;

namespace Finance.PciDss.Bridge.Octapay.Server.Services.Integrations.Contracts.Requests
{
    public class CreateOctapayInvoiceRequest
    {
        [LogMasked(ShowFirst = 5, ShowLast = 3, PreserveLength = true)]
        [JsonProperty("api_key")]
        public string ApiKey { get; set; }

        [LogMasked(ShowFirst = 1, ShowLast = 1, PreserveLength = true)]
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [LogMasked(ShowFirst = 1, ShowLast = 1, PreserveLength = true)]
        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [LogMasked(ShowFirst = 3, ShowLast = 3, PreserveLength = true)]
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("zip")]
        public string Zip { get; set; }

        [LogMasked(ShowFirst = 3, ShowLast = 3, PreserveLength = true)]
        [JsonProperty("email")]
        public string Email { get; set; }

        [LogMasked(ShowFirst = 3, ShowLast = 3, PreserveLength = true)]
        [JsonProperty("phone_no")]
        public string PhoneNo { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [LogMasked(ShowFirst = 6, ShowLast = 4, PreserveLength = true)]
        [JsonProperty("card_no")]
        public string CardNo { get; set; }

        [LogMasked(ShowFirst = 0, ShowLast = 0, PreserveLength = true)]
        [JsonProperty("ccExpiryMonth")]
        public string CcExpiryMonth { get; set; }

        [LogMasked(ShowFirst = 0, ShowLast = 0, PreserveLength = true)]
        [JsonProperty("ccExpiryYear")]
        public string CcExpiryYear { get; set; }

        [LogMasked(ShowFirst = 0, ShowLast = 0, PreserveLength = true)]
        [JsonProperty("cvvNumber")]
        public string CvvNumber { get; set; }

        [JsonProperty("ip_address")]
        public string IpAddress { get; set; }

        [JsonProperty("response_url")]
        public string ResponseUrl { get; set; }

        [JsonProperty("webhook_url")]
        public string WebhookUrl { get; set; }

        [JsonProperty("customer_order_id")]
        public string CustomerOrderId { get; set; }
    }
}