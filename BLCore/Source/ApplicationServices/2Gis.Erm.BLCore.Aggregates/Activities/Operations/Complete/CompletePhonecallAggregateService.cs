using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Complete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
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

        private readonly IActionLogger _actionLogger;

        private readonly IRepository<Phonecall> _repository;

        public CompletePhonecallAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IActionLogger actionLogger,
            IRepository<Phonecall> repository)
        {
            _operationScopeFactory = operationScopeFactory;
            _actionLogger = actionLogger;
            _repository = repository;
        }

        public void Complete(Phonecall phonecall)
        {
            if (phonecall == null)
            {
                throw new ArgumentNullException("phonecall");
            }

            if (phonecall.Status == ActivityStatus.Completed)
            {
                return;
            }

            if (phonecall.Status != ActivityStatus.InProgress)
            {
                throw new BusinessLogicException(string.Format(BLResources.CannotCompleteFinishedOrClosedActivity, phonecall.Header));
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CompleteIdentity, Phonecall>())
            {
                var originalValue = phonecall.Status;
                phonecall.Status = ActivityStatus.Completed;

                _repository.Update(phonecall);
                _repository.Save();

                _actionLogger.LogChanges(phonecall, x => x.Status, originalValue, phonecall.Status);

                operationScope.Updated<Phonecall>(phonecall.Id);
                operationScope.Complete();
            }
        }
    }
}
