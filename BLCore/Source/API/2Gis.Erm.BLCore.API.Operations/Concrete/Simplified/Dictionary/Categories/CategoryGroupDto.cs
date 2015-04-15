using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Categories
{
    [DataContract]
    public sealed class CategoryGroupDto
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool IsDefault { get; set; }
    }
}
