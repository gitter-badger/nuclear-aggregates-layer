using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Exceptions;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Deactivate
{
    public sealed class DeactivateLegalPersonService : IDeactivateGenericEntityService<LegalPerson>
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IDeactivateLegalPersonAggregateService _deactivateLegalPersonAggregateService;

        public DeactivateLegalPersonService(
            IUserContext userContext,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IOperationScopeFactory scopeFactory,
            ILegalPersonReadModel legalPersonReadModel,
            IDeactivateLegalPersonAggregateService deactivateLegalPersonAggregateService)
        {
            _userContext = userContext;
            _functionalAccessService = functionalAccessService;
            _scopeFactory = scopeFactory;
            _legalPersonReadModel = legalPersonReadModel;
            _deactivateLegalPersonAggregateService = deactivateLegalPersonAggregateService;
        }

        public DeactivateConfirmation Deactivate(long entityId, long ownerCode)
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.LegalPersonDeactivationOrActivation, _userContext.Identity.Code))
            {
                throw new OperationAccessDeniedException(DeactivateIdentity.Instance);
            }

                using (var operationScope = _scopeFactory.CreateSpecificFor<DeactivateIdentity, LegalPerson>())
                {
                var legalPerson = _legalPersonReadModel.GetLegalPerson(entityId);
                    if (!legalPerson.IsActive)
                    {
                    throw new InactiveEntityDeactivationException(typeof(LegalPerson), legalPerson.LegalName);
                    }

                if (_legalPersonReadModel.DoesLegalPersonHaveActiveNotArchivedAndNotRejectedOrders(entityId))
                    {
                    throw new LegalPersonWithOrdersDeactivationException(BLResources.CantDeativateObjectLinkedWithActiveOrders);
                    }

                var profiles = _legalPersonReadModel.GetProfilesByLegalPerson(entityId);

                _deactivateLegalPersonAggregateService.Deactivate(legalPerson, profiles);

                    operationScope
                    .Updated(legalPerson)
                        .Complete();
                }

            return null;
        }
    }
}
