using System;
using System.Globalization;

using DoubleGis.Erm.BLCore.MsCRM.Plugins.ErmServiceReference;

using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;
using Microsoft.Crm.SdkTypeProxy;

namespace DoubleGis.Erm.BLCore.MsCRM.Plugins.Concrete
{
    public class UpdateAndReplicateOpportunityToErmPlugin : IPlugin
    {
        private const string ScheduledStart = "scheduledstart";
        private const string Purpose = "dg_purpose";
        private const string RegardingObjectId = "regardingobjectid";
        private const string AfterSaleType = "dg_aftersaletype";
        private static readonly ColumnSet OrdersColumns = new ColumnSet(new[] { "dg_orderid" });
        private static readonly ColumnSet DomainNameColumn = new ColumnSet(new[] { "domainname" });
        private readonly string _ermServiceAddress;

        /// <summary>
        /// Конструктор для получения настроек плагина.
        /// </summary>
        // ReSharper disable UnusedParameter.Local : Два параметра - требование CRM к конструкторам плагинов.
        public UpdateAndReplicateOpportunityToErmPlugin(string unsecure, string secure)
        // ReSharper restore UnusedParameter.Local
        {
            _ermServiceAddress = unsecure;
        }

        public void Execute(IPluginExecutionContext context)
        {
            if (_ermServiceAddress == null)
            {
                throw new Exception("Не задан адрес сервиса ERM");
            }

            if (!(context.InputParameters.Properties.Contains("Target") &&
                  context.InputParameters.Properties["Target"] is DynamicEntity))
            {
                return;
            }

            var entity = (DynamicEntity)context.InputParameters.Properties["Target"];
            if (entity.Name != EntityName.phonecall.ToString() && entity.Name != EntityName.appointment.ToString())
            {
                return;
            }

            EntityName entityName = entity.Name == EntityName.phonecall.ToString() 
                                        ? EntityName.phonecall 
                                        : EntityName.appointment;

            // Проверяем, что для каждой из сущностей обрабатываем правильные события: для звонка - это Create Update
            if (entityName == EntityName.phonecall && context.MessageName != "Create" && context.MessageName != "Update")
            {
                throw new Exception("Possible error in plugin registration. Code 0.2.");
            }

            // <rage>Для встречи - это менее интуитивные Book и Reschedule</rage>
            if (entityName == EntityName.appointment && context.MessageName != "Book" && context.MessageName != "Reschedule")
            {
                throw new Exception("Possible error in plugin registration. Code 0.1.");
            }

            TargetActivitySte targetActivity = 
                entityName == EntityName.appointment && context.MessageName == "Book"
                    ? RetrieveActivityFromDynamicEntity(entity) // Для события создания встречи нельзя зарегистрировать post entity image, но зато сама сущность приходит в плагин качестве параметра.
                    : RetrieveActivityFromPostEntityImage(context); // Для звонка и для reschedule встречи необходимые поля получаем из post entity image.
            if (targetActivity == null)
            {
                return;
            }

            // При создании звонков/встреч с типом "ППС" пропускаем обновление этапа сделки.
            if ((context.MessageName == "Create" || context.MessageName == "Book") 
                && targetActivity.AfterSaleActivityType.HasValue 
                && targetActivity.AfterSaleActivityType.Value > 0)
            {
                return;
            }

            var service = context.CreateCrmService(true);

            UpdateOpportunity(service, _ermServiceAddress, targetActivity, context.InitiatingUserId);
        }

        /// <summary>
        /// Для звонка и для reschedule встречи необходимые поля получаем из post entity image.
        /// </summary>
        private static TargetActivitySte RetrieveActivityFromPostEntityImage(IPluginExecutionContext context)
        {
            // Если этих пропертей нет, то неправильно настроены post images при регистрации плагина.
            if (!context.PostEntityImages.Contains("Target"))
            {
                throw new Exception("Possible error in plugin registration. Code 1.");
            }

            var activity = (DynamicEntity)context.PostEntityImages["Target"];

            Guid? opportunityId = null;

            if (activity.Properties.Contains(RegardingObjectId) && activity.Properties[RegardingObjectId] is Lookup)
            {
                var opportunityLookup = (Lookup)activity.Properties[RegardingObjectId];
                if (opportunityLookup.type == EntityName.opportunity.ToString())
                {
                    opportunityId = opportunityLookup.Value;
                }
            }

            if (opportunityId == null)
            {
                // Встреча не привязана к сделке.
                return null;
            }

            if (!activity.Properties.Contains(ScheduledStart))
            {
                throw new Exception("Possible error in plugin registration. Code 2.");
            }

            if (!activity.Properties.Contains(Purpose))
            {
                throw new Exception("Possible error in plugin registration. Code 3.");
            }

            DateTime scheduledStart = ((CrmDateTime)activity.Properties[ScheduledStart]).UserTime;
            int activityPurpose = ((Picklist)activity.Properties[Purpose]).Value;

            int? afterSaleServiceType = null;
            if (activity.Properties.Contains(AfterSaleType) && activity.Properties[AfterSaleType] as Picklist != null)
            {
                afterSaleServiceType = ((Picklist)activity.Properties[AfterSaleType]).Value;
            }

            return new TargetActivitySte(scheduledStart, activityPurpose, afterSaleServiceType, opportunityId.Value);
        }

