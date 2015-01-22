using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Entities.Aspects.Integration;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.ServiceBus
{
    public sealed class SerializeObjectsRequest<TEntity, TProcessedOperationEntity> : Request
        where TEntity : class, IEntity, IEntityKey
        where TProcessedOperationEntity : class, IIntegrationProcessorState
    {
        public bool IsRecovery { get; private set; }
        public string SchemaName { get; private set; }
        public IEnumerable<PerformedBusinessOperation> Operations { get; private set; }
        public IEnumerable<ExportFailedEntity> FailedEntities { get; private set; }

        public static SerializeObjectsRequest<TEntity, TProcessedOperationEntity> Create(string schemaName, IEnumerable<ExportFailedEntity> failedEntities)
        {
            return new SerializeObjectsRequest<TEntity, TProcessedOperationEntity>
                {
                    IsRecovery = true,
                    SchemaName = schemaName,
                    FailedEntities = failedEntities
                };
        }

        public static SerializeObjectsRequest<TEntity, TProcessedOperationEntity> Create(string schemaName, IEnumerable<PerformedBusinessOperation> operations)
        {
            return new SerializeObjectsRequest<TEntity, TProcessedOperationEntity>
            {
                IsRecovery = false,
                SchemaName = schemaName,
                Operations = operations,
            };
        }
    }
}
