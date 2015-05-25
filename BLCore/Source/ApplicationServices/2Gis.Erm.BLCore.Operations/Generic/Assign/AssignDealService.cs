using DoubleGis.Erm.BLCore.API.Aggregates.Deals;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Old;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Assign
{
    public class AssignDealService : IAssignGenericEntityService<Deal>
    {
        private readonly IDealRepository _dealRepository;
        private readonly IPublicService _publicService;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ITracer _tracer;

        public AssignDealService(
            IDealRepository dealRepository, 
            IPublicService publicService, 
            IOperationScopeFactory scopeFactory, 
            ITracer tracer)
        {
            _dealRepository = dealRepository;
            _publicService = publicService;
            _scopeFactory = scopeFactory;
            _tracer = tracer;
        }

        public virtual AssignResult Assign(long entityId, long ownerCode, bool bypassValidation, bool isPartialAssign)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<AssignIdentity, Deal>())
            {
                _publicService.Handle(new ValidateOwnerIsNotReserveRequest<Deal> { Id = entityId });

                _dealRepository.Assign(entityId, ownerCode); 

                operationScope
                    .Updated<Deal>(entityId)
                    .Complete();
            }
            
            _tracer.InfoFormat("Куратором сделки с id={0} назначен пользователь {1}", entityId, ownerCode);

            return null;
        }
    }
}
