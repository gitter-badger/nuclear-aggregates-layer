using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.Platform.Model.Metadata.Operations.Detail.Concrete
{
    [DataContract]
    public sealed class AssignMetadata : IOperationMetadata<AssignIdentity>
    {
        [DataMember]
        public bool PartialAssignSupported { get; set; }
        public bool IsCascadeAssignForbidden { get; set; }
    }
}