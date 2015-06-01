using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Complete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities.Operations.Complete
{
    public class CompleteLetterAggregateService : ICompleteLetterAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<Letter> _repository;

        public CompleteLetterAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<Letter> repository)
        {
            _operationScopeFactory = operationScopeFactory;
            _repository = repository;
        }

        public void Complete(Letter letter)
        {
            if (letter == null)
            {
                throw new ArgumentNullException("letter");
            }
           
            if (letter.Status != ActivityStatus.InProgress)
            {
                throw new BusinessLogicException(string.Format(BLResources.CannotCompleteFinishedOrClosedActivity, letter.Header));
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CompleteIdentity, Letter>())
            {                
                letter.Status = ActivityStatus.Completed;

                _repository.Update(letter);
                _repository.Save();

                operationScope.Updated<Letter>(letter.Id);
                operationScope.Complete();
            }
        }
    }
}
