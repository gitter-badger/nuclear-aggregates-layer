using System;
using System.Transactions;

using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Notifications;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.Platform.Core.Notifications
{
    public class NotificationSender : INotificationSender
    {
        private readonly IRepository<NotificationEmails> _emailEntityRepository;
        private readonly IRepository<NotificationAddresses> _addressEntityRepository;
        private readonly IRepository<NotificationEmailsTo> _emailsToEntityRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IIdentityProvider _identityProvider;
        private readonly ICommonLog _commonLog;

        public NotificationSender(ICommonLog commonLog,
                                  IIdentityProvider identityProvider,
                                  IRepository<NotificationEmails> emailEntityRepository,
                                  IRepository<NotificationAddresses> addressEntityRepository,
                                  IRepository<NotificationEmailsTo> emailsToEntityRepository,
                                  IOperationScopeFactory scopeFactory)
        {
            _commonLog = commonLog;
            _identityProvider = identityProvider;
            _emailEntityRepository = emailEntityRepository;
            _addressEntityRepository = addressEntityRepository;
            _emailsToEntityRepository = emailsToEntityRepository;
            _scopeFactory = scopeFactory;
        }

        public void PostMessage(NotificationAddress[] to, string subject, string body)
        {
            PostMessage(to, subject, new NotificationBody { Body = body, IsHtml = false });
        }

        public void PostMessage(NotificationAddress[] to, String subject, NotificationBody body)
        {
            var toAddresses = new NotificationAddresses[to.Length];

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                using (var operationScope = _scopeFactory.CreateSpecificFor<CreateIdentity>(EntityName.NotificationAddress))
                {
                    // добавляем адреса получателей сообщения, пока используем не как справочник, поэтому просто делаем вставку
                    for (var i = 0; i < to.Length; i++)
                    {
                        NotificationAddresses currentAddress;
                        try
                        {
                            currentAddress = new NotificationAddresses
                                {
                                    Address = to[i].Address,
                                    DisplayName = to[i].DisplayName,
                                    DisplayNameEncoding = to[i].DisplayNameEncoding,
                                    IsActive = true
                                };
                            _identityProvider.SetFor(currentAddress);
                            _addressEntityRepository.Add(currentAddress);
                            _addressEntityRepository.Save();

                            operationScope.Added<NotificationAddresses>(currentAddress.Id);
                        }
                        catch (Exception ex)
                        {
                            _commonLog.ErrorEx(ex, "Can't add NotificationAddresses. " + to[i].Address);
                            throw;
                        }

                        toAddresses[i] = currentAddress;
                    }

                    operationScope.Complete();
                }

                NotificationEmails currentEmail;
                using (var operationScope = _scopeFactory.CreateSpecificFor<CreateIdentity>(EntityName.NotificationEmail))
                {
                    try
                    {
                        // создаем запись для самого email
                        currentEmail = new NotificationEmails { Subject = subject, Body = body.Body, IsBodyHtml = body.IsHtml, IsActive = true };

                        _identityProvider.SetFor(currentEmail);
                        _emailEntityRepository.Add(currentEmail);
                        _emailEntityRepository.Save();

                        operationScope.Added<NotificationEmails>(currentEmail.Id);
                    }
                    catch (Exception ex)
                    {
                        _commonLog.ErrorEx(ex, "Can't add NotificationEmails.");
                        throw;
                    }

                    operationScope.Complete();
                }

                using (var operationScope = _scopeFactory.CreateSpecificFor<CreateIdentity>(EntityName.NotificationEmailTo))
                {
                    foreach (var currentAddress in toAddresses)
                    {
                        try
                        {
                            // создаем записи для получателей сообщения
                            var currentEmailTo = new NotificationEmailsTo
                                {
                                    EmailId = currentEmail.Id,
                                    AddressId = currentAddress.Id,
                                    IsActive = true
                                };

                            _identityProvider.SetFor(currentEmailTo);
                            _emailsToEntityRepository.Add(currentEmailTo);
                            _emailsToEntityRepository.Save();

                            operationScope.Added<NotificationEmailsTo>(currentEmailTo.Id);
                        }
                        catch (Exception ex)
                        {
                            _commonLog.ErrorEx(ex, "Can't add NotificationEmailsTo. EmailId=" + currentEmail.Id + ". AddressId=" + currentAddress.Id);
                            throw;
                        }
                    }

                    operationScope.Complete();
                }

                transaction.Complete();
            }
        }

        public void PostMessage(NotificationAddress sender,
                                NotificationAddress[] to,
                                NotificationAddress[] cc,
                                string subject,
                                string body,
                                bool isBodyHtml,
                                NotificationAttachment[] attachments)
        {
            throw new NotImplementedException();
        }
    }
}
