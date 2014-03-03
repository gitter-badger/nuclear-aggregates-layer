using System.Net.Mime;

using DoubleGis.Erm.BL.API.Operations.Special.Concrete.Old.Reports;
using DoubleGis.Erm.BL.Reports;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;

namespace DoubleGis.Erm.BL.Operations.Special.Concrete.Old.Reports
{
    // FIXME {v.lapeev, 24.01.2014}: Похоже этот хендлер нигде не вызывается, учтонить и удалить если так
    public sealed class LegalPersonPaymentsHandler : RequestHandler<LegalPersonPaymentsRequest, StreamResponse>
    {
        private readonly IReportsSqlConnectionWrapper _reportsSqlConnectionWrapper;

        public LegalPersonPaymentsHandler(IReportsSqlConnectionWrapper reportsSqlConnectionWrapper)
        {
            _reportsSqlConnectionWrapper = reportsSqlConnectionWrapper;
        }

        protected override StreamResponse Handle(LegalPersonPaymentsRequest request)
        {
            var stream = _reportsSqlConnectionWrapper.GetLegalPersonPaymentsStream(request.OrganizationUnitId,
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
