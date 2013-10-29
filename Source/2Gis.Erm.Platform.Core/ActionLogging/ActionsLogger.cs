using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.Platform.Core.ActionLogging
{
    public sealed class ActionsLogger : IActionLogger
    {
        private readonly IRepository<ActionsHistory> _actionsHistoryRepository;
        private readonly IRepository<ActionsHistoryDetail> _actionsHistoryDetailRepository;
        private readonly IActionLoggingValidatorFactory _actionLoggingValidatorFactory;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public ActionsLogger(IRepository<ActionsHistory> actionsHistoryRepository,
                             IRepository<ActionsHistoryDetail> actionsHistoryDetailRepository,
                             IActionLoggingValidatorFactory actionLoggingValidatorFactory,
                             IIdentityProvider identityProvider,
                             IOperationScopeFactory operationScopeFactory)
        {
            _actionsHistoryRepository = actionsHistoryRepository;
            _actionsHistoryDetailRepository = actionsHistoryDetailRepository;
            _actionLoggingValidatorFactory = actionLoggingValidatorFactory;
            _identityProvider = identityProvider;
            _operationScopeFactory = operationScopeFactory;
        }

        public IDictionary<string, Tuple<object, object>> LogChanges(EntityName entityType, long entityId, object originalObject, object modifiedObject)
        {
            var differences = CompareObjectsHelper.CompareObjects(CompareObjectMode.Shallow, originalObject, modifiedObject, Enumerable.Empty<string>());
            LogChanges(entityType, entityId, differences);
            return differences;
        }

        public void LogChanges(EntityName entityType, long entityId, IDictionary<string, Tuple<object, object>> differences)
        {
            var validators = _actionLoggingValidatorFactory.GetValidators(entityType);

            var canLogChanges = validators.All(x => x.Validate(entityId));
            if (!canLogChanges)
            {
                return;
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, ActionsHistory>())
            {
                var actionsHistoryItem = new ActionsHistory
                    {
                        ActionType = (int)ActionType.Edit, // edit only for now
                        EntityType = (int)entityType,
                        EntityId = entityId
                    };
                _identityProvider.SetFor(actionsHistoryItem);
                _actionsHistoryRepository.Add(actionsHistoryItem);
                _actionsHistoryRepository.Save();
                operationScope.Added<ActionsHistory>(actionsHistoryItem.Id);

                foreach (var difference in differences)
                {
                    var actionsHistoryDetailItem = new ActionsHistoryDetail
                        {
                            ActionsHistoryId = actionsHistoryItem.Id,
                            PropertyName = difference.Key,
                            OriginalValue = difference.Value.Item1 != null ? difference.Value.Item1.ToString() : null,
                            ModifiedValue = difference.Value.Item2 != null ? difference.Value.Item2.ToString() : null,
                        };
                    _identityProvider.SetFor(actionsHistoryDetailItem);
                    _actionsHistoryDetailRepository.Add(actionsHistoryDetailItem);
                    operationScope.Added<ActionsHistoryDetail>(actionsHistoryDetailItem.Id);
                }

                _actionsHistoryDetailRepository.Save();
                operationScope.Complete();
            }
        }
    }
}