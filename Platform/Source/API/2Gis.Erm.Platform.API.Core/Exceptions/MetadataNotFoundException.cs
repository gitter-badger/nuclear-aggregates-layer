using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;

namespace DoubleGis.Erm.Platform.API.Core.Exceptions
{
    [Serializable]
    public class MetadataNotFoundException : BusinessLogicException
    {
        public MetadataNotFoundException(IMetadataElementIdentity metadataIdentity) :
            base(GenerateMessage(metadataIdentity))
        {
        }

        protected MetadataNotFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }

        private static string GenerateMessage(IMetadataElementIdentity metadataIdentity)
        {
            return string.Format("Metadata {0} not found", metadataIdentity.Id);
        }
    }
}
