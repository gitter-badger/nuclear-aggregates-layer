using System;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.ReadModel
{
    public static partial class OperationSpecs
    {
        public static class FinalProcessings
        {
            public static class Find
            {
                public static FindSpecification<PerformedOperationFinalProcessing> Initial
                {
                    get { return new FindSpecification<PerformedOperationFinalProcessing>(p => p.AttemptCount == 0); }
                }

                public static FindSpecification<PerformedOperationFinalProcessing> Failed
                {
                    get { return new FindSpecification<PerformedOperationFinalProcessing>(p => p.AttemptCount > 0); }
                }

                public static FindSpecification<PerformedOperationFinalProcessing> ByFlowId(Guid flowId)
                {
                    return new FindSpecification<PerformedOperationFinalProcessing>(p => p.MessageFlowId == flowId);
                }
            }
        }
    }
}
