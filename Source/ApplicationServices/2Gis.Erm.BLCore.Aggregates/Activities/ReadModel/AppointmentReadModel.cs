using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities.ReadModel
{
    public sealed class AppointmentReadModel : IAppointmentReadModel
    {
        private readonly IFinder _finder;

        public AppointmentReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public Appointment GetAppointment(long appointmentId)
        {
            return _finder.FindOne(Specs.Find.ById<Appointment>(appointmentId));
        }

        public IEnumerable<AppointmentRegardingObject> GetRegardingObjects(long appointmentId)
        {
            return _finder.FindMany(Specs.Find.Custom<AppointmentRegardingObject>(x => x.SourceEntityId == appointmentId)).ToList();
        }

        public IEnumerable<AppointmentAttendee> GetAttendees(long appointmentId)
        {
            return _finder.FindMany(Specs.Find.Custom<AppointmentAttendee>(x => x.SourceEntityId == appointmentId)).ToList();
        }

        public bool CheckIfAppointmentExistsRegarding(EntityName entityName, long entityId)
        {
            return _finder.FindMany(Specs.Find.Custom<AppointmentRegardingObject>(x => x.TargetEntityName == entityName && x.TargetEntityId == entityId)).Any();
        }
        
        public bool CheckIfOpenAppointmentExistsRegarding(EntityName entityName, long entityId)
        {
            var ids = (
                from reference in _finder.FindMany(Specs.Find.Custom<AppointmentRegardingObject>(x => x.TargetEntityName == entityName && x.TargetEntityId == entityId))
                select reference.SourceEntityId
                ).ToArray();

            return _finder.FindMany(
                Specs.Find.Active<Appointment>()
                & Specs.Find.Custom<Appointment>(x => x.Status == ActivityStatus.InProgress) 
                & Specs.Find.ByIds<Appointment>(ids)).Any();
        }

        public IEnumerable<Appointment> LookupAppointmentsRegarding(EntityName entityName, long entityId)
        {
            var ids = (
                from reference in _finder.FindMany(Specs.Find.Custom<AppointmentRegardingObject>(x => x.TargetEntityName == entityName && x.TargetEntityId == entityId))
                select reference.SourceEntityId
                ).ToArray();

            return _finder.FindMany(Specs.Find.Active<Appointment>() && Specs.Find.ByIds<Appointment>(ids));
        }

        public IEnumerable<Appointment> LookupOpenAppointmentsOwnedBy(long ownerCode)
        {
            return _finder.FindMany(Specs.Find.Owned<Appointment>(ownerCode) & Specs.Find.Custom<Appointment>(x => x.Status == ActivityStatus.InProgress));
        }
    }
}