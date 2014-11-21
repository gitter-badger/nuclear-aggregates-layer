using System.Runtime.Serialization;
using System.Text;

namespace DoubleGis.Erm.Platform.API.Metadata
{
    [DataContract]
    public class MetadataOperationErrorDescription
    {
        public MetadataOperationErrorDescription(string message, string description)
        {
            Message = message;
            Description = description;
        }

        [DataMember]
        public string Message { get; private set; }
        [DataMember]
        public string Description { get; private set; }

        public override string ToString()
        {
            return new StringBuilder()
                .AppendLine("Message: " + Message)
                .AppendLine("Description: " + Description)
                .ToString();
        }
    }
}
