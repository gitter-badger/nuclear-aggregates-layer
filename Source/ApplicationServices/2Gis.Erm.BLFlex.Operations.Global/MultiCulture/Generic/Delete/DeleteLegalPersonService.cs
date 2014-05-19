using System.Linq;
using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Dynamic.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.BLCore.Operations.Generic;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Caching;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Delete
{
    public sealed class DeleteLegalPersonService : IDeleteGenericEntityService<LegalPerson>
    {
        private readonly IUserContext _userContext;
        private readonly IFinder _finder;
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly ILegalPersonReadModel _readModel;
        private readonly IDeletePartableEntityAggregateService<LegalPerson, LegalPerson> _deleteLegalPersonSerice;
        private readonly ICacheAdapter _cacheAdapter;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public DeleteLegalPersonService(
            IUserContext userContext,
            IFinder finder,
            ILegalPersonRepository legalPersonRepository,
            ILegalPersonReadModel readModel,
            IDeletePartableEntityAggregateService<LegalPerson, LegalPerson> deleteLegalPersonSerice,
            ICacheAdapter cacheAdapter,
            ISecurityServiceEntityAccess entityAccessService,
            ISecurityServiceFunctionalAccess functionalAccessService, 
            IOperationScopeFactory operationScopeFactory)
        {
            _userContext = userContext;
            _finder = finder;
            _legalPersonRepository = legalPersonRepository;
            _readModel = readModel;
            _deleteLegalPersonSerice = deleteLegalPersonSerice;
            _cacheAdapter = cacheAdapter;
            _entityAccessService = entityAccessService;
            _functionalAccessService = functionalAccessService;
            _operationScopeFactory = operationScopeFactory;
           
        }

        public DeleteConfirmation Delete(long entityId)
        {
            var currentIdentity = _userContext.Identity;
            var deleteSession = OperationSession.GetSession(_cacheAdapter, BusinessOperation.Delete, entityId, currentIdentity.Code);
            try
            {
                using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, LegalPerson>())
                {
                    // TODO {v.lapeev, 20.02.2014}: перевести всю валидацию на ReadModel
                    var findResult = _finder.Find(Specs.Find.ById<LegalPerson>(entityId))
                        .Select(x => new
                            {
                                LegalPerson = x,
                                IsLinkedWithActiveOrders = x.Orders.Any(y => y.IsActive && !y.IsDeleted),
                                HasAccounts = x.Accounts.Any(a => !a.IsDeleted && a.IsActive)
                            })
                        .Single();

                    if (findResult.IsLinkedWithActiveOrders)
                    {
                        throw new NotificationException(BLResources.CantDeleteObjectLinkedWithActiveOrders);
                    }
                    if (findResult.LegalPerson.IsDeleted)
                    {
                        throw new NotificationException(BLResources.LegalPersonIsDeletedAlready);
                    }
                    if (findResult.LegalPerson.IsInSyncWith1C)
                    {
                        throw new NotificationException(BLResources.CantDeleteLegalPersonWhenItIsSyncedWith1C);
                    }
                    if (findResult.HasAccounts)
                    {
                        throw new NotificationException(BLResources.CannotDeleteLegalPersonWithLinkedAccounts);
                    }

                    var checkAggregateForDebtsRepository = _legalPersonRepository as ICheckAggregateForDebtsRepository<LegalPerson>;
                    checkAggregateForDebtsRepository.CheckForDebts(entityId, _userContext.Identity.Code, deleteSession.CanProceedWithAccountDebts);

                    var legalPerson = _readModel.GetLegalPerson(entityId);

                    var dtos = _readModel.GetBusinessEntityInstanceDto(legalPerson);

                    _deleteLegalPersonSerice.Delete(findResult.LegalPerson, dtos);

                    operationScope.Updated<LegalPerson>(entityId);
                    operationScope.Complete();
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

                deleteSession.CanProceedWithAccountDebts = true;
                return new DeleteConfirmation { ConfirmationMessage = string.Format(BLResources.ConfirmationMessageFormatString, ex.Message) };
                // нелокализовано намеренно, т.к. нужно прийти к единой реализации этих операций (например, смены куратора и деактивации/удаления)
            }
            
            deleteSession.Close(_cacheAdapter);
            return null;
        }

        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            var findResult = _finder.Find(Specs.Find.ById<LegalPerson>(entityId))
                .Select(x => new
                    {
                        LegalPerson = x,
                        IsLinkedWithActiveOrders = x.Orders.Any(y => x.IsActive && !y.IsDeleted)
                    })
                .SingleOrDefault();

            if (findResult == null)
            {
                return new DeleteConfirmationInfo
                    {
                        IsDeleteAllowed = false,
                        DeleteDisallowedReason = BLResources.EntityNotFound
                    };
            }

            var legalPerson = findResult.LegalPerson;
            var entityPrivileges = _entityAccessService.RestrictEntityAccess(EntityName.LegalPerson,
                                                                             EntityAccessTypes.All,
                                                                             _userContext.Identity.Code,
                                                                             legalPerson.Id,
                                                                             legalPerson.OwnerCode,
                                                                             null);

            // на текущий момент интересует только возможность удалить
            var isDeleteAllowed = entityPrivileges.HasFlag(EntityAccessTypes.Delete);
            if (!isDeleteAllowed)
            {
                return new DeleteConfirmationInfo
                    {
                        EntityCode = findResult.LegalPerson.LegalName,
                        IsDeleteAllowed = false,
                        DeleteDisallowedReason = BLResources.EntityAccessDenied
                    };
            }

            if (findResult.IsLinkedWithActiveOrders)
            {
                return new DeleteConfirmationInfo
                    {
                        EntityCode = findResult.LegalPerson.LegalName,
                        IsDeleteAllowed = false,
                        DeleteDisallowedReason = BLResources.CantDeleteObjectLinkedWithActiveOrders
                    };
            }
            // уже удален
            if (findResult.LegalPerson.IsDeleted)
            {
                return new DeleteConfirmationInfo
                    {
                        EntityCode = findResult.LegalPerson.LegalName,
                        IsDeleteAllowed = false,
                        DeleteDisallowedReason = BLResources.OrderIsAlreadyDeleted
                    };
            }
            if (findResult.LegalPerson.IsInSyncWith1C)
            {
                return new DeleteConfirmationInfo
                    {
                        EntityCode = findResult.LegalPerson.LegalName,
                        IsDeleteAllowed = false,
                        DeleteDisallowedReason = BLResources.CantDeleteLegalPersonWhenItIsSyncedWith1C
                    };
            }
            return new DeleteConfirmationInfo
                {
                    EntityCode = findResult.LegalPerson.ShortName, 
                    IsDeleteAllowed = true, 
                    DeleteConfirmation = string.Empty
                };
        }
    }
}
