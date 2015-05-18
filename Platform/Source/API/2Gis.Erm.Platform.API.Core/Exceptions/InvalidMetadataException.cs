using System;
using System.Runtime.Serialization;

using NuClear.Metamodeling.Elements.Identities;

namespace DoubleGis.Erm.Platform.API.Core.Exceptions
{
    [Serializable]
    public class InvalidMetadataException : BusinessLogicException
    {
        public InvalidMetadataException(IMetadataElementIdentity metadataIdentity, string comment) :
            base(GenerateMessage(metadataIdentity, comment))
        {
        }

        protected InvalidMetadataException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }

        private static string GenerateMessage(IMetadataElementIdentity metadataIdentity, string comment)
        {
            return string.Format("Invalid metadata in {0}. Comment: {1}", metadataIdentity.Id, comment);
        }
    }
}
