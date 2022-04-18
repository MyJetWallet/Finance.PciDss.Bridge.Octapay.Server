using SimpleTrading.SettingsReader;

namespace Finance.PciDss.Bridge.Octapay.Server
{
    [YamlAttributesOnly]
    public class SettingsModel
    {
        [YamlProperty("PciDssBridgeOctapay.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("PciDssBridgeOctapay.AuditLogGrpcServiceUrl")]
        public string AuditLogGrpcServiceUrl { get; set; }

        [YamlProperty("PciDssBridgeOctapay.OctapayApiUrl")]
        public string OctapayApiUrl { get; set; }

        [YamlProperty("PciDssBridgeOctapay.OctapayRedirectUrl")]
        public string OctapayRedirectUrl { get; set; }

        [YamlProperty("PciDssBridgeOctapay.OctapayNotifyUrl")]
        public string OctapayNotifyUrl { get; set; }

        [YamlProperty("PciDssBridgeOctapay.OctapayKey")]
        public string OctapayKey { get; set; }
    }
}