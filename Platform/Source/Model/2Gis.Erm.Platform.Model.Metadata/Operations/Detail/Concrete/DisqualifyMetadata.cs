using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.Platform.Model.Metadata.Operations.Detail.Concrete
{
    [DataContract]
    public sealed class DisqualifyMetadata : IOperationMetadata<DisqualifyIdentity>
    {
    }
}