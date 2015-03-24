using System.Runtime.Serialization;

using NuClear.Metamodeling.Domain.Operations.Detail;
using NuClear.Model.Common.Operations.Identity.Generic;

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