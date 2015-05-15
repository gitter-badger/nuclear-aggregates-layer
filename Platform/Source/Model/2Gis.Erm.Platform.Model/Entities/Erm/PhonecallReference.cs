using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class PhonecallReference : IEntity
    {
        public long PhonecallId { get; set; }
        public int Reference { get; set; }
        public int ReferencedType { get; set; }
        public long ReferencedObjectId { get; set; }

        public PhonecallBase PhonecallBase { get; set; }
    }
}