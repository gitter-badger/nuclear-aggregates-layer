using System;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.ReadModel
{
    public static partial class OperationSpecs
    {
        public static class Performed
        {
            public static class Find
            {
                private static readonly Guid DefaultUseCaseId = new Guid("00000000-0000-0000-0000-000000000000");
                
                public static FindSpecification<PerformedBusinessOperation> AfterDate(DateTime date)
                {
                    return new FindSpecification<PerformedBusinessOperation>(o => o.UseCaseId != DefaultUseCaseId &&
                                                                                  o.Date > date);
                }

                public static FindSpecification<PerformedBusinessOperation> InUseCase(Guid useCaseId)
                {
                    return new FindSpecification<PerformedBusinessOperation>(o => o.UseCaseId == useCaseId);
                }

                public static FindSpecification<PerformedBusinessOperation> Specific<TOperationIdentity, TEntity>()
                    where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, IEntitySpecificOperationIdentity, new()
                    where TEntity : class, IEntity
                {
                    var identity = new TOperationIdentity().SpecificFor<TEntity>();
                    var descriptor = identity.Entities.EvaluateHash();
                    var operationId = identity.OperationIdentity.Id;
                    return new FindSpecification<PerformedBusinessOperation>(
                        o => o.Operation == operationId && o.Descriptor == descriptor);
                }
            }
        }
    }
}
