using System;

using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Common.Infrastructure.MsCRM;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.CrmActivities;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security;

using Microsoft.Crm.SdkTypeProxy;

using Response = DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse.Response;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.UserOperations
{
    public class UpdateUserCrmRelatedEntitiesHandler : RequestHandler<UpdateUserCrmRelatedEntitiesRequest, EmptyResponse>
    {
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly IMsCrmSettings _msCrmSettings;
        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;
        public UpdateUserCrmRelatedEntitiesHandler(
            ISubRequestProcessor subRequestProcessor, 
            IMsCrmSettings msCrmSettings, 
            ISecurityServiceUserIdentifier securityServiceUserIdentifier)
        {
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
            _subRequestProcessor = subRequestProcessor;
            _msCrmSettings = msCrmSettings;
        }

        protected override EmptyResponse Handle(UpdateUserCrmRelatedEntitiesRequest request)
        {
            // Дальше обновляем все действия пользователя
            var crmDataContext = _msCrmSettings.CreateDataContext();

            Guid fromUserCode;
            var fromUserDomainAccount = _securityServiceUserIdentifier.GetUserInfo(request.DeactivatedUserCode).Account;
            try
            {
                fromUserCode = crmDataContext.GetSystemUserByDomainName(fromUserDomainAccount, false).UserId;
            }
            catch (Exception ex)
            {
                throw new NotificationException(string.Format(BLResources.Error_DynamicsCrmUserNotFound, fromUserDomainAccount ?? string.Empty), ex);
            }  

            Guid toUserCode;
            var toUserDomainAccount = _securityServiceUserIdentifier.GetUserInfo(request.AssignedUserCode).Account;
            try
            {
                toUserCode = crmDataContext.GetSystemUserByDomainName(toUserDomainAccount, false).UserId;
            }
            catch (Exception ex)
            {
                throw new NotificationException(string.Format(BLResources.Error_DynamicsCrmUserNotFound, toUserDomainAccount ?? string.Empty), ex);
            }

            _subRequestProcessor.HandleSubRequest(new UpdateRelatedCrmActivitiesRequest
                {
                    CrmFromObjectCode = fromUserCode,
                    CrmToObjectCode = toUserCode,
                    CrmObjectType = EntityName.systemuser
                }, Context);

            return Response.Empty;
        }
    }
    public class UpdateUserCrmRelatedEntitiesRequest : Platform.API.Core.Operations.RequestResponse.Request
    {
        public long AssignedUserCode { get; set; }
        public long DeactivatedUserCode { get; set; }
    }
}
