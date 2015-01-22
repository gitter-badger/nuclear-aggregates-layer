using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.ActionsHistory
{
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.ActionsHistory201303)]
    public class ActionsHistoryOperationErrorDescription : IBasicOperationErrorDescription
    {
        public ActionsHistoryOperationErrorDescription(IEntityType entityName, string message)
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