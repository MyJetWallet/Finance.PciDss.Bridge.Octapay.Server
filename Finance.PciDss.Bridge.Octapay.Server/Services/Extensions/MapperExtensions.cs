using System;
using System.Globalization;
using System.Linq;
using Finance.PciDss.Abstractions;
using Finance.PciDss.Bridge.Octapay.Server.Services.Integrations.Contracts.Requests;

namespace Finance.PciDss.Bridge.Octapay.Server.Services.Extensions
{
    public static class MapperExtensions
    {
        public static CreateOctapayInvoiceRequest ToOctapayRestModel(this IPciDssInvoiceModel model, SettingsModel settingsModel)
        {
            const string defaultField = "none"; 
            var nameArray = model.FullName.Split(" ");
            var lastName = "";

            string firstName;
            if (nameArray.Length >= 2)
            {
                firstName = nameArray.FirstOrDefault();
                lastName = nameArray.Skip(1).FirstOrDefault();
            }
            else
            {
                firstName = nameArray.FirstOrDefault() ?? model.FullName;
            }

            return new CreateOctapayInvoiceRequest
            {
                Amount = model.PsAmount.ToString(CultureInfo.InvariantCulture),
                Currency = model.PsCurrency,
                Address = model.Address ?? defaultField,
                CardNo = model.CardNumber,
                CcExpiryMonth = model.ExpirationDate.ToString("MM"),
                CcExpiryYear = model.ExpirationDate.ToString("yyyy"),
                City = model.City ?? defaultField,
                CvvNumber = model.Cvv,
                Country = model.Country,
                Email= model.Email,
                CustomerOrderId = model.OrderId,
                FirstName = firstName,
                LastName = lastName,
                IpAddress = model.Ip,
                PhoneNo= model.PhoneNumber ?? defaultField,
                ResponseUrl = settingsModel.OctapayRedirectUrl,
                WebhookUrl = settingsModel.OctapayNotifyUrl,
                State = "none",
                Zip = model.Zip ?? defaultField,
                ApiKey = settingsModel.OctapayKey
            };
        }
    }
}
