using System.Runtime.Serialization;

using NuClear.Metamodeling.Operations.Detail;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.Platform.Model.Metadata.Operations.Detail.Concrete
{
    [DataContract]
    public sealed class DisqualifyMetadata : IOperationMetadata<DisqualifyIdentity>
    {
    }
}