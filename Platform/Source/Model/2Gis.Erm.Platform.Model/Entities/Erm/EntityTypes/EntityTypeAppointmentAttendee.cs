using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeAppointmentAttendee : EntityTypeBase<EntityTypeAppointmentAttendee>
    {
        public override int Id
        {
            get { return EntityTypeIds.AppointmentAttendee; }
        }

        public override string Description
        {
            get { return "AppointmentAttendee"; }
        }
    }
}