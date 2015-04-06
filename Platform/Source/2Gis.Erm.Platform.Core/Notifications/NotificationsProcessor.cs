using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Transactions;

using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Notifications;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.Platform.Core.Notifications
{
    public class NotificationsProcessor : INotificationsProcessor
    {
        private const int EmailsBatchSize = 50;

        private readonly NotificationAddress _defaultSender;
        private readonly MailSenderAuthenticationSettings _authenticationSettings;
        private readonly string _smtpServerHost;

        private readonly IFinder _finder;
        private readonly IRepository<NotificationProcessings> _processingsEntityRepository;
        private readonly ITracer _tracer;
        private readonly IIdentityProvider _identityProvider;

        public NotificationsProcessor(
            INotificationProcessingSettings notificationProcessingSettings,
            IFinder finder,                            
            IRepository<NotificationProcessings> processingsEntityRepository,
            ITracer tracer, 
            IIdentityProvider identityProvider)
        {
            _defaultSender = notificationProcessingSettings.DefaultSender;
            _authenticationSettings = notificationProcessingSettings.AuthenticationSettings;
            _smtpServerHost = notificationProcessingSettings.SmtpServerHost;
            _finder = finder;
            _processingsEntityRepository = processingsEntityRepository;
            _tracer = tracer;
            _identityProvider = identityProvider;
        }

        public void Process()
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Suppress, DefaultTransactionOptions.Default))
            {
                using (var smtpClient = new SmtpClient(_smtpServerHost))
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    switch (_authenticationSettings.AuthenticationType)
                    {
                        case MailSenderAuthenticationType.Anonymous:
                        {
                            smtpClient.UseDefaultCredentials = false;
                            smtpClient.Credentials = null;
                            break;
                        }
                        case MailSenderAuthenticationType.WindowsAuthentication:
                        {
                            smtpClient.UseDefaultCredentials = true;
                            smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials;
                            break;
                        }
                        case MailSenderAuthenticationType.ClearText:
                        {
                            smtpClient.UseDefaultCredentials = false;
                            smtpClient.Credentials = new NetworkCredential(_authenticationSettings.UserName, _authenticationSettings.UserPass);
                            break;
                        }
                    }

                    foreach (var email in GetEmailsForProcessing())
                    {
                        ProcessEmail(smtpClient, email);
                    }
                }

                scope.Complete();
            }
        }

        private IEnumerable<EmailDescriptor> GetEmailsForProcessing()
        {
            var currentTime = DateTime.UtcNow;
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<NotificationEmails>())
                          .Select(email => new
                              {
                                  Email = email,
                                  Sender = email.Sender,
                                  ToAddresses =
                                               email.NotificationEmailsTo
                                                    .Where(e => e.IsActive && !e.IsDeleted)
                                                    .Select(e => e.NotificationAddress)
                                                    .Where(a => a.IsActive && !a.IsDeleted),
                                  CcAddresses =
                                               email.NotificationEmailsCc
                                                    .Where(e => e.IsActive && !e.IsDeleted)
                                                    .Select(e => e.NotificationAddress)
                                                    .Where(a => a.IsActive && !a.IsDeleted),
                                  Attachments = email.NotificationEmailsAttachments.Where(e => e.IsActive && !e.IsDeleted).Select(e => e.Files),
                                  LastProcessing =
                                               email.NotificationProcessings
                                                    .Where(p => p.IsActive && !p.IsDeleted)
                                                    .OrderByDescending(p => p.Id)
                                                    .FirstOrDefault()
                              })
                          .Where(entry =>
                                 (entry.LastProcessing == null || entry.LastProcessing.Status == (int)NotificationStatus.Error) &&
                                 (!entry.Email.MaxAttemptsCount.HasValue ||
                                  (entry.Email.MaxAttemptsCount.HasValue && entry.Email.MaxAttemptsCount.Value > 0 &&
                                   entry.LastProcessing.AttemptsCount < entry.Email.MaxAttemptsCount.Value)) &&
                                 (!entry.Email.ExpirationTime.HasValue ||
                                  (entry.Email.ExpirationTime.HasValue && currentTime <= entry.Email.ExpirationTime.Value)))
                          .Select(entry => new EmailDescriptor
                              {
                                  Email = entry.Email,
                                  Sender = entry.Sender,
                                  ToAddresses = entry.ToAddresses,
                                  CcAddresses = entry.CcAddresses,
                                  Attachments = entry.Attachments,
                                  AttemptCount = entry.LastProcessing == null ? 0 : entry.LastProcessing.AttemptsCount
                              })
                          .OrderBy(e => e.Email.ModifiedOn)
                          .Take(EmailsBatchSize)
                          .ToArray();
        }

        private void ProcessEmail(SmtpClient smtpClient, EmailDescriptor processingEmail)
        {
            var processings = new NotificationProcessings
            {
                AttemptsCount = processingEmail.AttemptCount + 1,
                EmailId = processingEmail.Email.Id,
                Status = (int)NotificationStatus.Processing,
                IsActive = true
            };

            _identityProvider.SetFor(processings);
            _processingsEntityRepository.Add(processings);
            _processingsEntityRepository.Save();

            var message = new MailMessage();
            Encoding encoding;
            message.Subject = processingEmail.Email.Subject;
            if (TryGetEncoding(processingEmail.Email.SubjectEncoding, out encoding))
            {
                message.SubjectEncoding = encoding;
            }

            message.Body = processingEmail.Email.Body;
            if (TryGetEncoding(processingEmail.Email.BodyEncoding, out encoding))
            {
                message.BodyEncoding = encoding;
            }

            message.IsBodyHtml = processingEmail.Email.IsBodyHtml;

            FillSenderAndFromProperties(message, processingEmail.Sender);
            FillAddressesProperty(message, processingEmail.ToAddresses);
            FillAddressesProperty(message, processingEmail.CcAddresses);
            ProcessAttachments(message, processingEmail.Attachments);

            try
            {
                smtpClient.Send(message);
                var notificationProcessings = new NotificationProcessings
                {
                    AttemptsCount = processingEmail.AttemptCount + 1,
                    EmailId = processingEmail.Email.Id,
                    Status = (int)NotificationStatus.Sended,
                    IsActive = true
                };
                _identityProvider.SetFor(notificationProcessings);
                _processingsEntityRepository.Add(notificationProcessings);
                _processingsEntityRepository.Save();
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, "Can't send notification message");
                var notificationProcessings = new NotificationProcessings
                {
                    AttemptsCount = processingEmail.AttemptCount + 1,
                    EmailId = processingEmail.Email.Id,
                    Status = (int)NotificationStatus.Error,
                    Description = "Can't send notification message. " + ex.Message,
                    IsActive = true
                };

                _identityProvider.SetFor(notificationProcessings);
                _processingsEntityRepository.Add(notificationProcessings);
                _processingsEntityRepository.Save();
            }
        }

        private void ProcessAttachments(MailMessage message, IEnumerable<File> attachments)
        {
            if (attachments == null)
            {
                return;
            }

            foreach (var attachmentDescriptor in attachments)
            {
                // пока вложения не поддерживаем, т.к. нет таких требований
            }
        }

        private bool TryGetEncoding(string encodingString, out Encoding encoding)
        {
            encoding = null;

            if (string.IsNullOrEmpty(encodingString))
            {
                return false;
            }
        
            try
            {
                encoding = Encoding.GetEncoding(encodingString);
                return true;
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, "Can't get encoding, using default");
                return false;
            }
        }
    
        private void FillSenderAndFromProperties(MailMessage message, NotificationAddresses sender)
        {
            if (sender != null)
            {
                if (string.IsNullOrEmpty(sender.DisplayNameEncoding))
                {
                    message.Sender = new MailAddress(sender.Address, sender.DisplayName);
                    message.From = new MailAddress(sender.Address, sender.DisplayName);
                }
                else
                {
                    Encoding encoding;
                    try
                    {
                        encoding = Encoding.GetEncoding(sender.DisplayNameEncoding);
                    }
                    catch (Exception ex)
                    {
                        _tracer.Error(ex, "Can't get encoding, using default");
                        encoding = Encoding.Default;
                    }

                    message.Sender = new MailAddress(sender.Address, sender.DisplayName, encoding);
                    message.From = new MailAddress(sender.Address, sender.DisplayName, encoding);
                }
            }
            else
            {
                message.Sender = new MailAddress(_defaultSender.Address, _defaultSender.DisplayName);
                message.From = new MailAddress(_defaultSender.Address, _defaultSender.DisplayName);
            }
        }

        private void FillAddressesProperty(MailMessage message, IEnumerable<NotificationAddresses> addresses)
        {
            if (addresses == null)
            {
                return;
            }

            foreach (var addr in addresses)
            {
                if (string.IsNullOrEmpty(addr.DisplayNameEncoding))
                {
                    message.To.Add(new MailAddress(addr.Address, addr.DisplayName));
                }
                else
                {
                    Encoding encoding;
                    try
                    {
                        encoding = Encoding.GetEncoding(addr.DisplayNameEncoding);
                    }
                    catch (Exception ex)
                    {
                        _tracer.Error(ex, "Can't get encoding, using default");
                        encoding = Encoding.Default;
                    }

                    message.To.Add(new MailAddress(addr.Address, addr.DisplayName, encoding));
                }
            }
        }

        private class EmailDescriptor
        {
            public int AttemptCount { get; set; }
            public NotificationEmails Email { get; set; }
            public NotificationAddresses Sender { get; set; }
            public IEnumerable<NotificationAddresses> ToAddresses { get; set; }
            public IEnumerable<NotificationAddresses> CcAddresses { get; set; }
            public IEnumerable<File> Attachments { get; set; }
        }
    }
}
