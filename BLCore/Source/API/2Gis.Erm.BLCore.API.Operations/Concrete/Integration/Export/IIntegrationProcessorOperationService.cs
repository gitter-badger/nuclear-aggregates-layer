using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export
{
    public interface IIntegrationProcessorOperationService : IOperation<ExportIdentity>
    {
        IEnumerable<PerformedBusinessOperation> GetPendingOperations(int maxOperationCount);
        void ExportOperations(FlowDescription flowDescription, IEnumerable<PerformedBusinessOperation> operations, int packageSize);
        void ExportFailedEntities(FlowDescription flowDescription, int packageSize);
    }

    public interface IGenericIntegrationProcessorOperationService<TEntity, TProcessedOperationEntity> : IEntityOperation<TEntity, TProcessedOperationEntity>, IIntegrationProcessorOperationService
        where TEntity : class, IEntity, IEntityKey
        where TProcessedOperationEntity : class, IEntity, IEntityKey
    {
    }
}