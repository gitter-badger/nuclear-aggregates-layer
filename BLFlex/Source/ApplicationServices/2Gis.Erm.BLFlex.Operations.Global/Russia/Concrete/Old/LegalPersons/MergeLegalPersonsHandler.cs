using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.API.Aggregates.Global.Russia.LegalPersons.ReadModel;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Old.LegalPersons
{
    public sealed class MergeLegalPersonsHandler : RequestHandler<MergeLegalPersonsRequest, EmptyResponse>, IRussiaAdapted
    {
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IUpdateAggregateRepository<LegalPersonProfile> _updateProfileRepository; 
        private readonly IOrderRepository _orderRepository;
        private readonly IAccountRepository _accountRepository;        
        private readonly IRussiaLegalPersonReadModel _legalPersonReadModel;
        private readonly IUpdateAggregateRepository<Bargain> _updateBargainAggregateService;
        private readonly IDeleteLegalPersonAggregateService _deleteLegalPersonAggregateService;

        public MergeLegalPersonsHandler(
            ISecurityServiceFunctionalAccess functionalAccessService,
            IOrderRepository orderRepository,
            IAccountRepository accountRepository,
            IUserContext userContext,
            IOperationScopeFactory scopeFactory,
            IUpdateAggregateRepository<LegalPersonProfile> updateProfileRepository,
            IRussiaLegalPersonReadModel legalPersonReadModel,
            IUpdateAggregateRepository<Bargain> updateBargainAggregateService,
            IDeleteLegalPersonAggregateService deleteLegalPersonAggregateService)
        {
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _scopeFactory = scopeFactory;
            _updateProfileRepository = updateProfileRepository;
            _legalPersonReadModel = legalPersonReadModel;
            _updateBargainAggregateService = updateBargainAggregateService;
            _deleteLegalPersonAggregateService = deleteLegalPersonAggregateService;
            _orderRepository = orderRepository;
            _accountRepository = accountRepository;
        }

        protected override EmptyResponse Handle(MergeLegalPersonsRequest request)
        {
            #region Проверки

            if (request.AppendedLegalPersonId == request.MainLegalPersonId)
            {
                throw new NotificationException(BLResources.MergeLegalPersonsSameIdError);
            }

            // Проверка функционального разрешения
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.MergeLegalPersons, _userContext.Identity.Code))
            {
                throw new NotificationException(BLResources.MergeLegalPersonsAccessDeniedError);
            }

            var mainLegalPerson = _legalPersonReadModel.GetInfoForMerge(request.MainLegalPersonId);
            var appendedLegalPerson = _legalPersonReadModel.GetInfoForMerge(request.AppendedLegalPersonId);

            if (mainLegalPerson == null)
            {
                throw new NotificationException(BLResources.MergeLegalPersonsMainNotFoundError);
            }

            if (appendedLegalPerson == null)
            {
                throw new NotificationException(BLResources.MergeLegalPersonsAppendedNotFoundError);
            }

            // У юр. лиц не должно быть более 1 основного профиля
            if (mainLegalPerson.Profiles.Count(x => x.IsMainProfile && x.IsActive && !x.IsDeleted) > 1)
            {
                throw new NotificationException(string.Format(BLResources.LegalPersonHasSeveralMainProfiles, mainLegalPerson.LegalPerson.LegalName));
            }

            if (appendedLegalPerson.Profiles.Count(x => x.IsMainProfile && x.IsActive && !x.IsDeleted) > 1)
            {
                throw new NotificationException(string.Format(BLResources.LegalPersonHasSeveralMainProfiles, appendedLegalPerson.LegalPerson.LegalName));
            }

            // Проверим, указаны ли ИНН,КПП, серия, номер паспорта в зависимости от типа юр. лица
            // Для юр. лица должны быть указаны ИНН и КПП
            if (mainLegalPerson.LegalPerson.LegalPersonTypeEnum == LegalPersonType.LegalPerson &&
                (string.IsNullOrWhiteSpace(mainLegalPerson.LegalPerson.Inn) ||
                 string.IsNullOrWhiteSpace(mainLegalPerson.LegalPerson.Kpp)))
            {
                throw new NotificationException(BLResources.MergeLegalPersonsEmptyAttributeError);
            }

            // Для ИП должен быть указан ИНН
            if (mainLegalPerson.LegalPerson.LegalPersonTypeEnum == LegalPersonType.Businessman &&
                string.IsNullOrWhiteSpace(mainLegalPerson.LegalPerson.Inn))
            {
                throw new NotificationException(BLResources.MergeLegalPersonsEmptyAttributeError);
            }

            // для физ. лица должны быть указаны серия и номер паспорта
            if (mainLegalPerson.LegalPerson.LegalPersonTypeEnum == LegalPersonType.NaturalPerson &&
                (string.IsNullOrWhiteSpace(mainLegalPerson.LegalPerson.PassportSeries) ||
                 string.IsNullOrWhiteSpace(mainLegalPerson.LegalPerson.PassportNumber)))
            {
                throw new NotificationException(BLResources.MergeLegalPersonsEmptyAttributeError);
            }

            if (mainLegalPerson.LegalPerson.LegalPersonTypeEnum != appendedLegalPerson.LegalPerson.LegalPersonTypeEnum)
            {
                throw new NotificationException(BLResources.MergeLegalPersonsDifferentTypesError);
            }

            if (mainLegalPerson.LegalPerson.LegalPersonTypeEnum == LegalPersonType.LegalPerson &&
                (mainLegalPerson.LegalPerson.Inn != appendedLegalPerson.LegalPerson.Inn ||
                mainLegalPerson.LegalPerson.Kpp != appendedLegalPerson.LegalPerson.Kpp))
            {
                throw new NotificationException(BLResources.MergeLegalPersonsDifferentKPPINNError);
            }

            if (mainLegalPerson.LegalPerson.LegalPersonTypeEnum == LegalPersonType.Businessman &&
                mainLegalPerson.LegalPerson.Inn != appendedLegalPerson.LegalPerson.Inn)
            {
                throw new NotificationException(BLResources.MergeLegalPersonsDifferentINNError);
            }

            if (mainLegalPerson.LegalPerson.LegalPersonTypeEnum == LegalPersonType.NaturalPerson &&
                (mainLegalPerson.LegalPerson.PassportNumber != appendedLegalPerson.LegalPerson.PassportNumber ||
                mainLegalPerson.LegalPerson.PassportSeries != appendedLegalPerson.LegalPerson.PassportSeries))
            {
                throw new NotificationException(BLResources.MergeLegalPersonsDifferentPassportError);
            }

            #endregion

            using (var operationScope = _scopeFactory.CreateSpecificFor<MergeIdentity, LegalPerson>())
            {
                // Проверяем на наличие у основного юр. лица клиента объекта "Лицевой счёт" у которого юр. лицо исполнителя совпадает по ID с юр. лицом исполнителя в любом из лицевых счетов не основного юр. лица клиента.
                // Если таковой лицевой счёт есть, выводим сообщение - "Не основное Юр. лицо клиента имеет лицевой счёт дубль - слияние не возможно.".
                var intersectionAccounts =
                    mainLegalPerson.Accounts.Where(
                        x => x.IsActive && !x.IsDeleted &&
                             appendedLegalPerson.Accounts.Where(y => y.IsActive && !y.IsDeleted).Select(
                                 l => l.BranchOfficeOrganizationUnitId).Contains(
                                     x.BranchOfficeOrganizationUnitId));
                foreach (var account in intersectionAccounts)
                {
                    var appendedAccountDetails =
                        appendedLegalPerson.AccountDetails.Where(
                            x => appendedLegalPerson.Accounts.Where(
                                y => y.BranchOfficeOrganizationUnitId == account.BranchOfficeOrganizationUnitId).Select(
                                    y => y.Id).ToArray().Contains(x.AccountId))
                                           .ToArray();

                    var mainAccountDetails =
                        mainLegalPerson.AccountDetails.Where(
                            x => mainLegalPerson.Accounts.Where(
                                y => y.BranchOfficeOrganizationUnitId == account.BranchOfficeOrganizationUnitId).Select(
                                    y => y.Id).ToArray().Contains(x.AccountId))
                                       .ToArray();

                    var appendedAccount =
                        appendedLegalPerson.Accounts.SingleOrDefault(
                            x =>
                            x.IsActive && !x.IsDeleted &&
                            x.BranchOfficeOrganizationUnitId == account.BranchOfficeOrganizationUnitId);

                    var mainAccount =
                        mainLegalPerson.Accounts.SingleOrDefault(
                            x => x.IsActive && !x.IsDeleted &&
                                 x.BranchOfficeOrganizationUnitId == account.BranchOfficeOrganizationUnitId);

                    if (!appendedAccountDetails.Any() && appendedAccount != null && appendedAccount.Balance == 0)
                    {
                        _accountRepository.Delete(appendedAccount);
                        operationScope.Deleted<Account>(appendedAccount.Id);
                        continue;
                    }

                    if (!mainAccountDetails.Any() && mainAccount != null && mainAccount.Balance == 0)
                    {
                        _accountRepository.Delete(mainAccount);
                        operationScope.Deleted<Account>(mainAccount.Id);
                        continue;
                    }

                    throw new NotificationException(BLResources.MergeLegalPersonsAccountsError);
                }

                // Если все проверки прошли, производим операцию слияния:
                // Производим перепривязку дочерних / связанных объектов не основного объекта к основному (объекты выбираются все: активные, не активные, удалённые, не удалённые):
                // Лицевой счёт (Accounts.LegalPersonId)
                foreach (var account in appendedLegalPerson.Accounts)
                {
                    account.LegalPersonId = mainLegalPerson.LegalPerson.Id;
                    _accountRepository.Update(account);
                }

                operationScope.Updated<Account>(appendedLegalPerson.Accounts.Select(x => x.Id));

                // Договор (Bargains.CustomerLegalPersonId)
                foreach (var bargain in appendedLegalPerson.Bargains)
                {
                    bargain.CustomerLegalPersonId = mainLegalPerson.LegalPerson.Id;
                    _updateBargainAggregateService.Update(bargain);
                }

                operationScope.Updated<Bargain>(appendedLegalPerson.Bargains.Select(x => x.Id));

                var mainLegalPersonHasMainProfile = mainLegalPerson.Profiles.Any(x => x.IsMainProfile && x.IsActive && !x.IsDeleted);

                // Профили юр. лиц (LegalPersonProfiles.LegalPersonId)
                foreach (var profile in appendedLegalPerson.Profiles)
                {
                    profile.IsMainProfile = !mainLegalPersonHasMainProfile && profile.IsMainProfile;
                    profile.LegalPersonId = mainLegalPerson.LegalPerson.Id;
                    _updateProfileRepository.Update(profile);
                }

                _deleteLegalPersonAggregateService.Delete(appendedLegalPerson.LegalPerson, Enumerable.Empty<LegalPersonProfile>());

                operationScope.Updated<LegalPersonProfile>(appendedLegalPerson.Profiles.Select(x => x.Id))
                              .Updated<LegalPerson>(appendedLegalPerson.LegalPerson.Id);

                // Заказы (Orders.LegalPersonId)
                foreach (var order in appendedLegalPerson.Orders)
                {
                    order.LegalPersonId = mainLegalPerson.LegalPerson.Id;
                    _orderRepository.Update(order);
                }

                operationScope.Updated<Order>(appendedLegalPerson.Orders.Select(x => x.Id));

                operationScope
                    .Updated<LegalPerson>(mainLegalPerson.LegalPerson.Id, appendedLegalPerson.LegalPerson.Id)
                    .Complete();
            }

            return Response.Empty;
        }
    }
}