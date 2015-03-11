using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities
{
    public sealed class AssignLetterAggregateService : IAssignLetterAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;

        private readonly IActionLogger _actionLogger;

        private readonly IRepository<Letter> _repository;

        public AssignLetterAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IActionLogger actionLogger,
            IRepository<Letter> repository)
        {
            _operationScopeFactory = operationScopeFactory;
            _actionLogger = actionLogger;
            _repository = repository;
        }

        public void Assign(Letter letter, long ownerCode)
        {
            if (letter == null)
            {
                throw new ArgumentNullException("letter");
            }

            if (letter.Status != ActivityStatus.InProgress)
            {
                throw new BusinessLogicException(BLResources.CannotAssignActivityNotInProgress);
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<AssignIdentity, Letter>())
            {
                var originalOwnerCode = letter.OwnerCode;
                letter.OwnerCode = ownerCode;

                _repository.Update(letter);
                _repository.Save();

                _actionLogger.LogChanges(letter, x => x.OwnerCode, originalOwnerCode, ownerCode);

                operationScope.Updated<Letter>(letter.Id);
                operationScope.Complete();
            }
        }
    }
}