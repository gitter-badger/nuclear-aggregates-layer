using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities
{
    public sealed class AssignLetterAggregateService : IAssignLetterAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<Letter> _repository;

        public AssignLetterAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<Letter> repository)
        {
            _operationScopeFactory = operationScopeFactory;
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
                letter.OwnerCode = ownerCode;

                _repository.Update(letter);
                _repository.Save();

                operationScope.Updated<Letter>(letter.Id);
                operationScope.Complete();
            }
        }
    }
}