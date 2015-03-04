using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Releasing.Releases;
using DoubleGis.Erm.BLCore.API.Releasing.Releases.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Releasing.Release.Old
{
    public sealed class DownloadReleaseInfoResultsHandler : RequestHandler<DownloadReleaseInfoResultsRequest, StreamResponse>
    {
        private readonly ITracer _tracer;
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly IReleaseReadModel _releaseRepository;

        public DownloadReleaseInfoResultsHandler(ITracer tracer, ISubRequestProcessor subRequestProcessor, IReleaseReadModel releaseRepository)
        {
            _tracer = tracer;
            _subRequestProcessor = subRequestProcessor;
            _releaseRepository = releaseRepository;
        }

        protected override StreamResponse Handle(DownloadReleaseInfoResultsRequest request)
        {
            var releaseInfo = _releaseRepository.GetReleaseInfo(request.ReleaseInfoId);
            var validationResults = _releaseRepository.GetReleaseValidationResults(request.ReleaseInfoId);

            var results = validationResults as ReleaseProcessingMessage[] ?? validationResults.ToArray();
            if (!results.Any())
            {
                _tracer.WarnFormat(BLResources.ReleaseValidationResultsNotFound, request.ReleaseInfoId);
                return new StreamResponse();
            }

            return (StreamResponse)_subRequestProcessor.HandleSubRequest(
                new PrepareValidationReportRequest
                    {
                        OrganizationUnitId = releaseInfo.OrganizationUnitId,
                        Period = new TimePeriod(releaseInfo.PeriodStartDate, releaseInfo.PeriodEndDate),
                        ValidationResults = results
                    },
                Context);
        }
    }
}
