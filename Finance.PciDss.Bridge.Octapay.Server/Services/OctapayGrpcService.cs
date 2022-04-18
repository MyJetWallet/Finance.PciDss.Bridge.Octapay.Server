using System;
using System.Threading.Tasks;
using Finance.PciDss.Abstractions;
using Finance.PciDss.Bridge.Octapay.Server.Services.Extensions;
using Finance.PciDss.Bridge.Octapay.Server.Services.Integrations;
using Finance.PciDss.PciDssBridgeGrpc;
using Finance.PciDss.PciDssBridgeGrpc.Contracts;
using Finance.PciDss.PciDssBridgeGrpc.Contracts.Enums;
using Flurl;
using MyCrm.AuditLog.Grpc;
using MyCrm.AuditLog.Grpc.Models;
using Serilog;
using SimpleTrading.Common.Helpers;
using SimpleTrading.ConvertService.Grpc;
using SimpleTrading.ConvertService.Grpc.Contracts;
using SimpleTrading.GrpcTemplate;

namespace Finance.PciDss.Bridge.Octapay.Server.Services
{
    public class OctapayGrpcService : IFinancePciDssBridgeGrpcService
    {
        private const string PaymentSystemId = "pciDssOctapayBankCards";
        private const string UsdCurrency = "USD";
        private readonly GrpcServiceClient<IMyCrmAuditLogGrpcService> _myCrmAuditLogGrpcService;
        private readonly ISettingsModelProvider _settingsModelProvider;
        private readonly ILogger _logger;
        private readonly IOctapayHttpClient _octapayHttpClient;

        public OctapayGrpcService(IOctapayHttpClient octapayHttpClient,
            GrpcServiceClient<IMyCrmAuditLogGrpcService> myCrmAuditLogGrpcService,
            ISettingsModelProvider settingsModelProvider,
            ILogger logger)
        {
            _octapayHttpClient = octapayHttpClient;
            _myCrmAuditLogGrpcService = myCrmAuditLogGrpcService;
            _settingsModelProvider = settingsModelProvider;
            _logger = logger;
        }

        private SettingsModel SettingsModel => _settingsModelProvider.Get();

        public async ValueTask<MakeBridgeDepositGrpcResponse> MakeDepositAsync(MakeBridgeDepositGrpcRequest request)
        {
            _logger.Information("OctapayGrpcService start process MakeBridgeDepositGrpcRequest {@request}", request);
            try
            {
                request.PciDssInvoiceGrpcModel.Country = CountryManager.Iso3ToIso2(request.PciDssInvoiceGrpcModel.Country);

                var response =
                    await _octapayHttpClient.RegisterInvoiceAsync(
                        request.PciDssInvoiceGrpcModel.ToOctapayRestModel(SettingsModel));

                if (response.IsFailed || response.SuccessResult is null || response.SuccessResult.IsFailed())
                {
                    _logger.Information("Octapay Fail create invoice. {@response}", response);
                    await SendMessageToAuditLogAsync(request.PciDssInvoiceGrpcModel, "Fail Octapay create invoice. Error" + response.FailedResult ?? response.SuccessResult?.GetError());
                    return MakeBridgeDepositGrpcResponse.Failed(DepositBridgeRequestGrpcStatus.ServerError,
                        response.FailedResult ?? response.SuccessResult?.GetError());
                }

                if (response.SuccessResult.IsSuccessWithoutRedirectTo3ds())
                {
                    response.SuccessResult.RedirectUrl = SettingsModel.OctapayRedirectUrl
                        .SetQueryParam("status", "success")
                        .SetQueryParam("message", "transaction was success without 3ds", false)
                        .SetQueryParam("order_id", response.SuccessResult.PsTransactionId)
                        .SetQueryParam("customer_order_id", request.PciDssInvoiceGrpcModel.OrderId);
                    _logger.Information("Octapay RedirectUrl {url} was built for traderId {traderId} and orderid {orderid}", response.SuccessResult.RedirectUrl,
                    request.PciDssInvoiceGrpcModel.TraderId, request.PciDssInvoiceGrpcModel.OrderId);
                }

                await SendMessageToAuditLogAsync(request.PciDssInvoiceGrpcModel, $"Created deposit invoice with id {request.PciDssInvoiceGrpcModel.OrderId}");
                return MakeBridgeDepositGrpcResponse.Create(response.SuccessResult.RedirectUrl, response.SuccessResult.PsTransactionId, DepositBridgeRequestGrpcStatus.Success);
            }
            catch (Exception e)
            {
                _logger.Error(e, "OctapayGrpcService. MakeDepositAsync failed for traderId {traderId}",
                    request.PciDssInvoiceGrpcModel.TraderId);
                await SendMessageToAuditLogAsync(request.PciDssInvoiceGrpcModel,
                    $"MakeDepositAsync failed for traderId {request.PciDssInvoiceGrpcModel.TraderId}");
                return MakeBridgeDepositGrpcResponse.Failed(DepositBridgeRequestGrpcStatus.ServerError, e.Message);
            }
        }

        public ValueTask<GetPaymentSystemGrpcResponse> GetPaymentSystemNameAsync()
        {
            return new ValueTask<GetPaymentSystemGrpcResponse>(GetPaymentSystemGrpcResponse.Create(PaymentSystemId));
        }

        public ValueTask<GetPaymentSystemCurrencyGrpcResponse> GetPsCurrencyAsync()
        {
            return new ValueTask<GetPaymentSystemCurrencyGrpcResponse>(
                GetPaymentSystemCurrencyGrpcResponse.Create(UsdCurrency));
        }

        public async ValueTask<GetPaymentSystemAmountGrpcResponse> GetPsAmountAsync(GetPaymentSystemAmountGrpcRequest request)
        {
            if (request.Currency.Equals(UsdCurrency, StringComparison.OrdinalIgnoreCase))
            {
                return GetPaymentSystemAmountGrpcResponse.Create(request.Amount, request.Currency);
            }
                
            return default;
        }

        public ValueTask<GetDepositStatusGrpcResponse> GetDepositStatusAsync(GetDepositStatusGrpcRequest request)
        {
            throw new NotImplementedException();
        }

        public ValueTask<DecodeBridgeInfoGrpcResponse> DecodeInfoAsync(DecodeBridgeInfoGrpcRequest request)
        {
            throw new NotImplementedException();
        }

        public ValueTask<MakeConfirmGrpcResponse> MakeDepositConfirmAsync(MakeConfirmGrpcRequest request)
        {
            throw new NotImplementedException();
        }

        private ValueTask SendMessageToAuditLogAsync(IPciDssInvoiceModel invoice, string message)
        {
            return _myCrmAuditLogGrpcService.Value.SaveAsync(new AuditLogEventGrpcModel
            {
                TraderId = invoice.TraderId,
                Action = "deposit",
                ActionId = invoice.OrderId,
                DateTime = DateTime.UtcNow,
                Message = message
            });
        }
    }
}