        private static TargetActivitySte RetrieveActivityFromDynamicEntity(DynamicEntity appointmentDynamicEntity)
        {
            Guid? opportunityId = null;

            if (appointmentDynamicEntity.Properties.Contains(RegardingObjectId) && appointmentDynamicEntity.Properties[RegardingObjectId] is Lookup)
            {
                var opportunityLookup = (Lookup)appointmentDynamicEntity.Properties[RegardingObjectId];
                if (opportunityLookup.type == EntityName.opportunity.ToString())
                {
                    opportunityId = opportunityLookup.Value;
                }
            }

            if (opportunityId == null)
            {
                // Встреча не привязана к сделке.
                return null;
            }

            DateTime scheduledStart = ((CrmDateTime)appointmentDynamicEntity.Properties[ScheduledStart]).UserTime;
            int activityPurpose = ((Picklist)appointmentDynamicEntity.Properties[Purpose]).Value;

            int? afterSaleServiceType = null;
            if (appointmentDynamicEntity.Properties.Contains(AfterSaleType) && appointmentDynamicEntity.Properties[AfterSaleType] as Picklist != null)
            {
                afterSaleServiceType = ((Picklist)appointmentDynamicEntity.Properties[AfterSaleType]).Value;
            }

            return new TargetActivitySte(scheduledStart, activityPurpose, afterSaleServiceType, opportunityId.Value);
        }

        private static void UpdateOpportunity(ICrmService service,
            string ermServiceAddress,
            TargetActivitySte targetActivity,
            Guid userId)
        {
            int linkedOrdersCount = GetLinkedOrdersCount(service, targetActivity.OpportunityId);
            if (linkedOrdersCount > 0)
            {
                return;
            }

            // Находим более ранний (чем текущая активность) звонок.
            var earlierPhonecall = GetEarlierActivity(service, targetActivity.OpportunityId, targetActivity.StartTime, EntityName.phonecall);

            // Находим более раннюю (чем текущая активность) встречу.
            var earlierAppointment = GetEarlierActivity(service, targetActivity.OpportunityId, targetActivity.StartTime, EntityName.appointment);

            int earlierActivityPurpose;
            if (earlierPhonecall != null && earlierAppointment != null)
            {
                earlierActivityPurpose = earlierPhonecall.StartTime < earlierAppointment.StartTime
                                             ? earlierPhonecall.ActivityPurpose
                                             : earlierAppointment.ActivityPurpose;
            }
            else if (earlierPhonecall == null && earlierAppointment == null)
            {
                earlierActivityPurpose = -1;
            }
            else
            {
                earlierActivityPurpose = earlierPhonecall != null ? earlierPhonecall.ActivityPurpose : earlierAppointment.ActivityPurpose;
            }

            int dealStageInt = earlierActivityPurpose >= 0 ? earlierActivityPurpose : targetActivity.ActivityPurpose;

            if (dealStageInt >= 3 && dealStageInt <= 6)
            {
                // Конвертим значение поля "Цель" звонка\встречи в значение поля "Этап" сделки: 3,4 => 3; 5,6 => 5
                if (dealStageInt == 4 || dealStageInt == 6)
                {
                    dealStageInt--;
                }

                SendErmRequest(service, ermServiceAddress, targetActivity.OpportunityId, (MsCrmDealStage)dealStageInt, userId);
            }
        }

        private static int GetLinkedOrdersCount(ICrmService service, Guid opportunityId)
        {
            var expression = new QueryExpression("dg_order");

            var condition1 = new ConditionExpression
            {
                AttributeName = "dg_opportunityid",
                Values = new object[] { opportunityId },
                Operator = ConditionOperator.Equal
            };

            var condition2 = new ConditionExpression
            {
                AttributeName = "statecode",
                Values = new object[] { 0 },
                Operator = ConditionOperator.Equal
            };

            var filter = new FilterExpression { FilterOperator = LogicalOperator.And };
            filter.Conditions.Add(condition1);
            filter.Conditions.Add(condition2);

            expression.Criteria.AddFilter(filter);

            expression.ColumnSet = OrdersColumns;
            expression.PageInfo.PageNumber = 1;
            expression.PageInfo.Count = 1;

            var result = RetrieveMultipleDynamic(service, expression);

            if (result == null)
            {
                return 0;
            }

            return result.BusinessEntities.Count;
        }

