using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;
using Microsoft.Crm.SdkTypeProxy;
using Microsoft.Xrm.Client.Data.Services;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Clients
{
    public class ActivityHelper
    {
        private readonly ICrmDataContext _crmDataContext;
        private readonly Guid _clientReplicationCode;
        private readonly Guid? _oldUserReplicationCode;
        private readonly SecurityPrincipal _assignee;

        public ActivityHelper(
            ICrmDataContext crmDataContext,
            Guid clientReplicationCode,
            Guid? oldUserReplicationCode,
            SecurityPrincipal assignee)
        {
            _crmDataContext = crmDataContext;
            _clientReplicationCode = clientReplicationCode;
            _oldUserReplicationCode = oldUserReplicationCode;
            _assignee = assignee;
        }

        public void AssignLinkedActivities(
            EntityName entityName,
            IEnumerable<int> allowedStateCodes,
            IEnumerable<int> participationTypes,
            Func<Guid, TargetOwned> targetCreator)
        {
            var activitiesIds = RetrieveAllLinkedActivities(_crmDataContext,
                                                            _clientReplicationCode,
                                                            entityName,
                                                            allowedStateCodes,
                                                            participationTypes,
                                                            _oldUserReplicationCode);
            foreach (var activityId in activitiesIds)
            {
                var asr = new AssignRequest
                {
                    Assignee = _assignee,
                    Target = targetCreator(activityId)
                };
                _crmDataContext.UsingService(service => service.Execute(asr));
            }
        }

        /// <summary>
        /// Возвращаются действия, привязанные к клиенту по полю "В отношении" + привязанные через указанные в аргументе participationTypes поля.
        /// </summary>
        public static IEnumerable<Guid> RetrieveAllLinkedActivities(
            ICrmDataContext dataContext,
            Guid entityReplicationCode,
            EntityName activityType,
            IEnumerable<int> allowedStateCodes,
            IEnumerable<int> participationTypes,
            Guid? oldOwner)
        {
            var result = RetrieveLinkedActivities(dataContext, entityReplicationCode, activityType, allowedStateCodes, null, oldOwner);
            if (participationTypes != null)
            {
                result = participationTypes.Aggregate(result,
                                                      (current, participationType) =>
                                                      current.Union(RetrieveLinkedActivities(dataContext,
                                                                                             entityReplicationCode,
                                                                                             activityType,
                                                                                             allowedStateCodes,
                                                                                             participationType,
                                                                                             oldOwner)));
            }

            return result;
        }

        private static IEnumerable<Guid> RetrieveLinkedActivities(
            ICrmDataContext dataContext,
            Guid entityReplicationCode,
            EntityName activityType,
            IEnumerable<int> allowedStateCodes,
            int? participationType,
            Guid? oldOwner)
        {
            // Вытаскивание действий, связанных с клиентом.
            // Если participationType = null, то вытаскиваются действия, привязанные через поле "В отношении"

            // Иначе - действия, привязанные через поле, указанное в параметре participationType
            // Например, для ParticipationType.Recipient - это поле "Получатель" (для звонка, факса, писем и т.п.)
            // ParticipationType.RequiredAttendee - поле "Обязательно" (для встречи).
            // Действия, привязанные через вышеуказанные поля нельзя безгеморройно вытащить через метод
            // ICRMEntity.GetRelatedEntities, поэтому все переписано на QueryExpression`ы.
            var queryCriteria = new FilterExpression();

            var query = new QueryExpression(activityType.ToString())
                {
                    Criteria = queryCriteria,
                    Distinct = true
                };
            // Возвращаем только Id действия.
            query.ColumnSet.AddColumn("activityid");
            
            var stateFilter = queryCriteria.AddFilter(LogicalOperator.And);
            var allowedStatesFilter = stateFilter.AddFilter(LogicalOperator.Or);
            foreach (int stateCode in allowedStateCodes)
            {
                allowedStatesFilter.AddCondition("statecode", ConditionOperator.Equal, stateCode);
            }

            // При поиске действий, привязанных через ParticipationType, исключаем привязанные через поле "В отношении".
            var regardingObjectFilter = queryCriteria.AddFilter(LogicalOperator.And);
            regardingObjectFilter.AddCondition("regardingobjectid",
                                                         participationType.HasValue ? ConditionOperator.NotEqual : ConditionOperator.Equal,
                                                         entityReplicationCode);
            if (oldOwner.HasValue)
            {
                var ownerIdFilter = queryCriteria.AddFilter(LogicalOperator.And);
                ownerIdFilter.AddCondition("ownerid", ConditionOperator.Equal, oldOwner.Value);
            }
            
            if (participationType.HasValue)
            {
                var linkCriteria = new FilterExpression();
                var participationTypeFilter = linkCriteria.AddFilter(LogicalOperator.And);
                participationTypeFilter.AddCondition("partyid", ConditionOperator.Equal, entityReplicationCode);
                participationTypeFilter.AddCondition("participationtypemask", ConditionOperator.Equal, participationType);

                var linkedEntity = query.AddLink(EntityName.activityparty.ToString(), "activityid", "activityid", JoinOperator.Natural);
                linkedEntity.LinkCriteria = linkCriteria;
            }
            
            BusinessEntityCollection collection = dataContext.UsingService(service => service.RetrieveMultiple(query));
            return collection.BusinessEntities.Cast<DynamicEntity>().Select(x => ((Key)x.Properties["activityid"]).Value);
        }
    }
}