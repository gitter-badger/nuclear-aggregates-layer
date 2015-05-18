using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.Platform.Core.ActionLogging
{
    public sealed class ActionsLogger : IActionLogger
    {
        private readonly IRepository<ActionsHistory> _actionsHistoryRepository;
        private readonly IRepository<ActionsHistoryDetail> _actionsHistoryDetailRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;

        public ActionsLogger(IRepository<ActionsHistory> actionsHistoryRepository,
                             IRepository<ActionsHistoryDetail> actionsHistoryDetailRepository,
                             IIdentityProvider identityProvider,
                             IOperationScopeFactory scopeFactory)
        {
            _actionsHistoryRepository = actionsHistoryRepository;
            _actionsHistoryDetailRepository = actionsHistoryDetailRepository;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
        }

        public void LogChanges(IEnumerable<ChangesDescriptor> changeDescriptors)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<BulkCreateIdentity, ActionsHistory>())
            {
                LogChanges(scope, changeDescriptors);
                scope.Complete();
            }
        }

        public void LogChanges(ChangesDescriptor changeDescriptor)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, ActionsHistory>())
            {
                LogChanges(scope, new[] { changeDescriptor });
                scope.Complete();
            }
        }

        private void LogChanges(IOperationScope scope, IEnumerable<ChangesDescriptor> changeDescriptors)
        {
            foreach (var changesDescriptor in changeDescriptors)
            {
                var actionsHistoryItem = new ActionsHistory
                    {
                        ActionType = (int)ActionType.Edit, // edit only for now
                        EntityType = changesDescriptor.EntityType.Id,
                        EntityId = changesDescriptor.EntityId
                    };

                _identityProvider.SetFor(actionsHistoryItem);
                _actionsHistoryRepository.Add(actionsHistoryItem);
                scope.Added<ActionsHistory>(actionsHistoryItem.Id);

                foreach (var difference in changesDescriptor.Changes)
                {
                    var actionsHistoryDetailItem = new ActionsHistoryDetail
                        {
                            ActionsHistoryId = actionsHistoryItem.Id,
                            PropertyName = difference.Key,
                            OriginalValue = difference.Value.OriginalValue != null ? difference.Value.OriginalValue.ToString() : null,
                            ModifiedValue = difference.Value.ModifiedValue != null ? difference.Value.ModifiedValue.ToString() : null,
                        };

                    _identityProvider.SetFor(actionsHistoryDetailItem);
                    _actionsHistoryDetailRepository.Add(actionsHistoryDetailItem);
                    scope.Added<ActionsHistoryDetail>(actionsHistoryDetailItem.Id);
                }
            }

            _actionsHistoryRepository.Save();
            _actionsHistoryDetailRepository.Save();
        }
    }
}