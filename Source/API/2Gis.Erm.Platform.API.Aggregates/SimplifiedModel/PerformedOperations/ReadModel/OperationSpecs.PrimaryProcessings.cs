using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.ReadModel
{
    public static partial class OperationSpecs
    {
        public static class PrimaryProcessings
        {
            public static class Find
            {
                public static FindSpecification<PerformedOperationPrimaryProcessing> ByFlowIds(IEnumerable<Guid> flowIds)
                {
                    return new FindSpecification<PerformedOperationPrimaryProcessing>(o => flowIds.Contains(o.MessageFlowId));
                }
            }
        }
    }
}
