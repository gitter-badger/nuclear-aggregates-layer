using System;
using System.Linq;
using System.Security;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms;
using DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeClient;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.ChangeClient
{
    public class ChangeFirmClientService : IChangeGenericEntityClientService<Firm>
    {
        private readonly IUserContext _userContext;
        private readonly IFirmRepository _firmRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IClientRepository _clientRepository;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;

        public ChangeFirmClientService(
            IUserContext userContext,
            IFirmRepository firmRepository,
            IOperationScopeFactory scopeFactory, 
            IClientRepository clientRepository,
            ISecurityServiceUserIdentifier userIdentifierService)
        {
            _userContext = userContext;
            _firmRepository = firmRepository;
            _scopeFactory = scopeFactory;
            _clientRepository = clientRepository;
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
                var changeAggregateClientRepository = _firmRepository as IChangeAggregateClientRepository<Firm>;
                var validationResult = changeAggregateClientRepository.Validate(entityId, _userContext.Identity.Code, reserveUserIdentity.Code);

                transaction.Complete();

                return new ChangeEntityClientValidationResult
                    {
                        Warnings = validationResult.SecurityErrors,
                        Errors = validationResult.DomainErrors.Union(validationResult.SecurityErrors)
                    };
            }
        }

        public ChangeEntityClientResult Execute(long firmId, long clientId, bool bypassValidation)
        {
            var reserveUserIdentity = _userIdentifierService.GetReserveUserIdentity();
            if (reserveUserIdentity == null)
            {
                throw new SecurityException(BLResources.ReserveIdentityIsNotFound);
            }

            using (var operationScope = _scopeFactory.CreateSpecificFor<ChangeClientIdentity, Firm>())
            {
                var changeAggregateClientRepository = _firmRepository as IChangeAggregateClientRepository<Firm>;
                var validationResult = changeAggregateClientRepository.Validate(firmId, _userContext.Identity.Code, reserveUserIdentity.Code);

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

                var oldClient = _firmRepository.GetFirmClient(firmId);
                if (oldClient != null)
                {
                    if (oldClient.MainFirmId == firmId)
                    {
                        _clientRepository.SetMainFirm(oldClient, null);
                    }

                    // Даже если фирма и не была основной - список фирм у старого клиента сменился
                    operationScope.Updated<Client>(oldClient.Id);
                }

                changeAggregateClientRepository.ChangeClient(firmId, clientId, _userContext.Identity.Code, bypassValidation);

                operationScope
                    .Updated<Firm>(firmId)
                    .Updated<Client>(clientId)
                    .Complete();
            }

            return null;
        }
    }
}