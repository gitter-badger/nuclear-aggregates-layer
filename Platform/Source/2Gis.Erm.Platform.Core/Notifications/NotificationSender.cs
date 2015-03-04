using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Notifications;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.Platform.Core.Notifications
{
    public class NotificationSender : INotificationSender
    {
        private readonly IRepository<NotificationEmails> _emailEntityRepository;
        private readonly IRepository<NotificationAddresses> _addressEntityRepository;
        private readonly IRepository<NotificationEmailsTo> _emailsToEntityRepository;
        private readonly IRepository<NotificationEmailsCc> _emailsCcEntityRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICommonLog _logger;
        private readonly IIdentityProvider _identityProvider;

        public NotificationSender(IRepository<NotificationEmails> emailEntityRepository,
                                  IRepository<NotificationAddresses> addressEntityRepository,
                                  IRepository<NotificationEmailsTo> emailsToEntityRepository,
                                  IRepository<NotificationEmailsCc> emailsCcEntityRepository,
                                  IIdentityProvider identityProvider,
                                  IOperationScopeFactory scopeFactory,
                                  ICommonLog logger)
        {
            _identityProvider = identityProvider;
            _emailEntityRepository = emailEntityRepository;
            _addressEntityRepository = addressEntityRepository;
            _emailsToEntityRepository = emailsToEntityRepository;
            _emailsCcEntityRepository = emailsCcEntityRepository;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        void INotificationSender.PostMessage(NotificationAddress[] to, string subject, string body)
        {
            PostMessage(null, to, Enumerable.Empty<NotificationAddress>(), subject, body, false, null);
        }

        void INotificationSender.PostMessage(NotificationAddress[] to, String subject, NotificationBody body)
        {
            PostMessage(null, to, Enumerable.Empty<NotificationAddress>(), subject, body.Body, body.IsHtml, null);
        }

        void INotificationSender.PostMessage(NotificationAddress sender,
                                NotificationAddress[] to,
                                NotificationAddress[] cc,
                                string subject,
                                string body,
                                bool isBodyHtml,
                                NotificationAttachment[] attachments)
        {
            PostMessage(sender, to, cc, subject, body, isBodyHtml, attachments);
        }

        private void PostMessage(NotificationAddress sender,
                                 IEnumerable<NotificationAddress> to,
                                 IEnumerable<NotificationAddress> cc,
                                 string subject,
                                 string body,
                                 bool isBodyHtml,
                                 NotificationAttachment[] attachments)
        {
            if (attachments != null && attachments.Any())
            {
                throw new NotSupportedException("Sending emails with attacments is not supported yet");
            }

            try
            {
                using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, NotificationEmails>())
                {
                    var emailMessage = new NotificationEmails { Subject = subject, Body = body, IsBodyHtml = isBodyHtml, IsActive = true };
                    if (sender != null)
                    {
                        var senderAddress = new NotificationAddresses
                            {
                                Address = sender.Address,
                                DisplayName = sender.DisplayName,
                                DisplayNameEncoding = sender.DisplayNameEncoding,
                                IsActive = true
                            };
                        _identityProvider.SetFor(senderAddress);
                        _addressEntityRepository.Add(senderAddress);
                        scope.Added<NotificationAddresses>(senderAddress.Id);

                        emailMessage.SenderId = senderAddress.Id;
                    }

                    _identityProvider.SetFor(emailMessage);
                    _emailEntityRepository.Add(emailMessage);
                    scope.Added<NotificationEmails>(emailMessage.Id);

                    ProcessAddresses(scope, emailMessage, to, cc ?? Enumerable.Empty<NotificationAddress>());

                    _addressEntityRepository.Save();
                    _emailEntityRepository.Save();
                    _emailsToEntityRepository.Save();
                    _emailsCcEntityRepository.Save();

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Can't send notifications");
                throw;
            }
        }

        private void ProcessAddresses(IOperationScope scope, NotificationEmails processingEmail, IEnumerable<NotificationAddress> to, IEnumerable<NotificationAddress> cc)
        {
            foreach (var toAddress in to)
            {
                var mapping = new NotificationEmailsTo
                {
                    EmailId = processingEmail.Id,
                    AddressId = PrepareAddress(scope, toAddress).Id,
                    IsActive = true
                };

                _identityProvider.SetFor(mapping);
                _emailsToEntityRepository.Add(mapping);
                scope.Added<NotificationEmailsTo>(mapping.Id);
            }

            foreach (var ccAddress in cc)
            {
                var mapping = new NotificationEmailsCc
                {
                    EmailId = processingEmail.Id,
                    AddressId = PrepareAddress(scope, ccAddress).Id,
                    IsActive = true
                };

                _identityProvider.SetFor(mapping);
                _emailsCcEntityRepository.Add(mapping);
                scope.Added<NotificationEmailsCc>(mapping.Id);
            }
        }

        private NotificationAddresses PrepareAddress(IOperationScope scope, NotificationAddress address)
        {
            var preparedAddress = new NotificationAddresses
            {
                Address = address.Address,
                DisplayName = address.DisplayName,
                DisplayNameEncoding = address.DisplayNameEncoding,
                IsActive = true
            };

            _identityProvider.SetFor(preparedAddress);
            _addressEntityRepository.Add(preparedAddress);
            scope.Added<NotificationAddresses>(preparedAddress.Id);

            return preparedAddress;
        }
    }
}