        private static ActivitySte GetEarlierActivity(ICrmService service, Guid opportunityId, DateTime scheduledStart, EntityName entity)
        {
            if (entity != EntityName.appointment && entity != EntityName.phonecall)
            {
                throw new ArgumentException("entity");
            }

            var expression = new QueryExpression(entity.ToString());
            var regardingCondition = new ConditionExpression
            {
                AttributeName = RegardingObjectId,
                Operator = ConditionOperator.Equal,
                Values = new object[] { opportunityId }
            };

            var stateCondition = new ConditionExpression
            {
                AttributeName = "statecode",
                Operator = ConditionOperator.In,
                Values = new object[] { (int)AppointmentState.Scheduled, (int)AppointmentState.Open }
            };

            var startCondition = new ConditionExpression
            {
                AttributeName = ScheduledStart,
                Operator = ConditionOperator.LessThan,
                Values = new object[] { scheduledStart.ToString(CultureInfo.InvariantCulture) }
            };

            var filter = new FilterExpression { FilterOperator = LogicalOperator.And };
            filter.Conditions.Add(regardingCondition);
            filter.Conditions.Add(stateCondition);
            filter.Conditions.Add(startCondition);

            expression.Criteria.AddFilter(filter);
            expression.AddOrder(ScheduledStart, OrderType.Ascending);
            expression.PageInfo.Count = 1;
            expression.PageInfo.PageNumber = 1;
            expression.ColumnSet = new ColumnSet(new[] { Purpose, ScheduledStart });

            var result = RetrieveMultipleDynamic(service, expression);
            if (result.BusinessEntities.Count == 0)
            {
                return null;
            }

            var dynamic = (DynamicEntity)result.BusinessEntities[0];
            return new ActivitySte(((CrmDateTime)dynamic.Properties[ScheduledStart]).UserTime, ((Picklist)dynamic.Properties[Purpose]).Value);
        }

        private static void SendErmRequest(ICrmService service, string ermServiceAddress, Guid opportunityId, MsCrmDealStage dealStage, Guid userId)
        {
            // do not use GetPropertyValue<DealStage> - wrong casting will occur
            if (!(dealStage == MsCrmDealStage.CollectInformation
                  || dealStage == MsCrmDealStage.HoldingProductPresentation
                  || dealStage == MsCrmDealStage.MatchAndSendProposition))
            {
                return;
            }
            
            string userDomainName = GetUserDomainName(service, userId) ?? string.Empty;
            ErmWcfServiceHelper.SendRequest(ermServiceAddress, x => x.ReplicateDealStage(opportunityId, dealStage, userDomainName));
        }

        private static string GetUserDomainName(ICrmService service, Guid userId)
        {
            var result = service.Retrieve(EntityName.systemuser.ToString(), userId, DomainNameColumn);
            if (result == null || !(result is systemuser))
            {
                return null;
            }

            var user = (systemuser)result;
            return user.domainname;
        }

        /// <summary>
        /// В отличие от вызова service.RetrieveMultiple, возвращает DynamicEntities, а не фиксированные сущности.
        /// </summary>
        private static BusinessEntityCollection RetrieveMultipleDynamic(ICrmService service, QueryBase query)
        {
            // Create the request.
            var request = new RetrieveMultipleRequest
            {
                Query = query,
                ReturnDynamicEntities = true
            };

            // Set the request properties.

            // Execute the request.
            var result = (RetrieveMultipleResponse)service.Execute(request);
            return result.BusinessEntityCollection;
        }

        private class ActivitySte
        {
            public ActivitySte(DateTime startTime, int purpose)
            {
                StartTime = startTime;
                ActivityPurpose = purpose;
            }

            public DateTime StartTime { get; private set; }
            public int ActivityPurpose { get; private set; }
        }

        private class TargetActivitySte : ActivitySte
        {
            public TargetActivitySte(DateTime startTime, int purpose, int? afterSaleActivityType, Guid opportunityId)
                : base(startTime, purpose)
            {
                AfterSaleActivityType = afterSaleActivityType;
                OpportunityId = opportunityId;
            }

            public Guid OpportunityId { get; private set; }
            public int? AfterSaleActivityType { get; private set; }
        }
    }
}
