using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Cancel;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Cancel;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities.Operations.Cancel
{
    public class CancelPhonecallAggregateService : ICancelPhonecallAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<Phonecall> _repository;

        public CancelPhonecallAggregateService(
            IOperationScopeFactory operationScopeFactory,
            ISecureRepository<Phonecall> repository)
        {
            _operationScopeFactory = operationScopeFactory;
            _repository = repository;
        }

        public void Cancel(Phonecall phonecall)
        {
            if (phonecall == null)
            {
                throw new ArgumentNullException("phonecall");
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CancelIdentity, Phonecall>())
            {
                phonecall.Status = ActivityStatus.Canceled;

                _repository.Update(phonecall);
                _repository.Save();

                operationScope.Updated<Phonecall>(phonecall.Id);
                operationScope.Complete();
            }
        }
    }
}
