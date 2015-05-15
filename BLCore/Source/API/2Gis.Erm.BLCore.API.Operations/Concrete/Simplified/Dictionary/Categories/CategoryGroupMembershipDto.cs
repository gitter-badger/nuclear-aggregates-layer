using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Categories
{
    [DataContract]
    public sealed class CategoryGroupMembershipDto
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public long CategoryId { get; set; }

        [DataMember]
        public string CategoryName { get; set; }

        [DataMember]
        public long? CategoryGroupId { get; set; }

        [DataMember]
        public int CategoryLevel { get; set; }
    }
}
