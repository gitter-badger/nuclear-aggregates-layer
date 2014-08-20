using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Activity;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities
{
	public sealed class UpdatePhonecallAggregationService : IUpdatePhonecallAggregateService, IUpdateRegardingObjectAggregateService<Phonecall>
	{
		private const string ActivityHasNoTheIdentityMessage = "The phonecall has no the identity.";

		private readonly IOperationScopeFactory _operationScopeFactory;
		private readonly IRepository<Phonecall> _repository;
		private readonly IRepository<RegardingObject<Phonecall>> _referenceRepository;

		public UpdatePhonecallAggregationService(
			IOperationScopeFactory operationScopeFactory, 
			IRepository<Phonecall> repository,
			IRepository<RegardingObject<Phonecall>> referenceRepository)
		{
			_operationScopeFactory = operationScopeFactory;
			_repository = repository;
			_referenceRepository = referenceRepository;
		}

		public void Update(Phonecall phonecall)
		{
			if (phonecall.Id == 0)
			{
				throw new ArgumentException(ActivityHasNoTheIdentityMessage, "phonecall");
			}

			using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Phonecall>())
			{
				_repository.Update(phonecall);
				operationScope.Updated<Phonecall>(phonecall.Id);

				_repository.Save();
				operationScope.Complete();
			}
		}

		public void ChangeRegardingObjects(IEnumerable<RegardingObject<Phonecall>> oldReferences, IEnumerable<RegardingObject<Phonecall>> newReferences)
		{
			using (var operationScope = _operationScopeFactory.CreateSpecificFor<AssignRegardingObjectIdentity, RegardingObject<Phonecall>>())
			{
				_referenceRepository.Update(oldReferences, newReferences);

				//operationScope.Updated<RegardingObject<Appointment>>(newReferences);

				operationScope.Complete();
			}
		}
	}
}