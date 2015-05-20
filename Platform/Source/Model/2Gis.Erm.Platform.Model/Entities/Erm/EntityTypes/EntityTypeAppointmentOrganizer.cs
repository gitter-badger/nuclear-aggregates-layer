using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeAppointmentOrganizer : EntityTypeBase<EntityTypeAppointmentOrganizer>
    {
        public override int Id
        {
            get { return EntityTypeIds.AppointmentOrganizer; }
        }

        public override string Description
        {
            get { return "AppointmentOrganizer"; }
        }
    }
}