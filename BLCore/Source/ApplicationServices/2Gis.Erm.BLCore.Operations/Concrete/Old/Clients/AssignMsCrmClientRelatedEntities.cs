using System;
using System.Linq;
using System.Net;

using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Common.Infrastructure.MsCRM;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using Microsoft.Crm.Sdk;
using Microsoft.Crm.SdkTypeProxy;
using Microsoft.Xrm.Client.Data.Services;

using Response = DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse.Response;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Clients
{
    public sealed class AssignClientRelatedEntitiesRequest : Platform.API.Core.Operations.RequestResponse.Request
    {
        public long ClientId { get; set; }
        public long OwnerCode { get; set; }
        public bool IsPartial { get; set; }
    }

    public sealed class AssignClientRelatedEntitiesHandler : RequestHandler<AssignClientRelatedEntitiesRequest, EmptyResponse>
    {
        private readonly IMsCrmSettings _msCrmSettings;
        private readonly ISecurityServiceUserIdentifier _securityService;
        private readonly ISecureFinder _finder;

        public AssignClientRelatedEntitiesHandler(IMsCrmSettings msCrmSettings, ISecurityServiceUserIdentifier securityService, ISecureFinder finder)
        {
            _msCrmSettings = msCrmSettings;
            _securityService = securityService;
            _finder = finder;
        }

        protected override EmptyResponse Handle(AssignClientRelatedEntitiesRequest request)
        {
            if (_msCrmSettings.EnableReplication)
            {
                var clientInfo = _finder.Find(Specs.Find.ById<Client>(request.ClientId))
                    .Select(x => new { ClientReplicationCode = x.ReplicationCode, ClientOwner = x.OwnerCode })
                    .Single();
                try
                {
                    var crmDataContext = _msCrmSettings.CreateDataContext();

                    Guid newUserReplicationCode = GetUserReplicationCode(crmDataContext, request.OwnerCode);
                    Guid? oldUserReplicationCode = request.IsPartial ? GetUserReplicationCode(crmDataContext, clientInfo.ClientOwner) : (Guid?)null;

                    var assignee = new SecurityPrincipal { PrincipalId = newUserReplicationCode, Type = SecurityPrincipalType.User };

                    var crmClient = crmDataContext.GetEntities(EntityName.account).SingleOrDefault(x => x.GetPropertyValue<Guid>("accountid") == clientInfo.ClientReplicationCode);
                    if (crmClient != null)
                    {
                        var activityHelper = new ActivityHelper(crmDataContext, clientInfo.ClientReplicationCode, oldUserReplicationCode, assignee);
                        // У задачи только одно поле привязки к клиенту - "В отношении"
                        activityHelper.AssignLinkedActivities(EntityName.task, new[] { (Int32)TaskState.Open }, null, x => new TargetOwnedTask() { EntityId = x });

                        // Факс может быть привязан к клиенту через поле "В отношении" и через поле "Получатель"
                        activityHelper.AssignLinkedActivities(EntityName.fax,
                            new[] { (Int32)FaxState.Open }, new[] { ParticipationType.Recipient },
                            x => new TargetOwnedFax() { EntityId = x });

                        // Звонок может быть привязан к клиенту через поле "В отношении" и через поле "Получатель"
                        activityHelper.AssignLinkedActivities(EntityName.phonecall,
                            new[] { (Int32)PhoneCallState.Open }, new[] { ParticipationType.Recipient },
                            x => new TargetOwnedPhoneCall() { EntityId = x });

                        // Email может быть привязан к клиенту через поле "В отношении" и через поле "Получатель"
                        activityHelper.AssignLinkedActivities(EntityName.email,
                            new[] { (Int32)EmailState.Open }, new[] { ParticipationType.Recipient },
                            x => new TargetOwnedEmail() { EntityId = x });

                        // Письмо может быть привязано к клиенту через поле "В отношении" и через поле "Получатель"
                        activityHelper.AssignLinkedActivities(EntityName.letter,
                            new[] { (Int32)LetterState.Open }, new[] { ParticipationType.Recipient },
                            x => new TargetOwnedLetter() { EntityId = x });

                        // Встреча может быть привязана к клиенту через поле "В отношении" и через поле "Обязательно"
                        activityHelper.AssignLinkedActivities(EntityName.appointment,
                            new[] { (Int32)AppointmentState.Open, (Int32)AppointmentState.Scheduled },
                            new[] { ParticipationType.RequiredAttendee }, x => new TargetOwnedAppointment() { EntityId = x });

                        // Сервисную встречу теперь нельзя создать из UI. Рано или поздно следует удалить это код.
                        activityHelper.AssignLinkedActivities(EntityName.serviceappointment,
                            new[] { (Int32)ServiceAppointmentState.Open, (Int32)ServiceAppointmentState.Scheduled },
                            new[] { ParticipationType.RequiredAttendee }, x => new TargetOwnedServiceAppointment() { EntityId = x });
                    }
                }
                catch (WebException ex)
                {
                    throw new NotificationException(BLResources.Errors_DynamicsCrmConectionFailed, ex);
                }
            }

            return Response.Empty;
        }


        private Guid GetUserReplicationCode(CrmDataContext crmDataContext, long ownerCode)
        {
            Guid userReplicationCode;
            var userDomainAccount = _securityService.GetUserInfo(ownerCode).Account;
            try
            {
                userReplicationCode = crmDataContext.GetSystemUserByDomainName(userDomainAccount, true).UserId;
            }
            catch (Exception ex)
            {
                throw new NotificationException(string.Format(BLResources.Error_DynamicsCrmUserNotFound, userDomainAccount ?? string.Empty), ex);
            }

            return userReplicationCode;
        }
    }
}
