using System;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.ReadModel
{
    public static partial class OperationSpecs
    {
        public static class Performed
        {
            public static class Find
            {
                private static readonly Guid DefaultUseCaseId = new Guid("00000000-0000-0000-0000-000000000000");
                
                public static FindSpecification<PerformedBusinessOperation> OnlyRoot
                {
                    get
                    {
                        return new FindSpecification<PerformedBusinessOperation>(o => o.Parent == null);
                    }
                }

                public static FindSpecification<PerformedBusinessOperation> AfterDate(DateTime date)
                {
                    return new FindSpecification<PerformedBusinessOperation>(o => o.UseCaseId != DefaultUseCaseId &&
                                                                                  o.Date > date);
                }
            }
        }
    }
}
