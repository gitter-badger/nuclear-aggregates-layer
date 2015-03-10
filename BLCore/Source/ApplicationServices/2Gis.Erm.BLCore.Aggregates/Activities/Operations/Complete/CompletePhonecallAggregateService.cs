using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Complete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Complete;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities.Operations.Complete
{
    public class CompletePhonecallAggregateService : ICompletePhonecallAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<Phonecall> _repository;

        public CompletePhonecallAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<Phonecall> repository)
        {
            _operationScopeFactory = operationScopeFactory;
            _repository = repository;
        }

        public void Complete(Phonecall phonecall)
        {
            if (phonecall == null)
            {
                throw new ArgumentNullException("phonecall");
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CompleteIdentity, Phonecall>())
            {
                if (phonecall.Status != ActivityStatus.InProgress)
                {
                    throw new BusinessLogicException(string.Format(BLResources.CannotCompleteFinishedOrClosedActivity, phonecall.Header));
                }

                phonecall.Status = ActivityStatus.Completed;

                _repository.Update(phonecall);
                _repository.Save();

                operationScope.Updated<Phonecall>(phonecall.Id);
                operationScope.Complete();
            }
        }
    }
}
