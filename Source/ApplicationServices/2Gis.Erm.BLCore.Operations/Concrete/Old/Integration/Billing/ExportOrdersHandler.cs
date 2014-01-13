using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Xml;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.Billing;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.Billing
{
    // тут finder потому что этот handler скорее всего вообще не нужен и его надо удалить, уточнить потом у антона
    public sealed class ExportOrdersHandler : RequestHandler<ExportOrdersRequest, IntegrationResponse>
    {
        private readonly IFinder _finder;

        public ExportOrdersHandler(IFinder finder)
        {
            _finder = finder;
        }

        protected override IntegrationResponse Handle(ExportOrdersRequest request)
        {
            var sourceOrganizationUnitDgppId = _finder.Find<OrganizationUnit>(item => item.Id == request.OrganizationUnitId).Select(item => item.DgppId).Single();
            if (sourceOrganizationUnitDgppId == null)
                throw new NotificationException(string.Format(CultureInfo.CurrentCulture, BLResources.OrganizationUnitWithIdDoesNotHaveDgppId, request.OrganizationUnitId));

            // todo: надо решить что делать с 23.59.59 - см проблему MSCRM-2758
            request.BeginDate = request.BeginDate.Date;
            request.EndDate = request.EndDate.Date.AddDays(1d).AddSeconds(-1d);

            var orderInfos = _finder.Find<Order>
                (order => order.IsActive
                                && !order.IsDeleted
                                && order.SourceOrganizationUnitId == request.OrganizationUnitId
                                && (order.BeginDistributionDate <= request.BeginDate && request.EndDate <= order.EndDistributionDateFact)
                                && order.BudgetType == (int)OrderBudgetType.Budget
                                && order.OrderPositions.Count(position => position.IsActive && !position.IsDeleted) > 0
                                && order.WorkflowStepId == (int)OrderState.Approved)
                .SelectMany(order => order.Locks
                                         .Where(@lock => @lock.IsActive && !@lock.IsDeleted &&
                                                         @lock.PeriodStartDate <= request.BeginDate && request.EndDate <= @lock.PeriodEndDate)
                                         .Select(@lock => new
                                                     {
                                                         @lock.Id,
                                                         order.Number,
                                                         FirmId = (long?)order.Firm.Id,
                                                         LegalPersonDgppId = order.LegalPerson.DgppId,
                                                         BranchOfficeOrganizationUnitDgppId = order.BranchOfficeOrganizationUnit.BranchOffice.DgppId,
                                                         order.CreatedOn,
                                                         DestOrganizationUnitDgppId = order.DestOrganizationUnit.DgppId,
                                                         AmountToWithdraw = @lock.PlannedAmount,
                                                         Vat = order.OrderReleaseTotals
                                                                        .Where(ort => ort.ReleaseBeginDate <= request.BeginDate && request.EndDate <= ort.ReleaseEndDate)
                                                                        .Select(ort => (decimal?)ort.Vat)
                                                                        .FirstOrDefault(),
                                                     }))
                .ToArray();

            // save results to xml stream
            var stream = new MemoryStream();
            using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8, OmitXmlDeclaration = false, CloseOutput = false, }))
            {
                writer.WriteStartElement("exchange");
                {
                    // header
                    writer.WriteStartElement("header");
                    {
                        writer.WriteStartElement("periodStart");
                        writer.WriteValue(request.BeginDate);
                        writer.WriteEndElement();

                        writer.WriteStartElement("periodEnd");
                        writer.WriteValue(request.EndDate);
                        writer.WriteEndElement();

                        writer.WriteStartElement("sourceSiteId");
                        writer.WriteValue(sourceOrganizationUnitDgppId.Value);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();

                    // body
                    writer.WriteStartElement("orders");
                    {
                        foreach (var orderInfo in orderInfos)
                        {
                            if (orderInfo.FirmId == null || orderInfo.LegalPersonDgppId == null || orderInfo.DestOrganizationUnitDgppId == null)
                                continue;

                            writer.WriteStartElement("order");

                            writer.WriteStartElement("id");
                            writer.WriteValue(orderInfo.Id);
                            writer.WriteEndElement();

                            writer.WriteStartElement("name");
                            writer.WriteValue(orderInfo.Number);
                            writer.WriteEndElement();

                            writer.WriteStartElement("firmId");
                            writer.WriteValue(orderInfo.FirmId.Value);
                            writer.WriteEndElement();

                            writer.WriteStartElement("juricPersonCustomerId");
                            writer.WriteValue(orderInfo.LegalPersonDgppId.Value);
                            writer.WriteEndElement();

                            writer.WriteStartElement("juricPersonOwnId");
                            writer.WriteValue(orderInfo.BranchOfficeOrganizationUnitDgppId);
                            writer.WriteEndElement();

                            // trim to seconds
                            var correctedCreatedOn = orderInfo.CreatedOn.TrimToSeconds();

                            writer.WriteStartElement("createDate");
                            writer.WriteValue(correctedCreatedOn);
                            writer.WriteEndElement();

                            writer.WriteStartElement("destSiteId");
                            writer.WriteValue(orderInfo.DestOrganizationUnitDgppId.Value);
                            writer.WriteEndElement();

                            writer.WriteStartElement("amount");
                            writer.WriteValue(orderInfo.AmountToWithdraw - (orderInfo.Vat ?? 0m));
                            writer.WriteEndElement();

                            writer.WriteEndElement();
                        }
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                stream.Position = 0;
                return new IntegrationResponse
                           {
                               Stream = stream,
                               ContentType = MediaTypeNames.Text.Xml,
                               FileName = "orders.xml",
                               ProcessedWithoutErrors = orderInfos.Length
                           };
            }
        }
    }
}