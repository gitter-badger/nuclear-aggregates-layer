using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Reopen;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities.Operations.Reopen
{
    public class ReopenPhonecallAggregateService : IReopenPhonecallAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<Phonecall> _repository;

        public ReopenPhonecallAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<Phonecall> repository)
        {
            _operationScopeFactory = operationScopeFactory;
            _repository = repository;
        }

        public void Reopen(Phonecall phonecall)
        {
            if (phonecall == null)
            {
                throw new ArgumentNullException("phonecall");
            }

            if (phonecall.Status == ActivityStatus.InProgress)
            {
                return;
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<ReopenIdentity, Phonecall>())
            {
                phonecall.Status = ActivityStatus.InProgress;

                _repository.Update(phonecall);
                _repository.Save();

                operationScope.Updated<Phonecall>(phonecall.Id);
                operationScope.Complete();
            }
        }
    }
}
