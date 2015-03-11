using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Reopen;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Reopen;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities.Operations.Reopen
{
    public class ReopenLetterAggregateService : IReopenLetterAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;

        private readonly IActionLogger _actionLogger;

        private readonly IRepository<Letter> _repository;

        public ReopenLetterAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IActionLogger actionLogger,
            IRepository<Letter> repository)
        {
            _operationScopeFactory = operationScopeFactory;
            _actionLogger = actionLogger;
            _repository = repository;
        }

        public void Reopen(Letter letter)
        {
            if (letter == null)
            {
                throw new ArgumentNullException("letter");
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<ReopenIdentity, Letter>())
            {
                var originalValue = letter.Status;
                letter.Status = ActivityStatus.InProgress;

                _repository.Update(letter);
                _repository.Save();

                _actionLogger.LogChanges(letter, x => x.Status, originalValue, letter.Status);

                operationScope.Updated<Letter>(letter.Id);
                operationScope.Complete();
            }
        }
    }
}
