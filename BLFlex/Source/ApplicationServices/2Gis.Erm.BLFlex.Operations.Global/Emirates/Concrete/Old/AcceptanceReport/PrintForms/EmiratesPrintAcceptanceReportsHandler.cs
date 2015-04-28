using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;

using DoubleGis.Erm.BLCore.API.Aggregates.OrganizationUnits.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLFlex.API.Aggregates.Global.Emirates.Orders.ReadModel;
using DoubleGis.Erm.BLFlex.API.Aggregates.Global.Emirates.SimplifiedModel;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Concrete.Old.AcceptanceReport;
using DoubleGis.Erm.BLFlex.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Common.Compression;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Concrete.Old.AcceptanceReport.PrintForms
{
    public class EmiratesPrintAcceptanceReportsHandler : RequestHandler<EmiratesPrintAcceptanceReportsRequest, Response>, IEmiratesAdapted
    {
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly IUserContext _userContext;
        private readonly IOrganizationUnitReadModel _organizationUnitReadModel;
        private readonly IEmiratesOrderReadModel _orderReadModel;
        private readonly ICreateAcceptanceReportsJournalRecordService _acceptanceReportsJournalRecordService;

        public EmiratesPrintAcceptanceReportsHandler(ISubRequestProcessor requestProcessor,
                                                     IUserContext userContext,
                                                     IOrganizationUnitReadModel organizationUnitReadModel,
                                                     IEmiratesOrderReadModel orderReadModel,
                                                     ICreateAcceptanceReportsJournalRecordService acceptanceReportsJournalRecordService)
        {
            _requestProcessor = requestProcessor;
            _userContext = userContext;
            _organizationUnitReadModel = organizationUnitReadModel;
            _orderReadModel = orderReadModel;
            _acceptanceReportsJournalRecordService = acceptanceReportsJournalRecordService;
        }

        protected override Response Handle(EmiratesPrintAcceptanceReportsRequest request)
        {
            var orderDtos = _orderReadModel.GetOrdersToGenerateAcceptanceReports(request.Month, request.OrganizationUnitId).ToArray();

            if (!orderDtos.Any())
            {
                throw new ThereIsNoOrdersToPrintException(BLResources.NoOrdersFound);
            }

            var ordersWithoutProfile = orderDtos.Where(x => !x.ProfileId.HasValue).ToArray();

            if (ordersWithoutProfile.Any())
            {
                throw new SomeOrdersToPrintAcceptanceReportDontHaveSpecifiedProfileException(
                    string.Format(BLResources.EmiratesCannotPrintAcceptanceReportSinceProfileIsNotSpecifiedForSomeOrders,
                                  string.Join(", ", ordersWithoutProfile.Select(x => x.OrderNumber))));
            }

            var printRequests = orderDtos.Select(x =>
                                                 new EmiratesPrintAcceptanceReportRequest
                                                     {
                                                         OrderId = x.OrderId
                                                     })
                                         .ToArray();

            var organizationUnitName = _organizationUnitReadModel.GetName(request.OrganizationUnitId);
            var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(request.Month.Month);

            var printResult = new StreamResponse
            {
                Stream = ProcessRequests(printRequests).ZipStreamDictionary(),
                ContentType = MediaTypeNames.Application.Zip,
                FileName = string.Format("Acceptance Report_{0}_{1}.zip", organizationUnitName, monthName),
            };

            _acceptanceReportsJournalRecordService.Create(new AcceptanceReportsJournalRecord
                {
                    AuthorId = _userContext.Identity.Code,
                    DocumentsAmount = orderDtos.Count(),
                    OrganizationUnitId = request.OrganizationUnitId,
                    EndDistributionDate = request.Month,
                    IsActive = true
                });

            return printResult;
        }

        private Dictionary<string, Stream> ProcessRequests(params EmiratesPrintAcceptanceReportRequest[] requests)
        {
            return requests
                .Select(request => (StreamResponse)_requestProcessor.HandleSubRequest(request, Context))
                .ToDictionary(response => response.FileName, response => response.Stream);
        }
    }
}