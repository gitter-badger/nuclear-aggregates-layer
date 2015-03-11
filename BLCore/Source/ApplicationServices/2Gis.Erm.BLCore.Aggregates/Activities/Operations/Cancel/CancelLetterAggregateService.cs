using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Cancel;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Cancel;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities.Operations.Cancel
{
    public class CancelLetterAggregateService : ICancelLetterAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;

        private readonly IActionLogger _actionLogger;

        private readonly ISecureRepository<Letter> _repository;

        public CancelLetterAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IActionLogger actionLogger,
            ISecureRepository<Letter> repository)
        {
            _operationScopeFactory = operationScopeFactory;
            _actionLogger = actionLogger;
            _repository = repository;
        }

        public void Cancel(Letter letter)
        {
            if (letter == null)
            {
                throw new ArgumentNullException("letter");
            }

            if (letter.Status != ActivityStatus.InProgress)
            {
                throw new BusinessLogicException(string.Format(BLResources.CannotCancelFinishedOrClosedActivity, letter.Header));
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CancelIdentity, Letter>())
            {
                var originalValue = letter.Status;
                letter.Status = ActivityStatus.Canceled;

                _repository.Update(letter);
                _repository.Save();

                _actionLogger.LogChanges(letter, x => x.Status, originalValue, letter.Status);

                operationScope.Updated<Letter>(letter.Id);
                operationScope.Complete();
            }
        }
    }
}
