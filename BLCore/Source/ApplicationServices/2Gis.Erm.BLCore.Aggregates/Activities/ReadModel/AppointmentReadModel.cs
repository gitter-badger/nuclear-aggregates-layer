using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Model.Common.Entities;
using NuClear.Storage.Readings;

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
            return _finder.Find(Specs.Find.ById<Appointment>(appointmentId)).One();
        }

        public IEnumerable<AppointmentRegardingObject> GetRegardingObjects(long appointmentId)
        {
            return _finder.Find(Specs.Find.Custom<AppointmentRegardingObject>(x => x.SourceEntityId == appointmentId)).Many();
        }

        public IEnumerable<AppointmentAttendee> GetAttendees(long appointmentId)
        {
            return _finder.Find(Specs.Find.Custom<AppointmentAttendee>(x => x.SourceEntityId == appointmentId)).Many();
        }

        public AppointmentOrganizer GetOrganizer(long appointmentId)
        {
            return _finder.Find(Specs.Find.Custom<AppointmentOrganizer>(x => x.SourceEntityId == appointmentId)).One();
        }

        public bool CheckIfAppointmentExistsRegarding(IEntityType entityName, long entityId)
        {
            return _finder.Find(ActivitySpecs.Find.ByReferencedObject<Appointment, AppointmentRegardingObject>(entityName, entityId)).Any();
        }
        
        public bool CheckIfOpenAppointmentExistsRegarding(IEntityType entityName, long entityId)
        {
            var ids = (from reference in _finder.Find(ActivitySpecs.Find.ByReferencedObject<Appointment, AppointmentRegardingObject>(entityName, entityId)).Many()
                       select reference.SourceEntityId)
                .ToArray();

            return _finder.Find(Specs.Find.Active<Appointment>() &&
                                    Specs.Find.Custom<Appointment>(x => x.Status == ActivityStatus.InProgress) && 
                                    Specs.Find.ByIds<Appointment>(ids))
                          .Any();
        }

        public IEnumerable<Appointment> LookupAppointmentsRegarding(IEntityType entityName, long entityId)
        {
            var ids = (from reference in _finder.Find(ActivitySpecs.Find.ByReferencedObject<Appointment, AppointmentRegardingObject>(entityName, entityId)).Many()
                       select reference.SourceEntityId)
                .ToArray();

            return _finder.Find(Specs.Find.Active<Appointment>() & Specs.Find.ByIds<Appointment>(ids)).Many();
        }

        public IEnumerable<Appointment> LookupOpenAppointmentsRegarding(IEntityType entityName, long entityId)
        {
            var ids = (from reference in _finder.Find(ActivitySpecs.Find.ByReferencedObject<Appointment, AppointmentRegardingObject>(entityName, entityId)).Many()
                       select reference.SourceEntityId)
                .ToArray();

            return _finder.Find(Specs.Find.Active<Appointment>() & Specs.Find.Custom<Appointment>(x => x.Status == ActivityStatus.InProgress) & Specs.Find.ByIds<Appointment>(ids)).Many();
        }

        public IEnumerable<Appointment> LookupOpenAppointmentsOwnedBy(long ownerCode)
        {
            return _finder.Find(Specs.Find.Owned<Appointment>(ownerCode) & Specs.Find.Custom<Appointment>(x => x.Status == ActivityStatus.InProgress)).Many();
        }
    }
}