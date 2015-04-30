using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Simplified
{
    public class OperationSpecifications
    {
        public static class Find
        {
            public static FindSpecification<Operation> ById(long id)
            {
                return new FindSpecification<Operation>(x => x.Id == id);
            }

            public static FindSpecification<OperationType> OperationTypeById(long id)
            {
                return new FindSpecification<OperationType>(x => x.Id == id);
            }
        }
    }
}