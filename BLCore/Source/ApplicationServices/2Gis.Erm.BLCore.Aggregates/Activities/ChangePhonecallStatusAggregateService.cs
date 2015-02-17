using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Activity;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities
{
    public class ChangePhonecallStatusAggregateService : IChangePhonecallStatusAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<Phonecall> _repository;

        public ChangePhonecallStatusAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<Phonecall> repository)
        {
            _operationScopeFactory = operationScopeFactory;
            _repository = repository;
        }

        public void Change(Phonecall phonecall, ActivityStatus status)
        {
            if (phonecall == null)
            {
                throw new ArgumentNullException("phonecall");
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<ChangeActivityStatusIdentity, Phonecall>())
            {
                phonecall.Status = status;

                _repository.Update(phonecall);
                _repository.Save();

                operationScope.Updated<Phonecall>(phonecall.Id);
                operationScope.Complete();
            }
        }
    }
}
