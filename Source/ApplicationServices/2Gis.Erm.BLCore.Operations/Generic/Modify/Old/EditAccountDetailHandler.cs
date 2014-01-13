﻿using DoubleGis.Erm.BLCore.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.Aggregates.BranchOffices;
using DoubleGis.Erm.BLCore.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Notifications;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditAccountDetailHandler : RequestHandler<EditRequest<AccountDetail>, EmptyResponse>
    {
        private readonly IAppSettings _appSettings;
        private readonly IEmployeeEmailResolver _employeeEmailResolver;
        private readonly INotificationSender _notificationSender;
        private readonly ICommonLog _logger;
        private readonly IAccountRepository _accountRepository;
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly IBranchOfficeRepository _branchOfficeRepository;

        public EditAccountDetailHandler(IAppSettings appSettings,
                                        IEmployeeEmailResolver employeeEmailResolver,
                                        INotificationSender notificationSender,
                                        ICommonLog logger,
                                        IAccountRepository accountRepository,
                                        ILegalPersonRepository legalPersonRepository,
                                        IBranchOfficeRepository branchOfficeRepository)
        {
            _appSettings = appSettings;
            _employeeEmailResolver = employeeEmailResolver;
            _notificationSender = notificationSender;
            _logger = logger;
            _accountRepository = accountRepository;
            _legalPersonRepository = legalPersonRepository;
            _branchOfficeRepository = branchOfficeRepository;
        }

        protected override EmptyResponse Handle(EditRequest<AccountDetail> request)
        {
            var accountDetail = request.Entity;
            if (accountDetail.IsNew())
            {
                _accountRepository.Create(accountDetail);
                _accountRepository.UpdateAccountBalance(new[] { accountDetail.AccountId });

                var isPlusAccountDetail = accountDetail.OperationTypeId == (int)AccountOperationTypeEnum.AccountChargeIsPlus ||
                                          accountDetail.OperationTypeId == (int)AccountOperationTypeEnum.CashIncomeIsPlus;
                if (isPlusAccountDetail)
                {
                    NotifyAboutPaymentReceived(accountDetail);
                }
            }
            else
            {
                _accountRepository.Update(accountDetail);
                _accountRepository.UpdateAccountBalance(new[] { accountDetail.AccountId });
            }

            return Response.Empty;
        }

        private void NotifyAboutPaymentReceived(AccountDetail accountDetail)
        {
            if (!_appSettings.EnableNotifications)
            {
                _logger.InfoEx("Notifications disabled in config file");
                return;
            }

            var account = _accountRepository.FindAccount(accountDetail.AccountId); 
            if (account != null)
            {
                string accountOwnerEmail;
                if (_employeeEmailResolver.TryResolveEmail(account.OwnerCode, out accountOwnerEmail) && !string.IsNullOrEmpty(accountOwnerEmail))
                {
                    var legalPerson = _legalPersonRepository.FindLegalPerson(account.LegalPersonId);
                    if (legalPerson == null)
                    {
                        _logger.ErrorEx("Can't find legal person with id: " + account.LegalPersonId);
                        return;
                    }

                    var branchOffice = _branchOfficeRepository.FindBranchOfficeOrganizationUnit(account.BranchOfficeOrganizationUnitId);
                    if (branchOffice == null)
                    {
                        _logger.ErrorEx("Can't find branch office with id: " + account.BranchOfficeOrganizationUnitId);
                        return;
                    }

                    _notificationSender.PostMessage(new[] { new NotificationAddress(accountOwnerEmail) },
                                                    BLResources.PaymentReceivedSubject,
                                                    string.Format(BLResources.PaymentReceivedBodyTemplate,
                                                                  account.Id,
                                                                  legalPerson.LegalName,
                                                                  branchOffice.ShortLegalName,
                                                                  accountDetail.Amount,
                                                                  accountDetail.TransactionDate.ToString(LocalizationSettings.ApplicationCulture)));
                }
                else
                {
                    _logger.ErrorEx("Can't send notification about - payment received. Can't get to_address email. Account id: " + account.Id + ". Owner code: " + account.OwnerCode);
                }
            }           
        }
    }
}
