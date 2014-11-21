using System;

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
                public static FindSpecification<PerformedOperationPrimaryProcessing> ByFlowId(Guid flowId)
                {
                    return new FindSpecification<PerformedOperationPrimaryProcessing>(o => flowId == o.MessageFlowId);
                }
            }
        }
    }
}
