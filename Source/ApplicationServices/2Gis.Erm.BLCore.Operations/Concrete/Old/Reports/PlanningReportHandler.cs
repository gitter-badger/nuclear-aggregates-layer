using System.Net.Mime;

using DoubleGis.Erm.BL.Reports;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Reports;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Reports
{
    public sealed class PlanningReportHandler : RequestHandler<PlanningReportRequest, PlanningReportResponse>
    {
        private readonly IReportsSqlConnectionWrapper _reportsSqlConnectionWrapper;

        public PlanningReportHandler(IReportsSqlConnectionWrapper reportsSqlConnectionWrapper)
        {
            _reportsSqlConnectionWrapper = reportsSqlConnectionWrapper;
        }

        protected override PlanningReportResponse Handle(PlanningReportRequest request)
        {
            var stream = _reportsSqlConnectionWrapper.GetPlanningReportStream(request.OrganizationUnitId, request.PlanningMonth, request.IsAdvertisingAgency);

            return new PlanningReportResponse { OutputStream = stream, ContentType = MediaTypeNames.Application.Octet };
        }
    }
}
