using System.Net.Mime;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Reports;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Reports;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Reports
{
    public sealed class PlanningReportHandler : RequestHandler<PlanningReportRequest, PlanningReportResponse>
    {
        private readonly IPlanningReportPersistenceService _planningReportPersistenceService;

        public PlanningReportHandler(IPlanningReportPersistenceService planningReportPersistenceService)
        {
            _planningReportPersistenceService = planningReportPersistenceService;
        }

        protected override PlanningReportResponse Handle(PlanningReportRequest request)
        {
            var stream = _planningReportPersistenceService.GetPlanningReportStream(request.OrganizationUnitId, request.PlanningMonth, request.IsAdvertisingAgency);

            return new PlanningReportResponse { OutputStream = stream, ContentType = MediaTypeNames.Application.Octet };
        }
    }
}
