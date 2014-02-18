using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.ServiceBus;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Notifications;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.ServiceBusBroker;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AccountDetail;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.ServiceBus.Import
{
    public class ImportFlowFinancialData1CHandler : RequestHandler<ImportFlowFinancialData1CRequest, EmptyResponse>
    {
        private readonly IClientProxyFactory _clientProxyFactory;
        private readonly IIntegrationSettings _integrationSettings;
        private readonly ICommonLog _logger;
        private readonly IAccountRepository _accountRepository;
        private readonly IAppSettings _appSettings;
        private readonly IEmployeeEmailResolver _employeeEmailResolver;
        private readonly INotificationSender _notificationSender;
        private readonly IReadOnlyCollection<OperationType> _operations;
        private readonly IOperationScopeFactory _scopeFactory;

        public ImportFlowFinancialData1CHandler(IClientProxyFactory clientProxyFactory,
                                                IIntegrationSettings integrationSettings,
                                                ICommonLog logger,
                                                IAccountRepository accountRepository,
                                                IAppSettings appSettings,
                                                IEmployeeEmailResolver employeeEmailResolver,
                                                INotificationSender notificationSender,
                                                IOperationScopeFactory scopeFactory)
        {
            _clientProxyFactory = clientProxyFactory;
            _integrationSettings = integrationSettings;
            _logger = logger;
            _accountRepository = accountRepository;
            _appSettings = appSettings;
            _employeeEmailResolver = employeeEmailResolver;
            _notificationSender = notificationSender;
            _scopeFactory = scopeFactory;
            _operations = _accountRepository.GetOperationsInSyncWith1C();
        }

        protected override EmptyResponse Handle(ImportFlowFinancialData1CRequest request)
        {
            var clientProxy = _clientProxyFactory.GetClientProxy<IBrokerApiReceiver>("NetTcpBinding_IBrokerApiReceiver");

            clientProxy.Execute(brokerApiReceiver =>
            {
                brokerApiReceiver.BeginReceiving(_integrationSettings.IntegrationApplicationName, "flowFinancialData1C");

                try
                {
                    while (true)
                    {
                        var package = brokerApiReceiver.ReceivePackage();
                        if (package == null)
                        {
                            _logger.InfoEx("Импорт оплат из 1С - шина пустая");
                            break;
                        }

                        _logger.InfoFormatEx("Импорт оплат из 1С - загружено {0} объектов из шины", package.Length);
                        if (package.Length != 0)
                        {
                            ProcessPackage(package);
                        }

                        brokerApiReceiver.Acknowledge();
                    }
                }
                finally
                {
                    brokerApiReceiver.EndReceiving();
                }
            });

            return Response.Empty;
        }

        private void ProcessPackage(IEnumerable<string> package)
        {
            var operationsInfos = package.Select(obj => XDocument.Parse(obj).Root)
                              .Where(root => root != null && root.Name.LocalName == "OperationsInfo")
                              .Select(OperationsInfoDto.Parse)
                              .ToArray();

            using (var scope = _scopeFactory.CreateNonCoupled<ImportFlowFinancialData1CIdentity>())
            {
                foreach (var operationsInfo in operationsInfos)
                {
                    ProcessOperations(operationsInfo);
                }

                scope.Complete();
            }
        }

        /// <summary>
        /// Обработка пришедших операций по ЛС
        /// </summary>
        private void ProcessOperations(OperationsInfoDto operationsInfo)
        {
            ValidateOperationDates(operationsInfo);

            var branchCodes = operationsInfo.LegalEntityBranchCode1C.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            var operationsByAccount = operationsInfo.Operations
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

            var accountInfosById = _accountRepository.GetAccountsForImportFrom1C(branchCodes, operationsInfo.StartDate, operationsInfo.EndDate).ToDictionary(x => x.Id);

            _accountRepository.Delete(accountInfosById.Values.SelectMany(x => x.AccountDetails));

            foreach (var operations in operationsByAccount)
            {
                var accountCode = operations.Key;
                AccountRepository.AccountInfoForImportFrom1C accountInfo;
                if (!accountInfosById.TryGetValue(accountCode, out accountInfo))
                {
                    throw new BusinessLogicException(string.Format("Не найден лицевой счет с Id = {0} и BranchOfficeCode1C in ({1})",
                                                                   accountCode,
                                                                   operationsInfo.LegalEntityBranchCode1C));
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

            _accountRepository.UpdateAccountBalance(operationsByAccount.Select(x => x.Key).Union(accountInfosById.Keys));
        }

        private static void ValidateOperationDates(OperationsInfoDto operationsInfo)
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
                var operationType = _operations.SingleOrDefault(x => x.IsPlus == operation.IsPlus && int.Parse(x.SyncCode1C) == operation.OperationTypeCode);
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
            if (!_appSettings.EnableNotifications)
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
                                         transactionDate.ToString(LocalizationSettings.ApplicationCulture));

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

        private class OperationsInfoDto
        {
            public DateTime StartDate { get; private set; }
            public DateTime EndDate { get; private set; }

            // FIXME {a.tukaev, 23.10.2013}: Это значение должно совпадать с кодом 1С юр. лица исполнителя, который связан с лицевым счетом, по которому происходит поступление
            // DONE {y.baranihin, 25.10.2013}: Запилил проверку 
            public string LegalEntityBranchCode1C { get; private set; }
            public ICollection<OperationDto> Operations { get; private set; }

            public static OperationsInfoDto Parse(XElement root)
            {
                var operationInfo = new OperationsInfoDto
                {
                    StartDate = (DateTime)root.Attribute("StartDate"),
                    EndDate = (DateTime)root.Attribute("EndDate"),
                    LegalEntityBranchCode1C = (string)root.Attribute("LegalEntityBranchCode1C"),
                    Operations = new List<OperationDto>()
                };

                foreach (var operationElement in root.Descendants("Operations").Single().Descendants("Operation"))
                {
                    operationInfo.Operations.Add(new OperationDto
                    {
                        AccountCode = (long)operationElement.Attribute("AccountCode"),
                        Amount = (decimal)operationElement.Attribute("Amount"),
                        TransactionDate = (DateTime)operationElement.Attribute("TransactionDate"),
                        OperationTypeCode = (int)operationElement.Attribute("OperationTypeCode"),
                        DocumentNumber = (string)operationElement.Attribute("DocumentNumber"),
                        IsPlus = (bool?)operationElement.Attribute("IsPlus") ?? true
                    });
                }

                return operationInfo;
            }
        }

        private class OperationDto
        {
            public long AccountCode { get; set; }
            public decimal Amount { get; set; }
            public DateTime TransactionDate { get; set; }
            public int OperationTypeCode { get; set; }
            public bool IsPlus { get; set; }
            public string DocumentNumber { get; set; }
        }
    }
}