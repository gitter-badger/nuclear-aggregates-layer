﻿using System.Collections.Generic;
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
        private readonly IOperationScopeFactory _scopeFactory;

        public ActionsLogger(IRepository<ActionsHistory> actionsHistoryRepository,
                             IRepository<ActionsHistoryDetail> actionsHistoryDetailRepository,
                             IActionLoggingValidatorFactory actionLoggingValidatorFactory,
                             IIdentityProvider identityProvider,
                             IOperationScopeFactory scopeFactory)
        {
            _actionsHistoryRepository = actionsHistoryRepository;
            _actionsHistoryDetailRepository = actionsHistoryDetailRepository;
            _actionLoggingValidatorFactory = actionLoggingValidatorFactory;
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
                if (!CanLog(changesDescriptor))
            {
                    continue;
            }

                var actionsHistoryItem = new ActionsHistory
                    {
                        ActionType = (int)ActionType.Edit, // edit only for now
                    EntityType = (int)changesDescriptor.EntityType,
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

        private bool CanLog(ChangesDescriptor changesDescriptor)
        {
            // FIXME {i.maslennikov, 24.09.2013}: в процессе рефакторинга - попробовать перенести в точки вызова/конфигурирования логирования изменений
            var validators = _actionLoggingValidatorFactory.GetValidators(changesDescriptor.EntityType);
            return validators.All(x => x.Validate(changesDescriptor.EntityId));
        }
    }
}