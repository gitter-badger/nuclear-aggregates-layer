using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Text;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Utils;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders
{
    public sealed class GetOrdersWithDummyAdvertisementHandler : RequestHandler<GetOrdersWithDummyAdvertisementRequest, GetOrdersWithDummyAdvertisementResponse>
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;
        private readonly IReleaseReadModel _releaseReadModel;
        private readonly IGlobalizationSettings _globalizationSettings;

        public GetOrdersWithDummyAdvertisementHandler(IReleaseReadModel releaseReadModel,
                                                      ISecurityServiceUserIdentifier securityServiceUserIdentifier,
                                                      IOrderReadModel orderReadModel,
                                                      IGlobalizationSettings globalizationSettings)
        {
            _releaseReadModel = releaseReadModel;
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
            _orderReadModel = orderReadModel;
            _globalizationSettings = globalizationSettings;
        }

        protected override GetOrdersWithDummyAdvertisementResponse Handle(GetOrdersWithDummyAdvertisementRequest request)
        {
            var checkResponse = new GetOrdersWithDummyAdvertisementResponse
            {
                ContentType = MediaTypeNames.Application.Octet,
                ReportFileName = CreateReportFileName(request),
            };

            var ordersWithDummyAdvertisement = _orderReadModel.GetOrdersWithDummyAdvertisement(request.OrganizationUnitId, request.OwnerId, request.IncludeOwnerDescendants);

            if (!ordersWithDummyAdvertisement.Any())
            {
                checkResponse.HasOrders = false;
                checkResponse.ReportContent = Encoding.GetEncoding(1251).GetBytes(string.Empty);
                checkResponse.Message = BLResources.CannotFindAppropriateOrders;
            }
            else
            {
                var dataTable = CreateReportDataTable(ordersWithDummyAdvertisement);
                var csvReportContent = dataTable.ToCsvEscaped(_globalizationSettings.ApplicationCulture.TextInfo.ListSeparator, true);
                var reportContent = Encoding.GetEncoding(1251).GetBytes(csvReportContent);

                checkResponse.HasOrders = true;
                checkResponse.ReportContent = reportContent;
            }

            return checkResponse;
        }

        private static DataTable CreateReportDataTable(IEnumerable<OrderWithDummyAdvertisementDto> reportLines)
        {
            var dataTable = new DataTable { Locale = CultureInfo.CurrentCulture };

            dataTable.Columns.Add(MetadataResources.OrderNumber);
            dataTable.Columns.Add(MetadataResources.SourceOrganizationUnit);
            dataTable.Columns.Add(MetadataResources.DestOrganizationUnit);
            dataTable.Columns.Add(MetadataResources.Firm);
            dataTable.Columns.Add(MetadataResources.BeginDistributionDate);
            dataTable.Columns.Add(MetadataResources.WorkflowStepId);
            dataTable.Columns.Add(MetadataResources.Owner);

            if (!reportLines.Any())
            {
                return dataTable;
            }

            foreach (var reportLine in reportLines)
            {
                dataTable.Rows.Add(reportLine.Number,
                                   reportLine.SourceOrganizationUnitName,
                                   reportLine.DestOrganizationUnitName,
                                   reportLine.FirmName,
                                   reportLine.BeginDistributionDate,
                                   reportLine.WorkflowStep,
                                   reportLine.OwnerName);
            }

            return dataTable;
        }

        private string CreateReportFileName(GetOrdersWithDummyAdvertisementRequest request)
        {
            var reportFileNameBuilder = new StringBuilder();

            var organizationUnitName = _releaseReadModel.GetOrganizationUnitName(request.OrganizationUnitId);
            reportFileNameBuilder.AppendFormat("_{0}", organizationUnitName);

            var ownerName = _securityServiceUserIdentifier.GetUserInfo(request.OwnerId).DisplayName;
            reportFileNameBuilder.AppendFormat("_{0}", ownerName);

            reportFileNameBuilder.Append(".csv");
            return reportFileNameBuilder.ToString();
        }
    }
}
