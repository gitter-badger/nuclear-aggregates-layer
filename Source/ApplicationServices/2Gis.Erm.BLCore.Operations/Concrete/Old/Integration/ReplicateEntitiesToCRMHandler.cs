using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration
{
    public sealed class ReplicateEntitiesToCrmHandler : RequestHandler<ReplicateEntitiesToCrmRequest, EmptyResponse>
    {
        private readonly IReplicationPersistenceService _replicationPersistenceService;

        public ReplicateEntitiesToCrmHandler(IReplicationPersistenceService replicationPersistenceService)
        {
            _replicationPersistenceService = replicationPersistenceService;
        }

        protected override EmptyResponse Handle(ReplicateEntitiesToCrmRequest request)
        {
            _replicationPersistenceService.ReplicateEntitiesToMscrm(request.ChunkSize, request.Timeout);

            return Response.Empty;
        }
    }
}
