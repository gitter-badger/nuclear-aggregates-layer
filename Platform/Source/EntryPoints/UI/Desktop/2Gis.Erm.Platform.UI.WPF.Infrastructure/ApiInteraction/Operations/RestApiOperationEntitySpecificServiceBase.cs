using System;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Operations
{
    public abstract class RestApiOperationEntitySpecificServiceBase<TEntity> : RestApiOperationServiceBase
        where TEntity : class, IEntityKey
    {
        private readonly Type _entityType;
        private readonly EntityName _entityName;

        protected RestApiOperationEntitySpecificServiceBase(IApiClient apiClient, ITracer logger, string operationApiTargetResource)
            : base(apiClient, logger, operationApiTargetResource)
        {
            _entityType = typeof(TEntity);
            _entityName = EntityType.AsEntityName();
        }

        protected Type EntityType
        {
            get
            {
                return _entityType;
            }
        }

        protected EntityName EntityName
        {
            get
            {
                return _entityName;
            }
        }
    }
}