using DoubleGis.Erm.BLCore.API.Aggregates.Firms;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Old;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Assign
{
    public class AssignFirmService : IAssignGenericEntityService<Firm>
    {
        private readonly IFirmRepository _firmRepository;
        private readonly IPublicService _publicService;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ITracer _tracer;

        public AssignFirmService(
            IFirmRepository firmRepository, 
            IPublicService publicService, 
            IOperationScopeFactory scopeFactory, 
            ITracer tracer)
        {
            _firmRepository = firmRepository;
            _publicService = publicService;
            _scopeFactory = scopeFactory;
            _tracer = tracer;
        }

        public virtual AssignResult Assign(long entityId, long ownerCode, bool bypassValidation, bool isPartialAssign)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<AssignIdentity, Firm>())
            {
                _publicService.Handle(new ValidateOwnerIsNotReserveRequest<Firm> { Id = entityId });

                _firmRepository.Assign(entityId, ownerCode);

                operationScope
                    .Updated<Firm>(entityId)
                    .Complete();
            }

            _tracer.InfoFormat("Куратором фирмы с id={0} назначен пользователь {1}", entityId, ownerCode);

            return null;
        }
    }
}
