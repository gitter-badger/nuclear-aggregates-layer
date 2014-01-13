using System.Net.Mime;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Reports;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Reports;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Reports
{
    public sealed class LegalPersonPaymentsHandler : RequestHandler<LegalPersonPaymentsRequest, StreamResponse>
    {
        private readonly IPlanningReportPersistenceService _planningReportPersistenceService;

        public LegalPersonPaymentsHandler(IPlanningReportPersistenceService planningReportPersistenceService)
        {
            _planningReportPersistenceService = planningReportPersistenceService;
        }

        protected override StreamResponse Handle(LegalPersonPaymentsRequest request)
        {
            var stream = _planningReportPersistenceService.GetLegalPersonPaymentsStream(request.OrganizationUnitId,
                                                                                 request.ReportDate,
                                                                                 request.CurrentUser,
                                                                                 request.ConsiderOnRegistration,
                                                                                 request.ConsiderOnApproval);

            return new StreamResponse
                {
                    Stream = stream,
                    ContentType = MediaTypeNames.Application.Octet,
                    FileName = "LegalPersonPayments.xlsx"
                };
        }
    }
}
