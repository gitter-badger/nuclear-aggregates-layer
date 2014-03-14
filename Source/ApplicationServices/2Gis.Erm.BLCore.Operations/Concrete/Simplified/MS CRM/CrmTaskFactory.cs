using System;
using System.Globalization;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.MsCRM;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.MsCRM.Dto;
using DoubleGis.Erm.BLCore.Common.Infrastructure.MsCRM;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;

using Microsoft.Crm.Sdk;
using Microsoft.Crm.SdkTypeProxy;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Simplified
{
    // FIXME {all, 01.01.2014}: уже есть типы с подобным функционалом, например, CrmDataContextExtensions, данный тип скорее должен быть таким же набором методов расширений, своего состояния у него фактически нет 
    [Obsolete]
    public sealed class CrmTaskFactory : ICrmTaskFactory
    {
        private const string MsCrmDateTimeFormat = "yyyy/MM/ddTHH:mm:ssZ";
        private readonly IMsCrmSettings _msCrmSettings;
        private readonly IMsCrmInteractionService _crmDynamicsInteractionService;

        public CrmTaskFactory(IMsCrmSettings msCrmSettings, IMsCrmInteractionService crmDynamicsInteractionService)
        {
            _msCrmSettings = msCrmSettings;
            _crmDynamicsInteractionService = crmDynamicsInteractionService;
        }

        public Guid CreateTask(UserDto owner, HotClientRequestDto hotClient, RegardingObject regardingObject)
        {
            try
            {
                var crmDataContext = _msCrmSettings.CreateDataContext();

                var crmOwner = _crmDynamicsInteractionService.GetCrmUserInfo(owner.Account);
                if (crmOwner == null || crmOwner.CrmUserId == Guid.Empty)
                {
                    throw new ErmCommunicationException(string.Format("Не удалось найти в Dynamics пользователя Id = {0}, Account = {1}", owner.Id, owner.Account));
                }

                var entity = crmDataContext.CreateEntity(EntityName.task.ToString());
                var description = string.Format(BLResources.HotClientDescriptionTemplate, hotClient.ContactPhone, hotClient.ContactName);
                if (!string.IsNullOrWhiteSpace(hotClient.Description))
                {
                    description += Environment.NewLine + hotClient.Description;
                }
                
                var activityStartTime = hotClient.CreationDate + crmOwner.TimeZoneTotalBias;
                var activityEndTime = activityStartTime + TimeSpan.FromDays(1);

                entity.SetPropertyValue("scheduledstart", new CrmDateTime(activityStartTime.ToString(MsCrmDateTimeFormat, CultureInfo.InvariantCulture)));
                entity.SetPropertyValue("scheduledend", new CrmDateTime(activityEndTime.ToString(MsCrmDateTimeFormat, CultureInfo.InvariantCulture)));

                entity.SetPropertyValue("actualdurationminutes", 60);
                entity.SetPropertyValue("scheduleddurationminutes", 60);

                entity.SetPropertyValue("dg_type", new Picklist(4));
                entity.SetPropertyValue("subject", BLResources.HotClientSubject);
                entity.SetPropertyValue("description", description);

                entity.SetPropertyValue("createdon", CrmDateTime.Now);
                entity.SetPropertyValue("modifiedon", CrmDateTime.Now);

                entity.SetPropertyValue("ownerid", new Owner(EntityName.systemuser.ToString(), crmOwner.CrmUserId));

                // В отношении
                if (regardingObject != null)
                {
                    entity.SetPropertyValue("regardingobjectid", new Lookup(regardingObject.EntityName, regardingObject.ReplicationCode));
                }

                crmDataContext.AddObject(EntityName.task.ToString(), entity);
                crmDataContext.SaveChanges();
                return entity.Id.Value;
            }
            catch (Exception ex)
            {
                var message = string.Format("Ошибка при репликации сущности {0} с идентификатором {1}",
                                            "HotClientRequest",
                                            hotClient.Id);
                throw new ErmCommunicationException(message, ex);
            }
        }
    }
}
