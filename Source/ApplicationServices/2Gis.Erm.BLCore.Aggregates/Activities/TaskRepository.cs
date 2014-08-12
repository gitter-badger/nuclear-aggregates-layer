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
	public sealed class TaskRepository : ITaskRepository
	{
		private const string ActivityHasAlreadyTheIdentityMessage = "The activity has already the identity.";
		private const string ActivityHasNoTheIdentityMessage = "The activity has no the identity.";
		private const string ActivityDoesNotExistForIdentityMessage = "The activity does not exist for the specified identity.";

		private readonly IFinder _finder;
		private readonly IIdentityProvider _identityProvider;
		private readonly IOperationScopeFactory _operationScopeFactory;
		private readonly IRelationalRepository<Task> _repository;
		private readonly IRepository<EntityToEntityReference> _referenceRepository;

		public TaskRepository(
			IFinder finder,
			IIdentityProvider identityProvider,
			IOperationScopeFactory operationScopeFactory,
			IRelationalRepository<Task> repository)
		{
			_finder = finder;
			_identityProvider = identityProvider;
			_operationScopeFactory = operationScopeFactory;
			_repository = repository; 
			_referenceRepository = _repository.GetRelatedRepository<EntityToEntityReference>();
		}

		public long Add(Task task)
		{
			if (!task.IsNew())
			{
				throw new ArgumentException(ActivityHasAlreadyTheIdentityMessage, "task");
			}

			using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, Task>())
			{
				_identityProvider.SetFor(task);
				// update the reference with the new identity
				task.RegardingObjects.ToList().ForEach(x => x.SourceEntityId = task.Id);

				_repository.Add(task);
				operationScope.Added<Task>(task.Id);
				_repository.Save();

				// add the references
				_referenceRepository.Update(null, task.RegardingObjects);

				operationScope.Complete();

				return task.Id;
			}
		}

		public int Delete(long entityId)
		{
			var task = _finder.FindOne(Specs.Find.ById<Task>(entityId));
			if (task == null)
				throw new ArgumentException(ActivityDoesNotExistForIdentityMessage, "entityId");
			return Delete(task);
		}

		public int Delete(Task task)
		{
			if (task.IsNew())
			{
				throw new ArgumentException(ActivityHasNoTheIdentityMessage, "task");
			}

			using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, Task>())
			{
				// delete the references
				_referenceRepository.Update(task.RegardingObjects, null);

				_repository.Delete(task);
				operationScope.Deleted<Task>(task.Id);
				var count = _repository.Save();
				
				operationScope.Complete();

				return count;
			}
		}

		public void UpdateContent(Task task)
		{
			if (task.Id == 0)
			{
				throw new ArgumentException(ActivityHasNoTheIdentityMessage, "task");
			}

			using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Task>())
			{
				_repository.Update(task);
				operationScope.Updated<Task>(task.Id);

				_repository.Save();
				operationScope.Complete();
			}
		}

		public void UpdateRegardingObjects(Task task)
		{
			if (task.Id == 0)
			{
				throw new ArgumentException(ActivityHasNoTheIdentityMessage, "task");
			}

			using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, EntityToEntityReference>())
			{
				var oldActivity = _finder.FindOne(Specs.Find.ById<Task>(task.Id));

				_referenceRepository.Update(oldActivity.RegardingObjects, task.RegardingObjects);

				// TODO {s.pomadin, 07.08.2014}: notify about the reference changes
				//operationScope.Updated<EntityToEntityReference>(regardingObjects.Select(x => x.));
				operationScope.Complete();
			}
		}
	}
}