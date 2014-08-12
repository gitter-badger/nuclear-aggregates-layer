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
	public sealed class AppointmentRepository : IAppointmentRepository
	{
		private const string ActivityHasAlreadyTheIdentityMessage = "The activity has already the identity.";
		private const string ActivityHasNoTheIdentityMessage = "The activity has no the identity.";
		private const string AppointmentDoesNotExistForIdentityMessage = "The activity does not exist for the specified identity.";

		private readonly IFinder _finder;
		private readonly IIdentityProvider _identityProvider;
		private readonly IOperationScopeFactory _operationScopeFactory;
		private readonly IRelationalRepository<Appointment> _repository;
		private readonly IRepository<EntityToEntityReference> _referenceRepository;

		public AppointmentRepository(
			IFinder finder,
			IIdentityProvider identityProvider,
			IOperationScopeFactory operationScopeFactory,
			IRelationalRepository<Appointment> repository)
		{
			_finder = finder;
			_identityProvider = identityProvider;
			_operationScopeFactory = operationScopeFactory;
			_repository = repository; 
			_referenceRepository = _repository.GetRelatedRepository<EntityToEntityReference>();
		}

		public long Add(Appointment appointment)
		{
			if (!appointment.IsNew())
			{
				throw new ArgumentException(ActivityHasAlreadyTheIdentityMessage, "appointment");
			}

			using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, Appointment>())
			{
				_identityProvider.SetFor(appointment);
				// update the reference with the new identity
				appointment.RegardingObjects.ToList().ForEach(x => x.SourceEntityId = appointment.Id);

				_repository.Add(appointment);
				operationScope.Added<Appointment>(appointment.Id);
				_repository.Save();

				// add the references
				_referenceRepository.Update(null, appointment.RegardingObjects);

				operationScope.Complete();

				return appointment.Id;
			}
		}

		public int Delete(long entityId)
		{
			var appointment = _finder.FindOne(Specs.Find.ById<Appointment>(entityId));
			if (appointment == null)
				throw new ArgumentException(AppointmentDoesNotExistForIdentityMessage, "entityId");
			return Delete(appointment);
		}

		public int Delete(Appointment appointment)
		{
			if (appointment.IsNew())
			{
				throw new ArgumentException(ActivityHasNoTheIdentityMessage, "appointment");
			}

			using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, Appointment>())
			{
				// delete the references
				_referenceRepository.Update(appointment.RegardingObjects, null);

				_repository.Delete(appointment);
				operationScope.Deleted<Appointment>(appointment.Id);
				var count = _repository.Save();
				
				operationScope.Complete();

				return count;
			}
		}

		public void UpdateContent(Appointment appointment)
		{
			if (appointment.Id == 0)
			{
				throw new ArgumentException(ActivityHasNoTheIdentityMessage, "appointment");
			}

			using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Appointment>())
			{
				_repository.Update(appointment);
				operationScope.Updated<Appointment>(appointment.Id);

				_repository.Save();
				operationScope.Complete();
			}
		}

		public void UpdateRegardingObjects(Appointment appointment)
		{
			if (appointment.Id == 0)
			{
				throw new ArgumentException(ActivityHasNoTheIdentityMessage, "appointment");
			}

			using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, EntityToEntityReference>())
			{
				var oldActivity = _finder.FindOne(Specs.Find.ById<Appointment>(appointment.Id));

				_referenceRepository.Update(oldActivity.RegardingObjects, appointment.RegardingObjects);

				// TODO {s.pomadin, 07.08.2014}: notify about the reference changes
				//operationScope.Updated<EntityToEntityReference>(regardingObjects.Select(x => x.));
				operationScope.Complete();
			}
		}
	}
}