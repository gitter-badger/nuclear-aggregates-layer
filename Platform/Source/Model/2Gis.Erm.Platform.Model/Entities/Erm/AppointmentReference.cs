using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class AppointmentReference : IEntity
    {
        public long AppointmentId { get; set; }
        public int Reference { get; set; }
        public int ReferencedType { get; set; }
        public long ReferencedObjectId { get; set; }

        public AppointmentBase AppointmentBase { get; set; }
    }
}