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
    public sealed class CzechMergeLegalPersonsHandler : RequestHandler<MergeLegalPersonsRequest, EmptyResponse>, ICzechAdapted
    {
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IBargainRepository _bargainRepository;

        public CzechMergeLegalPersonsHandler(
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
            #region ��������

            if (request.AppendedLegalPersonId == request.MainLegalPersonId)
            {
                throw new NotificationException(BLResources.MergeLegalPersonsSameIdError);
            }

            // �������� ��������������� ����������
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

            // ��������, ������� �� ���,���, �����, ����� �������� � ����������� �� ���� ��. ����
            // ��� ��. ���� ������ ���� ������� ��� � ���
            if (mainLegalPerson.LegalPerson.LegalPersonTypeEnum == (int)LegalPersonType.LegalPerson &&
                string.IsNullOrWhiteSpace(mainLegalPerson.LegalPerson.Inn))
            {
                throw new NotificationException(BLResources.MergeLegalPersonsEmptyAttributeError);
            }

            //��� �� ������ ���� ������ ���
            if (mainLegalPerson.LegalPerson.LegalPersonTypeEnum == (int)LegalPersonType.Businessman &&
                string.IsNullOrWhiteSpace(mainLegalPerson.LegalPerson.Inn))
            {
                throw new NotificationException(BLResources.MergeLegalPersonsEmptyAttributeError);
            }

            //��� ���. ���� ������ ���� ������� ����� � ����� ��������
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
                // ��������� �� ������� � ��������� ��. ���� ������� ������� "������� ����" � �������� ��. ���� ����������� ��������� �� ID � ��. ����� ����������� � ����� �� ������� ������ �� ��������� ��. ���� �������.
                // ���� ������� ������� ���� ����, ������� ��������� - "�� �������� ��. ���� ������� ����� ������� ���� ����� - ������� �� ��������.".
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

                // ���� ��� �������� ������, ���������� �������� �������:
                // ���������� ������������ �������� / ��������� �������� �� ��������� ������� � ��������� (������� ���������� ���: ��������, �� ��������, ��������, �� ��������):
                // ������� ���� (Accounts.LegalPersonId)
                foreach (var account in appendedLegalPerson.Accounts)
                {
                    account.LegalPersonId = mainLegalPerson.LegalPerson.Id;
                    _accountRepository.Update(account);
                }

                operationScope.Updated<Account>(appendedLegalPerson.Accounts.Select(x => x.Id));

                // ������� (Bargains.CustomerLegalPersonId)
                foreach (var bargain in appendedLegalPerson.Bargains)
                {
                    bargain.CustomerLegalPersonId = mainLegalPerson.LegalPerson.Id;
                    _bargainRepository.Update(bargain);
                }

                operationScope.Updated<Bargain>(appendedLegalPerson.Bargains.Select(x => x.Id));

                // ������ (Orders.LegalPersonId)
                foreach (var order in appendedLegalPerson.Orders)
                {
                    order.LegalPersonId = mainLegalPerson.LegalPerson.Id;
                    _orderRepository.Update(order);
                }

                operationScope.Updated<Order>(appendedLegalPerson.Orders.Select(x => x.Id));

                // ������� ��. ��� (LegalPersonProfiles.LegalPersonId)
                foreach (var profile in appendedLegalPerson.Profiles)
                {
                    profile.IsMainProfile = false;
                    profile.LegalPersonId = mainLegalPerson.LegalPerson.Id;
                    _legalPersonRepository.CreateOrUpdate(profile);
                }

                _legalPersonRepository.Deactivate(appendedLegalPerson.LegalPerson);

                operationScope.Updated<LegalPersonProfile>(appendedLegalPerson.Profiles.Select(x => x.Id))
                              .Updated<LegalPerson>(appendedLegalPerson.LegalPerson.Id);

                // TODO {all, 06.08.2013}: �������� ��� ���� ��������� ����� � ���� ������ - ����� ���������� ��������� ����� � operation log`
                operationScope
                    .Updated<LegalPerson>(mainLegalPerson.LegalPerson.Id, appendedLegalPerson.LegalPerson.Id)
                    .Complete();
            }

            return Response.Empty;
        }
    }
}