﻿using DoubleGis.Erm.BLCore.Aggregates.Firms;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Old;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Assign
{
    public class AssignFirmService : IAssignGenericEntityService<Firm>
    {
        private readonly IFirmRepository _firmRepository;
        private readonly IPublicService _publicService;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICommonLog _logger;

        public AssignFirmService(
            IFirmRepository firmRepository, 
            IPublicService publicService, 
            IOperationScopeFactory scopeFactory, 
            ICommonLog logger)
        {
            _firmRepository = firmRepository;
            _publicService = publicService;
            _scopeFactory = scopeFactory;
            _logger = logger;
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

            _logger.InfoFormatEx("Куратором фирмы с id={0} назначен пользователь {1}", entityId, ownerCode);

            return null;
        }
    }
}
