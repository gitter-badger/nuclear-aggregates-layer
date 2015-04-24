using System;
using System.Linq;
using System.Security;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeClient;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.ChangeClient
{
    public class ChangeLegalPersonClientService : IChangeGenericEntityClientService<LegalPerson>
    {
        private readonly IUserContext _userContext;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;

        public ChangeLegalPersonClientService(IUserContext userContext,
            IOperationScopeFactory scopeFactory, 
            ILegalPersonRepository legalPersonRepository,
            ISecurityServiceFunctionalAccess functionalAccessService,
            ISecurityServiceUserIdentifier userIdentifierService)
        {
            _userContext = userContext;
            _scopeFactory = scopeFactory;
            _legalPersonRepository = legalPersonRepository;
            _functionalAccessService = functionalAccessService;
            _userIdentifierService = userIdentifierService;
        }

        public ChangeEntityClientValidationResult Validate(long entityId, long clientId)
        {
            var reserveUserIdentity = _userIdentifierService.GetReserveUserIdentity();
            if (reserveUserIdentity == null)
            {
                throw new SecurityException(BLResources.ReserveIdentityIsNotFound);
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var changeAggregateClientRepository = _legalPersonRepository as IChangeAggregateClientRepository<LegalPerson>;
                var validationResult = changeAggregateClientRepository.Validate(entityId, _userContext.Identity.Code, reserveUserIdentity.Code);

                transaction.Complete();

                return new ChangeEntityClientValidationResult
                    {
                        Warnings = validationResult.SecurityErrors,
                        Errors = validationResult.DomainErrors.Union(validationResult.SecurityErrors)
                    };
            }
        }

        public ChangeEntityClientResult Execute(long entityId, long clientId, bool bypassValidation)
        {
            try
            {
                var reserveUserIdentity = _userIdentifierService.GetReserveUserIdentity();
                if (reserveUserIdentity == null)
                {
                    throw new SecurityException(BLResources.ReserveIdentityIsNotFound);
                }

                using (var operationScope = _scopeFactory.CreateSpecificFor<ChangeClientIdentity, LegalPerson>())
                {
                    var changeAggregateClientRepository = _legalPersonRepository as IChangeAggregateClientRepository<LegalPerson>;
                    var validationResult = changeAggregateClientRepository.Validate(entityId, _userContext.Identity.Code, reserveUserIdentity.Code);

                    var firstSecurityError = validationResult.SecurityErrors.FirstOrDefault();
                    if (firstSecurityError != null)
                    {
                        throw new SecurityException(firstSecurityError);
                    }

                    var firstDomainError = validationResult.DomainErrors.FirstOrDefault();
                    if (firstDomainError != null)
                    {
                        throw new ArgumentException(firstDomainError);
                    }

                    changeAggregateClientRepository.ChangeClient(entityId, clientId, _userContext.Identity.Code, bypassValidation);

                    operationScope
                        .Updated<LegalPerson>(entityId)
                        .Updated<Client>(clientId)
                        .Complete();
                }

                return new ChangeEntityClientResult();
            }
            catch (ProcessAccountsWithDebtsException ex)
            {
                var hasProcessAccountsWithDebtsPermissionGranted =
                    _functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.ProcessAccountsWithDebts, _userContext.Identity.Code);
                if (!hasProcessAccountsWithDebtsPermissionGranted)
                {
                    throw new SecurityException(string.Format("{0}. {1}", BLResources.OperationNotAllowed, ex.Message), ex);
                }

                return new ChangeEntityClientResult
                    {
                        EntityId = entityId,
                        CanProceed = true,
                        Message = ex.Message
                    };
            }
        }
    }
}