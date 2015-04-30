using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using NuClear.Storage;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities
{
    public sealed class AssignPhonecallAggregateService : IAssignPhonecallAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<Phonecall> _repository;

        public AssignPhonecallAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<Phonecall> repository)
        {
            _operationScopeFactory = operationScopeFactory;
            _repository = repository;
        }

        public void Assign(Phonecall phonecall, long ownerCode)
        {
            if (phonecall == null)
            {
                throw new ArgumentNullException("phonecall");
            }

            if (phonecall.Status != ActivityStatus.InProgress)
            {
                throw new BusinessLogicException(BLResources.CannotAssignActivityNotInProgress);
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<AssignIdentity, Phonecall>())
            {
                phonecall.OwnerCode = ownerCode;

                _repository.Update(phonecall);
                _repository.Save();

                operationScope.Updated<Phonecall>(phonecall.Id);
                operationScope.Complete();
            }
        }
    }
}