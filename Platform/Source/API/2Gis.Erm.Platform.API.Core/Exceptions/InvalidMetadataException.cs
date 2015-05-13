using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;

namespace DoubleGis.Erm.Platform.API.Core.Exceptions
{
    [Serializable]
    public class InvalidMetadataException : BusinessLogicException
    {
        public InvalidMetadataException(IMetadataElementIdentity metadataIdentity) :
            base(GenerateMessage(metadataIdentity))
        {
        }

        protected InvalidMetadataException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }

        private static string GenerateMessage(IMetadataElementIdentity metadataIdentity)
        {
            return string.Format("Invalid metadata for {0}", metadataIdentity.Id);
        }
    }
}
