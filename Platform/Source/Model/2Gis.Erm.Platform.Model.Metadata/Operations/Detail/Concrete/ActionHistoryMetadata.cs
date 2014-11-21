using System.Collections.Generic;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.Platform.Model.Metadata.Operations.Detail.Concrete
{
    [DataContract]
    public sealed class ActionHistoryMetadata : IOperationMetadata<ActionHistoryIdentity>
    {
        [DataMember]
        public IEnumerable<string> Properties { get; set; }
    }
}