using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities
{
	public sealed class PhonecallRepository : IPhonecallRepository
	{
		private const string ActivityHasAlreadyTheIdentityMessage = "The activity has already the identity.";
		private const string ActivityHasNoTheIdentityMessage = "The activity has no the identity.";
		private const string ActivityDoesNotExistForIdentityMessage = "The activity does not exist for the specified identity.";

		private readonly IFinder _finder;
		private readonly IIdentityProvider _identityProvider;
		private readonly IOperationScopeFactory _operationScopeFactory;
		private readonly IRelationalRepository<Phonecall> _repository;
		private readonly IRepository<EntityToEntityReference> _referenceRepository;

		public PhonecallRepository(
			IFinder finder,
			IIdentityProvider identityProvider,
			IOperationScopeFactory operationScopeFactory,
			IRelationalRepository<Phonecall> repository)
		{
			_finder = finder;
			_identityProvider = identityProvider;
			_operationScopeFactory = operationScopeFactory;
			_repository = repository; 
			_referenceRepository = _repository.GetRelatedRepository<EntityToEntityReference>();
		}

		public long Add(Phonecall phonecall)
		{
			if (!phonecall.IsNew())
			{
				throw new ArgumentException(ActivityHasAlreadyTheIdentityMessage, "phonecall");
			}

			using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, Phonecall>())
			{
				_identityProvider.SetFor(phonecall);
				// update the reference with the new identity
				phonecall.RegardingObjects.ToList().ForEach(x => x.SourceEntityId = phonecall.Id);

				_repository.Add(phonecall);
				operationScope.Added<Phonecall>(phonecall.Id);
				_repository.Save();

				// add the references
				_referenceRepository.Update(null, phonecall.RegardingObjects);

				operationScope.Complete();

				return phonecall.Id;
			}
		}

		public int Delete(long entityId)
		{
			var phonecall = _finder.FindOne(Specs.Find.ById<Phonecall>(entityId));
			if (phonecall == null)
				throw new ArgumentException(ActivityDoesNotExistForIdentityMessage, "entityId");
			return Delete(phonecall);
		}

		public int Delete(Phonecall phonecall)
		{
			if (phonecall.IsNew())
			{
				throw new ArgumentException(ActivityHasNoTheIdentityMessage, "phonecall");
			}

			using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, Phonecall>())
			{
				// delete the references
				_referenceRepository.Update(phonecall.RegardingObjects, null);

				_repository.Delete(phonecall);
				operationScope.Deleted<Phonecall>(phonecall.Id);
				var count = _repository.Save();
				
				operationScope.Complete();

				return count;
			}
		}

		public void UpdateContent(Phonecall phonecall)
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

		public void UpdateRegardingObjects(Phonecall phonecall)
		{
			if (phonecall.Id == 0)
			{
				throw new ArgumentException(ActivityHasNoTheIdentityMessage, "phonecall");
			}

			using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, EntityToEntityReference>())
			{
				var oldActivity = _finder.FindOne(Specs.Find.ById<Phonecall>(phonecall.Id));

				_referenceRepository.Update(oldActivity.RegardingObjects, phonecall.RegardingObjects);

				// TODO {s.pomadin, 07.08.2014}: notify about the reference changes
				//operationScope.Updated<EntityToEntityReference>(regardingObjects.Select(x => x.));
				operationScope.Complete();
			}
		}
	}
}