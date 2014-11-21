using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Disqualify;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Firms;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Disqualify
{
    [Obsolete("На текущий момент в UI нет кнопки вызова возврата фирмы в резерв. Необходимо уточнить и удалить в случае ненадобности")]
    public class DisqualifyFirmService : IDisqualifyGenericEntityService<Firm>
    {
        private readonly IUserContext _userContext;
        private readonly IFirmRepository _firmRepository;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IPublicService _publicService;
        private readonly ICommonLog _logger;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public DisqualifyFirmService(
            IUserContext userContext,
            IFirmRepository firmRepository,
            ISecurityServiceUserIdentifier userIdentifierService,
            IPublicService publicService, 
            ICommonLog logger,
            IOperationScopeFactory operationScopeFactory)
        {
            _userContext = userContext;
            _firmRepository = firmRepository;
            _userIdentifierService = userIdentifierService;
            _publicService = publicService;
            _logger = logger;
            _operationScopeFactory = operationScopeFactory;
        }

        public DisqualifyResult Disqualify(long entityId, bool bypassValidation)
        {
            _publicService.Handle(new CheckMsCrmFirmActivitiesRequest { Id = entityId });

            var reserveUser = _userIdentifierService.GetReserveUserIdentity();
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DisqualifyIdentity, Firm>())
            {
                var disqualifyAggregateRepository = _firmRepository as IDisqualifyAggregateRepository<Firm>;
                disqualifyAggregateRepository.Disqualify(entityId, _userContext.Identity.Code, reserveUser.Code, false, DateTime.UtcNow);

                operationScope.Updated<Firm>(entityId);
                operationScope.Complete();
            }

            _logger.InfoFormatEx("Фирма с id={0} возвращена в резерв", entityId);

            return null;
        }
    }
}