using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;

using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.API.OrderValidation.Remote;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

namespace DoubleGis.Erm.BLCore.OrderValidation
{
    public sealed class CheckOrdersReadinessForReleaseHandler : RequestHandler<CheckOrdersReadinessForReleaseRequest, CheckOrdersReadinessForReleaseResponse>
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;
        private readonly IReleaseReadModel _releaseReadModel;
        private readonly IClientProxyFactory _clientProxyFactory;

        public CheckOrdersReadinessForReleaseHandler(IOrderReadModel orderReadModel,
            IReleaseReadModel releaseReadModel,
            ISecurityServiceUserIdentifier securityServiceUserIdentifier,
            IClientProxyFactory clientProxyFactory)
        {
            _orderReadModel = orderReadModel;
            _releaseReadModel = releaseReadModel;
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
            _clientProxyFactory = clientProxyFactory;
        }

        protected override CheckOrdersReadinessForReleaseResponse Handle(CheckOrdersReadinessForReleaseRequest request)
        {
            var checkResponse = new CheckOrdersReadinessForReleaseResponse
                               {
                                   ContentType = "text/csv",
                                   ReportFileName = CreateReportFileName(request),
                               };

            var clientProxy = _clientProxyFactory.GetClientProxy<IOrderValidationApplicationService, WSHttpBinding>();

            var validationResults = clientProxy.Execute(orderValidationApplicationService => orderValidationApplicationService.ValidateOrders(
                request.CheckAccountBalance ? ValidationType.ManualReportWithAccountsCheck : ValidationType.ManualReport,
                request.OrganizationUnitId.Value,
                request.Period,
                request.OwnerId,
                request.IncludeOwnerDescendants));

            var errorCount = validationResults.Messages.Count(x => x.Type == MessageType.Error);
            var warningCount = validationResults.Messages.Count(x => x.Type == MessageType.Warning);
            if (errorCount == 0 && warningCount == 0)
            {
                checkResponse.HasErrors = false;
                checkResponse.ReportContent = Encoding.UTF8.GetBytes(string.Empty);
                checkResponse.Message = validationResults.OrderCount > 0 ?
                    string.Format(BLResources.ChekedOrdersCount_NoErrors, validationResults.OrderCount) : 
                    BLResources.CannotFindAppropriateOrders;
            }
            else
            {
                var significantMessages = validationResults.Messages.Where(x => x.Type == MessageType.Error || x.Type == MessageType.Warning).ToArray();
                var reportLines = PrepareReportLines(significantMessages);
                var dataTable = CreateReportDataTable(reportLines);
                var csvReportContent = dataTable.ToCsvEscaped(BLResources.CsvSeparator, true);
                var reportContent = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csvReportContent));

                checkResponse.Message = string.Format(BLResources.CheckedOrdersCount_ThereAreErorrs, validationResults.OrderCount, errorCount, warningCount);

                checkResponse.HasErrors = true;
                checkResponse.ReportContent = reportContent.ToArray();
            }
            return checkResponse;
        }

        private static DataTable CreateReportDataTable(ReportLine[] reportLines)
        {
            var dataTable = new DataTable { Locale = CultureInfo.CurrentCulture };

            dataTable.Columns.Add(BLResources.Type);
            dataTable.Columns.Add(MetadataResources.OrderNumber);
            dataTable.Columns.Add(MetadataResources.SourceOrganizationUnit);
            dataTable.Columns.Add(MetadataResources.DestOrganizationUnit);
            dataTable.Columns.Add(BLResources.ManagerFullName);
            dataTable.Columns.Add(MetadataResources.Firm);
            dataTable.Columns.Add(MetadataResources.LegalPerson);
            dataTable.Columns.Add(BLResources.ResultOfCheck);

            if (!reportLines.Any())
                return dataTable;

            foreach (var reportLine in reportLines)
            {
                if (reportLine.AdditionalInfo != null)
                {
                    dataTable.Rows.Add(reportLine.Type.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                                       reportLine.AdditionalInfo.Number,
                                       reportLine.AdditionalInfo.SourceOrganizationUnitName,
                                       reportLine.AdditionalInfo.DestOrganizationUnitName,
                                       reportLine.AdditionalInfo.OwnerName,
                                       reportLine.AdditionalInfo.FirmName,
                                       reportLine.AdditionalInfo.LegalPersonName,
                                       reportLine.ValidationMessage);
                }
                else
                {
                    dataTable.Rows.Add(reportLine.Type.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                                       null,
                                       null,
                                       null,
                                       null,
                                       reportLine.ValidationMessage);
                }
            }

            return dataTable;
        }

        private ReportLine[] PrepareReportLines(OrderValidationMessage[] validationMessages)
        {
            const int takeRows = 20;
            var orderInfos = new Dictionary<long, OrderValidationAdditionalInfo>();
            var orderIds = validationMessages.Select(vr => vr.OrderId).Distinct().ToArray();

            for (var i = 0; i < Math.Ceiling((decimal)orderIds.Length / takeRows); i++)
            {
                var orderIdsFilter = orderIds.Skip(takeRows * i).Take(takeRows).ToArray();

                var orderInfosPart = _orderReadModel.GetOrderValidationAdditionalInfos(orderIdsFilter);
                CsvFixup(orderInfosPart);

                foreach (var orderInfo in orderInfosPart)
                {
                    orderInfos[orderInfo.Id] = orderInfo;
                }
            }

            return validationMessages
                .Select(validationResult => new ReportLine
                    {
                        AdditionalInfo = orderInfos.ContainsKey(validationResult.OrderId) ? orderInfos[validationResult.OrderId] : null,
                        ValidationMessage = validationResult.MessageText,
                        Type = validationResult.Type,
                    })
                .ToArray();
        }

        private string CreateReportFileName(CheckOrdersReadinessForReleaseRequest request)
        {
            var reportFileNameBuilder = new StringBuilder();
            reportFileNameBuilder.AppendFormat("{0:dd-MM-yy}_{1:dd-MM-yy}", request.Period.Start, request.Period.End);

            if (request.OrganizationUnitId.HasValue)
            {
                var organizationUnitName = _releaseReadModel.GetOrganizationUnitName(request.OrganizationUnitId.Value);
                reportFileNameBuilder.AppendFormat("_{0}", organizationUnitName);
            }

            if (request.OwnerId.HasValue)
            {
                var ownerName = _securityServiceUserIdentifier.GetUserInfo(request.OwnerId.Value).DisplayName;
                reportFileNameBuilder.AppendFormat("_{0}", ownerName);
            }

            reportFileNameBuilder.Append(".csv");
            return reportFileNameBuilder.ToString();
        }

        private static void CsvFixup(IEnumerable<OrderValidationAdditionalInfo> orderInfosPart)
        {
            foreach (var orderInfo in orderInfosPart)
            {
                if (orderInfo.FirmName.Contains(';'))
                    orderInfo.FirmName = String.Concat("\"", orderInfo.FirmName.Replace("\"", "\"\""), "\"");
                if (orderInfo.LegalPersonName.Contains(';'))
                    orderInfo.LegalPersonName = String.Concat("\"", orderInfo.LegalPersonName.Replace("\"", "\"\""), "\"");
            }
        }

        private sealed class ReportLine
        {
            public string ValidationMessage { get; set; }
            public OrderValidationAdditionalInfo AdditionalInfo { get; set; }
            public MessageType Type { get; set; }
        }
    }
}
