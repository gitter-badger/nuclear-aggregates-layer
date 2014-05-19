using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;

using DoubleGis.Erm.BLCore.API.Aggregates.Releases.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Releasing.Releases;
using DoubleGis.Erm.BLCore.API.Releasing.Releases.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Common.Utils;

namespace DoubleGis.Erm.BLCore.Releasing.Release.Old
{
    public sealed class PrepareValidationReportHandler : RequestHandler<PrepareValidationReportRequest, StreamResponse>
    {
        private readonly IReleaseReadModel _releaseRepository;
        private readonly IGlobalizationSettings _globalizationSettings;

        public PrepareValidationReportHandler(IReleaseReadModel releaseRepository, IGlobalizationSettings globalizationSettings)
        {
            _releaseRepository = releaseRepository;
            _globalizationSettings = globalizationSettings;
        }

        protected override StreamResponse Handle(PrepareValidationReportRequest request)
        {
            var reportLines = CreateReportLines(request.ValidationResults);
            var dataTable = CreateReportDataTable(reportLines);
            var csvReportContent = dataTable.ToCsv(_globalizationSettings.ApplicationCulture.TextInfo.ListSeparator, true);
            var reportFileName = CreateReportFileName(request);

            return new StreamResponse
                {
                    Stream = new MemoryStream(Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csvReportContent)).ToArray()),
                    FileName = reportFileName,
                    ContentType = MediaTypeNames.Application.Octet,
                };
        }

        private static DataTable CreateReportDataTable(IEnumerable<ValidationReportLine> reportLines)
        {
            var dataTable = new DataTable { Locale = CultureInfo.CurrentCulture };

            dataTable.Columns.Add(MetadataResources.OrderNumber);
            dataTable.Columns.Add(BLResources.ManagerFullName);
            dataTable.Columns.Add(MetadataResources.Firm);
            dataTable.Columns.Add(MetadataResources.LegalPerson);
            dataTable.Columns.Add(BLResources.Error);

            foreach (var reportLine in reportLines)
            {
                    dataTable.Rows.Add(
                    reportLine.Number,
                    reportLine.OwnerName,
                    reportLine.FirmName,
                    reportLine.LegalPersonName,
                        reportLine.ValidationMessage);
                }

            return dataTable;
        }

        private string CreateReportFileName(PrepareValidationReportRequest request)
        {
            var organizationUnitName = _releaseRepository.GetOrganizationUnitName(request.OrganizationUnitId);
            return string.Format("{0:dd-MM-yy}_{1:dd-MM-yy}_{2}.csv", request.Period.Start, request.Period.End, organizationUnitName);
        }

        private IEnumerable<ValidationReportLine> CreateReportLines(IEnumerable<ReleaseProcessingMessage> validationResults)
        {
            // NOTE: DataContract для StartExportRequest, StartExportResponse, FinishExportRequest и FinishExportResponse обновлен на использование Int64-идентификаторов
            // TODO {d.ivanov, 28.05.2013}: Актуализировать при полном переходе ERM на Int64-идентификаторы
            var projectedValidationResults = validationResults.Select(x => new { OrderId = x.OrderId, x.Message }).ToArray();

            var orderIds = projectedValidationResults
                .Where(result => result.OrderId.HasValue)
                .Select(x => x.OrderId.Value)
                .ToArray();
            var orderInfosMap = _releaseRepository.GetOrderValidationLines(orderIds);

            return projectedValidationResults.Select(result =>
            {
                // Костыль: учитываем, что в таблицу Billing.ReleaseValidationResults мы записываем пришедшие с экспорта OrderId = 0.
                if (!result.OrderId.HasValue || result.OrderId.Value == 0)
                {
                    return new ValidationReportLine { ValidationMessage = result.Message };
                }

                var line = orderInfosMap[result.OrderId.Value];
                line.ValidationMessage = result.Message;
                return line;
            });
        }
    }
}
