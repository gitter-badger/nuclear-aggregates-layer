using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeAppointmentRegardingObject : EntityTypeBase<EntityTypeAppointmentRegardingObject>
    {
        public override int Id
        {
            get { return EntityTypeIds.AppointmentRegardingObject; }
        }

        public override string Description
        {
            get { return "AppointmentRegardingObject"; }
        }
    }
}