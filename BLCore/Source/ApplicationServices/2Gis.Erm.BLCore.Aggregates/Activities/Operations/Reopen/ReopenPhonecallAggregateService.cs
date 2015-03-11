using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Reopen;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Reopen;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities.Operations.Reopen
{
    public class ReopenPhonecallAggregateService : IReopenPhonecallAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;

        private readonly IActionLogger _actionLogger;

        private readonly IRepository<Phonecall> _repository;

        public ReopenPhonecallAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IActionLogger actionLogger,
            IRepository<Phonecall> repository)
        {
            _operationScopeFactory = operationScopeFactory;
            _actionLogger = actionLogger;
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
                var originalStatus = phonecall.Status;
                phonecall.Status = ActivityStatus.InProgress;

                _repository.Update(phonecall);
                _repository.Save();

                _actionLogger.LogChanges(phonecall, s => s.Status, originalStatus, phonecall.Status);

                operationScope.Updated<Phonecall>(phonecall.Id);
                operationScope.Complete();
            }
        }
    }
}
