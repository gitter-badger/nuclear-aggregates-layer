using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.ActionHistory
{
    public interface IActionsHistoryService : IOperation<ActionHistoryIdentity>
    {
        ActionsHistoryDto GetActionHistory(IEntityType entityName, long entityId);
    }

    [DataContract(Namespace = ServiceNamespaces.BasicOperations.ActionsHistory201303)]
    public class ActionsHistoryDto
    {
        [DataMember]
        public IEnumerable<ActionsHistoryItemDto> ActionHistoryData { get; set; }

        [DataMember]
        public IEnumerable<ActionsHistoryDetailDto> ActionHistoryDetailsData { get; set; }

        [DataContract(Namespace = ServiceNamespaces.BasicOperations.ActionsHistory201303)]
        public class ActionsHistoryItemDto
        {
            [DataMember]
            public long Id { get; set; }
            [DataMember]
            public string ActionType { get; set; }
            [DataMember]
            public string CreatedBy { get; set; }
            [DataMember]
            public DateTime CreatedOn { get; set; }
        }

        [DataContract(Namespace = ServiceNamespaces.BasicOperations.ActionsHistory201303)]
        public class ActionsHistoryDetailDto
        {
            [DataMember]
            public long Id { get; set; }
            [DataMember]
            public long ActionsHistoryId { get; set; }
            [DataMember]
            public string PropertyName { get; set; }
            [DataMember]
            public string OriginalValue { get; set; }
            [DataMember]
            public string ModifiedValue { get; set; }
        }
    }
}