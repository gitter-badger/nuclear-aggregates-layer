using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.UploadBinary
{
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.UploadBinary201307)]
    public class UploadBinaryOperationErrorDescription : IBasicOperationErrorDescription
    {
        public UploadBinaryOperationErrorDescription(EntityName entityName, string message)
        {
            EntityName = entityName;
            Message = message;
        }

        [DataMember]
        public EntityName EntityName { get; private set; }
        [DataMember]
        public string Message { get; private set; }
    }
}