using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.AccountDetails;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Notifications;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AccountDetail;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.AccountDetails
{
    public class NotifyAboutAccountDetailModificationOperationService : INotifyAboutAccountDetailModificationOperationService
    {
        private readonly ITracer _logger;
        private readonly INotificationsSettings _notificationsSettings;
        private readonly INotificationSender _notificationSender;
        private readonly IEmployeeEmailResolver _employeeEmailResolver;
        private readonly IAccountReadModel _accountReadModel;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly Dictionary<long, string> _ownerEmailsMap = new Dictionary<long, string>();

        public NotifyAboutAccountDetailModificationOperationService(ITracer logger,
                                                                    INotificationsSettings notificationsSettings,
                                                                    INotificationSender notificationSender,
                                                                    IEmployeeEmailResolver employeeEmailResolver,
                                                                    IAccountReadModel accountReadModel,
                                                                    IOperationScopeFactory operationScopeFactory)
        {
            _logger = logger;
            _notificationsSettings = notificationsSettings;
            _notificationSender = notificationSender;
            _employeeEmailResolver = employeeEmailResolver;
            _accountReadModel = accountReadModel;
            _operationScopeFactory = operationScopeFactory;
        }

        public void Notify(long accountDetailId, params long[] accountDetailIds)
        {
            if (!_notificationsSettings.EnableNotifications)
            {
                _logger.Info("Notifications disabled in config file");
                return;
            }

            using (var scope = _operationScopeFactory.CreateNonCoupled<NotifyAboutAccountDetailModificationIdentity>())
            {
                var allAccountDetailIds = accountDetailIds.Union(new[] { accountDetailId });
                var infoToSend = _accountReadModel.GetAccountDetailsInfoToSendNotification(allAccountDetailIds);
                var ownerCodes = infoToSend.Select(x => x.AccountOwnerCode).Distinct().ToArray();
                foreach (var ownerCode in ownerCodes)
                {
                    string accountOwnerEmail;
                    if (!_ownerEmailsMap.ContainsKey(ownerCode) && _employeeEmailResolver.TryResolveEmail(ownerCode, out accountOwnerEmail) && !string.IsNullOrEmpty(accountOwnerEmail))
                    {
                        _ownerEmailsMap.Add(ownerCode, accountOwnerEmail);
                    }
                }

                foreach (var infoToSendNotificationDto in infoToSend)
                {
                    if (_ownerEmailsMap.ContainsKey(infoToSendNotificationDto.AccountOwnerCode))
                    {
                        var accountOperationLetterBody = string.Format(
                            BLResources.AccountOperationLetterBodyTemplate,
                            infoToSendNotificationDto.LegalPersonName,
                            infoToSendNotificationDto.BranchOfficeName,
                            infoToSendNotificationDto.AccountId,
                            infoToSendNotificationDto.OperationName,
                            infoToSendNotificationDto.IsPlusOperation ? BLResources.Charge : BLResources.Withdrawal,
                            infoToSendNotificationDto.Amount,
                            infoToSendNotificationDto.TransactionDate.ToShortDateString());

                        _notificationSender.PostMessage(new[] { new NotificationAddress(_ownerEmailsMap[infoToSendNotificationDto.AccountOwnerCode]) },
                                                        BLResources.AccountOperationLetterSubject,
                                                        accountOperationLetterBody);
                    }
                    else
                    {
                        _logger.Error(string.Format("Can't send notification about - payment received. Can't get to_address email. Account id: {0}. Owner code: {1}",
                                                      infoToSendNotificationDto.AccountId,
                                                      infoToSendNotificationDto.AccountOwnerCode));
                    }
                }

                scope.Complete();
            }
        }
    }
}