using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities.ReadModel
{
    public class ActivityReadModel : IActivityReadModel
    {
        private readonly IFinder _finder;

        public ActivityReadModel(IFinder finder)
        {
            _finder = finder;
        }

		public Appointment GetAppointment(long appointmentId)
		{
			return _finder.FindOne(Specs.Find.ById<Appointment>(appointmentId));
		}

	    public Phonecall GetPhonecall(long phonecallId)
        {
			return _finder.FindOne(Specs.Find.ById<Phonecall>(phonecallId));
        }

	    public Task GetTask(long taskId)
		{
			return _finder.FindOne(Specs.Find.ById<Task>(taskId));
		}

	    public IEnumerable<RegardingObject<TEntity>> GetRegardingObjects<TEntity>(long taskId) where TEntity : class, IEntity
	    {
			return _finder.FindMany(FindObjects<TEntity>(taskId));
		}

	    public bool CheckIfActivityExistsRegarding(long clientId)
        {
			return
				_finder.FindMany(Specs.Find.Custom<RegardingObject<Appointment>>(x => x.TargetEntityId == clientId)).Any()
				|| _finder.FindMany(Specs.Find.Custom<RegardingObject<Phonecall>>(x => x.TargetEntityId == clientId)).Any()
				|| _finder.FindMany(Specs.Find.Custom<RegardingObject<Task>>(x => x.TargetEntityId == clientId)).Any();
        }

        public bool CheckIfOpenActivityExistsRegarding(EntityName entityName, long entityId)
        {
            return
                CheckIfOpenAppointmentExistsRegarding(entityName, entityId)
                || CheckIfOpenPhonecallExistsRegarding(entityName, entityId)
                || CheckIfOpenTaskExistsRegarding(entityName, entityId);
        }

        private bool CheckIfOpenAppointmentExistsRegarding(EntityName entityName, long entityId)
        {
            var ids = (
                from reference in _finder.FindMany(Specs.Find.Custom<RegardingObject<Appointment>>(x => x.TargetEntityName == entityName && x.TargetEntityId == entityId))
                select reference.SourceEntityId
                ).ToArray();

            return _finder.FindMany(Specs.Find.Custom<Appointment>(x => x.Status == ActivityStatus.InProgress) & Specs.Find.ByIds<Appointment>(ids)).Any();
        }

        private bool CheckIfOpenPhonecallExistsRegarding(EntityName entityName, long entityId)
        {
            var ids = (
                from reference in _finder.FindMany(Specs.Find.Custom<RegardingObject<Phonecall>>(x => x.TargetEntityName == entityName && x.TargetEntityId == entityId))
                select reference.SourceEntityId
                ).ToArray();

            return _finder.FindMany(Specs.Find.Custom<Phonecall>(x => x.Status == ActivityStatus.InProgress) & Specs.Find.ByIds<Phonecall>(ids)).Any();
        }

        private bool CheckIfOpenTaskExistsRegarding(EntityName entityName, long entityId)
        {
            var ids = (
                from reference in _finder.FindMany(Specs.Find.Custom<RegardingObject<Task>>(x => x.TargetEntityName == entityName && x.TargetEntityId == entityId))
                select reference.SourceEntityId
                ).ToArray();

            return _finder.FindMany(Specs.Find.Custom<Task>(x => x.Status == ActivityStatus.InProgress) & Specs.Find.ByIds<Task>(ids)).Any();
        }

		private static FindSpecification<RegardingObject<TEntity>> FindObjects<TEntity>(long entityId)
			where TEntity : class, IEntity
	    {
			return new FindSpecification<RegardingObject<TEntity>>(x => x.SourceEntityId == entityId);
	    }
   }
}