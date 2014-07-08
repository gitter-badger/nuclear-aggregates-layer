using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.ReadModel
{
    public static class PerformedOperationsProcessingSpecs
    {
        public static class Final
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
            }
        }
    }
}
