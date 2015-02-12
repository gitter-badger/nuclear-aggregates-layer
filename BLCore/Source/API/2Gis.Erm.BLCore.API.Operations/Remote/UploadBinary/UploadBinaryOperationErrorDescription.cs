using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.UploadBinary
{
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.UploadBinary201307)]
    public class UploadBinaryOperationErrorDescription : IBasicOperationErrorDescription
    {
        public UploadBinaryOperationErrorDescription(IEntityType entityName, string message)
        {
            EntityName = entityName.Description;
            Message = message;
        }

        [DataMember]
        public string EntityName { get; private set; }
        [DataMember]
        public string Message { get; private set; }
    }
}