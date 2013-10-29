using System.Linq;

using DoubleGis.Erm.BL.Aggregates.Accounts;
using DoubleGis.Erm.BL.Aggregates.LegalPersons;
using DoubleGis.Erm.BL.Aggregates.Orders;
using DoubleGis.Erm.BL.API.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BL.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Core.RequestResponse.LegalPersons;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BL.Handlers.LegalPersons
{
    public sealed class CyprusMergeLegalPersonsHandler : RequestHandler<MergeLegalPersonsRequest, EmptyResponse>, ICyprusAdapted
    {
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IBargainRepository _bargainRepository;

        public CyprusMergeLegalPersonsHandler(
            ISecurityServiceFunctionalAccess functionalAccessService,
            ILegalPersonRepository legalPersonRepository,
            IOrderRepository orderRepository,
            IAccountRepository accountRepository,
            IBargainRepository bargainRepository,
            IUserContext userContext,
            IOperationScopeFactory scopeFactory)
        {
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _scopeFactory = scopeFactory;
            _legalPersonRepository = legalPersonRepository;
            _orderRepository = orderRepository;
            _accountRepository = accountRepository;
            _bargainRepository = bargainRepository;
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

            var mainLegalPerson = _legalPersonRepository.GetInfoForMerging(request.MainLegalPersonId);
            var appendedLegalPerson = _legalPersonRepository.GetInfoForMerging(request.AppendedLegalPersonId);

            if (mainLegalPerson == null)
            {
                throw new NotificationException(BLResources.MergeLegalPersonsMainNotFoundError);
            }

            if (appendedLegalPerson == null)
            {
                throw new NotificationException(BLResources.MergeLegalPersonsAppendedNotFoundError);
            }

            // Проверим, указаны ли ИНН,КПП, серия, номер паспорта в зависимости от типа юр. лица
            // Для юр. лица должны быть указаны ИНН и КПП
            if (mainLegalPerson.LegalPerson.LegalPersonTypeEnum == (int)LegalPersonType.LegalPerson &&
                string.IsNullOrWhiteSpace(mainLegalPerson.LegalPerson.Inn))
            {
                throw new NotificationException(BLResources.MergeLegalPersonsEmptyAttributeError);
            }

            // Для ИП должен быть указан ИНН
            if (mainLegalPerson.LegalPerson.LegalPersonTypeEnum == (int)LegalPersonType.Businessman &&
                string.IsNullOrWhiteSpace(mainLegalPerson.LegalPerson.Inn))
            {
                throw new NotificationException(BLResources.MergeLegalPersonsEmptyAttributeError);
            }

            // для физ. лица должны быть указаны серия и номер паспорта
            if (mainLegalPerson.LegalPerson.LegalPersonTypeEnum == (int)LegalPersonType.NaturalPerson &&
                (string.IsNullOrWhiteSpace(mainLegalPerson.LegalPerson.PassportSeries) ||
                 string.IsNullOrWhiteSpace(mainLegalPerson.LegalPerson.PassportNumber)))
            {
                throw new NotificationException(BLResources.MergeLegalPersonsEmptyAttributeError);
            }

            if (mainLegalPerson.LegalPerson.LegalPersonTypeEnum != appendedLegalPerson.LegalPerson.LegalPersonTypeEnum)
            {
                throw new NotificationException(BLResources.MergeLegalPersonsDifferentTypesError);
            }

            if (mainLegalPerson.LegalPerson.LegalPersonTypeEnum == (int)LegalPersonType.LegalPerson &&
                (mainLegalPerson.LegalPerson.Inn != appendedLegalPerson.LegalPerson.Inn))
            {
                throw new NotificationException(BLResources.MergeLegalPersonsDifferentINNError);
            }

            if (mainLegalPerson.LegalPerson.LegalPersonTypeEnum == (int)LegalPersonType.Businessman &&
                mainLegalPerson.LegalPerson.Inn != appendedLegalPerson.LegalPerson.Inn)
            {
                throw new NotificationException(BLResources.MergeLegalPersonsDifferentINNError);
            }

            if (mainLegalPerson.LegalPerson.LegalPersonTypeEnum == (int)LegalPersonType.NaturalPerson &&
                (mainLegalPerson.LegalPerson.PassportNumber != appendedLegalPerson.LegalPerson.PassportNumber ||
                mainLegalPerson.LegalPerson.PassportSeries != appendedLegalPerson.LegalPerson.PassportSeries))
            {
                throw new NotificationException(BLResources.MergeLegalPersonsDifferentPassportNumberError);
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
                    _bargainRepository.Update(bargain);
                }

                operationScope.Updated<Bargain>(appendedLegalPerson.Bargains.Select(x => x.Id));

                // Заказы (Orders.LegalPersonId)
                foreach (var order in appendedLegalPerson.Orders)
                {
                    order.LegalPersonId = mainLegalPerson.LegalPerson.Id;
                    _orderRepository.Update(order);
                }

                operationScope.Updated<Order>(appendedLegalPerson.Orders.Select(x => x.Id));

                // Профили юр. лиц (LegalPersonProfiles.LegalPersonId)
                foreach (var profile in appendedLegalPerson.Profiles)
                {
                    profile.IsMainProfile = false;
                    profile.LegalPersonId = mainLegalPerson.LegalPerson.Id;
                    _legalPersonRepository.CreateOrUpdate(profile);
                }

                _legalPersonRepository.Deactivate(appendedLegalPerson.LegalPerson);

                operationScope.Updated<LegalPersonProfile>(appendedLegalPerson.Profiles.Select(x => x.Id))
                              .Updated<LegalPerson>(appendedLegalPerson.LegalPerson.Id);

                // TODO {all, 06.08.2013}: вносится ещё куча изменений прямо в этом методе - нужно обеспечить попадание этого в operation log`
                operationScope
                    .Updated<LegalPerson>(mainLegalPerson.LegalPerson.Id, appendedLegalPerson.LegalPerson.Id)
                    .Complete();
            }

            return Response.Empty;
        }
    }
}
