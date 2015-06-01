using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.HotClientRequest;

using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Clients.Operations
{
    public class BindTaskToHotClientRequestAggregateService : IBindTaskToHotClientRequestAggregateService
    {
        private readonly IRepository<HotClientRequest> _hotClientRequestRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public BindTaskToHotClientRequestAggregateService(
            IRepository<HotClientRequest> hotClientRequestRepository,
            IOperationScopeFactory scopeFactory)
        {
            _hotClientRequestRepository = hotClientRequestRepository;
            _scopeFactory = scopeFactory;
        }

        public void BindTask(HotClientRequest hotClientRequest, long taskId)
        {
            if (hotClientRequest == null)
            {
                throw new InvalidOperationException("The hot client request does not exist for the specified ID.");
            }

            using (var operationScope = _scopeFactory.CreateNonCoupled<BindTaskToHotClientRequestIdentity>())
            {
                hotClientRequest.TaskId = taskId;
                _hotClientRequestRepository.Update(hotClientRequest);
                _hotClientRequestRepository.Save();

                operationScope.Updated(hotClientRequest)
                              .Complete();
            }
        }
    }
}