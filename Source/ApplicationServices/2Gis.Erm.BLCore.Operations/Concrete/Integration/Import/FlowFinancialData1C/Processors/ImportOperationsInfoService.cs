using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.DTO;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.AccountDetails;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.FinancialData1C;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AccountDetail;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowFinancialData1C.Processors
{
    public class ImportOperationsInfoService : IImportOperationsInfoService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly Lazy<IReadOnlyCollection<OperationType>> _operations;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly INotifyAboutAccountDetailModificationOperationService _notifyAboutAccountDetailModificationOperationService;

        public ImportOperationsInfoService(IAccountRepository accountRepository,
                                           IOperationScopeFactory scopeFactory,
                                           INotifyAboutAccountDetailModificationOperationService notifyAboutAccountDetailModificationOperationService)
        {
            _accountRepository = accountRepository;
            _scopeFactory = scopeFactory;
            _notifyAboutAccountDetailModificationOperationService = notifyAboutAccountDetailModificationOperationService;
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

                var newAccountDetails = GetAccountDetailsByOperations(operations, accountInfo.OwnerCode);
                _accountRepository.Create(newAccountDetails);

                var newAccountDetailsIds = newAccountDetails.Select(x => x.Id).ToArray();
                _notifyAboutAccountDetailModificationOperationService.Notify(newAccountDetailsIds.First(), newAccountDetailsIds.Skip(1).ToArray());
            }

            var accountIds = operationsByAccount.Select(x => x.Key).Union(accountInfosById.Values.SelectMany(x => x.AccountDetails).Select(x => x.AccountId));
            _accountRepository.UpdateAccountBalance(accountIds);
        }

        private IReadOnlyCollection<AccountDetail> GetAccountDetailsByOperations(IEnumerable<OperationDto> operationGroup, long ownerCode)
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
    }
}