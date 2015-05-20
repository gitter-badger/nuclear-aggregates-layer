using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Territories;
using DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeTerritory;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.ChangeTerritory
{
    public class ChangeFirmTerritoryService : IChangeGenericEntityTerritoryService<Firm>
    {
        private readonly IUserContext _userContext;
        private readonly IFirmRepository _firmRepository;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IPublicService _publicService;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public ChangeFirmTerritoryService(
            IUserContext userContext,
            IFirmRepository firmRepository, 
            ISecurityServiceFunctionalAccess functionalAccessService,
            IPublicService publicService,
            IOperationScopeFactory operationScopeFactory)
        {
            _userContext = userContext;
            _firmRepository = firmRepository;
            _functionalAccessService = functionalAccessService;
            _publicService = publicService;
            _operationScopeFactory = operationScopeFactory;
        }

        public void ChangeTerritory(long entityId, long territoryId)
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.ChangeFirmTerritory, _userContext.Identity.Code))
            {
                throw new SecurityException(BLResources.AccessDeniedChangeFirmTerritory);
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<ChangeTerritoryIdentity, Firm>())
            {
                // TODO: Refactor for UserRepository usage
                _publicService.Handle(new ValidateTerritoryAvailabilityRequest { TerritoryId = territoryId });

                var firm = _firmRepository.GetFirm(entityId);
                var changeAggregateTerritoryRepository = _firmRepository as IChangeAggregateTerritoryRepository<Firm>;
                changeAggregateTerritoryRepository.ChangeTerritory(entityId, territoryId);

                // fixme {a.rechkalov}: правильное OperationIdentity
                operationScope.Updated<Firm>(entityId);
                operationScope.Updated<Territory>(territoryId, firm.TerritoryId);
                operationScope.Complete();
            }
        }
    }
}