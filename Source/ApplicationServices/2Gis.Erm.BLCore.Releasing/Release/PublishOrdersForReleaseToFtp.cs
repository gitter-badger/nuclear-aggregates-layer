using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;

using DoubleGis.Erm.BLCore.API.Releasing.Releases;
using DoubleGis.Erm.BLCore.Releasing.Release.Configuration;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Common.Compression;
using DoubleGis.Erm.Platform.Common.Ftp;
using DoubleGis.Erm.Platform.Common.Logging;

namespace DoubleGis.Erm.BLCore.Releasing.Release
{
    public sealed class PublishOrdersForReleaseToFtp : IPublishOrdersForReleaseToFileStorage
    {
        private const string OrdersXmlFileNameTemplate = "orders_{0}_{1}.xml{2}";
        private const string OrdersXmlFileNameDateFormat = "ddMMyyy";

        private readonly IFtpExportIntegrationModeSettings _ftpExportIntegrationModeSettings;
        private readonly IFtpService _ftpService;
        private readonly ICommonLog _logger;

        public PublishOrdersForReleaseToFtp(
            IFtpExportIntegrationModeSettings ftpExportIntegrationModeSettings, 
            IFtpService ftpService, 
            ICommonLog logger)
        {
            _ftpExportIntegrationModeSettings = ftpExportIntegrationModeSettings;
            _ftpService = ftpService;
            _logger = logger;
        }

        public void Publish(long organizationUnitId, int organizationUnitDgppId, TimePeriod period, bool isBeta, Stream ordersStream)
        {
            _logger.InfoFormatEx(
                "Starting publish orders with advertisement materials to FTP. Used ftp {0}. Release detail: organization unit id {1}, period {2}, {3} release",
                _ftpExportIntegrationModeSettings.FtpExportSite,
                organizationUnitId,
                period,
                isBeta ? "beta" : "final");

            var xmlExportFileName = string.Format(OrdersXmlFileNameTemplate,
                                                  period.Start.ToString(OrdersXmlFileNameDateFormat),
                                                  period.End.ToString(OrdersXmlFileNameDateFormat),
                                                  string.Empty);

            using (var zipOutputStream = ordersStream.ZipStream(xmlExportFileName))
            {
                var exportZipFileName = string.Format(OrdersXmlFileNameTemplate,
                                                      period.Start.ToString(OrdersXmlFileNameDateFormat),
                                                      period.End.ToString(OrdersXmlFileNameDateFormat),
                                                      ".zip");
                var networkCredential = new NetworkCredential(_ftpExportIntegrationModeSettings.FtpExportSiteUsername,
                                                              _ftpExportIntegrationModeSettings.FtpExportSitePassword);

                var organizationUnitDgppIdString = organizationUnitDgppId.ToString();
                var organizationUnitSettings = ExportIntegrationConfiguration.OrganizationUnits.SingleOrDefault(x => x.Key == organizationUnitDgppIdString);
                if (organizationUnitSettings == null)
                {
                    throw new ConfigurationErrorsException(string.Format("В файле exportIntegration.config не задана конфигурация для города с DgppId = {0}",
                                                                         organizationUnitDgppId));
                }

                var folderName = organizationUnitSettings.Value;
                _ftpService.UploadFile(Path.Combine(_ftpExportIntegrationModeSettings.FtpExportSite, folderName),
                                       networkCredential,
                                       exportZipFileName,
                                       zipOutputStream);
            }

            _logger.InfoFormatEx(
                "Finished publishing orders with advertisement materials to FTP. Used ftp {0}. Release detail: organization unit id {1}, period {2}, {3} release",
                _ftpExportIntegrationModeSettings.FtpExportSite,
                organizationUnitId,
                period,
                isBeta ? "beta" : "final");
        }
    }
}