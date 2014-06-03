using System.Linq;
using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Caching;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Deactivate
{
    public sealed class DeactivateLegalPersonService : IDeactivateGenericEntityService<LegalPerson>
    {
        private readonly IUserContext _userContext;
        private readonly IFinder _finder;
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly ICacheAdapter _cacheAdapter;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IOperationScopeFactory _scopeFactory;

        public DeactivateLegalPersonService(
            IUserContext userContext,
            IFinder finder,
            ILegalPersonRepository legalPersonRepository,
            ICacheAdapter cacheAdapter,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IOperationScopeFactory scopeFactory)
        {
            _userContext = userContext;
            _finder = finder;
            _legalPersonRepository = legalPersonRepository;
            _cacheAdapter = cacheAdapter;
            _functionalAccessService = functionalAccessService;
            _scopeFactory = scopeFactory;
        }

        public DeactivateConfirmation Deactivate(long entityId, long ownerCode)
        {
            var currentUserIdentity = _userContext.Identity;

            // COMMENT {all, 06.08.2013}: вопрос что быдет с такими сессиями при отключении привязки сессии к node 
            // COMMENT {all, 21.05.2014}: Нет никакой привязки сессии к ноде, есть *локальное* хранилище сессий в памями каждой веб-ноды. Этот вопрос решается распределенным кэшем на основе AppFabric
            var deactivateSession = OperationSession.GetSession(_cacheAdapter, BusinessOperation.Deactivate, entityId, currentUserIdentity.Code);

            try
            {
                using (var operationScope = _scopeFactory.CreateSpecificFor<DeactivateIdentity, LegalPerson>())
                {
                    var legalPerson = _finder.FindOne(Specs.Find.ById<LegalPerson>(entityId));
                    var findResult = _finder.Find(Specs.Find.ById<LegalPerson>(entityId))
                        .Select(x => new
                            {
                                IsLinkedWithActiveOrders = x.Orders
                                         .Any(y => y.IsActive && !y.IsDeleted &&
                                                   y.WorkflowStepId != (int)OrderState.Archive && y.WorkflowStepId != (int)OrderState.Rejected),

                                IsLinkedWithArchivedOrRejectedOrders = x.Orders
                                         .Any(y => y.IsActive && !y.IsDeleted &&
                                                   (y.WorkflowStepId == (int)OrderState.Archive || y.WorkflowStepId == (int)OrderState.Rejected)),

                                HasAccounts = x.Accounts.Any(a => !a.IsDeleted && a.IsActive)
                            })
                        .Single();

                    if (findResult.IsLinkedWithActiveOrders)
                    {
                        throw new NotificationException(BLResources.CantDeativateObjectLinkedWithActiveOrders);
                    }

                    if (!legalPerson.IsActive)
                    {
                        throw new NotificationException(BLResources.LegalPersonIsInactiveAlready);
                    }

                    if (legalPerson.IsInSyncWith1C)
                    {
                        throw new NotificationException(BLResources.CantDeactivateLegalPersonThatHasBeenIntegratedWithAccountingSystem);
                    }

                    if (findResult.HasAccounts)
                    {
                        throw new NotificationException(BLResources.CannotDeactivateLegalPersonWithLinkedAccounts);
                    }

                    var checkAggregateForDebtsRepository = _legalPersonRepository as ICheckAggregateForDebtsRepository<LegalPerson>;
                    checkAggregateForDebtsRepository.CheckForDebts(entityId, _userContext.Identity.Code, deactivateSession.CanProceedWithAccountDebts);

                    if (!deactivateSession.CanProceedWithArchiveOrClosedOrders && findResult.IsLinkedWithArchivedOrRejectedOrders)
                    {
                        var message = string.Format(BLResources.DeactivateLegalPersonConfirmation, legalPerson.LegalName);

                        deactivateSession.CanProceedWithArchiveOrClosedOrders = true;
                        return new DeactivateConfirmation {ConfirmationMessage = message, Id = entityId};
                    }

                    _legalPersonRepository.Deactivate(legalPerson);


                    operationScope
                        .Updated<LegalPerson>(legalPerson.Id)
                        .Complete();
                }
            }
            catch (ProcessAccountsWithDebtsException ex)
            {
                var hasProcessAccountsWithDebtsPermissionGranted =
                    _functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.ProcessAccountsWithDebts, _userContext.Identity.Code);
                if (!hasProcessAccountsWithDebtsPermissionGranted)
                {
                    throw new SecurityException(string.Format("{0}. {1}", BLResources.OperationNotAllowed, ex.Message), ex);
                }

                deactivateSession.CanProceedWithAccountDebts = true;
                return new DeactivateConfirmation {ConfirmationMessage = string.Format(BLResources.ConfirmationMessageFormatString, ex.Message), Id = entityId};
                // нелокализовано намеренно, т.к. нужно прийти к единой реализации этих операций (например, смены куратора и деактивации/удаления)
            }

            deactivateSession.Close(_cacheAdapter);

            return null;
        }
    }
}
