using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Old;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Assign
{
    public class AssignContactService : IAssignGenericEntityService<Contact>
    {
        private readonly ICommonLog _logger;
        private readonly IAssignAggregateRepository<Contact> _assignContactRepository;
        private readonly IPublicService _publicService;
        private readonly IOperationScopeFactory _scopeFactory;

        public AssignContactService(
            ICommonLog logger, 
            IAssignAggregateRepository<Contact> assignContactRepository, 
            IPublicService publicService, 
            IOperationScopeFactory scopeFactory)
        {
            _logger = logger;
            _assignContactRepository = assignContactRepository;
            _publicService = publicService;
            _scopeFactory = scopeFactory;
        }

        public virtual AssignResult Assign(long entityId, long ownerCode, bool bypassValidation, bool isPartialAssign)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<AssignIdentity, Contact>())
            {
                _publicService.Handle(new ValidateOwnerIsNotReserveRequest<Contact> { Id = entityId });

                _assignContactRepository.Assign(entityId, ownerCode);

                operationScope
                    .Updated<Contact>(entityId)
                    .Complete();
            }

            _logger.InfoFormat("Куратором фирмы с id={0} назначен пользователь {1}", entityId, ownerCode);

            return null;
        }
    }
}
