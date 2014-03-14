using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.BLCore.API.Releasing.Releases
{
    public enum ExportIntegrationMode
    {
        ServiceBusAndFtp = 1,
        Ftp = 2
    }

    public interface IFtpExportIntegrationModeSettings : ISettings
    {
        ExportIntegrationMode ExportIntegrationMode { get; }
        string FtpExportSite { get; }
        string FtpExportSiteUsername { get; }
        string FtpExportSitePassword { get; } 
    }
}