using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Cancel;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
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
        private readonly ISecureRepository<Letter> _repository;

        public CancelLetterAggregateService(
            IOperationScopeFactory operationScopeFactory,
            ISecureRepository<Letter> repository)
        {
            _operationScopeFactory = operationScopeFactory;
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
                letter.Status = ActivityStatus.Canceled;

                _repository.Update(letter);
                _repository.Save();

                operationScope.Updated<Letter>(letter.Id);
                operationScope.Complete();
            }
        }
    }
}
