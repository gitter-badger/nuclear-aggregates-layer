using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.DownloadBinary
{
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.DownloadBinary201307)]
    public class DownloadBinaryOperationErrorDescription : IBasicOperationErrorDescription
    {
        public DownloadBinaryOperationErrorDescription(IEntityType entityName, string message)
        {
            EntityName = entityName;
            Message = message;
        }

        [DataMember]
        public IEntityType EntityName { get; private set; }
        [DataMember]
        public string Message { get; private set; }
    }
}