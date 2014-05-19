using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.DTO;
using DoubleGis.Erm.BLCore.API.Common.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.FinancialData1C;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Notifications;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AccountDetail;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowFinancialData1C.Processors
{
    public class ImportOperationsInfoService : IImportOperationsInfoService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IEmployeeEmailResolver _employeeEmailResolver;
        private readonly ILocalizationSettings _localizationSettings;
        private readonly ICommonLog _logger;
        private readonly INotificationSender _notificationSender;
        private readonly INotificationsSettings _notificationsSettings;
        private readonly Lazy<IReadOnlyCollection<OperationType>> _operations;
        private readonly IOperationScopeFactory _scopeFactory;

        public ImportOperationsInfoService(IAccountRepository accountRepository,
                                           IEmployeeEmailResolver employeeEmailResolver,
                                           ILocalizationSettings localizationSettings,
                                           ICommonLog logger,
                                           INotificationSender notificationSender,
                                           INotificationsSettings notificationsSettings,
                                           IOperationScopeFactory scopeFactory)
        {
            _accountRepository = accountRepository;
            _employeeEmailResolver = employeeEmailResolver;
            _localizationSettings = localizationSettings;
            _logger = logger;
            _notificationSender = notificationSender;
            _notificationsSettings = notificationsSettings;
            _scopeFactory = scopeFactory;
            _operations = new Lazy<IReadOnlyCollection<OperationType>>(() => _accountRepository.GetOperationsInSyncWith1C());
        }

        public void Import(IEnumerable<IServiceBusDto> dtos)
        {
            var operationsInfoServiceBusDtos = dtos.Cast<OperationsInfoServiceBusDto>().ToArray();

            using (var scope = _scopeFactory.CreateNonCoupled<ImportOperationsInfoIdentity>())
            {
                foreach (var operationsInfo in operationsInfoServiceBusDtos)
                {
                    Process(operationsInfo);
                }

                scope.Complete();
            }
        }

        private void Process(OperationsInfoServiceBusDto dto)
        {
            ValidateOperationDates(dto);

            var branchCodes = dto.LegalEntityBranchCode1C.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            var operationsByAccount = dto.Operations
                                         .Select(x => new OperationDto
                                             {
                                                 AccountCode = x.AccountCode,
                                                 Amount = x.Amount,
                                                 DocumentNumber = x.DocumentNumber,
                                                 IsPlus = x.IsPlus,
                                                 OperationTypeCode = x.OperationTypeCode,
                                                 TransactionDate = x.TransactionDate
                                             })
                                         .ToLookup(x => x.AccountCode);

            var accountInfosById = _accountRepository.GetAccountsForImportFrom1C(branchCodes, dto.StartDate, dto.EndDate).ToDictionary(x => x.Id);

            _accountRepository.Delete(accountInfosById.Values.SelectMany(x => x.AccountDetails));

            foreach (var operations in operationsByAccount)
            {
                var accountCode = operations.Key;
                AccountInfoForImportFrom1C accountInfo;
                if (!accountInfosById.TryGetValue(accountCode, out accountInfo))
                {
                    throw new BusinessLogicException(string.Format("Не найден лицевой счет с Id = {0} и BranchOfficeCode1C in ({1})",
                                                                   accountCode,
                                                                   dto.LegalEntityBranchCode1C));
                }

                var newAccountDetails = CreateNewAccountDetails(operations, accountInfo.OwnerCode);
                _accountRepository.Create(newAccountDetails);

                foreach (var accountDetail in newAccountDetails)
                {
                    NotifyAboutPaymentReceived(accountInfo.Id,
                                               accountInfo.OwnerCode,
                                               accountInfo.LegalPersonName,
                                               accountInfo.BranchOfficeLegalName,
                                               accountDetail.Amount,
                                               accountDetail.TransactionDate);
                }
            }

            var accountIds = operationsByAccount.Select(x => x.Key).Union(accountInfosById.Values.SelectMany(x => x.AccountDetails).Select(x => x.AccountId));
            _accountRepository.UpdateAccountBalance(accountIds);
        }

        private static void ValidateOperationDates(OperationsInfoServiceBusDto operationsInfo)
        {
            var invalidOperations = operationsInfo.Operations
                                                  .Where(x => x.TransactionDate > operationsInfo.EndDate || x.TransactionDate < operationsInfo.StartDate)
                                                  .Select(x => x.DocumentNumber)
                                                  .ToArray();

            if (invalidOperations.Any())
            {
                throw new BusinessLogicException(
                    string.Format("Дата проведения операций с DocumentNumber = [{0}] не попадает в период [{1:G} - {2:G}], указанный в заголовке сообщения",
                                  string.Join(", ", invalidOperations),
                                  operationsInfo.StartDate,
                                  operationsInfo.EndDate));
            }
        }

        private IReadOnlyCollection<AccountDetail> CreateNewAccountDetails(IEnumerable<OperationDto> operationGroup, long ownerCode)
        {
            var result = new List<AccountDetail>();

            foreach (var operation in operationGroup)
            {
                var operationType =
                    _operations.Value.SingleOrDefault(x => x.IsPlus == operation.IsPlus && int.Parse(x.SyncCode1C) == operation.OperationTypeCode);
                if (operationType == null)
                {
                    throw new BusinessLogicException(string.Format("Импорт оплат из 1С - операция с кодом 1С = [{0}] и знаком [IsPlus={1}] не поддерживается",
                                                                   operation.OperationTypeCode,
                                                                   operation.IsPlus));
                }

                result.Add(new AccountDetail
                    {
                        AccountId = operation.AccountCode,
                        Amount = operation.Amount,
                        OperationTypeId = operationType.Id,
                        TransactionDate = operation.TransactionDate,
                        Description = operation.DocumentNumber,
                        IsActive = true,
                        OwnerCode = ownerCode
                    });
            }

            return result;
        }

        private void NotifyAboutPaymentReceived(long accountId,
                                                long accountOwnerCode,
                                                string legalPersonName,
                                                string branchOfficeLegalName,
                                                decimal accountDetailAmount,
                                                DateTime transactionDate)
        {
            if (!_notificationsSettings.EnableNotifications)
            {
                _logger.InfoEx("Notifications disabled in config file");
                return;
            }

            string accountOwnerEmail;
            if (_employeeEmailResolver.TryResolveEmail(accountOwnerCode, out accountOwnerEmail) && !string.IsNullOrEmpty(accountOwnerEmail))
            {
                var body = string.Format(BLResources.PaymentReceivedBodyTemplate,
                                         accountId,
                                         legalPersonName,
                                         branchOfficeLegalName,
                                         accountDetailAmount,
                                         transactionDate.ToString(_localizationSettings.ApplicationCulture));

                _notificationSender.PostMessage(new[] { new NotificationAddress(accountOwnerEmail) },
                                                BLResources.PaymentReceivedSubject,
                                                body);
            }
            else
            {
                _logger.ErrorFormatEx("Can't send notification about - payment received. Can't get to_address email. Account id: {0}. Owner code: {1}",
                                      accountId,
                                      accountOwnerCode);
            }
        }
    }
}