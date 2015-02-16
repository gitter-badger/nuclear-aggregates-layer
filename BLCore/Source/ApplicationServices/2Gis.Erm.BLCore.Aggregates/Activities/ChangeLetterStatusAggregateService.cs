using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Cancel;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Activity;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities
{
    public class ChangeLetterStatusAggregateService : IChangeLetterStatusAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<Letter> _repository;

        public ChangeLetterStatusAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<Letter> repository)
        {
            _operationScopeFactory = operationScopeFactory;
            _repository = repository;
        }

        public void Change(Letter letter, ActivityStatus status)
        {
            if (letter == null)
            {
                throw new ArgumentNullException("letter");
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<ChangeActivityStatusIdentity, Letter>())
            {
                letter.Status = status;

                _repository.Update(letter);
                _repository.Save();

                operationScope.Updated<Letter>(letter.Id);
                operationScope.Complete();
            }
        }
    }
}
