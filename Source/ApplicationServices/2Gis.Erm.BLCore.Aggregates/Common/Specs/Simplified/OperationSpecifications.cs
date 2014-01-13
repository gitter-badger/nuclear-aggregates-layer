using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Specs.Simplified
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
