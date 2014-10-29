using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class AppointmentReference :
        IEntity
    {
        public long AppointmentId { get; set; }
        public int Reference { get; set; }
        public int ReferencedType { get; set; }
        public long ReferencedObjectId { get; set; }

        public AppointmentBase AppointmentBase { get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}