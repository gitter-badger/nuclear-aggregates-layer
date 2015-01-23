using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Exceptions;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Activate;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic.Activate
{
    public sealed class EmiratesActivateLegalPersonService : IActivateGenericEntityService<LegalPerson>, IEmiratesAdapted
    {
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;

        public EmiratesActivateLegalPersonService(
            ILegalPersonReadModel legalPersonReadModel,
            ILegalPersonRepository legalPersonRepository,
            IOperationScopeFactory scopeFactory,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IUserContext userContext)
        {
            _legalPersonRepository = legalPersonRepository;
            _scopeFactory = scopeFactory;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _legalPersonReadModel = legalPersonReadModel;
        }

        public int Activate(long entityId)
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.LegalPersonDeactivationOrActivation, _userContext.Identity.Code))
            {
                throw new OperationAccessDeniedException(ActivateIdentity.Instance);
            }

            using (var operationScope = _scopeFactory.CreateSpecificFor<ActivateIdentity, LegalPerson>())
            {
                var restoringLegalPerson = _legalPersonReadModel.GetLegalPerson(entityId);
                if (restoringLegalPerson.IsActive)
                {
                    throw new ActiveEntityActivationException(typeof(LegalPerson), restoringLegalPerson.LegalName);
                }

                var duplicateLegalPersonName = _legalPersonReadModel.GetActiveLegalPersonNameWithSpecifiedInn(restoringLegalPerson.Inn);

                if (duplicateLegalPersonName != null)
                {
                    throw new NotificationException(string.Format(BLResources.ActivateLegalPersonError, duplicateLegalPersonName));
                }

                var result = _legalPersonRepository.Activate(entityId);

                operationScope
                    .Updated<LegalPerson>(entityId)
                    .Complete();

                return result;
            }
        }
    }
}