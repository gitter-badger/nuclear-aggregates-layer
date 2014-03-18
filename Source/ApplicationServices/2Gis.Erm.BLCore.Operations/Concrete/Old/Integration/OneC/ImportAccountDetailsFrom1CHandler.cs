using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Common.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.OneC;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Notifications;
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
        private readonly INotificationsSettings _notificationsSettings;
        private readonly ICommonLog _logger;
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;
        private readonly IAccountRepository _accountRepository;
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly IEmployeeEmailResolver _employeeEmailResolver;
        private readonly INotificationSender _notificationSender;

        public ImportAccountDetailsFrom1CHandler(ILocalizationSettings localizationSettings,
                                                 INotificationsSettings notificationsSettings,
                                                 ICommonLog logger,
                                                 IBranchOfficeReadModel branchOfficeReadModel,
                                                 IAccountRepository accountRepository,
                                                 ILegalPersonRepository legalPersonRepository,
                                                 IEmployeeEmailResolver employeeEmailResolver,
                                                 INotificationSender notificationSender)

        {
            _localizationSettings = localizationSettings;
            _notificationsSettings = notificationsSettings;
            _logger = logger;
            _branchOfficeReadModel = branchOfficeReadModel;
            _accountRepository = accountRepository;
            _legalPersonRepository = legalPersonRepository;
            _employeeEmailResolver = employeeEmailResolver;
            _notificationSender = notificationSender;
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

                var newAccountDetailContainers = new List<Tuple<LegalPerson, BranchOfficeOrganizationUnit, Account, AccountDetail>>();

                // skipping first row (header)
                for (var i = 1; i < nonParsedRows.Length; i++)
                {
                    Tuple<LegalPerson, BranchOfficeOrganizationUnit, Account, AccountDetail> accountDetailContainer;
                    if (!TryGetAccountDetailContainer(header, errors, i, nonParsedRows[i], out accountDetailContainer))
                    {
                        continue;
                    }

                    newAccountDetailContainers.Add(accountDetailContainer);
                    parsedRowsCount++;
                }

                var oldAccountDetails = _accountRepository.GetAccountDetailsForImportFrom1COperation(header.BranchOfficeOrganizationUnit1CCode,
                                                                                                     header.Period.Start,
                                                                                                     header.Period.End);

                var newAccountDetails = newAccountDetailContainers.Select(x => x.Item4).ToArray();

                _accountRepository.Delete(oldAccountDetails);
                _accountRepository.Create(newAccountDetails);

                var accountIds = oldAccountDetails.Select(x => x.AccountId).Union(newAccountDetails.Select(x => x.AccountId)).ToArray();
                _accountRepository.UpdateAccountBalance(accountIds);

                NotifyAboutPaymentReceived(newAccountDetailContainers);
                
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
                                                  out Tuple<LegalPerson, BranchOfficeOrganizationUnit, Account, AccountDetail> accountDetailContainer)
        {
            accountDetailContainer = null;
            AccountDetailsFrom1CHelper.CsvRow row;

            if (!AccountDetailsFrom1CHelper.CsvRow.TryParse(nonParsedRow, _localizationSettings.ApplicationCulture, out row))
            {
                var message = string.Format("Невозможно распознать строку при импорте списаний по лицевым счетам. Строка [{0}].", index);
                errors.Add(message);

                _logger.ErrorEx(message);
                return false;
            }

            if (!header.BranchOfficeOrganizationUnit1CCode.Contains(row.BranchOfficeOrganizationUnit1CCode))
            {
                var message = string.Format("Код юридического лица организации [{0}] не найден в заголовке файла. Строка будет пропущена. Строка [{1}].",
                                            row.BranchOfficeOrganizationUnit1CCode,
                                            index);
                errors.Add(message);

                _logger.ErrorEx(message);
                return false;
            }

            if (row.OperationDate < header.Period.Start || row.OperationDate > header.Period.End)
            {
                var message = string.Format("Дата операции [{0}] не попадает период, заданный в заголовке файла. Строка будет пропущена. Строка [{1}].",
                                            row.OperationDate,
                                            index);
                errors.Add(message);

                _logger.ErrorEx(message);
                return false;
            }

            var legalPerson = _legalPersonRepository.FindLegalPerson(row.LegalPerson1CCode, row.InnOrPassportSeries, row.KppOrPassportNumber);
            if (legalPerson == null)
            {
                var message = string.Format("Активное юридическое лицо клиента с кодом [{0}] не найдено. Строка [{1}].", row.LegalPerson1CCode, index);
                errors.Add(message);

                _logger.ErrorEx(message);
                return false;
            }

            var branchOfficeOrganizationUnit = _branchOfficeReadModel.GetBranchOfficeOrganizationUnit(row.BranchOfficeOrganizationUnit1CCode);
            if (branchOfficeOrganizationUnit == null)
            {
                var message = string.Format("Активное юридическое лицо отделения организации с кодом [{0}] не найдено. Строка [{1}].",
                                            row.BranchOfficeOrganizationUnit1CCode,
                                            index);
                errors.Add(message);

                _logger.ErrorEx(message);
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

                _logger.ErrorEx(message);
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

                _logger.ErrorEx(message);
                return false;
            }

            var accountDetail = new AccountDetail
                {
                    AccountId = account.Id,
                    OperationTypeId = operationType.Id,
                    Amount = row.Amount,
                    TransactionDate = row.OperationDate,
                    Description = string.Format("{0};{1}", row.DocumentNumber, row.DocumentDate.ToString("dd.MM.yyyy")),
                    OwnerCode = account.OwnerCode,
                    IsActive = true
                };

            accountDetailContainer = Tuple.Create(legalPerson, branchOfficeOrganizationUnit, account, accountDetail);

            return true;
        }

        private void NotifyAboutPaymentReceived(IEnumerable<Tuple<LegalPerson, BranchOfficeOrganizationUnit, Account, AccountDetail>> accountDetailContainers)
        {
            if (!_notificationsSettings.EnableNotifications)
            {
                _logger.InfoEx("Notifications disabled in config file");
                return;
            }

            foreach (var accountDetailContainer in accountDetailContainers)
            {
                var legalPerson = accountDetailContainer.Item1;
                var branchOffice = accountDetailContainer.Item2;
                var account = accountDetailContainer.Item3;
                var accountDetail = accountDetailContainer.Item4;

                string accountOwnerEmail;
                if (_employeeEmailResolver.TryResolveEmail(account.OwnerCode, out accountOwnerEmail) && !string.IsNullOrEmpty(accountOwnerEmail))
                {

                    _notificationSender.PostMessage(new[] { new NotificationAddress(accountOwnerEmail) },
                                                    BLResources.PaymentReceivedSubject,
                                                    string.Format(BLResources.PaymentReceivedBodyTemplate,
                                                                  account.Id,
                                                                  legalPerson.LegalName,
                                                                  branchOffice.ShortLegalName,
                                                                  accountDetail.Amount,
                                                                  accountDetail.TransactionDate.ToString(_localizationSettings.ApplicationCulture)));
                }
                else
                {
                    _logger.ErrorEx("Can't send notification about - payment received. Can't get to_address email. Account id: " + account.Id + ". Owner code: " + account.OwnerCode);
                }
            }
        }
    }
}
