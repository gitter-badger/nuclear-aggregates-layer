using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export
{
    public interface IOperationsExportService : IOperation<ExportIdentity>
    {
        IEnumerable<PerformedBusinessOperation> GetPendingOperations(int maxOperationCount);
        void ExportOperations(FlowDescription flowDescription, IEnumerable<PerformedBusinessOperation> operations, int packageSize);
        void ExportFailedEntities(FlowDescription flowDescription, int packageSize);
    }

    public interface IGenericOperationsExportService<TEntity, TProcessedOperationEntity> : IEntityOperation<TEntity, TProcessedOperationEntity>, IOperationsExportService
        where TEntity : class, IEntityKey
        where TProcessedOperationEntity : class, IEntityKey
    {
    }
}