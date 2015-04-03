using System;
using System.Linq;
using System.Security;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeClient;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.ChangeClient
{
    public class ChangeDealClientService : IChangeGenericEntityClientService<Deal>
    {
        private readonly IUserContext _userContext;
        private readonly IDealReadModel _dealReadModel;
        private readonly IChangeAggregateClientRepository<Deal> _changeAggregateClientRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;

        public ChangeDealClientService(
            IUserContext userContext,
            IDealReadModel dealReadModel,
            IChangeAggregateClientRepository<Deal> changeAggregateClientRepository,
            IOperationScopeFactory scopeFactory,
            ISecurityServiceUserIdentifier userIdentifierService)
        {
            _userContext = userContext;
            _dealReadModel = dealReadModel;
            _changeAggregateClientRepository = changeAggregateClientRepository;
            _scopeFactory = scopeFactory;
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
                var validationResult = _changeAggregateClientRepository.Validate(entityId, _userContext.Identity.Code, reserveUserIdentity.Code);

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
            var reserveUserIdentity = _userIdentifierService.GetReserveUserIdentity();
            if (reserveUserIdentity == null)
            {
                throw new SecurityException(BLResources.ReserveIdentityIsNotFound);
            }

            var linkedLegalPersons = _dealReadModel.GetDealLegalPersonNames(entityId);
            if (linkedLegalPersons.Any())
            {
                var message = string.Format(BLResources.CannotChangeDealClientWithLinkedLegalPersons, string.Join(", ", linkedLegalPersons));
                throw new ChangeClientOfLinkedDealException(message);
            }

            var linkedFirms = _dealReadModel.GetDealFirmNames(entityId);
            if (linkedFirms.Any())
            {
                var message = string.Format(BLResources.CannotChangeDealClientWithLinkedFirms, string.Join(", ", linkedFirms));
                throw new ChangeClientOfLinkedDealException(message);
            }

            using (var operationScope = _scopeFactory.CreateSpecificFor<ChangeClientIdentity, Deal>())
            {
                var validationResult = _changeAggregateClientRepository.Validate(entityId, _userContext.Identity.Code, reserveUserIdentity.Code);

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

                var deal = _dealReadModel.GetDeal(entityId);
                _changeAggregateClientRepository.ChangeClient(entityId, clientId, _userContext.Identity.Code, bypassValidation);

                operationScope
                    .Updated<Deal>(entityId)
                    .Updated<Client>(clientId, deal.ClientId)
                    .Complete();
            }

            return null;
        }
    }
}