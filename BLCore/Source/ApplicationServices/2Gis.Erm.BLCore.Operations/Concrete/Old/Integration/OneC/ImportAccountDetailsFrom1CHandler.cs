using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.AccountDetails;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.OneC;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.OneC
{
    [UseCase(Duration = UseCaseDuration.ExtraLong)]
    public sealed class ImportAccountDetailsFrom1CHandler : RequestHandler<ImportAccountDetailsFrom1CRequest, ImportResponse>
    {
        private readonly ILocalizationSettings _localizationSettings;
        private readonly ICommonLog _logger;
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;
        private readonly IAccountRepository _accountRepository;
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly INotifyAboutAccountDetailModificationOperationService _notifyAboutAccountDetailModificationOperationService;

        public ImportAccountDetailsFrom1CHandler(ILocalizationSettings localizationSettings,
                                                 ICommonLog logger,
                                                 IBranchOfficeReadModel branchOfficeReadModel,
                                                 IAccountRepository accountRepository,
                                                 ILegalPersonRepository legalPersonRepository,
                                                 INotifyAboutAccountDetailModificationOperationService notifyAboutAccountDetailModificationOperationService)

        {
            _localizationSettings = localizationSettings;
            _logger = logger;
            _branchOfficeReadModel = branchOfficeReadModel;
            _accountRepository = accountRepository;
            _legalPersonRepository = legalPersonRepository;
            _notifyAboutAccountDetailModificationOperationService = notifyAboutAccountDetailModificationOperationService;
        }

        protected override ImportResponse Handle(ImportAccountDetailsFrom1CRequest request)
        {
            var targetEncoding = _localizationSettings.ApplicationCulture.ToDefaultAnsiEncoding();
            var nonParsedRows = AccountDetailsFrom1CHelper.ParseStreamAsRows(request.InputStream, targetEncoding);
            if (!nonParsedRows.Any())
            {
                throw new NotificationException(string.Format(BLResources.FileIsEmpty, request.FileName));
            }

            AccountDetailsFrom1CHelper.CsvHeader header;
            if (!AccountDetailsFrom1CHelper.CsvHeader.TryParse(nonParsedRows[0], _localizationSettings.ApplicationCulture, out header))
            {
                throw new NotificationException(string.Format(BLResources.WrongFileFormat, request.FileName));
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var errors = new List<string>();
                var parsedRowsCount = 0;

                var newAccountDetails = new List<AccountDetail>();

                // skipping first row (header)
                for (var i = 1; i < nonParsedRows.Length; i++)
                {
                    AccountDetail newAccountDetail;
                    if (!TryGetAccountDetailContainer(header, errors, i, nonParsedRows[i], out newAccountDetail))
                    {
                        continue;
                    }

                    newAccountDetails.Add(newAccountDetail);
                    parsedRowsCount++;
                }

                var oldAccountDetails = _accountRepository.GetAccountDetailsForImportFrom1COperation(header.BranchOfficeOrganizationUnit1CCode,
                                                                                                     header.Period.Start,
                                                                                                     header.Period.End);

                _accountRepository.Delete(oldAccountDetails);
                _accountRepository.Create(newAccountDetails);

                var accountIds = oldAccountDetails.Select(x => x.AccountId).Union(newAccountDetails.Select(x => x.AccountId)).ToArray();
                _accountRepository.UpdateAccountBalance(accountIds);

                var newAccountDetailsIds = newAccountDetails.Select(x => x.Id).ToArray();
                _notifyAboutAccountDetailModificationOperationService.Notify(newAccountDetailsIds.First(), newAccountDetailsIds.Skip(1).ToArray());

                transaction.Complete();

                return new ImportResponse
                    {
                        Processed = parsedRowsCount,
                        Total = nonParsedRows.Length - 1,
                        Messages = errors,
                        OrganizationUnitId = null,
                    };
            }
        }

        private bool TryGetAccountDetailContainer(AccountDetailsFrom1CHelper.CsvHeader header,
                                                  ICollection<string> errors,
                                                  int index,
                                                  string nonParsedRow,
                                                  out AccountDetail newAccountDetail)
        {
            newAccountDetail = null;
            AccountDetailsFrom1CHelper.CsvRow row;

            if (!AccountDetailsFrom1CHelper.CsvRow.TryParse(nonParsedRow, _localizationSettings.ApplicationCulture, out row))
            {
                var message = string.Format("Невозможно распознать строку при импорте списаний по лицевым счетам. Строка [{0}].", index);
                errors.Add(message);

                _logger.Error(message);
                return false;
            }

            if (!header.BranchOfficeOrganizationUnit1CCode.Contains(row.BranchOfficeOrganizationUnit1CCode))
            {
                var message = string.Format("Код юридического лица организации [{0}] не найден в заголовке файла. Строка будет пропущена. Строка [{1}].",
                                            row.BranchOfficeOrganizationUnit1CCode,
                                            index);
                errors.Add(message);

                _logger.Error(message);
                return false;
            }

            if (row.OperationDate < header.Period.Start || row.OperationDate > header.Period.End)
            {
                var message = string.Format("Дата операции [{0}] не попадает период, заданный в заголовке файла. Строка будет пропущена. Строка [{1}].",
                                            row.OperationDate,
                                            index);
                errors.Add(message);

                _logger.Error(message);
                return false;
            }

            var legalPerson = _legalPersonRepository.FindLegalPerson(row.LegalPerson1CCode, row.InnOrPassportSeries, row.KppOrPassportNumber);
            if (legalPerson == null)
            {
                var message = string.Format("Активное юридическое лицо клиента с кодом [{0}] не найдено. Строка [{1}].", row.LegalPerson1CCode, index);
                errors.Add(message);

                _logger.Error(message);
                return false;
            }

            var branchOfficeOrganizationUnit = _branchOfficeReadModel.GetBranchOfficeOrganizationUnit(row.BranchOfficeOrganizationUnit1CCode);
            if (branchOfficeOrganizationUnit == null)
            {
                var message = string.Format("Активное юридическое лицо отделения организации с кодом [{0}] не найдено. Строка [{1}].",
                                            row.BranchOfficeOrganizationUnit1CCode,
                                            index);
                errors.Add(message);

                _logger.Error(message);
                return false;
            }

            var account = _accountRepository.FindAccount(branchOfficeOrganizationUnit.Id, legalPerson.Id);
            if (account == null)
            {
                var message = string.Format(
                    "Активный лицевой счет для юр. лица отделения организации с кодом [{0}] и юр. лица клиента с кодом [{1}] не найден. Строка [{2}].",
                    branchOfficeOrganizationUnit,
                    legalPerson,
                    index);
                errors.Add(message);

                _logger.Error(message);
                return false;
            }

            OperationType operationType = null;

            var operationTypes = _accountRepository.GetOperationTypes(row.DocumentType).ToArray();
            if (operationTypes.Count() > 1)
            {
                switch (row.OperationType)
                {
                    case AccountDetailsFrom1CHelper.InternalOperationType.Charge:
                        operationType = operationTypes.SingleOrDefault(x => x.IsPlus);
                        break;
                    case AccountDetailsFrom1CHelper.InternalOperationType.Withdrawal:
                        operationType = operationTypes.SingleOrDefault(x => !x.IsPlus);
                        break;
                }
            }
            else
            {
                operationType = operationTypes.SingleOrDefault();
            }

            if (operationType == null)
            {
                var message = string.Format("Активный тип операции с кодом [{0}] не найден. Строка [{1}].", row.DocumentType, index);
                errors.Add(message);

                _logger.Error(message);
                return false;
            }

            newAccountDetail = new AccountDetail
                {
                    AccountId = account.Id,
                    OperationTypeId = operationType.Id,
                    Amount = row.Amount,
                    TransactionDate = row.OperationDate,
                    Description = string.Format("{0};{1}", row.DocumentNumber, row.DocumentDate.ToString("dd.MM.yyyy")),
                    OwnerCode = account.OwnerCode,
                    IsActive = true
                };

            return true;
        }
    }
}